using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;

public class UIMenu : MonoBehaviour
{
    public Transform MainMenu;
    public TextMeshProUGUI[] MainMenuText;

    public Transform LoadGameMenu;
    public TextMeshProUGUI[] LoadSlotsText;
    public TextMeshProUGUI LoadGameText;

    public GameObject SubmitDeleteUi;
    private int SelectedDeleteSlot;
    public GameObject[] DeleteSlots;

    public Transform NewGameMenu;
    public TMP_InputField InputSlotName;
    public TMP_InputField InputPlayerName;
    public TextMeshProUGUI[] NewGameMenuText;

    public Transform SettingMenu;
    public TextMeshProUGUI[] SettingsMenuText;
    public TMP_Dropdown Dropdown;


    private LanguagesSystem Language;
    private Animator anim;
    public GameObject FirstSelected;
    private bool KeyNavigationOn;
    public EventSystem Event;
    public int FirstLevelLoad;
    public GameSetting GameSetting;

    private int NowBody = 0;
    private int NowFace = 0;
    private int NowHair = 0;
    public Slider HairColor;
    public Slider HairTone;
    public SpriteRenderer Hair;
    public Sprite[] HairType;
    public SpriteRenderer Face;
    public Sprite[] FaceType;
    public Slider TorsoTone;
    public SpriteRenderer Torso;
    public Sprite[] TorsoType;
    private int SelectedSlot;
    private int SelectedFace;
    private int SelectedTorso;

