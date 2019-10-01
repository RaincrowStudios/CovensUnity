using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Store;
using EnhancedUI.EnhancedScroller;

public class UIStoreItem : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI m_ItemTitle;
    [SerializeField] protected Image m_ItemIcon;
    [SerializeField] protected VerticalLayoutGroup m_CostLayoutGroup;
    [SerializeField] protected TextMeshProUGUI m_GoldCost;
    [SerializeField] protected TextMeshProUGUI m_SilverCost;
    [SerializeField] protected Button m_BuyButton;
    [SerializeField] protected TextMeshProUGUI m_BuyText;

    [Space()]
    [SerializeField] protected Sprite m_GreenSprite;
    [SerializeField] protected Sprite m_RedSprite;

    //private void OnEnable()
    //{
    //    transform.localScale = Vector3.one;
    //}

    //private void OnDisable()
    //{
    //    m_ItemIcon.overrideSprite = null;
    //}

    public void Setup(StoreItem item, CosmeticData cosmetic)
    {
        m_ItemTitle.text = LocalizeLookUp.GetStoreTitle(item.id);
        m_GoldCost.text = item.gold.ToString();
        m_SilverCost.text = item.silver.ToString();
        m_BuyButton.onClick.RemoveAllListeners();
        m_ItemIcon.overrideSprite = null;

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

    public void LoadIcon(string id)
    {
        DownloadedAssets.GetSprite(id, m_ItemIcon, true);
    }

    public void Setup(StoreItem item, CurrencyBundleData currency)
    {

    }

    //public void Setup(StoreItem item, ConsumableData consumable)
    //{

    //}

    //public void Setup(StoreItem item, List<ItemData> ingredients)
    //{

    //}
}
