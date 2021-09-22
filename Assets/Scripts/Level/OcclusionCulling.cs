using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionCulling : MonoBehaviour
{
    public BoxCollider2D Col;
    public Camera Cam;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.root.tag == "Enemy")
        {
            collision.transform.root.GetComponent<Ai>().AiOff();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.transform.root.tag == "Enemy")
        {
            
            collision.transform.root.GetComponent<Ai>().AiOn();
        }
    }

    private void FixedUpdate()
    {
        float Height = 2 * Cam.orthographicSize;
        float Width = Cam.aspect * Height;
        Col.size = new Vector2(1.75f * Width, 1.75f * Height);
    }
}
