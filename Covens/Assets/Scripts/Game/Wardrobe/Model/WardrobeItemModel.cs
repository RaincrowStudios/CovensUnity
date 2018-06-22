using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oktagon.Localization;

[Serializable]
public class WardrobeItemDB
{
    public WardrobeItemModel[] list;

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}

[Serializable]
public class WardrobeItemModel 
{
    public string ID;
    public string DisplayNameId;
    public string DescriptionId;
    public string Stat;
    public string Price;
    public string Variation;
    public string EquipmentSlot;    // EquipmentSlot
    public string Category;         // WardrobeCategory
    public string Gender;           // Gender
    public string Alignment;        // Alignment
    public string IsDefault;

    private bool m_bHasHandModes = false;
    private string m_sDisplayName = "";
    private string m_sDescription = "";
    private EnumWardrobeCategory m_eWardrobeCategory = EnumWardrobeCategory.None;
    private EnumEquipmentSlot m_eEquipmentSlotEnum = EnumEquipmentSlot.None;
    private EnumGender m_eGenderEnum = EnumGender.Undefined;
    private EnumAlignment m_eAlignmentEnum = EnumAlignment.None;

    private string m_sGenderChar;
    private string m_sBodyPartChar;
    private string m_sColorChar;
    private string m_sOutfitNameChar;
    private string m_sIDNotColored;

    public void Cache()
    {
        try
        {
            m_eWardrobeCategory = string.IsNullOrEmpty(Category) ? EnumWardrobeCategory.None : (EnumWardrobeCategory)Enum.Parse(typeof(EnumWardrobeCategory), Category);
        }
        catch (Exception e) { Debug.LogError("[" + Category + "]" + e.Message); Debug.LogError(JsonUtility.ToJson(this)); }
        try
        {
            m_eEquipmentSlotEnum = (EnumEquipmentSlot)Enum.Parse(typeof(EnumEquipmentSlot), EquipmentSlot);
        }
        catch (Exception e) { Debug.LogError("[" + EquipmentSlot + "]" + e.Message); Debug.LogError(JsonUtility.ToJson(this)); }
        try
        {
            m_eGenderEnum = string.IsNullOrEmpty(Gender) ? EnumGender.Undefined :(EnumGender)Enum.Parse(typeof(EnumGender), Gender);
        }
        catch (Exception e) { Debug.LogError(e.Message); Debug.LogError(JsonUtility.ToJson(this)); }
        try
        {
            m_eAlignmentEnum = string.IsNullOrEmpty(Alignment) ? EnumAlignment.None : (EnumAlignment)Enum.Parse(typeof(EnumAlignment), Alignment);
        }
        catch (Exception e) { Debug.LogError(e.Message); Debug.LogError(JsonUtility.ToJson(this)); }
        if (string.IsNullOrEmpty(Variation))
        {
            m_bHasHandModes = false;
        }
        else
        {
            m_bHasHandModes = Variation.Contains(HandMode.Censer.ToString());
        }
        // collect char ids
        string[] vChars = ID.Split('_');
        try
        {
            m_sGenderChar = vChars[0];
            m_sBodyPartChar = vChars[1];
            m_sColorChar = vChars[2];
            m_sOutfitNameChar = EquipmentSlotEnum == EnumEquipmentSlot.BaseBody || EquipmentSlotEnum == EnumEquipmentSlot.BaseHand ? vChars[2] : vChars[3];
        }
        catch (Exception e) {
            Debug.LogError(ID + ": " + e.Message);
            Debug.LogError(JsonUtility.ToJson(this));
        }
        m_sIDNotColored = string.Format("{0}_{1}_{2}", m_sGenderChar, m_sBodyPartChar, m_sOutfitNameChar);
        m_sDisplayName = !string.IsNullOrEmpty(DisplayNameId) ? Lokaki.GetText(DisplayNameId) : OutfitNameChar;
        m_sDescription = !string.IsNullOrEmpty(DescriptionId) ? Lokaki.GetText(DescriptionId) : OutfitNameChar;
    }

    public bool IsEqual(WardrobeItemModel pModel)
    {
        return ID == pModel.ID;
    }
    public bool IsEqualNotColor(WardrobeItemModel pModel)
    {
        return IDNotColored == pModel.IDNotColored;
    }

    public string Name
    {
        get
        {
            return ID;
        }
    }
    public string IDNotColored
    {
        get
        {
            return m_sIDNotColored;
        }
    }
    public bool IsDefaultB
    {
        get { return IsDefault == "true"; }
    }
    public string OutfitNameChar
    {
        get
        {
            return m_sOutfitNameChar;
        }
    }
    public string DisplayName
    {
        get
        {
            return m_sDisplayName;
        }
    }
    public string Description
    {
        get
        {
            return m_sDescription;
        }
    }

    public EnumWardrobeCategory WardrobeCategory
    {
        get
        {
            return m_eWardrobeCategory;
        }
    }
    public EnumEquipmentSlot EquipmentSlotEnum
    {
        get
        {
            return m_eEquipmentSlotEnum;
        }
    }
    public EnumGender GenderEnum
    {
        get
        {
            return m_eGenderEnum;
        }
    }
    public EnumAlignment AlignmentEnum
    {
        get
        {
            return m_eAlignmentEnum;
        }
    }
    
    public bool HasHandModes
    {
        get
        {
            return m_bHasHandModes;
        }
    }
    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }


    public string[] GetTexturesName(HandMode eHandMode )
    {
        // single texture
        if (string.IsNullOrEmpty(Variation))
        {
            return new string[] { ID };
        }
        string[] vSplit;
        if (!HasHandModes)
        {
            // multi texture
            vSplit = Variation.Split(',');
            for (int i = 0; i < vSplit.Length; i++)
            {
                vSplit[i] = string.Format("{0}_{1}", ID, vSplit[i]);
                vSplit[i] = vSplit[i].Replace(" ", "");
            }
            return vSplit;
        }
        return new string[] { string.Format("{0}_{1}", ID, eHandMode.ToString()) };
    }

}
