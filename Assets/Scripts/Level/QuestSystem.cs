using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{

    public QuestAll[] Story;

    public camerascript Cam;
    public Character Character;
    public LevelSystem levelSystem;
    public int ActionAiNum(int QuestIndex, int Stage, int Action)
    {
        GameObject ai = Story[QuestIndex].QuestPart[Stage].StartActions[Action].ActionTarget;
        return levelSystem.GetAiNum(ai);
    }
    public int ActionTargetAiNum(int QuestIndex, int Stage, int Action)
    {
        GameObject ai = Story[QuestIndex].QuestPart[Stage].StartActions[Action].Target;
        return levelSystem.GetAiNum(ai);
    }
    public int AiNum(int QuestIndex, int Stage, int num)
    {
        GameObject ai = Story[QuestIndex].QuestPart[Stage].Target[num];
        return levelSystem.GetAiNum(ai);
    }

    public void Awake()
    {

    }
    public void Start()
    {
    }

    public void QuestStart(int Index)
    {
        NowData.QuestIndex = Index;
        if (Index < Story.Length)
        {
            if (!Story[Index].Done)
            {
                if (!Story[Index].Started)
                {
                    Story[Index].Started = true;
                    Story[Index].NowStage = 0;
                    StartCoroutine(QuestStartStageDelay(0));
                }
                else
                {
                    
                }
            }
            else
            {
                
            }
        }
                
    }
    public IEnumerator QuestStartStageDelay(int Stage)
    {
        yield return new WaitForSeconds(Story[NowData.QuestIndex].QuestPart[Stage].Delay);
        QuestStartStage(Stage);
        yield break;
    }
    public void QuestStartStage(int Stage)
    {
        PrintTips(Stage);
        Story[NowData.QuestIndex].NowStage = Stage;
        if (Story[NowData.QuestIndex].GetPart().QuestType == QuestPart.Type.FindItem)
        {
            for(int i = 0; i < Story[NowData.QuestIndex].GetPart().Target.Length; i++)
            {
                int Index = Story[NowData.QuestIndex].GetPart().Target[i].GetComponent<Item>().Index;
                levelSystem.AddRequsetedItem(Index);
                Character.AddRequestItem(Index);
                for (int a = 0; a < Character.CountItem(Index); a++)
                {
                    Picked(Index);
                }
            }
            
        }
        else if (Story[NowData.QuestIndex].GetPart().QuestType == QuestPart.Type.GoTo)
        {
            
        }
        else if (Story[NowData.QuestIndex].GetPart().QuestType == QuestPart.Type.Null)
        {
            if (Story[NowData.QuestIndex].NowStage + 1 < Story[NowData.QuestIndex].QuestPart.Length)
            {
                Story[NowData.QuestIndex].NowStage++;
                StartCoroutine(QuestStartStageDelay(Story[NowData.QuestIndex].NowStage));
            }
            else
            {
                QuestEnd(NowData.QuestIndex);
            }
        }
        else if(Story[NowData.QuestIndex].GetPart().QuestType == QuestPart.Type.Kill)
        {
            for(int i = 0; i < Story[NowData.QuestIndex].GetPart().Target.Length; i++)
            {
                Story[NowData.QuestIndex].GetPart().Target[i].GetComponent<Ai>().AddToQuest(Character.gameObject);
            }
            if (Story[NowData.QuestIndex].GetPart().AiKilled >= Story[NowData.QuestIndex].GetPart().Target.Length)
            {
                if (Story[NowData.QuestIndex].NowStage + 1 < Story[NowData.QuestIndex].QuestPart.Length)
                {
                    Story[NowData.QuestIndex].NowStage++;
                    StartCoroutine(QuestStartStageDelay(Story[NowData.QuestIndex].NowStage));
                }
                else
                {
                    QuestEnd(NowData.QuestIndex);
                }
            }
        }
        else if (Story[NowData.QuestIndex].GetPart().QuestType == QuestPart.Type.Talk)
        {
            Story[NowData.QuestIndex].GetPart().Target[0].GetComponent<Ai>().AddToQuest(Character.gameObject);
            Story[NowData.QuestIndex].GetPart().Target[0].GetComponent<Quest>().AddStoryTalk(NowData.QuestIndex, Stage);
        }
        else if (Story[NowData.QuestIndex].GetPart().QuestType == QuestPart.Type.Give)
        {
            GameObject item = Story[NowData.QuestIndex].GetPart().Target[1];
            Story[NowData.QuestIndex].GetPart().Target[0].GetComponent<Quest>().AddStoryTakeItem(NowData.QuestIndex, Stage, item);
            Story[NowData.QuestIndex].GetPart().Target[0].GetComponent<Quest>().StartGlobalQuest(NowData.QuestIndex, Stage);
        }

        ActionOnStart();
    }
    public void QuestEnd(int QuestIndex)
    {
        Story[QuestIndex].Done = true;
        levelSystem.MainUI.TurnQuestTips(false);
    }

    public void ActionOnStart()
    {
        for (int i = 0; i < Story[NowData.QuestIndex].GetPart().StartActions.Length; i++)
        {
            
            if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.ShowTarget)
            {
                int Index = NowData.QuestIndex;
                int Stage = Story[Index].NowStage;
                float time = Story[NowData.QuestIndex].GetPart().StartActions[i].Delay;
                StartCoroutine(WaitTime(ShowTarget, Index, Stage, i, time));
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.StartNewQuest)
            {
                int Index = Story[NowData.QuestIndex].GetPart().StartActions[i].Varible;
                QuestStart(Index);
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.GiveItem)
            {
                int Index = NowData.QuestIndex;
                int Stage = Story[Index].NowStage;
                float time = Story[NowData.QuestIndex].GetPart().StartActions[i].Delay;
                StartCoroutine(WaitTime(GiveItem, Index, Stage, i, time));
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.ShowEnemies)
            {
                Cam.ShowTargets(Story[NowData.QuestIndex].GetPart().Target, 1f);
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.AttackCharacter)
            {
                float time = Story[NowData.QuestIndex].GetPart().StartActions[i].Delay;
                int Index = NowData.QuestIndex;
                int Stage = Story[Index].NowStage;
                StartCoroutine(WaitTime(AttackCharacter, Index, Stage, i, time));
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.WaitCameGiveItem)
            {
                Transform man = Story[NowData.QuestIndex].GetPart().StartActions[i].ActionTarget.transform;
                int Index = NowData.QuestIndex;
                int Stage = Story[Index].NowStage;
                float time = Story[NowData.QuestIndex].GetPart().StartActions[i].Varible;
                StartCoroutine(WaitCame(GiveItem, Index, Stage, i, man));
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.AttackTarget)
            {
                int Index = NowData.QuestIndex;
                int Stage = Story[Index].NowStage;
                float time = Story[NowData.QuestIndex].GetPart().StartActions[i].Delay;
                StartCoroutine(WaitTime(AttackTarget, Index, Stage, i, time));
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.GoToTarget)
            {
                int Index = NowData.QuestIndex;
                int Stage = Story[Index].NowStage;
                float time = Story[NowData.QuestIndex].GetPart().StartActions[i].Delay;
                StartCoroutine(WaitTime(GoToTarget, Index, Stage, i, time));
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.OffAgressive)
            {
                int Index = NowData.QuestIndex;
                int Stage = Story[Index].NowStage;
                float time = Story[NowData.QuestIndex].GetPart().StartActions[i].Delay;
                StartCoroutine(WaitTime(OffAgressive, Index, Stage, i, time));
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.GoToCharacter)
            {
                int Index = NowData.QuestIndex;
                int Stage = Story[Index].NowStage;
                float time = Story[NowData.QuestIndex].GetPart().StartActions[i].Delay;
                StartCoroutine(WaitTime(GoToCharacter, Index, Stage, i, time));
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.OffEffects)
            {
                int Index = NowData.QuestIndex;
                int Stage = Story[Index].NowStage;
                float time = Story[NowData.QuestIndex].GetPart().StartActions[i].Delay;
                StartCoroutine(WaitTime(OffEffects, Index, Stage, i, time));
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.WaitCameAndSay)
            {
                int Index = NowData.QuestIndex;
                int Stage = Story[Index].NowStage;
                Transform Target = Story[NowData.QuestIndex].GetPart().StartActions[i].ActionTarget.transform;
                StartCoroutine(WaitCame(Say, Index, Stage, i, Target));
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.Say)
            {
                int Index = NowData.QuestIndex;
                int Stage = Story[Index].NowStage;
                float time = Story[NowData.QuestIndex].GetPart().StartActions[i].Delay;
                StartCoroutine(WaitTime(Say, Index, Stage, i, time));
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.WaitCameAndDialog)
            {
                int Index = NowData.QuestIndex;
                int Stage = Story[Index].NowStage;
                Transform Target = Story[NowData.QuestIndex].GetPart().StartActions[i].ActionTarget.transform;
                StartCoroutine(WaitCame(StartDialog, Index, Stage, i, Target));
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.FollowCharacter)
            {
                int Index = NowData.QuestIndex;
                int Stage = Story[Index].NowStage;
                float time = Story[NowData.QuestIndex].GetPart().StartActions[i].Delay;
                StartCoroutine(WaitTime(FollowCharacter, Index, Stage, i, time));
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.NewScene)
            {
                int Index = NowData.QuestIndex;
                int Stage = Story[Index].NowStage;
                float time = Story[NowData.QuestIndex].GetPart().StartActions[i].Delay;
                StartCoroutine(WaitTime(NewScene, Index, Stage, i, time));
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.SpecialWeek)
            {
                Character.Hp = 1;
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.DieAndNewScene)
            {
                int Index = NowData.QuestIndex;
                int Stage = Story[Index].NowStage;
                StartCoroutine(WaitCharacterDeathAndLoadScene(Index, Stage, i));
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.SetAgressive)
            {
                Story[NowData.QuestIndex].GetPart().StartActions[i].ActionTarget.GetComponent<Ai>().Agressive = true;
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.SetAgressive)
            {
                Story[NowData.QuestIndex].GetPart().StartActions[i].ActionTarget.GetComponent<Ai>().Agressive = false;
            }
            else if (Story[NowData.QuestIndex].GetPart().StartActions[i].Action == QuestAction.ActionType.ActivateQuest)
            {
                if(Story[NowData.QuestIndex].GetPart().StartActions[i].ActionTarget != null)
                {
                    int stage = Story[NowData.QuestIndex].NowStage;
                    Story[NowData.QuestIndex].GetPart().StartActions[i].ActionTarget.GetComponent<Quest>().StartGlobalQuest(NowData.QuestIndex, stage);
                }
            }
        }
    }
    public IEnumerator DelayStartStage(int time)
    {
        yield return new WaitForSeconds(time);
        PrintTips(Story[NowData.QuestIndex].NowStage);
        if (Story[NowData.QuestIndex].GetPart().QuestType == QuestPart.Type.FindItem)
        {
            for (int i = 0; i < Story[NowData.QuestIndex].GetPart().Target.Length; i++)
            {
                int Index = Story[NowData.QuestIndex].GetPart().Target[i].GetComponent<Item>().Index;
                levelSystem.AddRequsetedItem(Index);
                Character.AddRequestItem(Index);
                for (int a = 0; a < Character.CountItem(Index); a++)
                {
                    Picked(Index);
                }
            }

        }
        else if (Story[NowData.QuestIndex].GetPart().QuestType == QuestPart.Type.GoTo)
        {

        }
        else if (Story[NowData.QuestIndex].GetPart().QuestType == QuestPart.Type.Null)
        {
            if (Story[NowData.QuestIndex].NowStage + 1 < Story[NowData.QuestIndex].QuestPart.Length)
            {
                Story[NowData.QuestIndex].NowStage++;
                StartCoroutine(QuestStartStageDelay(Story[NowData.QuestIndex].NowStage));
            }
            else
            {
                QuestEnd(NowData.QuestIndex);
            }
        }
        else if (Story[NowData.QuestIndex].GetPart().QuestType == QuestPart.Type.Kill)
        {
            for (int i = 0; i < Story[NowData.QuestIndex].GetPart().Target.Length; i++)
            {
                Story[NowData.QuestIndex].GetPart().Target[i].GetComponent<Ai>().AddToQuest(Character.gameObject);
            }
            if (Story[NowData.QuestIndex].GetPart().AiKilled >= Story[NowData.QuestIndex].GetPart().Target.Length)
            {
                if (Story[NowData.QuestIndex].NowStage + 1 < Story[NowData.QuestIndex].QuestPart.Length)
                {
                    Story[NowData.QuestIndex].NowStage++;
                    StartCoroutine(QuestStartStageDelay(Story[NowData.QuestIndex].NowStage));
                }
                else
                {
                    QuestEnd(NowData.QuestIndex);
                }
            }
        }
        ActionOnStart();
        yield break;
    }

    IEnumerator WaitTime(Delegate yourAction, int QuestIndex, int Stage, int num, float time)
    {
        yield return new WaitForSeconds(time);
        yourAction(QuestIndex, Stage, num);
        yield break;
    }
    IEnumerator WaitCame(Delegate yourAction, int QuestIndex, int Stage, int num, Transform Target)
    {
        while ((Character.transform.position - Target.position).magnitude > 2f)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Target.GetComponent<Rigidbody2D>().velocity *= 0.25f;
        yourAction(QuestIndex, Stage, num);
        yield break;
    }

    IEnumerator WaitCharacterDeathAndLoadScene(int QuestIndex, int Stage, int num)
    {
        while (Character.Hp > 0)
        {
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(1);
        levelSystem.anim.SetTrigger("FadeOut");
        float time = Story[QuestIndex].QuestPart[Stage].StartActions[num].Delay;
        int Scene = Story[QuestIndex].QuestPart[Stage].StartActions[num].Varible;
        yield return new WaitForSeconds(time);
        Character.Hp = Random.Range(1, 50);
        levelSystem.GetToNewLevel(Scene);
    }
    public void ShowTarget(int QuestIndex, int Stage, int num)
    {
        Transform Target = Story[QuestIndex].QuestPart[Stage].StartActions[num].ActionTarget.transform;
        float time = Story[QuestIndex].QuestPart[Stage].StartActions[num].Varible;
        Cam.ShowTargetForTime(Target, time);
    }
    public void AttackTarget(int QuestIndex, int Stage, int num)
    {
        Transform Target = Story[QuestIndex].QuestPart[Stage].StartActions[num].Target.transform;
        Story[QuestIndex].QuestPart[Stage].StartActions[num].ActionTarget.GetComponent<Ai>().AttackTarget(Target);
    }
    public void AttackCharacter(int QuestIndex, int Stage, int num)
    {
        Story[QuestIndex].QuestPart[Stage].StartActions[num].ActionTarget.GetComponent<Ai>().GetEnemy(Character.transform);
    }
    public void GiveItem(int QuestIndex, int Stage, int num)
    {
        GameObject item = Story[QuestIndex].QuestPart[Stage].StartActions[num].Target;
        Ai Man = Story[QuestIndex].QuestPart[Stage].StartActions[num].ActionTarget.GetComponent<Ai>();
        GameObject obj = Instantiate(item, Man.Hand.position, Man.Hand.rotation, null);
        obj.GetComponent<Item>().JumpTo(Character.GetComponent<Character>().Head, 2f);
    }
    public void GoToTarget(int QuestIndex, int Stage, int num)
    {
        Transform Target = Story[QuestIndex].QuestPart[Stage].StartActions[num].Target.transform;
        Story[QuestIndex].QuestPart[Stage].StartActions[num].ActionTarget.GetComponent<Ai>().GoToTarget(Target, false);
    }
    public void GoToCharacter(int QuestIndex, int Stage, int num)
    {
        Transform Target = Character.transform;
        Story[QuestIndex].QuestPart[Stage].StartActions[num].ActionTarget.GetComponent<Ai>().GoToTarget(Target, false);
    }
    public void FollowCharacter(int QuestIndex, int Stage, int num)
    {
        if (Story[QuestIndex].QuestPart[Stage].StartActions[num].Varible == 1)
        {
            Story[QuestIndex].QuestPart[Stage].StartActions[num].ActionTarget.GetComponent<Ai>().StartFollow(Character.transform);
        }
        else
        {
            Story[QuestIndex].QuestPart[Stage].StartActions[num].ActionTarget.GetComponent<Ai>().EndFollow();
        }
    }
    public void OffAgressive(int QuestIndex, int Stage, int num)
    {
        Ai ai = Story[QuestIndex].QuestPart[Stage].StartActions[num].ActionTarget.GetComponent<Ai>();
        ai.Agressive = false;
        ai.StartCoroutine(ai.LoseEnemy(0));
    }
    public void OffEffects(int QuestIndex, int Stage, int num)
    {
        Character.EndEffect();
    }
    public void NewScene(int QuestIndex, int Stage, int num)
    {
        levelSystem.GetToNewLevel(Story[QuestIndex].QuestPart[Stage].StartActions[num].Varible);
    }

    public void StartDialog(int QuestIndex, int Stage, int num)
    {
        Story[QuestIndex].QuestPart[Stage].StartActions[num].ActionTarget.GetComponent<Dialogs>().StartDialog(Character.transform);
    }
    public void Say(int QuestIndex, int Stage, int num)
    {
        string text = LanguagesSystem.Language.GlobalQuest[QuestIndex].Part[Stage].AiCameText[num];
        levelSystem.MainUI.PrintForTime(text, 5f);
    }
    private delegate void Delegate(int QuestIndex, int Stage, int num);

    public void PrintTips(int Stage)
    {
        if(LanguagesSystem.Language.GlobalQuest.Length > NowData.QuestIndex &&
        LanguagesSystem.Language.GlobalQuest[NowData.QuestIndex].Part.Length > Stage)
        levelSystem.MainUI.PrintTips(LanguagesSystem.Language.GlobalQuest[NowData.QuestIndex].Part[Stage].Tips);
    }

    public void TriggerEnter(int QuestIndex, int Stage)
    {
        if (QuestIndex > Story.Length || Stage > Story[QuestIndex].QuestPart.Length)
            return;
        if (!Story[QuestIndex].Started)
            return;

        if(NowData.QuestIndex == QuestIndex)
        {
            if (Story[NowData.QuestIndex].NowStage == Stage)
            {
                if (Story[NowData.QuestIndex].NowStage + 1 < Story[NowData.QuestIndex].QuestPart.Length)
                {
                    Story[NowData.QuestIndex].NowStage++;
                    StartCoroutine(QuestStartStageDelay(Story[NowData.QuestIndex].NowStage));
                }
                else
                {
                    QuestEnd(QuestIndex);
                }
            }
        }
    }
    public void Killed(int QuestIndex, int Stage)
    {
        if (QuestIndex == -1 || QuestIndex >= Story.Length)
            return;
        Story[QuestIndex].QuestPart[Stage].AiKilled++;
        if (!Story[QuestIndex].Started)
            return;
        if (NowData.QuestIndex == QuestIndex)
        {
            if (Story[NowData.QuestIndex].NowStage == Stage)
            {
                if (Story[NowData.QuestIndex].GetPart().AiKilled >= Story[NowData.QuestIndex].GetPart().Target.Length)
                {
                    if (Story[NowData.QuestIndex].NowStage + 1 < Story[NowData.QuestIndex].QuestPart.Length)
                    {
                        Story[NowData.QuestIndex].NowStage++;
                        StartCoroutine(QuestStartStageDelay(Story[NowData.QuestIndex].NowStage));
                    }
                    else
                    {
                        QuestEnd(QuestIndex);
                    }
                }
            }
        }
    }
    public void Attacked(int QuestIndex, int Stage)
    {
        if (QuestIndex == -1 || QuestIndex >= Story.Length)
            return;
        Story[QuestIndex].QuestPart[Stage].AiAttacked++;
        if (!Story[QuestIndex].Started)
            return;
        if (NowData.QuestIndex == QuestIndex)
        {
            if (Story[NowData.QuestIndex].NowStage == Stage)
            {
                if (Story[NowData.QuestIndex].GetPart().AiAttacked >= Story[NowData.QuestIndex].GetPart().Target.Length)
                {
                    if (Story[NowData.QuestIndex].NowStage + 1 < Story[NowData.QuestIndex].QuestPart.Length)
                    {
                        Story[NowData.QuestIndex].NowStage++;
                        StartCoroutine(QuestStartStageDelay(Story[NowData.QuestIndex].NowStage));
                    }
                    else
                    {
                        QuestEnd(QuestIndex);
                    }
                }
            }
        }
    }
    public void Talk(int QuestIndex, int Stage)
    {
        if (!Story[QuestIndex].Started)
            return;
        if (NowData.QuestIndex == QuestIndex)
        {
            if (Story[NowData.QuestIndex].NowStage == Stage)
            {
                Story[NowData.QuestIndex].GetPart().Target[0].GetComponent<Ai>().DeleteFromQuest();
                Story[NowData.QuestIndex].GetPart().Target[0].GetComponent<Quest>().DeleteStoryTalk(QuestIndex, Stage);
                if (Story[NowData.QuestIndex].NowStage + 1 < Story[NowData.QuestIndex].QuestPart.Length)
                {
                    Story[NowData.QuestIndex].NowStage++;
                    StartCoroutine(QuestStartStageDelay(Story[NowData.QuestIndex].NowStage));
                }
                else
                {
                    QuestEnd(QuestIndex);
                }
            }
        }
    }
    public void Picked(int Index)
    {
        if(!Story[NowData.QuestIndex].Started)
        {
            return;
        }
        if (Story[NowData.QuestIndex].GetPart().QuestType == QuestPart.Type.FindItem)
        {
            if (Story[NowData.QuestIndex].GetPart().Target[0].GetComponent<Item>().Index == Index)
            {
                Story[NowData.QuestIndex].QuestPart[Story[NowData.QuestIndex].NowStage].ItemCollected++;
            }
            if(Story[NowData.QuestIndex].GetPart().ItemCollected >= Story[NowData.QuestIndex].GetPart().ItemNum)
            {
                
                if (Story[NowData.QuestIndex].NowStage + 1 < Story[NowData.QuestIndex].QuestPart.Length)
                {
                    Story[NowData.QuestIndex].NowStage++;
                    StartCoroutine(QuestStartStageDelay(Story[NowData.QuestIndex].NowStage));
                }
                else
                {
                    QuestEnd(NowData.QuestIndex);
                }

            }
        }
    }
    public void Taken(int QuestIndex, int Stage)
    {
        if (QuestIndex == -1 || QuestIndex >= Story.Length)
            return;
        Story[QuestIndex].QuestPart[Stage].ItemCollected++;
        if (!Story[QuestIndex].Started)
            return;
        if (NowData.QuestIndex == QuestIndex)
        {
            if (Story[NowData.QuestIndex].NowStage == Stage)
            {
                if (Story[NowData.QuestIndex].GetPart().ItemCollected >= Story[NowData.QuestIndex].GetPart().ItemNum)
                {
                    if (Story[NowData.QuestIndex].NowStage + 1 < Story[NowData.QuestIndex].QuestPart.Length)
                    {
                        Story[NowData.QuestIndex].NowStage++;
                        StartCoroutine(QuestStartStageDelay(Story[NowData.QuestIndex].NowStage));
                    }
                    else
                    {
                        QuestEnd(QuestIndex);
                    }
                }
            }
        }
    }


    public void Load(QuestData data)
    {
        for(int i = 0; i < Story.Length; i++)
        {
            Story[i].Started = data.Started[i];
            Story[i].Done = data.Done[i];
            Story[i].NowStage = data.NowStage[i];
            for(int a = 0; a < Story[i].QuestPart.Length; a++)
            {
                //Story[i].QuestPart[a].QuestType =  (QuestPart.Type)data.QuestType[i, a];
                Story[i].QuestPart[a].AiKilled = data.AiKilled[i, a];
                Story[i].QuestPart[a].AiAttacked = data.AiAttacked[i, a];
                Story[i].QuestPart[a].ItemCollected = data.ItemCollected[i, a];
                if(Story[i].QuestPart[a].QuestType == QuestPart.Type.FindItem)
                {
                    //int index = data.TargetItem[i, a];
                    //if(index != -1 && Story[i].QuestPart[a].Target.Length > 0)
                        //Story[i].QuestPart[a].Target[0] = levelSystem.IndexSystem.Item(index);
                }
                else if (Story[i].QuestPart[a].QuestType == QuestPart.Type.Kill)
                {
                    for(int b = 0; b < Story[i].QuestPart[a].Target.Length; b++)
                    {
                        Story[i].QuestPart[a].Target[b] = levelSystem.GetAi(data.TargetAi[i, a, b]);
                    }
                }
                else if (Story[i].QuestPart[a].QuestType == QuestPart.Type.Talk)
                {
                    Story[i].QuestPart[a].Target[0] = levelSystem.GetAi(data.TargetAi[i, a, 0]);
                }
                else if (Story[i].QuestPart[a].QuestType == QuestPart.Type.Give)
                {
                    int ai = data.TargetAi[i, a, 0];
                    if (ai != -1)
                    {
                        Story[i].QuestPart[a].Target[0] = levelSystem.GetAi(ai);

                    }

                }

                for (int b = 0; b < Story[i].QuestPart[a].StartActions.Length; b++)
                {
                    if (data.ActionTargetAi[i, a, b] != -1)
                        Story[i].QuestPart[a].StartActions[b].Target = levelSystem.GetAi(data.ActionTargetAi[i, a, b]);
                    if (data.ActionAi[i, a, b] != -1)
                        Story[i].QuestPart[a].StartActions[b].ActionTarget = levelSystem.GetAi(data.ActionAi[i, a, b]);
                    
                }
                
                if (Story[NowData.QuestIndex].Started)
                {
                    StartCoroutine(DelayStartStage(1));
                }
            }
        }
    }
}
[System.Serializable]
public class QuestData
{
    public bool[] Started;
    public bool[] Done;
    public int[] NowStage;
    public float[,] Delay;

