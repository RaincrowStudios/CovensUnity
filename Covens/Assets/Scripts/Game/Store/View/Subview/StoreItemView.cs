using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemView : UIBaseAnimated
{

    [Header("Item")]
    public SimpleObjectPool m_ItemPool;
    public ScrollRect m_ItemScrollView;
    public ScrollbarDots m_ItemScrollbarDots;


    private List<StoreItem> m_WardrobeItemButtonCache;

    private void Start()
    {
        m_ItemPool.Setup();
    }

    public void Setup()
    {
        SetupItens(StoreController.Instance.GetWardrobeItems());
        m_ItemScrollView.horizontalScrollbar.value = 0;
    }

    public void SetupItens(List<WardrobeItemModel> vItens, bool bAnimate = true)
    {
        m_ItemPool.DespawnAll();
        m_WardrobeItemButtonCache = new List<StoreItem>();
        for (int i = 0; i < vItens.Count; i++)
        {
            // do not allow user to change its body
            if (vItens[i].EquipmentSlotEnum == EnumEquipmentSlot.BaseBody || vItens[i].EquipmentSlotEnum == EnumEquipmentSlot.BaseHand)
                continue;

            StoreItem pItemButton = m_ItemPool.Spawn<StoreItem>();
            pItemButton.Setup(vItens[i], WardrobeController.Instance.HasOwned(vItens[i].ID));
            pItemButton.OnClickBuyEvent += ItemButton_OnClickBuyEvent;
            pItemButton.OnClickTryEvent += ItemButton_OnClickTryEvent;
            m_WardrobeItemButtonCache.Add(pItemButton);
        }

        // setup scollbar
        int iAmount = UnityEngine.Mathf.CeilToInt( (float)vItens.Count / 4);
        m_ItemScrollbarDots.Setup(iAmount);
    }

    public void RefreshData()
    {
        for (int i = 0; i < m_WardrobeItemButtonCache.Count; i++)
        {
            WardrobeItemModel pItem = m_WardrobeItemButtonCache[i].ItemWardrobe;
            StoreItem pItemButton = m_WardrobeItemButtonCache[i];
            pItemButton.Setup(pItem, WardrobeController.Instance.HasOwned(pItem.ID));
            pItemButton.OnClickBuyEvent += ItemButton_OnClickBuyEvent;
            pItemButton.OnClickTryEvent += ItemButton_OnClickTryEvent;
        }
    }


    #region button click events

    private void OnClickItemsTab()
    {
        
    }
    private void OnClickStylesTab()
    {

    }

    private void ItemButton_OnClickTryEvent(StoreItem obj)
    {
        
    }

    private void ItemButton_OnClickBuyEvent(StoreItem obj, bool bUnlocked)
    {
        // already purchased item
        if (bUnlocked)
        {
            UIPurchaseNotification.ShowAlreadyUnlocked(obj.ItemWardrobe.DisplayName);
            return;
        }

        UIPurchaseOutfitConfirmationPopup pUI = UIManager.Show<UIPurchaseOutfitConfirmationPopup>();
        // ARE YOU SURE YOU WANT TO BUY THIS ELIXIR?
        pUI.Setup(
            //Oktagon.Localization.Lokaki.GetText("Store_BuyConfirmation"),
            //obj.ItemWardrobe..Replace("<value>", obj.ItemStore.Value.ToString()).Replace("<amount>", obj.ItemStore.Amount.ToString()),
            obj.ItemWardrobe,
            obj.ItemWardrobe.DisplayName,
            obj.ItemWardrobe.DisplayName,
            ItemDB.Instance.GetTexturePreview(obj.ItemWardrobe),
            obj.ItemWardrobe.GoldPrice,
            obj.ItemWardrobe.SilverPrice
            );
        pUI.OnClickBuyWithGoldEvent += UI_OnClickBuyWithGoldEvent;
        pUI.OnClickBuyWithSilverEvent += UI_OnClickBuyWithSilverEvent;
    }

    private void UI_OnClickBuyWithSilverEvent(UIPurchaseOutfitConfirmationPopup pUI)
    {
        UIGenericLoadingPopup.ShowLoading();
        StoreController.Instance.PurchaseItem(pUI.ItemModel, EnumCurrency.Silver, OnPurchaseComplete, OnPurchaseFail);
    }

    private void UI_OnClickBuyWithGoldEvent(UIPurchaseOutfitConfirmationPopup pUI)
    {
        UIGenericLoadingPopup.ShowLoading();
        StoreController.Instance.PurchaseItem(pUI.ItemModel, EnumCurrency.Gold, OnPurchaseComplete, OnPurchaseFail);
    }

    private void StyleScrollbarDots_OnIndexChangedEvent(int iIndex)
    {
        
    }
    #endregion

    void OnPurchaseComplete(string sOk)
    {
        RefreshData();
        UIGenericPopup.ShowConfirmPopup(
            //Item Unlocked!
            Oktagon.Localization.Lokaki.GetText("Store_ItemUnlockTitle"),
            // congratulations.. bla bla bla
            Oktagon.Localization.Lokaki.GetText("Store_ItemUnlockBody"),
            null
            );

        UIPurchaseOutfitConfirmationPopup pUI = UIManager.Get<UIPurchaseOutfitConfirmationPopup>();
        UIGenericLoadingPopup.CloseLoading();
        pUI.Close();
    }
    void OnPurchaseFail(string sFail)
    {
        UIGenericPopup.ShowErrorPopupLocalized(
            "Something went wrong.. not localized",
            null
            );
        UIGenericLoadingPopup.CloseLoading();
    }


}