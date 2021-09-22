using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class House : MonoBehaviour
{
    public string Name;
    public HouseDate Data;

    public GameObject[] Staff;
    public Vector3[] Position;
    public string WelcomeText;
    public Transform UI;
    
    private TextMeshProUGUI TextUI;

    public LevelSystem levelSystem;
    public int Index;
    public int Scene;
    public bool InHouse;
    private DayTime Clock;

    private void Awake()
    {
        if (Staff.Length > Data.ObjectsAtHouse.Length)
        {
            LoadStaff();
        }
    }
    private void Start()
    {
        TextUI = UI.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        levelSystem = GameObject.FindGameObjectWithTag("Level").GetComponent<LevelSystem>();
        Clock = GameObject.Find("DayTime").GetComponent<DayTime>();
    }

    void ShowText()
    {
        TextUI.text = WelcomeText;
    }
    void HideText()
    {
        TextUI.text = "";
    }

    void GetIntoHouse()
    {
        levelSystem.GetToHouse(transform.GetComponent<House>());
    }
    public void AiGetInto(GameObject Man)
    {
        Data.AiNum = Data.AiAtHouse.Length;
        for (int i = 0; i < Data.AiAtHouse.Length; i++)
        {
            if (Data.AiAtHouse[i] == -1 && Data.AiAtHouse[i] != levelSystem.GetAiNum(Man))
            {
                Data.AiAtHouse[i] = levelSystem.GetAiNum(Man);
                Data.DialogsAtHouse[i] = new DialogData(Man.GetComponent<Dialogs>());
                break;
            }
        }
    }
    public void AiGetOut(GameObject Man)
    {
        Data.AiNum = Data.AiAtHouse.Length;
        for (int i = 0; i < Data.AiNum; i++)
        {
            if (Data.AiAtHouse[i] == levelSystem.GetAiNum(Man))
            {
                Data.AiAtHouse[i] = -1;
                Data.DialogsAtHouse[i] = null;
                break;
            }
        }
    }

    void GetHit()
    {

    }
    void GetCrushEffect()
    {

    }

    public void LoadStaff()
    {
        Data.ObjectsAtHouse = new ObjectData[Staff.Length];
        Data.ObjNum = Staff.Length;
        for (int i = 0; i < Staff.Length; i++)
        {
            if (Staff[i] != null)
            {
                Object obj = Staff[i].GetComponent<Object>();
                obj.transform.position = Position[i];
                Data.ObjectsAtHouse[i] = new ObjectData(obj);
            }
            else
            {
                Data.ObjectsAtHouse[i] = new ObjectData(null);
                Data.ObjectsAtHouse[i].Index = -1;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.root.tag == "Player")
        {
            ShowText();
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.root.tag == "Player")
        {
            HideText();
        }
    }

    public void Save()
    {

    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
