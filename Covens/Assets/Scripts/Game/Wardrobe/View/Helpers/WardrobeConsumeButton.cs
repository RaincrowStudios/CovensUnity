using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WardrobeConsumeButton : UIButton
{
    public TextMeshProUGUI m_txtAmount;
    public GameObject m_goDisabled;
    public GameObject m_goLoading;
    public ConsumableItemModel m_Model;

    public event Action<WardrobeConsumeButton> OnClickEvent;


    public bool IsDisabled
    {
        get
        {
            return m_goDisabled.activeSelf;
        }
    }
    public bool IsLoading
    {
        get
        {
            return m_goLoading.activeSelf;
        }
    }


    public void Setup(ConsumableItemModel pItem)
    {
        m_Model = pItem;
        if (m_Model != null)
            m_txtAmount.text = m_Model.Count.ToString();
        else
            m_txtAmount.text = "0";
    }
    public void Consume(int iAmount)
    {
        Setup(m_Model);

    }


    public void SetLoading(bool bLoading)
    {
        m_goLoading.SetActive(bLoading);
    }
    public void SetDisabled(bool bLoading)
    {
        m_goDisabled.SetActive(bLoading);
    }

    public override void OnClickButton()
    {
        base.OnClickButton();
        if (OnClickEvent != null)
            OnClickEvent(this);
    }

}