using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForAndrey : MonoBehaviour
{
    public float Speed;
    public float JumpForce;
    private bool OnGround = true;
    private float prevx;

    private float startScale;

    private SpriteRenderer sprite;
    private Rigidbody2D rig;

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        startScale = transform.localScale.x;
    }

    private void Move()
    {
        float force = Input.GetAxis("MovementX") * Speed;
        if(force != 0)
        {
            prevx = force;
        }
        rig.velocity = new Vector2(force, rig.velocity.y);
    }
    private void Flip()
    {
        if(rig.velocity.x > 0)
        {
            transform.localScale = new Vector3(startScale, transform.localScale.y, 1);
        }
        else
        {
            transform.localScale = new Vector3(-startScale, transform.localScale.y, 1);
        }
    }
    private void Jump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && OnGround)
        {
            rig.AddForce(transform.up * JumpForce * 100f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Ground")
        {
            OnGround = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            OnGround = false;
        }
    }

    private void FixedUpdate()
    {
        Move();
        Flip();
        Jump();
    }
}
