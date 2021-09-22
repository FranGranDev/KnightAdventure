using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    private float Weight;
    private float Damage;
    private int ArrowsNum;
    public bool ArrowTaken = false;
    public GameObject Arrow;
    public Transform ArrowPlace;
    private Rigidbody2D rig;
    private Animator anim;
    private Collider2D Col;
    private int Stage = 0;
    private bool isFiring = false;
    private bool Button = false;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        Col = GetComponent<Collider2D>();

        Damage = GetComponent<ItemWeapon>().Damage;
        Weight = GetComponent<ItemWeapon>().Weight;
    }

    void SetFire(bool Fire)
    {
        Button = Fire;
    }

    IEnumerator StartFire(int Arrows)
    {
        if(!ArrowTaken)
        {
            TakeArrow(Arrows);
            isFiring = false;
            yield return new WaitForSeconds(0.1f);
            yield break;
        }
        else if (!isFiring)
        {
            ArrowsNum = Arrows;
            isFiring = true;
            while (Button)
            {
                if (Stage < 3)
                    Stage++;
                yield return new WaitForSeconds((Stage / 3) + (Weight / 5));
                if (transform.parent == null)
                    break;
            }
            StartCoroutine("LooseArrow");
        }
        yield break;
    }
    IEnumerator LooseArrow()
    {
        if (ArrowTaken && isFiring)
        {
            StopCoroutine("StartFire");
            int Power = Stage;
            for (; Stage > 0; Stage--)
                yield return new WaitForSeconds(0.01f);
            FireFire(Damage * Power);
            yield return new WaitForSeconds(0.2f);
            isFiring = false;
        }
        yield break;
    }
    void FireFire(float Power)
    {
        if(ArrowPlace.childCount > 0)
        {
            Transform Arrow = ArrowPlace.GetChild(0);
            Arrow.SendMessage("PhysicsOn", GetComponent<Collider2D>());
            Arrow.GetComponent<Item>().ThrowItem(transform.root, 2);
            Arrow.GetComponent<Rigidbody2D>().AddForce(transform.up * Power * 200f);
            Arrow.transform.parent = null;
            ArrowTaken = false;
        }
    }
    void TakeArrow(int Arrows)
    {
        if (transform.parent != null && Arrows > 0)
        {
            ArrowTaken = true;
            Instantiate(Arrow, ArrowPlace.position, ArrowPlace.rotation, ArrowPlace);
            if (transform.parent.root.tag == "Player")
            {
                transform.parent.root.SendMessage("DeleteItem", Arrow.GetComponent<Item>().Index);
            }
            else if (transform.parent.root.tag == "Enemy")
            {
                transform.parent.root.SendMessage("DeleteArrow");
            }
        }
    }
    void SetBegin()
    {
        Stage = 0;
        isFiring = false;
    }

    void FixedUpdate()
    {
        anim.Play(Stage.ToString());
        if (transform.parent == null)
        {
            rig.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            rig.bodyType = RigidbodyType2D.Kinematic;
            
        }
    }
}
