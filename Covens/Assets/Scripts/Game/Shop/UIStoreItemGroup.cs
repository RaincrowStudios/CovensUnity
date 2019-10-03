using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStoreItemGroup : MonoBehaviour
{
    [SerializeField] private UIStoreItem[] m_Items;
    
    private int m_Count = 0;
    private RectTransform m_RectTransform;
    private bool m_SingleRow = false;
    
    public bool IsVisible => m_RectTransform.position.x > 0 && m_RectTransform.position.x < Screen.width;
    public UIStoreItem[] Items => m_Items;

    private void Awake()
    {
        m_RectTransform = this.GetComponent<RectTransform>();
    }

    public void LateUpdate()
    {
        if (IsVisible)
        {
            foreach (var item in m_Items)
            {
                if (item.gameObject.activeSelf && !item.IconLoaded)
                    item.LoadIcon();
            }
        }
    }

    public void OnSpawn()
    {
        transform.localScale = Vector3.one;
        foreach (var item in m_Items)
            item.gameObject.SetActive(false);
        m_Count = 0;
    }

    public UIStoreItem GetItem()
    {
        if (m_Count < (m_SingleRow ? 1 : m_Items.Length))
        {
            m_Count++;
            m_Items[m_Count - 1].gameObject.SetActive(true);
            return m_Items[m_Count - 1];
        }

        return null;
    }

    public void SetSingleRowLayout(bool singleRow)
    {
        m_SingleRow = singleRow;

        if (singleRow)
            m_RectTransform.sizeDelta = new Vector2(560, 980 / 2);
        else
            m_RectTransform.sizeDelta = new Vector2(560, 980);
    }
}
