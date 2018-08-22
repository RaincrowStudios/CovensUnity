using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WardrobeView : UIBase
{
    public class GroupedWardrobeItemModel
    {
        public List<WardrobeItemModel> m_Items;
        public WardrobeItemModel First { get { return m_Items[0]; } }
        public bool HasAlignment(EnumAlignment eAlignment)
        {
            foreach (var pItem in m_Items)
                if (pItem.AlignmentEnum == eAlignment)
                    return true;
            return false;
        }
    }


    public Animator anim;
    public Text m_txtSubtitle;

    [Header("Character")]
    public GameObject m_FilterHightlight;
    public GameObject m_CharacterViewRoot;
    public GameObject m_CharacterMalePrefab;
    public GameObject m_CharacterFemalePrefab;

    [Header("Item Buttons")]
    public float m_ItemAnimTime = .6f;
    public float m_ItemAnimDelayTime = .05f;
    public GameObject m_Container;
    public SimpleObjectPool m_ItemPool;
    public List<WardrobeItemButton> m_WardrobeItemButtonCache;
    public Sprite m_NotFoundPreview;

    [Header("Item Buttons")]
    public WardrobeChooseColor m_ChooseColor;


    [Header("Consumables")]
    public WardrobeConsumeButton m_ConsumeEnergy;
    public WardrobeConsumeButton m_ConsumeWisdom;
    public WardrobeConsumeButton m_ConsumeAptitude;


    [Header("Tests")]
    public bool m_TestEnabled = false;
    public EnumGender m_EnumGenderTest;
    public bool m_GetAllItemTest;





    private WardobeFilterButton m_pCurrentFilterButton;
    private CharacterView m_Character;




    #region gets

    EnumGender Gender
    {
        get
        {
            // tester
            if(m_TestEnabled)
                return m_EnumGenderTest;
            return PlayerDataManager.Instance.Gender;
        }
    }
    protected WardrobeController Controller
    {
        get { return WardrobeController.Instance; }
    }
    protected CharacterView CharacterView
    {
        get { return m_Character; }
    }
    protected CharacterControllers CharacterController
    {
        get { return m_Character.m_Controller; }
    }
    public List<WardrobeItemModel> AvailableItemList
    {
        get
        {
            return Controller.GetAvailableItens(Gender, m_GetAllItemTest);
        }
    }

    #endregion



    #region show override

    private void Start()
    {
        m_ItemPool.Setup();
        m_ConsumeEnergy.OnClickEvent += ConsumeEnergy_OnClickEvent;
        m_ConsumeWisdom.OnClickEvent += ConsumeEnergy_OnClickEvent;
        m_ConsumeAptitude.OnClickEvent += ConsumeEnergy_OnClickEvent;

    }


    public override void Show()
    {
        base.Show();

        // setup character 
        GameObject pGOActive = Gender == EnumGender.Female ? m_CharacterFemalePrefab : m_CharacterMalePrefab;
        GameObject pGODisabled = Gender != EnumGender.Female ? m_CharacterFemalePrefab : m_CharacterMalePrefab;
        m_Character = pGOActive.GetComponent<CharacterView>();
        m_ChooseColor.CharacterView = CharacterView;
        pGOActive.SetActive(true);
        pGODisabled.SetActive(false);

        // reset attrs
        m_ItemPool.DespawnAll();
        m_FilterHightlight.SetActive(false);
        m_ChooseColor.Hide();
        m_txtSubtitle.text = "";

        // setup
        SetupItens(AvailableItemList, false);
        SetupConsumables();
        CharacterView.SetupChar();
    }
    public override void DoShowAnimation()
    {
        base.DoShowAnimation();
        anim.SetBool("animate", true);
        Invoke("OnShowFinish", 1f);
    }
    public override void OnShowFinish()
    {
        base.OnShowFinish();
    }
    public override void Close()
    {
        base.Close();
        CharacterController.SynchServer(null, null);
    }
    public override void DoCloseAnimation()
    {
        base.DoCloseAnimation();
        anim.SetBool("animate", false);
        Invoke("OnCloseFinish", 1f);
    }
    #endregion


    #region complex gets

    bool HasItem(List<GroupedWardrobeItemModel> vGroupedItens, WardrobeItemModel pItem)
    {
        for (int i = 0; i < vGroupedItens.Count; i++)
        {
            for (int j = 0; j < vGroupedItens[i].m_Items.Count; j++)
            {
                if (vGroupedItens[i].m_Items[j].ID == pItem.ID)
                    return true;
            }
        }
        return false;
    }
    public List<GroupedWardrobeItemModel> GetGroupedItem(List<WardrobeItemModel> vItems)
    {
        string sDebug = "";
        List<GroupedWardrobeItemModel> vGroupedItens = new List<GroupedWardrobeItemModel>();
        for(int i = 0; i < vItems.Count; i++)
        {
            if (HasItem(vGroupedItens, vItems[i]))
                continue;
            GroupedWardrobeItemModel pGroupedItem = new GroupedWardrobeItemModel();
            pGroupedItem.m_Items = new List<WardrobeItemModel>();
            pGroupedItem.m_Items.Add(vItems[i]);
            List<WardrobeItemModel> vItemsGroup = new List<WardrobeItemModel>();
            string sName = vItems[i].IDNotColored;
            sDebug += "\n  => " + sName + ":";
            sDebug += "\n    -> " + vItems[i].ID + ":";
            for (int j = 0; j < vItems.Count; j++)
            {
                if(i != j && vItems[i].IDNotColored == vItems[j].IDNotColored)
                {
                    pGroupedItem.m_Items.Add(vItems[j]);
                    sDebug += "\n    -> " + vItems[j].ID + ":";
                }
            }
            vGroupedItens.Add(pGroupedItem);
        }
        Debug.Log(sDebug);
        return vGroupedItens;
    }

    public WardrobeItemButton GetButtonItem(WardrobeItemModel pItem)
    {
        for (int i = 0; i < m_WardrobeItemButtonCache.Count; i++)
        {
            if (m_WardrobeItemButtonCache[i].WardrobeItemModel.IsEqual(pItem))
                return m_WardrobeItemButtonCache[i];
        }
        return null;
    }
    public WardrobeItemButton GetButtonItemNotColor(WardrobeItemModel pItem)
    {
        for (int i = 0; i < m_WardrobeItemButtonCache.Count; i++)
        {
            if (m_WardrobeItemButtonCache[i].WardrobeItemModel.IsEqualNotColor(pItem))
                return m_WardrobeItemButtonCache[i];
        }
        return null;
    }

    #endregion


    public void SetupConsumables()
    {
        m_ConsumeEnergy.Setup(CharacterController.GetAvailableConsumable(EnumConsumable.Energy));
        m_ConsumeWisdom.Setup(CharacterController.GetAvailableConsumable(EnumConsumable.Wisdom));
        m_ConsumeAptitude.Setup(CharacterController.GetAvailableConsumable(EnumConsumable.Aptitude));
    }

    public void SetupItens(List<WardrobeItemModel> vItens, bool bAnimate = true)
    {
        m_ItemPool.DespawnAll();
        List<GroupedWardrobeItemModel> vGrouped = GetGroupedItem(vItens);
        m_WardrobeItemButtonCache = new List<WardrobeItemButton>();
        for (int i = 0; i < vGrouped.Count; i++)
        {
            // do not allow user to change its body
            if (vGrouped[i].First.EquipmentSlotEnum == EnumEquipmentSlot.BaseBody || vGrouped[i].First.EquipmentSlotEnum == EnumEquipmentSlot.BaseHand)
                continue;

            WardrobeItemButton pItemButton = m_ItemPool.Spawn<WardrobeItemButton>();
            pItemButton.SetupGroup(vGrouped[i]);
            pItemButton.OnClickEvent += ItemData_OnClickEvent;
            m_WardrobeItemButtonCache.Add(pItemButton);

            // animate
            if (bAnimate)
            {
                pItemButton.transform.localScale = Vector3.zero;
                LeanTween.cancel(pItemButton.gameObject);
                LeanTween.scale(pItemButton.gameObject, Vector3.one, m_ItemAnimTime).setDelay(i * m_ItemAnimDelayTime);
            }
        }

        UpdateEquippedItem();
    }

    void UpdateEquippedItem()
    {
        for (int i = 0; i < m_WardrobeItemButtonCache.Count; i++)
        {
            if (m_WardrobeItemButtonCache[i] && m_WardrobeItemButtonCache[i].WardrobeItemModel != null)
            {
                m_WardrobeItemButtonCache[i].SetEquipped(
                    CharacterController.IsEquippedNotColored(m_WardrobeItemButtonCache[i].WardrobeItemModel)
                    );
            }

            // check conflicts
            m_WardrobeItemButtonCache[i].SetConflicts(CharacterController.Conflicts(m_WardrobeItemButtonCache[i].WardrobeItemModel));
        }
    }


    private void ActivateButton(WardrobeItemButton pItemButton, int iIdx)
    {
        GameObject pGO = pItemButton.gameObject;
        pGO.SetActive(true);
        pItemButton.transform.localScale = Vector3.zero;
        LeanTween.cancel(pGO);
        LeanTween.scale(pGO, Vector3.one, m_ItemAnimTime).setDelay(iIdx * m_ItemAnimDelayTime);
    }



    #region button callback

    public void OnClickBG()
    {
        m_ChooseColor.Close();
        //OnClickCloseChooseColor();
    }
    public void OnClickRandomize()
    {
        //m_Character.RandomItens(WardrobeController.Instance.AvailableItens);
    }
    public bool OnClickEquip(WardrobeItemModel pItem)
    {
        CharacterView.Equip(pItem);
        return CharacterView.IsEquippedNotColored(pItem);
    }

    public void OnClickFilterButton(WardobeFilterButton pButton)
    {
        if (pButton.m_Slot == EnumEquipmentSlot.None)
            OnClickFilter(pButton.m_Category, pButton.transform);
        else
            OnClickFilter(pButton.m_Slot, pButton.transform);

        if (m_pCurrentFilterButton != null)
            m_pCurrentFilterButton.SetSelected(false);
        m_pCurrentFilterButton = pButton;
        m_pCurrentFilterButton.SetSelected(true);
        m_ChooseColor.Close();
        m_txtSubtitle.text = pButton.Subtitle;
    }

    public void OnClickFilter(EnumWardrobeCategory eCat, Transform pFilterGO)
    {
        SetupItens(Controller.GetAvailableItens(eCat, Gender));
        HighlightTo(pFilterGO);
        m_ChooseColor.Close();
    }
    public void OnClickFilter(EnumEquipmentSlot eSlot, Transform pFilterGO)
    {
        SetupItens(Controller.GetAvailableItens(eSlot, Gender));
        HighlightTo(pFilterGO);
        m_ChooseColor.Close();
    }
    public void OnClickRemoveFilter()
    {
        SetupItens(AvailableItemList);
        HighlightTo(null);
        m_ChooseColor.Close();
        m_txtSubtitle.text = "";
    }

    private void ItemData_OnClickEvent(WardrobeItemButton obj)
    {
        if (obj.HasGroups)
        {
            m_ChooseColor.Close();
        }

        if (obj.IsGrouped)
        {
            OnClickGroupedItem(obj);
        }
        else
        {
            OnClickItem(obj);
        }
    }
    void OnClickGroupedItem(WardrobeItemButton obj)
    {
        m_ChooseColor.Show(obj.WardrobeGroupedItemModel);
        m_ChooseColor.OnClickEvent += OnClickItemColored;
        UpdateEquippedItem();
        obj.SetEquipped(true);
    }
    void OnClickItemColored(WardrobeItemButton obj)
    {
        OnClickItem(obj);
    }
    void OnClickItem(WardrobeItemButton obj)
    {
        bool bIsEquiped = CharacterView.IsEquippedNotColored(obj.WardrobeItemModel);
        if (bIsEquiped)
        {
            CharacterView.Unequip(obj.WardrobeItemModel);
        }
        else
        {
            CharacterView.Equip(obj.WardrobeItemModel);
        }

        // make sure the choices are closed
        m_ChooseColor.Close();

        UpdateEquippedItem();
    }

    void GoToStore()
    {
        Debug.LogError("TODO: show shop UI");
    }
    private void ConsumeEnergy_OnClickEvent(WardrobeConsumeButton obj)
    {
        if (obj.IsLoading)
            return;
        string sID = obj.m_Model.ID;

        // has no item to consume
        if (obj.m_Model.Count <= 0)
        {
            UIGenericPopup.Show(
                "",
                Oktagon.Localization.Lokaki.GetText("Wardrobe_ConsumeErrorTitle"),
                Oktagon.Localization.Lokaki.GetText("General_Ok"),
                Oktagon.Localization.Lokaki.GetText("Wardrobe_ConsumeErrorNo"), "",
                GoToStore, null, null
                );
            return;
        }

        // preparing to consume
        Action<string> Success = (string s) =>
        {
            obj.Consume(1);
            obj.SetLoading(false);
            string sItemName = Oktagon.Localization.Lokaki.GetText("Consumable_" + obj.m_Model.ConsumableType);
            string sConsumeNotification = Oktagon.Localization.Lokaki.GetText("Consumable_" + obj.m_Model.ConsumableType);
            sConsumeNotification = sConsumeNotification.Replace("<item>", sItemName);
            //<Item> Was consumed with success!
//            PlayerNotificationManager.Instance.showNotification(
//                null, false,
//                sConsumeNotification,
//                SpriteResources.GetSprite("Icon-" + obj.m_Model.ConsumableType));
        };
        Action<string> Fail = (string s) =>
        {
            obj.SetLoading(false);
        };


        obj.SetLoading(true);
        CharacterController.Consume(sID, 1, Success, Fail);
    }

    #endregion



    void HighlightTo(Transform pFilterGO)
    {
        if (pFilterGO != null)
        {
            m_FilterHightlight.SetActive(true);

            // animate
            Transform pTrans = m_FilterHightlight.transform;
            pTrans.LookAt(pFilterGO);
            pTrans.localScale = Vector3.zero;
            LeanTween.cancel(m_FilterHightlight);
            LeanTween.scale(m_FilterHightlight, Vector3.one, .3f);
        }
        else
        {
            if (m_pCurrentFilterButton != null)
                m_pCurrentFilterButton.SetSelected(false);
            m_FilterHightlight.SetActive(false);
        }
    }


    #region editor
#if UNITY_EDITOR
    [ContextMenu("Add Male Character")]
    public void AddMaleChar()
    {
        RemoveChar();
        GameObject pGO = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(m_CharacterMalePrefab);
        pGO.transform.parent = m_CharacterViewRoot.transform;
        //GameObject pGO = GameObject.Instantiate(m_CharacterMalePrefab, m_CharacterViewRoot.transform);
        m_Character = pGO.GetComponent<CharacterView>();
    }
    [ContextMenu("Add Female Character")]
    public void AddFemaleChar()
    {
        RemoveChar();
        GameObject pGO = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(m_CharacterFemalePrefab);
        pGO.transform.parent = m_CharacterViewRoot.transform;
        //GameObject pGO = GameObject.Instantiate(m_CharacterFemalePrefab, m_CharacterViewRoot.transform);
        m_Character = pGO.GetComponent<CharacterView>();
    }
    [ContextMenu("Remove Character")]
    public void RemoveChar()
    {
        CharacterView[] vChars = m_CharacterViewRoot.transform.GetComponentsInChildren<CharacterView>();
        for (int i = vChars.Length - 1; i >= 0; i--)
            DestroyImmediate(vChars[i].gameObject);
    }
#endif
#endregion
}