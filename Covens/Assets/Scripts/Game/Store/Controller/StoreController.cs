using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// controlls the items and its purchases
/// </summary>
public class StoreController : Patterns.SingletonComponent<StoreController>
{
    
    private Shop_DisplayResponse m_pShop_DisplayResponse;

    #region gets

    public ShopBundle[] DisplayList
    {
        get
        {
            return m_pShop_DisplayResponse.items;
        }
    }

    /// <summary>
    /// stores the current in purchase item. It will be used to restore the item
    /// </summary>
    public StoreItemModel PurchasingItemIAP
    {
        get
        {
            string sJson = PlayerPrefs.GetString("StoreController.PurchasingItemIAP", null);
            if (!string.IsNullOrEmpty(sJson))
            {
                StoreItemModel pModel = JsonUtility.FromJson<StoreItemModel>(sJson);
                return pModel;
            }
            return null;
        }
        set
        {
            PlayerPrefs.SetString("StoreController.PurchasingItemIAP", JsonUtility.ToJson(value));
        }
    }

    #endregion


    public static void Load()
    {
        Instance.Display(Instance.OnLoadShopDisplay, null);
    }

    public void OnLoadShopDisplay(Shop_DisplayResponse pResponse)
    {
        m_pShop_DisplayResponse = pResponse;
    }


    /// <summary>
    /// gets the wardrobe items to be selled
    /// </summary>
    /// <returns></returns>
    public List<WardrobeItemModel> GetWardrobeItems()
    {
        List<WardrobeItemModel> vList = new List<WardrobeItemModel>();
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
        }
        return vList;
    }
    /// <summary>
    /// get the wardrobe styles (its a wardrobe item but used in styles slot)
    /// </summary>
    /// <returns></returns>
    public List<WardrobeItemModel> GetWardrobeStyles()
    {
        List<WardrobeItemModel> vList = new List<WardrobeItemModel>();
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
        }
        return vList;
    }
    /// <summary>
    /// gets the store item (StoreDB)
    /// </summary>
    /// <param name="eStores"></param>
    /// <returns></returns>
    public List<StoreItemModel> GetStoreItems(params EnumStoreType[] eStores)
    {
        List<StoreItemModel> vItemsDB = StoreDB.Instance.GetItens(eStores);
        List<StoreItemModel> vList = new List<StoreItemModel>();
        // get iaps
        //List<StoreItemModel> vIAPList = new List<StoreItemModel>();
        for (int i = 0; i < vItemsDB.Count; i++)
        {
            if (vItemsDB[i].StoreTypeEnum == EnumStoreType.IAP)
                vList.Add(vItemsDB[i]);
        }

        // filter
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



    /// <summary>
    /// purchases the silver with In App Purchase
    /// </summary>
    /// <param name="pItem"></param>
    public void PurchaseIAP(StoreItemModel pItem, Action<string> pSuccess, Action<string> pError)
    {
        PurchasingItemIAP = pItem;
        Action<UnityEngine.Purchasing.PurchaseEventArgs> Success = (UnityEngine.Purchasing.PurchaseEventArgs pArgs) => 
        {
            PurchaseSilver(pItem.ID, pSuccess, pError);
        };
        Action<string> Failure = (string sNotOk) => {
            pError(sNotOk);
        };

        IAPController.Instance.Purchase(pItem.ID, Success, Failure);
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
        Action<string> Success = (string sOK) =>
        {
            PurchasingItemIAP = null;
            if (pSuccess != null) pSuccess(sOK);
        };
        StoreAPI.PurchaseSilver(sItemID, Success, pError);
    }
    public void Display(Action<Shop_DisplayResponse> pSuccess, Action<string> pError)
    {
        StoreAPI.Display(pSuccess, pError);
    }
    #endregion



    /*
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 150, 200, 50), "init"))
        {
            IAPController.Load();
        }
        if (GUI.Button(new Rect(0, 300, 200, 50), "test buy"))
        {
            var vIAPList = StoreDB.Instance.GetItens(new EnumStoreType[] { EnumStoreType.IAP });
            PurchaseIAP(vIAPList[0], null, null);
        }
    }*/


}