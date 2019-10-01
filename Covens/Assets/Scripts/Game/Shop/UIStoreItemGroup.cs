using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStoreItemGroup : MonoBehaviour
{
    [SerializeField] private UIStoreItem[] m_Items;

    private int count = 0;

    public void OnSpawn()
    {
        transform.localScale = Vector3.one;
        foreach (var item in m_Items)
            item.gameObject.SetActive(false);
        count = 0;
    }

    public UIStoreItem GetItem()
    {
        if (count < m_Items.Length)
        {
            count++;
            m_Items[count - 1].gameObject.SetActive(true);
            return m_Items[count - 1];
        }

        return null;
    }
}
