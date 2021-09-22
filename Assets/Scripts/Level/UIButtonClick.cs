using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIButtonClick : MonoBehaviour, IPointerClickHandler
{
    public int Slot;
    public bool IsMarket = false;
    public bool IsDialog = false;
    public bool IsArmor = false;
    private Transform UI;
    private UIScript Script;

    public void Start()
    {
        UI = transform.root.GetChild(1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if(IsDialog)
            {
                if (!Input.GetButton("PlusAction"))
                    UI.GetComponent<UIScript>().DialogButtonClick(Slot);
            }
            else if (IsMarket)
            {
                if (!Input.GetButton("PlusAction"))
                    UI.GetComponent<UIScript>().MarketIconClick(Slot);
            }
            else if(IsArmor)
            {
                if (!Input.GetButton("PlusAction"))
                    UI.GetComponent<UIScript>().ArmorIconClick(Slot);
            }
            else
            {
                if (!Input.GetButton("PlusAction"))
                    UI.GetComponent<UIScript>().IconClick(Slot);
                else
                    UI.GetComponent<UIScript>().LetOut(Slot);
            }

        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            //Debug.Log("Middle click");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Debug.Log("Right click");
        }
    }
}
