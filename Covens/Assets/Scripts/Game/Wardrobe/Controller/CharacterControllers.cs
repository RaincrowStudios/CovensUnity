using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


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
            if (IsEquipped(EnumEquipmentSlot.CarryOnLeft) || IsEquipped(EnumEquipmentSlot.CarryOnRight))
                return HandMode.Censer;
            return HandMode.Relaxed;
        }
    }

    public List<WardrobeItemModel> EquippedItems
    {
        get
        {
            if(vEquippedItems == null)
            {
                SetDefaultCharacter();
            }
            return vEquippedItems;
        }
        set
        {
            vEquippedItems = value;
        }
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
        RemoveConflicts(pItem);
        EquippedItems.Add(pItem);
        Debug.Log("==> " + pItem.ToString());
    }
    public void Equip(List<WardrobeItemModel> vItem, bool bReplace = true)
    {
        for (int i = 0; i < vItem.Count; i++)
        {
            Equip(vItem[i], bReplace);
        }
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
            if (pItem.IDNotColored == EquippedItems[i].IDNotColored)
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
    public bool IsEquippedNotColored(WardrobeItemModel pItem)
    {
        return GetEquippedItemNotColored(pItem) != null;
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
            if (sID == EquippedItems[i].IDNotColored)
            {
                return EquippedItems[i];
            }
        }
        return null;
    }
    public string GetEquippedItemID(params EnumEquipmentSlot[] eSlots)
    {
        for (int i = 0; i < EquippedItems.Count; i++)
        {
            foreach(EnumEquipmentSlot eSlot in eSlots)
            {
                if (eSlot == EquippedItems[i].EquipmentSlotEnum)
                {
                    return EquippedItems[i].ID;
                }
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
    public WardrobeItemModel GetEquippedItemNotColored(WardrobeItemModel pItem)
    {
        for (int i = 0; i < EquippedItems.Count; i++)
        {
            if (pItem.IDNotColored == EquippedItems[i].IDNotColored)
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


    #region conflicts region

    public List<WardrobeItemModel> GetConflictList(WardrobeItemModel pItem)
    {
        List<WardrobeItemModel> vConflicts = new List<WardrobeItemModel>();
        for (int i = 0; i < EquippedItems.Count; i++)
        {
            if (pItem.Conflicts(EquippedItems[i]))
                vConflicts.Add(EquippedItems[i]);
        }
        return vConflicts;
    }

    public void RemoveConflicts(WardrobeItemModel pItem)
    {
        List<WardrobeItemModel> vConflicts = GetConflictList(pItem);
        for (int i = EquippedItems.Count -1; i >= 0 ; i--)
        {
            if (vConflicts.Contains(EquippedItems[i]))
            {
                EquippedItems.RemoveAt(i);
            }
        }
    }
    public bool Conflicts(WardrobeItemModel pItem)
    {
        List<WardrobeItemModel> vConflicts = GetConflictList(pItem);
        for (int i = EquippedItems.Count - 1; i >= 0; i--)
        {
            if (vConflicts.Contains(EquippedItems[i]))
            {
                return true;
            }
        }
        return false;
    }
    #endregion


    #region synch server

    public void SynchServer(Action<string> pSuccess, Action<string> pError)
    {
        Equipped pEquipped = new Equipped();
        pEquipped.hat = GetEquippedItemID(EnumEquipmentSlot.Head);
        pEquipped.hair = GetEquippedItemID(EnumEquipmentSlot.Hair);
        pEquipped.neck = GetEquippedItemID(EnumEquipmentSlot.Neck);
        pEquipped.dress = GetEquippedItemID(EnumEquipmentSlot.Chest);
        pEquipped.wristRight = GetEquippedItemID(EnumEquipmentSlot.WristRight);
        pEquipped.wristLeft = GetEquippedItemID(EnumEquipmentSlot.WristLeft);
        pEquipped.handRight = GetEquippedItemID(EnumEquipmentSlot.Hands);
        pEquipped.handLeft = GetEquippedItemID(EnumEquipmentSlot.Hands);
        pEquipped.fingerRight = GetEquippedItemID(EnumEquipmentSlot.FingerRight);
        pEquipped.fingerLeft = GetEquippedItemID(EnumEquipmentSlot.FingerLeft);
        pEquipped.waist = GetEquippedItemID(EnumEquipmentSlot.Waist);
        pEquipped.legs = GetEquippedItemID(EnumEquipmentSlot.Legs);
        pEquipped.feet = GetEquippedItemID(EnumEquipmentSlot.Feet);
        pEquipped.carryOns = GetEquippedItemID(EnumEquipmentSlot.CarryOnLeft, EnumEquipmentSlot.CarryOnRight) ;
        pEquipped.skinFace = GetEquippedItemID(EnumEquipmentSlot.SkinFace);
        pEquipped.skinShoulder = GetEquippedItemID(EnumEquipmentSlot.SkinShoulder);
        pEquipped.skinChes = GetEquippedItemID(EnumEquipmentSlot.SkinChest);
        InventoryAPI.Equip(pEquipped, pSuccess, pError);
    }
    public void Consume(string sItemId, int iAmount, Action<string> pSuccess, Action<string> pError)
    {
        InventoryAPI.Consume(sItemId, iAmount, pSuccess, pError);
    }

    public void Display(Action<string> pSuccess, Action<string> pError)
    {
        InventoryAPI.Display(pSuccess, pError);
    }
    #endregion

    #region char preparation

    /// <summary>
    /// builds the default character. It usually has Base and Hands
    /// </summary>
    public void SetDefaultCharacter()
    {
        //Display(null, null);
        //EquippedItems = ItemDB.Instance.GetDefaultItens(m_eGender);
        PrepareCharacter();
    }
    public void SetDefaultBody()
    {
        EquippedItems = ItemDB.Instance.GetDefaultItens(m_eGender);
    }
    public void PrepareCharacter()
    {
        
        //SetDefaultCharacter();
        Equipped pEquipped = PlayerDataManager.Instance.EquippedChar;
        if (pEquipped != null)
        {
            SetDefaultBody();
            EquipIfFind(pEquipped.hat);
            EquipIfFind(pEquipped.hair);
            EquipIfFind(pEquipped.neck);
            EquipIfFind(pEquipped.dress);
            EquipIfFind(pEquipped.wristRight);
            EquipIfFind(pEquipped.wristLeft);
            EquipIfFind(pEquipped.handRight);
            EquipIfFind(pEquipped.handLeft);
            EquipIfFind(pEquipped.fingerRight);
            EquipIfFind(pEquipped.fingerLeft);
            EquipIfFind(pEquipped.waist);
            EquipIfFind(pEquipped.legs);
            EquipIfFind(pEquipped.feet);
            EquipIfFind(pEquipped.carryOns);
            EquipIfFind(pEquipped.skinFace);
            EquipIfFind(pEquipped.skinShoulder);
            EquipIfFind(pEquipped.skinChes);
            return;
        }
        EquippedItems = ItemDB.Instance.GetDefaultItens(m_eGender);
    }
    void EquipIfFind(string sItemID)
    {
        if (string.IsNullOrEmpty(sItemID)) return;
        WardrobeItemModel pItem = ItemDB.Instance.GetItem(sItemID);
        if(pItem != null)
        {
            EquippedItems.Add(pItem);
        }
    }
    #endregion


    public List<WardrobeItemModel> GetRandomItens(List<WardrobeItemModel> vItens)
    {
        var values = Enum.GetValues(typeof(EnumEquipmentSlot));
        List<WardrobeItemModel> vRandomItens = new List<WardrobeItemModel>();
        foreach (EnumEquipmentSlot eSlot in values)
        {
            // ignore base
            if (eSlot == EnumEquipmentSlot.Base || eSlot == EnumEquipmentSlot.Hands)
                continue;

            // filter by slot
            List<WardrobeItemModel> vSlotItens = new List<WardrobeItemModel>();
            for (int i = 0; i < vItens.Count; i++)
            {
                if (vItens[i].EquipmentSlotEnum == eSlot)
                    vSlotItens.Add(vItens[i]);
            }

            // has not item for the slot
            if (vSlotItens.Count <= 0)
            {
                continue;
            }

            int iIdx = UnityEngine.Random.Range(0, vSlotItens.Count);
            vRandomItens.Add(vSlotItens[iIdx]);
        }
        return vRandomItens;
    }


}