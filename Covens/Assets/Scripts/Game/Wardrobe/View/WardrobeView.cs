using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WardrobeView : UIBase
{
    public Animator anim;
    public Text subtitle;

    [Header("Character")]
    public CharacterView m_Character;
    public GameObject m_FilterHightlight;

    [Header("Item Buttons")]
    public float m_ItemAnimTime = .6f;
    public float m_ItemAnimDelayTime = .05f;
    public GameObject m_Container;
    public SimpleObjectPool m_ItemPool;
    public List<WardrobeItemButton> m_WardrobeItemButtonCache;
    public Sprite m_NotFoundPreview;


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
    }

    public override void OnShowFinish()
    {
        base.OnShowFinish();
        SetupItens(Controller.AvailableItens);


        m_Character.SetupChar();
    }



    public void SetupItens(List<WardrobeItemModel> vItens)
    {
        m_ItemPool.DespawnAll();
        m_WardrobeItemButtonCache = new List<WardrobeItemButton>();
        for (int i = 0; i < vItens.Count; i++)
        {
            // do not allow user to change its body
            if (vItens[i].EquipmentSlotEnum == EnumEquipmentSlot.BaseBody || vItens[i].EquipmentSlotEnum == EnumEquipmentSlot.BaseHand)
                continue;

            WardrobeItemButton pItemData = m_ItemPool.Spawn<WardrobeItemButton>();
            pItemData.Setup(vItens[i], this, m_NotFoundPreview);
            pItemData.OnClickEvent += ItemData_OnClickEvent;
            m_WardrobeItemButtonCache.Add(pItemData);

            // animate
            pItemData.transform.localScale = Vector3.zero;
            LeanTween.cancel(pItemData.gameObject);
            LeanTween.scale(pItemData.gameObject, Vector3.one, m_ItemAnimTime).setDelay(i * m_ItemAnimDelayTime);//.setEase(LeanTweenType.easeOutBack);
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
            if (m_Character.IsEquipped(obj.WardrobeItemModel))
            {
                obj.SetEquipped(true);
            }
            if (pReplacedItem != null)
            {
                WardrobeItemButton pButton = GetButtonItem(pReplacedItem);
                if (pButton != null)
                    pButton.SetEquipped(false);
            }
        }
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

    public void OnClickRandomize()
    {
        //m_Character.RandomItens(WardrobeController.Instance.AvailableItens);
    }
    public bool OnClickEquip(WardrobeItemModel pItem)
    {
        m_Character.SetItem(pItem);
        return m_Character.IsEquipped(pItem);
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
}