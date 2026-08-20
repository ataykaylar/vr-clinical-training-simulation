using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;


public enum PatientCondition
{
    STABLE = 0,
    GUARDED = 1,
    CRITICAL = 2
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //TODO: Edit the code.


    //public PatientCondition m_patientCondition { get; private set; } = PatientCondition.STABLE;
    public PatientCondition m_patientCondition = PatientCondition.STABLE;

    private int scenarioCurrentStep = 0;

    public GameObject AreaNearThePatientCube;

    public GameObject PlayerMoveProviderObject;
    public GameObject PlayerTurnProviderObject;
    public GameObject PlayerTeleportProviderObject;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(ChangePatientCondition());
        StartCoroutine(ScenarioLoop());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ChangePatientCondition()
    {
        yield return GameParameters.wait3Dot0;
        m_patientCondition = PatientCondition.CRITICAL;
    }

    public void CollideWithThat(string str)
    {
        if(str == "AreaNearThePatient")
        {
            if(scenarioCurrentStep == 0) scenarioCurrentStep = 1;
        }
    }

    IEnumerator ScenarioLoop()
    {
        yield return null;

        //restrict movement
        PlayerMoveProviderObject.SetActive(false);

        UIManager.Instance.infoUI[0].SetActive(true);//TutorialUI

        while (scenarioCurrentStep == 0){ yield return null; }

        //Open patient approach test UI.
        Debug.Log("Open patient approach test");

        UIManager.Instance.infoUI[0].SetActive(false);//TutorialUI
        UIManager.Instance.infoUI[1].SetActive(false);//GoToNearThePatient
        UIManager.Instance.selectionUI[0].SetActive(true);

        AreaNearThePatientCube.SetActive(false);

        while (scenarioCurrentStep == 1) { yield return null; }
    }

    public void ClickedTheUI(int index)
    {
        if (index == 1)
        {//Clicked okey to Tutorial Text.
            UIManager.Instance.infoUI[0].SetActive(false);//TutorialUI
            UIManager.Instance.infoUI[1].SetActive(true);//GoToNearThePatient

            AreaNearThePatientCube.SetActive(true);

            //remove movement restrictions
            PlayerMoveProviderObject.SetActive(true);
        }
        else if (index == 1) 
        { 
        
        }
    }

    
}
