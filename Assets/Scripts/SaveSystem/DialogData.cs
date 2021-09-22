using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DialogData
{
    public bool CanDialog;
    public int Money;
    public enum Type { AskAbout, AskAboutLand, AskAboutSeller, Compliment, Offend, Plunder, Hire, Market, Quest, Goodbye, NULL }
    public int HumanType;
    public int[] DialogType;
    public int[] DialogVar;
    public bool Seller;
    public ItemsData[] MarketItem;
    public bool Infinity;

    public string[] Friend;
    public string OnEnemy;

    public int NumQuest;
    public int[] QuestIndex;
    public int[] QuestStage;
    public int[] QuestType;
    public int[] QuestNum;
    public bool[] Reloaded;
    public float[] TimeToNextQuest;
    public float[] ReloadTime;
    public int[] Request;
    public int Killer;
    public int[] KillRequest;
    public int[] CountToRequset;
    public int[] CountCollected;
    public bool[] QuestStart;
    public bool[] QuestDone;
    public string[] RequestName;
    public string[] TargetName;
    public string[] RewardName;
    public int[] Reward;
    public bool[] GotReward;

    public DialogData(Dialogs Dialog)
    {
        MarketItem = new ItemsData[Dialog.MarketItem.Length];
        Infinity = Dialog.AutoFill;
        for (int i = 0; i < MarketItem.Length; i++)
        {
            if (Dialog.MarketItem[i] != null)
                MarketItem[i] = new ItemsData(Dialog.MarketItem[i].GetComponent<Item>());
            else
                MarketItem[i] = null;
        }
        CanDialog = Dialog.CanDialog;
        Money = Dialog.Money;
        HumanType = (int)Dialog.HumanType;
        DialogType = new int[Dialog.Dialog.Length];
        DialogVar = new int[Dialog.Dialog.Length];
        for (int i = 0; i < DialogType.Length; i++)
        {
            DialogType[i] = (int)Dialog.Dialog[i];
            DialogVar[i] = Dialog.DialogVar[i];
        }

        if (Dialog.ai.Enemy != null)
            OnEnemy = Dialog.ai.Enemy.name;
        else
            OnEnemy = "";
        if (Dialog.ai.Killer != null)
            Killer = Dialog.ai.levelSystem.GetAiNum(Dialog.ai.Killer);
        else
            Killer = -1;

        if (Dialog.GetComponent<Quest>() == null)
        {
            return;
        }
        Quest Quest = Dialog.QuestScript;
        NumQuest = Dialog.QuestScript.Quests.Length;
        QuestIndex = new int[Dialog.QuestScript.Quests.Length];
        QuestStage = new int[Dialog.QuestScript.Quests.Length];
        QuestType = new int[Dialog.QuestScript.Quests.Length];
        QuestStart = new bool[Dialog.QuestScript.Quests.Length];
        QuestDone = new bool[Dialog.QuestScript.Quests.Length];
        QuestNum = new int[Dialog.QuestScript.Quests.Length];
        GotReward = new bool[Dialog.QuestScript.Quests.Length];
        Request = new int[Dialog.QuestScript.Quests.Length];
        Reloaded = new bool[Dialog.QuestScript.Quests.Length];
        Reward = new int[Dialog.QuestScript.Quests.Length];
        TimeToNextQuest = new float[Dialog.QuestScript.Quests.Length];
        ReloadTime = new float[Dialog.QuestScript.Quests.Length];
        CountCollected = new int[Dialog.QuestScript.Quests.Length];
        CountToRequset = new int[Dialog.QuestScript.Quests.Length];
        KillRequest = new int[Dialog.QuestScript.Quests.Length];

        for (int i = 0; i < Dialog.QuestScript.Quests.Length; i++)
        {
            QuestIndex[i] = Quest.Quests[i].StoryIndex;
            QuestStage[i] = Quest.Quests[i].StoryStage;

            QuestType[i] = (int)Quest.Quests[i].QuestType;
            QuestStart[i] = Quest.Quests[i].QuestStart;
            QuestDone[i] = Quest.Quests[i].QuestDone;
            QuestNum[i] = Quest.Quests[i].QuestNum;
            GotReward[i] = Quest.Quests[i].GotReward;
            Reloaded[i] = Quest.Quests[i].Reloaded;
            TimeToNextQuest[i] = Quest.Quests[i].TimeToNextQuest;
            ReloadTime[i] = Quest.Quests[i].ReloadQuestTime;
            if (Quest.Quests[i].Reward != null)
                Reward[i] = Quest.Quests[i].Reward.GetComponent<Item>().Index;
            else
                Reward[i] = -1;

            if ((Quest.Quests[i].QuestType == QuestInfo.Type.Request ||
                Quest.Quests[i].QuestType == QuestInfo.Type.TakeItem) && Quest.Quests[i].Target != null)
            {
                Request[i] = Quest.Quests[i].Target.GetComponent<Item>().Index;
            }
            else
            {
                Request[i] = -1;
            }
            CountCollected[i] = Quest.Quests[i].CountCollected;
            CountToRequset[i] = Quest.Quests[i].CountToRequset;
            if (Quest.Quests[i].QuestType == QuestInfo.Type.Kill && Quest.Quests[i].Target != null)
                KillRequest[i] = Dialog.ai.levelSystem.GetAiNum(Quest.Quests[i].Target);
            else
                KillRequest[i] = -1;
        }
    
    }
}

