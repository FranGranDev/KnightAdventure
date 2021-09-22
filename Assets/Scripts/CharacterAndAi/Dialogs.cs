using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogs : MonoBehaviour
{
    public int Money;
    public bool CanDialog = false;
    private Transform Player;
    public enum TextType { Peasant, PeasantLady, Beggar, Solider, Knight, Seller, Bandit, Master, MasterLady }
    public TextType HumanType;
    public enum Type {AskAbout,AskAboutLand, AskAboutSeller,Compliment, Offend, Plunder, Hire, Market, Quest, TakeMessage, MainStory, Goodbye, NULL }
    public Type[] Dialog;
    public int[] DialogVar;
    public enum HobbieType {Work, Drink, Relax, Null};
    public HobbieType Hobbie;
    public enum ProblemType {Poorness, Age, Funny, Null };
    public ProblemType Problem;
    public bool inDialog = false;

    public Transform AiItemPlace;
    public GameObject[] MarketItem = new GameObject[30];
    public bool AutoFill;

    public Transform Target;
    private IndexSystem IndexSystem;
    public UIScript Ui;
    public Quest QuestScript;
    public Transform Hand;
    public Ai ai;

    private void Awake()
    {
        IndexSystem = Resources.Load<IndexSystem>("IndexSystem");
        Ui = GameObject.Find("MainUI").GetComponent<UIScript>();
        ai = GetComponent<Ai>();
        if(GetComponent<Quest>() != null)
        {
            QuestScript = GetComponent<Quest>();
        }
        Hand = transform.GetChild(1).GetChild(0);
    }
    void Start()
    {
        if (QuestScript != null)
        {
            QuestScript.GetDialogUi(transform.GetComponent<Dialogs>(), Ui);
        }
        if(AutoFill)
        {
            StartCoroutine(FillMarket());
        }
    }
    
    public void Load(int num, int Slot)
    {
        DialogData Data = SaveSystem.LoadLevel(Slot).DialogOnSceneData[num];
        Load(Data);
    }
    public void Load(DialogData Data)
    {
        if (Data.OnEnemy != "")
            ai.Enemy = GameObject.Find(Data.OnEnemy).transform;
        else
            ai.Enemy = null;
        MarketItem = new GameObject[Data.MarketItem.Length];
        AutoFill = Data.Infinity;
        for (int i = 0; i < Data.MarketItem.Length; i++)
        {
            if (Data.MarketItem[i] != null)
            {
                int index = Data.MarketItem[i].index;
                MarketItem[i] = IndexSystem.Item(index);
                MarketItem[i].GetComponent<Item>().Load(Data.MarketItem[i]);
            }
        }
        Money = Data.Money;
        Dialog = new Type[Data.DialogType.Length];
        DialogVar = new int[Data.DialogType.Length];
        CanDialog = Data.CanDialog;

        HumanType = (TextType)Data.HumanType;
        for (int i = 0; i < Dialog.Length; i++)
        {
            Dialog[i] = (Type)Data.DialogType[i];
            DialogVar[i] = Data.DialogVar[i];
        }

        if (GetComponent<Quest>() == null)
            return;
        QuestScript = transform.GetComponent<Quest>();
        QuestScript.Quests = new QuestInfo[Data.NumQuest];
        for (int i = 0; i < Data.NumQuest; i++)
        {
            QuestScript.Quests[i] = new QuestInfo(Data);
            QuestScript.Quests[i].QuestType = (QuestInfo.Type)Data.QuestType[i];
            QuestScript.Quests[i].StoryIndex = Data.QuestIndex[i];
            QuestScript.Quests[i].StoryStage = Data.QuestStage[i];
            QuestScript.Quests[i].QuestStart = Data.QuestStart[i];
            QuestScript.Quests[i].QuestDone = Data.QuestDone[i];
            QuestScript.Quests[i].QuestNum = Data.QuestNum[i];
            QuestScript.Quests[i].GotReward = Data.GotReward[i];
            QuestScript.Quests[i].Reloaded = Data.Reloaded[i];
            QuestScript.Quests[i].TimeToNextQuest = Data.TimeToNextQuest[i];
            QuestScript.Quests[i].ReloadQuestTime = Data.ReloadTime[i];
            if (Data.Reward[i] > -1)
                QuestScript.Quests[i].Reward = IndexSystem.Item(Data.Reward[i]);
            else
                QuestScript.Quests[i].Reward = null;
            if (Data.Request[i] > -1)
            {
                QuestScript.Quests[i].Target = IndexSystem.Item(Data.Request[i]);
            }
            else if (Data.KillRequest[i] > -1)
            {
                QuestScript.Quests[i].Target = ai.levelSystem.GetAi(Data.KillRequest[i]);
            }
            else
            {
                QuestScript.Quests[i].Target = null;
            }
            QuestScript.Quests[i].CountToRequset = Data.CountToRequset[i];
            QuestScript.Quests[i].CountCollected = Data.CountCollected[i];

            if (Data.Killer != -1)
            {
                ai.Killer = ai.levelSystem.GetAi(Data.Killer);
                ai.AddToQuest(ai.Killer);
            }
            else
                ai.Killer = null;
        }
    }

    public void StartDialog(Transform obj)
    {
        if (!CanDialog)
            return;
        Player = obj;
        Player.GetComponent<Character>().StartDialog(transform);
        if (ai != null)
        {
            ai.StartDialog();
        }
        inDialog = true;
        Ui.StartDialog(transform.root);
        for (int i = 0; i < Dialog.Length; i++)
        {
            string name = GetButtonName(i, (int)HumanType);
            Ui.SetDialogButton(i, name, (int)Dialog[i]);
            if (Dialog[i] == Type.Quest && QuestScript.Quests.Length > DialogVar[i])
            {
                QuestInfo.Type questtype = QuestScript.Quests[DialogVar[i]].QuestType;
                if (questtype == QuestInfo.Type.StoryTalk)
                {
                    Ui.SetDialogButtonColor(i, Color.blue);
                }
                else if (questtype == QuestInfo.Type.StartStory)
                {
                    Ui.SetDialogButtonColor(i, Color.red);
                }
                else
                {
                    Ui.SetDialogButtonColor(i, Color.white);
                }
            }
            else
            {
                Ui.SetDialogButtonColor(i, Color.white);
            }
        }
    }
    public void StartText()
    {
        if (!CanDialog)
            return;
        Ui.PrintText(LanguagesSystem.Language.DialogStartText[(int)HumanType]);
    }

    public void AskAbout(int num)
    {
        string text = LanguagesSystem.Language.Dialog[0].DialogText[(int)HumanType];
        string hobbie = LanguagesSystem.Language.LifeStaffs[(int)HumanType].Hobbie[(int)Hobbie];
        text = text.Replace("z1", hobbie);
        string problem = LanguagesSystem.Language.LifeStaffs[(int)HumanType].Problems[(int)Hobbie];
        text = text.Replace("z2", problem);
        Ui.PrintText(text);
    }
    public void AskAboutLand(int num)
    {
        string text = LanguagesSystem.Language.Dialog[1].DialogText[(int)HumanType];
        string land = LanguagesSystem.Language.LevelName[NowData.GlobalLevelNum];
        text = text.Replace("z1", land);
        Ui.PrintText(text);
    }
    public void AskAboutSeller(int num)
    {
        string text = "";
        if (Target == null)
        {
            int lenght = LanguagesSystem.Language.LifeStaffs[(int)HumanType].BadWord.Length;
            string name = LanguagesSystem.Language.LifeStaffs[(int)HumanType].BadWord[Random.Range(0, lenght)];
            text = LanguagesSystem.Language.Dialog[2].DialogNull[(int)HumanType];
            text = text.Replace("z1", name);
        }
        else
        {
            int lenght = LanguagesSystem.Language.LifeStaffs[(int)HumanType].BadWord.Length;
            string name = LanguagesSystem.Language.LifeStaffs[(int)HumanType].BadWord[Random.Range(0, lenght)];
            text = LanguagesSystem.Language.Dialog[2].DialogText[(int)HumanType];
            text = text.Replace("z1", name);
            ai.levelSystem.MainCamera.ShowTargetForTime(Target, 3f);
        }
        Ui.PrintText(text);
    }
    public void Compliment(int num)
    {
        Ui.PrintText(LanguagesSystem.Language.Dialog[3].DialogText[(int)HumanType]);
    }
    public void Offend(int num)
    {
        Ui.PrintText(LanguagesSystem.Language.Dialog[4].DialogText[(int)HumanType]);
        transform.SendMessage("GetEnemy", Player);
    }
    public void Plunder(int num)
    {
        int Rand = Random.Range(0, 3);
        if (Rand == 0)
        {
            Ui.PrintText(LanguagesSystem.Language.Dialog[5].DialogText[(int)HumanType]);
            Player.GetComponent<Character>().EndDialog(-1);
            for (int i = 0; i < MarketItem.Length; i++)
            {
                if (MarketItem[i] != null)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        ai.LetOutItem(MarketItem[i].transform);
                    }
                }
            }
            ai.GetEnemy(Player);
            DeleteDialogButton(num);
        }
        else
        {
            Ui.PrintText(LanguagesSystem.Language.Dialog[5].DialogAlt[(int)HumanType]);
            Player.GetComponent<Character>().EndDialog(-1);
            ai.StartCoroutine(ai.CallHelp(1f));
            ai.GetEnemy(Player);
            DeleteDialogButton(num);
        }
    }
    public void Hire(int num)
    {
        string text = LanguagesSystem.Language.Dialog[6].DialogText[(int)HumanType];
        Ui.PrintText(text);
    }
    public void TakeMessage(int num)
    {
        string text = LanguagesSystem.Language.Dialog[7].DialogText[(int)HumanType];
    }
    public void OpenMarket(int num)
    {
        Ui.AiMoney = Money;
        Ui.ClearMarket();
        for(int i = 0; i < MarketItem.Length; i++)
        {
            Ui.GetMarketItem(i, MarketItem[i]);
        }
        Ui.PrintText(/*LanguagesSystem.Language.Dialog[8].DialogText[(int)HumanType]*/ "Opened");
        Ui.TurnMarket(true);
    }
    public void MainStory(int num)
    {
        QuestTalk(num);
    }
    public void EndDialog(int num)
    {
        if (num != -1)
        {
            Ui.PrintText(LanguagesSystem.Language.Dialog[11].DialogText[(int)HumanType]);
        }
        Ui.EndDialog();
        inDialog = false;
        GetComponent<Ai>().EndDialog();
    }
    public void QuestTalk(int num)
    {
        if (QuestScript != null)
        {
            //QuestScript.MainQuestTalk(num);
        }
        else
        {
            Ui.PrintText(LanguagesSystem.Language.Dialog[8].DialogNull[(int)HumanType]);
        }
    }
    public void QuestKilled()
    {
        if (QuestScript != null)
        {
            //QuestScript.MainQuestKilled();
        }
    }
    public void StartQuest(int num)
    {
        if (QuestScript != null)
        {
            QuestScript.Start(num, DialogVar[num]);
        }
        else
        {
            Ui.PrintText(LanguagesSystem.Language.Dialog[num].DialogText[(int)HumanType]);
        }
    }

    public string GetButtonName(int num, int humantype)
    {
        string name = LanguagesSystem.Language.Dialog[(int)Dialog[num]].ButtonText[humantype];
        if (num > Dialog.Length - 1)
            return name;
        switch (Dialog[num])
        {
            case Type.Quest:
                if (DialogVar[num] != -1 && DialogVar[num] < QuestScript.Quests.Length)
                {
                    if(QuestScript.Quests[DialogVar[num]].QuestType == QuestInfo.Type.StoryTalk)
                    {
                        int Index = QuestScript.Quests[DialogVar[num]].StoryIndex;
                        int Stage = QuestScript.Quests[DialogVar[num]].StoryStage;
                        return LanguagesSystem.Language.GlobalQuest[Index].Part[Stage].ButtonText;
                    }
                } 

                break;
        }
        return name;
    }
    public void AddDialogButton(int num, int type, int value)
    {
        Type[] tempdialog = Dialog;
        int[] tempvar = DialogVar;
        Dialog = new Type[Dialog.Length + 1];
        DialogVar = new int[Dialog.Length];
        for (int i = 0; i < Dialog.Length - 1; i++)
        {
            Dialog[i] = tempdialog[i];
            DialogVar[i] = tempvar[i];
        }
        for (int i = Dialog.Length - 1; i > num; i--)
        {
            Dialog[i] = Dialog[i - 1];
            DialogVar[i] = tempvar[i - 1];
            Ui.SetDialogButton(i, GetButtonName(i, (int)HumanType), (int)Dialog[i]);
        }
        Dialog[num] = (Type)type;
        DialogVar[num] = value;
        Ui.SetDialogButton(num, GetButtonName(num, type), type);
        if(Dialog[num] == Type.Quest && QuestScript.Quests.Length > DialogVar[num])
        {
            QuestInfo.Type questtype = QuestScript.Quests[DialogVar[num]].QuestType;
            if(questtype == QuestInfo.Type.StoryTalk)
            {
                Ui.SetDialogButtonColor(num, Color.blue);
            }
            else if(questtype == QuestInfo.Type.StartStory)
            {
                Ui.SetDialogButtonColor(num, Color.red);
            }
            else
            {
                Ui.SetDialogButtonColor(num, Color.white);
            }
        }
        else
        {
            Ui.SetDialogButtonColor(num, Color.white);
        }

    }
    public void DeleteDialogButton(int num)
    {
        for (int i = num; i < Dialog.Length - 1; i++)
        {
            Dialog[i] = Dialog[i + 1];
            DialogVar[i] = DialogVar[i + 1];
            Ui.SetDialogButton(i, GetButtonName(i, (int)HumanType), (int)Dialog[i]);
        }
        Type[] tempdialog = Dialog;
        int[] tempvar = DialogVar;
        Dialog = new Type[Dialog.Length - 1];
        DialogVar = new int[Dialog.Length];
        for(int i = 0; i < Dialog.Length; i++)
        {
            Dialog[i] = tempdialog[i];
            DialogVar[i] = tempvar[i];
            Ui.SetDialogButton(i, GetButtonName(i, (int)HumanType), (int)Dialog[i]);
            if (Dialog[num] == Type.Quest && QuestScript.Quests.Length > DialogVar[num])
            {
                QuestInfo.Type questtype = QuestScript.Quests[DialogVar[num]].QuestType;
                if (questtype == QuestInfo.Type.StoryTalk)
                {
                    Ui.SetDialogButtonColor(num, Color.blue);
                }
                else if (questtype == QuestInfo.Type.StartStory)
                {
                    Ui.SetDialogButtonColor(num, Color.red);
                }
                else
                {
                    Ui.SetDialogButtonColor(num, Color.white);
                }
            }
            else
            {
                Ui.SetDialogButtonColor(num, Color.white);
            }
        }
        Ui.SetDialogButton(Dialog.Length, "", (int)Dialogs.Type.NULL);
    }
    public void SetDialogButton(int num, string text, int type)
    {
        Dialog[num] = (Type)type;
        Ui.SetDialogButton(num, text, type);
    }

    private IEnumerator FillMarket()
    {
        while(true)
        {
            yield return new WaitForSeconds(10f);
            int sum = 0;
            for(int i = 0; i < MarketItem.Length; i++)
            {
                if(MarketItem[i] == null)
                {
                    sum++;
                }
            }
            if(sum > 5)
            {
                GameObject item = Instantiate(IndexSystem.GetRandomFood());
                GetMarketItem(item.transform);
            }
        }
    }

    public void GetMarketItem(Transform item)
    {
        for(int i = 0; i < MarketItem.Length; i++)
        {
            if(MarketItem[i] == null)
            {
                MarketItem[i] = item.gameObject;
                if (MarketItem[i] != null)
                {
                    MarketItem[i].transform.parent = Hand;
                    MarketItem[i].transform.position = Hand.position;
                    MarketItem[i].transform.rotation = Hand.rotation;
                    MarketItem[i].gameObject.SetActive(false);
                }
                Ui.GetMarketItem(i, item.gameObject);
                break;
            }
        }
    }
    public int GetFreeSlot()
    {
        int sum = 0;
        for (int i = 0; i < MarketItem.Length; i++)
        {
            if(MarketItem[i] == null)
            {
                sum++;
            }
        }
        return sum;
    }
    public void GetMarketItem(int i, Transform item)
    {
        if (item != null)
        {
            MarketItem[i] = item.gameObject;
            MarketItem[i].transform.parent = Hand;
            MarketItem[i].transform.position = Hand.position;
            MarketItem[i].transform.rotation = Hand.rotation;
            MarketItem[i].gameObject.SetActive(false);
        }
        else
        {
            MarketItem[i] = null;
        }
        Ui.GetMarketItem(i, MarketItem[i]);
    }

    void FixedUpdate()
    {
        if (!CanDialog && inDialog)
            EndDialog(-1);
    }
}
