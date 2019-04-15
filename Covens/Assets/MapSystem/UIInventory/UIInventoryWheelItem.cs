using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventoryWheelItem : MonoBehaviour
{
    [SerializeField] public string m_ItemId;
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Desc;
    [SerializeField] private TextMeshProUGUI m_Amount;
    [SerializeField] private GameObject m_AmountObject;
    [SerializeField] private Button m_Button;
    [SerializeField] private Transform m_IconReference;

    public Transform iconReference { get { return m_IconReference; } }

    private UIInventoryWheel m_Wheel;
    public InventoryItems inventoryItem { get; private set; }
    public IngredientDict itemData { get; private set; }
    public int index { get; private set; }
    public string type { get { return m_Wheel.type; } }

    private void Awake()
    {
        m_Button.onClick.AddListener(OnClick);
    }

    public void Setup(InventoryItems item, UIInventoryWheel wheel, int index)
    {
        this.m_Wheel = wheel;
        this.inventoryItem = item;
        this.index = index;

        if (item != null)
            itemData = DownloadedAssets.GetCollectable(item.id);
        else if (string.IsNullOrEmpty(m_ItemId) == false)
            itemData = DownloadedAssets.GetCollectable(m_ItemId);
		
		itemData.forbidden = itemData.type == "tool" && item.forbidden;
        Refresh();
    }

    public void Refresh()
    {
        if (inventoryItem == null || itemData == null)
        {
            if (m_Title)
                m_Title.text = "Empty";
            if (m_Desc)
                m_Desc.gameObject.SetActive(false);
            m_AmountObject.SetActive(false);
        }
        else
        {
            m_AmountObject.SetActive(true);

            if (m_Title)
                m_Title.text = itemData.name;
            if (m_Desc)
            {
                m_Desc.text = "Rarity (" + itemData.rarity.ToString() + ")";
                m_Desc.gameObject.SetActive(true);
            }
            m_Amount.text = inventoryItem.count.ToString();
        }
    }

    private void OnClick()
    {
        m_Wheel.SelectItem(this);
    }

    public void SetIngredientPicker(int amount)
    {
        m_Wheel.SetPicker(this, amount);

        if (inventoryItem != null)
            SetAmount(inventoryItem.count - amount);
    }

    public void ResetAmount()
    {
        if (itemData != null && inventoryItem != null)
            m_Amount.text = inventoryItem.count.ToString();
        else
            m_Amount.text = "";
    }

    public void SetAmount(int amount)
    {
        m_Amount.text = amount.ToString();
    }
}
