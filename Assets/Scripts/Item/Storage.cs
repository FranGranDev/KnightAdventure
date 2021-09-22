using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public enum Type {Null,Arrows};
    public Type Item;
    public int MaxSlots;
    private int[,] Items = new int[10,2];
    private int Index;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

    }
    void SetIndex(int index)
    {
        Index = index;
    }
    void SetCount(int index)
    {
        int sum = 0;
        for (int i = 0; i < MaxSlots; i++)
        {
            if(Items[i, 0] == index)
            {
                sum += Items[i, 1];
            }
        }
        transform.root.SendMessage("GetCount", sum);
        
    }
    void ItemPlus(Transform item)
    {
        item.SendMessage("GetIndex", transform);

        for(int i = 0; i < MaxSlots; i++)
        {
            if (Items[i, 0] == Index || Items[i, 1] == 0)
            {
                
                if (Items[i, 1] < 10)
                {
                    Items[i, 1]++;
                    Items[i, 0] = Index;
                    item.SendMessage("Destroy");

                }
                else
                {
                    for (int a = i + 1; a < MaxSlots + 1; a++)
                    {
                        if ((Items[a, 1] < 10 && Items[a, 0] == Items[i, 0]) || Items[a, 0] == 0)
                        {
                            
                            Items[a, 1]++;
                            Items[a, 0] = Index;
                            item.SendMessage("Destroy");
                            break;
                        }
                    }
                }
                break;
            }
        }
    }
    void ItemMinus(int Index)
    {
        for(int i = 0; i < MaxSlots; i++)
        {
            if (Items[i, 1] == 0)
                return;
            if (Items[i, 0] == Index)
            {
                Items[i, 1]--;  
            }
            if (Items[i, 1] == 0)
                Items[i, 0] = 0;
        }
        

    }
    void SetBagType(Transform obj)
    {
        string name = "Null";
        switch (Item)
        {
            case Type.Arrows:
                name = "Arrows";
                break;
        }
        obj.SendMessage("GetStorageType", name);
    }
    private void FixedUpdate()
    {
        int Load = 0;
        for(int i = 0; i < MaxSlots; i++)
        {
            Load += Items[i, 1];
        }
        if(Load == 0)
            anim.SetInteger("Load", 0);
        else if(Load > 0 && Load <= 5)
            anim.SetInteger("Load", 1);
        else if (Load > 5 && Load <= 15)
            anim.SetInteger("Load", 2);
        else if (Load > 15)
            anim.SetInteger("Load", 3);
    }
}
