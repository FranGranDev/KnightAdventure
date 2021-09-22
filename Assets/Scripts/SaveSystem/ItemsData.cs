using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class ItemsData
{
    public string ReferenceName;
    public string Name;
    public int index;
    public float[] position;
    public float rotation;

    public int Owner;

    public bool OnScene;
    public bool Requseted;

    public int Level;
    public int Uses;
    public int armor;
    public bool Loaded;
    public int FoodStage;
    public bool FullyEaten;

    public ItemsData(Item items)
    {
        ReferenceName = items.name;
        Name = items.Name;
        index = items.GetComponent<Item>().Index;
        Requseted = items.GetComponent<Item>().Requested;

        position = new float[3];
        position[0] = items.transform.position.x;
        position[1] = items.transform.position.y;
        position[2] = items.transform.position.z;
        rotation = items.transform.rotation.z;

        if (items != null && items.Owner != null && items.Owner.tag != "Player" && items.LevelSystem != null)
        {
            
            Owner = items.LevelSystem.GetAiNum(items.Owner);
        }
        else
        {
            Owner = -1;
        }
        if(items.transform.root.tag == "Player" ||
           items.transform.root.tag== "Enemy")
        {
            OnScene = false;
        }
        else
        {
            OnScene = true;
        }

        switch(items.Type)
        {
            case Item.ItemType.Armor:
                {
                    ItemArmor Armor = items.GetComponent<ItemArmor>();
                    if(Armor != null)
                    {
                        armor = Armor.Armor;
                    }
                    ItemWeapon itemWeapon = items.GetComponent<ItemWeapon>();
                    if (itemWeapon != null)
                    {
                        Level = itemWeapon.Level;
                    }
                }
                break;
            case Item.ItemType.Bullet:
                {

                }
                break;
            case Item.ItemType.Food:
                {
                    ItemFood food = items.GetComponent<ItemFood>();
                    if(food != null)
                    {
                        FoodStage = food.nowStage;
                        FullyEaten = food.FullyEaten;
                    }
                }
                break;
            case Item.ItemType.LongRangeWeapon:
                {
                    LongRangeWeapon weapon = items.GetComponent<LongRangeWeapon>();
                    if(weapon != null)
                    {
                        Loaded = weapon.Loaded;
                    }
                    ItemWeapon itemWeapon = items.GetComponent<ItemWeapon>();
                    if (itemWeapon != null)
                    {
                        Level = itemWeapon.Level;
                        Uses = itemWeapon.Uses;
                    }
                }
                break;
            case Item.ItemType.Weapon:
                {
                    ItemWeapon itemWeapon = items.GetComponent<ItemWeapon>();
                    if (itemWeapon != null)
                    {
                        Level = itemWeapon.Level;
                        Uses = itemWeapon.Uses;
                    }
                }
                break;
        }
    }
}

