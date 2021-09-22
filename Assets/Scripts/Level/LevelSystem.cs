using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSystem : MonoBehaviour
{

    public Character Character;
    public camerascript MainCamera;
    public GameObject[] AiOnScene;
    public Dialogs[] DialogsOnScene;
    public GameObject[] ItemsOnScene;
    public GameObject[] ObjectOnScene;
    public GameObject[] HouseOnScene;

    public IndexSystem IndexSystem;
    public QuestSystem QuestSystem;
    public UIScript MainUI;
    public Animator anim;
    public DayTime Clock;
    public Particle ParticleSystem;
    public enum LocationType { World, House }
    public LocationType Location;
    public Scene Scene;
    public int NewScene;
    public InHouse House;
    public GameObject AiPref;
    public GameObject HousePref;
    private AsyncOperation async;
    public static bool AiLoadEnd;
    public int HouseNum;

    public Sprite[] HairType;
    public Sprite[] FaceType;
    public Sprite[] TorsoType;

    public int[] RequestedItem = new int[10];

    void Awake()
    {
        if(Character == null)
        {
            Character = GameObject.Find("Character").GetComponent<Character>();
        }
        NowData.Load();
        NowData.LevelNum = SceneManager.GetActiveScene().buildIndex;
        if (string.IsNullOrEmpty(NowData.SlotName))
        {
            NowData.SlotName = "TempSave";
        }
        if (SaveSystem.LoadLevel(NowData.LevelNum) != null && !NowData.inHouse)
        {
            StartCoroutine(LoadLevel(NowData.LevelNum));
        }
        if (SaveSystem.LoadPlayer() != null)
        {
            StartCoroutine(LoadCharacter(NowData.LevelNum));
        }
        if (NowData.FirstLoaded)
        {
            StartCoroutine(FirstLoaded());
        }
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        
        if (AiOnScene.Length == 0)
        {
            AiOnScene = new GameObject[EnemyAtScene().Length];
            AiOnScene = EnemyAtScene();
        }
        if (ObjectOnScene.Length == 0)
        {
            ObjectOnScene = new GameObject[ObjectAtScnene().Length];
            ObjectOnScene = ObjectAtScnene();
        }
        if (ItemsOnScene.Length == 0)
        {
            ItemsOnScene = new GameObject[ItemAtScene().Length];
            ItemsOnScene = ItemAtScene();
            for(int i = 0; i < ItemsOnScene.Length; i++)
            {
                ItemsOnScene[i].GetComponent<Item>().LevelSystem = this;
            }
        }
        if (HouseOnScene.Length == 0)
        {
            HouseOnScene = new GameObject[HouseAtScnene().Length];
            HouseOnScene = HouseAtScnene();
        }

    }

    #region System
    public GameObject[] ItemAtScene()
    {
        return GameObject.FindGameObjectsWithTag("Item");
    }
    public GameObject[] EnemyAtScene()
    {
        return GameObject.FindGameObjectsWithTag("Enemy");
    }
    public GameObject[] ObjectAtScnene()
    {
        return GameObject.FindGameObjectsWithTag("Object");
    }
    public GameObject[] HouseAtScnene()
    {
        return GameObject.FindGameObjectsWithTag("House");
    }

    IEnumerator FirstLoaded()
    {
        yield return new WaitForFixedUpdate();
        Vector4 SkinColor = new Vector4(NowData.SkinColor[0], NowData.SkinColor[1], NowData.SkinColor[2], NowData.SkinColor[3]);
        Vector4 HairColor = new Vector4(NowData.HairColor[0], NowData.HairColor[1], NowData.HairColor[2], NowData.HairColor[3]);
        Character.SetAppearence(NowData.Hair, NowData.Face, NowData.Torso, HairColor, SkinColor);
        Character.ChangeAppearance();

        DayTime.ClockTime = Clock.StartTime;

        NowData.FirstLoaded = false;
        yield break;
    }

    IEnumerator LoadLevel(int Slot)
    {
        if (SaveSystem.LoadLevel(Slot) == null)
        {
            Debug.Log("File error");
            yield break;
        }
        Scene.name = SaveSystem.LoadLevel(Slot).scene;
        if (Scene.name == "House")
        {
            Location = LocationType.House;
        }
        else
        {
            Location = LocationType.World;
        }
        DayTime.ClockTime = SaveSystem.LoadLevel(Slot).time;
        Clock.StartTime = SaveSystem.LoadLevel(Slot).timespeed;

        DeleteHouse();
        HouseOnScene = new GameObject[SaveSystem.LoadLevel(Slot).HouseNum];
        for (int i = 0; i < HouseOnScene.Length; i++)
        {
            int index = SaveSystem.LoadLevel(Slot).HouseOnSceneData[i].Index;
            HouseOnScene[i] = Instantiate(IndexSystem.Object[index]);
            HouseOnScene[i].transform.position = SaveSystem.LoadLevel(Slot).HouseOnSceneData[i].LoadPosition();
            HouseOnScene[i].GetComponent<House>().Data = SaveSystem.LoadLevel(Slot).HouseOnSceneData[i];
            HouseOnScene[i].name = SaveSystem.LoadLevel(Slot).HouseOnSceneData[i].ReferenceName;
        }


        DeleteObject();
        ObjectOnScene = new GameObject[SaveSystem.LoadLevel(Slot).ObjectNum];
        for (int i = 0; i < ObjectOnScene.Length; i++)
        {
            int index = SaveSystem.LoadLevel(Slot).ObjectOnSceneData[i].Index;
            ObjectOnScene[i] = Instantiate(IndexSystem.Object[index]);
            ObjectOnScene[i].GetComponent<Object>().Load(i, Slot);
        }

        DeleteAi();
        AiOnScene = new GameObject[SaveSystem.LoadLevel(Slot).AiNum];
        for (int i = 0; i < AiOnScene.Length; i++)
        {
            AiOnScene[i] = Instantiate(AiPref);
            AiOnScene[i].GetComponent<Ai>().Load(i, Slot);
        }

        DialogsOnScene = new Dialogs[SaveSystem.LoadLevel(Slot).DialogNum];
        for (int i = 0; i < DialogsOnScene.Length; i++)
        {
            if (SaveSystem.LoadLevel(Slot).DialogOnSceneData[i] != null)
            {
                AiOnScene[i].GetComponent<Dialogs>().Load(i, Slot);
            }
        }

        DeleteItems();
        ItemsOnScene = new GameObject[SaveSystem.LoadLevel(Slot).ItemsNum];
        for (int i = 0; i < ItemsOnScene.Length; i++)
        {
            if (SaveSystem.LoadLevel(Slot).ItemsOnSceneData[i].OnScene)
            {
                int index = SaveSystem.LoadLevel(Slot).ItemsOnSceneData[i].index;
                ItemsOnScene[i] = Instantiate(IndexSystem.Item(index));
                ItemsOnScene[i].GetComponent<Item>().LevelSystem = this;
                ItemsOnScene[i].GetComponent<Item>().Load(i, Slot);
            }
        }

        if (SaveSystem.HouseTemporaryLoad() != null)
        {
            HouseDate data = SaveSystem.HouseTemporaryLoad();
            HouseOnScene[NowData.HouseIndex].GetComponent<House>().Data = data;
            for (int i = 0; i < data.AiNum; i++)
            {
                if (GetAi(data.AiAtHouse[i]) != null)
                {
                    GetAi(data.AiAtHouse[i]).GetComponent<Dialogs>().Load(data.DialogsAtHouse[i]);
                }
            }
            SaveSystem.HouseTemporaryDelete();
        }

        if (SaveSystem.LoadLevel(Slot).QuestLevelData != null)
        {
            QuestSystem.Load(SaveSystem.LoadLevel(Slot).QuestLevelData);
        }
        yield break;
    }
    IEnumerator LoadCharacter(int Slot)
    {
        yield return new WaitForFixedUpdate();
        Character.Load();
        if (NowData.LoadPlayerPosition)
        {
            LoadCharacterPosition(NowData.LevelNum);
        }
        yield break;
    }
    public void LoadCharacterPosition(int Slot)
    {
        Character.transform.position = new Vector3(
        SaveSystem.LoadLevel(Slot).CharacterPosition[0],
        SaveSystem.LoadLevel(Slot).CharacterPosition[1],
        SaveSystem.LoadLevel(Slot).CharacterPosition[2]
        );
        Character.MainCamera.transform.position = Character.transform.position;
    }
    public void LoadDialogs(int Slot)
    {
        DialogsOnScene = new Dialogs[SaveSystem.LoadLevel(Slot).DialogNum];
        for (int i = 0; i < DialogsOnScene.Length; i++)
        {
            if (SaveSystem.LoadLevel(Slot).DialogOnSceneData[i] != null)
            {
                AiOnScene[i].AddComponent<Dialogs>();
                AiOnScene[i].GetComponent<Ai>().Dialog = AiOnScene[i].GetComponent<Dialogs>();
                AiOnScene[i].GetComponent<Dialogs>().Load(i, Slot);
            }
        }
    }
    public void DeleteHouse()
    {
        GameObject[] Houses = HouseAtScnene();
        for (int i = 0; i < Houses.Length; i++)
        {
            Houses[i].GetComponent<House>().Destroy();
        }
    }
    public void DeleteItems()
    {
        GameObject[] Items = ItemAtScene();
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].transform.parent == null)
            {
                Items[i].SetActive(false);
                Items[i].GetComponent<Item>().Destroy();
                Items[i] = null;
            }
        }
        
    }
    public void DeleteAi()
    {
        GameObject[] Enemy = EnemyAtScene();
        for (int i = 0; i < Enemy.Length; i++)
        {
            Enemy[i].GetComponent<Ai>().Destroy();
            Enemy[i] = null;
        }
    }
    public void DeleteObject()
    {
        GameObject[] Obj = ObjectAtScnene();
        for (int i = 0; i < Obj.Length; i++)
        {
            Obj[i].GetComponent<Object>().Destroy();
            Obj[i] = null;
        }
    }
    public void SetName(GameObject Obj)
    {
        for (int i = 0; i < ItemAtScene().Length; i++)
        {
            if (GameObject.Find(Obj.name + i) == null)
                Obj.name = Obj.name + i;
        }
    }

    public void StartLoadingScene()
    {
        async = SceneManager.LoadSceneAsync(NewScene, LoadSceneMode.Single);
        async.allowSceneActivation = false;
    }
    public void LoadScene()
    {
        StartCoroutine(WaitUntilLoad());
    }
    IEnumerator WaitUntilLoad()
    {
        anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.5f);
        while(async.isDone)
        {
            if(async.progress >= 0.9f)
            {
                async.allowSceneActivation = true;
            }
            yield return null;
        }
        async.allowSceneActivation = true;
        yield break;
    }
    #endregion

    public void GetToHouse(House House)
    {
        NowData.PrevLevelNum = SceneManager.GetActiveScene().buildIndex;
        NowData.LoadPlayerPosition = false;
        NowData.LevelNum = House.Scene;
        NowData.inHouse = true;
        Location = LocationType.House;
        for (int i = 0; i < HouseAtScnene().Length; i++)
        {
            if (HouseAtScnene()[i] == House.gameObject)
            {
                NowData.HouseIndex = i;
                break;
            }
        }
        NewScene = NowData.LevelNum;
        Save(NowData.PrevLevelNum);
        StartLoadingScene();
        LoadScene();
    }
    public void GetToNewLevel(int Level)
    {
        NowData.PrevLevelNum = SceneManager.GetActiveScene().buildIndex;
        NowData.LoadPlayerPosition = false;
        NowData.LevelNum = Level;
        NewScene = NowData.LevelNum;
        Save(NowData.PrevLevelNum);
        StartLoadingScene();
        LoadScene();
    }

    public int GetAiNum(GameObject Man)
    {
        for (int i = 0; i < AiOnScene.Length; i++)
        {
            if (AiOnScene[i] == Man)
            {
                return i;
            }
        }
        return -1;
    }
    public GameObject GetAi(int i)
    {
        if (i != -1 && i < AiOnScene.Length && AiOnScene[i] != null)
            return AiOnScene[i];
        else
            return null;
    }
    public void SaveLevel(int Slot)
    {
        SaveSystem.SaveLevel(transform.GetComponent<LevelSystem>(), Slot);
    }
    public void SaveCharacter()
    {
        Character.Save();
    }

    public void Save(int Level)
    {
        SaveLevel(Level);
        SaveCharacter();
        NowData.Save();
    }

    public void SaveAndExit()
    {
        Time.timeScale = 1;
        Save(NowData.LevelNum);
        SceneManager.LoadScene(0);
    }
    public void Exit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public GameObject GetRandomItem(int num)
    {
        bool Ready = false;
        int[] items = new int[IndexSystem.Index.Length];
        GameObject[] CurrentItems = new GameObject[ItemsOnScene.Length];
        int Stage = 0;
        int Sum = 0;
        for(int i = 0; i < ItemsOnScene.Length; i++)
        {
            if(ItemsOnScene[i] != null)
                items[ItemsOnScene[i].GetComponent<Item>().Index]++;
        }
        for(int i = 0; i < items.Length; i++)
        {
            if(items[i] >= num)
            {
                CurrentItems[Stage] = IndexSystem.Item(i);
                Stage++;
                Sum++;
            }
        }
        int Rand = Random.Range(0, Sum + 1);
        return CurrentItems[Rand];
    }
    public void AddRequsetedItem(int Index)
    {
        GameObject[] Items = ItemAtScene();
        for (int i = 0; i < RequestedItem.Length; i++)
        {
            if(RequestedItem[i] < 0)
            {
                RequestedItem[i] = Index;
                for(int a = 0; a < ItemAtScene().Length; a++)
                {
                    if(Items[a].GetComponent<Item>().Index == Index)
                    {
                        Items[a].GetComponent<Item>().Requested = true;
                    }
                }
                break;
            }
        }
    }
    public void DeleteRequsetedItem(int Index)
    {
        for (int i = 0; i < RequestedItem.Length; i++)
        {
            if (RequestedItem[i] == Index)
            {
                RequestedItem[i] = -1;
                for (int a = 0; a < ItemAtScene().Length; a++)
                {
                    if (ItemAtScene()[a].GetComponent<Item>().Index == Index)
                    {
                        ItemAtScene()[a].GetComponent<Item>().Requested = false;
                    }
                }
                break;
            }
        }
    }



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartLoadingScene();
            LoadScene();
        }
    }
}
