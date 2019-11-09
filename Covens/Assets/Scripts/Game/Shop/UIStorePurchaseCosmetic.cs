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
    [SerializeField] private RectTransform m_PreviewIcons;

    protected CosmeticData m_Cosmetic;
    private int m_PreviewIndex;

    private void Awake()
    {
        m_InstanceCosmetic = this;

        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_CanvasGroup.transform.localScale = Vector3.zero;

        m_SilverButton.onClick.AddListener(OnClickBuySilver);
        m_GoldButton.onClick.AddListener(OnClickBuyGold);
        m_ClaimButton.onClick.AddListener(OnClickBuySilver);
        m_CloseButton.onClick.AddListener(_Close);

        m_MaleView.gameObject.SetActive(false);
        m_FemaleView.gameObject.SetActive(false);

        m_PreviewButton.onClick.AddListener(TogglePreview);
    }

    public void _Show(StoreItem item, CosmeticData cosmetic, string type, string title, string description, Sprite icon, string locked, Action<string> onPurchase)
    {
        m_Cosmetic = cosmetic;

        m_MaleView.gameObject.SetActive(false);
        m_FemaleView.gameObject.SetActive(false);

        ApparelView apparel = PlayerDataManager.playerData.male ? m_MaleView : m_FemaleView;
        apparel.InitCharacter(PlayerDataManager.playerData.equipped);
        apparel.gameObject.SetActive(true);

        m_PreviewIndex = -1;
        TogglePreview();

        base._Show(item, type, title, description, icon, locked, onPurchase);
    }

    protected override void _OnClosed()
    {
        m_MaleView.ResetApparelView();
        m_FemaleView.ResetApparelView();
    }

    private void TogglePreview()
    {
        m_PreviewIndex++;

        List<ApparelType> available = new List<ApparelType>();
        if (m_Cosmetic.assets.baseAsset.Count > 0)
            available.Add(ApparelType.Base);
        if (m_Cosmetic.assets.shadow.Count > 0)
            available.Add(ApparelType.Shadow);
        if (m_Cosmetic.assets.grey.Count > 0)
            available.Add(ApparelType.Grey);
        if (m_Cosmetic.assets.white.Count > 0)
            available.Add(ApparelType.White);

        bool toggle = m_PreviewIndex >= 0 && m_PreviewIndex < available.Count;

        //setup text
        m_PreviewText.text = LocalizeLookUp.GetText(toggle ? "store_preview_on" : "store_preview_off");

        //toggle icon
        m_PreviewIcons.GetChild(0).gameObject.SetActive(true);
        m_PreviewIcons.GetChild(1).gameObject.SetActive(m_Cosmetic.assets.baseAsset.Count > 0);
        m_PreviewIcons.GetChild(2).gameObject.SetActive(m_Cosmetic.assets.shadow.Count > 0);
        m_PreviewIcons.GetChild(3).gameObject.SetActive(m_Cosmetic.assets.grey.Count > 0);
        m_PreviewIcons.GetChild(4).gameObject.SetActive(m_Cosmetic.assets.white.Count > 0);

        //toggle selected icon
        m_PreviewIcons.GetChild(0).GetChild(0).gameObject.SetActive(!toggle);
        m_PreviewIcons.GetChild(1).GetChild(0).gameObject.SetActive(toggle && available[m_PreviewIndex] == ApparelType.Base);
        m_PreviewIcons.GetChild(2).GetChild(0).gameObject.SetActive(toggle && available[m_PreviewIndex] == ApparelType.Shadow);
        m_PreviewIcons.GetChild(3).GetChild(0).gameObject.SetActive(toggle && available[m_PreviewIndex] == ApparelType.Grey);
        m_PreviewIcons.GetChild(4).GetChild(0).gameObject.SetActive(toggle && available[m_PreviewIndex] == ApparelType.White);

        //update apparel view
        ApparelView apparel = PlayerDataManager.playerData.male ? m_MaleView : m_FemaleView;
        if (toggle)
        {
            m_Cosmetic.apparelType = available[m_PreviewIndex];
            apparel.EquipApparel(m_Cosmetic);
        }
        else
        {
            apparel.InitCharacter(PlayerDataManager.playerData.equipped);
            m_PreviewIndex = -1;
        }
    }
}
