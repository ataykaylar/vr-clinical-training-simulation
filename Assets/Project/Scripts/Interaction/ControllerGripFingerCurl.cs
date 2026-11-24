using UnityEngine;
using UnityEngine.XR;

public class ControllerGripFingerCurl : MonoBehaviour
{
    [SerializeField] private XRNode controllerNode = XRNode.RightHand;
    [SerializeField] private Transform handRoot;

    [Header("Her Parmagin Uc Eklemindeki X Bukulmesi")]
    [SerializeField]
    private Vector3 curlAngles =
        new Vector3(40f, 65f, 45f);

    [SerializeField] private float animationSpeed = 8f;

    private readonly Transform[] joints = new Transform[9];
    private readonly Quaternion[] startRotations = new Quaternion[9];

    private readonly string[] fingerNames =
    {
        "Middle",
        "Ring",
        "Pinky"
    };

    private InputDevice controller;
    private float curl;
    private bool initialized;

    private void Awake()
    {
        if (handRoot == null)
        {
            Debug.LogError("Hand Root alanini doldur.", this);
            enabled = false;
            return;
        }

        Transform[] bones =
            handRoot.GetComponentsInChildren<Transform>(true);

        for (int finger = 0; finger < fingerNames.Length; finger++)
        {
            for (int joint = 0; joint < 3; joint++)
            {
                int index = finger * 3 + joint;

                string boneName =
                    handRoot.name +
                    fingerNames[finger] +
                    (joint + 1);

                foreach (Transform bone in bones)
                {
                    if (bone.name == boneName)
                    {
                        joints[index] = bone;
                        break;
                    }
                }

                if (joints[index] == null)
                {
                    Debug.LogError(
                        "Parmak kemigi bulunamadi: " + boneName,
                        this
                    );

                    enabled = false;
                    return;
                }

                startRotations[index] =
                    joints[index].localRotation;
            }
        }

        initialized = true;
    }

    private void Update()
    {
        if (!controller.isValid)
            controller = InputDevices.GetDeviceAtXRNode(controllerNode);

        float gripValue = 0f;

        if (controller.isValid &&
            controller.TryGetFeatureValue(
                CommonUsages.grip,
                out float value))
        {
            gripValue = Mathf.Clamp01(value);
        }

        curl = Mathf.MoveTowards(
            curl,
            gripValue,
            animationSpeed * Time.unscaledDeltaTime
        );

        for (int i = 0; i < joints.Length; i++)
        {
            float angle = curlAngles[i % 3] * curl;

            joints[i].localRotation =
                startRotations[i] *
                Quaternion.AngleAxis(angle, Vector3.right);
        }
    }

    private void OnDisable()
    {
        if (!initialized)
            return;

        for (int i = 0; i < joints.Length; i++)
        {
            if (joints[i] != null)
                joints[i].localRotation = startRotations[i];
        }

        curl = 0f;
    }
}