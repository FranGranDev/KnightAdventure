using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlant : MonoBehaviour
{
    public GameObject Object;
    private GameObject PlantedPlant;
    private GameObject Field;
    public bool GrowOnEvething = false;
    

    void Action(Vector3 Pos) //Plant
    {
        if (Object != null)
        {
            PlantedPlant = Instantiate(Object, Pos, new Quaternion(0, 0, 0, 0), null);
        }
        Destroy(gameObject);
    }
    void Action(Transform Field) //Plant
    {
        if (Object != null)
        {
            PlantedPlant = Instantiate(Object, Field.position, new Quaternion(0, 0, 0, 0), null);
            PlantedPlant.SendMessage("GetField", Field.gameObject);
        }
        Destroy(gameObject);
    }

}
