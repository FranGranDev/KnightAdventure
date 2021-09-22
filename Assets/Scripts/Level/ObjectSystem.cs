using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSystem : MonoBehaviour
{
    public GameObject[] Object;
    
    public Transform GetObject(int i)
    {
        if (Object[i] != null)
            return Object[i].transform;
        else
            return null;
    }


}
