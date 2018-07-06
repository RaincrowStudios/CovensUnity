using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreController : Patterns.SingletonComponent<StoreController>
{
    
    private Shop_DisplayResponse m_pShop_DisplayResponse;

    public ShopBundle[] DisplayList
    {
        get
        {
            return m_pShop_DisplayResponse.items;
        }
    }

    public static void Load()
    {
        Instance.Display(Instance.OnLoadShopDisplay, null);
    }

    public void OnLoadShopDisplay(Shop_DisplayResponse pResponse)
    {
        m_pShop_DisplayResponse = pResponse;
    }



    public List<WardrobeItemModel> GetWardrobeItems()
    {
        List<WardrobeItemModel> vList = new List<WardrobeItemModel>();
        //string sMissing = "";
        for (int i = 0; i < DisplayList.Length; i++)
        {
            if (DisplayList[i].Type != "cosmetics")
                continue;
            WardrobeItemModel pItem = ItemDB.Instance.GetItem(DisplayList[i].Id);
            if (pItem != null
                && pItem.EquipmentSlotEnum != EnumEquipmentSlot.SpecialSlot
                && pItem.GenderEnum == PlayerDataManager.Instance.Gender
            )
            {
                vList.Add(pItem);
            }
            //else
            //    sMissing += "\n  - " + DisplayList[i].Id;
        }
        //Debug.Log("GetWardrobeItems.Missing: " + sMissing);
        return vList;
    }
    public List<WardrobeItemModel> GetWardrobeStyles()
    {
        List<WardrobeItemModel> vList = new List<WardrobeItemModel>();
        //string sMissing = "";
        for (int i = 0; i < DisplayList.Length; i++)
        {
            if (DisplayList[i].Type != "cosmetics")
                continue;
            WardrobeItemModel pItem = ItemDB.Instance.GetItem(DisplayList[i].Id);
            if (pItem != null
                && pItem.EquipmentSlotEnum == EnumEquipmentSlot.SpecialSlot
                && pItem.GenderEnum == PlayerDataManager.Instance.Gender
            )
            {
                vList.Add(pItem);
            }
            //else
            //    sMissing += "\n  - " + DisplayList[i].Id;
        }
        //Debug.Log("GetWardrobeStyles.Missing: " + sMissing);
        return vList;
    }

    public List<StoreItemModel> GetStoreItems(params EnumStoreType[] eStores)
    {
        List<StoreItemModel> vItemsDB = StoreDB.Instance.GetItens(eStores);
        List<StoreItemModel> vList = new List<StoreItemModel>();
        for (int i = 0; i < DisplayList.Length; i++)
        {
            for (int j = 0; j < vItemsDB.Count; j++)
            {
                if(vItemsDB[j].ID == DisplayList[i].Id)
                {
                    vList.Add(vItemsDB[j]);
                    break;
                }
            }
        }
        return vList;
    }


    #region server synch

    public void PurchaseItem(WardrobeItemModel pItem, EnumCurrency eCurrency, Action<string> pSuccess, Action<string> pError)
    {
        Action<string> Success = (string sOK) =>
        {
            WardrobeController.Instance.OnPurchaseItem(pItem.ID);
            if (pSuccess != null) pSuccess(sOK);
        };
        StoreAPI.Purchase(pItem.ID, eCurrency.ToString().ToLower(), 1, Success, pError);
    }
    public void PurchaseStore(StoreItemModel pItem, EnumCurrency eCurrency, Action<string> pSuccess, Action<string> pError)
    {
        Action<string> Success = (string sOK) =>
        {
            PlayerDataManager.Instance.OnPurchaseItem(pItem.ID);
            if (pSuccess != null) pSuccess(sOK);
        };
        StoreAPI.Purchase(pItem.ID, eCurrency.ToString().ToLower(), (int)pItem.Amount, Success, pError);
    }
    public void PurchaseSilver(string sItemID, Action<string> pSuccess, Action<string> pError)
    {
        StoreAPI.PurchaseSilver(sItemID, pSuccess, pError);
    }
    public void Display(Action<Shop_DisplayResponse> pSuccess, Action<string> pError)
    {
        StoreAPI.Display(pSuccess, pError);
    }

    #endregion


}