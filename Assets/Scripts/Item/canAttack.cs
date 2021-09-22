using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canAttack : MonoBehaviour
{


    void CanAttack()
    {
        transform.parent.SendMessage("CanAttack", true);
    }
    void CantAttack()
    {
        transform.parent.SendMessage("CanAttack", false);
    }
}
