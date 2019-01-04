using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIItemsScroll : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ScrollRect m_pScrollRect;
    [SerializeField] private LayoutGroup m_pItemContainer;
    [SerializeField] private UIWheelItem m_pItemPrefab;
    private int m_iSelectedIndex;
    [SerializeField] private float m_fWidth = 5;

    [Header("Anim")]
    [SerializeField] private LeanTweenType m_eItemScaleTweenType = LeanTweenType.easeOutExpo;
    [SerializeField] private LeanTweenType m_eItemFadeTweenType = LeanTweenType.easeOutExpo;
    [SerializeField] private float m_fItemFadeDuration = 1;
    [SerializeField] private float m_fItemScaleDuration = 1f;
    [SerializeField] private float m_fItemMinScale = 0.8f;
    [SerializeField] private float m_fItemMinAlpha = 0.5f;

    private int m_iFocusTweenId;

    public System.Action<int> OnChangeSelected;
    public List<UIWheelItem> Items { get; private set; }
    public int SelectedIndex
    {
        get
        {
            return m_iSelectedIndex;
        }
        private set
        {
            if (m_iSelectedIndex == value)
                return;

            m_iSelectedIndex = value;

                int iLeft = value - 1;
                int iRight = value + 1;

                AnimateItem(Items[SelectedIndex], 0f);
                for (int i = iLeft; i >= 0; i--)
                    AnimateItem(Items[i], (i - SelectedIndex)/m_fWidth);
                for (int i = iRight; i < Items.Count; i++)
                    AnimateItem(Items[i], (i - SelectedIndex) /m_fWidth);

            if (OnChangeSelected != null)
                OnChangeSelected.Invoke(value);
        }
    }


    private void AnimateItem(UIWheelItem item, float normalizedDistance)
    {
        float fValue = LeanTween.easeOutCubic(0, 1, 1 - Mathf.Abs(normalizedDistance));
        float fAlpha = Mathf.Clamp(fValue == 1 ? 1 : fValue - 0.3f, m_fItemMinAlpha, 1f);
        float fScale = Mathf.Clamp(fValue == 1 ? 1 : fValue - 0.1f, m_fItemMinScale, 1f);
        float fPivot = 1 - (normalizedDistance + 1f) / 2f;

        item.SetPivot(fPivot);
        item.FadeContent(fAlpha, m_fItemFadeDuration, 0, m_eItemFadeTweenType);
        item.ScaleContent(fScale, m_fItemScaleDuration, 0, m_eItemScaleTweenType);
    }

    public virtual void OnBeginDrag(PointerEventData data)
    {
        LeanTween.cancel(m_iFocusTweenId);
    }

    public void OnDrag(PointerEventData data)
    {
        UpdateSelected();
    }

    public void OnEndDrag(PointerEventData data)
    {
        UpdateSelected();
        FocusOnSelected();
    }


    private void Awake()
    {
        m_pItemPrefab.gameObject.SetActive(false);
    }
    
    public void SetSelected(int index)
    {
        SelectedIndex = index;        
        FocusOnSelected();
    }

    public void Load(int count)
    {
        if (Items != null)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].gameObject.SetActive(false);
            }
        }
        Items = new List<UIWheelItem>();

        for (int i = 0; i < count; i++)
        {
            UIWheelItem pItem;
            if (i < m_pItemContainer.transform.childCount)
            {
                pItem = m_pItemContainer.transform.GetChild(i).GetComponent<UIWheelItem>();
            }
            else
            {
                pItem = Instantiate(m_pItemPrefab, m_pItemContainer.transform);
                pItem.name = "Item (" + i + ")";
            }
            
            pItem.gameObject.SetActive(true);

            int iAux = Items.Count;
            pItem.onClick = () => OnClickItem(iAux);

            Items.Add(pItem);
        }

        m_iSelectedIndex = -1;
        UpdateSelected();
        FocusOnSelected();
    }

    private void UpdateSelected()
    {
        int iIndex = Mathf.RoundToInt(m_pScrollRect.horizontalNormalizedPosition * (Items.Count - 1));

        if (iIndex != SelectedIndex)
            SelectedIndex = iIndex;
    }

    private void FocusOnSelected()
    {
        float fStartValue = m_pScrollRect.horizontalNormalizedPosition;
        float fTargetValue = SelectedIndex / (Items.Count - 1f);

        m_iFocusTweenId = LeanTween.value(fStartValue, fTargetValue, 0.5f)
            .setEaseOutSine()
            .setOnUpdate((float value) =>
            {
                m_pScrollRect.horizontalNormalizedPosition = value;
            })
            .uniqueId;
    }

    private void OnClickItem(int index)
    {
        SetSelected(index);
    }

#if UNITY_EDITOR

    [Header("Debug")]
    [SerializeField] private float m_fNormalizedScrollPos;

    private void Update()
    {
        m_fNormalizedScrollPos = m_pScrollRect.horizontalNormalizedPosition;
    }

    [ContextMenu("Init 5")]
    private void Debug_AddItem5()
    {
        Load(5);
    }

    [ContextMenu("Init 10")]
    private void Debug_AddItem10()
    {
        Load(10);
    }
#endif
}
