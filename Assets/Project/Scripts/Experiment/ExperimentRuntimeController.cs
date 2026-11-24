using UnityEngine;
using UnityEngine.SceneManagement;

public class ExperimentRuntimeController : MonoBehaviour
{
    private void Start()
    {
        string currentScene =
            SceneManager.GetActiveScene().name;

        Debug.Log(
            "Current Scene: " + currentScene
        );

        // Setup ekranında önceki deney bilgilerini yazdırma.
        if (currentScene == "ExperimentSetup")
            return;

        if (ExperimentConfig.DatabaseRecordID <= 0 ||
            string.IsNullOrWhiteSpace(
                ExperimentConfig.ParticipantID))
        {
            Debug.LogWarning(
                "Aktif deney kaydı bulunamadı."
            );
            return;
        }

        Debug.Log(
            $"VR Scene Started | " +
            $"Participant: {ExperimentConfig.ParticipantID}, " +
            $"Mode: {ExperimentConfig.ScenarioMode}, " +
            $"Guidance: {ExperimentConfig.GuidanceEnabled}, " +
            $"Feedback: " +
            $"{ExperimentConfig.ImmediateFeedbackEnabled}"
        );
    }
}