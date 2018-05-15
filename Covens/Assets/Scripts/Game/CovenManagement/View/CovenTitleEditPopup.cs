using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CovenTitleEditPopup : UIBaseAnimated
{
    
    public InputField m_NewUserTitle;
    public Text m_CurrentUserTitle;

    private Action<bool, string> m_pOnCloseCallback;

    private void Awake()
    {
    }


    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(string sCurrentTitle, Action<bool, string> pOnClose)
    {
        m_CurrentUserTitle.text = sCurrentTitle;
        m_NewUserTitle.text = "";
        m_pOnCloseCallback = pOnClose;
        base.Show();
    }

    public void OnClickConfirm()
    {
        if (m_pOnCloseCallback != null)
            m_pOnCloseCallback(true, m_NewUserTitle.text);
        Close();
    }
    public void OnClickCancel()
    {
        if (m_pOnCloseCallback != null)
            m_pOnCloseCallback(false, m_NewUserTitle.text);
        Close();
    }

    /*
    public void Show(CovenController.CovenRole eCurrentTitle, RectTransform pRectPosition)
    {
        m_SimpleObjectPool.DespawnAll();
        List<CovenController.CovenRole> vAllowedTitles = CovenController.GetAllowedTitles(eCurrentTitle);
        foreach(CovenController.CovenRole eTitle in vAllowedTitles)
        {
            CovenTitleEditItem pGo = m_SimpleObjectPool.Spawn<CovenTitleEditItem>();
            if(pGo != null)
            {
                pGo.Show(eTitle, eCurrentTitle == eTitle);
            }
        }

        TargetTransform.position = m_FixedPosition;
        //TargetTransform.position = pRectPosition.position + (Vector3)m_PositionOffset;

        base.Show();
    }


    public void OnClickSelect(CovenTitleEditItem pItem)
    {
        //Debug.Log("Will change the Title to: " + pItem.m_eTitle);
        Close();
    }



    [ContextMenu("Fix This Position")]
    public void FixPosition()
    {
        m_FixedPosition = m_Target.transform.position;
    }*/
}