using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject[] infoUI = new GameObject[10]; //0:Tuto , 1:GoNearThePatient, 2:
    public GameObject[] warningUI = new GameObject[10];//0:Privacy, 1: 
    public GameObject[] selectionUI = new GameObject[10];//0:ApproachThePatient
    public GameObject[] redirectUI = new GameObject[10];//0:EvaluateBleeding


    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenTutorailUI()
    {
        infoUI[0].SetActive(true);
    }

    
}
