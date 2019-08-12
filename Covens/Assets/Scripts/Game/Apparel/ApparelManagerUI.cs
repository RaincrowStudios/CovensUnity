using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Linq;
using Raincrow.Analytics.Events;

public class ApparelManagerUI : MonoBehaviour
{
    public static ApparelManagerUI Instance { get; set; }
    public Animator wardrobeAnim;
    public Transform container;
    public GameObject ApparelButton;
    public Text subTitle;
    public Dictionary<string, CosmeticData> buttonDict = new Dictionary<string, CosmeticData>();
    string currentFilter = "none";
    public List<GameObject> highlights;
    public static bool equipChanged = false;
    [SerializeField] private UIKytelerGrid ringsUI;
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
        
    void Awake()
    {
        Instance = this;
        DisableObjects();
    }

    public void Show()
    {
        ApparelManager.instance.SetupApparel();

        UIStateManager.Instance.CallWindowChanged(false);
        SoundManagerOneShot.Instance.MenuSound();
        container.parent.gameObject.SetActive(true);
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
        wardrobeAnim.Play("in");
        ShowItems();
        ShowAll();
    }

    public void Hide()
    {
        UIStateManager.Instance.CallWindowChanged(true);
        SoundManagerOneShot.Instance.MenuSound();
        wardrobeAnim.Play("out");

        if (CheckEquipsChanged())
        {
            EquipmentAnalytics.ChangeEquipment();
            ApparelManager.instance.SendEquipChar();
        }

        equipChanged = false;
        Invoke("DisableObjects", 1f);
    }

    private void DisableObjects()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
    }

    private bool CheckEquipsChanged()
    {
        List<EquippedApparel> equipList = PlayerDataManager.playerData.equipped;
        List<EquippedApparel> newEquips = ApparelManager.instance.ActiveViewPlayer.equippedApparel.Values.ToList();

        bool changedEquips = false;

        if (newEquips.Count != equipList.Count)
        {
            changedEquips = true;
        }
        else
        {
            int matchCount = 0;
            for (int i = 0; i < equipList.Count; i++)
            {
                for (int j = 0; j < newEquips.Count; j++)
                {
                    if (equipList[i].position != newEquips[j].position)
                        continue;

                    if (newEquips[j].id != equipList[i].id)
                        continue;

                    if (newEquips[j].assets.Count != equipList[j].assets.Count)
                        continue;

                    bool matchAssets = true;
                    for (int ii = 0; ii < equipList[i].assets.Count; ii++)
                    {
                        if (newEquips[j].assets.Contains(equipList[i].assets[ii]) == false)
                            matchAssets = false;
                    }

                    if (matchAssets == false)
                        continue;

                    matchCount++;
                }
            }

            if (matchCount != equipList.Count)
                changedEquips = true;
        }

        return changedEquips;
    }

    void ShowItems()
    {
        foreach (Transform item in container)
        {
            Destroy(item.gameObject);
        }
        buttonDict.Clear();
        foreach (var item in PlayerDataManager.playerData.inventory.cosmetics)
        {
            if (item.hidden)
                continue;

            var g = Utilities.InstantiateObject(ApparelButton, container);
            item.buttonData = g.GetComponent<ApparelButtonData>();
            item.buttonData.Setup(item);
            buttonDict.Add(item.id, item);
        }
        subTitle.text = "";
    }

    public void SetFilter(string id)
    {
        currentFilter = id;
        foreach (var item in buttonDict)
        {
            if (item.Value.catagory == id)
            {
                item.Value.buttonData.gameObject.SetActive(true);
            }
            else
            {
                item.Value.buttonData.gameObject.SetActive(false);
            }
        }
        subTitle.text = GetSubtitle(id);
        foreach (var item in highlights)
        {
            item.SetActive(false);
        }

        if (ringsUI.isOpen)
            ringsUI.Close();
    }

    public void ShowAll()
    {
        currentFilter = "none";
        foreach (var item in buttonDict)
        {
            item.Value.buttonData.gameObject.SetActive(true);

        }
        subTitle.text = "";
        foreach (var item in highlights)
        {
            item.SetActive(false);
        }

        if (ringsUI.isOpen)
            ringsUI.Close();
    }

    public void SetConflict(List<string> conflicts)
    {
        foreach (var button in buttonDict)
        {
            foreach (var item in conflicts)
            {
                if (button.Key == item)
                {
                    button.Value.buttonData.ConflictCG.alpha = .2f;
                }
                else
                {
                    button.Value.buttonData.ConflictCG.alpha = 1;
                }
            }
            //			button.Value.buttonData.Setup (button.Value);
        }
    }

    public void DisableOtherSelection(string id)
    {
        foreach (var item in buttonDict)
        {
            if (item.Key != id)
            {
                item.Value.buttonData.Selected.SetActive(false);
                item.Value.buttonData.closeButton.SetActive(false);
            }
        }
    }

    public void ClearConflicts()
    {
        foreach (var item in buttonDict)
        {
            item.Value.buttonData.ConflictCG.alpha = 1;
        }
    }

    string GetSubtitle(string id)
    {
        if (id == "head")
        {
			return LocalizeLookUp.GetText ("apparel_head");// "Head";
        }
        else if (id == "hair")
        {
			return LocalizeLookUp.GetText ("apparel_hair");// "Hair";
        }
        else if (id == "neck")
        {
			return LocalizeLookUp.GetText ("apparel_neck");// "Neck";
        }
        else if (id == "chest")
        {
			return LocalizeLookUp.GetText ("apparel_chest");// "Chest";
        }
        else if (id == "wrist")
        {
			return LocalizeLookUp.GetText ("apparel_wrist");// "Wrist";
        }
        else if (id == "hands")
        {
			return LocalizeLookUp.GetText ("apparel_hands");// "Hands";
        }
        else if (id == "legs")
        {
			return LocalizeLookUp.GetText ("apparel_legs");// "Legs";
        }
        else if (id == "feet")
        {
			return LocalizeLookUp.GetText ("apparel_feet");// "Feet";
        }
        else if (id == "carryOn")
        {
			return LocalizeLookUp.GetText ("apparel_carry");// "Carry On";
        }
        else if (id == "tattoo")
        {
			return LocalizeLookUp.GetText ("apparel_tattoo");// "Tattoo";
        }
        else
            return "";
    }

    public void OnClickRing()
    {
        SetFilter("ring");
        ringsUI.Show();
    }
}

