using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreStyleView : UIBaseAnimated
{

    [Header("Style")]
    public GameObject m_ItemRoot;
    public Button m_ItemButton;
    public SimpleObjectPool m_ItemPool;
    public ScrollRect m_ItemScrollView;
    public ScrollbarDots m_ItemScrollbarDots;

    [Header("ItemDisplay")]
    public GameObject m_PriceRoot; 
    public Text m_txtTitle;
    public Text m_txtDescription;
    public GameObject m_goGoldPrice;
    public Text m_txtGoldPrice;
    public GameObject m_goSilverPrice;
    public Text m_txtSilverPrice;

    public GameObject m_ButtonUnlock;
    public GameObject m_ButtonUnlocked;



    private List<StoreItem> m_WardrobeItemButtonCache;

    private void Start()
    {
        if(m_ItemPool != null)
            m_ItemPool.Setup();
        m_ItemScrollbarDots.OnIndexChangedEvent += StyleScrollbarDots_OnIndexChangedEvent;
    }

    public void Setup()
    {
        SetupItens(ItemDB.Instance.GetItens(EnumEquipmentSlot.SpecialSlot, PlayerDataManager.Instance.Gender));
        m_ItemScrollView.horizontalScrollbar.value = 0;
    }

    public void SetupItens(List<WardrobeItemModel> vItens, bool bAnimate = true)
    {
        Debug.Log(">>> " + vItens.Count);
        m_ItemPool.DespawnAll();
        m_WardrobeItemButtonCache = new List<StoreItem>();
        for (int i = 0; i < vItens.Count; i++)
        {
            // do not allow user to change its body
            if (vItens[i].EquipmentSlotEnum == EnumEquipmentSlot.BaseBody || vItens[i].EquipmentSlotEnum == EnumEquipmentSlot.BaseHand)
                continue;

            StoreItem pItemButton = m_ItemPool.Spawn<StoreItem>();
            pItemButton.Setup(vItens[i], WardrobeController.Instance.HasOwned(vItens[i].ID));
            m_WardrobeItemButtonCache.Add(pItemButton);
        }

        // setup scollbar
        int iAmount = UnityEngine.Mathf.CeilToInt((float)vItens.Count );
        m_ItemScrollbarDots.Setup(iAmount);
    }



    #region button click events

    public void OnClickUnlocked()
    {
        var pStyle = m_WardrobeItemButtonCache[m_ItemScrollbarDots.Index].ItemWardrobe;
        UIPurchaseNotification.ShowAlreadyUnlocked(pStyle.DisplayName);
    }
    public void OnClickUnlock()
    {
        var pStyle = m_WardrobeItemButtonCache[m_ItemScrollbarDots.Index].ItemWardrobe;
        UIPurchaseOutfitConfirmationPopup pUI = UIManager.Show<UIPurchaseOutfitConfirmationPopup>();
        // ARE YOU SURE YOU WANT TO BUY THIS ELIXIR?
        pUI.Setup(
            //Oktagon.Localization.Lokaki.GetText("Store_BuyConfirmation"),
            //obj.ItemWardrobe..Replace("<value>", obj.ItemStore.Value.ToString()).Replace("<amount>", obj.ItemStore.Amount.ToString()),
            pStyle.DisplayName,
            pStyle.DisplayName,
            ItemDB.Instance.GetTexturePreview(pStyle),
            pStyle.GoldPrice,
            pStyle.SilverPrice
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

    private void StyleScrollbarDots_OnIndexChangedEvent(int iIndex)
    {
        var pStyle = m_WardrobeItemButtonCache[m_ItemScrollbarDots.Index].ItemWardrobe;
        m_txtTitle.text = pStyle.DisplayName;
        m_txtDescription.text = pStyle.Description;
        m_goGoldPrice.SetActive(pStyle.GoldPrice > 0);
        m_txtGoldPrice.text = pStyle.GoldPrice.ToString();
        m_goSilverPrice.SetActive(pStyle.SilverPrice > 0);
        m_txtSilverPrice.text = pStyle.SilverPrice.ToString();

        // setup unlock button
        bool bUnlocked = WardrobeController.Instance.HasOwned(pStyle.ID);
        m_ButtonUnlock.SetActive(!bUnlocked);
        m_ButtonUnlocked.SetActive(bUnlocked);
        
        // animate
        float f = 1.05f;
        LeanTween.cancel(m_PriceRoot);
        m_PriceRoot.transform.localScale = Vector3.one;
        LeanTween.scale(m_PriceRoot, new Vector3(f, f, f), .2f).setEase(LeanTweenType.punch);
    }
    #endregion

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
