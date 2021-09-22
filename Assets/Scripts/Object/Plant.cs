using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public Sprite[] StateSprite;
    public int State = 0;
    public GameObject AnimObj;
    private SpriteRenderer Sprite;
    private Rigidbody2D Rig;
    private float Long = 1f;
    private float Stomp = 0f;

    private void Start()
    {
        Sprite = GetComponent<SpriteRenderer>();
        Sprite.sprite = StateSprite[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.root.tag == "Player" || collision.transform.root.tag == "Enemy")
        {
            Long = 0.1f;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.root.tag == "Player" || collision.transform.root.tag == "Enemy")
        {
            Long = 1 - Stomp / 15f;
            if(Stomp < 10f)
                Stomp += 0.5f;
        }
    }

    void GetStage(int x)
    {
        if(x < StateSprite.Length)
            State = x;
    }

    private void FixedUpdate()
    {
        Vector3 Size = new Vector3(1f, Long, 1f);
        transform.localScale = Vector3.Lerp(transform.localScale, Size, 0.1f);
        Sprite.sprite = StateSprite[State];
    }

    void Destroy(Transform obj)
    {
        Destroy(gameObject);
        if (AnimObj != null)
        {
            AnimObj = Instantiate(AnimObj, transform.position, transform.rotation, null);
            Vector2 Direction = new Vector2(obj.transform.position.x - transform.position.x, obj.transform.position.y - transform.position.y);
            AnimObj.GetComponent<SpriteRenderer>().sprite = StateSprite[State];
            AnimObj.GetComponent<Rigidbody2D>().AddForce(Direction * 200f);
            AnimObj.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-200f, 200f));
            AnimObj.SendMessage("DestroyTime", 0.2f);
        }
    }
}
