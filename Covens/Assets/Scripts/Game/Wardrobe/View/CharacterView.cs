using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterView : MonoBehaviour
{
    [Serializable]
    public class ItemSlot
    {
        public string Name;
        public EnumEquipmentSlot m_Slot;
        public GameObject m_Root;
        public Image[] m_Images;
        public bool IsEquipped        {            get { return m_Root.activeSelf; }        }
    }


    public CharacterControllers m_Controller;
    public ItemSlot[] m_ItemSlot;


    private void Reset()
    {
        List<ItemSlot> vSlots = new List<ItemSlot>();
        var values = Enum.GetValues(typeof(EnumEquipmentSlot));
        foreach (EnumEquipmentSlot slot in values)
        {
            ItemSlot pItem = new ItemSlot();
            pItem.Name = slot.ToString();
            pItem.m_Slot = slot;
            pItem.m_Root = GameObject.Find(slot.ToString());
            if (pItem.m_Root == null)
            {
                Debug.Log("ROOT is empty: " + slot);
                continue;
            }
            else
                pItem.m_Images = pItem.m_Root.GetComponentsInChildren<Image>();
            vSlots.Add(pItem);
        }
        m_ItemSlot = vSlots.ToArray();
    }




    #region TESTs
    /*
    [ContextMenu("SetupTest")]
    public void TestSetup()
    {
        RandomItens(ItemDB.Instance.GetItens(m_Controller.m_eGender));
    }

    public void RandomItens(List<WardrobeItemModel> vItens)
    {
        List<WardrobeItemModel> vRandomItens = GetRandomItens(vItens);
        SetupChar(vRandomItens);
    }
    public List<WardrobeItemModel> GetRandomItens(List<WardrobeItemModel> vItens)
    {
        var values = Enum.GetValues(typeof(EnumEquipmentSlot));
        List<WardrobeItemModel> vRandomItens = new List<WardrobeItemModel>();
        foreach (EnumEquipmentSlot eSlot in values)
        {
            // ignore base
            if (eSlot == EnumEquipmentSlot.Base || eSlot == EnumEquipmentSlot.Hands)
                continue;
            
            List<WardrobeItemModel> vSlotItens = new List<WardrobeItemModel>();
            for(int i = 0; i < vItens.Count; i++)
            {
                if (vItens[i].EquipmentSlotEnum == eSlot)
                    vSlotItens.Add(vItens[i]);
            }

            bool bEquipped = UnityEngine.Random.Range(0, 100) <= 70;
            if (!bEquipped || vSlotItens.Count <= 0)
            {
                SetActivatedSlot(eSlot, false);
                continue;
            }
            
            int iIdx = UnityEngine.Random.Range(0, vSlotItens.Count);
            vRandomItens.Add(vSlotItens[iIdx]);
            //SetItem(vSlotItens[iIdx]);
        }
        return vRandomItens;
    }*/

    #endregion



    public void SetupChar()
    {
        SetupChar(m_Controller.EquippedItems);
    }
    public void SetupChar(List<WardrobeItemModel> vItemList)
    {
        SetActivatedItens(false);
        string sLog = "";
        for (int i = 0; i < vItemList.Count; i++)
        {
            List<Sprite> vTexture = ItemDB.Instance.GetTextures(vItemList[i], m_Controller.HandMode);
            SetItem(vItemList[i].EquipmentSlotEnum, vTexture);
            sLog += vItemList[i].Name + ", ";
        }
        Debug.Log("Set: " + sLog);
    }

    public void Unequip(WardrobeItemModel pItem)
    {
        m_Controller.Unequip(pItem);
        SetupChar();
    }
    public WardrobeItemModel Equip(WardrobeItemModel pItem)
    {
        WardrobeItemModel pRemoved = m_Controller.RemoveIfEquipped(pItem.EquipmentSlotEnum);
        m_Controller.Equip(pItem);
        SetupChar();
        return pRemoved;
    }
    public WardrobeItemModel SetItem(WardrobeItemModel pItem)
    {
        WardrobeItemModel pRemoved = m_Controller.RemoveIfEquipped(pItem.EquipmentSlotEnum);
        m_Controller.Equip(pItem);
        SetupChar();
        return pRemoved;
    }

    private void SetItem(EnumEquipmentSlot eSlot, List<Sprite> vImages)
    {
        if (vImages == null || vImages.Count <= 0)
            return;
        for (int i = 0; i < m_ItemSlot.Length; i++)
        {
            if (m_ItemSlot[i].m_Root == null)
                continue;
            if (m_ItemSlot[i].m_Slot == eSlot)
            {
                m_ItemSlot[i].m_Root.SetActive(true);
                for(int j = 0; j < m_ItemSlot[i].m_Images.Length; j++)
                {
                    if (m_ItemSlot[i].m_Images == null || j >= m_ItemSlot[i].m_Images.Length  || j >= vImages.Count  )
                        continue;
                    m_ItemSlot[i].m_Images[j].sprite = vImages[j];
                }
                break;
            }
        }
    }


    public void SetActivatedItens(bool bActivated)
    {
        for (int i = 0; i < m_ItemSlot.Length; i++)
        {
            if (m_ItemSlot[i].m_Root == null)
                continue;
            m_ItemSlot[i].m_Root.SetActive(bActivated);
        }
    }
    public void SetActivatedSlot(EnumEquipmentSlot eSlot, bool bActivated)
    {
        for (int i = 0; i < m_ItemSlot.Length; i++)
        {
            if (m_ItemSlot[i].m_Root == null)
                continue;
            if(m_ItemSlot[i].m_Slot == eSlot)
            {
                m_ItemSlot[i].m_Root.SetActive(bActivated);
            }
        }
    }

    public bool IsEquipped(WardrobeItemModel pItem)
    {
        return m_Controller.IsEquipped(pItem);
    }
    public bool IsEquipped(EnumEquipmentSlot eSlot)
    {
        return m_Controller.IsEquipped(eSlot);
    }

}