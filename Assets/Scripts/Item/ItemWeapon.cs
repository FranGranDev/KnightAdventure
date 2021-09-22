using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWeapon : MonoBehaviour
{
    public float Damage;
    public float WeaponReach()
    {
        return Mathf.Abs(transform.localPosition.y - transform.GetChild(1).localPosition.y);      
    }
    public float Weight;
    public float StaminaDown;
    [Range(0f, 1f)]
    public float Shade;
    public int Level = 1;
    public bool Breakability;
    public int MaxUses;
    public int Uses;
    public GameObject BrokenWeapon;
    public GameObject Effect;
    private bool BladeHit = false;

    private Rigidbody2D Rig;
    private Item item;
    private Material material;

    private void Awake()
    {
        item = GetComponent<Item>();
        item.Weight = Weight;

    }
    private void Start()
    {
        Rig = GetComponent<Rigidbody2D>();
        material = GetComponent<SpriteRenderer>().material;
        material.SetColor("_Color", new Vector4(Shade, Shade, Shade, 0f));
    }

    IEnumerator GetIn(Transform obj)
    {
        Rig.velocity = Vector2.zero;
        Rig.angularVelocity = 0f;
        item.Fly = false;
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
        yield break;
    }
    IEnumerator GetInEnemy(Transform obj)
    {
        Rig.velocity = Vector2.zero;
        Rig.angularVelocity = 0f;
        transform.parent = obj;
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
        item.Fly = false;
        yield return new WaitForSeconds(1f);
        while (obj.GetComponent<Rigidbody2D>().velocity.magnitude > 0.1f)
        {
            yield return new WaitForSeconds(0.1f);
        }
        transform.parent = null;
        yield break;
    }

    public void GetBladeHit(Transform obj)
    {
        int CurrentDamage = Mathf.RoundToInt(Rig.velocity.magnitude * (1 + Weight / 5f) + Damage * 0.5f);
        Vector2 Impulse = Rig.velocity.normalized * Mathf.Abs(Rig.velocity.magnitude) * Weight;

        if (obj.tag == "Player")
        {
            obj.GetComponent<Character>().GetDistanceHit(CurrentDamage, transform);
            obj.GetComponent<Character>().GetKick(Impulse);
            if (item.Owner != null)
            {
                obj.SendMessage("GetEnemy", item.Owner);
            }
            StartCoroutine("GetInEnemy", obj);
            HpMinus();
        }
        else if (obj.tag == "Enemy")
        {
            if (item.Owner != null)
            {
                if (!item.Owner.GetComponent<SideOwn>().Comrade(obj.transform))
                {
                    obj.GetComponent<Ai>().GetDistanceHit(CurrentDamage, transform);
                    obj.GetComponent<Ai>().GetKick(Impulse);
                    obj.GetComponent<Ai>().GetEnemy(item.Owner.transform);
                    StartCoroutine("GetInEnemy", obj);
                    item.Owner = null;
                }
            }
            else
            {
                obj.GetComponent<Ai>().GetDistanceHit(CurrentDamage, transform);
                obj.GetComponent<Ai>().GetKick(Impulse);
                StartCoroutine("GetInEnemy", obj);
            }
            HpMinus();
        }
        else if (obj.tag == "Object")
        {
            obj.GetComponent<Object>().GetHit(CurrentDamage, transform);
            item.PlaySound("ObjectHit");
            StartCoroutine("GetIn", obj);
            HpMinus();
        }
    }

    public void HpMinus()
    {
        if (Breakability)
        {
            int Rand = Random.Range(0, 4);
            if (Rand > 0)
            {
                Uses--;
            }
            if(Uses <= 0)
            {
                Broke();
            }
        }
    }
    public void Broke()
    {
        if(transform.parent != null)
        {
            if(transform.parent.root.tag == "Player")
            {
                transform.parent.root.GetComponent<Character>().BrokeWeapon(this);
            }
            if (transform.parent.root.tag == "Enemy")
            {
                transform.parent.root.GetComponent<Ai>().BrokeWeapon(this);
            }
        }
    }

}
