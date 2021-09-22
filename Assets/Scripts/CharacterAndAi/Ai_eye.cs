using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai_eye : MonoBehaviour
{
    public float Distance;
    private bool OnEyes = false;
    private Ai Ai;
    private Transform PrevMan;
    private void Awake()
    {
        Ai = transform.root.GetComponent<Ai>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (transform.gameObject.activeSelf == false)
            return;
        if(Ai.Agressive && Ai.Enemy == null)
        {
            Transform obj = collision.transform.root;
            if (Ai.SideOwn.CheckConflick(obj))
            {
                if (obj.tag == "Enemy" && obj.GetComponent<Ai>().HP > 0)
                {
                    Ai.GetEnemy(obj);
                }
                else
                {
                    Ai.GetEnemy(obj);
                }
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (transform.gameObject.activeSelf == false)
            return;
        Transform obj = collision.transform.root;
        if (obj == PrevMan)
        {
            StartCoroutine("LooseEnemy", 10f);
        }
    }
    IEnumerator LooseEnemy(float Time)
    {
        yield return new WaitForSeconds(Time);
        if (PrevMan == Ai.Enemy)
        {
            Ai.StartCoroutine(Ai.LoseEnemy(1f));
        }
        PrevMan = null;
        yield break;
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}