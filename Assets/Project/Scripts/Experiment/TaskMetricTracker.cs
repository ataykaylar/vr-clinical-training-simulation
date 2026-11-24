using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TaskMetricTracker : MonoBehaviour
{
    [Header("Task Information")]
    [SerializeField] private int taskId;
    [SerializeField] private string taskName;

    [Header("Scenario Information")]
    [SerializeField, Min(1)]
    private int expectedTaskCount = 1;

    [SerializeField]
    private bool autoCompleteWhenCorrect = true;

    [SerializeField]
    private bool logSummaryAfterSave = true;

    [Header("Task Flow")]
    [SerializeField]
    private UnityEvent onTaskCompleted;

    private float taskStartTime;
    private string taskStartedAtUtc;
    private string firstActionAtUtc = "";

    private bool taskStarted;
    private bool taskTestMode;
    private int taskExperimentId;

    private bool firstActionRecorded;
    private bool firstAttemptRecorded;
    private bool firstAttemptCorrect;

    private bool taskCompleted;
    private bool resultSaved;
    private bool technicalFlag;

    private int attemptCount;
    private int irrelevantActionCount;
    private int manualScore = -1;

    private readonly List<string> actionHistory =
        new List<string>();

    private readonly List<string> errorHistory =
        new List<string>();

    public bool IsCompleted => taskCompleted;
    public int AttemptCount => attemptCount;

    public int FirstAttemptScore =>
        firstAttemptRecorded && firstAttemptCorrect ? 1 : 0;

    private bool CanTrackTask =>
        taskStarted &&
        !taskCompleted &&
        ScenarioStartController.IsScenarioRunning &&
        taskTestMode == ScenarioStartController.IsTestMode &&
        (
            taskTestMode ||
            taskExperimentId ==
                ScenarioStartController.ActiveExperimentId
        );

    private void Awake()
    {
        // Task 5, Başlat düğmesinden başlatılacak.
        // Sonraki görevler etkinleştirildiklerinde başlayacak.
        if (ScenarioStartController.IsScenarioRunning)
            BeginTask();
    }

    public void BeginTask()
    {
        if (!ScenarioStartController.IsScenarioRunning)
        {
            Debug.LogWarning(
                $"Task {taskId}: Önce senaryoyu Başlat düğmesiyle başlat.",
                this
            );

            return;
        }

        taskTestMode = ScenarioStartController.IsTestMode;
        taskExperimentId =
            ScenarioStartController.ActiveExperimentId;

        if (!taskTestMode && taskExperimentId <= 0)
        {
            Debug.LogError(
                $"Task {taskId}: Geçerli deney kaydı bulunamadı.",
                this
            );

            return;
        }

        taskStarted = true;

        taskStartTime = Time.time;
        taskStartedAtUtc = DateTime.UtcNow.ToString("o");
        firstActionAtUtc = "";

        firstActionRecorded = false;
        firstAttemptRecorded = false;
        firstAttemptCorrect = false;

        taskCompleted = false;
        resultSaved = false;
        technicalFlag = false;

        attemptCount = 0;
        irrelevantActionCount = 0;
        manualScore = -1;

        actionHistory.Clear();
        errorHistory.Clear();

        Debug.Log(
            $"{(taskTestMode ? "[TEST MODU] " : "")}" +
            $"Task {taskId} başladı: {taskName}"
        );
    }

    public void RegisterFirstAction()
    {
        if (!CanTrackTask || firstActionRecorded)
            return;

        firstActionRecorded = true;
        firstActionAtUtc = DateTime.UtcNow.ToString("o");

        Debug.Log(
            $"Task {taskId} | İlk ilgili işlem kaydedildi."
        );
    }

    public void RegisterAttempt(
        string selectedAnswerOrObject,
        bool isCorrect,
        string errorType = "")
    {
        if (!CanTrackTask)
            return;

        if (!firstActionRecorded)
            RegisterFirstAction();

        attemptCount++;

        if (!string.IsNullOrWhiteSpace(selectedAnswerOrObject))
            actionHistory.Add(selectedAnswerOrObject);

        if (!firstAttemptRecorded)
        {
            firstAttemptRecorded = true;
            firstAttemptCorrect = isCorrect;
        }

        if (!isCorrect &&
            !string.IsNullOrWhiteSpace(errorType) &&
            !errorHistory.Contains(errorType))
        {
            errorHistory.Add(errorType);
        }

        float elapsedTime = Time.time - taskStartTime;

        Debug.Log(
            $"Task {taskId} | " +
            $"Seçim: {selectedAnswerOrObject} | " +
            $"Doğru: {(isCorrect ? 1 : 0)} | " +
            $"İlk cevap: {FirstAttemptScore} | " +
            $"Deneme: {attemptCount} | " +
            $"Süre: {elapsedTime:F2} sn"
        );

        if (isCorrect && autoCompleteWhenCorrect)
            CompleteTask(true);
    }

    public void AddIrrelevantAction(
        string actionDescription = "")
    {
        if (!CanTrackTask)
            return;

        irrelevantActionCount++;

        Debug.Log(
            $"Task {taskId} | Alakasız işlem: " +
            $"{actionDescription} | Toplam: {irrelevantActionCount}"
        );
    }

    public void MarkTechnicalIssue(
        string issueDescription = "")
    {
        if (!CanTrackTask)
            return;

        technicalFlag = true;

        Debug.LogWarning(
            $"Task {taskId} | Teknik sorun: {issueDescription}"
        );
    }

    public void SetManualScore(int score)
    {
        if (!CanTrackTask)
            return;

        manualScore = Mathf.Clamp(score, 0, 1);
    }

    public void RecordSpecificMetric(
        string metricName,
        string metricValue,
        string unit = "")
    {
        if (!CanTrackTask || resultSaved)
            return;

        if (string.IsNullOrWhiteSpace(metricName))
        {
            Debug.LogWarning(
                $"Task {taskId} | Metrik adı boş bırakılamaz."
            );

            return;
        }

        if (taskTestMode)
        {
            Debug.Log(
                $"[TEST METRİĞİ — KAYDEDİLMEDİ] " +
                $"Task: {taskId} | {metricName}: " +
                $"{metricValue} {unit}"
            );

            return;
        }

        if (taskExperimentId <= 0)
        {
            Debug.LogError(
                $"Task {taskId} | Geçersiz deney ID; kayıt yapılmadı."
            );

            return;
        }

        TaskSpecificMetricRecord metric =
            new TaskSpecificMetricRecord
            {
                ExperimentId = taskExperimentId,
                TaskId = taskId,
                MetricName = metricName,
                MetricValue = metricValue,
                Unit = unit,
                RecordedAtUtc = DateTime.UtcNow.ToString("o")
            };

        ExperimentDatabase.SaveTaskSpecificMetric(metric);
    }

    public void CompleteTask(bool finalCorrect)
    {
        if (!CanTrackTask || resultSaved)
            return;

        float duration = Time.time - taskStartTime;

        int finalScore = manualScore >= 0
            ? manualScore
            : FirstAttemptScore;

        if (taskTestMode)
        {
            taskCompleted = true;

            Debug.Log(
                $"[TEST TAMAMLANDI — KAYDEDİLMEDİ] " +
                $"Task: {taskId} | Doğru: {finalCorrect} | " +
                $"Puan: {finalScore} | Deneme: {attemptCount} | " +
                $"Süre: {duration:F2} sn"
            );

            // Testte de sıradaki göreve geç.
            onTaskCompleted?.Invoke();
            return;
        }

        if (taskExperimentId <= 0)
        {
            Debug.LogError(
                $"Task {taskId} | Geçersiz deney ID; kayıt yapılmadı."
            );

            return;
        }

        TaskResultRecord record = new TaskResultRecord
        {
            ExperimentId = taskExperimentId,

            TaskId = taskId,
            TaskName = taskName,

            TaskStartedAtUtc = taskStartedAtUtc,
            FirstActionAtUtc = firstActionAtUtc,
            TaskDurationSeconds = duration,

            Completed = true,

            FirstAttemptCorrect =
                firstAttemptRecorded && firstAttemptCorrect,

            FinalCorrect = finalCorrect,
            Score = finalScore,

            AttemptCount = attemptCount,
            IrrelevantActionCount = irrelevantActionCount,

            SelectedAnswerOrObject =
                string.Join(" > ", actionHistory),

            ErrorType = string.Join("; ", errorHistory),

            TechnicalFlag = technicalFlag,
            SavedAtUtc = DateTime.UtcNow.ToString("o")
        };

        int savedId = ExperimentDatabase.SaveTaskResult(record);

        if (savedId <= 0)
        {
            Debug.LogError(
                $"Task {taskId} | Sonuç kaydedilemedi; görev geçişi durduruldu."
            );

            return;
        }

        taskCompleted = true;
        resultSaved = true;

        // Bütün görevler tamamlandıysa senaryo süresini durdur.
        if (expectedTaskCount > 0)
        {
            ExperimentMetricsSummary progress =
                ExperimentSummaryCalculator.Calculate(
                    record.ExperimentId,
                    expectedTaskCount
                );

            if (progress.CompletedTaskCount == expectedTaskCount)
            {
                bool endSaved =
                    ExperimentDatabase.SaveScenarioEnd(
                        record.ExperimentId
                    );

                if (!endSaved)
                {
                    Debug.LogWarning(
                        "Görev sonucu kaydedildi ancak " +
                        "senaryo bitiş zamanı kaydedilemedi."
                    );
                }
            }
        }

        if (logSummaryAfterSave)
        {
            ExperimentSummaryCalculator.LogSummary(
                record.ExperimentId,
                expectedTaskCount
            );
        }

        ExperimentCsvExporter.ExportExperiment(
            record.ExperimentId
        );

        onTaskCompleted?.Invoke();
    }
}