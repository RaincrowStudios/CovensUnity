using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreGenericView : UIBaseAnimated

{
    public SimpleObjectPool m_ItemPool;
    public EnumStoreType[] m_StoreItems;
    public ScrollRect m_ScrollView;
    public ScrollbarDots m_ScrollbarDots;

    public Text m_txtTitle;

    private List<StoreItem> m_WardrobeItemButtonCache = new List<StoreItem>();


    private void Start()
    {
        m_ItemPool.Setup();
    }

    public void SetupType(EnumStoreType[] vStoreItems)
    {
        m_StoreItems = vStoreItems;
    }

    public override void Show()
    {
        base.Show();
        var vItemList = StoreController.Instance.GetStoreItems(m_StoreItems);
        SetupItens(vItemList);
        if(m_StoreItems[0] == EnumStoreType.IAP)
        {
            m_txtTitle.text = Oktagon.Localization.Lokaki.GetText("Store_SilverTitle");
        }
        else
        {
            m_txtTitle.text = Oktagon.Localization.Lokaki.GetText("Store_ElixirTitle");
        }
    }

    public void SetupItens(List<StoreItemModel> vItens, bool bAnimate = true)
    {
        m_ItemPool.DespawnAll();
        //m_WardrobeItemButtonCache = new List<StoreItem>();
        for (int i = 0; i < vItens.Count; i++)
        {
            StoreItem pItemButton = GetStoreItem(vItens[i]);
            if (pItemButton == null)
            {
                pItemButton = m_ItemPool.SpawnNew<StoreItem>();
                m_WardrobeItemButtonCache.Add(pItemButton);
            }
            else
                pItemButton.gameObject.SetActive(true);
            // setups the item
            pItemButton.Setup(vItens[i]);
            pItemButton.OnClickBuyEvent += ItemButton_OnClickBuyEvent;
        }

        // setup scollbar
        int iAmount = UnityEngine.Mathf.CeilToInt((float)vItens.Count / 4);
        m_ScrollbarDots.Setup(iAmount);
    }

    /// <summary>
    /// purchases the item with IAP
    /// </summary>
    /// <param name="obj"></param>
    private void BuyIAP(StoreItem obj)
    {
        UIGenericLoadingPopup.ShowLoading();
        Action<string> Success = (string sOk) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            OnPurchaseComplete(obj);
        };
        Action<string> Fail = (string sNotOk) =>
        {
            UIGenericLoadingPopup.CloseLoading();
            UIGenericPopup.ShowErrorPopupLocalized(sNotOk, null);
        };
        StoreController.Instance.PurchaseIAP(obj.ItemStore, Success, Fail);
        //StartCoroutine(PurchaseIAP(obj));
    }
    /* test case
    IEnumerator PurchaseIAP(StoreItem obj)
    {
        UIGenericLoadingPopup.ShowLoading();
        yield return null;

        // purchase IAP SDK
        UIGenericLoadingPopup.SetTitle("Purchasing on Store");
        StoreController.Instance.PurchaseIAP(obj.ItemStore, null, null);
        //yield return new WaitForSeconds(1f);

        // Validate with server
        UIGenericLoadingPopup.SetTitle("Validating the purchase");
        yield return new WaitForSeconds(1f);

        // Call
        OnPurchaseComplete(obj);
    }*/
    /// <summary>
    /// purchases the item with silver
    /// </summary>
    /// <param name="obj"></param>
    private void BuyStoreItem(StoreItem obj)
    {
        UIPurchaseConfirmationPopup pUI = UIManager.Show<UIPurchaseConfirmationPopup>();
        // ARE YOU SURE YOU WANT TO BUY THIS ELIXIR?
        pUI.Setup(
            obj.ItemStore,
            obj.ItemStore.DisplayName,
            Oktagon.Localization.Lokaki.GetText("Store_BuyConfirmation"),
            obj.ItemStore.DisplayDescription.Replace("<value>", obj.ItemStore.Value.ToString()).Replace("<amount>", obj.ItemStore.Amount.ToString()),
            SpriteResources.GetSprite(obj.ItemStore.Icon),
            obj.ItemStore.GoldPrice,
            obj.ItemStore.SilverPrice
            );
        pUI.OnClickBuyWithGoldEvent += UI_OnClickBuyWithGoldEvent;
        pUI.OnClickBuyWithSilverEvent += UI_OnClickBuyWithSilverEvent;
    }


    #region button click events

    private void ItemButton_OnClickBuyEvent(StoreItem obj, bool bUnlocked)
    {
        switch (obj.ItemStore.StoreTypeEnum)
        {
            case EnumStoreType.IAP:
                BuyIAP(obj);
                break;

            default:
                BuyStoreItem(obj);
                break;
        }
        
    }

    private void UI_OnClickBuyWithSilverEvent(UIPurchaseConfirmationPopup pUI)
    {
        UIGenericLoadingPopup.ShowLoading();
        StoreController.Instance.PurchaseStore(pUI.ItemModel, EnumCurrency.Silver, OnPurchaseComplete, OnPurchaseFail);
    }

    private void UI_OnClickBuyWithGoldEvent(UIPurchaseConfirmationPopup pUI)
    {
        UIGenericLoadingPopup.ShowLoading();
        StoreController.Instance.PurchaseStore(pUI.ItemModel, EnumCurrency.Silver, OnPurchaseComplete, OnPurchaseFail);
    }

    #endregion
    void OnPurchaseComplete(StoreItem pItem)
    {
        UIPurchaseSuccess pUISuccess = UIManager.Show<UIPurchaseSuccess>();
        pUISuccess.Setup(pItem.ItemStore.DescriptionId, SpriteResources.GetSprite(pItem.ItemStore.Icon));
        UIGenericLoadingPopup.CloseLoading();
    }
    void OnPurchaseComplete(string sOk)
    {
        UIPurchaseSuccess pUISuccess = UIManager.Show<UIPurchaseSuccess>();
        UIPurchaseConfirmationPopup pUI = UIManager.Get<UIPurchaseConfirmationPopup>();
        pUISuccess.Setup(pUI.m_txtDescription.text, pUI.m_ItemImage.sprite);
        UIGenericLoadingPopup.CloseLoading();
        pUI.Close();
    }
    void OnPurchaseFail(string sOk)
    {
        UIGenericLoadingPopup.CloseLoading();
        UIGenericPopup.ShowErrorPopupLocalized(
            "Something went wrong.. not localized",
            null
            );
    }
    /*
    IEnumerator Test(System.Action pAct)
    {
        yield return new WaitForSeconds(1);
        pAct();
    }*/
    StoreItem GetStoreItem(StoreItemModel pItem)
    {
        if (m_WardrobeItemButtonCache != null)
        {
            for(int i =0; i < m_WardrobeItemButtonCache.Count; i++)
            {
                if (m_WardrobeItemButtonCache[i].ID == pItem.ID)
                    return m_WardrobeItemButtonCache[i];
            }
        }
        return null;
    }
}