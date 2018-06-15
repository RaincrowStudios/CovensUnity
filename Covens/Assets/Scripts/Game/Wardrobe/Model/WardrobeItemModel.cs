using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oktagon.Localization;

[Serializable]
public class WardrobeItemDB
{
    public WardrobeItemModel[] list;


    [UnityEditor.MenuItem("Test/Load")]
    public static void Load()
    {
        TextAsset pText = Resources.Load<TextAsset>("GameSettings/ItemDB.json");
        Debug.Log("Load: " + pText.text);
        WardrobeItemDB pDB = JsonUtility.FromJson<WardrobeItemDB>(pText.text);
        Debug.Log("success!");
        Debug.Log(pDB.ToString());
    }

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
    public string DisplayName
    {
        get
        {
            return Lokaki.GetText(DisplayNameId);
        }
    }
    public string Description
    {
        get
        {
            return Lokaki.GetText(DescriptionId);
        }
    }

    public EnumWardrobeCategory WardrobeCategory
    {
        get
        {
            try
            {
                EnumWardrobeCategory eCat = (EnumWardrobeCategory)Enum.Parse(typeof(EnumWardrobeCategory), Category);
                return eCat;
            }
            catch(Exception e) { Debug.LogError(e.Message); }
            return EnumWardrobeCategory.None;
        }
    }
    public EnumEquipmentSlot EquipmentSlotEnum
    {
        get
        {
            try
            {
                EnumEquipmentSlot eCat = (EnumEquipmentSlot)Enum.Parse(typeof(EnumEquipmentSlot), EquipmentSlot);
                return eCat;
            }
            catch (Exception e) { Debug.LogError(e.Message); }
            return EnumEquipmentSlot.None;
        }
    }
    public EnumGender GenderEnum
    {
        get
        {
            try
            {
                EnumGender eCat = (EnumGender)Enum.Parse(typeof(EnumGender), Gender);
                return eCat;
            }
            catch (Exception e) { Debug.LogError(e.Message); }
            return EnumGender.Undefined;
        }
    }
    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }


    public string[] GetTexturesName()
    {
        // single texture
        if (string.IsNullOrEmpty(Variation))
        {
            return new string[] { ID };
        }
        // multi texture
        string[] vSplit = Variation.Split(',');
        for (int i = 0; i < vSplit.Length; i++)
        {
            vSplit[i] = string.Format("{0}_{1}", ID, vSplit[i]);
            vSplit[i] = vSplit[i].Replace(" ", "");
        }
        return vSplit;
    }



    [UnityEditor.MenuItem("Test/WardrobeItemModel")]
    public static void Test()
    {
        WardrobeItemModel w = new WardrobeItemModel();
        w.ID = "m_WL_W_DWO";
        w.DisplayNameId = "321";
        w.DescriptionId = "321";
        w.Stat = "asdfasfd";
        w.Price = "adf";
        w.Category = "Censor";
        w.Gender = "Female";
        w.Alignment = "All";

        Debug.Log(JsonUtility.ToJson(w));
    }
}
