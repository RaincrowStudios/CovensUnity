using System;
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
            return m_vAvailableItemList;
        }
    }


    public List<WardrobeItemModel> GetAvailableItens(EnumGender eGender, bool GetAll = false)
    {
        if(AvailableItemList == null)
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
        return AvailableItemList;
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



    public InventoryItems[] GetAvailableConsumables()
    {
        return PlayerDataManager.Instance.Consumables;
    }

    public InventoryItems GetAvailableConsumableEnergy()
    {
        foreach(var p in GetAvailableConsumables())
        {
            if (p.id == "consumable_energyPotion100")
                return p;
        }
        return null;
    }
    public InventoryItems GetAvailableConsumableWisdom()
    {
        foreach (var p in GetAvailableConsumables())
        {
            if (p.id == "consumable_wisdomBooster1")
                return p;
        }
        return null;
    }
    public InventoryItems GetAvailableConsumableAptitude()
    {
        foreach (var p in GetAvailableConsumables())
        {
            if (p.id == "consumable_aptitudeBooster1")
                return p;
        }
        return null;
    }
    //public List<WardrobeItemModel> GetAvailableConsumables()
    //{
    //    List<WardrobeItemModel> vList = new List<WardrobeItemModel>();
    //    foreach(var v in PlayerDataManager.Instance.Consumables)
    //    {
    //        WardrobeItemModel pItem = ItemDB.Instance.GetItem(v.id);
    //        if (pItem != null)
    //            vList.Add(pItem);
    //    }
    //    return vList;
    //}

    public void ConsumeItem(WardrobeItemModel pItem)
    {
        
    }
}