    public int[,] QuestType;
    public int[,,] TargetAi;
    //public int[,] TargetItem;
    public int[,] ItemCollected;
    public int[,] AiKilled;
    public int[,] AiAttacked;

    public int[,,] ActionType;
    public float[,,] ActionDelay;
    public int[,,] ActionVarible;
    public int[,,] ActionItem;
    public int[,,] ActionAi;
    public int[,,] ActionTargetAi;

    public QuestData(QuestSystem quest)
    {
        Started = new bool[quest.Story.Length];
        Done = new bool[quest.Story.Length];
        NowStage = new int[quest.Story.Length];
        QuestType = new int[quest.Story.Length, 15];
        TargetAi = new int[quest.Story.Length, 15, 5];
        //TargetItem = new int[quest.Story.Length, 15];
        ItemCollected = new int[quest.Story.Length, 15];
        AiKilled = new int[quest.Story.Length, 15];
        AiAttacked = new int[quest.Story.Length, 15];
        ActionType = new int[quest.Story.Length, 15, 10];
        ActionVarible = new int[quest.Story.Length, 15, 10];
        ActionItem = new int[quest.Story.Length, 15, 10];
        ActionAi = new int[quest.Story.Length, 15, 10];
        ActionTargetAi = new int[quest.Story.Length, 15, 10];
        for (int i = 0; i < quest.Story.Length; i++)
        {
            Started[i] = quest.Story[i].Started;
            Done[i] = quest.Story[i].Done;
            NowStage[i] = quest.Story[i].NowStage;

            for (int a = 0; a < quest.Story[i].QuestPart.Length; a++)
            {
                QuestType[i, a] = (int)quest.Story[i].QuestPart[a].QuestType;
                //Delay[i, a] = quest.Story[i].QuestPart[a].Delay;
                for(int b = 0; b < quest.Story[i].QuestPart[a].Target.Length; b++)
                {
                    if (quest.Story[i].QuestPart[a].Target[b] != null &&
                        quest.Story[i].QuestPart[a].Target[b].tag == "Enemy")
                    {
                        TargetAi[i, a, b] = quest.AiNum(i, a, b);
                    }
                    else
                    {
                        TargetAi[i, a, b] = -1;
                    }
                }
                AiKilled[i, a] = quest.Story[i].QuestPart[a].AiKilled;
                AiAttacked[i, a] = quest.Story[i].QuestPart[a].AiAttacked;
                /*
                for (int b = 0; b < quest.Story[i].QuestPart[a].Target.Length; b++)
                {
                    if (quest.Story[i].QuestPart[a].Target[b] != null &&
                        quest.Story[i].QuestPart[a].Target[b].tag == "Item")
                    {
                        TargetItem[i, a] = quest.Story[i].QuestPart[a].Target[b].GetComponent<Item>().Index;
                    }
                }
                */
                ItemCollected[i, a] = quest.Story[i].QuestPart[a].ItemCollected;

                for (int b = 0; b < quest.Story[i].QuestPart[a].StartActions.Length; b++)
                {
                    if (quest.Story[i].QuestPart[a].StartActions[b].ActionTarget != null &&
                       quest.Story[i].QuestPart[a].StartActions[b].ActionTarget.tag == "Enemy")
                    {
                        ActionAi[i, a, b] = quest.ActionAiNum(i, a, b);
                    }
                    else
                    {
                        ActionAi[i, a, b] = -1;
                    }

                    if (quest.Story[i].QuestPart[a].StartActions[b].Target != null &&
                       quest.Story[i].QuestPart[a].StartActions[b].Target.tag == "Enemy")
                    {
                        ActionTargetAi[i, a, b] = quest.ActionTargetAiNum(i, a, b);
                    }
                    else
                    {
                        ActionTargetAi[i, a, b] = -1;
                    }

                    if (quest.Story[i].QuestPart[a].StartActions[b].ActionTarget != null &&
                        quest.Story[i].QuestPart[a].StartActions[b].ActionTarget.tag == "Item")
                    {
                        ActionItem[i, a, b] = quest.Story[i].QuestPart[a].StartActions[b].ActionTarget.GetComponent<Item>().Index;
                    }
                    else
                    {
                        ActionItem[i, a, b] = -1;
                    }
                }
            }
        }
    }
}
    

[System.Serializable]
public struct QuestAll
{
    public string name;
    public bool Started;
    public bool Done;
    public int NowStage;
    public QuestPart[] QuestPart;
    public QuestPart GetPart()
    {
        return QuestPart[NowStage];
    }
}
[System.Serializable]
public struct QuestPart
{
    public string name;
    public float Delay;

    public enum Type {FindItem, Talk, Kill, Give, Attack, GoTo, Null };
    public Type QuestType;

    public GameObject[] Target;

    public int ItemNum;
    public int ItemCollected;
    public int AiKilled;
    public int AiAttacked;

    public QuestAction[] StartActions;
}
[System.Serializable]
public struct QuestAction
{
    public enum ActionType { Null, ShowTarget, ShowEnemies, WaitCameGiveItem, AttackCharacter, AttackTarget, GoToCharacter, WaitCameAndSay, WaitCameAndDialog, GoToTarget, FollowCharacter, NewScene, SpecialWeek, DieAndNewScene, ActivateQuest, Say, ShowTargetForTime, GiveItem, SetAgressive, OffAgressive, StartNewQuest, OffEffects };
    public ActionType Action;

    public GameObject ActionTarget;
    public GameObject Target;
    public float Delay;
    public int Varible;
}


