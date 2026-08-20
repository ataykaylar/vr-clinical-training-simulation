using TMPro;
using UnityEngine;

public class ClearInputOnStart : MonoBehaviour
{
    private TMP_InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    private void Start()
    {
        inputField.SetTextWithoutNotify("");
        inputField.ForceLabelUpdate();
    }
}