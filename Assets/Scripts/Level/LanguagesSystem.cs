using UnityEngine;
using System.Collections;
using System.IO;

public class LanguagesSystem : MonoBehaviour
{
    private string Json;
    private string NowLanguage = "ru_RU";
    public string[] LanguagesArray = {"en_US", "ru_RU", "by_BY" };
    public static LanguageData Language = new LanguageData();

    private void OnEnable()
    {
        LoadLanguage(GameSetting.SystemLanguage);
    }

    public void LoadLanguage(int Num)
    {
        if (Num > LanguagesArray.Length)
        {
            GameSetting.SystemLanguage = 0;
            NowLanguage = LanguagesArray[0];
            Debug.Log("Missing Language.Set to English");
        }
        else
        {
            GameSetting.SystemLanguage = Num;
            NowLanguage = LanguagesArray[Num];
        }
        Json = File.ReadAllText(Application.streamingAssetsPath + "/Languages/" + NowLanguage + ".json");
        Language = JsonUtility.FromJson<LanguageData>(Json);

       
    }
}
[System.Serializable]
public class LanguageData
{

    public string[] ItemName;
    public string[] ItemGenitiveName;
    public string[] ItemPluralGenitiveName;
    public string[] ItemPluralName;
    public string[] ObjectName;

    public string[] DialogStartText;
    public LanguageDialogData[] Dialog;
    public LifeStaff[] LifeStaffs;
    public LanguageGlobalQuestData[] GlobalQuest;
    public LanguageLocalQuestData[] LocalQuest;

    public string[] Names;
    public string[] LastNames;

    public string[] LevelName;
    public string[] Fraction;
    public string[] ManType;

    public string[] MenuMain;
    public string[] MenuLoadGame;
    public string MenuEmptySlot;
    public string MenuDelete;
    public string[] MenuNewGame;
    public string[] MenuSettings;
    public string[] MenuLanguage;

    public NamePlate[] NamePlate;

    public string[] Market;

}
[System.Serializable]
public class LanguageGlobalQuestData
{
    public string StoryName;
    public LanguageQuestPart[] Part;
}
[System.Serializable]
public class LanguageQuestPart
{
    public string Tips;
    public string AiText;
    public string ButtonText;
    public string[] AiCameText;
}
[System.Serializable]
public class LanguageLocalQuestData
{
    public string Name;
    public string[] Start;
    public string[] Started;
    public string[] Done;
    public string[] ButtonDone;
    public string[] Null;
}
[System.Serializable]
public class LanguageDialogData
{
    public string[] ButtonText;
    public string[] DialogAlt;
    public string[] DialogText;
    public string[] DialogNull;
}
[System.Serializable]
public class LifeStaff
{
    public string Name;
    public string[] Hobbie;
    public string[] Problems;
    public string[] BadWord;
    public string[] GoodWord;
}
[System.Serializable]
public class NamePlate
{
    public string[] Text;
}
