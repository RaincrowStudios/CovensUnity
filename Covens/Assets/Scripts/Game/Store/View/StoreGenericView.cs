using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StoreGenericView : UIBaseAnimated

{
    public SimpleObjectPool m_ItemPool;
    public EnumStoreType[] m_StoreItems;
    public ScrollRect m_ScrollView;
    private List<StoreItem> m_WardrobeItemButtonCache = new List<StoreItem>();


    private void Start()
    {
        m_ItemPool.Setup();
    }

    public void SetupType(EnumStoreType[] vStoreItems)
    {
        m_StoreItems = vStoreItems;
    }

    public override void Show()
    {
        //base.Show();
        Invoke("ShowScheduled", .4f);
    }

    void ShowScheduled()
    {
        base.Show();
        var vItemList = StoreDB.Instance.GetItens(m_StoreItems);
        SetupItens(vItemList);// ItemDB.Instance.GetItens(PlayerDataManager.Instance.Gender));
        m_ScrollView.horizontalScrollbar.value = 0;
    }

    public override void DoShowAnimation()
    {
        base.DoShowAnimation();
    }

    public void SetupItens(List<StoreItemModel> vItens, bool bAnimate = true)
    {
        m_ItemPool.DespawnAll();
        //m_WardrobeItemButtonCache = new List<StoreItem>();
        for (int i = 0; i < vItens.Count; i++)
        {
            StoreItem pItemButton = GetStoreItem(vItens[i]);
            if (pItemButton == null)
            {
                pItemButton = m_ItemPool.SpawnNew<StoreItem>();
                m_WardrobeItemButtonCache.Add(pItemButton);
            }
            else
                pItemButton.gameObject.SetActive(true);
            pItemButton.Setup(vItens[i]);
        }
    }
    StoreItem GetStoreItem(StoreItemModel pItem)
    {
        if (m_WardrobeItemButtonCache != null)
        {
            for(int i =0; i < m_WardrobeItemButtonCache.Count; i++)
            {
                if (m_WardrobeItemButtonCache[i].ID == pItem.ID)
                    return m_WardrobeItemButtonCache[i];
            }
        }
        return null;
    }
}