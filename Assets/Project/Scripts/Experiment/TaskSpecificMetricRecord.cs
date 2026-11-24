using SQLite;

[Table("TaskSpecificMetrics")]
public class TaskSpecificMetricRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int ExperimentId { get; set; }

    [Indexed]
    public int TaskId { get; set; }

    public string MetricName { get; set; }

    public string MetricValue { get; set; }

    public string Unit { get; set; }

    public string RecordedAtUtc { get; set; }
}