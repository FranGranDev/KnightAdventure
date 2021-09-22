using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Nameplate : MonoBehaviour
{
    public enum TextType {Tips, Text, HouseName, Hz1, Hz2}
    public TextType Text;
    public int TextNum;
    public UIScript UI;
    private TextMeshProUGUI LocalText;
    private Transform LocalUI;
    private string MainText;

    void Start()
    {
        if(UI == null)
        {
            UI = GameObject.Find("MainUi").GetComponent<UIScript>();
        }
        MainText = LanguagesSystem.Language.NamePlate[(int)Text].Text[TextNum];
        LocalUI = transform.GetChild(0).GetChild(0);
        LocalText = LocalUI.GetChild(0).GetComponent<TextMeshProUGUI>();
        LocalText.text = MainText;
    }

    public void ShowText()
    {
        UI.PrintForTime(MainText, 1f);
    }
    
}
