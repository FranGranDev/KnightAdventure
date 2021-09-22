using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class HouseDate
{
    public string ReferenceName;
    public int Index;
    public int ItemsNum;
    public ItemsData[] ItemsAtHouse;
    public int ObjNum;
    public ObjectData[] ObjectsAtHouse;
    public int AiNum;
    public int[] AiAtHouse;
    public DialogData[] DialogsAtHouse;
    public float[] position;

    public HouseDate(InHouse House)
    {
        ReferenceName = House.ReferenceName;
        ItemsNum = House.ItemAtHome.Length;
        ItemsAtHouse = new ItemsData[ItemsNum];
        Index = House.Index;
        for(int i = 0; i < ItemsNum; i++)
        {
            ItemsAtHouse[i] = new ItemsData(House.ItemAtHome[i].GetComponent<Item>());
        }

        ObjNum = House.ObjectAtHome.Length;
        ObjectsAtHouse = new ObjectData[ObjNum];
        for (int i = 0; i < ObjNum; i++)
        {
            ObjectsAtHouse[i] = new ObjectData(House.ObjectAtHome[i].GetComponent<Object>());
        }

        AiNum = House.AiAtHome.Length;
        AiAtHouse = new int[AiNum];
        DialogsAtHouse = new DialogData[AiNum];
        for(int i = 0; i < AiNum; i++)
        {
            if(House.AiAtHome[i] != null)
            {
                AiAtHouse[i] = House.AiAtHomeNum[i];
                DialogsAtHouse[i] = new DialogData(House.AiAtHome[i].GetComponent<Dialogs>());
                
            }
            else
            {
                AiAtHouse[i] = -1;
            }
        }

    }

    public void SavePosition(GameObject House)
    {
        position = new float[3];
        position[0] = House.transform.position.x;
        position[1] = House.transform.position.y;
        position[2] = House.transform.position.z;
    }
    public Vector3 LoadPosition()
    {
        return new Vector3(position[0], position[1], position[2]);
    }

}
