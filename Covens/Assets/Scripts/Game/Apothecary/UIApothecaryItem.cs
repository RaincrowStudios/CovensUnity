using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIApothecaryItem : UIWheelItem
{
    [Header("ApothecaryItem")]
    [SerializeField] private Image m_pImage;

    public Item Consumable { get; private set; }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
    }

    public override void Setup(object data)
    {
        if (data is Item == false)
        {
            Debug.LogError("data is not a " + typeof(Item) + ". Is " + data.GetType() + " instead.");
            m_pImage.gameObject.SetActive(false);
            Consumable = null;
            return;
        }

        Consumable = data as Item;
        DownloadedAssets.GetSprite(
            Consumable.id, 
            spr =>
            {
                m_pImage.sprite = spr;
            },
            true
        );
        m_pImage.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        m_pImage.sprite = null;
    }
}
