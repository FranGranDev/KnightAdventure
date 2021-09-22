using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Ai : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public Image Health;
    public GameObject HealthBar;
    public Text Level;
    private float HP;
    private float MaxHP;
    private Ai Player;

    private void Start()
    {
        Player = transform.root.GetComponent<Ai>();
    }
    public void GetLVL(string text)
    {
        Level.text = text;
    }
    public void GetName(string text)
    {
        Name.text = text;
    }

    public void GetMaxHP(float HealthPoints)
    {
        MaxHP = HealthPoints;
    }
    public void GetHP(float HealthPoints)
    {
        HP = HealthPoints;
    }
    public void HpTurn(bool On)
    {
        if(HealthBar != null)
            HealthBar.gameObject.SetActive(On);    
    }
    public void NameTurn(bool On)
    {
        if (Name == null)
            return;
        Name.gameObject.SetActive(On);
    }
    public void Requested(bool On)
    {
        if(On)
        {
            Name.color = Color.blue;
        }
        else
        {
            Name.color = Color.white;
        }
    }

    public void PrintForTime(string text, float time)
    {
        if (Name == null || !gameObject.activeSelf)
            return;
        StartCoroutine(Print(text, time));
    }
    private IEnumerator Print(string text, float time)
    {
        Name.gameObject.SetActive(true);
        string prevText = Name.text;
        Color color = Name.color;
        Name.text = text;
        Name.color = Color.white;
        yield return new WaitForSeconds(time);
        Name.text = prevText;
        Name.color = color;
        Name.gameObject.SetActive(false);
        yield break;
    }
    

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        Health.fillAmount = Mathf.Lerp(Health.fillAmount, HP / MaxHP, 0.1f);
    }
}
