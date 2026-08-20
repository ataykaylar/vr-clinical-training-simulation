using System.Collections;
using TMPro;
using UnityEngine;

[System.Serializable]
public class VitalValues
{
    // Patient's heart rate in beats per minute (BPM).
    public int heartBeat;

    // Systolic blood pressure (the higher value).
    public int systolicPressure;
    // Diastolic blood pressure (the lower value).
    public int diastolicPressure;

    // Body temperature in Celsius (using float for precision).
    public float temperature;

    // Oxygen Saturation (SpO2) percentage in the blood.
    public int oxygenSaturation;

    // Respiration rate (breaths per minute).
    public int respirationRate;
}



public class PatientMonitorManager : MonoBehaviour
{
    public VitalValues[] OptimalVitalValuesAccordingToPatientCondition;

    public VitalValues vitalValuesChangeIndex;//Bu deðerin hangi aralýkta deðiþeceðini gösteriyor.

    public VitalValues currentPatientVitalValues = new VitalValues();

    public TextMeshProUGUI[] vitalValuesText;//0:heartBeat, 1:Pressure, 2:temperature, 3: oxygenSaturation,4: respirationRate

    private PatientCondition currentPatinetCondition;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPatinetCondition = GameManager.Instance.m_patientCondition;
        StartCoroutine(UpdateSimulatedVitalValues());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator UpdateSimulatedVitalValues()
    {
        while (true)
        {
            currentPatientVitalValues.systolicPressure =
                Random.Range(-vitalValuesChangeIndex.systolicPressure, vitalValuesChangeIndex.systolicPressure) +
                OptimalVitalValuesAccordingToPatientCondition[(int)GameManager.Instance.m_patientCondition].systolicPressure;

            currentPatientVitalValues.diastolicPressure =
                Random.Range(-vitalValuesChangeIndex.diastolicPressure, vitalValuesChangeIndex.diastolicPressure) +
                OptimalVitalValuesAccordingToPatientCondition[(int)GameManager.Instance.m_patientCondition].diastolicPressure;

            currentPatientVitalValues.temperature =
                Random.Range(-vitalValuesChangeIndex.temperature, vitalValuesChangeIndex.temperature) +
                OptimalVitalValuesAccordingToPatientCondition[(int)GameManager.Instance.m_patientCondition].temperature;

            currentPatientVitalValues.oxygenSaturation =
                Random.Range(-vitalValuesChangeIndex.oxygenSaturation, vitalValuesChangeIndex.oxygenSaturation) +
                OptimalVitalValuesAccordingToPatientCondition[(int)GameManager.Instance.m_patientCondition].oxygenSaturation;

            currentPatientVitalValues.respirationRate =
                Random.Range(-vitalValuesChangeIndex.respirationRate, vitalValuesChangeIndex.respirationRate) +
                OptimalVitalValuesAccordingToPatientCondition[(int)GameManager.Instance.m_patientCondition].respirationRate;

            for (int i = 0; i < 10; i++)
            {
                //Debug.Log("1");
                if (currentPatinetCondition != GameManager.Instance.m_patientCondition)//Update all condition suddenly.
                {
                    UpdateAllVitalValuesSuddenly();
                    currentPatinetCondition = GameManager.Instance.m_patientCondition;
                }

                currentPatientVitalValues.heartBeat =
                        Random.Range(-vitalValuesChangeIndex.heartBeat, vitalValuesChangeIndex.heartBeat) +
                        OptimalVitalValuesAccordingToPatientCondition[(int)GameManager.Instance.m_patientCondition].heartBeat;

                UpdateMonitorValuesUI();
                yield return GameParameters.wait1Dot0;
            }

            UpdateMonitorValuesUI();
        }
    }

    public void UpdateMonitorValuesUI()
    {
        vitalValuesText[0].text = currentPatientVitalValues.heartBeat.ToString();
        vitalValuesText[1].text = currentPatientVitalValues.systolicPressure.ToString() + "/" + currentPatientVitalValues.diastolicPressure.ToString();
        vitalValuesText[2].text = currentPatientVitalValues.temperature.ToString("F1");
        vitalValuesText[3].text = currentPatientVitalValues.oxygenSaturation.ToString();
        vitalValuesText[4].text = currentPatientVitalValues.respirationRate.ToString();
    }

    public void UpdateAllVitalValuesSuddenly()
    {
        currentPatientVitalValues.systolicPressure =
                Random.Range(-vitalValuesChangeIndex.systolicPressure, vitalValuesChangeIndex.systolicPressure) +
                OptimalVitalValuesAccordingToPatientCondition[(int)GameManager.Instance.m_patientCondition].systolicPressure;

        currentPatientVitalValues.diastolicPressure =
            Random.Range(-vitalValuesChangeIndex.diastolicPressure, vitalValuesChangeIndex.diastolicPressure) +
            OptimalVitalValuesAccordingToPatientCondition[(int)GameManager.Instance.m_patientCondition].diastolicPressure;

        currentPatientVitalValues.temperature =
            Random.Range(-vitalValuesChangeIndex.temperature, vitalValuesChangeIndex.temperature) +
            OptimalVitalValuesAccordingToPatientCondition[(int)GameManager.Instance.m_patientCondition].temperature;

        currentPatientVitalValues.oxygenSaturation =
            Random.Range(-vitalValuesChangeIndex.oxygenSaturation, vitalValuesChangeIndex.oxygenSaturation) +
            OptimalVitalValuesAccordingToPatientCondition[(int)GameManager.Instance.m_patientCondition].oxygenSaturation;

        currentPatientVitalValues.respirationRate =
            Random.Range(-vitalValuesChangeIndex.respirationRate, vitalValuesChangeIndex.respirationRate) +
            OptimalVitalValuesAccordingToPatientCondition[(int)GameManager.Instance.m_patientCondition].respirationRate;

        currentPatientVitalValues.heartBeat =
                    Random.Range(-vitalValuesChangeIndex.heartBeat, vitalValuesChangeIndex.heartBeat) +
                    OptimalVitalValuesAccordingToPatientCondition[(int)GameManager.Instance.m_patientCondition].heartBeat;

        UpdateMonitorValuesUI();
    }

}
