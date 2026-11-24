using TMPro;
using UnityEngine;

public class CallForHelpTask : MonoBehaviour
{
    [SerializeField]
    private TMP_Text feedbackText;

    [SerializeField]
    private TaskMetricTracker metricTracker;

    [SerializeField]
    private string helpMethod =
        "Yardım çağrı butonu";

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
        {
            feedbackText.text = "";
        }
    }

    public void CallForHelp()
    {
        if (metricTracker == null)
        {
            Debug.LogWarning(
                "Task 6 Metric Tracker bulunamadı."
            );
            return;
        }

        if (metricTracker.IsCompleted)
            return;

        metricTracker.RegisterFirstAction();

        metricTracker.RecordSpecificMetric(
            "HelpMethod",
            helpMethod
        );

        if (feedbackText != null)
        {
            feedbackText.text =
                "Yardım başarıyla çağrıldı.";
        }

        metricTracker.RegisterAttempt(
            helpMethod,
            true
        );
    }
}