using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableItem : MonoBehaviour
{
    public Image[] m_IteractableItem;
    public Color m_ColorNormal;
    public Color m_ColorEquipped;
    public bool m_ChangeAlpha = false;


    public bool IsSelected
    {
        get;
        private set;
    }

    [ContextMenu("SetEquipped")]
    public void SetSelected()
    {
        SetSelected(true);
    }
    [ContextMenu("SetUnequipped")]
    public void SetUnselected()
    {
        SetSelected(false);
    }

    public void SetSelected(bool bSelected)
    {
        IsSelected = bSelected;
        for (int i = 0; i < m_IteractableItem.Length; i++)
        {
            Color pColor = bSelected ? m_ColorEquipped : m_ColorNormal;
            if(!m_ChangeAlpha)
                pColor.a = m_IteractableItem[i].color.a;
            m_IteractableItem[i].color = pColor;
        }
    }
}