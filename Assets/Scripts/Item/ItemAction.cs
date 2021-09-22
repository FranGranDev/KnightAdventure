using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAction : MonoBehaviour
{
    private Rigidbody2D rig;
    private Collider2D Col;

    void Start()
    {
        Col = GetComponent<BoxCollider2D>();
        rig = GetComponent<Rigidbody2D>();
        Vector2 Force = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100));
        rig.AddForce(Force);
    }

    void Action()
    {
        Debug.Log(name);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

}
