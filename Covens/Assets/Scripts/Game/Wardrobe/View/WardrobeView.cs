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
    public Text subtitle;

    [Header("Character")]
    public CharacterView m_Character;
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
    public GameObject m_ChooseColorRoot;
    public WardrobeItemButton m_WhiteButton;
    public WardrobeItemButton m_GreyButton;
    public WardrobeItemButton m_ShadowButton;




    private WardobeFilterButton m_pCurrentFilterButton;

    protected WardrobeController Controller
    {
        get { return WardrobeController.Instance; }
    }


    public override void DoShowAnimation()
    {
        base.DoShowAnimation();
        anim.SetBool("animate", true);
        Invoke("OnShowFinish", 1f);
    }
    public override void DoCloseAnimation()
    {
        base.DoCloseAnimation();
        anim.SetBool("animate", false);
        Invoke("OnCloseFinish", 1f);
    }

    private void Start()
    {
        m_ItemPool.Setup();
    }


    public override void Show()
    {
        base.Show();
        m_ItemPool.DespawnAll();
        m_FilterHightlight.SetActive(false);
        m_ChooseColorRoot.SetActive(false);
    }

    public override void OnShowFinish()
    {
        base.OnShowFinish();
        /*if (m_Character == null)
        {
            GameObject pPrefab = PlayerDataManager.Instance.Gender == EnumGender.Male ? m_CharacterMalePrefab : m_CharacterFemalePrefab;
            GameObject pGO = GameObject.Instantiate(pPrefab, m_CharacterViewRoot.transform);
            m_Character = pGO.GetComponent<CharacterView>();
        }*/


        SetupItens(Controller.AvailableItens);
        m_Character.SetupChar();
    }

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
    public void SetupItens(List<WardrobeItemModel> vItens)
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
            pItemButton.Setup(vGrouped[i], this, m_NotFoundPreview);
            pItemButton.OnClickEvent += ItemData_OnClickEvent;
            m_WardrobeItemButtonCache.Add(pItemButton);

            // animate
            pItemButton.transform.localScale = Vector3.zero;
            LeanTween.cancel(pItemButton.gameObject);
            LeanTween.scale(pItemButton.gameObject, Vector3.one, m_ItemAnimTime).setDelay(i * m_ItemAnimDelayTime);//.setEase(LeanTweenType.easeOutBack);
        }

        foreach (var pItem in m_Character.m_Controller.EquippedItems)
        {
            WardrobeItemButton pButton = GetButtonItem(pItem);
            if (pButton == null)
                continue;
            pButton.SetEquipped(true);
        }
    }


    private void ItemData_OnClickEvent(WardrobeItemButton obj)
    {
        if (obj.HasGroups)
        {
            m_ChooseColorRoot.SetActive(false);
            m_WhiteButton.gameObject.SetActive(false);
            m_GreyButton.gameObject.SetActive(false);
            m_ShadowButton.gameObject.SetActive(false);
            m_WardrobeItemButtonCache.Remove(m_WhiteButton);
            m_WardrobeItemButtonCache.Remove(m_GreyButton);
            m_WardrobeItemButtonCache.Remove(m_ShadowButton);
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
        int i = 0;
        foreach (var pItem in obj.WardrobeGroupedItemModel.m_Items)
        {
            WardrobeItemButton pButton = null;
            switch (pItem.AlignmentEnum)
            {
                case EnumAlignment.White: pButton = m_WhiteButton; break;
                case EnumAlignment.Gray: pButton = m_GreyButton; break;
                case EnumAlignment.Shadow: pButton = m_ShadowButton; break;
            }
            if (pButton != null)
            {
                pButton.Setup(pItem, this, m_NotFoundPreview);
                pButton.OnClickEvent += ItemData_OnClickEvent;
                m_WardrobeItemButtonCache.Add(pButton);
                ActivateButton(pButton, i++);
                m_ChooseColorRoot.SetActive(true);
                if (m_Character.IsEquipped(pItem))
                {
                    pButton.SetEquipped(true);
                }
            }
        }
        obj.SetEquipped(true);
    }
    void OnClickItem(WardrobeItemButton obj)
    {
        if (obj.IsEquipped)
        {
            m_Character.Unequip(obj.WardrobeItemModel);
            WardrobeItemButton pButton = GetButtonItem(obj.WardrobeItemModel);
            if (pButton != null)
                pButton.SetEquipped(false);
        }
        else
        {
            WardrobeItemModel pReplacedItem = m_Character.Equip(obj.WardrobeItemModel);
            if (pReplacedItem != null)
            {
                WardrobeItemButton pButton = GetButtonItem(pReplacedItem);
                if (pButton != null)
                    pButton.SetEquipped(false);
            }
            if (m_Character.IsEquippedNotColored(obj.WardrobeItemModel))
            {
                obj.SetEquipped(true);
            }
        }
    }

    private void ActivateButton(WardrobeItemButton pItemButton, int iIdx)
    {
        GameObject pGO = pItemButton.gameObject;
        pGO.SetActive(true);
        pItemButton.transform.localScale = Vector3.zero;
        LeanTween.cancel(pGO);
        LeanTween.scale(pGO, Vector3.one, m_ItemAnimTime).setDelay(iIdx * m_ItemAnimDelayTime);//.setEase(LeanTweenType.easeOutBack);
    }
    public WardrobeItemButton GetButtonItem(WardrobeItemModel pItem)
    {
        for(int i =0; i < m_WardrobeItemButtonCache.Count; i++)
        {
            if (m_WardrobeItemButtonCache[i].WardrobeItemModel.ID == pItem.ID)
                return m_WardrobeItemButtonCache[i];
        }
        return null;
    }


    #region button callback

    public void OnClickBG()
    {
        OnClickCloseChooseColor();
    }
    public void OnClickRandomize()
    {
        //m_Character.RandomItens(WardrobeController.Instance.AvailableItens);
    }
    public bool OnClickEquip(WardrobeItemModel pItem)
    {
        m_Character.SetItem(pItem);
        return m_Character.IsEquippedNotColored(pItem);
    }
    public void OnClickFilter(EnumWardrobeCategory eCat, Transform pFilterGO)
    {
        SetupItens(Controller.GetAvailableItens(eCat));
        HighlightTo(pFilterGO);
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
    }
    public void OnClickFilter(EnumEquipmentSlot eSlot, Transform pFilterGO)
    {
        SetupItens(Controller.GetAvailableItens(eSlot));
        HighlightTo(pFilterGO);
    }
    public void OnClickRemoveFilter()
    {
        SetupItens(Controller.AvailableItens);
        HighlightTo(null);
    }
    public void OnClickCloseChooseColor()
    {
        m_ChooseColorRoot.SetActive(false);
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