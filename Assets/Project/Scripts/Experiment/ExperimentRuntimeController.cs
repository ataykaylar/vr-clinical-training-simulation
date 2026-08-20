using UnityEngine;
using UnityEngine.SceneManagement;

public class ExperimentRuntimeController : MonoBehaviour
{
    private void Start()
    {
         Debug.Log("Current Scene: " + SceneManager.GetActiveScene().name);

        Debug.Log(
            $"VR Scene Started | " +
            $"Participant: {ExperimentConfig.ParticipantID}, " +
            $"Mode: {ExperimentConfig.ScenarioMode}, " +
            $"Guidance: {ExperimentConfig.GuidanceEnabled}, " +
            $"Feedback: {ExperimentConfig.ImmediateFeedbackEnabled}"
        );
    }
}