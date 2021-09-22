using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class Item : MonoBehaviour
{
    public string Name;
    public enum Tag {Item, Food, Spear, Sword, Axe, Bow, CrossBow, Gun, Bullet, Seeds, Armor, Helmet};
    public Tag ItemTag;
    public enum ItemType {Weapon, LongRangeWeapon, Armor, Bullet, Food, Plant, Key, SimpleItem};
    public ItemType Type;
    public bool isWeapon;
    public int Index;
    public int Cost;
    public Sprite Icon;
    public float Weight;
    public float ThrowDamage;
    public enum cost { common, rare, epic, legendary };
    public cost Value;
    public GameObject Owner;
    public static float Heavy = 3f;

    public float HitRate = 1;
    public bool Requested;
    public bool inObj;
    public bool Fly;

    private Collider2D Col;
    private Rigidbody2D Rig;
    private LayerMask Layer;
    private Transform UI;
    private TextMeshProUGUI TextUI;
    private Collider2D PlayerCol;
    private AudioData AudioData;
    private AudioSource[] audioSource = new AudioSource[5];
    private string[] audioName = new string[5];
    public LevelSystem LevelSystem;

    void Awake()
    {

        Layer = gameObject.layer;
        Rig = GetComponent<Rigidbody2D>();
        Col = GetComponent<Collider2D>();
        UI = transform.GetChild(0);
        AudioData = Resources.Load<AudioData>("AudioData");

        TextUI = UI.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        Rig.mass = Mathf.Sqrt(Weight);


        if (transform.parent != null)
        {
            Col.enabled = false;
            Rig.bodyType = RigidbodyType2D.Static;
        }
        if (Icon == null)
        {
            Icon = GetComponent<SpriteRenderer>().sprite;
        }
    }
    void Start()
    {
        Name = LanguagesSystem.Language.ItemName[Index];
    }
    public void Load(int num, int Slot)
    {
        ItemsData Data = SaveSystem.LoadLevel(Slot).ItemsOnSceneData[num];
        Load(Data);
    }
    public void Load(ItemsData Data)
    {
        name = Data.ReferenceName;
        //Name = Data.Name;
        transform.position = new Vector3(
        Data.position[0],
        Data.position[1],
        Data.position[2]);
        transform.rotation = new Quaternion(
        transform.rotation.x, transform.rotation.y,
        Data.rotation, transform.rotation.w);
        Requested = Data.Requseted;

        if(Data.Owner != -1)
        {
            Owner = LevelSystem.GetAi(Data.Owner);
        }

        switch (Type)
        {
            case ItemType.Armor:
                {
                    ItemArmor Armor = GetComponent<ItemArmor>();
                    if (Armor != null)
                    {
                        Armor.Armor = Data.armor;
                    }
                    ItemWeapon itemWeapon = GetComponent<ItemWeapon>();
                    if (itemWeapon != null)
                    {
                        itemWeapon.Level = Data.Level;
                    }
                }
                break;
            case ItemType.Bullet:
                {

                }
                break;
            case ItemType.Food:
                {
                    ItemFood food = GetComponent<ItemFood>();
                    if(food != null)
                    {
                        food.nowStage = Data.FoodStage;
                        food.FullyEaten = Data.FullyEaten;
                        //food.GetStage(food.nowStage);
                    }
                }
                break;
            case ItemType.LongRangeWeapon:
                {
                    LongRangeWeapon weapon = GetComponent<LongRangeWeapon>();
                    if (weapon != null)
                    {
                        weapon.Loaded = Data.Loaded;
                    }
                    ItemWeapon itemWeapon = GetComponent<ItemWeapon>();
                    if (itemWeapon != null)
                    {
                        itemWeapon.Level = Data.Level;
                        itemWeapon.Uses = Data.Uses;
                    }
                }
                break;
            case ItemType.Weapon:
                {
                    ItemWeapon itemWeapon = GetComponent<ItemWeapon>();
                    if (itemWeapon != null)
                    {
                        itemWeapon.Level = Data.Level;
                        itemWeapon.Uses = Data.Uses;
                    }
                }
                break;
        }
    }

    public Vector2 Direct(Transform obj)
    {
        return new Vector2(obj.transform.position.x - transform.position.x, obj.transform.position.y - transform.position.y);
    }
    public Vector2 Direct(Vector2 obj)
    {
        return new Vector2(obj.x - transform.position.x, obj.y - transform.position.y);
    }
    IEnumerator PhysicsOn()
    {
        if(Rig != null)
            Rig.bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(0.1f);
        while (inObj)
        {
            yield return new WaitForSeconds(0.1f);
        }
        Col.enabled = true;
        yield break;
    }
    IEnumerator PhysicsOff(Collider2D Collider)
    {
        Col.enabled = false;
        PlayerCol = Collider;
        Rig.rotation = 0;
        Rig.bodyType = RigidbodyType2D.Static;
        yield break;
    }
    IEnumerator PhysicsEnter(Collider2D Collider)
    {
        yield return new WaitForSeconds(0.1f);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0.7f);
        if(Collider != null && Col != null)
            Physics2D.IgnoreCollision(Col, Collider);
        PlayerCol = Collider;
        yield break;
    }
    IEnumerator Throw()
    {
        while(Fly)
        {
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.layer = Layer;

        yield break;
    }

    public void JumpTo(Transform target, float Power)
    {
        StartCoroutine(PhysicsOn());
        transform.GetComponent<Rigidbody2D>().AddForce(Direct(target) * Power * 100f);
        transform.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-1f, 1f) * 10f);
    }
    public void RandomMove()
    {
        Vector2 force = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        transform.GetComponent<Rigidbody2D>().AddForce(force * 30f);
        transform.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-1f, 1f) * 30f);
    }
    void GetIndex(Transform obj)
    {
        obj.SendMessage("GetIndex", Index);
    }
    void GetName(Transform obj)
    {
        obj.SendMessage("SetName", Name);
    }

    public void UIOn()
    {
        if (transform.parent != null || TextUI == null)
            return;
        TextUI.gameObject.SetActive(true);
        TextUI.text = Name;
        if (Requested)
        {
            TextUI.color = Color.blue;
        }
        else
        {
            TextUI.color = Color.white;
        }
        StopCoroutine("UIOffFixed");
    }
    public void UIOff()
    {
        TextUI.gameObject.SetActive(false);
        TextUI.text = "";
    }
    IEnumerator UIOffFixed()
    {
        yield return new WaitForSeconds(0.2f);
        TextUI.gameObject.SetActive(false);
        TextUI.text = "";
        yield break;
    }

    public void ThrowItem(Transform Player, float Hit)
    {
        transform.parent = null;
        Fly = true;
        Owner = Player.gameObject;
        HitRate = Hit;
    }
    IEnumerator TurnFly()
    {
        yield return new WaitForSeconds(0.2f);
        if (Rig.velocity.magnitude < 0.2f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0.7f);
            Rig.velocity = Vector2.zero;
            Fly = false;
        }
        yield break;
    }

    public void PlaySound(string name)
    {
        int num = 0;
        for(int i = 0; i < audioSource.Length; i++)
        {
            if (audioSource[i] == null)
                num = i;
        }
        Sound sound = AudioData.GetSound(name);
        audioName[num] = name;
        audioSource[num] = gameObject.AddComponent<AudioSource>();
        audioSource[num].spatialBlend = 1;
        audioSource[num].clip = sound.clip;
        audioSource[num].volume = sound.volume;
        audioSource[num].pitch = sound.pitch;
        audioSource[num].Play();
        StartCoroutine(OnSoundStop(num));
    }
    public void PlaySoundRandom(string name)
    {
        int num = 0;
        for (int i = 0; i < audioSource.Length; i++)
        {
            if (audioSource[i] == null)
                num = i;
        }
        Sound sound = AudioData.GetSound(name);
        audioName[num] = name;
        audioSource[num] = gameObject.AddComponent<AudioSource>();
        audioSource[num].spatialBlend = 1;
        audioSource[num].clip = sound.clip;
        audioSource[num].volume = sound.volume * Random.Range(0.7f, 1f); ;
        audioSource[num].pitch = sound.pitch * Random.Range(0.7f, 1.2f); ;
        audioSource[num].Play();
        StartCoroutine(OnSoundStop(num));
    }
    public void StopSound(string name)
    {
        for(int i = 0; i < audioName.Length; i++)
        {
            if(audioName[i] == name && audioSource[i] != null)
            {
                Destroy(audioSource[i]);
                audioSource[i] = null;
                audioName[i] = "";
            }
        }
    }
    private IEnumerator OnSoundStop(int num)
    {
        yield return new WaitForSeconds(audioSource[num].clip.length);
        Destroy(audioSource[num]);
        audioSource[num] = null;
        audioName[num] = "";
        yield break;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Transform obj = collision.transform.root;
        if (collision.gameObject.layer == 16 && collision.isTrigger)
        {
            if (obj.tag == "Obstacle")
            {
                Rig.velocity = Vector2.zero;
            }
            return;
        }
        
        if (Fly && Rig.velocity.magnitude > 5f && Owner != null && Owner.transform != obj
        && obj.gameObject.layer != gameObject.layer && transform.parent == null)
        {
            int LVL = 0;
            if (Owner.tag == "Player")
            {
                LVL = Owner.GetComponent<Character>().LVL;
            }
            else if (Owner.tag == "Enemy")
            {
                LVL = Owner.GetComponent<Ai>().LVL;
            }

            int Power = Mathf.RoundToInt(HitRate * (1 + LVL / 10f));
            Vector2 Impulse = Rig.velocity.normalized * Rig.velocity.magnitude * Weight;
            Vector2 Velocity = Rig.velocity;

            switch (Type)
            {
                case ItemType.Bullet:
                    transform.GetComponent<ItemBullet>().Hit(obj, Impulse, Power);
                    Fly = false;
                    break;
                case ItemType.Food:
                    Hit(obj.gameObject, Mathf.RoundToInt(Power * Weight + ThrowDamage), Impulse);
                    Rig.velocity = Vector2.zero;
                    Rig.angularVelocity = Rig.angularVelocity / 5f;
                    Rig.AddForce(-Direct(obj.transform) * Velocity);
                    StartCoroutine(EndFly(0.1f));
                    break;
                case ItemType.SimpleItem:
                    Hit(obj.gameObject, Mathf.RoundToInt(Power * Weight + ThrowDamage), Impulse);
                    Rig.velocity = Vector2.zero;
                    Rig.angularVelocity = Rig.angularVelocity / 5f;
                    Rig.AddForce(-Direct(obj.transform) * Velocity);
                    StartCoroutine(EndFly(0.1f));
                    break;
                case ItemType.Weapon:
                    Hit(obj.gameObject, Mathf.RoundToInt(Power * Weight), Impulse);
                    break;
            }

        }
        else if (transform.parent == null)
        {
            StartCoroutine("PhysicsEnter", collision);
        }
    }

    private void Hit(GameObject obj, int Damage, Vector2 Impulse)
    {

        if (obj.tag == "Player")
        {
            obj.GetComponent<Character>().GetDistanceHit(Damage, transform);
            obj.GetComponent<Character>().GetKick(Impulse);
            PlaySound("FlyHit");
        }
        else if (obj.tag == "Enemy")
        {
            if (Owner != null)
            {
                if (!Owner.GetComponent<SideOwn>().Comrade(obj.transform))
                {
                    obj.GetComponent<Ai>().GetDistanceHit(Damage, transform);
                    obj.GetComponent<Ai>().GetKick(Impulse);
                    obj.GetComponent<Ai>().GetEnemy(Owner.transform);
                    Owner = null;
                }
            }
            else
            {
                obj.GetComponent<Ai>().GetDistanceHit(Damage, transform);
                obj.GetComponent<Ai>().GetKick(Impulse);
            }
            PlaySound("FlyHit");
        }
        else if (obj.tag == "Animal")
        {
            obj.GetComponent<Animal>().GetDistanceHit(Damage, transform);
            obj.GetComponent<Animal>().GetKick(Impulse);
            PlaySound("FlyHit");
        }
        else if (obj.tag == "Object")
        {
            obj.GetComponent<Object>().GetHit(Damage, transform);
            PlaySound("ObjectHit");
            EndFly(0);
        }
    }
    private IEnumerator EndFly(float time)
    {
        yield return new WaitForSeconds(time);
        Fly = false;
        HitRate = 1;
        Owner = null;
        yield break;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        string a = collision.transform.root.tag;
        if ((a == "Player" || a == "Enemy" || a == "NPC") && transform.parent == null)
        {
            inObj = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        string a = collision.transform.root.tag;
        if (transform.parent == null)
        {
            if(collision != null)
                Physics2D.IgnoreCollision(Col, collision, false);
            if(gameObject.activeSelf)
                StartCoroutine("PhysicsOn", collision);
            if (a == "Player" || a == "Enemy")
            {
                inObj = false;
            }
        }
    }

    public void Destroy()
    {
        Destroy(gameObject, 0.5f);
    }

    void FixedUpdate()
    {
        UI.rotation = Quaternion.identity;
        if (transform.parent != null)
        {
            Col.enabled = false;
            Rig.bodyType = RigidbodyType2D.Kinematic;
            string tag = transform.parent.root.tag;
            if (transform.parent != transform.parent.root && (tag == "Enemy" || tag == "Player"))
            {
                gameObject.layer = LayerMask.NameToLayer("ItemInHand");
                transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            if (Rig.velocity.magnitude < 0.2f && Fly)
                StartCoroutine("TurnFly");
            transform.localScale = new Vector3(1f, 1f, 1f);
            Rig.bodyType = RigidbodyType2D.Dynamic;
            if(!Fly)
                gameObject.layer = Layer;
        }
    }

}
