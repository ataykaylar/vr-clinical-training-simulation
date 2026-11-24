using UnityEngine;

[DefaultExecutionOrder(100)]
public class ControllerHandVisuals : MonoBehaviour
{
    [Header("Sol El")]
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform leftHandRoot;
    [SerializeField] private Vector3 leftPositionOffset;
    [SerializeField] private Vector3 leftRotationOffset;

    [Header("Sag El")]
    [SerializeField] private Transform rightController;
    [SerializeField] private Transform rightHandRoot;
    [SerializeField] private Vector3 rightPositionOffset;
    [SerializeField] private Vector3 rightRotationOffset;

    private Quaternion leftModelRotation;
    private Quaternion rightModelRotation;

    private void Awake()
    {
        if (leftController == null ||
            rightController == null ||
            leftHandRoot == null ||
            rightHandRoot == null)
        {
            Debug.LogError(
                "ControllerHandVisuals: El ve kontrolcu alanlarini doldur.",
                this
            );

            enabled = false;
            return;
        }

        // Her kemigin model icindeki baslangic yonunu koru.
        Quaternion inverseModelRotation =
            Quaternion.Inverse(transform.rotation);

        leftModelRotation =
            inverseModelRotation * leftHandRoot.rotation;

        rightModelRotation =
            inverseModelRotation * rightHandRoot.rotation;
    }

    private void OnEnable()
    {
        Application.onBeforeRender += UpdateHands;
    }

    private void OnDisable()
    {
        Application.onBeforeRender -= UpdateHands;
    }

    private void LateUpdate()
    {
        UpdateHands();
    }

    [BeforeRenderOrder(100)]
    private void UpdateHands()
    {
        FollowController(
            leftController,
            leftHandRoot,
            leftPositionOffset,
            leftRotationOffset,
            leftModelRotation
        );

        FollowController(
            rightController,
            rightHandRoot,
            rightPositionOffset,
            rightRotationOffset,
            rightModelRotation
        );
    }

    private static void FollowController(
        Transform controller,
        Transform handRoot,
        Vector3 positionOffset,
        Vector3 rotationOffset,
        Quaternion modelRotation)
    {
        if (controller == null || handRoot == null)
            return;

        Vector3 position =
            controller.TransformPoint(positionOffset);

        Quaternion rotation =
            controller.rotation *
            Quaternion.Euler(rotationOffset) *
            modelRotation;

        handRoot.SetPositionAndRotation(position, rotation);
    }
}