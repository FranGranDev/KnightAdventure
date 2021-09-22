using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class PlayerData
{
    public string name;
    public int hp;
    public int maxhp;
    public float stamina;
    public int Effect;
    public float EffectTime;
    public int EffectPower;
    public int lvl;
    public int money;
    public float[] position;
    public float rotation;
    public float[] camposition;
    public bool isMan;
    public float[] SkinColor;
    public float[] HairColor;
    public int Hair;
    public int Face;
    public int Body;

    public int nowweaponslot;
    public ItemsData[] weaponslot;
    public ItemsData[] itemslot;

    public ItemsData armor;

    public ItemsData helmet;

    public PlayerData(Character Player)
    {
        hp = Player.Hp;
        maxhp = Player.MaxHp;
        name = Player.Name;
        stamina = Player.Stamina;
        lvl = Player.LVL;
        money = Player.Money;
        position = new float[3];
        position[0] = Player.transform.position.x;
        position[1] = Player.transform.position.y;
        position[2] = Player.transform.position.z;

        Effect = (int)Player.Effect;
        EffectTime = Player.EffectTime;
        EffectPower = Player.EffectPower;

        Hair = Player.HairType;
        Face = Player.FaceType;
        Body = Player.BodyType;
        SkinColor = new float[4];
        SkinColor[0] = Player.SkinColor.x;
        SkinColor[1] = Player.SkinColor.y;
        SkinColor[2] = Player.SkinColor.z;
        SkinColor[3] = Player.SkinColor.w;
        HairColor = new float[4];
        HairColor[0] = Player.HairColor.x;
        HairColor[1] = Player.HairColor.y;
        HairColor[2] = Player.HairColor.z;
        HairColor[3] = Player.HairColor.w;

        rotation = Player.transform.GetChild(1).rotation.z;

        if (Player.MainCamera != null)
        {
            camposition = new float[3];
            camposition[0] = Player.MainCamera.transform.position.x;
            camposition[1] = Player.MainCamera.transform.position.y;
            camposition[2] = Player.MainCamera.transform.position.z;
        }

        nowweaponslot = Player.nowWeaponSlot;
        weaponslot = new ItemsData[Player.WeaponSlot.Length];
        for (int i = 0; i < Player.WeaponSlot.Length; i++)
        {
            if (Player.WeaponSlot[i] != null)
            {
                weaponslot[i] = new ItemsData(Player.WeaponSlot[i].GetComponent<Item>());

            }
            else
            {
                weaponslot[i] = null;
            }
        }

        itemslot = new ItemsData[Player.ItemSlot.Length];
        for (int i = 0; i < Player.ItemSlot.Length; i++)
        {
            if (Player.ItemSlot[i] != null)
            {
                itemslot[i] = new ItemsData(Player.ItemSlot[i].GetComponent<Item>());
            }
            else
            {
                itemslot[i] = null;
            }
        }

        if (Player.Helmet != null)
        {
            helmet = new ItemsData(Player.Helmet.GetComponent<Item>());
        }
        else
        {
            helmet = null;
        }
        if (Player.Armor != null)
        {
            armor = new ItemsData(Player.Armor.GetComponent<Item>());
        }
        else
        {
            armor = null;
        }
    }
}
