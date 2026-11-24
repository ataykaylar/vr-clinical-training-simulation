using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class ScenarioStartController : MonoBehaviour
{
    [Header("Connections")]
    [SerializeField] private TaskMetricTracker firstTaskTracker;
    [SerializeField] private BloodPadVisual bloodPadVisual;
    [SerializeField] private GameObject startCanvas;

    [Header("Editor Testing")]
    [Tooltip("Editor'da deney kaydı olmadan test yapar. DB ve CSV kaydı yapılmaz.")]
    [SerializeField] private bool developerTestMode = false;

    private static ScenarioStartController instance;

    private bool scenarioStarted;
    private bool testModeActive;
    private int experimentIdForThisRun;

    public static bool IsTestMode =>
        instance != null && instance.testModeActive;

    public static bool IsScenarioRunning =>
        instance != null && instance.scenarioStarted;

    public static int ActiveExperimentId =>
        IsScenarioRunning && !IsTestMode
            ? instance.experimentIdForThisRun
            : 0;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetRuntimeState()
    {
        instance = null;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError(
                "Sahnede birden fazla ScenarioStartController var.",
                this
            );

            enabled = false;
            return;
        }

        instance = this;

        // Test modu yalnızca Unity Editor içinde kullanılabilir.
        testModeActive = Application.isEditor && developerTestMode;

        scenarioStarted = false;
        experimentIdForThisRun = 0;

        if (testModeActive)
        {
            Debug.LogWarning(
                "[TEST MODU] Deney kaydı gerekmiyor. " +
                "Görev sonuçları DB veya CSV'ye yazılmayacak.",
                this
            );
        }
    }

    public void StartScenario()
    {
        if (instance != this ||
            !isActiveAndEnabled ||
            scenarioStarted)
        {
            return;
        }

        if (firstTaskTracker == null ||
            bloodPadVisual == null ||
            startCanvas == null)
        {
            Debug.LogError(
                "Başlatma bağlantıları eksik. Inspector alanlarını doldur.",
                this
            );

            return;
        }

        if (!testModeActive)
        {
            int experimentId = ExperimentConfig.DatabaseRecordID;

            bool saved =
                ExperimentDatabase.SaveScenarioStart(experimentId);

            if (!saved)
            {
                Debug.LogWarning(
                    "Başlangıç kaydedilemedi. Normal deney için " +
                    "ExperimentSetup üzerinden yeni deney oluştur.",
                    this
                );

                return;
            }

            experimentIdForThisRun = experimentId;
        }

        scenarioStarted = true;

        firstTaskTracker.BeginTask();
        bloodPadVisual.StartBleeding();
        startCanvas.SetActive(false);

        Debug.Log(
            testModeActive
                ? "[TEST MODU] Senaryo başladı. Kalıcı kayıt kapalı."
                : $"Senaryo başladı. Experiment ID: {experimentIdForThisRun}",
            this
        );
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}