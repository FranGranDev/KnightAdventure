using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelTrigger : MonoBehaviour
{
    public enum TapType {NewScene};
    public TapType TapAction;
    public enum EnterType {Tips, Text, CameraEffect, Kill };
    public EnterType EnterAction;
    private string text = "Press F to suck";
    public float Timer = 0f;
    public int Scene;
    public TextMeshProUGUI UiText;
    public camerascript Cam;
    public LevelSystem levelSystem;

    private void Awake()
    {
        if(levelSystem == null)
        {
            levelSystem = GameObject.Find("LevelSystem").GetComponent<LevelSystem>();
        }
        if(Cam == null)
        {
            Cam = GameObject.Find("MainCamera").GetComponent<camerascript>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch(EnterAction)
        {
            case EnterType.Tips:
                levelSystem.MainUI.PrintText(text);
                break;
            case EnterType.Kill:
                GameObject obj = collision.transform.root.gameObject;
                if(obj.tag == "Player")
                {
                    obj.GetComponent<Character>().Hp = 0;
                }
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        levelSystem.MainUI.PrintText("");
    }

    public void Action()
    {
        switch (TapAction)
        {
            case TapType.NewScene:
                levelSystem.GetToNewLevel(Scene);
                break;
        }
        Debug.Log("4");
    }
}
