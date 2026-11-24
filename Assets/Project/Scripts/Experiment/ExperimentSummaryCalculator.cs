using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class ExperimentMetricsSummary
{
    public int ExperimentId;

    public int ExpectedTaskCount;
    public int RecordedTaskCount;
    public int CompletedTaskCount;
    public int SkippedTaskCount;

    public int TotalScore;

    public double CompletionRate;
    public double FirstAttemptAccuracy;
    public double FinalAccuracy;
    public double CorrectionRate;

    public double SessionDurationSeconds;
    public double TotalTaskDurationSeconds;
    public double AverageTaskDurationSeconds;
    public double MedianTaskDurationSeconds;

    public int TotalAttemptCount;
    public double AverageAttemptCount;

    public int TotalIrrelevantActionCount;
    public double IrrelevantActionsPerTask;

    public int TechnicalIssueCount;
    public int SequenceErrorCount;
}

public static class ExperimentSummaryCalculator
{
    public static ExperimentMetricsSummary Calculate(
        int experimentId,
        int expectedTaskCount)
    {
        List<TaskResultRecord> allResults =
            ExperimentDatabase.GetTaskResults(experimentId);

        // Her task için en son kaydı kullan.
        List<TaskResultRecord> results = allResults
            .GroupBy(result => result.TaskId)
            .Select(group => group
                .OrderByDescending(result => result.Id)
                .First())
            .ToList();

        ExperimentMetricsSummary summary =
            new ExperimentMetricsSummary();

        summary.ExperimentId = experimentId;
        summary.ExpectedTaskCount = expectedTaskCount;
        summary.RecordedTaskCount = results.Count;

        summary.CompletedTaskCount =
            results.Count(result => result.Completed);

        summary.SkippedTaskCount = Math.Max(
            0,
            expectedTaskCount - summary.CompletedTaskCount
        );

        summary.TotalScore =
            results.Sum(result => result.Score);

        summary.CompletionRate = Percentage(
            summary.CompletedTaskCount,
            expectedTaskCount
        );

        summary.FirstAttemptAccuracy = Percentage(
            results.Count(result => result.FirstAttemptCorrect),
            results.Count
        );

        summary.FinalAccuracy = Percentage(
            results.Count(result => result.FinalCorrect),
            results.Count
        );

        int initiallyWrongCount =
            results.Count(result => !result.FirstAttemptCorrect);

        int correctedCount =
            results.Count(result =>
                !result.FirstAttemptCorrect &&
                result.FinalCorrect);

        summary.CorrectionRate = Percentage(
            correctedCount,
            initiallyWrongCount
        );

        summary.TotalTaskDurationSeconds =
            results.Sum(result => result.TaskDurationSeconds);

        summary.AverageTaskDurationSeconds =
            results.Count > 0
                ? results.Average(result => result.TaskDurationSeconds)
                : 0;

        summary.MedianTaskDurationSeconds = CalculateMedian(
            results
                .Select(result => result.TaskDurationSeconds)
                .ToList()
        );

        summary.TotalAttemptCount =
            results.Sum(result => result.AttemptCount);

        summary.AverageAttemptCount =
            results.Count > 0
                ? (double)summary.TotalAttemptCount / results.Count
                : 0;

        summary.TotalIrrelevantActionCount =
            results.Sum(result => result.IrrelevantActionCount);

        summary.IrrelevantActionsPerTask =
            results.Count > 0
                ? (double)summary.TotalIrrelevantActionCount /
                  results.Count
                : 0;

        summary.TechnicalIssueCount =
            results.Count(result => result.TechnicalFlag);

        summary.SequenceErrorCount =
            results.Count(result =>
                !string.IsNullOrEmpty(result.ErrorType) &&
                result.ErrorType.IndexOf(
                    "sıra",
                    StringComparison.OrdinalIgnoreCase
                ) >= 0
            );

        // Genel süre: Başlat butonuna basıldığı andan itibaren.
        ExperimentRecord experiment =
            ExperimentDatabase.GetExperiment(experimentId);

        if (experiment != null &&
            DateTime.TryParse(
                experiment.ScenarioStartedAtUtc,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out DateTime scenarioStartedAt))
        {
            // Senaryo devam ediyorsa şu anı kullan.
            DateTime endTimeUtc = DateTime.UtcNow;

            // Senaryo bittiyse kayıtlı bitiş zamanını kullan.
            if (!string.IsNullOrWhiteSpace(
                    experiment.ScenarioEndedAtUtc))
            {
                if (!DateTime.TryParse(
                        experiment.ScenarioEndedAtUtc,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind,
                        out DateTime scenarioEndedAt))
                {
                    Debug.LogWarning(
                        "Senaryo bitiş zamanı okunamadı."
                    );

                    return summary;
                }

                endTimeUtc =
                    scenarioEndedAt.ToUniversalTime();
            }

            summary.SessionDurationSeconds = Math.Max(
                0,
                (endTimeUtc -
                 scenarioStartedAt.ToUniversalTime()).TotalSeconds
            );
        }

        return summary;
    }

    public static void LogSummary(
        int experimentId,
        int expectedTaskCount)
    {
        if (experimentId <= 0)
        {
            Debug.LogWarning(
                "Genel metrikler için geçerli " +
                "bir ExperimentId bulunamadı."
            );

            return;
        }

        ExperimentMetricsSummary summary =
            Calculate(experimentId, expectedTaskCount);

        Debug.Log(
            "===== GENEL DENEY METRİKLERİ =====\n" +
            $"Experiment ID: {summary.ExperimentId}\n" +
            $"Tamamlanan Task: " +
            $"{summary.CompletedTaskCount}/" +
            $"{summary.ExpectedTaskCount}\n" +
            $"Tamamlama Oranı: " +
            $"{summary.CompletionRate:F1}%\n" +
            $"Toplam Puan: {summary.TotalScore}\n" +
            $"İlk Deneme Accuracy: " +
            $"{summary.FirstAttemptAccuracy:F1}%\n" +
            $"Nihai Accuracy: " +
            $"{summary.FinalAccuracy:F1}%\n" +
            $"Düzeltme Oranı: " +
            $"{summary.CorrectionRate:F1}%\n" +
            $"Oturum Süresi: " +
            $"{summary.SessionDurationSeconds:F2} sn\n" +
            $"Ortalama Task Süresi: " +
            $"{summary.AverageTaskDurationSeconds:F2} sn\n" +
            $"Medyan Task Süresi: " +
            $"{summary.MedianTaskDurationSeconds:F2} sn\n" +
            $"Toplam Deneme: {summary.TotalAttemptCount}\n" +
            $"Ortalama Deneme: " +
            $"{summary.AverageAttemptCount:F2}\n" +
            $"Toplam Alakasız İşlem: " +
            $"{summary.TotalIrrelevantActionCount}\n" +
            $"Teknik Sorun: {summary.TechnicalIssueCount}\n" +
            $"Sıra Hatası: {summary.SequenceErrorCount}"
        );
    }

    private static double Percentage(int value, int total)
    {
        if (total <= 0)
            return 0;

        return value * 100.0 / total;
    }

    private static double CalculateMedian(List<double> values)
    {
        if (values == null || values.Count == 0)
            return 0;

        values.Sort();

        int middleIndex = values.Count / 2;

        if (values.Count % 2 == 1)
            return values[middleIndex];

        return (
            values[middleIndex - 1] +
            values[middleIndex]
        ) / 2.0;
    }
}