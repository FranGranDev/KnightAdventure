using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    public Camera Camera;
    public Sprite[] CloudSprite;
    public GameObject CloudPref;
    public int CloudNum;
    public BoxCollider2D Collider2D;
    public GameObject[] Cloud;

    public void CreateCloud(bool Start,Vector2 Wind, int CloudNum)
    {
        Vector2 RandomPosition;
        if (Start)
        {
            RandomPosition = new Vector3(Random.Range(
            transform.position.x - Collider2D.size.x / 2,
            transform.position.x + Collider2D.size.x / 2),
            Random.Range(transform.position.y - Collider2D.size.y / 2,
            transform.position.y + Collider2D.size.y / 2), -transform.localPosition.z);
        }
        else
        {
            RandomPosition = new Vector3(
            transform.position.x - Collider2D.size.x / 1.8f,
            Random.Range(transform.position.y - Collider2D.size.y / 2,
            transform.position.y + Collider2D.size.y / 2), -transform.localPosition.z);
        }
        Cloud[CloudNum] = Instantiate(CloudPref, RandomPosition, transform.rotation, transform);
        float RandomScale = Random.Range(0.8f, 1.2f);
        Cloud[CloudNum].transform.localScale = new Vector3(RandomScale, RandomScale, 1f);
        Cloud[CloudNum].GetComponent<SpriteRenderer>().sprite = CloudSprite[Random.Range(0, CloudSprite.Length)];
        Cloud[CloudNum].GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 0);
        Cloud[CloudNum].GetComponent<Rigidbody2D>().AddForce(Wind);
    }
    private void Start()
    {
        Cloud = new GameObject[CloudNum];
        for(int i = 0; i < CloudNum; i++)
        {
            CreateCloud(true, new Vector2(100,0), i);
        }

        
    }

    private void FixedUpdate()
    {
        if (Camera.orthographicSize > 12)
        {
            float Clear = Camera.orthographicSize / 12 - 1.1f;
            for (int i = 0; i < transform.childCount; i++)
            {
                Cloud[i].GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, Clear);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.transform.parent == transform)
        {
            for (int i = 0; i < CloudNum; i++)
            {
                if(Cloud[i] == collision.transform.gameObject)
                {
                    
                    CreateCloud(false ,new Vector2(100, 0), i);
                    collision.transform.SendMessage("Destroy");
                }
            }
            
        }
    }
}
