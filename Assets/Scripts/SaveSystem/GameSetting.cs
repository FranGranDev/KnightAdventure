using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    public static int SystemLanguage;
    public bool[] SlotFull;
    public string[] SlotName;

    public void Save()
    {
        SaveSystem.SaveSettingData(this);
    }
    public void Load()
    {
        if (SaveSystem.LoadSettingData() == null)
            return;
        GameSettingData data = SaveSystem.LoadSettingData();
        SystemLanguage = data.SystemLanguage;
        SlotFull = data.SlotFull;
        SlotName = data.SlotName;
    }
}
[System.Serializable]
public class GameSettingData
{
    public int SystemLanguage;
    public bool[] SlotFull;
    public string[] SlotName;

    public GameSettingData(GameSetting Data)
    {
        SystemLanguage = GameSetting.SystemLanguage;
        SlotFull = Data.SlotFull;
        SlotName = Data.SlotName;
    }
}