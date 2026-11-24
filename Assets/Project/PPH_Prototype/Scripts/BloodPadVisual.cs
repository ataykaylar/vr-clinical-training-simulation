using System.Collections;
using UnityEngine;

public class BloodPadVisual : MonoBehaviour
{
    [SerializeField] private Renderer padRenderer;
    [SerializeField] private Material cleanMaterial;
    [SerializeField] private Material severeBloodMaterial;
    [SerializeField] private GameObject assessmentCanvas;
    [SerializeField] private float fillDuration = 5f;

    private Material runtimeMaterial;
    private Coroutine fillCoroutine;


    

    private void Awake()
    {
        if (assessmentCanvas != null)
            assessmentCanvas.SetActive(false);

        if (padRenderer == null)
            padRenderer = GetComponent<Renderer>();

        if (padRenderer == null ||
            cleanMaterial == null ||
            severeBloodMaterial == null)
        {
            Debug.LogError("BloodPadVisual references are missing.", this);
            enabled = false;
            return;
        }

        runtimeMaterial = new Material(cleanMaterial);
        padRenderer.material = runtimeMaterial;
        
    }

  
    public void StartBleeding()
    {
        if (fillCoroutine == null)
            fillCoroutine = StartCoroutine(FillPad());
    }

    private IEnumerator FillPad()
    {
        Color startColor = cleanMaterial.color;
        Color targetColor = severeBloodMaterial.color;
        float elapsedTime = 0f;

        while (elapsedTime < fillDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / fillDuration);
            runtimeMaterial.color =
                Color.Lerp(startColor, targetColor, progress);

            yield return null;
        }

        runtimeMaterial.color = targetColor;

        if (assessmentCanvas != null)
            assessmentCanvas.SetActive(true);
            
        fillCoroutine = null;

    }

    public void ResetPad()
    {
        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
            fillCoroutine = null;
        }

        if (runtimeMaterial != null)
            runtimeMaterial.color = cleanMaterial.color;
    }

    [ContextMenu("Test Start Bleeding")]
    private void TestStartBleeding()
    {
        StartBleeding();
    }
}