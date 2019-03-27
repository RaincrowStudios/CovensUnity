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

    private UIInventoryWheel m_Wheel;
    public InventoryItems item { get; private set; }
    public IngredientDict itemData { get; private set; }
    public int index { get; private set; }

    private void Awake()
    {
        m_Button.onClick.AddListener(OnClick);
    }

    public void Setup(InventoryItems item, UIInventoryWheel wheel, int index)
    {
        m_Wheel = wheel;
        this.item = item;
        if (item != null)
            itemData = DownloadedAssets.GetCollectable(item.id);
        else 
            itemData = DownloadedAssets.GetCollectable(m_ItemId);
        this.index = index;

        Refresh();
    }

    public void Refresh()
    {
        if (item == null || itemData == null)
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
            m_Amount.text = item.count.ToString();
        }
    }

    private void OnClick()
    {
        m_Wheel.SelectItem(this.index);
    }
}
