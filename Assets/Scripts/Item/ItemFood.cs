using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFood : MonoBehaviour
{
    public int Satiety;
    public bool Drink;
    public bool FullyEaten = false;
    public enum EffectType { Null, Drunk, Power};
    public EffectType Effect;
    public float EffectTime;
    public Sprite[] Stage = new Sprite[5];
    private SpriteRenderer sprite;
    private int maxStage;
    public int nowStage = 0;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        for (int i = 0; i <= Stage.Length; i++)
        {
            if (Stage[i] == null || i == Stage.Length - 1)
            {
                maxStage = i;
                return;
            }
        }
    }
    void Start()
    {

        GetStage(nowStage);
    }
    public void GetStage(int stage)
    {
        nowStage = stage;
        sprite.sprite = Stage[nowStage];
        transform.GetComponent<Item>().Icon = Stage[nowStage];
    }

    public void Eat()
    {
        if (nowStage + 1 < Stage.Length)
        {
            nowStage++;
            sprite.sprite = Stage[nowStage];
            transform.GetComponent<Item>().Icon = Stage[nowStage];
            transform.parent.root.GetComponent<Character>().UpdateItem(transform);
            transform.parent.root.GetComponent<Character>().Health(Mathf.RoundToInt(Satiety / maxStage));
            transform.parent.root.GetComponent<Character>().GetEffect((int)Effect, EffectTime / maxStage);
            if (Drink)
            {
                GetComponent<Item>().PlaySoundRandom("Drink");
            }
            else
            {
                GetComponent<Item>().PlaySoundRandom("Eat");
            }
        }
        else if(FullyEaten)
        {
            if (Drink)
            {
                GetComponent<Item>().PlaySoundRandom("Drink");
            }
            else
            {
                GetComponent<Item>().PlaySoundRandom("Eat");
            }
            transform.root.GetComponent<Character>().DeleteItem(transform);
            Destroy(gameObject, 0.3f);
        }
    }
}
