using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexSystem : MonoBehaviour
{
    public GameObject[] Index;
    public GameObject GetRandomItem(int MinCost, int MaxCost)
    {
        int[] Items = new int[0];
        int num = 0;
        for(int i = 0; i < Index.Length; i++)
        {
            if(Cost(i) > MinCost && Cost(i) < MaxCost)
            {
                num++;
                int[] TempItems = Items;
                Items = new int[num];
                for(int a = 0; a < TempItems.Length; a++)
                {
                    Items[a] = TempItems[a]; 
                }
                Items[num - 1] = i;
            }
        }
        if (Items.Length > 0)
            return Index[Items[Random.Range(0, Items.Length)]];
        return null;
    }
    public GameObject GetRandomFood(int MaxCost)
    {
        return null;
    }
    public GameObject GetRandomWeapon(int MaxCost)
    {
        return null;
    }
    public GameObject GetRandomFood()
    {
        int Rand = Random.Range(100, 114);
        while (Index[Rand] == null)
        {
            Rand = Random.Range(100, 114);
        }
        return Index[Rand];
    }

    public GameObject[] Object;
    public GameObject[] Effect;
    private void Start()
    {
        //item = Resources.Load<Item>("Item");
    }

    public GameObject Item(int i)
    {
        if (Index[i] != null)
            return Index[i];
        else
            return null;
    }
    public GameObject Item(GameObject obj)
    {
        if (obj != null)
            return Index[obj.GetComponent<Item>().Index];
        else
            return null;
    }

    public string Name(int i)
    {
        if (Index[i] != null)
            return Index[i].GetComponent<Item>().Name;
        else
            return "NoName";
    }
    public bool isWeapon(int i)
    {
        if (Index[i] != null)
            return Index[i].GetComponent<Item>().isWeapon;
        else
            return false;
    }
    public int Cost(int i)
    {
        if (Index[i] != null)
            return Index[i].GetComponent<Item>().Cost;
        else
            return 0;
    }
    public int Cost(Transform i)
    {
        if (Index[i.GetComponent<Item>().Index] != null)
            return Index[i.GetComponent<Item>().Index].GetComponent<Item>().Cost;
        else
            return 0;
    }
    public float Weight(int i)
    {
        if (Index[i] != null)
            return Index[i].GetComponent<Item>().Weight;
        else
            return 1;
    }
    public int Armor(int i)
    {
        if (Index[i] != null)
            return Index[i].GetComponent<ItemArmor>().Armor;
        else
            return 0;
    }
    public Sprite Icon(int i)
    {
        if (Index[i] != null)
            return Index[i].GetComponent<Item>().Icon;
        else
            return null;
    }
    public int Value(int i)
    {
        if (Index[i] != null)
            return (int)Index[i].GetComponent<Item>().Value;
        else
            return 0;
    }


}
