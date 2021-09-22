using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiDurakWall : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.collider.gameObject;
        if(obj.tag == "Item")
        {
            obj.GetComponent<Rigidbody2D>().velocity *= -1;
        }
    }
}
