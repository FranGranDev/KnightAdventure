using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour
{
    public string Name;
    public int MaxHp;
    public int Hp;
    public float Stamina = 100;
    public float RunSpeed;
    public float WalkSpeed;
    //[HideInInspector]
    public enum EffectType {Null, Drunk, Poison};
    //[HideInInspector]
    public EffectType Effect;
    //[HideInInspector]
    public float EffectTime;
    //[HideInInspector]
    public int EffectPower;
    //[HideInInspector]
    private float speed;
    public float Experience;
    public int LVL = 1;
    public int Money = 100;
    public UIScript UI;
    private int ArmorPoints = 0;
    private int HelmetPoints = 0;
    public int HairType;
    public int FaceType;
    public int BodyType;
    public Vector4 SkinColor;
    public Vector4 HairColor;
    public Transform Helmet;
    public Transform Armor;

    [HideInInspector]
    public bool Dead = false;
    private bool Parry = false;
    private bool Block = false;
    private bool Shock = false;
    private float HitRate = 1f;
    private bool isAttack = false;
    private int MoveNum = 0;
    private int Power = 1;
    public Transform Mouse;
    public SideOwn SideOwn;
    private Transform prevObj = null;
    private Transform prevItem = null;
    private Transform prevAi = null;
    private Transform prevChest;
    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;
    public AudioSource FootAudio0;
    public AudioSource FootAudio1;
    private Material[] Material = new Material[3];
    public Transform Hair;
    public Transform Head;
    public Transform Hand;
    public Transform Body;
    public SpriteRenderer Shadow;

    private Vector2 Direction;

    private float x;
    private float y;
    private float prevLx;
    private float prevLy;
    private float prevRx = 0;
    private float prevRy = 0;
    private float relativelyX = 0;

    private float WeaponDamage = 0;
    private float WeaponWeight = 1;
    public float WeaponLenght = 1;
    private float ArmorWeight = 0f;
    private float HelmetWeight = 0f;

    public Transform Weapon;
    public string WeaponTag;
    public string WeaponType;
    public enum States { Idle, Swing, StartAttack, Attack, AttackIdle, AttackEnd, BlockStart, Block, BlockEnd, Parry, Loading, ChangeWeapon, Other };
    public States HandStates = States.Idle;

    public Transform[] ItemSlot = new Transform[30]; //for hand items
    public Transform[] WeaponSlot = new Transform[4];//for weapon;
    private bool WeaponSkillOn = false;
    public int nowWeaponSlot = 0;

    public int RequestedItem = -1; 


    private Transform Enemy;
    private Transform SelectedEnemy;
    private Transform Dialog;

    private bool canMoveHand = true;
    private bool inDialog = false;
    private bool inInventory = false;

    public camerascript MainCamera;
    public QuestSystem questSystem;
    public LevelSystem levelSystem;
    public DepthSystem DepthSystem;
    private IndexSystem IndexSystem;
    private AudioData AudioData;
    private Item ItemSystem;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        //FootAudio = GetComponent<AudioSource>();
        AudioData = Resources.Load<AudioData>("AudioData");
        IndexSystem = Resources.Load<IndexSystem>("IndexSystem");
        ItemSystem = Resources.Load<Item>("Item");
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<camerascript>();
        SideOwn = GetComponent<SideOwn>();

        Material[0] = Hair.GetComponent<SpriteRenderer>().material;
        Material[1] = Head.GetComponent<SpriteRenderer>().material;
        Material[2] = Body.GetComponent<SpriteRenderer>().material;
    }
    private void Start()
    {
        speed = WalkSpeed;
        anim.SetFloat("AttackSpeed", 1f);

        StartCoroutine("StaminaUp");
        InvokeRepeating("ClearBody", 20f, 20f);

        UI.GetMaxHP(MaxHp);
        UI.GetHP(Hp);

        UI.GetMaxStamina(Stamina);
        UI.GetStamina(Stamina);

        StartCoroutine(LateLoad());
    }
    private IEnumerator LateLoad()
    {
        yield return new WaitForFixedUpdate();
        for (int i = 0; i < ItemSlot.Length; i++)
        {
            if (ItemSlot[i] != null)
            {
                if (!ItemSlot[i].gameObject.scene.IsValid())
                {
                    ItemSlot[i] = Instantiate(ItemSlot[i]);
                    GetItem(i, ItemSlot[i]);
                }
            }
        }
        if(Head.GetChild(0).childCount == 0 && Helmet != null)
        {
            Helmet = Instantiate(Helmet, Head.GetChild(0).position, transform.rotation, Head.GetChild(0)).transform;
            DressHelmet(Head.GetChild(0).GetChild(0));
        }
        if (Body.GetChild(0).childCount == 0 && Armor != null)
        {
            Armor = Instantiate(Armor, Body.GetChild(0).position, transform.rotation, Body.GetChild(0)).transform;
            DressArmor(Body.GetChild(0).GetChild(0));
        }
        switch (Effect)
        {
            case EffectType.Drunk:
                MainCamera.GetEffect((int)Effect, EffectPower);
                break;
        }
        yield break;
    }
    #region Movement

    void Move()
    {
        if (Mathf.Abs(Input.GetAxis("MovementX")) == 1 &&
            Mathf.Abs(Input.GetAxis("MovementY")) == 1)
            x = Input.GetAxis("MovementX") > 0 ? 0.707f : -0.707f;
        else
            x = Input.GetAxis("MovementX");
        if (Mathf.Abs(Input.GetAxis("MovementY")) == 1 &&
            Mathf.Abs(Input.GetAxis("MovementX")) == 1)
            y = Input.GetAxis("MovementY") > 0 ? 0.707f : -0.707f;
        else
            y = Input.GetAxis("MovementY");
        if (Input.GetAxis("Run") == 1 && Stamina > 0f)
        {
            speed = RunSpeed;
            StaminaDown(0.25f);
        }
        else
        {
            speed = WalkSpeed; 
        }

        float CurrentSpeed = speed * Mathf.Sqrt((100 - (ArmorWeight + HelmetWeight + WeaponWeight)) / 100f);
        Vector2 dir = new Vector2(x * CurrentSpeed, y * CurrentSpeed);
        rb.velocity = Vector2.Lerp(rb.velocity, dir, 2f / speed);

        anim.SetFloat("Velocity", rb.velocity.magnitude / WalkSpeed);


        if (Input.GetAxis("MovementX") != 0)
        {
            prevLx = Input.GetAxis("MovementX");
        }
        if (Input.GetAxis("MovementY") != 0)
        {
            prevLy = Input.GetAxis("MovementY");
        }
    }
    void HandMove()
    {
        Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float LocalX = MousePosition.x - Hand.position.x;
        float LocalY = MousePosition.y - Hand.position.y;

        if (LocalX * LocalX + LocalY * LocalY > 0.01f)
        {
            prevLx = MousePosition.x;
            prevLy = MousePosition.y;
        }
        Mouse.position = new Vector3(prevLx, prevLy, -5f);
        if (!canMoveHand)
            return;
        Direction = new Vector2(LocalX, LocalY).normalized;
        Hand.transform.up = Vector2.Lerp(Hand.transform.up, Direction, 0.5f / WeaponWeight);
        relativelyX = prevLx - transform.position.x;
    }
    void ChooseSlot()
    {
        if (HandStates != States.Idle)
            return;
        if (Weapon != null)
        {
            WeaponTag = Weapon.GetComponent<Item>().ItemTag.ToString();
            WeaponType = Weapon.GetComponent<Item>().Type.ToString();
        }
        if (Input.GetButtonDown("Slot1"))
        {
            nowWeaponSlot = 0;
            UI.GetActiveSlot(0);
            anim.Play("ChangeWeapon");
        }
        if (Input.GetButtonDown("Slot2"))
        {
            nowWeaponSlot = 1;
            UI.GetActiveSlot(1);
            anim.Play("ChangeWeapon");
        }
        if (Input.GetButtonDown("Slot3"))
        {
            nowWeaponSlot = 2;
            UI.GetActiveSlot(2);
            anim.Play("ChangeWeapon");
        }
        if (Input.GetButtonDown("Slot4"))
        {
            nowWeaponSlot = 3;
            UI.GetActiveSlot(3);
            anim.Play("ChangeWeapon");
        }
    }
    void ChangeWeapon()
    {
        for (int i = 0; i < WeaponSlot.Length; i++)
        {
            if (WeaponSlot[i] != null)
                WeaponSlot[i].gameObject.SetActive(false);
        }
        Weapon = WeaponSlot[nowWeaponSlot];
        if (WeaponSlot[nowWeaponSlot] != null)
        {
            Weapon.parent = Hand.GetChild(0);
            Weapon.position = Hand.GetChild(0).position;
            Weapon.rotation = Hand.GetChild(0).rotation;
            WeaponWeight = Weapon.GetComponent<Item>().Weight;
            if (Weapon.GetComponent<Item>().Type == Item.ItemType.Weapon)
            {
                WeaponDamage = Weapon.GetComponent<ItemWeapon>().Damage;
                WeaponLenght = Weapon.GetComponent<ItemWeapon>().WeaponReach();
            }
            else
            {
                WeaponDamage = WeaponWeight / 2f;
                WeaponLenght = 0.5f;
            }
            Weapon.gameObject.SetActive(true);
        }
        else
        {
            WeaponDamage = 0;
            WeaponWeight = 1;
        }
    }
    void MoveGamepad()
    {

        if (Input.GetAxis("MovementXgamepad") != 0)
        {
            prevLx = Input.GetAxis("MovementXgamepad");
        }
        if (Input.GetAxis("MovementYgamepad") != 0)
        {
            prevLy = Input.GetAxis("MovementYgamepad");
        }
        x = Input.GetAxis("MovementXgamepad");
        y = Input.GetAxis("MovementYgamepad");

        Vector2 dir = new Vector2(x * speed, y * speed);
        rb.velocity = Vector2.Lerp(rb.velocity, dir, 0.3f);

        float Speed = Mathf.Abs(x) + Mathf.Abs(y);
        anim.SetFloat("Velocity", Speed);

        if (Speed != 0)
        {
            anim.speed = Speed;
        }
        else
        {
            anim.speed = 1f;
        }
    }
    void HandMoveGamepad()
    {
        if (Mathf.Abs(Input.GetAxis("RHorizontal")) > 0.1f)
        {
            prevRx = Input.GetAxis("RHorizontal");
        }
        if (Mathf.Abs(Input.GetAxis("RVertical")) > 0.1f)
        {
            prevRy = Input.GetAxis("RVertical");
        }
        float x = Input.GetAxis("RHorizontal");
        float y = Input.GetAxis("RVertical");
        /*
        if ((x * x + y * y) > 0.9f)
        {
            Vector2 direction = new Vector2(x, y);
            Hand.up = Vector2.Lerp(Hand.up, direction, 1 / WeaponWeight);
        }
        else
        {
            Direction = new Vector2(prevRx, prevRy);
            Hand.up = Vector2.Lerp(Hand.up, Direction, 1 / WeaponWeight);
        }
        */
        //Debug.Log(new Vector2(prevRx, prevRy));

        if ((x * x + y * y) == 1f)
        {
            float rotation = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            Hand.transform.rotation = Quaternion.Lerp(Hand.rotation,
                        Quaternion.Euler(0, 0, rotation - 90), 0.5f / WeaponWeight);
        }
        else
        {
            Direction = new Vector2(prevRx, prevRy);
            float rotation = Mathf.Atan2(prevRy, prevRx) * Mathf.Rad2Deg;
            Hand.transform.rotation = Quaternion.Lerp(Hand.rotation,
                        Quaternion.Euler(0, 0, rotation - 90), 0.5f / WeaponWeight);
        }

    }
    void Animation()
    {
        Shadow.color = new Vector4(1, 1, 1, DayTime.LightIntensity * 0.5f);
        if(Time.time > EffectTime)
        {
            EndEffect(EffectType.Drunk);
        }
        /*
        if (relativelyX > 0)
        {
            Body.GetComponent<SpriteRenderer>().flipX = false;
            Head.GetComponent<SpriteRenderer>().flipX = false;
            if (Armor != null)
                Armor.GetComponent<SpriteRenderer>().flipX = false;
            if (Helmet != null)
                Helmet.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            Body.GetComponent<SpriteRenderer>().flipX = true;
            Head.GetComponent<SpriteRenderer>().flipX = true;
            if (Armor != null)
                Armor.GetComponent<SpriteRenderer>().flipX = true;
            if (Helmet != null)
                Helmet.GetComponent<SpriteRenderer>().flipX = true;
        }
        */
        if (relativelyX > 0)
        {
            Body.localScale = new Vector3(1, 1, 1);
            Head.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            Body.localScale = new Vector3(-1, 1, 1);
            Head.localScale = new Vector3(-1, 1, 1);
        }
        if (Hand.rotation.eulerAngles.z < 360f && Hand.rotation.eulerAngles.z > 180f)
        {
            Hand.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            Hand.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        if (Weapon != null && Weapon.GetComponent<Item>().Type == Item.ItemType.Weapon)
        {
            anim.SetFloat("AttackSpeed", 1.5f / Mathf.Sqrt(WeaponWeight));
        }
        else if(Weapon != null)
        {
            anim.SetFloat("AttackSpeed", 1.5f /Mathf.Sqrt(Weapon.GetComponent<Item>().Weight));
        }
        else
        {
            anim.SetFloat("AttackSpeed", 2f);
        }
        /*
        if (HandStates == States.Loading && Weapon != null && !Weapon.GetComponent<LongRangeWeapon>().isLoading)
        {
            if(WeaponTag == "CrossBow")
                anim.Play("LoadingCrossbowEnd", 1);
            if (WeaponTag == "Gun")
                anim.Play("LoadingGunEnd", 1);
            canMoveHand = true;
        }
        */

        AnimatorStateInfo a = anim.GetCurrentAnimatorStateInfo(1);
        if (a.IsName("Idle") || Weapon == null)
        {
            HandStates = States.Idle;
        }
        else if (isAttack)
        {
            HandStates = States.Attack;
        }
        else if(a.IsName("Attack0Idle") || a.IsName(WeaponTag + "Attack" + MoveNum + "Idle")
            || a.IsName(WeaponTag + "HeavyAttack" + MoveNum + "Idle"))
            {
            HandStates = States.AttackIdle;
        }
        else if (a.IsName("Attack0End") || a.IsName(WeaponTag + "Attack" + MoveNum + "End")
                                  || a.IsName(WeaponTag + "HeavyAttack" + MoveNum + "End"))
        {
            HandStates = States.AttackEnd;
        }
        else if(a.IsName("Attack0Start"))
        {
            HandStates = States.StartAttack;
        }
        else if (!isAttack && (a.IsName("Attack0Start4") ||
        a.IsName(WeaponTag + "Attack" + MoveNum) || a.IsName(WeaponTag + "HeavyAttack" + MoveNum)))
        {
            HandStates = States.Swing;
        }
        else if (a.IsName("BlockIdle"))
        {
            HandStates = States.Block;
        }
        else if (a.IsName("BlockEnd"))
        {
            HandStates = States.BlockEnd;
        }
        else if (a.IsName("ChangeWeapon"))
        {
            HandStates = States.ChangeWeapon;
        }
        else if (Parry)
        {
            HandStates = States.Parry;
        }
        else if((WeaponTag == "CrossBow" || WeaponTag == "Gun" || WeaponTag == "Bow")
         && Weapon.GetComponent<LongRangeWeapon>().isLoading)
        {
            HandStates = States.Loading;
        }
        else
        {
            HandStates = States.Other;
        }

        if (HandStates == States.Loading && speed == RunSpeed)
        {
            if (Weapon == null)
                return;
            switch (Weapon.GetComponent<Item>().ItemTag)
            {
                case Item.Tag.CrossBow:
                    anim.Play("LoadingCrossbowEnd", 1);
                    break;
                case Item.Tag.Gun:
                    anim.Play("LoadingGunEnd", 1);
                    break;
            }
            

            Weapon.GetComponent<LongRangeWeapon>().BreakLoading();
            canMoveHand = true;
        }

        if(prevChest != null && Vector2.Distance(prevChest.position, transform.position) > 2f)
        {
            CloseChest();
        }
    }

    #endregion
    #region Interact

    private void Take()
    {
        if (Input.GetButtonDown("Take") && HandStates == States.Idle)
        {
            Transform item = null;
            if (CheckObject(9) != null && CheckObject(9).transform.parent == null &&
            (Hand.position - CheckObject(9).transform.position).magnitude < 2.5f)
            {
                item = CheckObject(9).transform.root;
                OnWeaponPicked(item.gameObject);
                if (item.GetComponent<Item>().Index == RequestedItem)
                {
                    questSystem.Picked(RequestedItem);
                }
                if (item.GetComponent<Item>().Owner != null &&
                    item.GetComponent<Item>().Owner.tag == "Enemy")
                {
                    item.GetComponent<Item>().Owner.GetComponent<Ai>().Robbery(gameObject);
                }
                WeaponTag = item.GetComponent<Item>().ItemTag.ToString();
                bool inWeaponSlot = false;
                for (int i = 0; i < WeaponSlot.Length; i++)
                {
                    if (WeaponSlot[i] == null)
                    {
                        WeaponSlot[i] = item;
                        inWeaponSlot = true;
                        UI.GetWeapon(i, item);
                        ToHand(item);
                        if (nowWeaponSlot == i)
                        {
                            Weapon = WeaponSlot[i];
                            WeaponWeight = Weapon.GetComponent<Item>().Weight;
                            if (item.GetComponent<Item>().Type == Item.ItemType.Weapon)
                            {
                                WeaponDamage = item.GetComponent<ItemWeapon>().Damage;
                                WeaponLenght = item.GetComponent<ItemWeapon>().WeaponReach();
                            }
                            else
                            {
                                WeaponDamage = WeaponWeight / 2f;
                                WeaponLenght = 0.5f;
                            }
                        }
                        else
                        {
                            WeaponSlot[i].gameObject.SetActive(false);
                        }
                        return;
                    }
                    else if (i == WeaponSlot.Length - 1)
                    {
                        for (int a = 0; a < WeaponSlot.Length; a++)
                        {
                            if (isWeapon(item) && !isWeapon(WeaponSlot[a]))
                            {
                                LetOutWeapon(a);
                                WeaponSlot[a] = item;
                                inWeaponSlot = true;
                                UI.GetWeapon(a, item);
                                ToHand(item);
                                if (nowWeaponSlot == a)
                                {
                                    WeaponSlot[a].gameObject.SetActive(true);
                                    Weapon = WeaponSlot[a];
                                    WeaponDamage = Weapon.GetComponent<ItemWeapon>().Damage;
                                    WeaponWeight = Weapon.GetComponent<ItemWeapon>().Weight;
                                    WeaponLenght = Weapon.GetComponent<ItemWeapon>().WeaponReach();
                                }
                                else
                                {
                                    WeaponSlot[a].gameObject.SetActive(false);
                                }
                                WeaponSlot[nowWeaponSlot].gameObject.SetActive(true);
                                return;
                            }
                            else if (isWeapon(item))
                            {
                                if (isWeapon(item))
                                    inWeaponSlot = true;
                                item.gameObject.SetActive(true);
                                WeaponSlot[nowWeaponSlot].gameObject.SetActive(true);
                            }
                        }
                    }
                }
                if (!inWeaponSlot)
                {
                    for (int i = 0; i < ItemSlot.Length; i++)
                    {
                        if (ItemSlot[i] == null)
                        {
                            ItemSlot[i] = item;
                            ToHand(item);
                            UI.GetItem(i, item);
                            ItemSlot[i].gameObject.SetActive(false);
                            return;
                        }
                    }
                }
                if (WeaponTag == "Bullet")
                {


                }
                if (WeaponTag == "Storage")
                {

                }

            }
        }
    }
    private void DressArmor(Transform item)
    {
        if (Armor != null)
        {
            Armor.SendMessage("PhysicsOn");
            ImpusleThrow(Armor.gameObject, 50f, 40f);
            Armor.parent = null;
        }
        Armor = item;
        ToBody(item);
        ArmorWeight = item.GetComponent<ItemArmor>().Weight;
        ArmorPoints = item.GetComponent<ItemArmor>().Armor;
    }
    private void DressHelmet(Transform item)
    {
        if (Helmet != null)
        {
            Helmet.SendMessage("PhysicsOn");
            ImpusleThrow(Helmet.gameObject, 50f, 40f);
            Helmet.parent = null;
        }
        Helmet = item;
        ToHead(item);
        HelmetWeight = item.GetComponent<ItemArmor>().Weight;
        HelmetPoints = item.GetComponent<ItemArmor>().Armor;
    }
    public void GetArmor(int i, Transform item)
    {
        if(i == 0)
        {
            if(item != null)
            {
                DressHelmet(item);
            }
            else
            {
                Helmet = null;
                HelmetWeight = 0;
                HelmetPoints = 0;
            }
            UI.GetArmor(0, Helmet);
        }
        else
        {
            if (item != null)
            {
                DressArmor(item);
            }
            else
            {
                Armor = null;
                ArmorPoints = 0;
                ArmorWeight = 0;
            }
            UI.GetArmor(1, Armor);
        }
    }
    public void ChangeAppearance()
    {
        anim.SetInteger("BodyType", BodyType);
        Hair.GetComponent<SpriteRenderer>().sprite = levelSystem.HairType[HairType];
        Head.GetComponent<SpriteRenderer>().sprite = levelSystem.FaceType[FaceType];
        Body.GetComponent<SpriteRenderer>().sprite = levelSystem.FaceType[FaceType];
        Material[0].SetColor("_Color", HairColor);
        Material[1].SetColor("_SkinColor", SkinColor);
        Material[2].SetColor("_SkinColor", SkinColor);
    }
    public void SetAppearence(int Hair, int Face, int Body, Vector4 Color, Vector4 SkinTone)
    {
        HairType = Hair;
        FaceType = Face;
        BodyType = Body;
        HairColor = Color;
        SkinColor = SkinTone;

       //ChangeAppearance();
    }

    void LetOut()
    {
        if (Input.GetButtonDown("LetOut"))
        {
            if (Weapon != null && HandStates == States.Idle)
            {
                OnWeaponDrop(Weapon.gameObject);
                if(Weapon.GetComponent<Item>().Owner != null &&
                Weapon.GetComponent<Item>().Owner.tag == "Enemy")
                {
                    Weapon.GetComponent<Item>().Owner.GetComponent<Ai>().GaveBack();
                }
                Weapon.SendMessage("PhysicsOn");
                ImpusleThrow(Weapon.gameObject, 200f, 10f);
                WeaponDamage = 0;
                WeaponWeight = 1;
                WeaponSlot[nowWeaponSlot] = null;
                UI.GetWeapon(nowWeaponSlot, WeaponSlot[nowWeaponSlot]);
                Weapon = null;
            }
        }
    }
    public void LetOutWeapon(int Slot)
    {
        if(Slot < 0)
        {
            Slot = nowWeaponSlot;
        }
        if (WeaponSlot[Slot] != null)
        {
            WeaponSlot[Slot].gameObject.SetActive(true);
            if (WeaponSlot[Slot].GetComponent<Item>().Owner != null &&
            WeaponSlot[Slot].GetComponent<Item>().Owner.tag == "Enemy")
            {
                WeaponSlot[Slot].GetComponent<Item>().Owner.GetComponent<Ai>().GaveBack();
            }
            OnWeaponDrop(WeaponSlot[Slot].gameObject);
            WeaponSlot[Slot].SendMessage("PhysicsOn");
            ImpusleThrow(WeaponSlot[Slot].gameObject, 200f, 50f);
            if (Weapon == WeaponSlot[Slot])
            {
                Weapon = null;
                WeaponDamage = 0;
                WeaponWeight = 1;
            }
            WeaponSlot[Slot].parent = null;
            WeaponSlot[Slot] = null;
            UI.GetWeapon(Slot, WeaponSlot[Slot]);
        }
    }
    public void LetOutItem(int Slot)
    {
        if (ItemSlot[Slot] != null)
        {
            ItemSlot[Slot].gameObject.SetActive(true);
            if (ItemSlot[Slot].GetComponent<Item>().Owner != null &&
            ItemSlot[Slot].GetComponent<Item>().Owner.tag == "Enemy")
            {
                ItemSlot[Slot].GetComponent<Item>().Owner.GetComponent<Ai>().GaveBack();
            }
            OnWeaponDrop(ItemSlot[Slot].gameObject);
            ImpusleThrow(ItemSlot[Slot].gameObject, 200f, 50f);
            ItemSlot[Slot].parent = null;
            ItemSlot[Slot] = null;
            UI.GetItem(Slot, ItemSlot[Slot]);
        }
    }
    public void LetOutArmor(int Slot)
    {
        if(Slot == 0)
        {
            if(Helmet != null)
            {
                if (Helmet.GetComponent<Item>().Owner != null &&
                Helmet.GetComponent<Item>().Owner.tag == "Enemy")
                {
                    Helmet.GetComponent<Item>().Owner.GetComponent<Ai>().GaveBack();
                }
                ImpusleThrow(Helmet.gameObject, 100f, 50f);
                OnWeaponDrop(Helmet.gameObject);
                Helmet.parent = null;
                Helmet = null;
                UI.GetArmor(Slot, Helmet);
            }
            Hair.gameObject.SetActive(true);
        }
        else
        {
            if(Armor != null)
            {
                if (Armor.GetComponent<Item>().Owner != null &&
                Armor.GetComponent<Item>().Owner.tag == "Enemy")
                {
                    Armor.GetComponent<Item>().Owner.GetComponent<Ai>().GaveBack();
                }
                ImpusleThrow(Armor.gameObject, 100f, 50f);
                if (Armor.GetComponent<AudioSource>() != null)
                {
                    OnWeaponDrop(Armor.gameObject);
                }
                Armor.parent = null;
                Armor = null;
                UI.GetArmor(Slot, Armor);
            }
        }
    }
    public bool isWeapon(Transform Item)
    {
        if (Item.GetComponent<Item>().Type == global::Item.ItemType.Weapon ||
          Item.GetComponent<Item>().Type == global::Item.ItemType.LongRangeWeapon)
            return true;
        else
            return false;
    }
    void ToHand(Transform item)
    {
        if (prevItem != null)
        {
            prevItem.GetComponent<Item>().UIOff();
        }
        item.parent = Hand.GetChild(0);
        item.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        item.GetComponent<Rigidbody2D>().angularVelocity = 0;
        item.transform.localScale = new Vector3(1f, 1f, 1f);
        item.rotation = Hand.rotation;
        item.position = Hand.position;
        item.localScale = transform.localScale;
    }
    void ToBody(Transform item)
    {
        UI.GetArmor(1, item);
        item.gameObject.SetActive(true);
        item.parent = Body.GetChild(0);
        item.rotation = Body.GetChild(0).rotation;
        item.position = Body.GetChild(0).position;
        item.localScale = transform.localScale;
    }
    void ToHead(Transform item)
    {
        UI.GetArmor(0, item);
        Hair.gameObject.SetActive(false);
        item.gameObject.SetActive(true);
        item.parent = Head.GetChild(0);
        item.rotation = Head.GetChild(0).rotation;
        item.position = Head.GetChild(0).position;
        item.localScale = transform.localScale;
    }

    void SelectEnemy()
    {
        if (Input.GetButtonDown("GetObject"))
        {
            if (CheckObject(14) != null)
                SelectedEnemy = CheckObject(14).transform;
        }
    }
    void ShowItemName()
    {
        if (CheckObject(9) != null && prevItem != null && prevItem != CheckObject(9))
        {
            prevItem.GetComponent<Item>().UIOff();
            prevItem = null;
        }

        if (CheckObject(9) != null && CheckObject(9).transform.root.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            prevItem = CheckObject(9).transform.root;
            prevItem.GetComponent<Item>().UIOn();
        }
        else if (prevItem != null && prevItem.gameObject.activeSelf)
        {
            prevItem.GetComponent<Item>().UIOff();
            prevItem = null;
        }

    }
    void ShowName()
    {
        if (CheckObject(14) != null && CheckObject(14).transform.root != prevAi && prevAi != null)
        {
            prevAi.GetComponent<Ai>().UIOff();
            prevAi = null;
        }
        if (CheckObject(14) != null && CheckObject(14).transform.root.tag == "Enemy")
        {
            prevAi = CheckObject(14).transform.root;
            prevAi.GetComponent<Ai>().UIOn();
        }
        if (CheckObject(14) == null && prevAi != null)
        {
            prevAi.GetComponent<Ai>().UIOff();
            prevAi = null;
        }

    }
    void ShowText()
    {
        if (CheckObject(16) != null && CheckObject(16).tag == "NamePlate")
        {
            CheckObject(16).GetComponent<Nameplate>().ShowText();
        }

    }
    public float GetImpulse(Transform obj)
    {
        Vector2 Imp1 = obj.GetComponent<Rigidbody2D>().velocity;
        Vector2 Impulse = Imp1 - rb.velocity;
        return Impulse.magnitude;
    }
    public Vector2 Direct(Transform obj)
    {
        return new Vector2(obj.transform.position.x - transform.position.x, obj.transform.position.y - transform.position.y);
    }
    public GameObject CheckObject(int a)
    {
        GameObject obj = null;
        Vector2 Position = new Vector3(Mouse.position.x, Mouse.position.y);
        Collider2D hit = Physics2D.OverlapPoint(Position, 1 << a);

        if (hit != null)
            obj = hit.transform.gameObject;
        return (obj);
    }
    public GameObject CheckObject()
    {
        GameObject obj = null;
        Vector2 Position = new Vector3(Mouse.position.x, Mouse.position.y);
        Collider2D hit = Physics2D.OverlapPoint(Position);

        if (hit != null)
            obj = hit.transform.gameObject;
        return (obj);
    }
    public bool CheckLayer(int x)
    {
        Vector2 Position = new Vector3(Mouse.position.x, Mouse.position.y);
        Collider2D hit = Physics2D.OverlapPoint(Position, 1 << x);

        if (hit != null)
            return true;
        return false;
    }
    void DeleteSelected(Transform enemy)
    {
        if (SelectedEnemy == enemy)
            SelectedEnemy = null;
    }


    public void FindDialog()
    {
        if (Input.GetButtonDown("Take") && rb.velocity.magnitude <= 3f)
        {
            RaycastHit2D hit = Physics2D.Raycast(Hand.position, Direction, 2f, 1 << 14);
            if (hit.collider != null)
            {
                if (CheckObject(14) != null && CheckObject(14).transform.root.GetComponent<Dialogs>() != null && !inDialog)
                {
                    Dialogs ai = CheckObject(14).transform.root.GetComponent<Dialogs>();
                    ai.StartDialog(transform);
                    ai.StartText();
                }
            }
        }
        if (inDialog && rb.velocity.magnitude > 3f)
        {
            EndDialog(-1);
        }
    }
    public void StartDialog(Transform obj)
    {
        inDialog = true;
        canMoveHand = false;
        Dialog = obj;
        Vector3 Effect = (obj.position - transform.position) / 2f;
        MainCamera.GetComponent<camerascript>().StartDialog(obj);
    }
    public void AskAbout(int num)
    {
        Dialog.GetComponent<Dialogs>().AskAbout(num);
    }
    public void AskAboutLand(int num)
    {
        Dialog.GetComponent<Dialogs>().AskAboutLand(num);
    }
    public void AskAboutSeller(int num)
    {
        Dialog.GetComponent<Dialogs>().AskAboutSeller(num);
    }
    public void Compliment(int num)
    {
        Dialog.GetComponent<Dialogs>().Compliment(num);
    }
    public void Hire(int num)
    {
        Dialog.GetComponent<Dialogs>().Hire(num);
    }
    public void Offend(int num)
    {
        Dialog.GetComponent<Dialogs>().Offend(num);
    }
    public void Plunder(int num)
    {
        Dialog.GetComponent<Dialogs>().Plunder(num);
    }
    public void TakeMessage(int num)
    {
        Dialog.GetComponent<Dialogs>().TakeMessage(num);
    }
    public void DialogCallHelp(int num)
    {
        Dialog.SendMessage("CallHelp", num);
        Dialog.SendMessage("Offend", num);
    }
    public void OpenMarket(int num)
    {
        Dialog.SendMessage("OpenMarket", num);
        inInventory = true;
        UI.OpenInventory(inInventory);
    }
    public void StartQuest(int num)
    {
        Dialog.GetComponent<Dialogs>().StartQuest(num);
    }
    public void MainStory(int num)
    {
        Dialog.GetComponent<Dialogs>().MainStory(num);
    }
    public void EndDialog(int num)
    {
        inDialog = false;
        canMoveHand = true;
        if (Dialog != null)
        {
            Dialog.GetComponent<Dialogs>().EndDialog(num); // -1 Leave dialog no words
        }
        MainCamera.GetComponent<camerascript>().EndDialog();
        if (inInventory)
        {
            inInventory = !inInventory;
            UI.OpenInventory(inInventory);
        }
    }

    public void OpenChest(Chest Chest, int KeyIndex)
    {
        if(KeyIndex == -1)
        {
            prevChest = Chest.transform;
            inInventory = true;
            UI.OpenInventory(true);
            UI.ClearMarket();
            for(int i = 0; i < Chest.Items.Length; i++)
            {
                UI.OpenChestMarket(Chest.transform);
                if(Chest.Items[i] != null)
                    UI.GetMarketItem(i, Chest.Items[i].gameObject);
                else
                    UI.GetMarketItem(i, null);
            }
            Chest.Closed = false;
        }
        else if(CountItem(KeyIndex) > 0)
        {
            UI.PrintForTime("Opened", 1f);
            Chest.Closed = false;
            DeleteItem(KeyIndex);
        }
        else
        {
            UI.PrintForTime("No Key", 1f);
        }

    }
    public void CloseChest()
    {
        prevChest.GetComponent<Chest>().Close();
        prevChest = null;
        inInventory = false;
        UI.OpenInventory(false);
        UI.CloseMarket();
    }

    #endregion
    #region Effect
    public void PlaySound(string Name)
    {
        if (AudioData.GetSound(Name) != null)
        {
            Sound sound = AudioData.GetSound(Name);
            AudioSource audio = gameObject.AddComponent<AudioSource>();
            audio.clip = sound.clip;
            audio.volume = sound.volume;
            audio.pitch = sound.pitch;
            audio.spatialBlend = 1;

            audio.Play();
            StartCoroutine(OnSoundStop(audio));
        }
    }
    private IEnumerator OnSoundStop(AudioSource audio)
    {
        yield return new WaitForSeconds(audio.clip.length);
        Destroy(audio);
        yield break;
    }

    public void FootStep(int i)
    {
        if (i == 0)
        {
            FootAudio0.pitch = Random.Range(0.9f, 1f);
            FootAudio0.volume = Random.Range(0.5f, 1f);
            FootAudio0.Play();
        }
        else
        {
            FootAudio1.pitch = Random.Range(0.9f, 1f);
            FootAudio1.volume = Random.Range(0.5f, 1f);
            FootAudio1.Play();
        }
    }

    public void OnWeaponHide()
    {
        if (Weapon == null)
            return;
        Item item = Weapon.GetComponent<Item>();
        switch(item.ItemTag)
        {
            case Item.Tag.Sword:
                item.PlaySoundRandom("SwordHide");
                break;
            case Item.Tag.Axe:
                item.PlaySoundRandom("AxeHide");
                break;
            case Item.Tag.Spear:
                item.PlaySoundRandom("SpearHide");
                break;
            case Item.Tag.Bow:
                item.PlaySoundRandom("ItemHide");
                break;
            case Item.Tag.Bullet:
                item.PlaySoundRandom("ItemHide");
                break;
            case Item.Tag.CrossBow:
                item.PlaySoundRandom("ItemHide");
                break;
            case Item.Tag.Food:
                item.PlaySoundRandom("ItemHide");
                break;
            case Item.Tag.Gun:
                item.PlaySoundRandom("ItemHide");
                break;
            case Item.Tag.Helmet:
                item.PlaySoundRandom("ItemHide");
                break;
            case Item.Tag.Item:
                item.PlaySoundRandom("ItemHide");
                break;
            case Item.Tag.Seeds:
                item.PlaySoundRandom("ItemHide");
                break;
        }
    }
    public void OnWeaponTake()
    {
        if (Weapon == null)
            return;
        Item item = Weapon.GetComponent<Item>();
        switch (item.ItemTag)
        {
            case Item.Tag.Sword:
                item.PlaySoundRandom("SwordTake");
                break;
            case Item.Tag.Axe:
                item.PlaySoundRandom("AxeTake");
                break;
            case Item.Tag.Spear:
                item.PlaySoundRandom("SpearTake");
                break;
            case Item.Tag.Bow:
                item.PlaySoundRandom("ItemTake");
                break;
            case Item.Tag.Bullet:
                item.PlaySoundRandom("ItemTake");
                break;
            case Item.Tag.CrossBow:
                item.PlaySoundRandom("ItemTake");
                break;
            case Item.Tag.Food:
                item.PlaySoundRandom("ItemTake");
                break;
            case Item.Tag.Gun:
                item.PlaySoundRandom("ItemTake");
                break;
            case Item.Tag.Helmet:
                item.PlaySoundRandom("ItemTake");
                break;
            case Item.Tag.Item:
                item.PlaySoundRandom("ItemTake");
                break;
            case Item.Tag.Seeds:
                item.PlaySoundRandom("ItemTake");
                break;
        }
    }
    public void OnWeaponAttack()
    {
        if (Weapon == null)
            return;
        Weapon.GetComponent<Item>().PlaySoundRandom("Swing");
    }
    public void OnWeaponDrop(GameObject obj)
    {
        if (obj == null )
            return;
        Item item = obj.GetComponent<Item>();
        switch (item.ItemTag)
        {
            case Item.Tag.Sword:
                item.PlaySoundRandom("SwordDrop");
                break;
            case Item.Tag.Axe:
                item.PlaySoundRandom("AxeDrop");
                break;
            case Item.Tag.Spear:
                item.PlaySoundRandom("SpearDrop");
                break;
            case Item.Tag.Item:
                item.PlaySoundRandom("ItemDrop");
                break;
            case Item.Tag.Bow:
                item.PlaySoundRandom("ItemDrop");
                break;
            case Item.Tag.Bullet:
                item.PlaySoundRandom("ItemDrop");
                break;
            case Item.Tag.CrossBow:
                item.PlaySoundRandom("ItemDrop");
                break;
            case Item.Tag.Food:
                item.PlaySoundRandom("ItemDrop");
                break;
            case Item.Tag.Gun:
                item.PlaySoundRandom("ItemDrop");
                break;
            case Item.Tag.Helmet:
                item.PlaySoundRandom("ItemDrop");
                break;
            case Item.Tag.Seeds:
                item.PlaySoundRandom("ItemDrop");
                break;
        }
    }
    public void OnWeaponPicked(GameObject obj)
    {
        if (obj == null)
            return;
        Item item = obj.GetComponent<Item>();
        switch (item.ItemTag)
        {
            case Item.Tag.Sword:
                item.PlaySoundRandom("SwordPick");
                break;
            case Item.Tag.Axe:
                item.PlaySoundRandom("AxePick");
                break;
            case Item.Tag.Spear:
                item.PlaySoundRandom("SpearPick");
                break;
            case Item.Tag.Item:
                item.PlaySoundRandom("ItemPick");
                break;
            case Item.Tag.Bow:
                item.PlaySoundRandom("ItemPick");
                break;
            case Item.Tag.Bullet:
                item.PlaySoundRandom("ItemPick");
                break;
            case Item.Tag.CrossBow:
                item.PlaySoundRandom("ItemPick");
                break;
            case Item.Tag.Food:
                item.PlaySoundRandom("ItemPick");
                break;
            case Item.Tag.Gun:
                item.PlaySoundRandom("ItemPick");
                break;
            case Item.Tag.Helmet:
                item.PlaySoundRandom("ItemPick");
                break;
            case Item.Tag.Seeds:
                item.PlaySoundRandom("ItemPick");
                break;
        }
    }
    public void OnWeaponThrow(GameObject obj)
    {
        if (obj == null )
            return;
        Item item = obj.GetComponent<Item>();
        item.PlaySoundRandom("Throw");
    }
    public void OnWeaponBlock(GameObject obj)
    {

    }

    public void GetEffect(int effect, float time)
    {
        switch((EffectType)effect)
        {
            case EffectType.Drunk:
                if (Effect == EffectType.Drunk && EffectPower > 0)
                {
                    EffectTime += time;
                    EffectPower++;
                    if(EffectPower > 8)
                    {
                        Health(-10);
                    }
                }
                else
                {
                    EffectTime = Time.time + time;
                    EffectPower = 1;
                }
                Effect = EffectType.Drunk;
                MainCamera.GetEffect(effect, Mathf.RoundToInt(EffectPower / 2));
                break;
        }
    }
    public void EndEffect(EffectType effect)
    {
        Effect = EffectType.Null;
        EffectPower = 0;
        MainCamera.EndEffect((int)effect);
    }
    public void EndEffect()
    {
        Effect = EffectType.Null;
        EffectPower = 0;
        MainCamera.EndEffect();
    }
    #endregion
    #region Inventory

    public void GetWeapon(int i, Transform item)
    {
        if (i == -1)
            i = nowWeaponSlot;
        if (item != null)
        {
            WeaponSlot[i] = item;
            UI.GetWeapon(i, WeaponSlot[i]);
            WeaponSlot[i].parent = Hand.GetChild(0);
            WeaponSlot[i].position = Hand.GetChild(0).position;
            WeaponSlot[i].rotation = Hand.GetChild(0).rotation;
            if (i == nowWeaponSlot)
            {
                Weapon = WeaponSlot[i];
                Weapon.gameObject.SetActive(true);
                WeaponWeight = Weapon.GetComponent<Item>().Weight;
                if (isWeapon(item))
                {
                    WeaponDamage = Weapon.GetComponent<ItemWeapon>().Damage;
                }
                else
                {
                    WeaponDamage = WeaponWeight / 3f;
                }

            }
            else
            {
                WeaponSlot[i].gameObject.SetActive(false);
            }
            if (item.GetComponent<Item>().Index == RequestedItem)
            {
                questSystem.Picked(RequestedItem);
            }
        }
        else
        {
            WeaponSlot[i] = null;
            UI.GetWeapon(i, null);
        }
    }
    public void GetItem(int i, Transform item)
    {
        if (item != null)
        {
            ItemSlot[i] = item;
            UI.GetItem(i, ItemSlot[i]);
            ItemSlot[i].parent = Hand.GetChild(0);
            ItemSlot[i].position = Hand.GetChild(0).position;
            ItemSlot[i].rotation = Hand.GetChild(0).rotation;
            ItemSlot[i].gameObject.SetActive(false);
            if (item.GetComponent<Item>().Index == RequestedItem)
            {
                questSystem.Picked(RequestedItem);
            }
        }
        else
        {
            ItemSlot[i] = null;
            UI.GetItem(i, null);
        }
    }
    public void UpdateItem(Transform item)
    {
        for(int i = 0; i < WeaponSlot.Length; i++)
        {
            if(WeaponSlot[i] == item)
            {
                UI.GetWeapon(i, item);
                return;
            }
        }
        for(int i = 0; i < ItemSlot.Length; i++)
        {
            if(ItemSlot[i] == item)
            {
                UI.GetItem(i, item);
            }
        }
    }
    public int CountItem(int index)
    {
        int sum = 0;
        for (int i = 0; i < WeaponSlot.Length; i++)
        {
            if (WeaponSlot[i] != null)
            {
                if (WeaponSlot[i].GetComponent<Item>().Index == index)
                    sum++;
            }
        }
        for (int i = 0; i < ItemSlot.Length; i++)
        {
            if (ItemSlot[i] != null)
            {
                if (ItemSlot[i].GetComponent<Item>().Index == index)
                    sum++;
            }
        }
        return sum;
    }
    public void UseKey()
    {

    }
    public void DeleteItem(int Index)
    {
        for (int i = 0; i < WeaponSlot.Length; i++)
        {
            if (WeaponSlot[i] != null && WeaponSlot[i].GetComponent<Item>().Index == Index)
            {
                WeaponSlot[i].GetComponent<Item>().Destroy();
                WeaponSlot[i] = null;
                if(i == nowWeaponSlot)
                {
                    Weapon = null;
                }
                UI.GetWeapon(i, WeaponSlot[i]);
                return;
            }
        }
        for (int i = 0; i < ItemSlot.Length; i++)
        {
            if (ItemSlot[i] != null && ItemSlot[i].GetComponent<Item>().Index == Index)
            {
                ItemSlot[i].GetComponent<Item>().Destroy();
                ItemSlot[i] = null;
                UI.GetItem(i, ItemSlot[i]);
                return;
            }
        }

    }
    public void DeleteItem(Transform item)
    {
        for (int i = 0; i < WeaponSlot.Length; i++)
        {
            if (WeaponSlot[i].transform == item)
            {
                WeaponSlot[i].GetComponent<Item>().Destroy();
                WeaponSlot[i] = null;
                if (i == nowWeaponSlot)
                {
                    Weapon = null;
                }
                UI.GetWeapon(i, WeaponSlot[i]);
                return;
            }
        }
        for (int i = 0; i < ItemSlot.Length; i++)
        {
            if (ItemSlot[i].transform == item)
            {
                ItemSlot[i].GetComponent<Item>().Destroy();
                ItemSlot[i] = null;
                UI.GetItem(i, ItemSlot[i]);
                return;
            }
        }

    }
    void DestroyItem(GameObject item)
    {
        if (item.activeSelf == false)
        {
            item.SetActive(true);
            item.SendMessage("Destroy", transform);
            item.SetActive(false);
        }
        else
        {
            item.SendMessage("Destroy", transform);
        }
    }

    #endregion
    #region Quest
    public void AddRequestItem(int Index)
    {
        RequestedItem = Index;
        for(int i = 0; i < CountItem(Index); i++)
        {
            questSystem.Picked(Index);
        }
    }
    #endregion
    #region GetInfo
    public void GetEnemy(Transform enemy)
    {
        Enemy = enemy;
    }
    public void ManKilled(GameObject obj)
    {

    }

    void GetWeaponReach(float Lenght)
    {
        return;
    }
    void GetDamageInfo(float damage)
    {
        WeaponDamage = damage;
    }
    void GetWeight(float weight)
    {
        WeaponWeight = weight;
    }

    void GetArmorWeight(float weight)
    {
        ArmorWeight = weight;
    }
    void GetArmorPoints(int Points)
    {
        ArmorPoints = Points;
    }

    void GetHelmetWeight(float weight)
    {
        HelmetWeight = weight;
    }
    void GetHelmetPoints(int Points)
    {
        HelmetPoints = Points;
    }

    #endregion
    #region SaveLoad

    public void Save()
    {
        SaveSystem.SavePlayer(this);
    }
    public void Load()
    {
        if (SaveSystem.LoadPlayer() != null)
        {
            PlayerData data = SaveSystem.LoadPlayer();

            Hand.rotation = new Quaternion(Hand.rotation.x,
            Hand.rotation.y, data.rotation, Hand.rotation.w);
            GameObject Cam = GameObject.Find("MainCamera");
            if(Cam != null)
            {
                MainCamera = Cam.GetComponent<camerascript>();
                MainCamera.transform.position = transform.position
              + MainCamera.GetComponent<camerascript>().offset;
            }
            Name = data.name;
            MaxHp = data.maxhp;
            Hp = data.hp;
            UI.GetMaxHP(MaxHp);
            UI.GetHP(Hp);
            Stamina = data.stamina;
            UI.GetMaxStamina(Stamina);
            UI.GetStamina(Stamina);
            LVL = data.lvl;
            Money = data.money;

            Effect = (EffectType)data.Effect;
            EffectPower = data.EffectPower;
            EffectTime = data.EffectTime;

            nowWeaponSlot = data.nowweaponslot;
            UI.GetActiveSlot(nowWeaponSlot);
            
            for (int i = 0; i < WeaponSlot.Length; i++)
            {
                if (WeaponSlot[i] == null)
                {
                    if (data.weaponslot[i] != null)
                    {
                        int index = data.weaponslot[i].index;
                        WeaponSlot[i] = Instantiate(IndexSystem.Item(index), Hand.GetChild(0).position,
                        Hand.GetChild(0).rotation, Hand.GetChild(0)).transform;
                        WeaponSlot[i].GetComponent<Item>().Load(data.weaponslot[i]);

                        UI.GetWeapon(i, WeaponSlot[i]);
                        WeaponSlot[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        WeaponSlot[i] = null;
                        UI.GetWeapon(i, null);
                    }
                }
            }

            for(int i = 0; i < ItemSlot.Length; i++)
            {
                if (data.itemslot[i] != null)
                {
                    int index = data.itemslot[i].index;
                    ItemSlot[i] = Instantiate(IndexSystem.Item(index)).transform;
                    ItemSlot[i].GetComponent<Item>().Load(data.itemslot[i]);
                    ItemSlot[i].parent = Hand.GetChild(0);
                    UI.GetItem(i, ItemSlot[i]);
                    ItemSlot[i].gameObject.SetActive(false);
                }
                else
                {
                    ItemSlot[i] = null;
                    UI.GetItem(i, null);
                }
            }
            HairType = data.Hair;
            FaceType = data.Face;
            BodyType = data.Body;
            SkinColor = new Vector4(data.SkinColor[0], data.SkinColor[1], data.SkinColor[2], data.SkinColor[3]);
            HairColor = new Vector4(data.HairColor[0], data.HairColor[1], data.HairColor[2], data.HairColor[3]);

            ChangeAppearance();
            if (data.helmet != null)
            {
                int index = data.helmet.index;
                Helmet = IndexSystem.Item(index).transform;
            }
            if (data.armor != null)
            {
                int index = data.armor.index;
                Armor = IndexSystem.Item(index).transform;
            }

            //StartCoroutine(LateLoad());
        }
    }

    #endregion
    #region Actions

    private void Action()
    {

        if (Input.GetButtonDown("Take") && CheckObject(16) != null)
        {
            float Distance = Vector2.Distance(transform.position, CheckObject(16).transform.position);

            if (CheckObject(16).tag == "House" && Distance < 1.5f)
            {
                CheckObject(16).transform.root.SendMessage("GetIntoHouse");
            }
            else if (CheckObject(16).tag == "Level" && Distance < 3f)
            {
                CheckObject(16).transform.root.SendMessage("Action");
            }
            else if (CheckObject(16).tag == "Object" && Distance < 2f)
            {
                if (CheckObject(16).transform.root.GetComponent<Object>().ObjType == Object.Type.Object)
                {

                }
                else if (CheckObject(16).transform.root.GetComponent<Object>().ObjType == Object.Type.Plant)
                {
                    CheckObject(16).transform.root.gameObject.GetComponent<GrowUp>().Take();
                }
                else if (CheckObject(16).transform.root.GetComponent<Object>().ObjType == Object.Type.Chest)
                {
                    CheckObject(16).transform.root.GetComponent<Chest>().Open(transform);
                }
            }
        }
        if (Input.GetButtonDown("Inventory"))
        {
            inInventory = !inInventory;
            UI.OpenInventory(inInventory);
        }

        if (Input.GetButtonDown("Cancel"))
        {
            
        }
        
        if (Weapon == null)
            return;

        if(Input.GetButtonDown("Action1") && !inInventory && !inDialog)
        {
            if (WeaponType == "Weapon")
            {
                StartCoroutine("WeaponMove");
            }
            else if(WeaponType == "LongRangeWeapon")
            {
                if (WeaponTag == "Bow")
                {
                    if (!Weapon.GetComponent<LongRangeWeapon>().Loaded)
                    {
                        int ArrowIndex = Weapon.GetComponent<LongRangeWeapon>().BulletIndex;
                        if (CountItem(ArrowIndex) > 0)
                        {
                            Weapon.GetComponent<LongRangeWeapon>().Load();
                        }
                    }
                    else
                    {
                        Weapon.GetComponent<LongRangeWeapon>().KeepTension = true;
                        Weapon.GetComponent<LongRangeWeapon>().Strike();
                    }
                }
                else if (WeaponTag == "CrossBow")
                {
                    if (!Weapon.GetComponent<LongRangeWeapon>().Loaded)
                    {
                        int BoltIndex = Weapon.GetComponent<LongRangeWeapon>().BulletIndex;
                        if (CountItem(BoltIndex) > 0 && rb.velocity.magnitude < 1)
                        {
                            Weapon.GetComponent<LongRangeWeapon>().Load();
                            anim.Play("LoadingCrossbowStart");
                            canMoveHand = false;
                        }
                    }
                    else
                    {
                        Weapon.GetComponent<LongRangeWeapon>().Strike();
                        canMoveHand = true;
                    }
                }
                else if (WeaponTag == "Gun")
                {
                    if (!Weapon.GetComponent<LongRangeWeapon>().Loaded)
                    {
                        int BulletIndex = Weapon.GetComponent<LongRangeWeapon>().BulletIndex;
                        if (CountItem(BulletIndex) > 0 && rb.velocity.magnitude < 1)
                        {
                            Weapon.GetComponent<LongRangeWeapon>().Load();
                            anim.Play("LoadingGunStart");
                            canMoveHand = false;
                        }
                    }
                    else
                    {
                        Weapon.GetComponent<LongRangeWeapon>().Strike();
                        canMoveHand = true;
                    }
                }
            }
            else if (WeaponType == "SimpleItem")
            {
                if (Weapon.GetComponent<Item>().isWeapon)
                {
                    StartCoroutine("WeaponMove");
                }
                else
                {
                    anim.Play("ThrowItem", 1);
                }
            }

        }
        if(Input.GetButtonUp("Action1") && !inInventory && !inDialog)
        {
            if (WeaponTag == "Bow")
            {
                if (Weapon.GetComponent<LongRangeWeapon>().Loaded &&
                    Weapon.GetComponent<LongRangeWeapon>().KeepTension)
                {
                    Weapon.SendMessage("LooseArrow");
                    Weapon.GetComponent<Item>().PlaySoundRandom("BowFire");
                }
            }
        }

        if (Input.GetButton("Action2") && !inInventory)
        {
            if (WeaponType == "Weapon" && Weapon.GetComponent<Item>().isWeapon)
            {
                if (HandStates == States.Idle || HandStates == States.AttackEnd || HandStates == States.AttackIdle)
                {
                    anim.Play("Block", 1);
                }
            }
            if (Input.GetButtonDown("Action2")) //---------------------------Items-----------------------
            {
                if (WeaponType == "Food")
                {
                    if (HandStates == States.Idle)
                    {
                        Weapon.GetComponent<ItemFood>().Eat();
                    }
                }
                else if(WeaponType == "Armor")
                {
                    if(WeaponTag == "Armor")
                    {
                        DressArmor(Weapon);
                        GetWeapon(nowWeaponSlot, null);
                    }
                    else
                    {
                        DressHelmet(Weapon);
                        GetWeapon(nowWeaponSlot, null);
                    }
                }
                else if (WeaponType == "Plant" && CheckObject(12) == null)
                {
                    if (Weapon.GetComponent<ItemPlant>().GrowOnEvething)
                    {
                        if (CheckObject(0) != null && CheckObject(0).tag == "Field")
                            Weapon.SendMessage("Action", CheckObject(0).transform);
                        else
                            Weapon.SendMessage("Action", Mouse.position);
                        GetWeapon(nowWeaponSlot, null);
                    }
                    else if (CheckObject(0) != null && CheckObject(0).tag == "Field")
                    {
                        Weapon.SendMessage("Action", CheckObject(0).transform);
                        GetWeapon(nowWeaponSlot, null);
                    }
                }
            }
        }
        else if(anim.GetCurrentAnimatorStateInfo(1).IsName("BlockIdle"))
        {
            anim.Play("BlockEnd", 1);
        }

        if (Input.GetButtonDown("Throw") && !inDialog && !inInventory)
        {
            if (HandStates == States.Idle)
            {
                if (WeaponTag == "Sword" || WeaponTag == "Axe" || WeaponTag == "Spear")
                {
                    if (WeaponWeight >= 5f)
                        anim.Play(WeaponTag + "HeavyThrow", 1);
                    else
                        anim.Play(WeaponTag + "Throw", 1);
                }
                else if (WeaponTag == "Item")
                {
                    anim.Play("ThrowItem", 1);
                }
            }
        }
        if (Input.GetButtonDown("SuperAction") && !inDialog && !inInventory)
        {
            if (HandStates == States.Idle)
            {
                if (WeaponTag == "Sword")
                {
                    if (WeaponWeight >= 5)
                    {
                        anim.Play(WeaponTag + "Parry", 1);
                    }
                    else
                    {
                        anim.Play(WeaponTag + "Parry", 1);
                    }
                }
                else if (WeaponTag == "Axe")
                {
                    if (WeaponWeight >= 5)
                    {
                        if(Stamina > 90f)
                            anim.Play(WeaponTag + "Spin", 1);
                    }
                    else
                    {
                        if (SelectedEnemy == null)
                        {
                            RaycastHit2D hit = Physics2D.Raycast(Hand.position, Direction, 20f, 1 << 14);
                            if(hit.collider != null)
                                SelectedEnemy = hit.collider.transform.root;
                                              
                        }
                        if(SelectedEnemy != null)
                            anim.Play(WeaponTag + "Following", 1);
                    }
                }
                else if (WeaponTag == "Spear")
                {
                    if (Stamina <= 50)
                        return;
                    StartCoroutine("SpearSkill");
                }

            }
        }
    }


    IEnumerator WeaponMove()
    {
        if (Stamina <= 0)
            yield break;
        int Power = 0;
        float time()
        {
            return anim.GetCurrentAnimatorStateInfo(1).length;
        }
            
        if (HandStates == States.Idle || HandStates == States.BlockEnd)
        {
            MoveNum = 0;

            anim.Play("Attack" + MoveNum + "Start", 1);
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(time());
            if (Input.GetButton("Action1"))
            {
                while (Input.GetButton("Action1"))
                {
                    anim.Play("Attack" + MoveNum + "Start" + Power, 1);
                    if (Power < 4)
                        Power++;
                    yield return new WaitForSeconds(time());
                }
            }
            anim.Play("Attack" + MoveNum + Power, 1);
            //--------------------------------AIBlock----------------------------------------
            RaycastHit2D hit = Physics2D.Raycast(Hand.position, Direction, 5f, 1 << 14);
            if (hit.collider != null)
            {
                hit.collider.transform.root.SendMessage("Defend", 70);
            }
        }
        else if(WeaponTag != "Item" && (anim.GetCurrentAnimatorStateInfo(1).IsName("Attack" + MoveNum + "Idle")
                         || anim.GetCurrentAnimatorStateInfo(1).IsName(WeaponTag + "Attack" + MoveNum + "Idle")
               || anim.GetCurrentAnimatorStateInfo(1).IsName(WeaponTag + "Heavy" + "Attack" + MoveNum + "Idle" )))
        {
             MoveNum++;
             if (WeaponWeight >= Item.Heavy)
             {
                anim.Play(WeaponTag + "Heavy" + "Attack" + MoveNum, 1);
             }
             else
             {
                anim.Play(WeaponTag + "Attack" + MoveNum, 1);
             }
             //--------------------------------AIBlock----------------------------------------
             RaycastHit2D hit = Physics2D.Raycast(Hand.position, Direction, 5f, 1 << 14);
             if (hit.collider != null)
             {
                 hit.collider.transform.root.SendMessage("Defend", 70);
             }
        }
        
        yield break;
    }
    void Throw()
    {
        Vector2 ImpulseVector = Direction * 8f  + rb.velocity / 2f;
        float Impulse = ImpulseVector.magnitude;
        OnWeaponThrow(Weapon.gameObject);
        if (WeaponTag == "Item")
        {
            Weapon.GetComponent<Item>().ThrowItem(transform, 0.5f + LVL / 100f);
            ImpusleThrow(Weapon.gameObject, Impulse * 200f, 500f);
            WeaponSlot[nowWeaponSlot] = null;
            Weapon = null;
            UI.GetWeapon(nowWeaponSlot, WeaponSlot[nowWeaponSlot]);
        }
        else if (WeaponTag == "Sword")
        {
            Weapon.GetComponent<Item>().ThrowItem(transform, 0.75f + LVL / 100f);
            if (WeaponWeight >= Item.Heavy)
                ImpusleThrow(Weapon.gameObject, Impulse * 500f, 0f);
            else
                ImpusleThrow(Weapon.gameObject, Impulse * 500f, 1000f);
            WeaponSlot[nowWeaponSlot] = null;
            Weapon = null;
            UI.GetWeapon(nowWeaponSlot, WeaponSlot[nowWeaponSlot]);
        }
        else if (WeaponTag == "Axe")
        {
            Weapon.GetComponent<Item>().ThrowItem(transform, 1f + LVL / 100f);
            if (WeaponWeight >= Item.Heavy)
                ImpusleThrow(Weapon.gameObject, Impulse * 500f, 500f);
            else
                ImpusleThrow(Weapon.gameObject, Impulse * 400f, 1000f);
            WeaponSlot[nowWeaponSlot] = null;
            Weapon = null;
            UI.GetWeapon(nowWeaponSlot, WeaponSlot[nowWeaponSlot]);
        }
        else if (WeaponTag == "Spear")
        {
            Weapon.GetComponent<Item>().ThrowItem(transform, 1f + LVL / 100f);
            ImpusleThrow(Weapon.gameObject, Impulse * 700f, 0f);
            WeaponSlot[nowWeaponSlot] = null;
            Weapon = null;
            UI.GetWeapon(nowWeaponSlot, WeaponSlot[nowWeaponSlot]);
        }
    }
    void AxeFollowing()
    {
        Weapon.SendMessage("Action", SelectedEnemy);
        Weapon.GetComponent<Item>().ThrowItem(transform, 3);

        ImpusleThrow(Weapon.gameObject, 1000f * Power, 1000f);
        WeaponSlot[nowWeaponSlot] = null;
        Weapon = null;
        UI.GetWeapon(nowWeaponSlot, WeaponSlot[nowWeaponSlot]);
    }
    IEnumerator SpearSkill()
    {
        if(WeaponWeight >= Item.Heavy) 
            anim.Play("SpearHeavyRun", 1);
        else
            anim.Play("SpearHeavyRun", 1);
        while (rb.velocity.magnitude > 1f && Stamina > 0f)
        {
            yield return new WaitForSeconds(0.1f);
        }
        anim.Play("SpearHeavyRunEnd", 1);
        yield break;
    }
    IEnumerator WeaponSkill()
    {
        float a = speed;
        float b = WeaponDamage;
        WeaponSkillOn = true;
        float Power = 0;
        while (Input.GetButton("Action1") && Power < 5)
        {
            Power += 0.5f;
            yield return new WaitForSeconds(0.2f);
            Debug.Log(Power);
        }
        speed = Power / 2f * a;
        WeaponDamage = Power * b;
        yield return new WaitForSeconds(Power / 3f);
        speed = a;
        WeaponDamage = b;
        WeaponSkillOn = false;
        yield break;
    }

    public void OnWeaponLoaded()
    {
        if (WeaponTag == "CrossBow")
            anim.Play("LoadingCrossbowEnd", 1);
        if (WeaponTag == "Gun")
            anim.Play("LoadingGunEnd", 1);
        canMoveHand = true;
    }

    void Step()
    {
        if (WeaponWeight <= 0)
            WeaponWeight = 1;
        rb.AddForce(Hand.up * 1200f / Mathf.Sqrt(WeaponWeight));
    }
    void BackStep()
    {
        if (WeaponWeight <= 0)
            WeaponWeight = 1;
        rb.AddForce(-Hand.up * 800f / Mathf.Sqrt(WeaponWeight));
    }
    void StartAttack(float Hit)
    {
        isAttack = true;
        HitRate = Hit;
        OnWeaponAttack();
    }
    void EndAttack()
    {
        isAttack = false;
    }
    void ThrustAttack()
    {

    }
    void ChopAttack()
    {

    }

    void GetBlockState(int on)
    {
        if (on == 0)
            Block = false;
        else
            Block = true;
    }
    void GetParryState(int on)
    {
        if (on == 0)
            Parry = false;
        else
            Parry = true;
    }
    void SetShock(int a)
    {
        if (a == 1)
            Shock = true;
        else
            Shock = false;
    }
    void SetSpeed(float Sp)
    {
        speed = Sp; //5 - normal
    }
    IEnumerator StaminaUp()
    {
        while(!Dead)
        {
            if(HandStates == States.Idle && Stamina < 100)
            {
                if (rb.velocity.magnitude < 1f)
                {
                    Stamina += 5;
                }
                else if(rb.velocity.magnitude < 5f)
                {
                    Stamina += 2;
                }
                UI.GetStamina(Stamina);
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield break;
    }
    void StaminaDown(float x)
    {
        if (Stamina < 0)
            Stamina = 0;
        Stamina -= x;
        UI.GetStamina(Stamina) ;
    }
    void AttackStaminaDown(float x)
    {
        if (Weapon != null && Weapon.GetComponent<Item>().Type == Item.ItemType.Weapon)
        {
            if (Stamina < 0)
                Stamina = 0;
            Stamina -= Weapon.GetComponent<ItemWeapon>().StaminaDown;
            UI.GetStamina(Stamina);
        }
    }

    void ImpusleThrow(GameObject item, float Force, float Torque)
    {
        item.SendMessage("PhysicsOn");
        item.transform.parent = null;
        item.GetComponent<Rigidbody2D>().simulated = true;
        item.GetComponent<Rigidbody2D>().AddForce(Direction * Force);
        if (Direction.x > 0)
            item.GetComponent<Rigidbody2D>().AddTorque(-Torque);
        else
            item.GetComponent<Rigidbody2D>().AddTorque(Torque);
    }
    float Impulse(Transform enemy)
    {
        if (enemy.GetComponent<Rigidbody2D>() != null)
        {
            float x = enemy.GetComponent<Rigidbody2D>().velocity.x;
            float y = enemy.GetComponent<Rigidbody2D>().velocity.y;
            float x2 = rb.velocity.x;
            float y2 = rb.velocity.y;
            return Mathf.Sqrt((x2 - x) * (x2 - x) + (y2 - y) * (y2 - y)) / 20 + 1;
        }
        else
            return 1;

    }

    void ClearBody()
    {
        int a = 0;
        while(a < transform.childCount && (transform.GetChild(a).tag == "Player" || transform.GetChild(a).tag == "DepthSystem"))
        {
            a++;
        }
        for(int i = a; i < transform.childCount;  i++)
        {
            transform.GetChild(i).transform.parent = null;
        }
    }

    public void Health(int Health)
    {
        if(Hp + Health > MaxHp)
        {
            Hp = MaxHp;
        }
        else
        {
            Hp += Health;
        }
        UI.GetHP(Hp);
    }
    IEnumerator CameraEffect(float zoom)
    {
        Vector3 Effect = (Enemy.position - transform.position) / 2f;
        MainCamera.SendMessage("GetZoom", zoom + 1);
        yield return new WaitForSeconds(2f);
        MainCamera.SendMessage("GetZoom", 1);
        yield break;
    }

    void Attack(Transform enemy)
    {
        SetSpeed(8);
        if (!isAttack || enemy == null  || Parry || SideOwn.Comrade(enemy))
            return;
        int CurrentDamage = Mathf.RoundToInt(WeaponDamage * Impulse(enemy) * HitRate * (1 + LVL / 10f) / 2);
        enemy.GetComponent<Ai>().GetKick(Direct(enemy.transform) * WeaponWeight * 10);
        enemy.GetComponent<Ai>().GetHit(CurrentDamage, Weapon.GetChild(1));
        enemy.GetComponent<Ai>().GetEnemy(transform);

        if(Weapon.GetComponent<Item>().Type == Item.ItemType.Weapon)
        {
            Weapon.GetComponent<ItemWeapon>().HpMinus();
        }
    }
    void AttackAnimal(Transform animal)
    {
        SetSpeed(8);
        if (!isAttack || animal == null || Parry)
            return;
        int CurrentDamage = Mathf.RoundToInt(WeaponDamage * Impulse(animal) * HitRate * (1 + LVL / 10f) / 2);
        animal.GetComponent<Animal>().GetHit(CurrentDamage, Weapon.GetChild(0));
        animal.GetComponent<Animal>().GetKick(Direct(animal.transform) * CurrentDamage);
        if (Weapon.GetComponent<Item>().isWeapon)
        {
            Weapon.GetComponent<ItemWeapon>().HpMinus();
        }
    }
    void AttackObject(Transform Obj)
    {
        if (!isAttack || Parry || CheckObject(16) == null) 
            return; //you will hit object only if your mouse on it
        int CurrentDamage = Mathf.RoundToInt((1 + LVL / 10f) * WeaponDamage * HitRate / 2);
        Obj.GetComponent<Object>().GetHit(CurrentDamage, Weapon);
        Weapon.GetComponent<Item>().PlaySoundRandom("ObjectHit");
        if (Weapon.GetComponent<Item>().isWeapon)
        {
            Weapon.GetComponent<ItemWeapon>().HpMinus();
        }
    }

    public void BrokeWeapon(ItemWeapon item)
    {
        if(Weapon != null)
        {
            Weapon.parent = null;
            GameObject BrokenWeapon = Instantiate(item.BrokenWeapon, Hand.GetChild(0).position, Hand.GetChild(0).rotation, null);
            if (item.Effect != null)
            {
                GameObject Effect = Instantiate(item.Effect, Hand.GetChild(0).position, Hand.GetChild(0).rotation, null);
            }
            Weapon.GetComponent<Item>().Destroy();
            GetWeapon(nowWeaponSlot, BrokenWeapon.transform);
        }
    }

    public void GetHit(int Damage, Transform obj)
    {
        if(Shock)
        {
            Hp -= Mathf.RoundToInt(Damage * (150f - (ArmorPoints + HelmetPoints)) / 100f);
            StartCoroutine("DamageEffect", obj);
            UI.GetHP(Hp);
        }
        else if(HandStates == States.StartAttack)
        {
            Enemy.SendMessage("Sparks");
            Enemy.SendMessage("GetKick", Direct(Enemy.transform) * 10f);
            GetKick(-Direct(Enemy.transform) * 10f);
            StaminaDown(Mathf.Sqrt(Damage));
            Weapon.GetComponent<Item>().PlaySoundRandom("Chamber");
        }
        else if (HandStates == States.Swing)
        {
            Enemy.SendMessage("Sparks");
            Enemy.SendMessage("GetKick", Direct(Enemy.transform) * 10f);
            GetKick(-Direct(Enemy.transform) * 10f);
            StaminaDown(Mathf.Sqrt(Damage));
            Weapon.GetComponent<Item>().PlaySoundRandom("Chamber");
        }
        else if(HandStates == States.Parry && Stamina > 0f)
        {
            StartCoroutine("CameraEffect", 1f);
            StaminaDown(Damage / WeaponWeight);
            Sparks();
            Enemy.SendMessage("GetKick", Direct(Enemy.transform) * 20f);
            Enemy.SendMessage("GetShock", 2.5f);
            Weapon.GetComponent<Item>().PlaySoundRandom("Parry");
        }
        else if(Block)
        {
            if (Stamina > Mathf.Sqrt(Damage) * 1.5f)
            {
                StaminaDown(Mathf.Sqrt(Damage) * 1.5f);
                Enemy.SendMessage("Sparks");
                Enemy.SendMessage("GetKick", Direct(Enemy.transform) * 20f);
                Weapon.GetComponent<Item>().PlaySoundRandom("Block");
            }
            else
            {
                StartCoroutine("GetShock", Mathf.Sqrt(Damage) / 2f);
            }
        }
        else //just damage
        {
            Hp -= Mathf.RoundToInt(Damage * (150f - (ArmorPoints + HelmetPoints)) / 150f);
            StartCoroutine("DamageEffect", obj);
            UI.GetHP(Hp);
        }
        Enemy = obj.root;
    }
    public void GetDistanceHit(int Damage, Transform obj)
    {
        if(HandStates == States.BlockStart)
        {
            obj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            obj.GetComponent<Rigidbody2D>().AddForce(-Direct(obj) * 100f);
            obj.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-100, 100f));
        }
        if(Damage > 0)
        {
            StartCoroutine("DamageEffect", obj);
            Hp -= Mathf.RoundToInt(Damage * (200f - (ArmorPoints + HelmetPoints)) / 100f);
            UI.GetHP(Hp);
        }
        Enemy = obj.root;
    }
    public void EffectHit(int Damage)
    {
        StartCoroutine("DamageEffect", Body);
        Hp -= Damage;
        UI.GetHP(Hp);
    }
    IEnumerator GetShock(float Time)
    {
        anim.Play("Shocked", 1);
        StartCoroutine("CameraEffect", 1f);
        Shock = true;
        yield return new WaitForSeconds(Time);
        Shock = false;
        anim.Play("ShockedEnd", 1);
        yield break;
    }
    public void GetKick(Vector2 Direction)
    {
        rb.velocity = Vector2.zero;
        if(Block)
        {
            rb.AddForce(Direction * 0.75f);
        }
        else if (Hp > 0)
            rb.AddForce(Direction);
    }

    void Sparks()
    {
        if(Weapon != null)
        {
            levelSystem.ParticleSystem.CreateParticle(Particle.ParticlesType.Sparks, Weapon.GetChild(0).position, Weapon.transform.up);
        }
    }
    IEnumerator DamageEffect(Transform obj)
    {
        Vector2 Position = (obj.position + Body.position) / 2;
        levelSystem.ParticleSystem.CreateParticle(Particle.ParticlesType.Blood, Position, obj.transform.up);
        PlaySound("Hit");
        Head.GetComponent<SpriteRenderer>().color = new Vector4(1f, 0.5f, 0.5f, 1);
        Body.GetComponent<SpriteRenderer>().color = new Vector4(1f, 0.5f, 0.5f, 1);
        yield return new WaitForSeconds(0.1f);
        Head.GetComponent<SpriteRenderer>().color = Color.white;
        Body.GetComponent<SpriteRenderer>().color = Color.white;
        yield break;
    }
    IEnumerator Death()
    {
        transform.GetChild(5).gameObject.SetActive(false);
        if (rb.velocity.x >= 0f)
            anim.Play("DieBackward");
        else
            anim.Play("DieForward");
        LetOutWeapon(nowWeaponSlot);

        Dead = true;
        rb.velocity = Vector2.zero;
        transform.localPosition = new Vector3(transform.position.x, transform.position.y, 1f);
        yield break;

    }
    #endregion
    
    void FixedUpdate()
    {
        if (Hp > 0)
        {
            if(!Shock)
                Move();
            if (!Shock)
                HandMove();
            Animation();
        }
        else if(!Dead)
        {
            StartCoroutine("Death");
        }
    }
    void Update()
    {
        if (Hp > 0)
        {
            Take();
            LetOut();
            Action();
            ShowItemName();
            ShowName();
            ShowText();
            ChooseSlot();
            SelectEnemy();
            FindDialog();
        }
    }

}
