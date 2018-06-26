using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeConsumeButton : MonoBehaviour
{
    public GameObject m_goDisabled;
    public GameObject m_goLoading;

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