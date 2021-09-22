using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class Ai : MonoBehaviour
{
    public enum Names { NoName, Stas, Ivan, VseSlav };
    public Names Name;
    public enum LastNames { Null, Stas, Ivan, VseSlav };
    public LastNames LastName;
    public string GetName()
    {
        string name = "";
        if (Name == Names.NoName)
        {
            name = LanguagesSystem.Language.LifeStaffs[(int)Dialog.HumanType].Name;
        }
        else
        {
            name = LanguagesSystem.Language.Names[(int)Name];
            if (LastName != LastNames.Null)
            {
                name += " " + LanguagesSystem.Language.LastNames[(int)LastName];
            }
        }
        return name;
    }
    public int HP = 100;
    public int MaxHp;
    public int LVL = 1;
    public bool Coward;
    public bool Idle = false;
    public bool Important = false;
    public bool Fight = false;
    public bool Agressive = false;
    public bool LongRangeAttack;

    public enum States { idle, walk, freewalk, GoTo, Follow, attack, defend, work, indialog, runaway, gohome, athome, death };
    public States AiState;

    public enum WorkType { Fermer, Walk, Drink, Eat, NULL };
    public WorkType Work = WorkType.Fermer;
    public GameObject Home;
    [HideInInspector]
    public bool EnteredHouse;
    public Transform Enemy;
    [HideInInspector]
    public Transform QuestTarget;
    public enum AttackState { Idle, StartAttack, Attack, changing, Loading, Block }
    [HideInInspector]
    public AttackState HandState;
    [HideInInspector]
    public States AiPrevState;
    private bool FriendCalled = false;
    public GameObject[] Friend;

    private Transform Character;

    public Transform[] Metier;
    [HideInInspector]
    public bool AiLoaded = true;
    [HideInInspector]
    public bool AiEnabled = true;
    public bool CanOff()
    {
        return (!isDead && !EnteredHouse && AiState != States.GoTo && AiState != States.Follow && AiState != States.attack);
    }
    private float warspeed = 12f;
    private float walkspeed = 4f;
    [HideInInspector]
    public float ArmorPoints = 0;

    private Transform Hair;
    private Transform Head;
    private Transform Eye;
    [HideInInspector]
    public Transform Hand;
    private Transform Arm;
    [HideInInspector]
    public Transform Body;
    private Material[] Material = new Material[3];

    private SpriteRenderer Shadow;
    public Transform Helmet;
    public Transform Armor;
    public AudioSource Step0;
    public AudioSource Step1;
    public GameObject[] Slot = new GameObject[2];
    public int NowSlot = 0;
    [HideInInspector]
    public GameObject Weapon;
    private string WeaponTag;
    private float WeaponDamage;
    private float HitRate;
    private int RandAttack = 0;
    private float WeaponWeight;
    private float Heavy = 3;
    private float ArmorWeight = 0f;
    private float HelmetWeight = 0f;
    private float WeaponReach;
    public int BulletsNum = 0;
    public float Distance = 10f;

    private bool AbleToAttack = true;
    [HideInInspector]
    public bool isWork = false;
    [HideInInspector]
    public bool isSaying = false;
    private bool isAttack = false;
    private bool isLoaded = false;
    private bool isLoading = false;
    private bool isFiring = false;
    private bool isBlock = false;
    private bool Shocked = false;
    [HideInInspector]
    public bool isHandRight;
    private UI_Ai UI;
    [HideInInspector]
    public SideOwn SideOwn;
    public int HairNum;
    public int FaceNum;
    public int BodyNum;
    [Range(0f, 1f)]
    public float SkinTone;
    public Color HairColor;

    private Vector2 target;
    [HideInInspector]
    public bool GotStartPoint;
    public Vector2 StartPoint;
    [HideInInspector]
    public GameObject Killer;
    private Coroutine Loose;
    private Coroutine WorkCour;
    private float speed;
    private Vector2 RandomDirection;
    private Vector2 force;
    [HideInInspector]
    public bool isDead = false;

    public Transform[] WalkTargets = new Transform[11];
    private int CurrentWalkTarget = 0;

    private Vector2 Direction;
    private float nextWaypointdistance = 0.5f;
    private int currentWaypoint = 0;

    Collider2D Col;
    Rigidbody2D rig;
    Seeker seeker;
    Path path;
    Animator anim;
    private Coroutine lastRoutine = null;
    private Coroutine GoToRoutine = null;
    [HideInInspector]
    public int Index;
    private DepthSystem DepthSystem;
    [HideInInspector]
    public Dialogs Dialog;
    public LevelSystem levelSystem;
    private IndexSystem IndexSystem;
    private AudioData AudioData;
    private DayTime Clock;

    void GetDressed()
    {
        if (Head.GetChild(0).childCount == 0)
        {
            if (Helmet != null)
            {
                Helmet = Instantiate(Helmet, Head.GetChild(0).position, Head.rotation, Head.GetChild(0));
                Hair.gameObject.SetActive(false);
            }
            else
            {
                Helmet = null;
            }
        }
        else
        {
            Helmet = Head.GetChild(0).GetChild(0).transform;
        }

        if (Body.GetChild(0).childCount == 0)
        {
            if (Armor != null)
                Armor = Instantiate(Armor, Body.GetChild(0).position, Body.rotation, Body.GetChild(0));
            else
                Armor = null;
        }
        else
        {
            Armor = Body.GetChild(0).GetChild(0).transform;
        }

        if (Helmet != null)
        {
            HelmetWeight = Helmet.GetComponent<ItemArmor>().Weight;
            ArmorPoints += Helmet.GetComponent<ItemArmor>().Armor;
        }
        if (Armor != null)
        {
            ArmorWeight = Armor.GetComponent<ItemArmor>().Weight;
            ArmorPoints += Armor.GetComponent<ItemArmor>().Armor;
        }
    }
    void GetWeapon()
    {
        Transform Arm = Hand.GetChild(0);
        Slot = new GameObject[Arm.childCount];
        for (int i = 0; i < Slot.Length; i++)
        {
            Slot[i] = Arm.GetChild(i).gameObject;
            Slot[i].SetActive(false);
        }
        if (Slot.Length > 0)
        {
            Slot[NowSlot].SetActive(true);
            Weapon = Slot[NowSlot];
        }
        if (Weapon != null)
        {
            if (Weapon.GetComponent<Item>().Type == Item.ItemType.Weapon)
            {
                WeaponWeight = Weapon.GetComponent<ItemWeapon>().Weight;
                WeaponDamage = Weapon.GetComponent<ItemWeapon>().Damage;
                WeaponReach = Weapon.GetComponent<ItemWeapon>().WeaponReach();
                LongRangeAttack = false;
            }
            else if(Weapon.GetComponent<Item>().Type == Item.ItemType.LongRangeWeapon)
            {
                WeaponWeight = Weapon.GetComponent<Item>().Weight;
                WeaponDamage = WeaponWeight / 2f;
                WeaponReach = 0.5f;
                LongRangeAttack = true;
            }
            else
            {
                WeaponWeight = Weapon.GetComponent<Item>().Weight;
                WeaponDamage = WeaponWeight / 2f;
                WeaponReach = 0.5f;
            }
            WeaponTag = Weapon.GetComponent<Item>().ItemTag.ToString();
        }
    }
    void GetAppearence()
    {
        anim.SetInteger("BodyType", BodyNum);
        Hair.GetComponent<SpriteRenderer>().sprite = levelSystem.HairType[HairNum];
        Head.GetComponent<SpriteRenderer>().sprite = levelSystem.FaceType[FaceNum];
        Body.GetComponent<SpriteRenderer>().sprite = levelSystem.FaceType[BodyNum];
        Material[0].SetColor("_Color", HairColor);
        Color Col = Color.HSVToRGB(0.2f, 0.2f, SkinTone * 0.8f);
        Col = new Vector4(Col.r, Col.g, Col.b, 0);
        Material[1].SetColor("_SkinColor", Col);
        Material[2].SetColor("_SkinColor", Col);

        if(Helmet != null)
        {
            Hair.gameObject.SetActive(false);
        }
        else
        {
            Hair.gameObject.SetActive(true);
        }
    }

    private void Awake()
    {
        levelSystem = GameObject.Find("LevelSystem").GetComponent<LevelSystem>();
        AudioData = Resources.Load<AudioData>("AudioData");
        IndexSystem = levelSystem.IndexSystem;
        Character = levelSystem.Character.transform;
        Clock = levelSystem.Clock;

        SideOwn = GetComponent<SideOwn>();
        seeker = GetComponent<Seeker>();
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        Col = GetComponent<Collider2D>();

        Head = transform.GetChild(0);
        Hair = Head.GetChild(1);
        Hand = transform.GetChild(1);
        Arm = Hand.GetChild(0);
        Body = transform.GetChild(2);
        Eye = transform.GetChild(3);
        Shadow = transform.GetChild(7).GetComponent<SpriteRenderer>();

        SideOwn = GetComponent<SideOwn>();
        Dialog = GetComponent<Dialogs>();
        DepthSystem = transform.GetChild(5).GetComponent<DepthSystem>();

        Material[0] = Hair.GetComponent<SpriteRenderer>().material;
        Material[1] = Head.GetComponent<SpriteRenderer>().material;
        Material[2] = Body.GetComponent<SpriteRenderer>().material;

        UI = transform.GetChild(4).GetComponent<UI_Ai>();
    }
    void Start()
    {
        Hair.GetComponent<SpriteRenderer>().sprite = levelSystem.HairType[HairNum];
        Head.GetComponent<SpriteRenderer>().sprite = levelSystem.FaceType[FaceNum];
        Body.GetComponent<SpriteRenderer>().sprite = levelSystem.TorsoType[BodyNum];

        if (!GotStartPoint)
        {
            StartPoint = transform.position;
            GotStartPoint = true;
        }
        
        StartCoroutine(LateLoad(0.1f));

        if (AiEnabled)
        {
            AiOn();
        }
        else
        {
            AiOff();
        }
        InvokeRepeating("FindPath", 0f, 0.5f);
        InvokeRepeating("RandomWay", 0f, 1f);
    }
    IEnumerator LateLoad(float Time)
    {
        GetDressed();
        GetWeapon();
        GetAppearence();
        if (Weapon != null)
        {
            WeaponTag = Weapon.GetComponent<Item>().ItemTag.ToString();
        }
        if (Important && isDead)
        {
            StartCoroutine(Revive(5f));
        }
        UI.GetComponent<UI_Ai>().GetName(GetName());
        UI.GetComponent<UI_Ai>().GetLVL(LVL.ToString());
        HP = HP * (1 + LVL / 10);
        MaxHp = MaxHp * (1 + LVL / 10);
        UI.GetComponent<UI_Ai>().GetMaxHP(MaxHp);
        UI.GetComponent<UI_Ai>().GetHP(HP);
        UI.GetComponent<UI_Ai>().HpTurn(false);
        anim.SetBool("Dead", isDead);
        anim.Play("GOTO", 0);
        yield break;
    }

    public Vector2 Direct(Transform obj)
    {
        if (obj == null)
            return (new Vector2(1, 0));
        return new Vector2(obj.transform.position.x - transform.position.x, obj.transform.position.y - transform.position.y).normalized;
    }
    public Vector2 Direct(Vector2 obj)
    {
        if (obj == null)
            return (new Vector2(1, 0));
        return new Vector2(obj.x - transform.position.x, obj.y - transform.position.y).normalized;
    }

    #region GetInfo

    void GetItemToInventory(Transform Item)
    {
        if (Dialog != null)
        {
            Dialog.GetMarketItem(Item);
        }
    }

    void GetArmorWeight(float weight)
    {
        ArmorWeight = weight;
    }
    void GetArmorPoints(int Points)
    {
        ArmorPoints += Points;
    }
    void GetHelmetWeight(float weight)
    {
        HelmetWeight = weight;
    }
    void GetHelmetPoints(int Points)
    {
        ArmorPoints += Points;
    }

    public void GetEnemy(Transform obj)
    {
        if (Enemy == obj)
            return;
        Fight = true;
        if (Loose != null)
        {
            StopCoroutine(Loose);
        }
        if (obj != null)
        {
            Enemy = obj;
            target = obj.position;
        }
        AiPrevState = AiState;
        if (Dialog != null)
        {
            Dialog.CanDialog = false;
            Dialog.QuestScript.MainQuestAttacked();
        }
        if (UI != null)
        {
            UI.GetComponent<UI_Ai>().HpTurn(true);
        }
        if (Weapon == null || !Weapon.GetComponent<Item>().isWeapon)
        {
            FindWeapon();
        }
        UIOff();
        
    }
    public IEnumerator LoseEnemy(float Time)
    {
        yield return new WaitForSeconds(Time);
        Enemy = null;
        Fight = false;
        if (Dialog != null)
        {
            Dialog.CanDialog = true;
        }
        if (UI != null)
        {
            UI.GetComponent<UI_Ai>().HpTurn(false);
        }
        FindItem();
        yield break;
    }
    public IEnumerator CallHelp(float Time)
    {
        yield return new WaitForSeconds(Time);
        for (int i = 0; i < Friend.Length; i++)
        {
            if (Friend[i] == null || Enemy == null)
                yield break;
            Friend[i].SendMessage("GetEnemy", Enemy);
            yield return new WaitForSeconds(0.1f);
        }
        yield break;
    }
    public void ManKilled(GameObject obj)
    {
        if(obj.transform == Enemy)
        {
            LoseEnemy(1f);
        }
    }

    #endregion
    #region Movement
    public void Walk()
    {
        if (WalkTargets.Length < 1)
            return;
        speed = walkspeed;
        int TCount = 0;
        for (; TCount < WalkTargets.Length; TCount++)
            if (WalkTargets[TCount] == null)
                break;
        if (CurrentWalkTarget >= TCount)
            CurrentWalkTarget = 0;
        target = WalkTargets[CurrentWalkTarget].position;
        if (Vector2.Distance(target, transform.position) < 2f)
        {
            if (TCount == 1)
                AiState = States.idle;
            CurrentWalkTarget++;
        }
        Goto(0f);
    }
    public void HandRotation()
    {
        if (AiState == States.attack)
        {
            if (HandState == AttackState.Idle)
            {
                Hand.transform.up = Vector2.Lerp(Hand.transform.up, Direct(target), 0.1f * (1 + LVL / 10f) / WeaponWeight);
            }
            else if (HandState == AttackState.StartAttack)
            {
                Hand.transform.up = Vector2.Lerp(Hand.transform.up, Direct(target), 0.05f * (1 + LVL / 10f) / WeaponWeight);
            }
            else if (HandState == AttackState.Block)
            {

            }
            else if (HandState == AttackState.Loading)
            {

            }

        }
    }
    public void FootStep(int i)
    {
        if (i == 0)
        {
            Step0.pitch = Random.Range(0.9f, 1f);
            Step0.volume = Random.Range(0.6f, 1f);
            Step0.Play();
        }
        else
        {
            Step1.pitch = Random.Range(0.9f, 1f);
            Step1.volume = Random.Range(0.6f, 1f);
            Step1.Play();
        }
    }
    void Go(Vector2 Direction)
    {
        float CurrentSpeed = speed * Mathf.Sqrt((100 - ArmorWeight - HelmetWeight - WeaponWeight) / 100f);
        rig.AddForce((Direction + RandomDirection * 0.1f).normalized * speed);
    }
    void Goto(float Distance)
    {
        if (path == null || Shocked || !AiEnabled)
            return;
        if (currentWaypoint >= path.vectorPath.Count)
            return;
        float CurrentSpeed = speed * Mathf.Sqrt((100 - ArmorWeight - HelmetWeight - WeaponWeight) / 100f);
        if (Vector2.Distance(transform.position, target) > Distance)
        {
            Direction = ((Vector2)path.vectorPath[currentWaypoint] - (Vector2)transform.position).normalized;
            force = (Direction + RandomDirection * 0.1f) * CurrentSpeed * 100f * Time.deltaTime;
            rig.AddRelativeForce(force);
        }
        float distance = Vector2.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointdistance)
        {
            currentWaypoint++;
        }
    }
    void Stay(Vector2 Point)
    {
        if (Vector2.Distance(Point, transform.position) > 1f)
        {
            FlipTo(rig.velocity.x);
            speed = warspeed;
            target = StartPoint;
            if (rig.velocity.magnitude > 1f)
                Hand.transform.up = Vector2.Lerp(Hand.transform.up, Direct(target), 0.1f * (1 + LVL / 10f) / WeaponWeight);
            Goto(0);
        }
        else
        {
            rig.velocity = rig.velocity * 0.5f;
        }
    }
    void RandomWay()
    {
        RandomDirection = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
    }
    void FindPath()
    {
        if (target != null && seeker.IsDone())
        {
            seeker.StartPath(rig.position, target, PathComplete);
        }
    }
    void PathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    void Animation()
    {
        if (!isDead && !EnteredHouse)
        {
            Shadow.color = new Vector4(1, 1, 1, DayTime.LightIntensity * 0.5f);
        }
        AnimatorStateInfo a = anim.GetCurrentAnimatorStateInfo(1);
        if (a.IsName("Idle") || Weapon == null)
        {
            HandState = AttackState.Idle;

        }
        else if (a.IsName(WeaponTag + "StartAttack" + RandAttack) ||
        a.IsName("Heavy" + WeaponTag + "StartAttack" + RandAttack))
        {
            HandState = AttackState.StartAttack;
        }
        else if (a.IsName(WeaponTag + "Attack" + RandAttack) ||
        a.IsName("Heavy" + WeaponTag + "Attack" + RandAttack))
        {
            HandState = AttackState.Attack;
        }
        else if (a.IsName("ChangeWeapon"))
        {
            HandState = AttackState.changing;
        }
        else if ((WeaponTag == "CrossBow" || WeaponTag == "Gun" || WeaponTag == "Bow")
         && isLoading)
        {
            HandState = AttackState.Loading;
        }
        

        anim.SetFloat("Velocity", 2 * rig.velocity.magnitude / speed);
        anim.SetFloat("AttackSpeed", Mathf.Sqrt(2f / WeaponWeight));
        if (isAttack && rig.bodyType == RigidbodyType2D.Dynamic)
        {
            rig.velocity = Vector2.zero;
        }


    }
    void FlipTo(float x)
    {
        if (x >= 0.1f)
        {
            Head.localScale = new Vector3(1, 1, 1);
            Body.localScale = new Vector3(1, 1, 1);
        }
        else if (x <= -0.1f)
        {
            Head.localScale = new Vector3(-1, 1, 1);
            Body.localScale = new Vector3(-1, 1, 1);
        }
        if (Hand.rotation.eulerAngles.z < 360f && Hand.rotation.eulerAngles.z > 180f)
        {
            Hand.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            Hand.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

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
    public void PlaySoundRandom(string Name)
    {
        if (AudioData.GetSound(Name) != null)
        {
            Sound sound = AudioData.GetSound(Name);
            AudioSource audio = gameObject.AddComponent<AudioSource>();
            audio.clip = sound.clip;
            audio.volume = sound.volume * Random.Range(0.8f, 1.2f); ;
            audio.pitch = sound.pitch * Random.Range(0.8f, 1.2f); ;
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

    public void AiOff()
    {
        if (!CanOff())
            return;
        AiEnabled = false;
        rig.velocity = Vector2.zero;
        rig.bodyType = RigidbodyType2D.Kinematic;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        transform.GetChild(6).gameObject.SetActive(true);
    }
    public void AiOn()
    {
        if (isDead)
            return;
        AiEnabled = true;
        rig.bodyType = RigidbodyType2D.Dynamic;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

        if (!AiLoaded)
        {
            int Index = levelSystem.GetAiNum(gameObject);
            Load(Index, NowData.LevelNum);
        }
    }

    void EnterToHouse()
    {
        EnteredHouse = true;
        rig.bodyType = RigidbodyType2D.Static;
        Shadow.GetComponent<SpriteRenderer>().color = Color.clear;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    void ExitHouse()
    {
        EnteredHouse = false;
        Col.enabled = true;
        AiState = AiPrevState;
        rig.bodyType = RigidbodyType2D.Dynamic;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        Shadow.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void UIOn()
    {
        if (!Fight && !EnteredHouse && UI != null)
            UI.NameTurn(true);
    }
    public void UIOff()
    {
        if (UI != null)
            UI.NameTurn(false);
    }

    void OnTriggerStay2D(Collider2D collision)
    {

    }
    void OnTriggerExit2D(Collider2D collision)
    {

    }

    #endregion
    #region Work
    IEnumerator Fermer()
    {
        if (Metier.Length == 0 || !FindSeed())
        {
            yield break;
        }
        isWork = true;
        speed = walkspeed;
        for (int i = 0; i < Metier.Length; i++)
        {
            while (Vector2.Distance(Metier[i].position, transform.position) > 1f)
            {
                target = Metier[i].position;
                Goto(0f);
                yield return new WaitForFixedUpdate();
            }
            if (Metier[i].GetComponent<Field>().Planted)
            {
                if (Metier[i].GetComponent<Field>().isReady)
                {
                    Metier[i].GetComponent<Field>().Take();
                    yield return new WaitForSeconds(1f);
                    for (int a = 0; a < Metier[i].GetComponent<Field>().DropItem.Length; a++)
                    {
                        if (Metier[i].GetComponent<Field>().DropItem[a] != null)
                        {
                            if (Dialog != null && Dialog.GetFreeSlot() > 0)
                            {
                                GetItemToInventory(Metier[i].GetComponent<Field>().DropItem[a].transform);
                            }
                            else
                            {
                                Metier[i].GetComponent<Field>().DropItem[a].GetComponent<Item>().Destroy();
                            }
                            Metier[i].GetComponent<Field>().DropItem[a] = null;
                        }
                    }
                    for (int a = 0; a < Metier[i].GetComponent<Field>().DropSeed.Length; a++)
                    {
                        if (Metier[i].GetComponent<Field>().DropSeed[a] != null)
                        {
                            if (Dialog != null && Dialog.GetFreeSlot() > 0)
                            {
                                GetItemToInventory(Metier[i].GetComponent<Field>().DropSeed[a].transform);
                            }
                            else
                            {
                                Metier[i].GetComponent<Field>().DropSeed[a].GetComponent<Item>().Destroy();
                            }
                            Metier[i].GetComponent<Field>().DropSeed[a] = null;
                        }
                    }
                }
            }
            else
            {
                rig.velocity = Vector2.zero;

                yield return new WaitForSeconds(1f);
                GameObject Seed = Instantiate(Weapon, Hand.position, Hand.rotation, Hand);
                Seed.SendMessage("Action", Metier[i]);
            }
        }
        isWork = false;
        yield break;
    }
    IEnumerator RandomWalk()
    {
        isWork = true;
        speed = walkspeed / 2f;
        float Radius = 10f;
        float x;
        float y;
        if (Random.Range(0, 2) == 0)
            x = Random.Range(0.5f * Radius, Radius);
        else
            x = -Random.Range(0.5f * Radius, Radius);
        if (Random.Range(0, 2) == 0)
            y = Random.Range(0.5f * Radius, Radius);
        else
            y = -Random.Range(0.5f * Radius, Radius);
        target = (Vector2)StartPoint + new Vector2(x, y);


        int Frames = 0;
        while (Vector2.Distance(transform.position, target) > 1f)
        {
            Goto(0f);
            Frames++;
            if (Frames >= 10 && rig.velocity.magnitude < 0.2f)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        isWork = false;
        yield break;
    }
    IEnumerator Drink()
    {
        isWork = true;
        anim.Play("Drink", 1);
        if(Head.localScale.x > 0)
        {
            Hand.transform.up = new Vector2(1, 0);
        }
        else
        {
            Hand.transform.up = new Vector2(-1, 0);
        }
        yield return new WaitForSeconds(Random.Range(5, 15));
        isWork = false;
        yield break;
    }
    IEnumerator Eat()
    {
        isWork = true;
        anim.Play("Eat", 1);
        yield return new WaitForSeconds(Random.Range(4, 13));
        isWork = false;
        yield break;
    }
    IEnumerator RandomPhrase()
    {
        isSaying = true;
        yield return new WaitForSeconds(Random.Range(5, 25));
        UI.PrintForTime("Эх сука, жызнб боль", 4);
        isSaying = false;
        yield break;
    }
    #endregion
    #region Quest
    public void StartDialog()
    {
        if (!Fight && Dialog.CanDialog)
        {
            AiPrevState = AiState;
            AiState = States.indialog;

            if(WorkCour != null)
            {
                StopCoroutine(WorkCour);
                isWork = false;
            }
        }
    }
    public void EndDialog()
    {
        AiState = AiPrevState;
        if (AiState == States.freewalk)
        {
            WorkCour = StartCoroutine(RandomWalk());
        }
        else if (AiState == States.work)
        {
            switch (Work)
            {
                case WorkType.Fermer:
                    WorkCour = StartCoroutine(Fermer());
                    break;
            }
        }
        else if (AiState == States.freewalk)
        {
            WorkCour = StartCoroutine(RandomWalk());
        }
    }

    public void GoToTarget(Transform Target, bool fast)
    {
        if (!AiEnabled)
        {
            AiOn();
        }
        if (GoToRoutine != null)
            StopCoroutine(GoToRoutine);
        GoToRoutine = StartCoroutine(GoToTargetCour(Target, fast));
    }
    IEnumerator GoToTargetCour(Transform Target, bool fast)
    {
        AiPrevState = AiState;
        AiState = States.GoTo;
        speed = fast ? warspeed : walkspeed;
        target = Target.transform.position;
        while (((Vector2)transform.position - target).magnitude > 1.5f)
        {
            target = Target.transform.position;
            Goto(0f);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(5f);
        StartPoint = transform.position;
        AiState = AiPrevState;
        yield break;
    }

    public void StartFollow(Transform Target)
    {
        if (!AiEnabled)
        {
            AiOn();
        }
        AiPrevState = AiState;
        AiState = States.Follow;
        QuestTarget = Target;
    }
    public void EndFollow()
    {
        AiState = AiPrevState;
        QuestTarget = null;
    }

    public void AttackTarget(Transform Player)
    {
        if (!AiEnabled)
        {
            AiOn();
        }
        GetEnemy(Player);
    }

    public void AddToQuest(GameObject Ai)
    {
        Killer = Ai;
        UI.GetComponent<UI_Ai>().Requested(true);
    }
    public void DeleteFromQuest()
    {
        UI.GetComponent<UI_Ai>().Requested(false);
    }
    #endregion
    #region AttackStuff

    void Attack(Transform enemy)
    {
        if (isAttack && !SideOwn.Comrade(enemy))
        { 
            int CurrentDamage = Mathf.RoundToInt(WeaponDamage * HitRate * (1 + LVL / 10f) / 2f);
            enemy.SendMessage("GetEnemy", transform);
            enemy.gameObject.SendMessage("GetKick", Direct(enemy.transform) * WeaponDamage * 2f);
            if (enemy.tag == "Enemy")
            {
                enemy.GetComponent<Ai>().GetHit(CurrentDamage, Weapon.transform.GetChild(1));
            }
            else
            {
                enemy.GetComponent<Character>().GetHit(CurrentDamage, Weapon.transform.GetChild(1));
            }

            if (Weapon.GetComponent<Item>().isWeapon)
            {
                Weapon.GetComponent<ItemWeapon>().HpMinus();
            }
        }
    }
    void AttackObject(Transform obj)
    {

    }
    void AttackAnimal(Transform animal)
    {

    }
    IEnumerator Fire()
    {
        if (isFiring && HandState == AttackState.Idle)
        {
            if (WeaponTag == "Bow")
            {

            }
            else if (WeaponTag == "Gun")
            {
                if (Weapon.GetComponent<LongRangeWeapon>().Loaded)
                {
                    Weapon.GetComponent<LongRangeWeapon>().Strike();
                    yield return new WaitForSeconds(1f);
                }
            }
            else if (WeaponTag == "CrossBow")
            {
                if (Weapon.GetComponent<LongRangeWeapon>().Loaded)
                {
                    Weapon.GetComponent<LongRangeWeapon>().Strike();
                    yield return new WaitForSeconds(1f);
                }
            }
            isFiring = false;
        }
        yield break;
    }
    IEnumerator LoadWeapon()
    {
        if (BulletsNum > 0)
        {
            yield return new WaitForSeconds(0.5f);
            if (WeaponTag == "Gun")
            {
                Weapon.GetComponent<LongRangeWeapon>().Load();
                anim.Play("LoadingGunStart", 1);
            }
            else if (WeaponTag == "CrossBow")
            {
                Weapon.GetComponent<LongRangeWeapon>().Load();
                anim.Play("LoadingGunStart", 1);
            }
        }
        yield break;
    }
    public void OnWeaponLoaded()
    {
        if (WeaponTag == "Gun")
        {
            anim.Play("LoadingGunEnd", 1);
        }
        else if (WeaponTag == "CrossBow")
        {
            anim.Play("LoadingGunEnd", 1);
        }
    }

    void Defend(int Chanse)
    {
        if(Weapon == null || Weapon.GetComponent<Item>().Type != Item.ItemType.Weapon || AiState != States.attack)
            return;
        float WeaponLenght;
        if (Enemy.tag == "Player")
            WeaponLenght = Enemy.GetComponent<Character>().WeaponLenght;
        else
            WeaponLenght = Enemy.GetComponent<Ai>().WeaponReach;

        int Choouse = Random.Range(0, 2);
        if (Vector2.Distance(Enemy.position, transform.position) > WeaponReach * 1.5f && Choouse == 0)
        {
            rig.velocity = Vector2.zero;
            rig.AddForce(-Direct(Enemy) * 200f);
        }
        else if (HandState != AttackState.StartAttack)
        {
            int Rand = Random.Range(0, 100);
            if (Chanse >= Rand)
            {
                float RandTime = Random.Range(1f, 3f);
                StartCoroutine("Block", RandTime);
            }

        }
    }
    IEnumerator Block(float Time)
    {
        anim.SetBool("Block", true);
        isBlock = true;
        anim.Play("Block", 1);
        yield return new WaitForSeconds(Time);
        anim.SetBool("Block", false);
        isBlock = false;
    }

    public void LetOutItem(Transform item)
    {
        if (item.gameObject.scene.IsValid())
        {
            item.gameObject.SetActive(true);
            item.GetComponent<Item>().RandomMove();
        }
        else
        {
            Transform obj = Instantiate(item, Hand.transform.position, Hand.transform.rotation, null);
            obj.GetComponent<Item>().RandomMove();
        }
    }
    public void LetOutWeapon()
    {
        if (Weapon != null)
        {
            Weapon.transform.parent = null;
            Weapon.GetComponent<Rigidbody2D>().AddForce(Hand.transform.up);
            if (Weapon == Slot[NowSlot])
                Slot[NowSlot] = null;
        }


        Invoke("ChangeWeapon", 1f);
    }
    public void TakeWeapon()
    {
        for (int i = 0; i < Slot.Length; i++)
        {
            Slot[i].SetActive(false);
        }
        if (Slot[NowSlot] != null)
        {
            Slot[NowSlot].SetActive(true);
            Weapon = Slot[NowSlot];
            WeaponTag = Weapon.GetComponent<Item>().ItemTag.ToString();
            if (Weapon.GetComponent<Item>().Type == Item.ItemType.Weapon)
            {
                WeaponWeight = Weapon.GetComponent<ItemWeapon>().Weight;
                WeaponDamage = Weapon.GetComponent<ItemWeapon>().Damage;
                WeaponReach = Weapon.GetComponent<ItemWeapon>().WeaponReach();
            }
            
        }
        else
        {
            Weapon = null;
        }
    }
    public bool FindWeapon()
    {
        if (Weapon != null && (Weapon.GetComponent<Item>().Type == Item.ItemType.Weapon ||
        Weapon.GetComponent<Item>().Type == Item.ItemType.LongRangeWeapon))
            return true;
        for(int i = 0; i < Slot.Length; i++)
        {
            if (Slot[i] == null)
                return false;
            if (Slot[i].GetComponent<Item>().Type == Item.ItemType.Weapon)
            {
                LongRangeAttack = false;
                NowSlot = i;
                anim.Play("ChangeWeapon", 1);
                return true;
            }
            else if (Slot[i].GetComponent<Item>().Type == Item.ItemType.LongRangeWeapon)
            {
                LongRangeAttack = true;
                NowSlot = i;
                anim.Play("ChangeWeapon", 1);
                return true;
            }
        }
        return false;
    }
    public bool FindItem()
    {
        if (Weapon != null && Weapon.GetComponent<Item>().Type != Item.ItemType.Weapon &&
        Weapon.GetComponent<Item>().Type != Item.ItemType.LongRangeWeapon)
        {
            return true;
        }
        for (int i = 0; i < Slot.Length; i++)
        {
            if (Slot[i] == null)
                return false;
            if (Slot[i].GetComponent<Item>().Type != Item.ItemType.Weapon &&
               Slot[i].GetComponent<Item>().Type != Item.ItemType.LongRangeWeapon)
            {
                NowSlot = i;
                anim.Play("ChangeWeapon", 1);
                return true;
            }
        }
        return false;
    }
    public bool FindSeed()
    {
        if (Weapon != null && Weapon.GetComponent<Item>().Type == Item.ItemType.Plant)
        {
            return true;
        }
        for (int i = 0; i < Slot.Length; i++)
        {
            if (Slot[i] == null)
                return false;
            if (Slot[i].GetComponent<Item>().Type == Item.ItemType.Plant)
            {
                LongRangeAttack = false;
                NowSlot = i;
                anim.Play("ChangeWeapon", 1);
                return true;
            }
        }
        return false;
    }
    public void ChangeWeapon()
    {
        if (NowSlot + 2 > Slot.Length)
        {
            NowSlot = 0;
        }
        else
        {
            NowSlot += 1;
        }
        TakeWeapon();
    }


    void RandomAttackMove()
    {
        if (Weapon == null || !Weapon.GetComponent<Item>().isWeapon)
            return;
        if (HandState == AttackState.Idle)
        {
            if (WeaponWeight >= Heavy)
            {
                if (rig.velocity.magnitude > 2.6f)
                {
                    RandAttack = 0;
                    anim.Play("Heavy" + WeaponTag + "StartAttack" + RandAttack, 1);
                }
                else
                {
                    RandAttack = Random.Range(1, 3);
                    anim.Play("Heavy" + WeaponTag + "StartAttack" + RandAttack, 1);
                }
            }
            else
            {
                if (rig.velocity.magnitude > 2.6f)
                {
                    RandAttack = 0;
                    anim.Play(WeaponTag + "StartAttack" + RandAttack, 1);
                }
                else
                {
                    RandAttack = Random.Range(1, 3);
                    anim.Play(WeaponTag + "StartAttack" + RandAttack, 1);
                }
            }

            RaycastHit2D hit = Physics2D.Raycast(Hand.position, Direction, 5f, 1 << 14);
            if (hit.collider != null)
            {
                hit.collider.transform.root.SendMessage("Defend", 30);
            }
        }
    }
    float Impulse(Transform enemy)
    {
        if (enemy.GetComponent<Rigidbody2D>() != null)
        {
            float x = enemy.GetComponent<Rigidbody2D>().velocity.x;
            float y = enemy.GetComponent<Rigidbody2D>().velocity.y;
            float x2 = rig.velocity.x;
            float y2 = rig.velocity.y;
            return Mathf.Sqrt((x2 - x) * (x2 - x) + (y2 - y) * (y2 - y)) / 20 + 1;
        }
        else
            return 1;

    }
    void StartAttack(float Rate)
    {
        if(Weapon != null)
            Weapon.GetComponent<Item>().PlaySound("Swing");
        isAttack = true;
        HitRate = Rate;
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
    void Step()
    {
        if (Enemy != null)
        {
            rig.velocity = Vector2.zero;
            rig.AddForce(100f * Direct(Enemy) / Mathf.Sqrt(WeaponWeight));
        }
    }
    void BackStep()
    {
        if (Enemy != null)
        {
            rig.velocity = Vector2.zero;
            rig.AddForce(100f * -Direct(Enemy) / Mathf.Sqrt(WeaponWeight));
        }
    }
    public void BrokeWeapon(ItemWeapon item)
    {
        if (Weapon != null)
        {
            Weapon.transform.parent = null;
            GameObject BrokenWeapon = Instantiate(item.BrokenWeapon, Arm.position, transform.rotation, Hand.GetChild(0));
            if (item.Effect != null)
            {
                GameObject Effect = Instantiate(item.Effect, Arm.position, Arm.rotation, null);
            }
            Weapon.GetComponent<Item>().Destroy();
            Weapon = BrokenWeapon;

            Slot = new GameObject[Arm.childCount];
            for (int i = 0; i < Slot.Length; i++)
            {
                Slot[i] = Arm.GetChild(i).gameObject;
                Slot[i].SetActive(false);
            }
            if (Slot.Length > 0)
            {
                Slot[NowSlot].SetActive(true);
                Weapon = Slot[NowSlot];
            }
        }
    }
    #endregion
    #region EffectAndDie

    public void GetHit(int Damage, Transform obj)
    {
        if (!FriendCalled)
        {
            FriendCalled = true;
            StartCoroutine("CallHelp", 0.5f);
        }

        if (isBlock && Enemy != null)
        {
            Enemy.SendMessage("Sparks");
            GetKick(-Direct(Enemy) * Damage);
            Weapon.GetComponent<Item>().PlaySoundRandom("Block");
        }
        else if (Shocked)
        {
            Hit(Damage, obj, 300);
            Shocked = false;
            AbleToAttack = true;
        }
        else if (HandState == AttackState.StartAttack)
        {
            anim.Play("Idle");
            Hit(Damage, obj, 150);
        }
        else
        {
            Hit(Damage, obj, 150);
        }

        if (UI != null)
        {
            UI.GetComponent<UI_Ai>().GetHP(HP);
        }
    }
    public void GetDistanceHit(int Damage, Transform obj)
    {
        if (!FriendCalled)
        {
            FriendCalled = true;
            StartCoroutine("CallHelp", 0.5f);
        }
        if (Damage > 0)
        {
            StartCoroutine("SpriteHit", obj);
            HP -= Mathf.RoundToInt(Damage * (300f - ArmorPoints) / 300f);
        }
        if (UI != null)
        {
            UI.GetComponent<UI_Ai>().GetHP(HP);
        }
    }
    public void GetKick(Vector2 Direction)
    {
        if (HP > 0)
        {
            rig.velocity = Vector2.zero;
            rig.AddForce(Direction);
        }

    }
    public IEnumerator GetShock(float Time)
    {
        if (Shocked)
            yield break;
        Shocked = true;
        anim.Play("GetShock", 1);
        AbleToAttack = false;
        rig.velocity = Vector2.zero;
        yield return new WaitForSeconds(Time);
        anim.Play("GetShockEnd", 1);
        Shocked = false;
        AbleToAttack = true;
        yield break;
    }
    public void EffectHit(int Damage)
    {
        HP -= Damage;
        StartCoroutine("SpriteHit", Body);
    }
    private void Hit(float Damage, Transform obj, int Armor)
    {
        HP -= Mathf.RoundToInt(Damage * (Armor - ArmorPoints) * (100 - LVL) / 15000);
        PlaySoundRandom("Hit");
        StartCoroutine("SpriteHit", obj);
        GetKick(-Direct(Enemy) * Damage / 2);
    }
    IEnumerator SpriteHit(Transform obj)
    {
        Vector2 Position = (obj.position + Body.position) / 2f;
        levelSystem.ParticleSystem.CreateParticle(Particle.ParticlesType.Blood, Position, obj.transform.up);

        AbleToAttack = false;
        Head.GetComponent<SpriteRenderer>().color = new Vector4(1f, 0.5f, 0.5f, 1);
        Body.GetComponent<SpriteRenderer>().color = new Vector4(1f, 0.5f, 0.5f, 1);
        yield return new WaitForSeconds(0.2f);
        AbleToAttack = true;
        Head.GetComponent<SpriteRenderer>().color = Color.white;
        Body.GetComponent<SpriteRenderer>().color = Color.white;
    }
    void ThrowWeapon()
    {
        if (Weapon == null)
            return;
        Vector2 direction = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        Weapon.SendMessage("PhysicsOn");
        Weapon.GetComponent<Rigidbody2D>().simulated = true;
        Weapon.GetComponent<Rigidbody2D>().AddForce(direction * 500f);
        Weapon.GetComponent<Rigidbody2D>().AddTorque(Random.Range(100f, 300f));
        Weapon.transform.parent = null;
        Weapon = null;
    }
    void KnockOutWeapon()
    {
        ThrowWeapon();
        FindWeapon();
    }
    void ThrowHelmet()
    {
        if (Helmet == null)
            return;
        Vector2 direction = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        Helmet.SendMessage("PhysicsOn");
        Helmet.GetComponent<Rigidbody2D>().simulated = true;
        Helmet.GetComponent<Rigidbody2D>().AddForce(direction * 500f);
        Helmet.GetComponent<Rigidbody2D>().AddTorque(Random.Range(100f, 300f));
        Helmet.transform.parent = null;
        Helmet = null;
    }
    void ThrowArmor()
    {
        if (Armor == null)
            return;
        Vector2 direction = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
        Armor.SendMessage("PhysicsOn");
        Armor.GetComponent<Rigidbody2D>().simulated = true;
        Armor.GetComponent<Rigidbody2D>().AddForce(direction * 500f);
        Armor.GetComponent<Rigidbody2D>().AddTorque(Random.Range(100f, 300f));
        Armor.transform.parent = null;
        Armor = null;
    }
    public void Sparks()
    {
        if (Weapon != null)
        {
            levelSystem.ParticleSystem.CreateParticle(Particle.ParticlesType.Sparks, Weapon.transform.GetChild(0).position, Weapon.transform.up);
        }
    }



    public void Robbery(GameObject obj)
    {
        UI.PrintForTime("Отдай живо", 2);
        lastRoutine = StartCoroutine(WaitAndAttack(obj, 5f));
    }
    public void GaveBack()
    {
        StopCoroutine(lastRoutine);
    }
    IEnumerator WaitAndAttack(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        GetEnemy(obj.transform);
        yield break;
    }


    public void Destroy()
    {
        Destroy(gameObject);
    }
    public IEnumerator Die()
    {
        if (Killer != null && Killer.tag == "Enemy")
        {
            UI.GetComponent<UI_Ai>().Requested(false);
            Killer.GetComponent<Quest>().OnEnemyDie(gameObject);
        }
        if (Enemy != null)
        {
            if (Enemy.tag == "Enemy")
            {
                //Enemy.GetComponent<Ai>().ManKilled(gameObject);
            }
            if(Enemy.tag == "Player")
            {
                Enemy.SendMessage("DeleteSelected", transform);
                Enemy.GetComponent<Character>().ManKilled(gameObject);
            }
        }
        isAttack = false;
        Dialog.QuestScript.MainQuestKilled();
        Dialog.CanDialog = false;

        UI.gameObject.SetActive(false);
        Col.enabled = false;
        Eye.gameObject.SetActive(false);
        anim.Play("Idle", 1);
        yield return new WaitForSeconds(0.5f);
        if (rig.velocity.x >= 0f)
        {
            anim.Play("DieBackward");
        }
        else
        {
            anim.Play("DieForward");
        }
        anim.SetBool("Dead", true);
        transform.GetChild(6).gameObject.SetActive(false);
        transform.localPosition = new Vector3(transform.position.x, transform.position.y, 0.4f);
        rig.bodyType = RigidbodyType2D.Static;
        //StopAllCoroutines();

        if(Important)
        {
            StartCoroutine(Revive(10f));
        }
        else
        {
            int RandW = Random.Range(0, 10);
            if (RandW == 0)
            {
                ThrowWeapon();
            }

            int RandH = Random.Range(0, 10);
            if (RandH == 0)
            {
                ThrowHelmet();
            }

            int RandA = Random.Range(0, 10);
            if (RandA == 0)
            {
                ThrowArmor();
            }
        }

        yield break;
    }
    public IEnumerator Revive(float time)
    {
        yield return new WaitForSeconds(time);
        Dialog.CanDialog = true;
        UI.gameObject.SetActive(true);
        Col.enabled = true;
        Eye.gameObject.SetActive(true);
        transform.GetChild(6).gameObject.SetActive(true);

        if(Armor != null)
        {
            Armor.gameObject.SetActive(true);
        }
        if(Helmet != null)
        {
            Helmet.gameObject.SetActive(true);
        }

        Enemy = null;
        Fight = false;

        isDead = false;
        anim.Play("GOTO", 0);
        anim.SetBool("Dead", false);
        HP = MaxHp;
        AiState = AiPrevState;
        rig.bodyType = RigidbodyType2D.Dynamic;
        UI.GetComponent<UI_Ai>().GetHP(MaxHp);
        UI.GetComponent<UI_Ai>().HpTurn(false);
        yield break;
    }
    #endregion
    #region SaveLoad
    
    public void LoadPrev(int num, int Slot)
    {
        LevelSystem.AiLoadEnd = false;
        StartCoroutine(LoadPrevCoroutine(num, Slot));
    }
    IEnumerator LoadPrevCoroutine(int num, int Slot)
    {
        AiData data = SaveSystem.LoadLevel(Slot).AiOnSceneData[num];

        AiLoaded = false;
 
        name = data.ReferenceName;
        AiEnabled = data.enabled;

        transform.position = new Vector3(
        data.position[0],
        data.position[1],
        data.position[2]);

        LevelSystem.AiLoadEnd = true;
        yield break;
    }
    public void Load(int num, int Slot)
    {
        AiData data = SaveSystem.LoadLevel(Slot).AiOnSceneData[num];
        StartCoroutine(LoadCoroutine(data));
    }
    public void Load(AiData data)
    {
        StartCoroutine(LoadCoroutine(data));
    }
    IEnumerator LoadCoroutine(AiData data)
    {
        if (data != null)
        {
            name = data.ReferenceName;
            Name = (Names)data.Name;
            LastName = (LastNames)data.LastName;
            AiEnabled = data.enabled;

            transform.position = new Vector3(
            data.position[0],
            data.position[1],
            data.position[2]);

            StartPoint = new Vector2(
            data.startposition[0],
            data.startposition[1]);
            GotStartPoint = data.GotStartPoint;

            name = data.ReferenceName;
            HP = data.hp;
            MaxHp = data.maxhp;
            LVL = data.LVL;
            AiState = (States)data.State;

            Fight = data.Fight;

            isDead = data.Dead;
            Important = data.Important;

            Work = (WorkType)data.Work;

            Idle = data.Idle;
            HairNum = data.hairnum;
            FaceNum = data.facenum;
            BodyNum = data.bodynum;

            SkinTone = data.skinTone;
            HairColor = new Vector4(data.hairColor[0], data.hairColor[1], data.hairColor[2], data.hairColor[3]);

            Hand.rotation = new Quaternion(
            Hand.rotation.x, Hand.rotation.y,
            data.rotation, Hand.rotation.w);

            yield return new WaitForFixedUpdate();
            Work = (WorkType)data.Work;
            AiState = (States)data.State;
            AiPrevState = (States)data.PrevState;
            Agressive = data.Agressive;
            SideOwn.ManSide = (SideOwn.Side)data.AiSide;
            if (data.Home != "")
            {
                Home = GameObject.Find(data.Home);
            }
            else
            {
                Home = null;
            }
            if(data.Enemy != "")
            {
                Enemy = GameObject.Find(data.Enemy).transform;
                UI.GetComponent<UI_Ai>().HpTurn(true);
            }
            else
            {
                Enemy = null;
            }
            Slot = new GameObject[data.itemslot.Length];
            for(int i = 0; i < Slot.Length; i++)
            {
                if (data.itemslot[i] != null)
                {
                    int index = data.itemslot[i].index;
                    Slot[i] = Instantiate(IndexSystem.Item(index), Hand.GetChild(0).position,
                    transform.rotation, Hand.GetChild(0));
                    Slot[i].GetComponent<Item>().Load(data.itemslot[i]);
                    Slot[i].transform.rotation = new Quaternion(0, 0, 0, 0);
                    Slot[i].SetActive(false);
                }
            }
            NowSlot = data.NowSlot;

            if (data.helmet > -1)
            {
                int index = data.helmet;
                Helmet = Instantiate(IndexSystem.Item(index), Head.GetChild(0).position,
                    Head.GetChild(0).rotation, Head.GetChild(0)).transform;
            }
            if (data.armor > -1)
            {
                int index = data.armor;
                Armor = Instantiate(IndexSystem.Item(index), Body.GetChild(0).position,
                    Body.GetChild(0).rotation, Body.GetChild(0)).transform;
            }
            EnteredHouse = data.EnteredHouse;
            if (EnteredHouse && !NowData.inHouse)
            {
                EnterToHouse();
            }
            else if (UI != null)
            {
                UI.GetComponent<UI_Ai>().GetMaxHP(MaxHp);
                UI.GetComponent<UI_Ai>().GetHP(HP);
            }
            if (!NowData.inHouse)
            {
                Friend = new GameObject[data.Frined.Length];
                for (int i = 0; i < Friend.Length; i++)
                {
                    if (data.Frined[i] != -1)
                    {
                        Friend[i] = levelSystem.GetAi(data.Frined[i]);
                    }
                    else
                    {
                        Friend[i] = null;
                    }
                }
                Metier = new Transform[data.Metier.Length];

                for (int i = 0; i < Metier.Length; i++)
                {
                    if (data.Metier[i] != "")
                    {
                        Metier[i] = GameObject.Find(data.Metier[i]).transform;
                    }
                    else
                    {
                        Metier[i] = null;
                    }
                }

                WalkTargets = new Transform[data.WalkTarget.Length];
                for (int i = 0; i < WalkTargets.Length; i++)
                {
                    if (data.WalkTarget[i] != "")
                    {
                        WalkTargets[i] = GameObject.Find(data.WalkTarget[i]).transform;
                    }
                    else
                    {
                        WalkTargets[i] = null;
                    }
                }
            }
            Body.localScale = new Vector3(data.Scale[0], data.Scale[1], data.Scale[2]);
            Head.localScale = Body.localScale;

            StartCoroutine(LateLoad(0f));
            AiLoaded = true;
        }
        yield break;
    }
    #endregion

    void StatesLogic()
    {
        if (HP <= 0)
        {
            AiPrevState = AiState;
            AiState = States.death;
        }
        else if (Enemy != null && Fight)
        {
            if (FindWeapon())
            {
                AiState = States.attack;
            }
            else
            {
                AiState = States.runaway;
            }
        }
        else if (AiState == States.athome)
        {

        }
        else if (Dialog != null && Dialog.inDialog)
        {
            AiState = States.indialog;
        }
        else if (AiState == States.GoTo)
        {

        }
        else if (AiState == States.Follow)
        {

        }
        else if (AiState == States.athome)
        {

        }
        else if (Home != null && DayTime.ClockTime > 8)
        {
            AiState = States.gohome;
        }
        else if (Idle)
        {
            AiState = States.idle;
        }
        else
        {
            AiState = States.work;
            if (!isWork)
            {
                if (Work == WorkType.NULL)
                {
                    WorkCour = StartCoroutine(RandomWalk());
                }
                else if (Work == WorkType.Fermer)
                {
                    int Rand = Random.Range(0, 2);
                    if (Rand == 0)
                    {
                        WorkCour = StartCoroutine(Fermer());
                    }
                    if (Rand == 1)
                    {
                        WorkCour = StartCoroutine(RandomWalk());
                    }
                }
            }
        }
    }
    void Main()
    {
        switch (AiState)
        {
            case States.attack:
                {
                    if (Enemy == null || Weapon == null || !Weapon.GetComponent<Item>().isWeapon)
                        return;
                    if(Enemy.tag == "Enemy" && Enemy.GetComponent<Ai>().HP <= 0)
                    {
                        Enemy = null;
                        Loose = StartCoroutine(LoseEnemy(5));
                        return;
                    }
                    if (LongRangeAttack)
                    {
                        target = Enemy.position;
                        speed = warspeed;
                        bool NoBarrier = false;
                        bool AimOn = false;
                        isLoaded = Weapon.GetComponent<LongRangeWeapon>().Loaded;
                        isLoading = Weapon.GetComponent<LongRangeWeapon>().isLoading;
                        FlipTo(Direct(Enemy).x);
                        if (isLoaded)
                        {
                            Hand.transform.up = Vector2.Lerp(Hand.transform.up, Direct(target), 0.2f * (1 + LVL / 10f) / WeaponWeight);
                        }
                        Eye.transform.up = Direct(target);
                        RaycastHit2D hit = Physics2D.Raycast(Hand.position, Direct(Enemy), Distance, 1 << 8 | 1 << 14 | 1 << 16);
                        if (hit.collider != null)
                        {
                            NoBarrier = true;
                        }
                        RaycastHit2D aim = Physics2D.Raycast(Hand.transform.position, Hand.transform.up, Distance, 1 << 13 | 1 << 8 | 1 << 14);
                        if (aim.collider != null && aim.collider.transform.root == Enemy)
                        {
                            AimOn = true;
                        }
                        if(!isLoaded)
                        {
                            if (!isLoading)
                            {
                                Weapon.GetComponent<LongRangeWeapon>().isLoading = true;
                                StartCoroutine(LoadWeapon());
                                Hand.transform.up = Direct(target);
                            }
                            if(Vector2.Distance(Enemy.position, transform.position) < 3f)
                            {
                                Go(-Direct(Enemy) * warspeed);
                            }
                        }
                        else if(NoBarrier && AimOn)
                        {
                            isFiring = true;
                            StartCoroutine(Fire());
                            rig.velocity = rig.velocity / 5f;
                        }
                        else
                        {
                            Goto(0);
                        }
                        
                    }
                    else
                    {
                        Eye.transform.up = Direct(target);
                        target = Enemy.position;
                        speed = warspeed;
                        FlipTo(Enemy.position.x - transform.position.x);
                        if (HandState == AttackState.Idle)
                        {
                            Hand.transform.up = Vector2.Lerp(Hand.transform.up, Direct(target), 1f * (1 + LVL / 10f) / WeaponWeight);
                        }
                        else if (HandState == AttackState.StartAttack)
                        {
                            Hand.transform.up = Vector2.Lerp(Hand.transform.up, Direct(target), 0.5f * (1 + LVL / 10f) / WeaponWeight);
                        }
                        if (!AbleToAttack)
                            break;
                        if (Vector2.Distance(Enemy.position, transform.position) < WeaponReach * 0.7f)
                        {
                            Go(-Direct(Enemy));
                        }
                        else if (Vector2.Distance(Enemy.position, transform.position) > WeaponReach * 0.7f &&
                                Vector2.Distance(Enemy.position, transform.position) <= WeaponReach * 1.2f)
                        {
                            RandomAttackMove();
                        }
                        else if (Vector2.Distance(Enemy.position, transform.position) > WeaponReach * 1.2f &&
                                Vector2.Distance(Enemy.position, transform.position) < WeaponReach * 2f)
                        {
                            Goto(WeaponReach);
                            RandomAttackMove();
                        }
                        else if (Vector2.Distance(Enemy.position, transform.position) >= WeaponReach * 2f)
                        {
                            Goto(WeaponReach);
                        }
                    }
                }
                break;
            case States.idle:
                {
                    Eye.transform.up = Hand.transform.up;
                    Stay(StartPoint);
                }
                break;
            case States.indialog:
                {
                    FlipTo(Character.position.x - transform.position.x);
                    Hand.transform.up = Vector2.Lerp(Hand.transform.up, Direct(target), 0.05f);
                    rig.velocity = Vector2.zero;
                }
                break;
            case States.gohome:
                {
                    if (Home == null)
                    {
                        AiState = States.idle;
                        break;
                    }
                    if (Vector2.Distance(Home.transform.position, transform.position) > 3f)
                    {
                        if(WorkCour != null)
                        {
                            StopCoroutine(WorkCour);
                            isWork = false;
                            WorkCour = null;
                        }
                        FlipTo(rig.velocity.x);
                        speed = walkspeed;
                        target = Home.transform.position;
                        Hand.transform.up = Vector2.Lerp(Hand.transform.up, Direct(target), 0.5f * (1 + LVL / 10f) / WeaponWeight);
                        Goto(0);
                    }
                    else
                    {
                        AiState = States.athome;
                    }
                }
                break;
            case States.athome:
                {
                    if (Home == null)
                    {
                        AiState = States.idle;
                        break;
                    }
                    if (!EnteredHouse)
                    {
                        Debug.Log(Home);
                        Home.GetComponent<House>().AiGetInto(gameObject);
                        EnterToHouse();
                        EnteredHouse = true;
                    }
                    else if (DayTime.ClockTime > 2f && DayTime.ClockTime < 5f)
                    {
                        Home.GetComponent<House>().AiGetOut(gameObject);
                        ExitHouse();
                    }
                }
                break;
            case States.GoTo:
                {
                    Hand.transform.up = Vector2.Lerp(Hand.transform.up, Direct(target), 0.5f * (1 + LVL / 10f) / WeaponWeight);
                    Eye.transform.up = Direct(target);
                    FlipTo(rig.velocity.x);
                }
                break;
            case States.Follow:
                {
                    target = QuestTarget.position;
                    Hand.transform.up = Vector2.Lerp(Hand.transform.up, Direct(target), 0.5f * (1 + LVL / 10f) / WeaponWeight);
                    Eye.transform.up = Hand.transform.up;
                    FlipTo(rig.velocity.x);
                    Goto(0);
                }
                break;
            case States.walk:
                {
                    Hand.transform.up = Vector2.Lerp(Hand.transform.up, Direct(target), 0.5f * (1 + LVL / 10f) / WeaponWeight);
                    Eye.transform.up = Hand.transform.up;
                    FlipTo(rig.velocity.x);
                    Walk();
                }
                break;
            case States.freewalk:
                {
                    Hand.transform.up = Vector2.Lerp(Hand.transform.up, Direct(target), 0.5f * (1 + LVL / 10f) / WeaponWeight);
                    Eye.transform.up = Direct(target);
                    FlipTo(rig.velocity.x);
                }
                break;
            case States.work:
                {
                    FlipTo(rig.velocity.x);
                    Eye.transform.up = Hand.transform.up;
                    switch (Work)
                    {
                        case WorkType.Fermer:
                            if (!isWork)
                            {
                                StartCoroutine(Fermer());
                            }
                            break;
                        case WorkType.Drink:
                            if (!isWork)
                            {
                                StartCoroutine(Drink());
                            }
                            if(!isSaying)
                            {
                                //StartCoroutine(RandomPhrase());
                            }
                            Stay(StartPoint);
                            break;
                        case WorkType.Eat:
                            if (!isWork)
                            {
                                StartCoroutine(Eat());
                            }
                            Stay(StartPoint);
                            break;
                        case WorkType.Walk:
                            if (!isWork)
                            {
                                StartCoroutine(RandomWalk());
                            }
                            break;
                    }
                }
                break;
            case States.runaway:
                {
                    if (AiState == States.work && WorkCour != null)
                    {
                        StopCoroutine(WorkCour);
                        isWork = false;
                        WorkCour = null;
                    }
                    if (Enemy == null)
                        return;
                    speed = warspeed;
                    Eye.transform.up = Direct(target);
                    FlipTo(rig.velocity.x);
                    if (Vector2.Distance(transform.position, Enemy.position) < 20f)
                    {
                        Go(-Direct(Enemy));
                    }
                    else
                    {
                        StartCoroutine(LoseEnemy(10f));
                    }
                }
                break;
            case States.death:
                {
                    if (!isDead)
                    {
                        StartCoroutine(Die());
                        isDead = true;
                    }
                }
                break;
        }
        if (AiState != States.athome)
        {
            if (Dialog != null && Fight && Enemy != null)
                Dialog.CanDialog = false;
            Animation();
        }
    }

    void FixedUpdate()
    {

        if (AiEnabled)
        {
            Animation();
            StatesLogic();
            Main();
        }
    }
    private void Update()
    {

    }
}
