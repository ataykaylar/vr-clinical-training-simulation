using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(200)]
public class FundusContactTask : MonoBehaviour
{
    [Header("Task")]
    [SerializeField] private TaskMetricTracker metricTracker;

    [SerializeField, Min(0.1f)]
    private float requiredContactDuration = 2f;

    [Header("Avuc Temas Noktalari")]
    [SerializeField] private Transform leftPalmContact;
    [SerializeField] private Transform rightPalmContact;

    [SerializeField, Min(0.001f)]
    private float contactRadius = 0.025f;

    [SerializeField, Min(0f)]
    private float contactBreakTolerance = 0.1f;

    [Header("Istege Bagli Mouse Testi")]
    [SerializeField] private bool enableMouseTesting = false;
    [SerializeField] private Camera interactionCamera;

    private Collider targetCollider;

    private bool contactActive;
    private bool mouseContactActive;
    private bool touchingPreviousFrame;

    private float contactDuration;
    private float lastContactTime;
    private string contactInputMethod;



    public bool IsContactActive =>
    isActiveAndEnabled && contactActive;

    public bool IsTouchingTarget =>
        IsContactActive && touchingPreviousFrame;

    public bool IsTaskCompleted =>
        metricTracker != null && metricTracker.IsCompleted;

    public float ContactProgress =>
        IsTaskCompleted
            ? 1f
            : Mathf.Clamp01(
                contactDuration /
                Mathf.Max(requiredContactDuration, 0.1f)
            );
    private void Awake()
    {
        if (metricTracker == null)
            metricTracker = GetComponentInParent<TaskMetricTracker>();

        if (interactionCamera == null)
            interactionCamera = Camera.main;

        targetCollider = GetComponent<Collider>();

        if (metricTracker == null || targetCollider == null)
        {
            Debug.LogError(
                "FundusContactTask: Metric Tracker veya Collider eksik.",
                this
            );

            enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (metricTracker == null || metricTracker.IsCompleted)
        {
            ResetContactState();
            return;
        }

        HandleMouseInput();

        bool handTouching = HasHandContact();
        bool touching = handTouching || mouseContactActive;

        if (touching)
        {
            if (!contactActive)
            {
                BeginContact();
                return;
            }

            // Yalnizca temas bulunan araliklari say.
            if (touchingPreviousFrame)
                contactDuration += Time.deltaTime;

            touchingPreviousFrame = true;
            lastContactTime = Time.time;

            if (contactDuration >= requiredContactDuration)
                FinishContact(true);
        }
        else
        {
            touchingPreviousFrame = false;

            if (contactActive &&
                Time.time - lastContactTime >= contactBreakTolerance)
            {
                EndContact();
            }
        }
    }

    private bool HasHandContact()
    {
        if (targetCollider == null ||
            !targetCollider.enabled ||
            !targetCollider.gameObject.activeInHierarchy)
        {
            return false;
        }

        return IsPalmTouching(leftPalmContact) ||
               IsPalmTouching(rightPalmContact);
    }

    private bool IsPalmTouching(Transform palm)
    {
        if (palm == null || !palm.gameObject.activeInHierarchy)
            return false;

        Vector3 closestPoint =
            targetCollider.ClosestPoint(palm.position);

        return (palm.position - closestPoint).sqrMagnitude <=
               contactRadius * contactRadius;
    }

    public void BeginContact()
    {
        if (!isActiveAndEnabled ||
            metricTracker == null ||
            metricTracker.IsCompleted ||
            contactActive)
        {
            return;
        }

        bool handTouching = HasHandContact();

        // Eski isin event'i tek basina temas baslatamasin.
        if (!handTouching && !mouseContactActive)
            return;

        contactActive = true;
        contactDuration = 0f;
        lastContactTime = Time.time;
        touchingPreviousFrame = true;

        contactInputMethod = handTouching
            ? "PalmProximity"
            : "MouseTest";

        metricTracker.RegisterFirstAction();

        Debug.Log("Task 7 | Fundus temasi basladi.");
    }

    public void EndContact()
    {
        if (HasHandContact() || mouseContactActive)
            return;

        FinishContact(false);
    }

    private void FinishContact(bool successful)
    {
        if (!contactActive)
            return;

        float duration = contactDuration;
        string inputMethod = contactInputMethod;

        ResetContactState();

        if (metricTracker == null || metricTracker.IsCompleted)
            return;

        metricTracker.RecordSpecificMetric(
            "ContactRegion",
            "Fundus"
        );

        metricTracker.RecordSpecificMetric(
            "ContactDurationSeconds",
            duration.ToString("F2", CultureInfo.InvariantCulture),
            "sn"
        );

        metricTracker.RecordSpecificMetric(
            "ContactInputMethod",
            inputMethod
        );

        metricTracker.RegisterAttempt(
            "FundusTargetZone",
            successful,
            successful ? "" : "Yetersiz temas süresi"
        );

        Debug.Log(
            $"Task 7 | {(successful ? "Temas tamamlandi" : "Temas erken sonlandi")}: " +
            $"{duration:F2} sn"
        );
    }

    private void HandleMouseInput()
    {
        if (!enableMouseTesting ||
            Mouse.current == null ||
            interactionCamera == null ||
            targetCollider == null ||
            !targetCollider.enabled ||
            !Application.isFocused)
        {
            mouseContactActive = false;
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = interactionCamera.ScreenPointToRay(
                Mouse.current.position.ReadValue()
            );

            mouseContactActive =
                Physics.Raycast(
                    ray,
                    out RaycastHit hit,
                    100f,
                    Physics.DefaultRaycastLayers,
                    QueryTriggerInteraction.Collide
                ) &&
                hit.collider == targetCollider;
        }

        if (!Mouse.current.leftButton.isPressed)
            mouseContactActive = false;
    }

    private void OnDisable()
    {
        // Obje kapatilirken yanlis deneme kaydetme.
        ResetContactState();
    }

    private void ResetContactState()
    {
        contactActive = false;
        mouseContactActive = false;
        touchingPreviousFrame = false;
        contactDuration = 0f;
        contactInputMethod = "";
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (leftPalmContact != null)
            Gizmos.DrawWireSphere(leftPalmContact.position, contactRadius);

        if (rightPalmContact != null)
            Gizmos.DrawWireSphere(rightPalmContact.position, contactRadius);
    }
}