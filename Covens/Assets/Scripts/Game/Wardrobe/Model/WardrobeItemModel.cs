using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oktagon.Localization;

[Serializable]
public class WardrobeItemDB
{
    public WardrobeItemModel[] list;

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Test/Load")]
    public static void Load()
    {
        TextAsset pText = Resources.Load<TextAsset>("GameSettings/ItemDB.json");
        Debug.Log("Load: " + pText.text);
        WardrobeItemDB pDB = JsonUtility.FromJson<WardrobeItemDB>(pText.text);
        Debug.Log("success!");
        Debug.Log(pDB.ToString());
        // cache the variables
        foreach (var pItem in pDB.list)
            pItem.Cache();
    }
#endif
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
    public string Name
    {
        get
        {
            return ID;
        }
    }


    private bool m_bHasHandModes = false;
    private string m_sDisplayName = "";
    private string m_sDescription = "";
    private EnumWardrobeCategory m_eWardrobeCategory = EnumWardrobeCategory.None;
    private EnumEquipmentSlot m_eEquipmentSlotEnum = EnumEquipmentSlot.None;
    private EnumGender m_eGenderEnum = EnumGender.Undefined;


    public void Cache()
    {
        m_sDisplayName = !string.IsNullOrEmpty(DisplayNameId) ? Lokaki.GetText(DisplayNameId) : ID;
        m_sDescription = !string.IsNullOrEmpty(DescriptionId) ? Lokaki.GetText(DescriptionId) : ID;
        try
        {
            m_eWardrobeCategory = (EnumWardrobeCategory)Enum.Parse(typeof(EnumWardrobeCategory), Category);
        }
        catch (Exception e) { Debug.LogError("[" + Category + "]" + e.Message); }
        try
        {
            m_eEquipmentSlotEnum = (EnumEquipmentSlot)Enum.Parse(typeof(EnumEquipmentSlot), EquipmentSlot);
        }
        catch (Exception e) { Debug.LogError("[" + EquipmentSlot + "]" + e.Message); }
        try
        {
            m_eGenderEnum = (EnumGender)Enum.Parse(typeof(EnumGender), Gender);
        }
        catch (Exception e) { Debug.LogError(e.Message); }

        if (string.IsNullOrEmpty(Variation))
        {
            m_bHasHandModes = false;
        }
        else
        {
            m_bHasHandModes = Variation.Contains(HandMode.Censer.ToString());
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
