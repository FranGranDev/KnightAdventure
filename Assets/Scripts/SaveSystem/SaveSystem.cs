using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem{
    public static DirectoryInfo LastCreatedSlot;

    public static void SavePlayer(Character character)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string DirectoryPath = Application.persistentDataPath + "/" + NowData.SlotName;
        if (!Directory.Exists(DirectoryPath))
        {
            LastCreatedSlot = Directory.CreateDirectory(DirectoryPath);
        }
        string path = Application.persistentDataPath + "/" + NowData.SlotName + "/PlayerData.blin";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(character);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static void DeletePlayerSave()
    {
        string path = Application.persistentDataPath + "/" + NowData.SlotName + "/PlayerData.blin";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/" + NowData.SlotName + "/PlayerData.blin";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    public static void HouseTemporarySave(InHouse House)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string DirectoryPath = Application.persistentDataPath + "/" + NowData.SlotName;
        if (!Directory.Exists(DirectoryPath))
        {
            LastCreatedSlot = Directory.CreateDirectory(DirectoryPath);
        }
        string path = Application.persistentDataPath + "/" + NowData.SlotName + "/HouseTempData.blin";
        
        FileStream stream = new FileStream(path, FileMode.Create);

        HouseDate data = new HouseDate(House);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static HouseDate HouseTemporaryLoad()
    {
        string path = Application.persistentDataPath + "/" + NowData.SlotName + "/HouseTempData.blin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            
            HouseDate data = formatter.Deserialize(stream) as HouseDate;
            stream.Close();
            return data;
        }
        else
        {
            return null;
        }
    }
    public static void HouseTemporaryDelete()
    {
        string path = Application.persistentDataPath + "/" + NowData.SlotName + "/HouseTempData.blin";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static void SaveLevel(LevelSystem level, int Slot)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string DirectoryPath = Application.persistentDataPath + "/" + NowData.SlotName;
        if (!Directory.Exists(DirectoryPath))
        {
            LastCreatedSlot = Directory.CreateDirectory(DirectoryPath);
        }
        string path = Application.persistentDataPath + "/" + NowData.SlotName +  "/LevelData" + Slot + ".blin";
        FileStream stream = new FileStream(path, FileMode.Create);

        LevelData data = new LevelData(level, Slot);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static void DeleteLevelSave(int Slot)
    {
        string path = Application.persistentDataPath + "/" + NowData.SlotName + "/LevelData" + Slot + ".blin";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
    public static LevelData LoadLevel(int Slot)
    {
        string path = Application.persistentDataPath + "/" + NowData.SlotName + "/LevelData" + Slot + ".blin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            LevelData data = formatter.Deserialize(stream) as LevelData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    public static void SaveNowData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string DirectoryPath = Application.persistentDataPath + "/" + NowData.SlotName;
        if (!Directory.Exists(DirectoryPath))
        {
            LastCreatedSlot = Directory.CreateDirectory(DirectoryPath);
        }
        string path = Application.persistentDataPath + "/" + NowData.SlotName + "/NowData.blin";
        FileStream stream = new FileStream(path, FileMode.Create);

        SystemData data = new SystemData();

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static SystemData LoadNowData()
    {
        string path = Application.persistentDataPath + "/" + NowData.SlotName + "/NowData.blin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SystemData data = formatter.Deserialize(stream) as SystemData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    public static void SaveSettingData(GameSetting gameSetting)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/SettingData.blin";
        FileStream stream = new FileStream(path, FileMode.Create);

        GameSettingData data = new GameSettingData(gameSetting);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static GameSettingData LoadSettingData()
    {
        string path = Application.persistentDataPath + "/SettingData.blin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameSettingData data = formatter.Deserialize(stream) as GameSettingData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    public static void CreateTempSlot()
    {
        string path = Application.persistentDataPath + @"/" + "TempSave";
        if (!Directory.Exists(path))
        {
            LastCreatedSlot = Directory.CreateDirectory(path);
        }
    }
    public static void CreateSlot(string SlotName)
    {
        string path = Application.persistentDataPath + "/" + SlotName;
        if (!Directory.Exists(path))
        {
            LastCreatedSlot = Directory.CreateDirectory(path);
        }
    }
    public static bool ExistSlot(string SlotName)
    {
        string path = Application.persistentDataPath + "/" + SlotName;
        if (Directory.Exists(SlotName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void DeleteTempSlot()
    {
        string path = Application.persistentDataPath + "/TempSave";
        if (Directory.Exists(path))
        {

            Directory.Delete(path, true);
        }
    }
    public static void DeleteSlot(string SlotName)
    {
        string path = Application.persistentDataPath + "/" + SlotName;
        if (Directory.Exists(path))
        {
     
            Directory.Delete(path, true);
        }
    }
}
