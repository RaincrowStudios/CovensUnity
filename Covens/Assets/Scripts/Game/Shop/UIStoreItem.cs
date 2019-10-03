using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Store;
using EnhancedUI.EnhancedScroller;

public class UIStoreItem : MonoBehaviour
{
    [System.Serializable]
    public struct UIStoreItemCost
    {
        public TextMeshProUGUI text;
        public Image icon;
    }

    [SerializeField] private TextMeshProUGUI m_ItemTitle;
    [SerializeField] private Image m_ItemIcon;
    [SerializeField] private TextMeshProUGUI m_ItemSubtitle;
    [SerializeField] private TextMeshProUGUI m_ItemAmount;

    [Header("Cost")]
    [SerializeField] private UIStoreItemCost m_CostA;
    [SerializeField] private UIStoreItemCost m_CostB;
    [SerializeField] private TextMeshProUGUI m_CostOr;
    [SerializeField] private Sprite m_SilverIcon;
    [SerializeField] private Sprite m_GoldIcon;

    [Header("Buy")]
    [SerializeField] private Button m_BuyButton;
    [SerializeField] private TextMeshProUGUI m_BuyText;
    [Space()]
    [SerializeField] private Sprite m_GreenSprite;
    [SerializeField] private Sprite m_RedSprite;

    [Header("Tag")]
    [SerializeField] private GameObject m_Tag;
    [SerializeField] private TextMeshProUGUI m_TagText;


    public void LoadIcon(string id)
    {
        DownloadedAssets.GetSprite(id, m_ItemIcon, true);
    }

    public void Setup(StoreItem item, object data)
    {
        if (data is CosmeticData)
            Setup(item, data as CosmeticData);
        else if (data is CurrencyBundleData)
            Setup(item, (CurrencyBundleData)data);
    }

    public void Setup(StoreItem item, CosmeticData cosmetic)
    {
        m_ItemTitle.text = LocalizeLookUp.GetStoreTitle(item.id);
        m_ItemSubtitle.text = "";
        m_ItemAmount.text = "";        
        m_ItemIcon.overrideSprite = null;
        m_BuyButton.onClick.RemoveAllListeners();
        m_Tag.SetActive(false);
        SetIconLayoutA();

        if (item.silver > 0)
        {
            SetCost(item.silver, 0, false);
            SetCost(item.gold, 1, true);
        }
        else
        {
            SetCost(item.gold, 0, true);
            SetCost(item.silver, 1, false);
        }

        bool locked = false;
        //check unlocks
        //spirit unlock
        //school mastery unlock
        if (string.IsNullOrEmpty(item.tooltip) == false)
            locked = true;

        if (locked)
        {
            m_BuyButton.image.sprite = m_RedSprite;
            m_BuyText.text = LocalizeLookUp.GetText("store_gear_locked_upper");
            m_BuyButton.onClick.AddListener(() =>
            {
                UIGlobalPopup.ShowPopUp(
                    null,
                    LocalizeLookUp.GetText("shop_condition_locked") + "\n" + LocalizeLookUp.GetText(item.tooltip));
            });
        }
        else
        {
            bool owned = PlayerDataManager.playerData.inventory.cosmetics.Exists(cos => cos.id == item.id);
            m_BuyText.text = owned ? LocalizeLookUp.GetText("store_gear_owned_upper")/*"OWNED"*/ : LocalizeLookUp.GetText("store_buy_upper");//"BUY";
            m_BuyButton.image.sprite = owned ? m_GreenSprite : m_RedSprite;
            //m_BuyButton.onClick.AddListener(() => { onClick(item, this); });
        }
    }
    
    public void Setup(StoreItem item, CurrencyBundleData currency)
    {
        m_ItemTitle.text = LocalizeLookUp.GetStoreTitle(item.id);
        m_ItemSubtitle.text = "";
        m_BuyButton.gameObject.SetActive(false);
        m_ItemIcon.overrideSprite = null;
        SetIconLayoutB();

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
            m_CostA.text.text = "U$ " + currency.cost.ToString();
        }
        else
        {
            m_CostA.text.text = iap.metadata.localizedPriceString;
        }

        SetCost(0, 1, null);
    }

    //public void Setup(StoreItem item, ConsumableData consumable)
    //{

    //}

    //public void Setup(StoreItem item, List<ItemData> ingredients)
    //{

    //}

    private void SetCost(int amount, int idx, bool? isGold = null)
    {
        TextMeshProUGUI text;
        Image icon;

        if (idx == 0)
        {
            text = m_CostA.text;
            icon = m_CostA.icon;
        }
        else
        {
            text = m_CostB.text;
            icon = m_CostB.icon;
        }

        if (amount > 0)
        {
            text.text = amount.ToString();
            if (isGold.HasValue)
                icon.overrideSprite = isGold.Value ? m_GoldIcon : m_SilverIcon;
            else
                icon.overrideSprite = null;

            if (idx > 0)
                m_CostOr.gameObject.SetActive(true);
        }
        else
        {
            text.text = "";
            icon.overrideSprite = null;

            if (idx > 0)
                m_CostOr.gameObject.SetActive(false);
        }
    }

    private void SetIconLayoutA()
    {
        RectTransform rectTransform = m_ItemIcon.rectTransform;
        rectTransform.pivot = new Vector2(1, 0.5f);
        rectTransform.anchoredPosition = new Vector2(182.9999f, 2.600006f);
        rectTransform.sizeDelta = new Vector2(251f, 364.5f);
    }

    private void SetIconLayoutB()
    {
        RectTransform rectTransform = m_ItemIcon.rectTransform;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(135.0002f, -1f);
        rectTransform.sizeDelta = new Vector2(390f, 376f);
    }
}
