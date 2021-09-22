using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{

    private bool isAttack = false;
    private Rigidbody2D rig;
    public GameObject EffectWind;
    private Item Weapon;

    void Start()
    {
        rig = transform.parent.GetComponent<Rigidbody2D>();
        if(transform.root.GetComponent<Item>() != null)
        {
            Weapon = transform.root.GetComponent<Item>();
        }
    }

    Vector2 Direct(Transform obj)
    {
        return new Vector2(obj.transform.position.x - transform.position.x, obj.transform.position.y - transform.position.y);
    }

    void SendReach(Transform obj)
    {
        obj.SendMessage("GetWeaponReach", Mathf.Abs(transform.localPosition.y - transform.parent.localPosition.y));
    }

    IEnumerator WindEffect()
    {
        if (EffectWind == null)
            yield break;
        EffectWind.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        EffectWind.SetActive(false);
        yield break;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Transform obj = collision.transform.root;
        if (transform.parent.parent != null)
        {
            if (isAttack || obj == transform.root)
                return;
            if ((obj.tag == "Player" || obj.tag == "Enemy") && transform.root != transform.parent)
            {
                transform.root.SendMessage("Attack", obj);
                isAttack = true;
            }
            else if (obj.tag == "Object")
            {
                transform.root.SendMessage("AttackObject", obj);
            }
            else if(obj.tag == "Animal")
            {
                transform.root.SendMessage("AttackAnimal", obj);
            }
        }
        else if(Weapon != null && Weapon.Fly)
        {
            if (obj.tag == "Player" || obj.tag == "Enemy" || obj.tag == "Object")
            {
                transform.parent.GetComponent<ItemWeapon>().GetBladeHit(obj);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Transform obj = collision.transform.root;

        if (obj.tag == "Player" || obj.tag == "Enemy")
        {
            isAttack = false;
        }
    }

    private void FixedUpdate()
    {
        if (transform.parent != null)
        {
            gameObject.layer = transform.parent.gameObject.layer;
        }
    }
}