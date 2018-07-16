using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// the character's view
/// </summary>
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

    [Header("Test. Call 'Json Test Equip'")]
    public string JsonTestEquip;

    public bool IsEquipped(WardrobeItemModel pItem)
    {
        return m_Controller.IsEquipped(pItem);
    }
    public bool IsEquippedNotColored(WardrobeItemModel pItem)
    {
        return m_Controller.IsEquippedNotColored(pItem);
    }
    public bool IsEquipped(EnumEquipmentSlot eSlot)
    {
        return m_Controller.IsEquipped(eSlot);
    }


    [ContextMenu("Json Test Equip")]
    public void JsonTestEquipCall()
    {
        Equipped pResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<Equipped>(JsonTestEquip);
        if(pResponseData!= null)
        {
            SetupChar(pResponseData);
            Debug.Log("Done");
        }
        else
        {
            Debug.LogError("Noooo");
        }
    }

    #region unity triggers

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
        m_Controller = gameObject.GetComponent<CharacterControllers>();
    }
    /*
    private void OnEnable()
    {
        DisableSlots();
    }*/

    #endregion

    
    #region equip/unequip

    /// <summary>
    /// equips the item and replace old one
    /// </summary>
    /// <param name="pItem"></param>
    public void Equip(WardrobeItemModel pItem)
    {
        WardrobeItemModel pRemoved = m_Controller.RemoveIfEquipped(pItem.EquipmentSlotEnum);
        m_Controller.Equip(pItem);
        SetupChar();
    }

    /// <summary>
    /// unequips the character by using the controller
    /// </summary>
    /// <param name="pItem"></param>
    public void Unequip(WardrobeItemModel pItem)
    {
        m_Controller.Unequip(pItem);
        SetupChar();
    }

    [ContextMenu("DisableSlots")]
    void DisableSlots()
    {
        for (int i = 0; i < m_ItemSlot.Length; i++)
        {
            m_ItemSlot[i].m_Root.SetActive(false);
            for (int j = 0; j < m_ItemSlot[i].m_Images.Length; j++)
            {
                m_ItemSlot[i].m_Images[j].gameObject.SetActive(false);
            }
        }
    }

    #endregion


    #region sets the item view
    public void SetupChar(Equipped pEquipped)
    {
        DisableSlots();
        m_Controller.SetEquippedChar(pEquipped);
        SetupChar(m_Controller.EquippedItems);
    }
    public void SetupChar()
    {
        DisableSlots();
        //m_Controller.SetDefaultCharacter();
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
    private void SetItem(EnumEquipmentSlot eSlot, List<Sprite> vImages)
    {
        if (vImages == null || vImages.Count <= 0)
            return;
        for (int i = 0; i < m_ItemSlot.Length; i++)
        {
            //bool bInUse = false;
            if (m_ItemSlot[i].m_Root != null && m_ItemSlot[i].m_Slot == eSlot)
            {
                //bInUse = true;
                for (int j = 0; j < m_ItemSlot[i].m_Images.Length; j++)
                {
                    if (m_ItemSlot[i].m_Images == null || j >= m_ItemSlot[i].m_Images.Length || j >= vImages.Count || vImages[j] == null)
                    {
                        m_ItemSlot[i].m_Images[j].gameObject.SetActive(false);
                        continue;
                    }
                    m_ItemSlot[i].m_Root.SetActive(true);
                    m_ItemSlot[i].m_Images[j].sprite = vImages[j];
                    m_ItemSlot[i].m_Images[j].gameObject.SetActive(true);
                }
                break;
            }
            //m_ItemSlot[i].m_Root.SetActive(bInUse);
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

    #endregion


    #region TESTs

    [ContextMenu("SetupTest")]
    public void SetRandomItems()
    {
        RandomItens(ItemDB.Instance.GetItens(m_Controller.m_eGender));
    }

    public void RandomItens(List<WardrobeItemModel> vItens)
    {
        List<WardrobeItemModel> vRandomItens = m_Controller.GetRandomItens(vItens);
        m_Controller.Equip(vRandomItens);
        SetupChar();
    }

    #endregion
}