    void Awake()
    {
        anim = GetComponent<Animator>();
        Language = GetComponent<LanguagesSystem>();
        NowData.Load();
        GameSetting.Load();
    }
    void Start()
    {
        SetLanguage(GameSetting.SystemLanguage);
    }
    #region NewGame
    public void Play()
    {
        NowData.FirstLoaded = true;
        string PlayerName = InputPlayerName.text;
        NowData.PlayerName = PlayerName;
        NowData.Hair = NowFace * 4 + NowHair;
        NowData.Face = NowFace;
        NowData.Torso = NowBody;
        NowData.HairColor = new float[4];
        NowData.HairColor[0] = Hair.material.GetColor("_Color").r;
        NowData.HairColor[1] = Hair.material.GetColor("_Color").g;
        NowData.HairColor[2] = Hair.material.GetColor("_Color").b;
        NowData.HairColor[3] = Hair.material.GetColor("_Color").a;
        NowData.SkinColor = new float[4];
        NowData.SkinColor[0] = Torso.material.GetColor("_SkinColor").r;
        NowData.SkinColor[1] = Torso.material.GetColor("_SkinColor").g;
        NowData.SkinColor[2] = Torso.material.GetColor("_SkinColor").b;
        NowData.SkinColor[3] = Torso.material.GetColor("_SkinColor").a;

        NowData.LevelNum = FirstLevelLoad;
        NowData.PrevLevelNum = FirstLevelLoad;
        NowData.inHouse = false;
        NowData.SlotName = "";
        SaveSystem.DeleteTempSlot();
        SceneManager.LoadScene(FirstLevelLoad);
    }
    public void NextHair(bool isNext)
    {
        if (isNext)
        {
            if (NowHair + 2 > 4)
            {
                NowHair = 0;
            }
            else
            {
                NowHair++;
            }
        }
        else
        {
            if (NowHair - 1 < 0)
            {
                NowHair = 3;
            }
            else
            {
                NowHair--;
            }
        }
        Hair.sprite = HairType[NowFace * 4 + NowHair];
    }
    public void NextHead(bool isNext)
    {
        if (isNext)
        {
            if (NowFace + 2 > FaceType.Length)
            {
                NowFace = 0;
            }
            else
            {
                NowFace++;
            }
        }
        else
        {
            if (NowFace - 1 < 0)
            {
                NowFace = FaceType.Length - 1;
            }
            else
            {
                NowFace--;
            }
        }
        Face.sprite = FaceType[NowFace];
        Hair.sprite = HairType[NowFace * 4 + NowHair];
    }
    public void NextBody(bool isNext)
    {
        if(isNext)
        {
            if(NowBody + 2 > TorsoType.Length)
            {
                NowBody = 0;
            }
            else
            {
                NowBody++;
            }

        }
        else
        {
            if (NowBody - 1 < 0)
            {
                NowBody = TorsoType.Length - 1;
            }
            else
            {
                NowBody--;
            }
            
        }
        Torso.sprite = TorsoType[NowBody];
    }
    public void ChangeHairColor()
    {
        Color color = Color.HSVToRGB(HairColor.value * 0.75f, 0.7f, HairTone.value);
        Hair.material.SetColor("_Color", color);
    }
    public void ChangeHairTone()
    {
        Color color = Color.HSVToRGB(HairColor.value * 0.75f, 0.7f, HairTone.value);
        Hair.material.SetColor("_Color", color);
    }
    public void ChangeSkinTone()
    {
        Color Col = Color.HSVToRGB(0.2f, 0.2f, TorsoTone.value * 0.8f);
        Col = new Vector4(Col.r, Col.g, Col.b, 0);
        Face.material.SetColor("_SkinColor", Col);
        Torso.material.SetColor("_SkinColor", Col);
    }
    #endregion
    #region LoadGame
    public void LoadGame(int Slot)
    {
        if (Slot < GameSetting.SlotName.Length )
        {
            NowData.SlotName = GameSetting.SlotName[Slot];
            NowData.Load();
            NowData.Save();

            SceneManager.LoadScene(NowData.LevelNum);
        }
        else
        {
            
        }
    }
    public void DeleteLoad(int Slot)
    {
        if (Slot < GameSetting.SlotName.Length)
        {
            SelectedDeleteSlot = Slot;
            SubmitDeleteUi.SetActive(true);
        }
    }
    public void SubmitDelete(bool Delete)
    {
        if(Delete)
        {
            SaveSystem.DeleteSlot(GameSetting.SlotName[SelectedDeleteSlot]);
            SubmitDeleteUi.SetActive(false);
            GoLoadMenu();
        }
        else
        {
            SubmitDeleteUi.SetActive(false);
        }
    }
    #endregion
    #region MenuNavigation
    public void GoMainMenu()
    {
        KeyNavigationOn = false;
        SettingMenu.gameObject.SetActive(false);
        LoadGameMenu.gameObject.SetActive(false);
        NewGameMenu.gameObject.SetActive(false);

        MainMenu.gameObject.SetActive(true);
        FirstSelected = MainMenuText[MainMenuText.Length - 1].transform.parent.gameObject;

        GameSetting.Save();
    }
    public void GoLoadMenu()
    {
        KeyNavigationOn = false;
        MainMenu.gameObject.SetActive(false);
        LoadGameMenu.gameObject.SetActive(true);

        //FirstSelected = LoadGameMenuText[LoadGameMenuText.Length - 1].transform.parent.gameObject;

        string[] Directories = Directory.GetDirectories(Application.persistentDataPath);
        GameSetting.SlotName = new string[Directories.Length];
        for(int i = 0; i < LoadSlotsText.Length; i++)
        {
            LoadSlotsText[i].color = Color.grey;
            LoadSlotsText[i].text = LanguagesSystem.Language.MenuEmptySlot;
        }
        for(int i = 0; i < DeleteSlots.Length; i++)
        {
            DeleteSlots[i].SetActive(false);
        }
        for (int i = 0; i < Directories.Length; i++)
        {
            GameSetting.SlotName[i] = Directories[i].Replace(Application.persistentDataPath + @"\", "");
            LoadSlotsText[i].text = GameSetting.SlotName[i];
            LoadSlotsText[i].color = Color.white;

            DeleteSlots[i].SetActive(true);
        }
        
    }
    public void GoNewGameMenu()
    {
        KeyNavigationOn = false;
        MainMenu.gameObject.SetActive(false);
        NewGameMenu.gameObject.SetActive(true);

        FirstSelected = NewGameMenuText[NewGameMenuText.Length - 1].transform.parent.gameObject;
       
    }
    public void GoSettingsMenu()
    {
        KeyNavigationOn = false;
        MainMenu.gameObject.SetActive(false);
        SettingMenu.gameObject.SetActive(true);

        FirstSelected = SettingsMenuText[SettingsMenuText.Length - 1].transform.parent.gameObject;
    }
    public void ButtonExit()
    {
        anim.SetTrigger("Exit");
    }
    #endregion
    #region Settings
    public void ChangeLanguage()
    {
        SetLanguage(Dropdown.value);
        GameSetting.SystemLanguage = Dropdown.value;
        GameSetting.Save();
    }
    public void SetLanguage(int Lang)
    {
        Language.LoadLanguage(Lang);
        Dropdown.value = GameSetting.SystemLanguage;
        for (int i = 0; i < Language.LanguagesArray.Length; i++)
        {
            string text = LanguagesSystem.Language.MenuLanguage[i];
            Dropdown.options[i].text = text;
        }
        Dropdown.captionText.text = LanguagesSystem.Language.MenuLanguage[GameSetting.SystemLanguage];

        for (int i = 0; i < MainMenuText.Length; i++)
        {
            if (MainMenuText[i] != null)
            {
                MainMenuText[i].text = LanguagesSystem.Language.MenuMain[i];
            }
        }

        for (int i = 0; i < LoadSlotsText.Length; i++)
        {
            if (LoadSlotsText[i] != null)
            {
                LoadSlotsText[i].text = LanguagesSystem.Language.MenuEmptySlot;
            }
        }

        for (int i = 0; i < NewGameMenuText.Length; i++)
        {
            if (NewGameMenuText[i] != null)
            {
                NewGameMenuText[i].text = LanguagesSystem.Language.MenuNewGame[i];
            }
        }

        for (int i = 0; i < SettingsMenuText.Length; i++)
        {
            if (SettingsMenuText[i] != null)
            {
                SettingsMenuText[i].text = LanguagesSystem.Language.MenuSettings[i];
            }
        }
    }
    #endregion
    
    private void Exit()
    {
        GameSetting.Save();
        Application.Quit();
    }

    private void Update()
    {
        if(Input.GetButtonDown("MovementY"))
        {
            if (!KeyNavigationOn)
            {
                KeyNavigationOn = true;
                Event.SetSelectedGameObject(FirstSelected, new BaseEventData(Event));
            }
        }
    }
}
