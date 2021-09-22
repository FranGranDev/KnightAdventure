using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public bool Planted = false;
    public bool isReady = false;
    public GameObject Plant;
    public GameObject[] DropItem = new GameObject[5];
    public GameObject[] DropSeed = new GameObject[5];

    void Start()
    {
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 1f);
        transform.tag = "Field";
        if(Plant != null)
        {
            Plant.GetComponent<GrowUp>().field = gameObject;
        }
    }

    public void GetPlant(GameObject obj)
    {
        Plant = obj;
        if (obj != null)
            Planted = true;
    }
    public void GetItem(int i, GameObject item)
    {
        DropItem[i] = item;
    }
    public void GetSeed(int i, GameObject Seed)
    {
        DropSeed[i] = Seed;
    }


    public void Take()
    {
        if(Plant != null)
        {
            Plant.GetComponent<GrowUp>().TakeAI(transform);
        }
    }

    private void Update()
    {
        if(Plant != null)
        {
            Planted = true;
            isReady = Plant.GetComponent<GrowUp>().IsReady;
        }
        else
        {
            Planted = false;
            isReady = false;
        }
    }


}
