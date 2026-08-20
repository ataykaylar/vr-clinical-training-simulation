using System.IO;
using SQLite;
using UnityEngine;

public static class ExperimentDatabase
{
    private static string DatabasePath =>
        Path.Combine(Application.persistentDataPath, "pph_experiments.db");

    public static void Initialize()
    {
        using (SQLiteConnection database =
               new SQLiteConnection(DatabasePath))
        {
            database.CreateTable<ExperimentRecord>();
        }

        Debug.Log("Database ready: " + DatabasePath);
    }

    public static int SaveExperiment(ExperimentRecord record)
    {
        using (SQLiteConnection database =
               new SQLiteConnection(DatabasePath))
        {
            database.CreateTable<ExperimentRecord>();
            database.Insert(record);
        }
        Debug.Log("Database file: " + DatabasePath);

        Debug.Log(
            $"DATABASE SAVED | ID: {record.Id}, " +
            $"Participant: {record.ParticipantID}"
        );

        return record.Id;
    }
}