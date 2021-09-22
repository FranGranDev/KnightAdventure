using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideOwn : MonoBehaviour
{
    public enum Side {Good, Bad, Neutral, Agressive, You}
    public Side ManSide;

    public void SetSide(Side x)
    {
        ManSide = x;
    }
    public void SetSide(int x)
    {
        ManSide = (Side)x;
    }
    public void SetSide(string x)
    {
        if (x == Side.Good.ToString())
            ManSide = Side.Good;
        if (x == Side.Bad.ToString())
            ManSide = Side.Bad;
        if (x == Side.Agressive.ToString())
            ManSide = Side.Agressive;
        if (x == Side.Neutral.ToString())
            ManSide = Side.Neutral;
        if (x == Side.You.ToString())
            ManSide = Side.You;
    }

    public bool CheckConflick(Transform Man)
    {
        if (Man == null || Man.GetComponent<SideOwn>() == null)
            return false;
        if (ManSide == Side.Good)
        {
            Side EnemySide = Man.GetComponent<SideOwn>().ManSide;
            if (EnemySide == Side.Good)
                return false;
            else if (EnemySide == Side.Bad)
                return true;
            else if (EnemySide == Side.Neutral)
                return false;
            else if (EnemySide == Side.Agressive)
                return true;
            else if (EnemySide == Side.You)
                return false;
            else
                return false;
        }
        else if (ManSide == Side.Bad)
        {
            Side EnemySide = Man.GetComponent<SideOwn>().ManSide;
            if (EnemySide == Side.Good)
                return true;
            else if (EnemySide == Side.Bad)
                return false;
            else if (EnemySide == Side.Neutral)
                return false;
            else if (EnemySide == Side.Agressive)
                return true;
            else if (EnemySide == Side.You)
                return false;
            else
                return false;
        }
        else if (ManSide == Side.Agressive)
        {
            Side EnemySide = Man.GetComponent<SideOwn>().ManSide;
            if (EnemySide == Side.Good)
                return true;
            else if (EnemySide == Side.Bad)
                return true;
            else if (EnemySide == Side.Neutral)
                return true;
            else if (EnemySide == Side.Agressive)
                return false;
            else if (EnemySide == Side.You)
                return true;
            else
                return true;
        }
        else if (ManSide == Side.Neutral)
        {
            Side EnemySide = Man.GetComponent<SideOwn>().ManSide;
            if (EnemySide == Side.Good)
                return false;
            else if (EnemySide == Side.Bad)
                return false;
            else if (EnemySide == Side.Neutral)
                return false;
            else if (EnemySide == Side.Agressive)
                return false;
            else if (EnemySide == Side.You)
                return false;
            else
                return false;
        }
        else if (ManSide == Side.You)
        {
            Side EnemySide = Man.GetComponent<SideOwn>().ManSide;
            if (EnemySide == Side.Good)
                return false;
            else if (EnemySide == Side.Bad)
                return true;
            else if (EnemySide == Side.Neutral)
                return false;
            else if (EnemySide == Side.Agressive)
                return true;
            else if (EnemySide == Side.You)
                return false;
            else
                return false;
        }
        else
            return false;
    }
    public bool Comrade(Transform Man)
    {
        if (ManSide == Man.GetComponent<SideOwn>().ManSide
        && Man.GetComponent<SideOwn>().ManSide != Side.Agressive)
            return true;
        else
            return false;
    }
}
