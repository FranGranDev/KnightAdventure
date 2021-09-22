using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public enum Type { Object, Plant, Chest };
    public Type ObjType;
    public string Name;
    public int Index;
    public float HP;
    public float MaxHP;
    private enum State { Idle, Crushed, Destroyed };
    private State Brokeness = State.Idle;
    public GameObject ItemDrop;
    public int Chanсe = 0;
    public GameObject ItemRareDrop;
    public GameObject EffectOnCrash;
    private Animator anim;
    private SpriteRenderer Sprite;
    private Rigidbody2D Rig;
    private Collider2D Col;
    private IndexSystem IndexSystem;

    private void Awake()
    {
        Rig = GetComponent<Rigidbody2D>();
        Col = GetComponent<Collider2D>();
        Sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        IndexSystem = Resources.Load<IndexSystem>("IndexSystem");

    }
    void Start()
    {
        Rig = GetComponent<Rigidbody2D>();
        Col = GetComponent<Collider2D>();
        Sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        if (ObjType == Type.Object)
        {
            if (HP <= 0)
                HP = 1;
            anim.SetFloat("HpProcent", HP / MaxHP);
        }
        //Name = LanguagesSystem.Language.ObjectName[Index];
    }

    public void Load(int num, int Slot)
    {
        ObjectData Data = SaveSystem.LoadLevel(Slot).ObjectOnSceneData[num];
        Load(Data);
    }
    public void Load(ObjectData Data)
    {
        transform.name = Data.ReferenceName;
        transform.position = new Vector3(
        Data.position[0],
        Data.position[1],
        Data.position[2]);
        transform.localScale = new Vector3(
        Data.scale[0],
        Data.scale[1],
        Data.scale[2]);

        if (ObjType == Type.Object)
        {
            HP = Data.HP;
            MaxHP = Data.MaxHP;
        }
        else if (ObjType == Type.Plant)
        {
            GrowUp Plant = transform.GetComponent<GrowUp>();
            Plant.Stage = Data.Stage;
            Plant.IsReady = Data.isReady;
            if (Data.Field != "")
                Plant.field = GameObject.Find(Data.Field);
        }
        else if (ObjType == Type.Chest)
        {
            Chest chest = transform.GetComponent<Chest>();
            chest.Closed = Data.Closed;
            chest.NeedKey = Data.NeedKey;
            if(Data.Key != -1)
            {
                chest.Key = IndexSystem.Index[Data.Key];
            }
            else
            {
                chest.Key = null;
            }
            chest.Items = new Transform[Data.Items.Length];
            for(int i = 0; i < chest.Items.Length; i++)
            {
                if (Data.Items[i] != null)
                {
                    Transform item = IndexSystem.Index[Data.Items[i].index].transform;
                    chest.Items[i] = item;
                }
            }
        }
    }

    public void GetHit(int Damage, Transform obj)
    {
        if (ObjType == Type.Object)
        {
            if (Brokeness != State.Destroyed)
            {
                HP -= Damage;
                float HpProcent = HP / MaxHP;
                anim.SetFloat("HpProcent", HpProcent);
                anim.Play("Hit", 1);

                //Instantiate(EffectOnCrash, obj.position, transform.rotation, null);
            }
        }
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (ObjType == Type.Object)
        {
            if (HP < MaxHP / 2 && Brokeness == State.Idle)
            {
                Brokeness = State.Crushed;
                if (ItemDrop != null)
                {
                    ItemDrop = Instantiate(ItemDrop, transform.position, transform.rotation, null);
                    ItemDrop.SendMessage("RandomMove");
                }
                if (ItemRareDrop != null)
                {
                    int a = Random.Range(1, 101);
                    if (a <= Chanсe)
                    {
                        ItemRareDrop = Instantiate(ItemRareDrop, transform.position, transform.rotation, null);
                        ItemRareDrop.SendMessage("RandomMove");
                    }
                }
            }
            if (HP < 0 && (Brokeness == State.Idle || Brokeness == State.Crushed))
            {
                Brokeness = State.Destroyed;
            }
        }
    }

}