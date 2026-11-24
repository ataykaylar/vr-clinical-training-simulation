using UnityEngine;
using UnityEngine.XR;

public class ControllerIndexFingerCurl : MonoBehaviour
{
    [SerializeField] private XRNode controllerNode = XRNode.RightHand;

    [Header("Isaret Parmagi Kemikleri")]
    [SerializeField] private Transform joint1;
    [SerializeField] private Transform joint2;
    [SerializeField] private Transform joint3;

    [Header("X Ekseninde Bukulme Acilari")]
    [SerializeField]
    private Vector3 curlAngles =
        new Vector3(35f, 65f, 45f);

    [SerializeField] private float animationSpeed = 8f;

    private Quaternion startRotation1;
    private Quaternion startRotation2;
    private Quaternion startRotation3;

    private InputDevice controller;
    private float curl;
    private bool initialized;

    private void Awake()
    {
        if (joint1 == null || joint2 == null || joint3 == null)
        {
            Debug.LogError(
                "Isaret parmaginin uc kemigini atamalisin.",
                this
            );

            enabled = false;
            return;
        }

        startRotation1 = joint1.localRotation;
        startRotation2 = joint2.localRotation;
        startRotation3 = joint3.localRotation;

        initialized = true;
    }

    private void Update()
    {
        if (!controller.isValid)
            controller = InputDevices.GetDeviceAtXRNode(controllerNode);

        float triggerValue = 0f;

        if (controller.isValid &&
            controller.TryGetFeatureValue(
                CommonUsages.trigger,
                out float value))
        {
            triggerValue = Mathf.Clamp01(value);
        }

        curl = Mathf.MoveTowards(
            curl,
            triggerValue,
            animationSpeed * Time.unscaledDeltaTime
        );

        joint1.localRotation =
            startRotation1 *
            Quaternion.AngleAxis(curlAngles.x * curl, Vector3.right);

        joint2.localRotation =
            startRotation2 *
            Quaternion.AngleAxis(curlAngles.y * curl, Vector3.right);

        joint3.localRotation =
            startRotation3 *
            Quaternion.AngleAxis(curlAngles.z * curl, Vector3.right);
    }

    private void OnDisable()
    {
        if (!initialized)
            return;

        if (joint1 != null)
            joint1.localRotation = startRotation1;

        if (joint2 != null)
            joint2.localRotation = startRotation2;

        if (joint3 != null)
            joint3.localRotation = startRotation3;

        curl = 0f;
    }
}