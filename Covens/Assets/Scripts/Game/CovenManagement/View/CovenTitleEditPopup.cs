using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CovenTitleEditPopup : UIBaseAnimated
{
    public SimpleObjectPool m_SimpleObjectPool;
    public Vector2 m_PositionOffset;
    public Vector2 m_FixedPosition;

    private void Awake()
    {
        m_SimpleObjectPool.Setup();
    }


    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(CovenController.CovenTitle eCurrentTitle, RectTransform pRectPosition)
    {
        m_SimpleObjectPool.DespawnAll();
        List<CovenController.CovenTitle> vAllowedTitles = CovenController.GetAllowedTitles(eCurrentTitle);
        foreach(CovenController.CovenTitle eTitle in vAllowedTitles)
        {
            CovenTitleEditItem pGo = m_SimpleObjectPool.Spawn<CovenTitleEditItem>();
            if(pGo != null)
            {
                pGo.Show(eTitle, eCurrentTitle == eTitle);
            }
        }

        //Vector3 vPos = TargetTransform.position;
        //vPos.x = pRectPosition.position.x + m_PositionOffset.x;
        //TargetTransform.position = vPos;
        TargetTransform.position = m_FixedPosition;
        //TargetTransform.position = pRectPosition.position + (Vector3)m_PositionOffset;

        base.Show();
    }


    public void OnClickSelect(CovenTitleEditItem pItem)
    {
        Debug.Log("Will change the Title to: " + pItem.m_eTitle);
        Close();
    }



    [ContextMenu("Fix This Position")]
    public void FixPosition()
    {
        m_FixedPosition = m_Target.transform.position;
    }
}