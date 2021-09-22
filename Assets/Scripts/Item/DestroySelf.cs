using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    IEnumerator DestroyTime(float Time)
    {
        yield return new WaitForSeconds(Time);
        Destroy(gameObject);
        yield break;
    }
    void Destroy()
    {
        Destroy(gameObject);
    }
}
