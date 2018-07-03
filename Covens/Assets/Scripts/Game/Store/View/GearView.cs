using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearView : UIBaseAnimated
{
    public SimpleObjectPool m_ItemPool;
    public ScrollRect m_ScrollView;
    public ScrollbarDots m_ScrollbarDots;

    private List<StoreItem> m_WardrobeItemButtonCache;



    private void Start()
    {
        m_ItemPool.Setup();
    }

    public override void Show()
    {
        Invoke("ShowScheduled", .4f);
    }
    void ShowScheduled()
    {
        base.Show();
        SetupItens(ItemDB.Instance.GetItens(PlayerDataManager.Instance.Gender));
        m_ScrollView.horizontalScrollbar.value = 0;
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
            pItemButton.Setup(vItens[i]);
            pItemButton.OnClickBuyEvent += ItemButton_OnClickBuyEvent;
            pItemButton.OnClickTryEvent += ItemButton_OnClickTryEvent;
            m_WardrobeItemButtonCache.Add(pItemButton);
        }

        // setup scollbar
        int iAmount = UnityEngine.Mathf.CeilToInt( (float)vItens.Count / 6);
        m_ScrollbarDots.Setup(iAmount);
    }

    private void ItemButton_OnClickTryEvent(StoreItem obj)
    {
        
    }

    private void ItemButton_OnClickBuyEvent(StoreItem obj)
    {
        UIPurchaseOutfitConfirmationPopup pUI = UIManager.Show<UIPurchaseOutfitConfirmationPopup>();
        // ARE YOU SURE YOU WANT TO BUY THIS ELIXIR?
        pUI.Setup(
            //Oktagon.Localization.Lokaki.GetText("Store_BuyConfirmation"),
            //obj.ItemWardrobe..Replace("<value>", obj.ItemStore.Value.ToString()).Replace("<amount>", obj.ItemStore.Amount.ToString()),
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
        StartCoroutine(Test(OnPurchaseComplete));
        //StoreController.Purchase()
    }

    private void UI_OnClickBuyWithGoldEvent(UIPurchaseOutfitConfirmationPopup pUI)
    {
        UIGenericLoadingPopup.ShowLoading();
        StartCoroutine(Test(OnPurchaseComplete));
        //StoreController.Purchase()
    }


    void OnPurchaseComplete()
    {
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
    void OnPurchaseFail()
    {
        UIGenericPopup.ShowErrorPopupLocalized(
            "Something went wrong.. not localized",
            null
            );
        UIGenericLoadingPopup.CloseLoading();
    }


    IEnumerator Test(System.Action pAct)
    {
        yield return new WaitForSeconds(1);
        pAct();
    }
}