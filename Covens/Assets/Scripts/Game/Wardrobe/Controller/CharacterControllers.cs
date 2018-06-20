using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// the character controller. It is not the view, so only the core funcionalities are here
/// </summary>
public class CharacterControllers : MonoBehaviour
{
    public EnumGender m_eGender;
    private List<WardrobeItemModel> vEquippedItems;


    /// <summary>
    /// whitch kind of hand mode are we using?
    /// </summary>
    public HandMode HandMode
    {
        get
        {
            if (IsEquipped(EnumEquipmentSlot.CarryOn))
                return HandMode.Censer;
            return HandMode.Relaxed;
        }
    }

    public List<WardrobeItemModel> EquippedItems
    {
        get
        {
            return vEquippedItems;
        }
        set
        {
            vEquippedItems = value;
        }
    }

    private void Start()
    {
        SetDefaultCharacter();
    }



    /// <summary>
    /// equips the item
    /// </summary>
    /// <param name="pItem"></param>
    /// <returns></returns>
    public void Equip(WardrobeItemModel pItem, bool bReplace = true)
    {
        if (pItem == null)
            return;
        if (bReplace)
        {
            RemoveIfEquipped(pItem.EquipmentSlotEnum);
        }
        EquippedItems.Add(pItem);
    }

    /// <summary>
    /// unequips the item
    /// </summary>
    /// <param name="pItem"></param>
    /// <returns></returns>
    public bool Unequip(WardrobeItemModel pItem)
    {
        if (pItem == null)
            return false;

        for (int i = 0; i < EquippedItems.Count; i++)
        {
            if (pItem.ID == EquippedItems[i].ID)
            {
                EquippedItems.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
    public bool UnequipSlot(EnumEquipmentSlot eSlot)
    {
        return Unequip(GetEquippedItem(eSlot));
    }

    /// <summary>
    /// is the slot equiped?
    /// </summary>
    /// <param name="eSlot"></param>
    /// <returns></returns>
    public bool IsEquipped(EnumEquipmentSlot eSlot)
    {
        return GetEquippedItem(eSlot) != null;
    }
    public bool IsEquipped(WardrobeItemModel pItem)
    {
        return GetEquippedItem(pItem) != null;
    }

    /// <summary>
    /// get the equiped item based in its ID
    /// </summary>
    /// <param name="sID"></param>
    /// <returns></returns>
    public WardrobeItemModel GetEquippedItem(string sID)
    {
        for (int i = 0; i < EquippedItems.Count; i++)
        {
            if (sID == EquippedItems[i].ID)
            {
                return EquippedItems[i];
            }
        }
        return null;
    }
    /// <summary>
    /// gets the equipped item on the slot
    /// </summary>
    /// <param name="eSlot"></param>
    /// <returns></returns>
    public WardrobeItemModel GetEquippedItem(EnumEquipmentSlot eSlot)
    {
        for (int i = 0; i < EquippedItems.Count; i++)
        {
            if (eSlot == EquippedItems[i].EquipmentSlotEnum)
            {
                return EquippedItems[i];
            }
        }
        return null;
    }
    public WardrobeItemModel GetEquippedItem(WardrobeItemModel pItem)
    {
        for (int i = 0; i < EquippedItems.Count; i++)
        {
            if (pItem.ID == EquippedItems[i].ID)
            {
                return EquippedItems[i];
            }
        }
        return null;
    }
    public WardrobeItemModel RemoveIfEquipped(EnumEquipmentSlot eSlot)
    {
        for (int i = 0; i < EquippedItems.Count; i++)
        {
            if (eSlot == EquippedItems[i].EquipmentSlotEnum)
            {
                // unequip this to be replaced
                var pEquiped = EquippedItems[i];
                EquippedItems.RemoveAt(i);
                return pEquiped;
            }
        }
        return null;
    }


    /// <summary>
    /// builds the default character. It usually has Base and Hands
    /// </summary>
    public void SetDefaultCharacter()
    {
        EquippedItems = new List<WardrobeItemModel>();
        List<WardrobeItemModel> vHands = ItemDB.Instance.GetItens(EnumWardrobeCategory.Hand, m_eGender);
        List<WardrobeItemModel> vBases = ItemDB.Instance.GetItens(EnumWardrobeCategory.Body, m_eGender);
        if(vBases.Count > 0)
            Equip(vBases[0], false);
        if (vHands.Count > 0)
            Equip(vHands[0], false);
    }

}