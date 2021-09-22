using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBullet : MonoBehaviour
{
    public float Damage;
    public float Weight;
    public int DropChance = 50;

    private Rigidbody2D Rig;
    private Item item;

    private void Awake()
    {
        Rig = GetComponent<Rigidbody2D>();
        item = GetComponent<Item>();
        item.Weight = Weight;
    }

    public void Hit(Transform obj, Vector2 Velocity, int Hit)
    {
        Vector2 Impulse = Velocity;
        int CurrantDamage = Mathf.RoundToInt(Hit + Damage);
        if (obj.tag == "Player")
        {
            if (item.Owner != null)
            {
                if (!item.Owner.GetComponent<SideOwn>().Comrade(obj))
                {
                    obj.GetComponent<Character>().GetEnemy(item.Owner.transform);
                    obj.GetComponent<Character>().GetDistanceHit(CurrantDamage, transform);
                    obj.GetComponent<Character>().GetKick(Impulse);
                }
                item.Owner = null;
            }
            else
            {
                obj.GetComponent<Character>().GetDistanceHit(CurrantDamage, transform);
                obj.GetComponent<Character>().GetKick(Impulse);
            }
            item.PlaySound("BulletHit");
            StartCoroutine("GetInEnemy", obj);
        }
        else if (obj.tag == "Enemy")
        {
            if (item.Owner != null)
            {
                if (!item.Owner.GetComponent<SideOwn>().Comrade(obj))
                {
                    obj.GetComponent<Ai>().GetDistanceHit(CurrantDamage, transform);
                    obj.GetComponent<Ai>().GetKick(Impulse);
                    obj.GetComponent<Ai>().GetEnemy(item.Owner.transform);
                    item.Owner = null;
                }
            }
            item.PlaySound("BulletHit");
            StartCoroutine("GetInEnemy", obj);
        }
        else if(obj.tag == "Object")
        {
            obj.GetComponent<Object>().GetHit(CurrantDamage, transform);
            item.PlaySound("BulletObjectHit");
            StartCoroutine("GetIn", obj);
        }

        item.Fly = false;
        item.HitRate = 1;
        item.Owner = null;
        if (DropChance < Random.Range(0,100))
        {
            GetComponent<SpriteRenderer>().color = Color.clear;
            Destroy(gameObject, 1f);
        }
    }
    IEnumerator GetInEnemy(Transform obj)
    {
        yield return new WaitForFixedUpdate();
        Rig.velocity = Vector2.zero;
        transform.parent = obj;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0.5f);
        yield return new WaitForSeconds(5f);
        while(obj.GetComponent<Rigidbody2D>().velocity.magnitude > 0.2f)
        {
            yield return new WaitForSeconds(0.2f);
        }
        transform.parent = null;
        yield break;
    }
    IEnumerator GetIn(Transform obj)
    {
        yield return new WaitForFixedUpdate();
        Rig.velocity = Vector2.zero;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0.5f);
        yield break;
    }
}
