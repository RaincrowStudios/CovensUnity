using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StoreItemDB
{
    public StoreItemModel[] list;

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }
}

[Serializable]
public partial class StoreItemModel
{
    public string Type;
    public string ID;
    public long Value;
    public long Amount;
    public long SilverPrice;
    public long GoldPrice;
    public string Iap;
    public long Offer;
    public string DisplayNameId;
    public string DescriptionId;
    public string Icon;

    // filled
    public EnumStoreType StoreTypeEnum;

    public void Cache()
    {
        try
        {
            StoreTypeEnum = string.IsNullOrEmpty(Type) ? EnumStoreType.None : (EnumStoreType)Enum.Parse(typeof(EnumStoreType), Type);
        }
        catch (Exception e) { Debug.LogError(e.Message); Debug.LogError(JsonUtility.ToJson(this)); }
    }
    
    public string DisplayName
    {
        get
        {
            return Oktagon.Localization.Lokaki.GetText(DisplayNameId);
        }
    }
    public string DisplayDescription
    {
        get
        {
            return Oktagon.Localization.Lokaki.GetText(DescriptionId);
        }
    }

}


public enum EnumStoreType
{
    None,
    Energy,
    Experience,
    Aptitude,
    Bundles,
    IAP,
    Cosmetics,
}
