using System.Collections;
using UnityEngine;

[System.Serializable]
public class NowData : MonoBehaviour
{
    public static string SlotName;
    public static string PrevSlotName;

    public static int LevelNum;
    public static int PrevLevelNum;
    public static int GlobalLevelNum;

    public static bool LoadPlayerPosition;

    public static int HouseIndex;
    public static bool inHouse;

    public static int QuestIndex;
    public static bool[] QuestStarted;
    public static bool[] QuestDone;

    public static string PlayerName;
    public static bool FirstLoaded;

    public static int Hair;
    public static float[] HairColor;
    public static int Face;
    public static int Torso;
    public static float[] SkinColor;

    

    public static void Save()
    {
        SaveSystem.SaveNowData();
    }
    public static void Load()
    {
        if (SaveSystem.LoadNowData() != null)
        {
            SystemData data = SaveSystem.LoadNowData();
            LevelNum = data.LevelNum;
            PrevLevelNum = data.PrevLevelNum;
            GlobalLevelNum = data.GlobalLevelNum;
            HouseIndex = data.HouseIndex;
            inHouse = data.inHouse;
            LoadPlayerPosition = data.LoadPlayerPosition;
            QuestIndex = data.QuestIndex;
        }
    }
}
[System.Serializable]
public class SystemData
{
    public int QuestIndex;
    public int LevelNum;
    public int PrevLevelNum;
    public int GlobalLevelNum;
    public int HouseIndex;
    public bool LoadPlayerPosition;
    public bool inHouse;

    public SystemData()
    {
        QuestIndex = NowData.QuestIndex;
        LevelNum = NowData.LevelNum;
        PrevLevelNum = NowData.PrevLevelNum;
        GlobalLevelNum = NowData.GlobalLevelNum;
        HouseIndex = NowData.HouseIndex;
        inHouse = NowData.inHouse;
        LoadPlayerPosition = NowData.LoadPlayerPosition;
    }
}
