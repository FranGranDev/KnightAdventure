using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeWeapon : MonoBehaviour
{
    public float ReloadedTime;
    public float Scatter = 0; // from 0 to 1;
    public Transform BulletPlace;
    public GameObject Bullet;
    public GameObject EffectOnShot;
    public Transform EffectPosition;
    public int BulletIndex;
    private Transform LoadedBullet;
    private Animator anim;
    private AnimationClip LoadClip;
    private ItemWeapon ItemWeapon;
    private Item item;
    private Coroutine Loading;
    private int Stage;
    public bool KeepTension;
    public bool Loaded;
    public bool isLoading;

    void Start()
    {
        anim = GetComponent<Animator>();
        ItemWeapon = GetComponent<ItemWeapon>();
        item = GetComponent<Item>();
        BulletIndex = Bullet.GetComponent<Item>().Index;

        if(Loaded)
        {
            LoadedBullet = Instantiate(Bullet, BulletPlace.position, BulletPlace.rotation, BulletPlace).transform;
        }
    }

    public void Strike()
    {
        switch (item.ItemTag)
        {
            case Item.Tag.Bow:
                StartCoroutine("BowFire");
                break;
            case Item.Tag.CrossBow:
                anim.Play("Fire");
                break;
            case Item.Tag.Gun:
                anim.Play("Fire");
                break;
        }
    }
    IEnumerator LooseArrow()
    {
        float Power = Stage / 3f;
        for (; Stage > 0; Stage--)
            yield return new WaitForSeconds(0.01f);
        FireBullet(Power);
        KeepTension = false;
        yield break;
    }
    IEnumerator BowFire()
    {
        while (KeepTension)
        {
            if (Stage < 3)
            {
                item.PlaySound("BowLoad");
                Stage++;
            }
            yield return new WaitForSeconds(ReloadedTime / 3);
            if (transform.parent == null)
                break;
        }
        StartCoroutine("LooseArrow");
    }
    private IEnumerator StartLoading()
    {
        anim.SetFloat("Speed", 1f);
        anim.Play("Load");
        yield return new WaitForFixedUpdate();
        float speed = anim.GetCurrentAnimatorStateInfo(0).length / anim.GetFloat("Speed");
        anim.SetFloat("Speed", speed / ReloadedTime);
        isLoading = true;
        yield return new WaitForSeconds(ReloadedTime);
        SetLoaded();
        yield break;
    }
    public void Load()
    {
        switch (item.ItemTag)
        {
            case Item.Tag.Bow:
                isLoading = true;
                SetLoaded();
                break;
            case Item.Tag.CrossBow:
                Loading = StartCoroutine(StartLoading());
                break;
            case Item.Tag.Gun:
                Loading = StartCoroutine(StartLoading());
                break;
        }
    }
    public void BreakLoading()
    {
        StopCoroutine(Loading);
        anim.Play("Idle");
        isLoading = false;
    }

    private void SetLoaded()
    {
        isLoading = false;
        Loaded = true;
        if (transform.parent == null)
            return;
        switch (item.ItemTag)
        {
            case Item.Tag.Bow:
                item.StopSound("BowFire");
                break;
            case Item.Tag.CrossBow:
                item.StopSound("CrossbowLoad");
                break;
            case Item.Tag.Gun:
                item.StopSound("GunLoad");
                break;
        }
        LoadSound();
        if (transform.parent.root.tag == "Player")
        {
            transform.parent.root.GetComponent<Character>().DeleteItem(BulletIndex);
            transform.parent.root.GetComponent<Character>().OnWeaponLoaded();
        }
        else if (transform.parent.root.tag == "Enemy")
        {
            transform.parent.root.GetComponent<Ai>().BulletsNum--;
            transform.parent.root.GetComponent<Ai>().OnWeaponLoaded();
        }
        LoadedBullet = Instantiate(Bullet, BulletPlace.position, BulletPlace.rotation, BulletPlace).transform;
        LoadedBullet.GetComponent<Item>().Owner = transform.root.gameObject;
    }
    private void FireBullet(float Power)
    {
        Loaded = false;
        isLoading = false;
        if (LoadedBullet != null)
        {
            LoadedBullet.parent = null;
            LoadedBullet.gameObject.layer = LayerMask.NameToLayer("ItemInHand");
            float Impulse = Power * 2000f;
            Vector2 Direction = (Vector2)transform.up + new Vector2(Random.Range(0, Scatter), Random.Range(0, Scatter));
            LoadedBullet.SendMessage("PhysicsOn", GetComponent<Collider2D>());
            LoadedBullet.GetComponent<Item>().ThrowItem(transform.root, ItemWeapon.Damage * Power);
            LoadedBullet.GetComponent<Rigidbody2D>().AddForce(Impulse * Direction);
            transform.root.GetComponent<Rigidbody2D>().AddForce(Mathf.Sqrt(ItemWeapon.Damage) * 10f * -transform.up);
            if (EffectOnShot != null)
            {
                Instantiate(EffectOnShot, EffectPosition.position, EffectPosition.rotation, EffectPosition);
            }
            if (item.isWeapon)
            {
                ItemWeapon.HpMinus();
            }
            
            LoadedBullet = null;
        }
    }

    public void SetBulletSound()
    {
        switch (item.ItemTag)
        {
            case Item.Tag.Bow:
                item.PlaySound("BowFire");
                break;
            case Item.Tag.CrossBow:
                item.PlaySound("CrossbowSetBolt");
                break;
            case Item.Tag.Gun:
                item.PlaySound("GunSetBullet");
                break;
        }
    }
    public void LoadSound()
    {
        switch (item.ItemTag)
        {
            case Item.Tag.Bow:
                item.PlaySound("BowFire");
                break;
            case Item.Tag.CrossBow:
                item.PlaySound("CrossbowLoad");
                break;
            case Item.Tag.Gun:
                item.PlaySound("GunLoad");
                break;
        }
    }
    public void FireSound()
    {
        switch (item.ItemTag)
        {
            case Item.Tag.Bow:
                //item.PlaySound("BowFire");
                break;
            case Item.Tag.CrossBow:
                item.PlaySound("CrossbowFire");
                break;
            case Item.Tag.Gun:
                item.PlaySound("GunFire");
                break;
        }
    }

    private void FixedUpdate()
    {
        if (item.ItemTag == Item.Tag.Bow)
        {
            anim.Play(Stage.ToString());
        }
        anim.SetBool("Loaded", Loaded);

    }
    private void Update()
    {
    }
}