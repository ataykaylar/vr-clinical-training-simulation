using SQLite;

[Table("Experiments")]
public class ExperimentRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string ParticipantID { get; set; }
    public int Age { get; set; }
    public string ClinicalExperience { get; set; }
    public string PreviousVRExperience { get; set; }

    public string ScenarioMode { get; set; }
    public bool GuidanceEnabled { get; set; }
    public bool ImmediateFeedbackEnabled { get; set; }

    public string StartedAtUtc { get; set; }
}