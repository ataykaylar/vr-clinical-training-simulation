using SQLite;

[Table("TaskResults")]
public class TaskResultRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int ExperimentId { get; set; }

    public int TaskId { get; set; }
    public string TaskName { get; set; }

    public string TaskStartedAtUtc { get; set; }
    public string FirstActionAtUtc { get; set; }
    public double TaskDurationSeconds { get; set; }

    public bool Completed { get; set; }
    public bool FirstAttemptCorrect { get; set; }
    public bool FinalCorrect { get; set; }

    public int Score { get; set; }
    public int AttemptCount { get; set; }
    public int IrrelevantActionCount { get; set; }

    public string SelectedAnswerOrObject { get; set; }
    public string ErrorType { get; set; }

    public bool TechnicalFlag { get; set; }
    public string SavedAtUtc { get; set; }
}