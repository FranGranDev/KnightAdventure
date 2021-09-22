using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage;
    private bool isFly = false;
    private float Power = 0;
    private int layer;
    private int Flylayer;

    public Transform Player;
    public Transform Bow;

    private Rigidbody2D rig;
    private BoxCollider2D Col;

    private void Awake()
    {
        layer = gameObject.layer;
        Flylayer = 11;
    }
    void Start()
    {
        Col = GetComponent<BoxCollider2D>();
        rig = GetComponent<Rigidbody2D>();
    }

    void Action()
    {
        Debug.Log(name);
    }

    void GetPlayer(Transform x)
    {
        Player = x;
    }

    public void Fly(float power, Transform Keeper, Transform FromBow)
    {
        transform.parent = null;
        Player = Keeper;
        Bow = FromBow;
        gameObject.layer = Flylayer;
        isFly = true;
        Power = power;
        rig.AddForce(transform.up * power);
    }
    Vector2 Direct(Transform obj)
    {
        return new Vector2(obj.transform.position.x - transform.position.x, obj.transform.position.y - transform.position.y).normalized;
    }
    void Destroy()
    {
        Destroy(gameObject);
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(isFly && collision.transform.root != Player)
        {
            Hit(collision.transform);
        }
    }
    void Hit(Transform obj)
    {
        int CurrantDamage = Mathf.RoundToInt(Mathf.Sqrt(rig.velocity.magnitude * Damage));
        if (obj.root.tag == "Player")
        {
            Character character = obj.root.GetComponent<Character>();
            character.GetDistanceHit(CurrantDamage , transform);
            transform.parent = obj.root;
            rig.velocity = Vector2.zero;
        }
        else if(obj.root.tag == "Enemy")
        {
            Ai ai = obj.root.GetComponent<Ai>();
            ai.GetDistanceHit(CurrantDamage, transform);
            transform.parent = obj.root;
            rig.velocity = Vector2.zero;
        }
        else
        {

        }
        gameObject.layer = layer;
        isFly = false;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0.8f);
    }
    IEnumerator EndFly()
    {
        if(isFly && rig.velocity.magnitude < 0.2f)
        {
            rig.velocity = Vector2.zero;
            isFly = false;
            gameObject.layer = layer;
        }
        yield break;
    }

    void FixedUpdate()
    {
        if (transform.parent != null)
        {
            rig.bodyType = RigidbodyType2D.Kinematic;
        }
        else if(isFly && rig.velocity.magnitude == 0)
        {
            StartCoroutine("EndFly");
        }
    }
}
