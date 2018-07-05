using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// a scrollbar that uses dots instead of a bar.
/// It requires the DotObject as a object template to use with SimpleObjectPool
/// </summary>
public class ScrollbarDots : MonoBehaviour
{
    public SimpleObjectPool m_DotsPool;
    public ScrollRect m_ScrollRect;
    public int m_DotAmount;

    [Header("Selection Effect")]
    public Vector3 m_Scale = new Vector3(1.4f, 1.4f, 1.4f);
    public Color m_Color = Color.white;

    [Header("Focus")]
    public bool m_FocusWhenStopDrag = false;
    public ScrollbarInputListener m_ScrollbarInputListener;
    public float m_FocusWhenLessThan = 0.05f;


    // internal vars
    private int m_iLastIndex = -1;
    private DotObject[] m_DotsList;
    // drag/focus
    private float m_fLastValue = -1;
    private bool m_bFocusAnimation = false;

    public event Action<int> OnIndexChangedEvent;


    #region gets

    Scrollbar Scroll
    {
        get { return m_ScrollRect.horizontalScrollbar; }
    }

    float Value
    {
        get { return Scroll.value; }
        set { Scroll.value = value; }
    }

    int PageAmount
    {
        get
        {
            return m_DotAmount;
        }
    }
    float PageArea
    {
        get
        {
            return (1 / ((float)PageAmount - 1));
        }
    }
    public int Index
    {
        get { return m_iLastIndex; }
        private set {
            m_iLastIndex = value;
            if (OnIndexChangedEvent != null)
                OnIndexChangedEvent(value);
        }
    }
    public bool IsLastPage
    {
        get
        {
            return Index >= PageAmount -1;
        }
    }
    public bool IsFirstPage
    {
        get
        {
            return Index <= 0;
        }
    }
    public DotObject CurrentDot
    {
        get
        {
            return m_DotsList[Index];
        }
    }
    #endregion


    private void Start()
    {
        Setup(m_DotAmount);
        // preparing focusable
        if (m_FocusWhenStopDrag && m_ScrollbarInputListener != null)
        {
            m_ScrollbarInputListener.OnEndDragEvent += ScrollbarInputListener_OnEndDragEvent;
            m_ScrollbarInputListener.OnBeginDragEvent += ScrollbarInputListener_OnBeginDragEvent;
        }
        // listen to value change
        Scroll.onValueChanged.AddListener(OnValueChanged);
    }

    public void Setup(int iDotAmount)
    {
        m_bFocusAnimation = false;
        m_fLastValue = -1;
        m_iLastIndex = -1;
        m_DotsPool.Setup();
        m_DotsPool.DespawnAll();
        m_DotAmount = iDotAmount;
        m_DotsList = new DotObject[m_DotAmount];
        for (int i = 0; i < m_DotAmount; i++)
        {
            m_DotsList[i] = m_DotsPool.Spawn<DotObject>();
            m_DotsList[i].m_Index = i;
        }
    }

    void OnValueChanged(float f)
    {
        UpdateDots(Value);
        UpdateFocus(Value);
        m_fLastValue = Value;
    }



    #region update dots view

    /// <summary>
    /// updates the dots view by changing its selected index
    /// </summary>
    /// <param name="fValue"></param>
    void UpdateDots(float fValue)
    {
        float fPart = 1 / (float)m_DotAmount;
        float fVal = Value / fPart;
        int iIndex = Mathf.FloorToInt(fVal);
        iIndex = iIndex >= m_DotAmount ? m_DotAmount - 1 : iIndex;
        iIndex = iIndex <= 0 ? 0 : iIndex;
        if (iIndex == Index)
            return;

        if (Index != -1)
        {
            LeanTween.scale(m_DotsList[Index].m_GORoot, Vector3.one, .1f);
            m_DotsList[Index].m_GOImage.color = m_DotsList[iIndex].m_GOImage.color;
        }
        LeanTween.scale(m_DotsList[iIndex].m_GORoot, m_Scale, .1f);
        m_DotsList[iIndex].m_GOImage.color = m_Color;
        Index = iIndex;
    }

    #endregion



    #region Focus codes

    private void ScrollbarInputListener_OnEndDragEvent()
    {
        
    }
    private void ScrollbarInputListener_OnBeginDragEvent()
    {
        CancelFocusAnimation();
    }
    void UpdateFocus(float fValue)
    {
        if (!m_FocusWhenStopDrag || m_bFocusAnimation)
            return;

        if (!m_ScrollbarInputListener.IsDragging)
        {
            float fDiff = Mathf.Abs(m_fLastValue - Value);
            if (fDiff < m_FocusWhenLessThan)
            {
                FocusToIndex(Index);
            }
        }
    }
    void FocusToIndex(int iIndex)
    {
        if (m_bFocusAnimation)
        {
            //m_bFocusing = false;
            return;
        }
        m_bFocusAnimation = true;
        //Value = iIndex * PageArea;
        float fValue = iIndex * PageArea;
        MoveToValue(fValue);
        //LeanTween.value(Value, fValue, .1f).setOnUpdate(OnUpdateValue);
    }
    private void MoveToValue(float fValue)
    {
        CancelFocusAnimation();
        m_bFocusAnimation = true;
        LeanTween.value(Value, fValue, .1f).setOnUpdate(OnUpdateValue).setOnComplete(OnUpdateComplete);
    }
    void OnUpdateValue(float fValue)
    {
        Value = fValue;
    }
    void OnUpdateComplete()
    {
        m_bFocusAnimation = false;
    }
    private void CancelFocusAnimation()
    {
        if (m_bFocusAnimation)
        {
            LeanTween.cancel(Scroll.gameObject);
        }
        m_bFocusAnimation = false;
    }


    #endregion


    public void PunchObject(DotObject pObj)
    {
        LeanTween.cancel(CurrentDot.gameObject);
        CurrentDot.transform.localScale = m_Scale;
        LeanTween.scale(CurrentDot.gameObject, m_Scale + new Vector3(.3f, .3f, .3f), .4f).setEase(LeanTweenType.punch);
    }

    public void OnClickNextPage()
    {
        if (IsLastPage)
        {
            PunchObject(CurrentDot);
            return;
        }
        FocusToIndex(Index + 1);
    }
    public void OnClickPreviousPage()
    {
        if (IsFirstPage)
        {
            PunchObject(CurrentDot);
            return;
        }
        FocusToIndex(Index - 1);
    }







    /*
     * when reach limits, it shakes. maybe later we can improve it
    private float m_fLastSize = -1;
    void UpdateReachLimit()
    {
        if (m_ScrollRect.horizontalScrollbar.value == 0 || m_ScrollRect.horizontalScrollbar.value == 1)
        {
            if(m_fLastSize != m_ScrollRect.horizontalScrollbar.size)
            {
                float fRand = UnityEngine.Random.Range(m_Scale.x - .1f, m_Scale.x + .1f);
                Vector3 vNew = new Vector3(fRand, fRand, fRand);
                m_DotsList[m_iLastValue].m_GORoot.transform.localScale = vNew;
            }
        }
        else
        {
            m_fLastSize = m_ScrollRect.horizontalScrollbar.size;
        }
    }*/

    public void OnClickDot(DotObject pObj)
    {
        FocusToIndex(pObj.m_Index);
    }

}