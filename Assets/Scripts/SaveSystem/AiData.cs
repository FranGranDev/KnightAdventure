using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class AiData
{
    public string ReferenceName;
    public int Name;
    public int LastName;
    public bool Coward;
    public bool Idle;
    public int hp;
    public int maxhp;
    public int LVL;
    public int AiSide;
    public int Work;
    public int State;
    public int PrevState;
    public int Tool;
    public int[] Frined;
    public string[] Metier;
    public string[] WalkTarget;
    public bool Seller;
    public bool Dead;
    public bool Important;

    public bool Right;

    public float VelocityX;
    public float VelocityY;

    public int hairnum;
    public int facenum;
    public int bodynum;
    public float[] hairColor;
    public float skinTone;

    public int stamina;
    public int money;
    public float[] position;
    public float[] startposition;
    public bool GotStartPoint;
    public float rotation;
    public float[] Scale;

    public bool loaded;
    public bool enabled;
    public int index;

    public string Enemy;
    public bool Agressive;
    public bool Fight;

    public int NowSlot;
    public int helmet;
    public int armor;
    public ItemsData[] itemslot;
    public ItemsData[] slot;

    public string Killer;
    public string Home;
    public bool EnteredHouse;

    public AiData(Ai Aiplayer)
    {
        ReferenceName = Aiplayer.transform.name;
        Name = (int)Aiplayer.Name;
        LastName = (int)Aiplayer.LastName;
        Coward = Aiplayer.Coward;
        Idle = Aiplayer.Idle;
        LVL = Aiplayer.LVL;

        VelocityX = Aiplayer.GetComponent<Rigidbody2D>().velocity.x;
        VelocityY = Aiplayer.GetComponent<Rigidbody2D>().velocity.y;

        AiSide = (int)Aiplayer.transform.GetComponent<SideOwn>().ManSide;

        hp = Aiplayer.HP;
        maxhp= Aiplayer.MaxHp;

        Dead = Aiplayer.isDead;
        Important = Aiplayer.Important;
        Fight = Aiplayer.Fight;

        hairnum = Aiplayer.HairNum;
        facenum = Aiplayer.FaceNum;
        bodynum = Aiplayer.BodyNum;

        hairColor = new float[4];
        hairColor[0] = Aiplayer.HairColor.r;
        hairColor[1] = Aiplayer.HairColor.g;
        hairColor[2] = Aiplayer.HairColor.b;
        hairColor[3] = Aiplayer.HairColor.a;
        skinTone = Aiplayer.SkinTone;

        position = new float[3];
        position[0] = Aiplayer.transform.position.x;
        position[1] = Aiplayer.transform.position.y;
        position[2] = Aiplayer.transform.position.z;

        startposition = new float[2];
        startposition[0] = Aiplayer.StartPoint.x;
        startposition[1] = Aiplayer.StartPoint.y;
        GotStartPoint = Aiplayer.GotStartPoint;

        Scale = new float[3];
        Scale[0] = Aiplayer.Body.transform.localScale.x;
        Scale[1] = Aiplayer.Body.transform.localScale.y;
        Scale[2] = Aiplayer.Body.transform.localScale.z;

        rotation = Aiplayer.transform.GetChild(1).rotation.z;

        loaded = Aiplayer.AiLoaded;
        enabled = Aiplayer.AiEnabled;

        NowSlot = Aiplayer.NowSlot;
        itemslot = new ItemsData[Aiplayer.Slot.Length];
        for(int i = 0; i < Aiplayer.Slot.Length; i++)
        {
            if(Aiplayer.Slot[i] != null)
            {
                itemslot[i] = new ItemsData(Aiplayer.Slot[i].GetComponent<Item>());
            }
            else
            {
                itemslot[i] = null;
            }
        }
        Frined = new int[Aiplayer.Friend.Length];
        for (int i = 0; i < Frined.Length; i++)
        {
            if (Aiplayer.Friend[i] != null)
            {
                Frined[i] = Aiplayer.levelSystem.GetAiNum(Aiplayer.Friend[i]);
            }
            else
            {
                Frined[i] = -1;
            }
        }

        WalkTarget = new string[Aiplayer.WalkTargets.Length];
        for (int i = 0; i < Aiplayer.WalkTargets.Length; i++)
        {
            if (Aiplayer.WalkTargets[i] != null)
            {
                WalkTarget[i] = Aiplayer.WalkTargets[i].name;
            }
            else
            {
                WalkTarget[i] = "";
            }
        }

        Metier = new string[Aiplayer.Metier.Length];
        for (int i = 0; i < Metier.Length; i++)
        {
            if (Aiplayer.Metier[i] != null)
            {
                Metier[i] = Aiplayer.Metier[i].name;
            }
            else
            {
                Metier[i] = "";
            }
        }

        if (Aiplayer.Armor != null)
        {
            armor = Aiplayer.Armor.GetComponent<Item>().Index;
        }
        else
        {
            armor = -1;
        }
        if (Aiplayer.Helmet != null)
        {
            helmet = Aiplayer.Helmet.GetComponent<Item>().Index;
        }
        else
        {
            helmet = -1;
        }
        if (Aiplayer.Enemy != null)
        {
            Enemy = Aiplayer.Enemy.name;
        }
        else
        {
            Enemy = "";
        }

        Agressive = Aiplayer.Agressive;

        Work = (int)Aiplayer.Work;
        State = (int)Aiplayer.AiState;
        PrevState = (int)Aiplayer.AiPrevState;

        
        if (Aiplayer.Home != null)
            Home = Aiplayer.Home.name;
        else
            Home = "";
        EnteredHouse = Aiplayer.EnteredHouse;
    }
}
