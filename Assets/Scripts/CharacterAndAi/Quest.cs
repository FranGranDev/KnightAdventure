using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{

    public QuestInfo[] Quests;

    private camerascript Camera;
    private LevelSystem levelSystem;
    private IndexSystem IndexSystem;
    private Dialogs Dialog;
    private UIScript Ui;

    void Start()
    {
        Dialog = GetComponent<Dialogs>();
        IndexSystem = Resources.Load<IndexSystem>("IndexSystem");
        levelSystem = GameObject.Find("LevelSystem").GetComponent<LevelSystem>();
        Camera = GameObject.Find("MainCamera").GetComponent<camerascript>();

        StartCoroutine(CheckForReload());
    }

    public void GetDialogUi(Dialogs x, UIScript y)
    {
        Dialog = x;
        Ui = y;
    }
    public void Start(int num, int questnum)
    {
        if (Quests[questnum].QuestType == QuestInfo.Type.Kill && !Quests[questnum].QuestStart && Quests[questnum].Target == null)
        {
            Ui.PrintText(LanguagesSystem.Language.LocalQuest[(int)Quests[questnum].QuestType].Null[3]);
            return;
        }
        if (Quests[questnum].QuestType == QuestInfo.Type.Request && !Quests[questnum].QuestStart && Quests[questnum].Target == null)
        {
            Ui.PrintText(LanguagesSystem.Language.LocalQuest[(int)Quests[questnum].QuestType].Null[3]);
            return;
        }
        Quests[questnum].QuestNum = num;
        switch (Quests[questnum].QuestType)
        {
            case QuestInfo.Type.Request:
                if (Quests[questnum].QuestDone)
                {
                    string text = LanguagesSystem.Language.LocalQuest[(int)Quests[questnum].QuestType].Done[(int)Dialog.HumanType];
                    Ui.PrintText(text);
                    Dialog.DeleteDialogButton(num);
                    if (!Quests[questnum].GotReward)
                    {
                        levelSystem.DeleteRequsetedItem(Quests[questnum].Target.GetComponent<Item>().Index);
                        if (Quests[questnum].Reward != null)
                        {
                            GameObject myReward = Instantiate(Quests[questnum].Reward, transform.position, transform.rotation, null);
                            myReward.SendMessage("RandomMove");
                        }
                        Quests[questnum].Reward = null;
                        Quests[questnum].Target = null;
                        Quests[questnum].GotReward = true;
                        if (Quests[questnum].Reloaded)
                        {
                            Quests[questnum].TimeToNextQuest = Time.time + Quests[questnum].ReloadQuestTime;
                        }
                    }
                    return;
                }
                if (!Quests[questnum].QuestStart)
                {
                    string item = LanguagesSystem.Language.LocalQuest[(int)Quests[questnum].QuestType].Null[0];
                    if(item != "")
                    {
                        item = Quests[questnum].CountToRequset + " " + Quests[questnum].Target.name;
                    }
                    string itemReward = LanguagesSystem.Language.LocalQuest[(int)Quests[questnum].QuestType].Null[0];
                    if (Quests[questnum].Target != null)
                    {
                        if (Quests[questnum].CountToRequset > 1)
                        {
                            int Index = Quests[questnum].Target.GetComponent<Item>().Index;
                            item = Quests[questnum].CountToRequset + " " + LanguagesSystem.Language.ItemPluralGenitiveName[Index];
                        }
                        else
                        {
                            int Index = Quests[questnum].Target.GetComponent<Item>().Index;
                            item = LanguagesSystem.Language.ItemGenitiveName[Index];
                        }
                    }
                    if (Quests[questnum].Reward != null)
                    {
                        itemReward = LanguagesSystem.Language.ItemName[Quests[questnum].Reward.GetComponent<Item>().Index];
                    }
                    string text = LanguagesSystem.Language.LocalQuest[(int)Quests[questnum].QuestType].Start[(int)Dialog.HumanType];
                    text = text.Replace("z1", item);
                    text = text.Replace("z2", itemReward);
                    levelSystem.AddRequsetedItem(Quests[questnum].Target.GetComponent<Item>().Index);
                    Quests[questnum].QuestStart = true;
                    Ui.PrintText(text);
                    return;
                }
                else if(Quests[questnum].QuestStart)
                {
                    string item = LanguagesSystem.Language.LocalQuest[(int)Quests[questnum].QuestType].Null[0];
                    if (Quests[questnum].Target != null)
                    {
                        if (Quests[questnum].CountToRequset > 1)
                        {
                            int Index = Quests[questnum].Target.GetComponent<Item>().Index;
                            item = Quests[questnum].CountToRequset + " " + LanguagesSystem.Language.ItemPluralGenitiveName[Index];
                        }
                        else
                        {
                            int Index = Quests[questnum].Target.GetComponent<Item>().Index;
                            item = LanguagesSystem.Language.ItemGenitiveName[Index];
                        }
                    }
                    string text = LanguagesSystem.Language.LocalQuest[(int)Quests[questnum].QuestType].Started[(int)Dialog.HumanType];
                    text = text.Replace("z1", item);
                    Ui.PrintText(text);
                    return;
                }
                break;

            case QuestInfo.Type.Kill:
                if (Quests[questnum].QuestDone)
                {
                    string text = LanguagesSystem.Language.LocalQuest[(int)Quests[questnum].QuestType].Done[(int)Dialog.HumanType];
                    Ui.PrintText(text);
                    Ui.DeleteDialogButton(num);
                    Dialog.DeleteDialogButton(num);
                    if (!Quests[questnum].GotReward)
                    {
                        if (Quests[questnum].Reward != null)
                        {
                            Quests[questnum].Reward = Instantiate(Quests[questnum].Reward, transform.position, transform.rotation, null);
                            Quests[questnum].Reward.SendMessage("RandomMove");
                        }
                        Quests[questnum].GotReward = true;
                    }
                    return;
                }
                if (!Quests[questnum].QuestStart)
                {
                    Quests[questnum].Target.GetComponent<Ai>().AddToQuest(gameObject);
                    Camera.ShowTargetForTime(Quests[questnum].Target.transform, 3f);
                    string text = LanguagesSystem.Language.LocalQuest[(int)Quests[questnum].QuestType].Start[(int)Dialog.HumanType];
                    text = text.Replace("z1", Quests[questnum].Target.GetComponent<Ai>().GetName());
                    if (Quests[questnum].Reward != null)
                    {
                        text = text.Replace("z2", Quests[questnum].Reward.GetComponent<Item>().Name);
                    }
                    else
                    {
                        text = text.Replace("z2", LanguagesSystem.Language.LocalQuest[(int)Quests[questnum].QuestType].Null[0]);
                    }
                    Ui.PrintText(text);
                    Quests[questnum].QuestStart = true;
                    return;
                }
                else if (Quests[questnum].QuestStart)
                {
                    string text = LanguagesSystem.Language.LocalQuest[(int)Quests[questnum].QuestType].Started[(int)Dialog.HumanType];
                    text = text.Replace("z1", Quests[questnum].Target.GetComponent<Ai>().GetName());
                    Ui.PrintText(text);
                }
                break;
            case QuestInfo.Type.StartStory:
                if (Quests[questnum].QuestDone)
                {
                    Debug.Log("Hz");
                    return;
                }
                if (!Quests[questnum].QuestStart)
                {
                    levelSystem.QuestSystem.QuestStart(Quests[questnum].StoryIndex);
                    string text = "You start quest number " + Quests[questnum].StoryIndex;
                    Ui.PrintText(text);
                    Quests[questnum].QuestStart = true;
                    return;
                }
                else if (Quests[questnum].QuestStart)
                {
                    string text = "Pipirka";
                    Ui.PrintText(text);
                    return;
                }
                break;
            case QuestInfo.Type.StoryTalk:
                if (Quests[questnum].QuestDone)
                {
                    return;
                }
                if (!Quests[questnum].QuestStart)
                {
                    levelSystem.QuestSystem.Talk(Quests[questnum].StoryIndex, Quests[questnum].StoryStage);
                    string text = LanguagesSystem.Language.GlobalQuest[Quests[questnum].StoryIndex].Part[Quests[questnum].StoryStage].AiText;
                    Ui.PrintForTime(text, 5f);
                    Quests[questnum].QuestStart = true;
                    return;
                }
                else if (Quests[questnum].QuestStart)
                {
                    return;
                }
                break;
        }

    }
    public void OnEnemyDie(GameObject enemy)
    {
        for(int i = 0; i < Quests.Length; i++)
        {
            if(Quests[i].QuestType == QuestInfo.Type.Kill && Quests[i].Target == enemy)
            {
                Debug.Log("suk");
                End(-1, i);
            }
        }
    }
    public void End(int num, int questnum)
    {
        if (num == -1)
            num = Quests[questnum].QuestNum;
        switch(Quests[questnum].QuestType)
        {
            case QuestInfo.Type.Request:
                Dialog.SetDialogButton(Quests[questnum].QuestNum, LanguagesSystem.Language.LocalQuest[(int)Quests[questnum].QuestType].ButtonDone[(int)Dialog.HumanType], (int)Dialogs.Type.Quest);
                break;
            case QuestInfo.Type.Kill:

                break;
            case QuestInfo.Type.StartStory:

                break;
            case QuestInfo.Type.StoryTalk:

                break;
            case QuestInfo.Type.TakeItem:
                
                break;
        }
        Quests[questnum].QuestDone = true;
        Quests[questnum].CountCollected = 0;

    }
    public void Reload(int num, int questnum)
    {
        Quests[questnum].QuestDone = false;
        Quests[questnum].QuestStart = false;
        Quests[questnum].GotReward = false;
        Quests[questnum].CountCollected = 0;
        if(Quests[questnum].QuestType == QuestInfo.Type.Request)
        {
            int Num = Random.Range(1, 6);
            while(levelSystem.GetRandomItem(Num) == null && Num > 2)
            {
                Num--;
            }
            Quests[questnum].CountToRequset = Num;
            Quests[questnum].Target = levelSystem.GetRandomItem(Num);
            int minCost = Mathf.RoundToInt(Quests[questnum].Target.GetComponent<Item>().Cost * Num * Random.Range(0.25f, 1f));
            int maxCost = Quests[questnum].Target.GetComponent<Item>().Cost * Num * Random.Range(1, 3);
            while(IndexSystem.GetRandomItem(minCost, maxCost) == null && minCost > 10)
            {
                minCost = Mathf.RoundToInt(minCost * 0.5f);
            }
            Quests[questnum].Reward = IndexSystem.GetRandomItem(minCost, maxCost);

            Dialog.AddDialogButton(Quests[questnum].QuestNum, (int)Dialogs.Type.Quest, questnum);
            Debug.Log("3");
        }
    }

    public void MainQuestTalk(int num, int questnum)
    {
        levelSystem.QuestSystem.Talk(Quests[questnum].StoryIndex, Quests[questnum].StoryStage);
        string text = "Talk";
        Ui.PrintText(text);
    }
    public void MainQuestKilled()
    {
        for (int i = 0; i < Quests.Length; i++)
        {
            if (Quests[i].QuestType == QuestInfo.Type.BeKilled)
            {
                levelSystem.QuestSystem.Killed(Quests[i].StoryIndex, Quests[i].StoryStage);
                Quests[i].QuestDone = true;
                break;
            }
        }
    }
    public void MainQuestAttacked()
    {
        for (int i = 0; i < Quests.Length; i++)
        {
            if (Quests[i].QuestType == QuestInfo.Type.BeAttacked && !Quests[i].QuestDone)
            {
                levelSystem.QuestSystem.Attacked(Quests[i].StoryIndex, Quests[i].StoryStage);
                Quests[i].QuestDone = true;
                break;
            }
        }
    }

    public void StartGlobalQuest(int index, int part)
    {
        for(int i = 0; i < Quests.Length; i++)
        {
            if(Quests[i].StoryIndex == index && Quests[i].StoryStage == part)
            {
                Quests[i].QuestStart = true;
                break;
            }
        }
    }

    public void AddStoryTakeItem(int index, int stage, GameObject Item)
    {
        QuestInfo[] tempquest = Quests;
        Quests = new QuestInfo[tempquest.Length + 1];
        for (int i = 0; i < tempquest.Length; i++)
        {
            Quests[i] = tempquest[i];
        }
        Quests[Quests.Length - 1] = new QuestInfo();
        Quests[Quests.Length - 1].QuestType = QuestInfo.Type.TakeItem;
        Quests[Quests.Length - 1].StoryIndex = index;
        Quests[Quests.Length - 1].StoryStage = stage;
        Quests[Quests.Length - 1].Target = Item;
    }
    public void AddStoryTalk(int index, int stage)
    {
        StartCoroutine(AddTalk(index, stage));

    }
    public IEnumerator AddTalk(int index, int stage)
    {
        yield return new WaitForFixedUpdate();
        QuestInfo[] tempquest = Quests;
        Quests = new QuestInfo[tempquest.Length + 1];
        for (int i = 0; i < tempquest.Length; i++)
        {
            Quests[i] = tempquest[i];
        }
        Quests[Quests.Length - 1] = new QuestInfo();
        Quests[Quests.Length - 1].QuestType = QuestInfo.Type.StoryTalk;
        Quests[Quests.Length - 1].StoryIndex = index;
        Quests[Quests.Length - 1].StoryStage = stage;
        Dialog.AddDialogButton(0, (int)Dialogs.Type.Quest, Quests.Length - 1);
        if(Dialog.inDialog)
        {

        }
    }
    public void DeleteStoryTalk(int index, int stage)
    {
        StartCoroutine(DeleteTalk(index, stage));

    }
    private IEnumerator DeleteTalk(int index, int stage)
    {
        yield return new WaitForFixedUpdate();
        for (int i = 0; i < Quests.Length; i++)
        {
            if (Quests[i].QuestType == QuestInfo.Type.StoryTalk && Quests[i].StoryIndex == index && Quests[i].StoryStage == stage)
            {
                for (int a = i; a < Quests.Length - 1; a++)
                {
                    Quests[a] = Quests[a + 1];
                }
                QuestInfo[] quest = Quests;
                Quests = new QuestInfo[Quests.Length - 1];
                for (int a = 0; a < Quests.Length; a++)
                {
                    Quests[a] = quest[a];
                }
                for(int a = 0; a < Dialog.DialogVar.Length; a++)
                {
                    if(Dialog.DialogVar[a] == i)
                    {
                        Dialog.DeleteDialogButton(a);
                    }
                }
                yield break;
            }
        }
        yield break;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        for(int i = 0; i < Quests.Length; i++)
        {
            if (collision.transform.parent != null)
                return;
            if (Quests[i] == null)
                return;
            if(Quests[i].QuestType == QuestInfo.Type.Request)
            {
                int questnum = i;
                if (Quests[questnum].Target == null)
                    return;
                if (Quests[questnum].QuestStart && !Quests[questnum].QuestDone)
                {
                    if (collision.transform.root.gameObject.layer == 9)
                    {
                        int Index = collision.transform.root.GetComponent<Item>().Index;
                        if (Quests[questnum].Target.GetComponent<Item>().Index == Index && Quests[questnum].prevItem != collision.transform.root)
                        {
                            Quests[questnum].prevItem = collision.transform.root;
                            collision.transform.root.SendMessage("Destroy");
                            Quests[questnum].CountCollected++;
                        }
                        if (Quests[questnum].CountCollected >= Quests[questnum].CountToRequset)
                        {
                            End(Quests[questnum].QuestNum, questnum);
                        }
                    }
                }
            }
            else if(Quests[i].QuestType == QuestInfo.Type.TakeItem)
            {
                int questnum = i;
                if (Quests[questnum].Target == null)
                    return;
                if (!Quests[questnum].QuestStart)
                    return;
                if (collision.transform.root.gameObject.layer == 9)
                {
                    int Index = collision.transform.root.GetComponent<Item>().Index;
                    if (Quests[questnum].Target.GetComponent<Item>().Index == Index && Quests[questnum].prevItem != collision.transform.root)
                    {
                        levelSystem.QuestSystem.Taken(Quests[questnum].StoryIndex, Quests[questnum].StoryStage);
                        Quests[questnum].prevItem = collision.transform.root;
                        collision.transform.root.SendMessage("Destroy");
                        Quests[questnum].CountCollected++;
                    }
                }
            }
        }
    }

    IEnumerator CheckForReload()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            for (int i = 0; i < Quests.Length; i++)
            {
                if (Quests[i].Reloaded && Quests[i].QuestDone && Quests[i].GotReward)
                {
                    if (Time.time > Quests[i].TimeToNextQuest)
                    {
                        Reload(Quests[i].QuestNum, i);
                    }
                }
            }
        }
    }
    private void FixedUpdate()
    {
        
        
    }
}
[System.Serializable]
public class QuestInfo
{
    public enum Type {Request, Kill, StartStory, StoryTalk, TakeItem, BeKilled, BeAttacked, Null };
    public Type QuestType;
    [HideInInspector]
    public int QuestNum;
    [HideInInspector]
    public Transform prevItem;
    public GameObject Target;
    public int CountToRequset = 1;
    [HideInInspector]
    public int CountCollected = 0;


    public int StoryIndex;
    public int StoryStage;
    //[HideInInspector]
    public bool QuestStart;
    //[HideInInspector]
    public bool QuestDone;
    //[HideInInspector]
    public bool GotReward;
    public GameObject Reward;
    public bool Reloaded;
    [HideInInspector]
    public float TimeToNextQuest;
    public float ReloadQuestTime;

    public QuestInfo(DialogData data)
    {
        prevItem = null;
        Target = null;
        Reward = null;
    }
    public QuestInfo()
    {
        prevItem = null;
        Target = null;
        Reward = null;
    }
}
