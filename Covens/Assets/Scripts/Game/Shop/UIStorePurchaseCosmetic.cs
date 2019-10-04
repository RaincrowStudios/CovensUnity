using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Store;
using UnityEngine.UI;
using TMPro;
using System;

public class UIStorePurchaseCosmetic : UIStorePurchase
{
    [Space()]
    [SerializeField] private ApparelView m_MaleView;
    [SerializeField] private ApparelView m_FemaleView;
    [SerializeField] private Button m_PreviewButton;
    [SerializeField] private TextMeshProUGUI m_PreviewText;

    private CosmeticData m_Cosmetic;
    private int m_PreviewIndex;
    private static UIStorePurchaseCosmetic m_Instance;

    public static void Show(StoreItem item, CosmeticData data, Image icon, string locked, System.Action<string> onPurchase)
    {
        if (m_Instance == null)
            return;
               
        m_Instance.m_Cosmetic = data;
        m_Instance._Show(item, StoreManagerAPI.TYPE_COSMETIC, LocalizeLookUp.GetStoreTitle(item.id), "", icon, locked, onPurchase);
    }

    private void Awake()
    {
        m_Instance = this;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_CanvasGroup.transform.localScale = Vector3.zero;

        m_SilverButton.onClick.AddListener(OnClickBuySilver);
        m_GoldButton.onClick.AddListener(OnClickBuyGold);
        m_ClaimButton.onClick.AddListener(OnClickBuySilver);
        m_CloseButton.onClick.AddListener(_Close);

        m_Instance = this;
        m_MaleView.gameObject.SetActive(false);
        m_FemaleView.gameObject.SetActive(false);

        m_PreviewButton.onClick.AddListener(TogglePreview);
    }

    protected override void _Show(StoreItem item, string type, string title, string description, Image icon, string locked, Action<string> onPurchase)
    {
        m_MaleView.gameObject.SetActive(false);
        m_FemaleView.gameObject.SetActive(false);

        ApparelView apparel = PlayerDataManager.playerData.male ? m_MaleView : m_FemaleView;
        apparel.InitializeChar(PlayerDataManager.playerData.equipped);
        apparel.gameObject.SetActive(true);

        m_PreviewIndex = -1;
        TogglePreview();

        base._Show(item, type, title, description, icon, locked, onPurchase);
    }

    private void TogglePreview()
    {
        m_PreviewIndex++;

        List<ApparelType> available = new List<ApparelType>();
        if (m_Cosmetic.assets.baseAsset.Count > 0)
            available.Add(ApparelType.Base);
        if (m_Cosmetic.assets.white.Count > 0)
            available.Add(ApparelType.White);
        if (m_Cosmetic.assets.shadow.Count > 0)
            available.Add(ApparelType.Shadow);
        if (m_Cosmetic.assets.grey.Count > 0)
            available.Add(ApparelType.Grey);
        
        bool toggle = m_PreviewIndex >= 0 && m_PreviewIndex < available.Count;
        m_PreviewText.text = LocalizeLookUp.GetText(toggle ? "store_preview_on" : "store_preview_off");

        ApparelView apparel = PlayerDataManager.playerData.male ? m_MaleView : m_FemaleView;

        if (toggle)
        {
            m_Cosmetic.apparelType = available[m_PreviewIndex];
            apparel.EquipApparel(m_Cosmetic);
        }
        else
        {
            apparel.InitializeChar(PlayerDataManager.playerData.equipped);
            m_PreviewIndex = -1;
        }
    }
}
