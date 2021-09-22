using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class ObjectData
{
    public string ReferenceName;
    public string Name;
    public int Index;
    public float HP;
    public float MaxHP;
    public float[] position;
    public float[] scale;
    public int Type;
    //-----Plant------
    public int Stage;
    public bool isReady;
    public string Field;
    //-----Chest------
    public bool NeedKey;
    public int Key;
    public bool Closed;
    public ItemsData[] Items;

    public ObjectData(Object obj)
    {
        if (obj == null)
            return;
        Name = obj.Name;
        ReferenceName = obj.transform.name;
        Index = obj.Index;
        position = new float[3];
        position[0] = obj.transform.position.x;
        position[1] = obj.transform.position.y;
        position[2] = obj.transform.position.z;
        scale = new float[3];
        scale[0] = obj.transform.localScale.x;
        scale[1] = obj.transform.localScale.y;
        scale[2] = obj.transform.localScale.z;
        Type = (int)obj.ObjType;

        if (obj.ObjType == Object.Type.Plant)
        {
            isReady = obj.GetComponent<GrowUp>().IsReady;
            Stage = obj.GetComponent<GrowUp>().Stage;
            if (obj.GetComponent<GrowUp>().field != null)
            {
                Field = obj.GetComponent<GrowUp>().field.name;
            }
            else
            {
                Field = "";
            }
        }
        else if(obj.ObjType == Object.Type.Object)
        {
            HP = obj.HP;
            MaxHP = obj.MaxHP;

        }
        else if (obj.ObjType == Object.Type.Chest)
        {
            Chest chest = obj.GetComponent<Chest>();
            Closed = chest.Closed;
            NeedKey = chest.NeedKey;
            if (chest.Key != null)
            {
                Key = chest.Key.GetComponent<Item>().Index;
            }
            else
            {
                Key = -1;
            }
            Items = new ItemsData[chest.Items.Length];
            for(int i = 0; i < Items.Length; i++)
            {
                if (chest.Items[i] != null)
                {
                    Items[i] = new ItemsData(obj.GetComponent<Chest>().Items[i].GetComponent<Item>());
                }
                else
                {
                    Items[i] = null;
                }
            }
        }


    }
}