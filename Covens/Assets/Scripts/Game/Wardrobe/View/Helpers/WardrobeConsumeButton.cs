using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WardrobeConsumeButton : MonoBehaviour
{
    public Text m_txtAmount;
    public GameObject m_goDisabled;
    public GameObject m_goLoading;
    public InventoryItems m_Model;

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


    public void Setup(InventoryItems pItem)
    {
        m_Model = pItem;
        if (m_Model != null)
            m_txtAmount.text = m_Model.count.ToString();
        else
            m_txtAmount.text = "0";
    }
    public void Consumed(int iAmount)
    {
        m_Model.count = m_Model.count - iAmount;
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

    public void OnClickButton()
    {
        if (OnClickEvent != null)
            OnClickEvent(this);
    }

}