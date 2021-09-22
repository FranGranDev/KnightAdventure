using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthSystem : MonoBehaviour
{
    public Transform Object;
    public Transform BottomPoint;
    public bool Clear = false;
    public SpriteRenderer[] Sprite;
    public float Offset;
    public float[] Layer;
    public int NowLayer = 6;
    public float MaxY = 0;

    private void Start()
    {
        Layer = new float[15]{-0.35f,-0.3f,-0.25f,-0.2f, -0.15f, -0.1f, -0.05f, 0f, 0.05f, 0.1f, 0.15f, 0.2f, 0.25f, 0.3f, 0.35f};
    }

    public void SetLayer(int layer)
    {
        if(layer >= 0 && layer <= Layer.Length - 1)
        {
            NowLayer = layer;
        }
    }
    private void LayerPlus(Transform Obj)
    {
        if (NowLayer <= Obj.GetComponent<DepthSystem>().NowLayer)
        {
            if (Obj.GetComponent<DepthSystem>().NowLayer < Layer.Length - 1)
            {
                NowLayer = Obj.GetComponent<DepthSystem>().NowLayer + 1;
            }
            else
            {
                Obj.GetComponent<DepthSystem>().SetLayer(7);
                NowLayer = 8;
            }
        }
        
    }
    private void LayerMinus(Transform Obj)
    {
        if (NowLayer >= Obj.GetComponent<DepthSystem>().NowLayer)
        {
            if (Obj.GetComponent<DepthSystem>().NowLayer > 0)
            {
                NowLayer = Obj.GetComponent<DepthSystem>().NowLayer - 1;
            }
            else
            {
                Obj.GetComponent<DepthSystem>().SetLayer(7);
                NowLayer = 6;
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Transform obj = collision.contacts[i].collider.transform;
            if (transform != obj.root && obj.tag == "DepthSystem")
            {
                if (BottomPoint.position.y < obj.position.y + Offset)
                {
                    LayerMinus(obj);
                    if (Clear && obj.root.tag == "Player")
                    {
                        for (int a = 0; a < Sprite.Length; a++)
                        {
                            Sprite[a].color = new Vector4(1f, 1f, 1f, 0.8f);
                        }
                    }
                }
                else if(Clear)
                { 
                    for (int a = 0; a < Sprite.Length; a++)
                    {
                        Sprite[a].color = new Vector4(1f, 1f, 1f, 1f);
                    }
                }
                Object.localPosition = new Vector3(Object.localPosition.x, Object.localPosition.y, Layer[NowLayer]);
            }
        }

    }
    void OnCollisionExit2D(Collision2D collision)
    { 
        Transform obj = collision.collider.transform;
        if (transform != obj.root && obj.tag == "DepthSystem")
        {
            if(collision.contactCount < 1)
            {
                NowLayer = 7;
                MaxY = 0;
            }
            if (Clear)
            {
                for (int i = 0; i < Sprite.Length; i++)
                {
                    Sprite[i].color = new Vector4(1f, 1f, 1f, 1f);
                }
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        Transform obj = collision.transform;
        if (transform != obj.root && (obj.tag == "DepthSystem"))
        {
            
        }
    }

    private void FixedUpdate()
    {
        
    }
}
