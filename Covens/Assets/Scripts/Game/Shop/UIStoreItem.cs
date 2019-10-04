using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Store;
using EnhancedUI.EnhancedScroller;

public class UIStoreItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_ItemTitle;
    [SerializeField] private Image m_ItemIcon;
    [SerializeField] private TextMeshProUGUI m_ItemSubtitle;
    [SerializeField] private TextMeshProUGUI m_ItemAmount;

    [Header("Cost")]
    [SerializeField] private LayoutGroup m_CostLayout;
    [SerializeField] private TextMeshProUGUI m_SilverCost;
    [SerializeField] private Image m_SilverIcon;
    [SerializeField] private TextMeshProUGUI m_GoldCost;
    [SerializeField] private Image m_GoldIcon;
    [SerializeField] private GameObject m_CostOr;

    [Header("Buy")]
    [SerializeField] private Button m_BuyButton;
    [SerializeField] private Image m_BuyIcon;
    [SerializeField] private TextMeshProUGUI m_BuyText;
    [Space()]
    [SerializeField] private Sprite m_GreenSprite;
    [SerializeField] private Sprite m_RedSprite;
    [SerializeField] private Sprite m_GreySprite;

    [Header("Tag")]
    [SerializeField] private GameObject m_Tag;
    [SerializeField] private TextMeshProUGUI m_TagText;

    private string m_IconId = null;
    public bool IconLoaded { get; private set; }
    public static bool IsLoadingIcon { get; private set; }
    
    public void LoadIcon()
    {
        if (IsLoadingIcon)
            return;

        IconLoaded = true;
        IsLoadingIcon = true;

        DownloadedAssets.GetSprite(
            m_IconId, 
            spr =>
            {
                IsLoadingIcon = false;
                m_ItemIcon.overrideSprite = spr;
            }, 
            true);
    }

    private void SetIconLayout_Cosmetic()
    {
        RectTransform rectTransform = m_ItemIcon.rectTransform;
        rectTransform.pivot = new Vector2(1, 0.5f);
        rectTransform.anchoredPosition = new Vector2(182.9999f, 219f);
        rectTransform.sizeDelta = new Vector2(251f, 364.5f);
    }

    private void SetIconLayout_IngredientBundle()
    {
        RectTransform rectTransform = m_ItemIcon.rectTransform;
        rectTransform.pivot = new Vector2(1, 0f);
        rectTransform.anchoredPosition = new Vector2(229.3f, 38);
        rectTransform.sizeDelta = new Vector2(346.8f, 338.4f);
    }

    private void SetIconLayout_CurrencyBundle()
    {
        RectTransform rectTransform = m_ItemIcon.rectTransform;
        rectTransform.pivot = new Vector2(0.5f, 0f);
        rectTransform.sizeDelta = new Vector2(385f, 372.5f);
        rectTransform.anchoredPosition = new Vector2(143.3996f, 28f);
    }

    public void Setup(StoreItem item, object data)
    {
        m_IconId = item.id;
        m_ItemIcon.overrideSprite = null;
        IconLoaded = false;

        if (data is CosmeticData)
            Setup(item, data as CosmeticData);
        else if (data is CurrencyBundleData)
            Setup(item, (CurrencyBundleData)data);
        else if (data is ConsumableData)
            Setup(item, (ConsumableData)data);
        else if (data is ItemData[])
            Setup(item, data as ItemData[]);
    }

    public void Setup(StoreItem item, CosmeticData cosmetic)
    {
        Setup(item);
        SetIconLayout_Cosmetic();
        SetupButton(item, cosmetic, StoreManagerAPI.TYPE_COSMETIC);

        m_IconId = cosmetic.iconId;

        bool locked = false;
        string locked_tooltip = null;
        //check unlocks
        //spirit unlock
        //school mastery unlock
        if (string.IsNullOrEmpty(item.tooltip) == false)
            locked = true;

        if (locked)
        {
            m_BuyIcon.sprite = m_GreySprite;
            m_BuyText.text = LocalizeLookUp.GetText("store_gear_locked_upper");
            locked_tooltip = LocalizeLookUp.GetText("item.tooltip");
        }
        else
        {
            bool owned = PlayerDataManager.playerData.inventory.cosmetics.Exists(cos => cos.id == item.id);
            if (owned)
            {
                m_BuyIcon.sprite = m_GreenSprite;
                m_BuyText.text = LocalizeLookUp.GetText("store_gear_owned_upper");
            }
            //m_BuyButton.onClick.AddListener(() => { onClick(item, this); });
        }

        m_BuyButton.onClick.RemoveAllListeners();
        m_BuyButton.onClick.AddListener(() =>
        {
            LoadingOverlay.Show();
            UIStorePurchaseCosmetic.Show(
                item,
                cosmetic,
                m_ItemIcon,
                locked_tooltip,
                (error) =>
                {
                    LoadingOverlay.Hide();
                    UIStorePurchase.Close();

                    if (string.IsNullOrEmpty(error))
                    {
                        Setup(item, cosmetic);
                        Debug.LogError("TODO: SHOW PURCHASE SUCCESS");
                    }
                    else
                    {
                        UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                    }
                });
        });
    }
    
    public void Setup(StoreItem item, CurrencyBundleData currency)
    {
        Setup(item);
        m_BuyIcon.gameObject.SetActive(false);
        SetIconLayout_CurrencyBundle();
        SetupButton(item, currency, StoreManagerAPI.TYPE_CURRENCY);

        if (currency.silver > 0)
        {
            m_ItemAmount.text = currency.silver.ToString();
            if (currency.silverBonus == 0)
            {
                m_Tag.SetActive(false);
            }
            else
            {
                m_TagText.text = "+" + currency.silverBonus;
                m_Tag.SetActive(true);
            }
        }
        else if (currency.gold > 0)
        {
            m_ItemAmount.text = currency.gold.ToString();

            if (currency.goldBonus == 0)
            {
                m_Tag.SetActive(false);
            }
            else
            {
                m_TagText.text = "+" + currency.goldBonus;
                m_Tag.SetActive(true);
            }
        }

        UnityEngine.Purchasing.Product iap = IAPSilver.instance.GetProduct(item.id);

        if (iap == null)
        {
            Debug.LogException(new System.Exception("product not found for \"" + item.id + "\""));
            m_SilverCost.text = "$ " + currency.cost.ToString();
        }
        else
        {
            m_SilverCost.text = iap.metadata.localizedPriceString;
        }
        m_SilverCost.gameObject.SetActive(true);
        m_SilverIcon.enabled = false;


    }

    public void Setup(StoreItem item, ConsumableData consumable)
    {
        Setup(item);
        SetIconLayout_Cosmetic();
        SetupButton(item, consumable, StoreManagerAPI.TYPE_ELIXIRS);
    }

    public void Setup(StoreItem item, ItemData[] ingredients)
    {
        Setup(item);
        SetIconLayout_IngredientBundle();
        SetupButton(item, ingredients, StoreManagerAPI.TYPE_INGREDIENT_BUNDLE);
    }

    private void Setup(StoreItem item)
    {
        name = "[UIStoreItem] " + item.id;

        m_ItemTitle.text = LocalizeLookUp.GetStoreTitle(item.id);
        m_ItemSubtitle.text = LocalizeLookUp.GetStoreSubtitle(item.id);
        m_ItemAmount.text = "";
        m_ItemIcon.overrideSprite = null;
        m_Tag.SetActive(false);

        m_SilverCost.text = item.silver.ToString();
        m_GoldCost.text = item.gold.ToString();

        m_SilverCost.gameObject.SetActive(item.silver > 0);
        m_SilverIcon.enabled = m_SilverCost.gameObject.activeSelf;

        m_GoldCost.gameObject.SetActive(item.gold > 0);
        m_GoldIcon.enabled = m_SilverCost.gameObject.activeSelf;

        m_CostOr.gameObject.SetActive(item.silver > 0 && item.gold > 0);
        m_BuyText.text = item.silver == 0 && item.gold == 0 ? LocalizeLookUp.GetText("store_claim").ToUpperInvariant() : LocalizeLookUp.GetText("store_buy_upper");

        m_CostLayout.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_CostLayout.GetComponent<RectTransform>());
        m_CostLayout.enabled = false;
    }

    private void SetupButton(StoreItem item, object data, string type)
    {
        m_BuyIcon.sprite = m_RedSprite;
        m_BuyButton.onClick.RemoveAllListeners();
        m_BuyButton.onClick.AddListener(() =>
        {
            LoadingOverlay.Show();
            UIStorePurchase.Show(
                item,
                type,
                LocalizeLookUp.GetStoreTitle(item.id),
                LocalizeLookUp.GetStoreDesc(item.id),
                m_ItemIcon,
                null,
                (error) =>
                {
                    LoadingOverlay.Hide();
                    UIStorePurchase.Close();

                    if (string.IsNullOrEmpty(error))
                    {
                        Setup(item, data);
                        Debug.LogError("TODO: SHOW PURCHASE SUCCESS");
                    }
                    else
                    {
                        UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                    }
                });
        });
    }
}
