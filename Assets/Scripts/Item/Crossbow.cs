using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow : MonoBehaviour
{
    public float Speed;
    public Transform BoltPlace;
    public GameObject Bolt;
    private Transform LoadedBolt;
    private Animator anim;
    private ItemWeapon ItemWeapon;
    public bool Loaded;
    public bool isLoading;

    void Start()
    {
        anim = GetComponent<Animator>();
        ItemWeapon = GetComponent<ItemWeapon>();
        anim.SetFloat("Speed", Speed / 2);
    }

    public void Strike()
    {
        if (isLoading)
            return;
        if(!Loaded)
        {
            anim.Play("Load");
            isLoading = true;
        }
        else
        {
            anim.Play("Fire");
        }
    }
    public void BreakLoading()
    {
        anim.Play("Idle");
        isLoading = false;
    }

    private void SetLoaded()
    {
        isLoading = false;
        Loaded = true;

        LoadedBolt = Instantiate(Bolt, BoltPlace.position, BoltPlace.rotation, BoltPlace).transform;
    }
    private void FireBullet()
    {
        Loaded = false;
        isLoading = false;

        LoadedBolt.parent = null;
        float Impulse = ItemWeapon.Damage * 300f;
        LoadedBolt.SendMessage("PhysicsOn", GetComponent<Collider2D>());
        LoadedBolt.GetComponent<Item>().ThrowItem(transform.root, 2);
        LoadedBolt.GetComponent<Rigidbody2D>().AddForce(Impulse * transform.up);

        transform.root.GetComponent<Rigidbody2D>().AddForce(ItemWeapon.Damage * 50 * -transform.up);
    }
}
