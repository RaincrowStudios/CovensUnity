using UnityEngine;
using System.Collections;
using System;

public class WardobeFilterButton : MonoBehaviour
{
	public EnumEquipmentSlot m_Slot;
    [EnumFlag("Category Flag")]
    public EnumWardrobeCategory m_Category;
    public SelectableItem m_SelectableItem;
    public event Action<WardobeFilterButton> OnClickEvent;


    WardrobeView m_pWardrobeView;


    void Start()
	{
        m_pWardrobeView = GameObject.FindObjectOfType<WardrobeView>();
        
    }
    
    private void OnEnable()
    {
        SetSelected(false);
    }
    
    public void onClick()
	{
        m_pWardrobeView.OnClickFilterButton(this);
//        if(m_Slot == EnumEquipmentSlot.None)
//            m_pWardrobeView.OnClickFilter(m_Category, transform);
//        else
//            m_pWardrobeView.OnClickFilter(m_Slot, transform);
        //SetSelected(true);
	}

    public void SetSelected(bool bSelected)
    {
        if(m_SelectableItem != null)
            m_SelectableItem.SetSelected(bSelected);
    }

}

