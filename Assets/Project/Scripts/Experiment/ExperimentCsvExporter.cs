using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public static class ExperimentCsvExporter
{
    public static void ExportCurrentExperiment()
    {
        int experimentId = ExperimentConfig.DatabaseRecordID;

        if (experimentId <= 0)
        {
            Debug.LogWarning(
                "CSV oluşturulamadı: Aktif deney kaydı bulunamadı."
            );
            return;
        }

        ExportExperiment(experimentId);
    }

    public static void ExportExperiment(int experimentId)
    {
        ExperimentRecord experiment =
            ExperimentDatabase.GetExperiment(experimentId);

        if (experiment == null)
        {
            Debug.LogWarning(
                $"CSV oluşturulamadı: Experiment {experimentId} bulunamadı."
            );
            return;
        }

        var taskResults =
            ExperimentDatabase.GetTaskResults(experimentId);

        var specificMetrics =
            ExperimentDatabase.GetTaskSpecificMetrics(experimentId);

        string exportFolder = Path.Combine(
            Application.persistentDataPath,
            "Exports"
        );

        Directory.CreateDirectory(exportFolder);

        string filePrefix = $"PPH_Experiment_{experimentId}";

        string participantPath = Path.Combine(
            exportFolder,
            $"{filePrefix}_Participant.csv"
        );

        string taskResultsPath = Path.Combine(
            exportFolder,
            $"{filePrefix}_TaskResults.csv"
        );

        string specificMetricsPath = Path.Combine(
            exportFolder,
            $"{filePrefix}_SpecificMetrics.csv"
        );

        WriteParticipantCsv(participantPath, experiment);
        WriteTaskResultsCsv(taskResultsPath, taskResults);
        WriteSpecificMetricsCsv(
            specificMetricsPath,
            specificMetrics
        );

        Debug.Log(
            $"CSV EXPORT COMPLETED | Experiment ID: {experimentId}\n" +
            $"Folder: {exportFolder}"
        );
    }

    private static void WriteParticipantCsv(
        string path,
        ExperimentRecord experiment)
    {
        StringBuilder csv = new StringBuilder();

        csv.AppendLine(
            "ExperimentId;ParticipantID;Age;" +
            "ClinicalExperience;PreviousVRExperience;" +
            "ScenarioMode;GuidanceEnabled;" +
            "ImmediateFeedbackEnabled;StartedAtUtc"
        );

        csv.AppendLine(string.Join(";",
            Csv(experiment.Id),
            Csv(experiment.ParticipantID),
            Csv(experiment.Age),
            Csv(experiment.ClinicalExperience),
            Csv(experiment.PreviousVRExperience),
            Csv(experiment.ScenarioMode),
            Csv(experiment.GuidanceEnabled ? 1 : 0),
            Csv(experiment.ImmediateFeedbackEnabled ? 1 : 0),
            Csv(experiment.StartedAtUtc)
        ));

        WriteUtf8File(path, csv.ToString());
    }

    private static void WriteTaskResultsCsv(
        string path,
        System.Collections.Generic.List<TaskResultRecord> results)
    {
        StringBuilder csv = new StringBuilder();

        csv.AppendLine(
            "Id;ExperimentId;TaskId;TaskName;" +
            "TaskStartedAtUtc;FirstActionAtUtc;" +
            "TaskDurationSeconds;Completed;" +
            "FirstAttemptCorrect;FinalCorrect;Score;" +
            "AttemptCount;IrrelevantActionCount;" +
            "SelectedAnswerOrObject;ErrorType;" +
            "TechnicalFlag;SavedAtUtc"
        );

        foreach (TaskResultRecord result in results)
        {
            csv.AppendLine(string.Join(";",
                Csv(result.Id),
                Csv(result.ExperimentId),
                Csv(result.TaskId),
                Csv(result.TaskName),
                Csv(result.TaskStartedAtUtc),
                Csv(result.FirstActionAtUtc),
                Csv(result.TaskDurationSeconds.ToString(
                    "F3",
                    CultureInfo.InvariantCulture
                )),
                Csv(result.Completed ? 1 : 0),
                Csv(result.FirstAttemptCorrect ? 1 : 0),
                Csv(result.FinalCorrect ? 1 : 0),
                Csv(result.Score),
                Csv(result.AttemptCount),
                Csv(result.IrrelevantActionCount),
                Csv(result.SelectedAnswerOrObject),
                Csv(result.ErrorType),
                Csv(result.TechnicalFlag ? 1 : 0),
                Csv(result.SavedAtUtc)
            ));
        }

        WriteUtf8File(path, csv.ToString());
    }

    private static void WriteSpecificMetricsCsv(
        string path,
        System.Collections.Generic.List<TaskSpecificMetricRecord> metrics)
    {
        StringBuilder csv = new StringBuilder();

        csv.AppendLine(
            "Id;ExperimentId;TaskId;MetricName;" +
            "MetricValue;Unit;RecordedAtUtc"
        );

        foreach (TaskSpecificMetricRecord metric in metrics)
        {
            csv.AppendLine(string.Join(";",
                Csv(metric.Id),
                Csv(metric.ExperimentId),
                Csv(metric.TaskId),
                Csv(metric.MetricName),
                Csv(metric.MetricValue),
                Csv(metric.Unit),
                Csv(metric.RecordedAtUtc)
            ));
        }

        WriteUtf8File(path, csv.ToString());
    }

    private static string Csv(object value)
    {
        string text = value?.ToString() ?? "";
        text = text.Replace("\"", "\"\"");

        return $"\"{text}\"";
    }

    private static void WriteUtf8File(
        string path,
        string content)
    {
        File.WriteAllText(
            path,
            content,
            new UTF8Encoding(true)
        );
    }
}