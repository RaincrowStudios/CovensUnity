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
    [SerializeField] private TextMeshProUGUI m_BuyText2;
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

        m_ItemIcon.preserveAspect = true;
    }

    private void SetIconLayout_IngredientBundle()
    {
        RectTransform rectTransform = m_ItemIcon.rectTransform;
        rectTransform.pivot = new Vector2(1, 0f);
        rectTransform.anchoredPosition = new Vector2(229.3f, 38);
        rectTransform.sizeDelta = new Vector2(346.8f, 338.4f);

        m_ItemIcon.preserveAspect = true;
    }

    private void SetIconLayout_CurrencyBundle()
    {
        RectTransform rectTransform = m_ItemIcon.rectTransform;
        rectTransform.pivot = new Vector2(0.5f, 0f);
        rectTransform.sizeDelta = new Vector2(385f, 372.5f);
        rectTransform.anchoredPosition = new Vector2(143.3996f, 28f);

        m_ItemIcon.preserveAspect = true;
    }

    private void SetIconLayout_Full()
    {
        RectTransform rectTransform = m_ItemIcon.rectTransform;
        rectTransform.pivot = new Vector2(0.5f, 0f);
        rectTransform.sizeDelta = new Vector2(546.7f, 391.5f);
        rectTransform.anchoredPosition = new Vector2(276.2f, 16);

        m_ItemIcon.preserveAspect = false;
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
        SetupButton(item, cosmetic, LocalizeLookUp.GetText(item.id), StoreManagerAPI.TYPE_COSMETIC);

        m_IconId = cosmetic.iconId;

        bool locked = false;
        string locked_tooltip = null;
        //check unlocks
        //spirit unlock
        //school mastery unlock
        //if (string.IsNullOrEmpty(item.tooltip) == false)
        //    locked = true;

        if (Utilities.TimespanFromJavaTime(item.unlockOn).TotalMinutes > 0)
        {
            locked = true;
        }
        else if (cosmetic.position == "carryOnRight" || cosmetic.position == "carryOnLeft")
        {
            if (item.id == "cosmetic_f_CR_SHADOW" && PlayerDataManager.playerData.degree != -14)
                locked = true;
            else if (item.id == "cosmetic_f_CR_GRAY")
                locked = true;
            else if (item.id == "cosmetic_f_CR_WHITE" && PlayerDataManager.playerData.degree != 14)
                locked = true;
            else if (string.IsNullOrEmpty(item.tooltip) == false)
                locked = true;
            else if (string.IsNullOrEmpty(item.tooltip) == false)
                locked = true;
        }
        else if (cosmetic.position == "petFeet")
        {
            string spirit = null;
            switch (item.id)
            {
                case "cosmetic_f_PF_SENTINALOWL": spirit = "spirit_sentinelOwl"; break;
                case "cosmetic_m_PF_SENTINALOWL": spirit = "spirit_sentinelOwl"; break;
                case "cosmetic_f_PF_WENDIGO": spirit = "spirit_wendigo"; break;
                case "cosmetic_m_PF_WENDIGO": spirit = "spirit_wendigo"; break;
                case "cosmetic_f_PF_BARGHEST": spirit = "spirit_barghest"; break;
                case "cosmetic_m_PF_BARGHEST": spirit = "spirit_barghest"; break;
                case "cosmetic_f_PF_GRINDYLOW": spirit = "spirit_grindylow"; break;
                case "cosmetic_m_PF_GRINDYLOW": spirit = "spirit_grindylow"; break;
                case "cosmetic_f_PF_CATSIDHE": spirit = "spirit_catSidhe"; break;
                case "cosmetic_m_PF_CATSIDHE": spirit = "spirit_catSidhe"; break;
            }

            if (string.IsNullOrEmpty(spirit) == false && PlayerDataManager.playerData.knownSpirits.Exists(spr => spr.spirit == spirit) == false)
                locked = true;
        }
        else if (string.IsNullOrEmpty(item.tooltip) == false)
        {
            locked = true;
        }

        if (locked)
        {
            m_BuyIcon.overrideSprite = m_GreySprite;
            m_BuyText.text = LocalizeLookUp.GetText("store_gear_locked_upper");
            locked_tooltip = LocalizeLookUp.GetText(item.tooltip);
        }
        else
        {
            bool owned = PlayerDataManager.playerData.inventory.cosmetics.Exists(cos => cos.id == item.id);
            if (owned)
            {
                m_BuyIcon.overrideSprite = m_GreenSprite;
                m_BuyText.text = locked_tooltip = LocalizeLookUp.GetText("store_gear_owned_upper");
            }
        }

        m_BuyButton.onClick.RemoveAllListeners();
        m_BuyButton.onClick.AddListener(() =>
        {
            LoadingOverlay.Show();
            UIStorePurchaseCosmetic.Show(
                item,
                cosmetic,
                m_ItemIcon.overrideSprite,
                locked_tooltip,
                (error) =>
                {
                    LoadingOverlay.Hide();
                    UIStorePurchaseCosmetic.Close();

                    if (string.IsNullOrEmpty(error))
                    {
                        Sprite icon = m_ItemIcon.overrideSprite;
                        Setup(item, cosmetic);
                        m_ItemIcon.overrideSprite = icon;

                        UIStorePurchaseSuccess.Show(
                            LocalizeLookUp.GetStoreTitle(item.id),
                            "",
                            icon,
                            null);
                    }
                    else
                    {
                        UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                    }
                });
        });
    }
    
    public void Setup(string id, PackData data)
    {
        m_ItemTitle.text = LocalizeLookUp.GetStoreTitle(id);
        m_ItemSubtitle.text = "";
        m_ItemAmount.text = "";
        m_SilverCost.text = "";
        m_GoldCost.text = "";

        name = "[UIStoreItem] " + id + " - " + m_ItemTitle.text;

        m_Tag.SetActive(false);
        m_SilverCost.gameObject.SetActive(false);
        m_GoldCost.gameObject.SetActive(false);
        m_CostOr.SetActive(false);

        SetIconLayout_Full();

        m_ItemIcon.overrideSprite = null;
        //DownloadedAssets.GetSprite(
        //    id + "_icon",
        //    spr => m_ItemIcon.overrideSprite = spr,
        //    true
        //);
        m_IconId = id + "_icon";

        bool claimed = PlayerDataManager.playerData.OwnedPacks.Contains(id);
        m_BuyIcon.gameObject.SetActive(false);
        m_BuyText2.text = claimed ? LocalizeLookUp.GetText("store_gear_owned_upper") : LocalizeLookUp.GetText("claim_gift").ToUpper();
        m_BuyButton.onClick.RemoveAllListeners();

        if (!claimed)
        {
            m_BuyButton.onClick.AddListener(() =>
            {
                LoadingOverlay.Show();
                StoreManagerAPI.Purchase(id, StoreManagerAPI.TYPE_PACK, null, 
                    (error) => 
                    {
                        LoadingOverlay.Hide();
                        if (string.IsNullOrEmpty(error))
                        {
                            var aux = m_ItemIcon.overrideSprite;
                            Setup(id, data);
                            m_ItemIcon.overrideSprite = aux;

                            UIStorePurchaseSuccess.Show(data.content);
                        }
                        else if (string.IsNullOrEmpty(error) == false)
                        {
                            if (error.StartsWith("PurchaseFailureReason"))
                            {
                                UnityEngine.Purchasing.PurchaseFailureReason reason = (UnityEngine.Purchasing.PurchaseFailureReason)int.Parse(error.Replace("PurchaseFailureReason", ""));
                                if (reason != UnityEngine.Purchasing.PurchaseFailureReason.UserCancelled)
                                    UIGlobalPopup.ShowError(null, reason.ToString());
                            }
                            else
                            {
                                UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                            }
                        }
                    });
            });
        }
    }

    public void Setup(StoreItem item, CurrencyBundleData currency)
    {
        Setup(item);
        m_BuyIcon.gameObject.SetActive(false);
        SetIconLayout_CurrencyBundle();
        SetupButton(item, currency, LocalizeLookUp.GetText(item.id), StoreManagerAPI.TYPE_CURRENCY);

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

        UnityEngine.Purchasing.Product iap = IAPSilver.instance.GetProduct(currency.product);

        if (iap == null)
        {
            Debug.LogException(new System.Exception("product not found for \"" + currency.product + "\""));
            m_SilverCost.text = "$ " + currency.cost.ToString();
            gameObject.SetActive(false);
            return;
        }
        else
        {
            m_SilverCost.text = iap.metadata.localizedPriceString;
        }
        m_SilverCost.gameObject.SetActive(true);
        m_SilverIcon.enabled = false;

        m_BuyButton.onClick.RemoveAllListeners();
        m_BuyButton.onClick.AddListener(() =>
        {
            LoadingOverlay.Show();
            IAPSilver.instance.BuyProductID(item.id, (error) =>
            {
                LoadingOverlay.Hide();
                if (string.IsNullOrEmpty(error))
                {
                    UIStorePurchaseSuccess.Show(
                        LocalizeLookUp.GetStoreTitle(item.id),
                        m_ItemAmount.text,
                        m_ItemIcon.overrideSprite,
                        null);
                }
                else if (string.IsNullOrEmpty(error) == false)
                {
                    if (error.StartsWith("PurchaseFailureReason"))
                    {
                        UnityEngine.Purchasing.PurchaseFailureReason reason = (UnityEngine.Purchasing.PurchaseFailureReason)int.Parse(error.Replace("PurchaseFailureReason", ""));
                        if (reason != UnityEngine.Purchasing.PurchaseFailureReason.UserCancelled)
                            UIGlobalPopup.ShowError(null, reason.ToString());
                    }
                    else
                    {
                        UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                    }
                }
            });
        });
    }

    public void Setup(StoreItem item, ConsumableData consumable)
    {
        int amount = 0;
        foreach (var elixir in PlayerDataManager.playerData.inventory.consumables)
        {
            if (elixir.id == item.id)
            {
                amount = elixir.count;
                break;
            }
        }

        Setup(item);
        SetIconLayout_Cosmetic();
        SetupButton(
            item, 
            consumable, 
            LocalizeLookUp.GetStoreTitle(item.id),
            StoreManagerAPI.TYPE_ELIXIRS,
            "\n" + $" ({LocalizeLookUp.GetText("store_gear_owned_upper")}: {(amount == 0 ? LocalizeLookUp.GetText("lt_none") : amount.ToString())})"
            );

    }

    public void Setup(StoreItem item, ItemData[] ingredients)
    {
        Setup(item);
        SetIconLayout_IngredientBundle();
        SetupButton(item, ingredients, LocalizeLookUp.GetStoreTitle(item.id), StoreManagerAPI.TYPE_INGREDIENT_BUNDLE);
    }

    private void Setup(StoreItem item)
    {
        m_ItemTitle.text = LocalizeLookUp.GetStoreTitle(item.id);
        m_ItemSubtitle.text = LocalizeLookUp.GetStoreSubtitle(item.id);
        m_ItemAmount.text = "";
        m_ItemIcon.overrideSprite = null;
        m_Tag.SetActive(false);
        m_BuyIcon.gameObject.SetActive(true);

        name = "[UIStoreItem] " + item.id + " - " + m_ItemTitle.text;

        m_SilverCost.text = item.silver.ToString();
        m_GoldCost.text = item.gold.ToString();

        m_SilverCost.color = PlayerDataManager.playerData.silver >= item.silver ? Color.white : Color.red;
        m_GoldCost.color = PlayerDataManager.playerData.gold >= item.gold ? Color.white : Color.red;

        m_SilverCost.gameObject.SetActive(item.silver > 0);
        m_SilverIcon.enabled = m_SilverCost.gameObject.activeSelf;

        m_GoldCost.gameObject.SetActive(item.gold > 0);
        m_GoldIcon.enabled = m_GoldCost.gameObject.activeSelf;

        m_CostOr.gameObject.SetActive(item.silver > 0 && item.gold > 0);
        m_BuyText.text = item.silver == 0 && item.gold == 0 ? LocalizeLookUp.GetText("store_claim").ToUpper() : LocalizeLookUp.GetText("store_buy_upper");
        m_BuyText2.text = "";

        m_CostLayout.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_CostLayout.GetComponent<RectTransform>());
        m_CostLayout.enabled = false;
    }

    private void SetupButton(StoreItem item, object data, string title, string type, string description = "")
    {
        m_BuyIcon.overrideSprite = m_RedSprite;
        m_BuyButton.onClick.RemoveAllListeners();
        m_BuyButton.onClick.AddListener(() =>
        {
            LoadingOverlay.Show();
            UIStorePurchase.Show(
                item,
                type,
                title,
                LocalizeLookUp.GetStoreDesc(item.id) + description,
                m_ItemIcon.overrideSprite,
                null,
                (error) =>
                {
                    LoadingOverlay.Hide();
                    UIStorePurchase.Close();

                    if (string.IsNullOrEmpty(error))
                    {
                        Sprite icon = m_ItemIcon.overrideSprite;
                        Setup(item, data);
                        m_ItemIcon.overrideSprite = icon;

                        UIStorePurchaseSuccess.Show(
                            LocalizeLookUp.GetStoreTitle(item.id),
                            LocalizeLookUp.GetStoreSubtitle(item.id),
                            icon,
                            null);
                    }
                    else
                    {
                        UIGlobalPopup.ShowError(null, APIManager.ParseError(error));
                    }
                });
        });
    }
}
