using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarDots : MonoBehaviour
{
    public SimpleObjectPool m_DotsPool;
    public ScrollRect m_ScrollRect;
    public int m_DotAmount;

    [Header("Selection Effect")]
    public Vector3 m_Scale = new Vector3(1.4f, 1.4f, 1.4f);
    public Color m_Color = Color.white;

    private int m_iLastValue = -1;
    private DotObject[] m_DotsList;

    private void Start()
    {
        Setup(m_DotAmount);
    }

    public void Setup(int iDotAmount)
    {
        m_DotsPool.Setup();
        m_DotsPool.DespawnAll();
        m_DotAmount = iDotAmount;
        m_DotsList = new DotObject[m_DotAmount];
        for (int i = 0; i < m_DotAmount; i++)
        {
            m_DotsList[i] = m_DotsPool.Spawn<DotObject>();
        }
    }

    private void Update()
    {
        float fPart = 1 / (float)m_DotAmount;
        float fVal = m_ScrollRect.horizontalScrollbar.value / fPart;
        int iIndex = Mathf.FloorToInt(fVal);
        iIndex = iIndex >= m_DotAmount ? m_DotAmount - 1 : iIndex;
        iIndex = iIndex <= 0 ? 0 : iIndex;

        if (iIndex == m_iLastValue)
            return;

        if (m_iLastValue != -1)
        {
            LeanTween.scale(m_DotsList[m_iLastValue].m_GORoot, Vector3.one, .1f);
            m_DotsList[m_iLastValue].m_GOImage.color = m_DotsList[iIndex].m_GOImage.color;
        }

        LeanTween.scale(m_DotsList[iIndex].m_GORoot, m_Scale, .1f);
        m_DotsList[iIndex].m_GOImage.color = m_Color;


        m_iLastValue = iIndex;
    }

}