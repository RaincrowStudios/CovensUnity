using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Store;

public class UIStoreItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_ItemTitle;
    [SerializeField] private Image m_ItemIcon;
    [SerializeField] private VerticalLayoutGroup m_CostLayoutGroup;
    [SerializeField] private TextMeshProUGUI m_GoldCost;
    [SerializeField] private TextMeshProUGUI m_SilverCost;
    [SerializeField] private Button m_BuyButton;
    [SerializeField] private TextMeshProUGUI m_BuyText;

    [Space()]
    [SerializeField] private Sprite m_GreenButton;
    [SerializeField] private Sprite m_RedButton;
    
    public void Setup(StoreItem item, CosmeticData cosmetic)
    {
        m_ItemTitle.text = LocalizeLookUp.GetStoreTitle(item.id);
        m_GoldCost.text = item.gold.ToString();
        m_SilverCost.text = item.silver.ToString();
        m_BuyButton.onClick.RemoveAllListeners();

        bool locked = false;
        //check unlocks
        //spirit unlock
        //school mastery unlock
        if (string.IsNullOrEmpty(item.tooltip) == false)
            locked = true;

        if (locked)
        {
            m_BuyButton.image.sprite = m_RedButton;
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
            //m_BuyText.text = item.owned ? LocalizeLookUp.GetText("store_gear_owned_upper")/*"OWNED"*/ : LocalizeLookUp.GetText("store_buy_upper");//"BUY";
            //button.sprite = item.owned ? green : red;
            //m_BuyButton.onClick.AddListener(() => { onClick(item, this); });
        }
    }

    public void Setup(StoreItem item, CurrencyBundleData currency)
    {

    }

    public void Setup(StoreItem item, ConsumableData consumable)
    {

    }

    public void Setup(StoreItem item, List<ItemData> ingredients)
    {

    }
}
