using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class LevelData
{
    public int AiNum;
    public AiData[] AiOnSceneData;

    public int ItemsNum;
    public ItemsData[] ItemsOnSceneData;

    public int ObjectNum;
    public ObjectData[] ObjectOnSceneData;

    public int DialogNum;
    public DialogData[] DialogOnSceneData;

    public int HouseNum;
    public HouseDate[] HouseOnSceneData;

    public QuestData QuestLevelData;

    public float[] CharacterPosition = new float[3];
    public string scene;
    public int Location;
    public float time;
    public float timespeed;

    public int[] RequsetedItem;

    public LevelData(LevelSystem level, int Slot)
    {
        CharacterPosition[0] = level.Character.transform.position.x;
        CharacterPosition[1] = level.Character.transform.position.y;
        CharacterPosition[2] = level.Character.transform.position.z;

        AiNum = level.AiOnScene.Length;
        AiOnSceneData = new AiData[AiNum];
        if (AiNum > 0)
        {
            Ai[] Ai = new Ai[AiNum];
            for (int i = 0; i < level.AiOnScene.Length; i++)
            {
                Ai[i] = level.AiOnScene[i].GetComponent<Ai>();
                AiOnSceneData[i] = new AiData(Ai[i]);
            }
        }

        DialogNum = AiNum;
        DialogOnSceneData = new DialogData[DialogNum];
        if(DialogNum > 0)
        {
            Dialogs[] dialogs = new Dialogs[DialogNum];
            for(int i = 0; i < DialogNum; i++)
            {
                if (level.AiOnScene[i].GetComponent<Dialogs>() != null)
                {
                    dialogs[i] = level.AiOnScene[i].GetComponent<Dialogs>();
                    DialogOnSceneData[i] = new DialogData(dialogs[i]);
                }
                else
                {
                    DialogOnSceneData[i] = null;
                    dialogs[i] = null;
                }
            }
        }

        ObjectNum = level.ObjectAtScnene().Length;
        ObjectOnSceneData = new ObjectData[ObjectNum];
        if(ObjectNum > 0)
        {
            Object[] Obj = new Object[ObjectNum];
            for (int i = 0; i < Obj.Length; i++)
            {
                Obj[i] = level.ObjectAtScnene()[i].GetComponent<Object>();
                ObjectOnSceneData[i] = new ObjectData(Obj[i]);
            }
        }

        ItemsNum = level.ItemAtScene().Length;
        ItemsOnSceneData = new ItemsData[ItemsNum];
        if(ItemsNum > 0)
        {
            Item[] items = new Item[ItemsNum];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = level.ItemAtScene()[i].GetComponent<Item>();
                ItemsOnSceneData[i] = new ItemsData(items[i]);
            }
        }

        HouseNum = level.HouseAtScnene().Length;
        HouseOnSceneData = new HouseDate[HouseNum];
        if(HouseNum > 0)
        {
            for(int i = 0; i < HouseNum; i++)
            {
                HouseOnSceneData[i] = level.HouseAtScnene()[i].GetComponent<House>().Data;
                HouseOnSceneData[i].ReferenceName = level.HouseAtScnene()[i].name;
                HouseOnSceneData[i].SavePosition(level.HouseAtScnene()[i]);
            }
        }

        QuestLevelData = new QuestData(level.QuestSystem);

        scene = level.Scene.name;
        time = DayTime.ClockTime;
        timespeed = level.Clock.Speed;
        Location = (int)level.Location;

    }
}

