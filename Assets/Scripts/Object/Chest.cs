using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public bool Closed;
    public bool NeedKey;
    public GameObject Key;
    public Transform[] Items;
    private Animator anim;
    private UIScript Ui;
    public Transform ItemsPlace;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        Ui = GameObject.Find("MainUI").GetComponent<UIScript>();
    }

    public void GetItem(int i, Transform item)
    {
        if (item != null)
        {
            Items[i] = item;
            Ui.GetMarketItem(i, item.gameObject);
        }
        else
        {
            Items[i] = null;
            Ui.GetMarketItem(i, null);
        }
    }

    public void Open(Transform Player)
    {
        if(Closed && NeedKey && Key != null)
        {
            int KeyIndex = Key.GetComponent<Item>().Index;
            Player.GetComponent<Character>().OpenChest(this, KeyIndex);
            
        }
        else
        {
            Player.GetComponent<Character>().OpenChest(this, -1);
            anim.Play("OpenAnim");
        }
        
    }
    public void Close()
    {
        anim.Play("CloseAnim");
    }

}
