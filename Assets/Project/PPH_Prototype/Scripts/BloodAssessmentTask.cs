using TMPro;
using UnityEngine;

public class BloodAssessmentTask : MonoBehaviour
{
    [SerializeField]
    private TMP_Text feedbackText;

    [SerializeField]
    private TaskMetricTracker metricTracker;

    private void Awake()
    {
        if (metricTracker == null)
        {
            metricTracker =
                GetComponent<TaskMetricTracker>();
        }
    }

    private void Start()
    {
        if (feedbackText != null)
            feedbackText.text = "";

        if (metricTracker == null)
        {
            Debug.LogError(
                "BloodAssessmentTask için " +
                "TaskMetricTracker bulunamadı."
            );
        }
    }

    public void SelectSevere()
    {
        EvaluateAnswer("Şiddetli", true);
    }

    public void SelectModerate()
    {
        EvaluateAnswer(
            "Orta şiddetli",
            false
        );
    }

    public void SelectMild()
    {
        EvaluateAnswer("Hafif", false);
    }

    private void EvaluateAnswer(
        string selectedAnswer,
        bool isCorrect)
    {
        if (metricTracker == null ||
            metricTracker.IsCompleted)
        {
            return;
        }

        if (feedbackText != null)
        {
            feedbackText.text = isCorrect
                ? "Kanama miktarı doğru değerlendirildi."
                : "Tekrar değerlendiriniz.";
        }

        metricTracker.RecordSpecificMetric(
            "SelectedBleedingLevel",
                selectedAnswer
         );

        metricTracker.RegisterAttempt(
            selectedAnswer,
            isCorrect,
            isCorrect ? "" : "Yanlış seçim"
        );
    }
}