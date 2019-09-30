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

    public void Setup(StoreItem item, CosmeticData cosmetic)
    {

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
