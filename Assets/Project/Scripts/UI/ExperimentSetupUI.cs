using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ExperimentSetupUI : MonoBehaviour
{
    public TMP_InputField participantIDInput;
    public TMP_InputField ageInput;

    public TMP_Dropdown experienceDropdown;
    public TMP_Dropdown vrExperienceDropdown;
    public TMP_Dropdown modeDropdown;

    public Toggle guidanceToggle;
    public Toggle feedbackToggle;
 
    public void SaveExperimentSettings()
    {
        if (string.IsNullOrWhiteSpace(participantIDInput.text))
        {
            Debug.LogWarning("Participant ID is required.");
            return;
        }

        if (!int.TryParse(ageInput.text, out int participantAge))
        {
            Debug.LogWarning("Please enter a valid age.");
            return;
        }

        if (experienceDropdown.value == 0 ||
            vrExperienceDropdown.value == 0 ||
            modeDropdown.value == 0)
        {
            Debug.LogWarning("Please complete all dropdown selections.");
            return;
        }
         Debug.Log("Current Scene: " + SceneManager.GetActiveScene().name);


        ExperimentConfig.ParticipantID = participantIDInput.text.Trim();
        ExperimentConfig.Age = participantAge;

        ExperimentConfig.ClinicalExperience =
            experienceDropdown.options[experienceDropdown.value].text;

        ExperimentConfig.PreviousVRExperience =
            vrExperienceDropdown.options[vrExperienceDropdown.value].text;

        ExperimentConfig.ScenarioMode =
            modeDropdown.options[modeDropdown.value].text;

        ExperimentConfig.GuidanceEnabled = guidanceToggle.isOn;
        ExperimentConfig.ImmediateFeedbackEnabled = feedbackToggle.isOn;

        Debug.Log(  $"Participant: {ExperimentConfig.ParticipantID}, " +$"Age: {ExperimentConfig.Age}, " +$"Experience: {ExperimentConfig.ClinicalExperience}, " +$"VR Experience: {ExperimentConfig.PreviousVRExperience}, " +$"Mode: {ExperimentConfig.ScenarioMode}, " + $"Guidance: {ExperimentConfig.GuidanceEnabled}, " +$"Feedback: {ExperimentConfig.ImmediateFeedbackEnabled}");
        ExperimentRecord record = new ExperimentRecord
    {
    ParticipantID = ExperimentConfig.ParticipantID,
    Age = ExperimentConfig.Age,
    ClinicalExperience = ExperimentConfig.ClinicalExperience,
    PreviousVRExperience = ExperimentConfig.PreviousVRExperience,
    ScenarioMode = ExperimentConfig.ScenarioMode,
    GuidanceEnabled = ExperimentConfig.GuidanceEnabled,
    ImmediateFeedbackEnabled =
        ExperimentConfig.ImmediateFeedbackEnabled,
    StartedAtUtc = System.DateTime.UtcNow.ToString("o")
    };

ExperimentConfig.DatabaseRecordID =
    ExperimentDatabase.SaveExperiment(record);

        SceneManager.LoadScene("VR-Nurse_Training_Simulation");
    }
}