﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WardrobeController : Patterns.SingletonClass<WardrobeController>
{
    private List<WardrobeItemModel> m_vAvailableItemList;
    public List<WardrobeItemModel> AvailableItemList
    {
        get{
            return GetAvailableItens(PlayerDataManager.Instance.Gender);
            //return m_vAvailableItemList;
        }
    }


    public List<WardrobeItemModel> GetAvailableItens(EnumGender eGender, bool GetAll = false)
    {
        if(m_vAvailableItemList == null)
        {
            if (GetAll)
            {
                Debug.Log("Using Get all for testing purpoise");
                m_vAvailableItemList = ItemDB.Instance.GetItens(eGender);
                return m_vAvailableItemList;
            }
            string[] vCosmetics = PlayerDataManager.Instance.Cosmetics;
            List<WardrobeItemModel> vList = new List<WardrobeItemModel>();
            for (int i = 0; i < vCosmetics.Length; i++)
            {
                WardrobeItemModel pItem = ItemDB.Instance.GetItem(vCosmetics[i]);
                if (pItem != null && pItem.GenderEnum == eGender)
                {
                    vList.Add(pItem);
                    continue;
                }
            }
            m_vAvailableItemList = vList;
        }
        return m_vAvailableItemList;
    }

    public List<WardrobeItemModel> GetAvailableItens(EnumEquipmentSlot eSlot, EnumGender eGender)
    {
        
        List<WardrobeItemModel> vItemList = new List<WardrobeItemModel>();
        List<WardrobeItemModel> vAvailableList = GetAvailableItens(eGender);
        for (int i = 0; i < vAvailableList.Count; i++)
        {
            if (vAvailableList[i].GenderEnum == eGender && vAvailableList[i].EquipmentSlotEnum == eSlot )
            {
                vItemList.Add(vAvailableList[i]);
            }
        }
        return vItemList;
    }
    public List<WardrobeItemModel> GetAvailableItens(EnumWardrobeCategory eCategories, EnumGender eGender)
    {
        List<WardrobeItemModel> vItemList = new List<WardrobeItemModel>();
        List<WardrobeItemModel> vAvailableList = GetAvailableItens(eGender);
        for (int i = 0; i < vAvailableList.Count; i++)
        {
            if (vAvailableList[i].GenderEnum == eGender && (vAvailableList[i].WardrobeCategory & eCategories) != 0)
            {
                vItemList.Add(vAvailableList[i]);
            }
        }
        return vItemList;
    }

    public bool HasOwned(string sItemID)
    {
        for (int i =0; i < AvailableItemList.Count; i++)
        {
            if (AvailableItemList[i].ID == sItemID)
            {
                return true;
            }
        }
        return false;
    }


    public void OnPurchaseItem(string sId)
    {
        PlayerDataManager.Instance.OnPurchaseItem(sId);
        m_vAvailableItemList = null;
        // just to cache the list
        int i = AvailableItemList.Count;
    }    

}