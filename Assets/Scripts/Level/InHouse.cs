using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InHouse : MonoBehaviour
{

    public string ReferenceName;
    public int Index;

    public GameObject[] AiAtHome;
    public int[] AiAtHomeNum;
    public AiData AiDataAtHome;
    public GameObject[] ObjectAtHome;
    public GameObject[] ItemAtHome;

    public Transform EnterPosition;
    public Transform Position;
    public Transform[] StaffPosition;
    public GameObject AiPref;

    public IndexSystem IndexSystem;
    public LevelSystem levelSystem;

    public GameObject[] ItemHere()
    {
        return GameObject.FindGameObjectsWithTag("Item");
    }
    public GameObject[] ObjectHere()
    {
        return GameObject.FindGameObjectsWithTag("Object");
    }

    private void Awake()
    {
        if (levelSystem == null)
        {
            levelSystem = GameObject.Find("LevelSystem").GetComponent<LevelSystem>();
        }
        if(IndexSystem == null)
        {
            IndexSystem = Resources.Load<IndexSystem>("IndexSystem");
        }
        Debug.Log(NowData.SlotName);
    }
    void Start()
    {
        for (int i = 0; i < Position.childCount; i++)
        {
            StaffPosition[i] = Position.GetChild(i);
        }
        Load(NowData.PrevLevelNum);
        levelSystem.NewScene = NowData.PrevLevelNum;
        levelSystem.StartLoadingScene();
    }

    public void Load(int Slot)
    {
        int num = NowData.HouseIndex;
        HouseDate data = SaveSystem.LoadLevel(Slot).HouseOnSceneData[num];
        ReferenceName = data.ReferenceName;
        Index = data.Index;
        AiAtHome = new GameObject[data.AiNum];
        AiAtHomeNum = new int[data.AiNum];
        
        for (int i = 0; i < data.AiNum; i++)
        {
            if (data.AiAtHouse[i] > -1)
            {
                int index = data.AiAtHouse[i];
                AiAtHomeNum[i] = index;
                AiAtHome[i] = Instantiate(AiPref);
                AiAtHome[i].GetComponent<Ai>().Load(index, Slot);
                AiAtHome[i].GetComponent<Dialogs>().Load(data.DialogsAtHouse[i]);
                AiAtHome[i].transform.position = StaffPosition[i].position;
                AiAtHome[i].GetComponent<Ai>().Home = null;
                AiAtHome[i].GetComponent<Ai>().Metier = new Transform[0];
                AiAtHome[i].GetComponent<Ai>().AiState = Ai.States.walk;
            }
            else
                AiAtHomeNum[i] = -1;
        }

        ObjectAtHome = new GameObject[data.ObjNum];
        for(int i = 0; i < ObjectAtHome.Length; i++)
        {
            int index = data.ObjectsAtHouse[i].Index;
            if(index != -1)
            {
                ObjectAtHome[i] = Instantiate(IndexSystem.Object[index]);
                ObjectAtHome[i].GetComponent<Object>().Load(data.ObjectsAtHouse[i]);
            }
            
        }

        ItemAtHome = new GameObject[data.ItemsNum];
        for(int i = 0; i < ItemAtHome.Length; i++)
        {
            int index = data.ItemsAtHouse[i].index;
            if(IndexSystem.Item(index) != null && data.ItemsAtHouse[i].OnScene)
            {
                ItemAtHome[i] = Instantiate(IndexSystem.Item(index));
                ItemAtHome[i].GetComponent<Item>().Load(data.ItemsAtHouse[i]);

            }
        }
    }
    public void Save()
    {
        ItemAtHome = ItemHere();
        ObjectAtHome = ObjectHere();
        NowData.Save();
        levelSystem.SaveCharacter();
        SaveSystem.HouseTemporarySave(this);
    }

    public void Leave()
    {
        NowData.inHouse = false;
        NowData.LevelNum = NowData.PrevLevelNum;
        NowData.LoadPlayerPosition = true;
        NowData.PrevLevelNum = SceneManager.GetActiveScene().buildIndex;
        Save();
        levelSystem.LoadScene();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Leave();
        }
    }
}
