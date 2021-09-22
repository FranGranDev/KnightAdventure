using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour
{
    public Character Player;
    private Transform InteractWith;

    public Transform Text;
    private TextMeshProUGUI TextUI;
    private Image BackText;
    private Color BackTextColor;

    public Transform Tips;
    private TextMeshProUGUI TextTips;
    private Image BackTips;
    private Color BackTipsColor;

    private IndexSystem IndexSystem;

    public Transform Dialog;
    private Image DialogBG;
    private Color DialogBGColor;
    private Transform[] DialogButton = new Transform[7];
    private TextMeshProUGUI[] DialogButtonName = new TextMeshProUGUI[7];
    public enum DialogType { AskAbout, AskAboutLand, AskAboutSeller, Compliment, Offend, Plunder, Hire, Market, Quest, TakeMessage, MainStory, Goodbye, NULL }
    public DialogType[] DialogButtonType = new DialogType[7];

    public Animator anim;

    public Image Health;
    private float HP;
    private float MaxHP;

    public Image Stamina;
    private float ST;
    private float MaxST;

    public Transform Inventory;
    public Transform Mouse;
    private bool Taken = false;
    private bool WeaponTaken = false;
    private bool Buying = false;
    public bool isChest = false;
    private int LastSlot;
    public Transform ItemAtMousePrefab;
    private Transform ItemAtMouse;
    private Transform ItemSlotAtMouse;

    public Transform Market;
    public TextMeshProUGUI Budget;
    public TextMeshProUGUI MarketName;
    public TextMeshProUGUI MoneyName;
    public int AiMoney = 0;
    public Transform[] MarketItemSlot = new Transform[30];
    private Image[] MarketItemIcon = new Image[30];

    public Sprite PassiveSlot;
    public Sprite ActiveSlot;
    public Transform[] WeaponSlotImagine;
    private Transform[] WeaponSlot = new Transform[4];
    private Image[] WeaponIcon = new Image[4];
    public int NowWeaponSlot = 1;
    
    private Transform[] ItemSlot = new Transform[30];
    private Image[] ItemIcon = new Image[30];

    private Transform[] ArmorSlot = new Transform[2];
    private Image[] ArmorImagine = new Image[2];

    public Transform Equipment;

    private void Awake()
    {
        IndexSystem = Resources.Load<IndexSystem>("IndexSystem");
        anim = GetComponent<Animator>();

        for (int i = 1; i < 8; i++)
        {
            DialogButton[i - 1] = Dialog.GetChild(i);
        }
        for (int i = 0; i < 7; i++)
        {
            DialogButtonName[i] = DialogButton[i].GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        for (int i = 0; i < 30; i++)
        {
            ItemIcon[i] = Inventory.GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>();
            MarketItemIcon[i] = Market.GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>();
        }
        for (int i = 0; i < 4; i++)
        {
            WeaponIcon[i] = WeaponSlotImagine[i].GetChild(0).GetComponent<Image>();
        }
        DialogBG = Dialog.GetChild(0).GetComponent<Image>();
        DialogBGColor = new Vector4(0f, 0f, 0f, 0.3f);


        TextUI = Text.GetChild(1).GetComponent<TextMeshProUGUI>();
        BackText = Text.GetChild(0).GetComponent<Image>();
        BackTextColor = BackText.color;

        TextTips = Tips.GetChild(1).GetComponent<TextMeshProUGUI>();
        BackTips = Tips.GetChild(0).GetComponent<Image>();
        BackTipsColor = BackTips.color;

        ArmorImagine[0] = Equipment.GetChild(0).GetChild(0).GetComponent<Image>();

        ArmorImagine[1] = Equipment.GetChild(1).GetChild(0).GetComponent<Image>();
    }
    private void Start()
    {
        Player = GameObject.Find("Character").GetComponent<Character>();
    }

    public void PrintText(string text)
    {
        if (text == "")
        {
            StartCoroutine(HideText(0.1f));
            return;
        }       
        TextUI.text = text;
        BackTextColor = new Vector4(0.3f, 0.3f, 0.3f, 0.4f);
        StopCoroutine("HideText");
    }
    public void PrintForTime(string text, float Time)
    {
        if (text == "")
            return;
        StopCoroutine("HideText");
        TextUI.text = text;
        BackTextColor = new Vector4(0.3f, 0.3f, 0.3f, 0.4f);
        StartCoroutine("HideText", Time);
    }
    IEnumerator HideText(float Time)
    {
        yield return new WaitForSeconds(Time);
        BackTextColor = Color.clear;
        TextUI.text = "";
        yield break;
    }

    public void TurnQuestTips(bool On)
    {
        Tips.gameObject.SetActive(On);
    }
    public void PrintTips(string text)
    {
        if (text == "")
        {
            Tips.gameObject.SetActive(false);
        }
        else
        {
            Tips.gameObject.SetActive(true);
            TextTips.text = text;
        }

    }

    public void GetMaxHP(float HealthPoints)
    { 
        MaxHP = HealthPoints;
    }
    public void GetHP(float HealthPoints)
    {
        HP = HealthPoints;
    }

    public void GetMaxStamina(float Stamina)
    {
        MaxST = Stamina;
    }
    public void GetStamina(float Stamina)
    {
        ST = Stamina;
    }


    public void GetActiveSlot(int Slot)
    {
        for (int i = 0; i < 4; i++)
        {
            WeaponSlotImagine[i].GetComponent<Image>().sprite = PassiveSlot;
        }
        WeaponSlotImagine[Slot].GetComponent<Image>().sprite = ActiveSlot;
    }
    public void GetWeapon(int Slot, Transform Weapon)
    {
        if (Weapon != null)
        {
            WeaponSlot[Slot] = Weapon;
            WeaponIcon[Slot].sprite = Weapon.GetComponent<Item>().Icon;
            WeaponIcon[Slot].color = Color.white;
        }
        else
        {
            WeaponSlot[Slot] = null;
            WeaponIcon[Slot].color = Color.clear;
            //WeaponIcon[Slot].sprite = null;
        }
    }
    public void GetItem(int i, Transform Item)
    {
        if(Item != null)
        {
            ItemSlot[i] = Item;
            ItemIcon[i].sprite = Item.GetComponent<Item>().Icon;
            ItemIcon[i].color = Color.white;
        }
        else
        {
            ItemSlot[i] = null;
            ItemIcon[i].color = Color.clear;
        }
    }

    public void ArmorIconClick(int i)
    {
        if(Taken && !Buying)
        {
            
            if (ArmorSlot[i] == null)
            {
                Debug.Log("Put");
                Player.GetArmor(i, ItemSlotAtMouse);
                ItemAtMouse.SendMessage("Destroy");
                ItemSlotAtMouse = null;
                Taken = false;
            }
        }
        else if(!Taken && !Buying)
        {
            Debug.Log("Take");
            if (ArmorSlot[i] != null)
            {
                Taken = true;
                ItemAtMouse = Instantiate(ItemAtMousePrefab, new Vector3(Mouse.position.x, Mouse.position.y, -0.2f), Mouse.rotation, Mouse);
                ItemSlotAtMouse = ArmorSlot[i];
                ItemAtMouse.GetComponent<Image>().sprite = ArmorImagine[i].sprite;
                ItemAtMouse.GetComponent<Image>().SetNativeSize();
                Player.GetArmor(i, null);
            }
        }
    }

    public void IconClick(int i)
    {
        if (!Taken) //TakeFromInventory
        {
            if (i < 0)
            {
                if (WeaponSlot[-i - 1] == null)
                    return;
                else
                {
                    WeaponTaken = true;
                    LastSlot = -i - 1;
                    Buying = false;
                }
            }
            else
            {
                if (ItemSlot[i] == null)
                    return;
                else
                {
                    WeaponTaken = false;
                    LastSlot = i;
                }
            }

            if (i < 0) //WeaponSlot
            {
                i = Mathf.Abs(i) - 1;
                Taken = true;
                ItemAtMouse = Instantiate(ItemAtMousePrefab, new Vector3(Mouse.position.x, Mouse.position.y, -0.2f), Mouse.rotation, Mouse);
                ItemSlotAtMouse = WeaponSlot[i];
                ItemAtMouse.GetComponent<Image>().sprite = WeaponIcon[i].sprite;
                ItemAtMouse.GetComponent<Image>().SetNativeSize();
                Player.GetWeapon(i, null);
            }
            else
            {
                Taken = true;
                ItemAtMouse = Instantiate(ItemAtMousePrefab, new Vector3(Mouse.position.x, Mouse.position.y, -0.2f), Mouse.rotation, Mouse);
                ItemSlotAtMouse = ItemSlot[i];
                ItemAtMouse.GetComponent<Image>().sprite = ItemIcon[i].sprite;
                ItemAtMouse.GetComponent<Image>().SetNativeSize();
                Player.GetItem(i, null);
            }
        }
        else if(Taken) //PutIntoInventory
        {
            if (i < 0) //WeaponSlot
            {
                i = Mathf.Abs(i) - 1;
                if (Buying && !isChest)
                {
                    string[] Replica = LanguagesSystem.Language.Market;
                    if (Player.Money>= MarketItemSlot[i].GetComponent<Item>().Cost)
                    {
                        int Cost = MarketItemSlot[i].GetComponent<Item>().Cost;
                        Player.Money-= Cost;
                        PrintText(Replica[0] + " " + Cost);
                        Budget.text = Player.Money.ToString() + " " + Replica[4];
                        AiMoney += Cost;
                        MoneyName.text = AiMoney.ToString() + " " + Replica[4];
                        StartCoroutine("HideText", 1f);
                    }
                    else
                    {
                        PrintText(Replica[2]);
                        StartCoroutine("HideText", 1f);
                        return;
                    }
                }
                if (WeaponSlot[i] == null)
                {
                    Player.GetWeapon(i, ItemSlotAtMouse);
                    WeaponIcon[i].sprite = ItemAtMouse.GetComponent<Image>().sprite;
                    ItemAtMouse.SendMessage("Destroy");
                    Buying = false;
                    ItemSlotAtMouse = null;
                    Taken = false;
                }
                else if(!Buying)
                {
                    if (WeaponTaken)
                    {
                        Player.GetWeapon(LastSlot, WeaponSlot[i]);
                        WeaponSlot[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        Player.GetItem(LastSlot, WeaponSlot[i]);
                        WeaponSlot[i].gameObject.SetActive(false);
                        
                    }
                    Player.GetWeapon(i, ItemSlotAtMouse);
                    WeaponIcon[i].sprite = ItemAtMouse.GetComponent<Image>().sprite;
                    ItemAtMouse.SendMessage("Destroy");
                    ItemSlotAtMouse = null;
                    Taken = false;
                }
                
            }
            else
            {
                if (ItemSlot[i] == null)
                {
                    if (Buying && !isChest)
                    {
                        string[] Replica = LanguagesSystem.Language.Market;
                        if (Player.Money >= ItemSlotAtMouse.GetComponent<Item>().Cost)
                        {
                            int Cost = ItemSlotAtMouse.GetComponent<Item>().Cost;
                            Player.Money -= Cost;
                            Budget.text = Player.Money.ToString() + " " + Replica[4];
                            PrintText(Replica[0] + " " + Cost);
                            AiMoney += Cost;
                            MoneyName.text = AiMoney.ToString() + " " + Replica[4];
                            StartCoroutine("HideText", 1f);
                        }
                        else
                        {
                            PrintText(Replica[2]);
                            StartCoroutine("HideText", 1f);
                            return;
                        }
                    }
                    Player.GetItem(i, ItemSlotAtMouse);
                    ItemIcon[i].sprite = ItemAtMouse.GetComponent<Image>().sprite;
                    ItemAtMouse.SendMessage("Destroy");
                    Buying = false;
                    ItemSlotAtMouse = null;
                    Taken = false;
                }
                else if(!Buying)
                {
                    if (WeaponTaken)
                    {
                        Player.GetWeapon(LastSlot, ItemSlot[i]);
                        WeaponSlot[LastSlot].gameObject.SetActive(false);
                    }
                    else
                    {
                        Player.GetItem(LastSlot, ItemSlot[i]);
                        ItemSlot[i].gameObject.SetActive(false);
                    }
                    Player.GetItem(i, ItemSlotAtMouse);
                    ItemIcon[i].sprite = ItemAtMouse.GetComponent<Image>().sprite;
                    ItemAtMouse.SendMessage("Destroy");
                    ItemSlotAtMouse = null;
                    Taken = false;
                }
                
            }
            
        }
    }
    public void LetOut(int i)
    {
        if(i < 0)
        {
            i = Mathf.Abs(i) - 1;
            Player.LetOutWeapon(i);
        }
        else
        {
            Player.LetOutItem(i);
        }
    }

    public void MarketIconClick(int i)
    {
        if (!Taken)
        {
            if (MarketItemSlot[i] == null)
            {
                return;
            }
            WeaponTaken = false;
            Taken = true;
            Buying = true;
            ItemAtMouse = Instantiate(ItemAtMousePrefab, new Vector3(Mouse.position.x, Mouse.position.y, -0.2f), Mouse.rotation, Mouse);
            if (!MarketItemSlot[i].gameObject.scene.IsValid())
            {
                ItemSlotAtMouse = Instantiate(MarketItemSlot[i]);
                ItemSlotAtMouse.gameObject.SetActive(false);
            }
            else
            {
                ItemSlotAtMouse = MarketItemSlot[i];
            }
            ItemAtMouse.GetComponent<Image>().sprite = MarketItemIcon[i].sprite;
            ItemAtMouse.GetComponent<Image>().SetNativeSize();
            if (!isChest)
            {
                InteractWith.GetComponent<Dialogs>().GetMarketItem(i, null);
            }
            else
            {
                InteractWith.GetComponent<Chest>().GetItem(i, null);
            }
        }
        else if(MarketItemSlot[i] == null)
        {
            string[] Replica = LanguagesSystem.Language.Market;
            
            if (!Buying && !isChest)
            {
                int Cost = ItemSlotAtMouse.GetComponent<Item>().Cost;
                if (InteractWith.GetComponent<Dialogs>().Money >= Cost)
                {
                    Player.Money += Cost;
                    InteractWith.GetComponent<Dialogs>().Money -= Cost;
                    PrintText(Replica[1] + " " + Cost);
                    Budget.text = Player.Money.ToString() + " " + Replica[4];
                    MoneyName.text = InteractWith.GetComponent<Dialogs>().Money.ToString() + " " + Replica[4];
                    StartCoroutine("HideText", 1f);
                }
                else
                {
                    PrintText(Replica[2]);
                    StartCoroutine("HideText", 1f);
                    return;
                }
            }
            if (!isChest)
            {
                InteractWith.GetComponent<Dialogs>().GetMarketItem(i, ItemSlotAtMouse);
                ItemSlotAtMouse.parent = InteractWith.GetComponent<Dialogs>().Hand;
                ItemSlotAtMouse.gameObject.SetActive(false);
            }
            else
            {
                InteractWith.GetComponent<Chest>().GetItem(i, ItemSlotAtMouse);
                ItemSlotAtMouse.parent = InteractWith.GetComponent<Chest>().ItemsPlace;
                ItemSlotAtMouse.gameObject.SetActive(false);
            }
            MarketItemIcon[i].sprite = ItemAtMouse.GetComponent<Image>().sprite;
            ItemAtMouse.SendMessage("Destroy");
            Taken = false;
            Buying = false;
            ItemSlotAtMouse = null;
        }
    }
    public void GetMarketItem(int i, GameObject Item)
    {
        if (Item != null)
        {
            MarketItemSlot[i] = Item.transform;
            MarketItemIcon[i].sprite = Item.GetComponent<Item>().Icon;
            MarketItemIcon[i].color = Color.white;
        }
        else
        {
            MarketItemSlot[i] = null;
            MarketItemIcon[i].color = Color.clear;
        }
    }
    public void ClearMarket()
    {
        for(int i = 0; i < MarketItemSlot.Length; i++)
        {
            GetMarketItem(i, null);
        }
    }
    public void OpenChestMarket(Transform obj)
    {
        MarketName.text = "Chest";
        MoneyName.text = "";
        InteractWith = obj;
        isChest = true;
        Market.gameObject.SetActive(true);
    }
    public void TurnMarket(bool x)
    {
        string[] Replica = LanguagesSystem.Language.Market;
        if (x)
        {
            MarketName.text = InteractWith.GetComponent<Ai>().GetName();
            Budget.gameObject.SetActive(true);
            Budget.text = Player.Money.ToString() + " " + Replica[4];
            MoneyName.text = AiMoney.ToString() + " " + Replica[4];
            Dialog.gameObject.SetActive(false);
        }
        else
        {
            Player.GetComponent<Character>().EndDialog(-1);
            Budget.gameObject.SetActive(false);
            Buying = false;
        }
        isChest = false;
        Market.gameObject.SetActive(x);
    }
    public void CloseMarket()
    {
        if (isChest)
        {
            Market.gameObject.SetActive(false);
            OpenInventory(false);
            InteractWith.GetComponent<Chest>().Close();
        }
        else if(InteractWith != null)
        {
            Player.GetComponent<Character>().EndDialog(-1);
            Market.gameObject.SetActive(false);
            Budget.gameObject.SetActive(false);
            StartCoroutine("HideText", 0.5f);
            Buying = false;
        }
        Market.gameObject.SetActive(false);
    }


    public void GetArmor(int i, Transform Armor)
    {
        if (Armor == null)
        {
            ArmorSlot[i] = null;
            ArmorImagine[i].sprite = null;
            ArmorImagine[i].color = Color.clear;
        }
        else
        {
            ArmorSlot[i] = Armor;
            ArmorImagine[i].sprite = Armor.GetComponent<Item>().Icon;
            ArmorImagine[i].color = Color.white;
        }
    }

    public void StartDialog(Transform Ai)
    {
        Dialog.gameObject.SetActive(true);
        InteractWith = Ai;
    }
    public void SetDialogButton(int num, string text, int Type)
    {
        if ((DialogType)Type != DialogType.NULL)
            DialogButton[num].gameObject.SetActive(true);
        else
            DialogButton[num].gameObject.SetActive(false);
        DialogButtonName[num].text = text;
        DialogButtonType[num] = (DialogType)Type;
    }
    public void SetDialogButtonColor(int num, Color color)
    {
        DialogButtonName[num].color = color;
    }
    public void DeleteDialogButton(int num)
    {
        for (int i = num; i < DialogButton.Length; i++)
        {
            if (DialogButtonType[i] == DialogType.NULL)
            {
                
                DialogButton[i - 1].gameObject.SetActive(false);
            }
        }
        for(int i = num; i < DialogButton.Length - 1; i++)
        {
            DialogButtonName[num].text = DialogButtonName[num + 1].text;
            DialogButtonType[num] = DialogButtonType[num + 1];
        }

    }
    public void DialogButtonClick(int num)
    {
        switch(DialogButtonType[num])
        {
            case DialogType.AskAbout:
                Player.GetComponent<Character>().AskAbout(num);
                break;
            case DialogType.AskAboutLand:
                Player.GetComponent<Character>().AskAboutLand(num);
                break;
            case DialogType.AskAboutSeller:
                Player.GetComponent<Character>().AskAboutSeller(num);
                break;
            case DialogType.Compliment:
                Player.GetComponent<Character>().Compliment(num);
                break;
            case DialogType.Goodbye:
                Player.GetComponent<Character>().EndDialog(num);
                break;
            case DialogType.Hire:
                Player.GetComponent<Character>().Hire(num);
                break;
            case DialogType.Market:
                Player.GetComponent<Character>().OpenMarket(num);
                break;
            case DialogType.Offend:
                Player.GetComponent<Character>().Offend(num);
                break;
            case DialogType.Plunder:
                Player.GetComponent<Character>().Plunder(num);
                break;
            case DialogType.TakeMessage:
                Player.GetComponent<Character>().TakeMessage(num);
                break;
            case DialogType.Quest:
                Player.GetComponent<Character>().StartQuest(num);
                break;
            case DialogType.MainStory:
                Player.GetComponent<Character>().MainStory(num);
                break;
        }
    }
    public void EndDialog()
    {
        for (int i = 0; i < DialogButton.Length; i++)
        {
            DialogButton[i].gameObject.SetActive(false);
        }
        if(Market.gameObject.activeSelf)
        {
            Market.gameObject.SetActive(false);
        }
        Dialog.gameObject.SetActive(false);
        StartCoroutine(HideText(1));
        InteractWith = null;
    }

    public void OpenInventory(bool open)
    {
        Inventory.gameObject.SetActive(open);
    }


    private void FixedUpdate()
    {
        
        float HpFill = HP / MaxHP;
        Health.fillAmount = Mathf.Lerp(Health.fillAmount, HpFill, 0.1f);
        Stamina.fillAmount = Mathf.Lerp(Stamina.fillAmount, ST / MaxST, 0.1f);
        BackText.color = BackTextColor;
        Mouse.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Mouse.localScale = transform.GetChild(0).localScale * 10f;
        
    }
    private void Update()
    {
        if (Input.GetButtonDown("Action1"))
        {
            GameObject obj = Player.CheckObject(5);
            if (Taken && !Buying && obj == null)
            {
                if (WeaponTaken)
                {
                    Player.GetWeapon(LastSlot, ItemSlotAtMouse);
                    Player.LetOutWeapon(LastSlot);
                    WeaponIcon[LastSlot].sprite = ItemAtMouse.GetComponent<Image>().sprite;
                }
                else
                {
                    Player.GetItem(LastSlot, ItemSlotAtMouse);
                    Player.LetOutItem(LastSlot);
                    ItemIcon[LastSlot].sprite = ItemAtMouse.GetComponent<Image>().sprite;
                }
                ItemAtMouse.SendMessage("Destroy");
                Taken = false;
            }
        }
    }
}

