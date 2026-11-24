using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(300)]
public class FundusContactUI : MonoBehaviour
{
    [Header("Connections")]
    [SerializeField] private FundusContactTask contactTask;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Image progressFill;

    [Header("Colors")]
    [SerializeField] private Color waitingColor = Color.white;

    [SerializeField]
    private Color contactColor =
        new Color32(54, 214, 197, 255);

    [SerializeField]
    private Color interruptedColor =
        new Color32(255, 200, 100, 255);

    [SerializeField]
    private Color completedColor =
        new Color32(100, 230, 150, 255);

    private bool wasContactActive;
    private bool contactInterrupted;

    private void Awake()
    {
        if (contactTask == null ||
            statusText == null ||
            progressSlider == null)
        {
            Debug.LogError(
                "FundusContactUI: Inspector bağlantıları eksik.",
                this
            );

            enabled = false;
            return;
        }

        progressSlider.minValue = 0f;
        progressSlider.maxValue = 1f;
        progressSlider.wholeNumbers = false;
        progressSlider.interactable = false;

        if (progressFill == null &&
            progressSlider.fillRect != null)
        {
            progressFill =
                progressSlider.fillRect.GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        wasContactActive = false;
        contactInterrupted = false;

        ShowStatus(
            "El teması bekleniyor",
            waitingColor,
            0f
        );
    }

    private void LateUpdate()
    {
        if (contactTask == null)
            return;

        if (contactTask.IsTaskCompleted)
        {
            ShowStatus(
                "Temas tamamlandı",
                completedColor,
                1f
            );

            wasContactActive = false;
            return;
        }

        if (contactTask.IsContactActive)
        {
            wasContactActive = true;
            contactInterrupted = false;

            if (contactTask.IsTouchingTarget)
            {
                ShowStatus(
                    "Temas algılandı. Elini burada tut.",
                    contactColor,
                    contactTask.ContactProgress
                );
            }
            else
            {
                ShowStatus(
                    "Temas kesiliyor. Elini bölgeye geri getir.",
                    interruptedColor,
                    contactTask.ContactProgress
                );
            }

            return;
        }

        if (wasContactActive)
            contactInterrupted = true;

        wasContactActive = false;

        ShowStatus(
            contactInterrupted
                ? "Temas kesildi. Tekrar dene."
                : "El teması bekleniyor",
            contactInterrupted
                ? interruptedColor
                : waitingColor,
            0f
        );
    }

    private void ShowStatus(
        string message,
        Color color,
        float progress)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
        }

        if (progressSlider != null)
            progressSlider.SetValueWithoutNotify(progress);

        if (progressFill != null)
            progressFill.color = color;
    }
}