using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowUp : MonoBehaviour
{
    public Transform[] Plant = new Transform[15];
    private int SpikeNum = 1;
    public GameObject ItemDrop;
    public GameObject SeedsDrop;
    public GameObject field;
    public int NumItem = 1;
    public int Stage = 0;

    public int MaxStage = 12;
    public int ReadyStage = 0;
    public bool IsReady;
    public float AveregeTime = 30;
    private Transform Bottom;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).tag == "Plant")
            {
                Plant[i] = transform.GetChild(i);
                SpikeNum = i + 1;
            }
        }
        if(field != null)
        {
            field.SendMessage("GetPlant", gameObject);
        }
        StartCoroutine("Grow", AveregeTime);
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0f);
    }

    IEnumerator Grow(float AveregeTime)
    {
        while (Stage < ReadyStage)
        {
            float time = AveregeTime / 4 + Random.Range(-1, 1);
            Stage++;
            for (int i = 0; i < SpikeNum; i++)
            {
                Plant[i].gameObject.SendMessage("GetStage", Stage - 1);
            }
            yield return new WaitForSeconds(time);
        }
        IsReady = true;

        yield return new WaitForSeconds(2 * AveregeTime);

        while (Stage <= MaxStage)
        {
            float time = AveregeTime / 2 + Random.Range(0, 5);
            Stage++;
            for (int i = 0; i < SpikeNum; i++)
            {
                Plant[i].gameObject.SendMessage("GetStage", Stage);
            }
            yield return new WaitForSeconds(time);
        }
        yield break;
    }

    public void GetField(GameObject obj)
    {
        field = obj;
        field.SendMessage("GetPlant", gameObject);
    }
    public void TakeAI(Transform obj)
    {
        
        if (ItemDrop != null)
        {
            for (int i = 0; i < NumItem; i++)
            {
                GameObject item = Instantiate(ItemDrop, transform.position, Quaternion.identity, null);
                item.SetActive(true);
                field.GetComponent<Field>().GetItem(i, item);
            }
           
        }
        if (SeedsDrop != null)
        {
            int Rand = Random.Range(0, 2);
            for (int i = 0; i < Rand; i++)
            {
                GameObject Seed = Instantiate(SeedsDrop, transform.position, Quaternion.identity, null);
                Seed.SetActive(true);
                field.GetComponent<Field>().GetSeed(i, Seed);
            }
        }
        for (int i = 0; i < SpikeNum; i++)
        {
            Plant[i].gameObject.SendMessage("Destroy", transform);
        }
        field.GetComponent<Field>().Planted = false;
        field.GetComponent<Field>().Plant = null;
        field = null;
        Destroy(gameObject);
    }
    public void Take()
    {

        if (field != null)
        {
            field.GetComponent<Field>().Plant = null;
            field = null;
        }
        if (IsReady)
        {
            if (ItemDrop != null)
            {
                for (int i = 0; i < NumItem; i++)
                {
                    GameObject item = Instantiate(ItemDrop, transform.position, Quaternion.identity, null);
                    ItemDrop = item;
                }
            }
            if (SeedsDrop != null)
            {
                int Rand = Random.Range(0, 2);
                for (int i = 0; i < Rand; i++)
                {
                    GameObject Seeds = Instantiate(SeedsDrop, transform.position, Quaternion.identity, null);
                    SeedsDrop = Seeds;
                }
            }
        }
        else if (SeedsDrop != null)
        {
            SeedsDrop = Instantiate(SeedsDrop, transform.position, Quaternion.identity, null);
        }
        for (int i = 0; i < SpikeNum; i++)
        {
            Plant[i].gameObject.SendMessage("Destroy", transform);
        }
     
        Destroy(gameObject);
    }
}