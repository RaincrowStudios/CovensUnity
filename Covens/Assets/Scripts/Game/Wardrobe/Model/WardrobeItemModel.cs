using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oktagon.Localization;
using System.Text.RegularExpressions;


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
    public string Conflict;

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
    private string[] m_vConflicts;

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

        if (!string.IsNullOrEmpty(Conflict))
        {
            m_vConflicts = Conflict.Replace(" ", "").Split(',');
            for (int i = 0; i < m_vConflicts.Length; i++)
            {
                m_vConflicts[i] = ParseToRegex(m_vConflicts[i]);
            }
        }
    }

    public bool IsEqual(WardrobeItemModel pModel)
    {
        return ID == pModel.ID;
    }
    public bool IsEqualNotColor(WardrobeItemModel pModel)
    {
        return IDNotColored == pModel.IDNotColored;
    }
    public bool Conflicts(WardrobeItemModel pModel)
    {
        if (m_vConflicts == null)
            return false;
        for (int i = 0; i < m_vConflicts.Length; i++)
        {
            if (Matches(pModel.ID, m_vConflicts[i]))
                return true;
        }
        return false;
    }

    [UnityEditor.MenuItem("Test/Regex  %g")]
    public static void asdfasdf()
    {
        string sValue = ParseToRegex(TestInputValues.String1());
        TestInputValues.String2(sValue);

        string[] s = { "f_E_B", "m_E_B", "f_EO_H", "m_EO_H", "f_HE_W_DWO", "f_HE_G_DWO", "f_HE_S_DWO", "f_HE_S_HDWO", "m_HE_W_DWO", "m_HE_G_DWO", "m_HE_S_DWO", "m_HE_S_HDWO", "m_HE_W_BKSO", "m_HE_G_BKSO", "m_HE_S_BKSO", "m_HE_W_KSO", "m_HE_G_KSO", "m_HE_S_KSO", "f_HA_W_BHDWO", "f_HA_W_DWO", "f_HA_G_FKFO", "f_HA_G_KSO", "f_N_W_DWO", "f_N_G_KSO", "m_N_G_KSO", "f_C_W_DWO", "f_C_G_DWO", "f_C_S_DWO", "f_C_W_KFO", "f_C_G_KFO", "f_C_S_KFO", "f_C_W_KSO", "f_C_G_KSO", "f_C_S_KSO", "m_C_W_DWO", "m_C_G_DWO", "m_C_S_DWO", "m_C_W_DSO", "m_C_G_DSO", "m_C_S_DSO", "m_C_W_HFO", "m_C_G_HFO", "m_C_S_HFO", "m_C_W_KSO", "m_C_G_KSO", "m_C_S_KSO", "m_C_W_DTO", "m_C_G_DTO", "m_C_S_DTO", "f_WL_S_DWO", "f_WL_G_KFO", "m_WL_W_DWO", "m_WL_G_DWO", "m_WL_S_DWO", "m_WL_W_KSO", "f_WR_W_DWO", "f_WR_G_KFO", "m_WR_W_DWO", "m_WR_G_DWO", "m_WR_S_DWO", "m_WR_G_KSO", "f_H_S_DWO", "f_FL_G_KSO", "f_FR_G_KSO", "f_L_G_DWO", "f_L_S_DWO", "f_L_W_KFO", "f_L_G_KFO", "f_L_S_KFO", "f_L_W_KSO", "f_L_G_KSO", "f_L_S_KSO", "m_L_W_DWO", "m_L_G_DWO", "m_L_S_DWO", "m_L_W_DSO", "m_L_G_DSO", "m_L_S_DSO", "m_L_W_HFO", "m_L_G_HFO", "m_L_S_HFO", "m_L_W_KSO", "m_L_G_KSO", "m_L_S_KSO", "m_L_W_DTO", "m_L_G_DTO", "m_L_S_DTO", "f_F_S_BDWO", "f_F_G_SBDWO", "f_F_W_SDWO", "f_F_S_KFO", "f_F_S_KSO", "m_F_G_DWO", "m_F_G_CHFO", "m_F_G_DSO", "m_F_G_DTO", "m_F_G_HFO", "m_CL_S_DWO", "f_CR_S_DWO", "f_SS_S_DWO", "m_SS_S_DWO", "f_SC_S_DWO", "m_SC_S_DWO" };
        string sLog = "";
        int iCount = 0;
        foreach (var sVal in s)
        {
            if (Matches(sVal, TestInputValues.String2()))
            {
                sLog += sVal + "\n";
                iCount++;
            }
        }
        Debug.Log(iCount + "/" + s.Length + " found.\n" + sLog);
    }

    public static string ParseToRegex(string sValue)
    {
        if (string.IsNullOrEmpty(sValue))
            return null;
        // add \b in end of the value
        sValue = !sValue.EndsWith("*") ? (sValue + @"\b") : sValue;
        sValue = !sValue.StartsWith("*") ? (@"\b" + sValue) : sValue;
        // replace * to (\w+?) that means any word
        sValue = sValue.Replace("*", @"(\w+?)");
        return sValue;
    }

    public static bool Matches(string sString, string sPattern)
    {
        string text = sString;
        Regex r = new Regex(sPattern, RegexOptions.IgnoreCase);
        Match m = r.Match(text);
        return m.Success;
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
