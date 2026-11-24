using System;
using System.IO;
using System.Collections.Generic;
using SQLite;
using UnityEngine;

public static class ExperimentDatabase
{
    private static string DatabasePath =>
        Path.Combine(
            Application.persistentDataPath,
            "pph_experiments.db"
        );

    public static void Initialize()
    {
        using (SQLiteConnection database =
               new SQLiteConnection(DatabasePath))
        {
            database.CreateTable<ExperimentRecord>();
            database.CreateTable<TaskResultRecord>();
            database.CreateTable<TaskSpecificMetricRecord>();
        }

        Debug.Log("Database ready: " + DatabasePath);
    }

    public static int SaveExperiment(ExperimentRecord record)
    {
        using (SQLiteConnection database =
               new SQLiteConnection(DatabasePath))
        {
            database.CreateTable<ExperimentRecord>();
            database.CreateTable<TaskResultRecord>();
            database.CreateTable<TaskSpecificMetricRecord>();

            database.Insert(record);
        }

        Debug.Log("Database file: " + DatabasePath);

        Debug.Log(
            $"DATABASE SAVED | ID: {record.Id}, " +
            $"Participant: {record.ParticipantID}"
        );

        return record.Id;
    }

    public static int SaveTaskResult(TaskResultRecord record)
    {
        if (record == null)
        {
            Debug.LogError("Task result record is null.");
            return 0;
        }

        if (record.ExperimentId <= 0)
        {
            Debug.LogWarning(
                "Task result does not have a valid ExperimentId."
            );
        }

        using (SQLiteConnection database =
               new SQLiteConnection(DatabasePath))
        {
            database.CreateTable<TaskResultRecord>();
            database.Insert(record);
        }

        Debug.Log(
            $"TASK RESULT SAVED | " +
            $"Experiment ID: {record.ExperimentId}, " +
            $"Task: {record.TaskId}, " +
            $"Correct: {record.FinalCorrect}, " +
            $"Score: {record.Score}"
        );

        return record.Id;
    }

    public static ExperimentRecord GetExperiment(int experimentId)
    {
        using (SQLiteConnection database =
               new SQLiteConnection(DatabasePath))
        {
            database.CreateTable<ExperimentRecord>();

            return database.Find<ExperimentRecord>(experimentId);
        }
    }

    public static List<TaskResultRecord> GetTaskResults(
        int experimentId)
    {
        using (SQLiteConnection database =
               new SQLiteConnection(DatabasePath))
        {
            database.CreateTable<TaskResultRecord>();

            return database
                .Table<TaskResultRecord>()
                .Where(result => result.ExperimentId == experimentId)
                .ToList();
        }
    }

    public static int SaveTaskSpecificMetric(
        TaskSpecificMetricRecord metric)
    {
        using (SQLiteConnection database =
               new SQLiteConnection(DatabasePath))
        {
            database.CreateTable<TaskSpecificMetricRecord>();
            database.Insert(metric);
        }

        Debug.Log(
            $"TASK METRIC SAVED | " +
            $"Experiment ID: {metric.ExperimentId}, " +
            $"Task: {metric.TaskId}, " +
            $"Metric: {metric.MetricName}, " +
            $"Value: {metric.MetricValue} {metric.Unit}"
        );

        return metric.Id;
    }

    public static List<TaskSpecificMetricRecord>
        GetTaskSpecificMetrics(int experimentId)
    {
        using (SQLiteConnection database =
               new SQLiteConnection(DatabasePath))
        {
            database.CreateTable<TaskSpecificMetricRecord>();

            return database
                .Table<TaskSpecificMetricRecord>()
                .Where(metric => metric.ExperimentId == experimentId)
                .OrderBy(metric => metric.RecordedAtUtc)
                .ToList();
        }
    }

    public static bool SaveScenarioStart(int experimentId)
    {
        if (experimentId <= 0)
        {
            Debug.LogWarning(
                "Senaryo başlatılamadı: Geçersiz deney ID."
            );
            return false;
        }

        using (SQLiteConnection database =
               new SQLiteConnection(DatabasePath))
        {
            database.CreateTable<ExperimentRecord>();

            ExperimentRecord record =
                database.Find<ExperimentRecord>(experimentId);

            if (record == null)
            {
                Debug.LogWarning(
                    "Senaryo başlatılamadı: Deney bulunamadı."
                );
                return false;
            }

            if (!string.IsNullOrWhiteSpace(record.ScenarioStartedAtUtc) ||
                !string.IsNullOrWhiteSpace(record.ScenarioEndedAtUtc))
            {
                Debug.LogWarning(
                    "Bu deney daha önce başlatılmış. " +
                    "Yeni test için ExperimentSetup üzerinden yeni deney aç."
                );
                return false;
            }

            record.ScenarioStartedAtUtc =
                DateTime.UtcNow.ToString("o");

            if (database.Update(record) != 1)
                return false;

            Debug.Log(
                $"SCENARIO STARTED | Experiment ID: {experimentId}"
            );

            return true;
        }
    }

    public static bool SaveScenarioEnd(int experimentId)
    {
        if (experimentId <= 0)
            return false;

        using (SQLiteConnection database =
               new SQLiteConnection(DatabasePath))
        {
            database.CreateTable<ExperimentRecord>();

            ExperimentRecord record =
                database.Find<ExperimentRecord>(experimentId);

            if (record == null)
                return false;

            if (string.IsNullOrWhiteSpace(record.ScenarioStartedAtUtc))
            {
                Debug.LogWarning(
                    "Senaryo bitişi kaydedilemedi: Başlangıç zamanı yok."
                );
                return false;
            }

            // Bitiş zaten kaydedilmişse zamanı değiştirme.
            if (!string.IsNullOrWhiteSpace(record.ScenarioEndedAtUtc))
                return true;

            record.ScenarioEndedAtUtc =
                DateTime.UtcNow.ToString("o");

            if (database.Update(record) != 1)
                return false;

            Debug.Log(
                $"SCENARIO ENDED | Experiment ID: {experimentId}"
            );

            return true;
        }
    }
}