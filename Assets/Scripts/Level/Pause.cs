using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;

public class Pause : MonoBehaviour
{
    public LevelSystem levelSystem;
    public string[] ExistSlot;


    public enum SureWindow { Exit, Save, Load};
    public SureWindow Sure;

    public TextMeshProUGUI[] Text;
    public static bool OnPause = false;
    public Image Panel;
    public GameObject Canvas;
   
    public GameObject MainMenu;
    public TextMeshProUGUI[] MainText;

    public GameObject SaveMenu;
    private int SlotToSave;
    public TMP_InputField SaveName;
    public TextMeshProUGUI[] SaveSlot;

    public GameObject LoadMenu;
    private int SlotToLoad;
    public TextMeshProUGUI[] LoadSlot;

    public GameObject SureMenu;

    public void PauseGame()
    {
        OnPause = true;
        Canvas.SetActive(true);
        Time.timeScale = 0;
    }
    public void PlayGame()
    {
        OnPause = false;
        Canvas.SetActive(false);
        Time.timeScale = 1;
    }

    public void SaveGameButton()
    {
        MainMenu.SetActive(false);
        SaveMenu.SetActive(true);
    }
    public void GoMainMenu()
    {
        MainMenu.SetActive(true);
        SaveMenu.SetActive(false);
        LoadMenu.SetActive(false);
    }
    public void GoSaveMenu()
    {
        SaveMenu.SetActive(true);
        MainMenu.SetActive(false);

        for(int i = 0; i < SaveSlot.Length; i++)
        {
            SaveSlot[i].color = Color.gray;
            SaveSlot[i].text = "Empty Slot";
        }
        ExistSlot = Directory.GetDirectories(Application.persistentDataPath);
        for(int i = 0; i < ExistSlot.Length; i++)
        {
            SaveSlot[i].color = Color.white;
            SaveSlot[i].text = ExistSlot[i].Replace(Application.persistentDataPath + @"\", "");
        }
    }
    public void GoLoadMenu()
    {
        MainMenu.SetActive(false);
        LoadMenu.SetActive(true);

        for (int i = 0; i < SaveSlot.Length; i++)
        {
            LoadSlot[i].color = Color.gray;
            LoadSlot[i].text = "Empty Slot";
        }
        ExistSlot = Directory.GetDirectories(Application.persistentDataPath);
        for (int i = 0; i < ExistSlot.Length; i++)
        {
            SaveSlot[i].color = Color.white;
            LoadSlot[i].text = ExistSlot[i].Replace(Application.persistentDataPath + @"\", "");
        }
    }
    public void Exit()
    {
        Sure = SureWindow.Exit;
        SureMenu.SetActive(true);
    }


    public void SaveTo(int Slot)
    {
        if (SaveName.text != "")
        {
            string CopyFolder = Application.persistentDataPath + "/" + NowData.SlotName;
            string[] Files = new string[0];
            if (Directory.Exists(CopyFolder))
            {
                Files = Directory.GetFiles(CopyFolder);
            }
            NowData.PrevSlotName = NowData.SlotName;
            NowData.SlotName = SaveName.text;
            string NewFolder = Application.persistentDataPath + "/" + NowData.SlotName;
            if (!Directory.Exists(NewFolder))
            {
                Directory.CreateDirectory(NewFolder);
            }
            if (Directory.Exists(CopyFolder) && CopyFolder != NewFolder)
            {
                for (int i = 0; i < Files.Length; i++)
                {
                    string NewDest = NewFolder + "/" + Files[i].Replace(CopyFolder, "");
                    File.Copy(Files[i], NewDest);
                }
            }
            NowData.LoadPlayerPosition = true;
            levelSystem.Save(NowData.LevelNum);
            NowData.SlotName = NowData.PrevSlotName;
            GoMainMenu();
            GoSaveMenu();
        }
    }
    public void Load(int Slot)
    {
        string Name = ExistSlot[Slot].Replace(Application.persistentDataPath + @"\", "");
        NowData.LoadPlayerPosition = true;
        NowData.SlotName = Name;
        NowData.Load();
        NowData.Save();
        PlayGame();
        SceneManager.LoadScene(NowData.LevelNum);
    }

    public void SureYes()
    {
        SureMenu.SetActive(false);
        switch(Sure)
        {
            case SureWindow.Exit:
                levelSystem.Exit();
                break;
            case SureWindow.Load:
                NowData.SlotName = ExistSlot[SlotToLoad];
                NowData.Load();
                NowData.Save();
                SceneManager.LoadScene(NowData.LevelNum);
                break;
            case SureWindow.Save:

                break;
        }
    }
    public void SureNo()
    {
        SureMenu.SetActive(false);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(OnPause)
            {
                GoMainMenu();
                PlayGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
}
