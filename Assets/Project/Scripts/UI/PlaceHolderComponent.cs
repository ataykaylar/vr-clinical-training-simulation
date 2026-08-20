using TMPro;
using UnityEngine;

public class PlaceholderController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text placeholderText;

    private void Awake()
    {
        inputField.SetTextWithoutNotify("");
        inputField.ForceLabelUpdate();
    }

    private void LateUpdate()
    {
        bool shouldShow =
            !inputField.isFocused &&
            string.IsNullOrWhiteSpace(inputField.text);

        placeholderText.enabled = shouldShow;
    }
}