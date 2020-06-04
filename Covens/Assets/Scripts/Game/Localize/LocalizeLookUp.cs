using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizeLookUp : MonoBehaviour
{
    public string prefix = "";
    public string id = "";
    public string suffix = "";
    public bool toUpper = false;

    void Start()
    {
        //if (DownloadAssetBundle.isDictLoaded)
        RefreshText();
        LocalizationManager.OnChangeLanguage += RefreshText;

        this.enabled = false;
    }

    void OnDestroy()
    {
        LocalizationManager.OnChangeLanguage -= RefreshText;
    }

    void RefreshText()
    {
        TMPro.TextMeshProUGUI textPro = GetComponent<TMPro.TextMeshProUGUI>();
        if (textPro != null)
        {
            if (toUpper)
                textPro.text = prefix + GetText(id).ToUpperInvariant() + suffix;
            else
                textPro.text = prefix + GetText(id) + suffix;
            return;
        }

        Text text = GetComponent<Text>();
        if (text != null)
        {
            text.text = prefix + GetText(id) + suffix;
        }
    }

    public static string GetText(string id)
    {
        if (DownloadedAssets.LocalizationDictionary.ContainsKey(id))
            return DownloadedAssets.LocalizationDictionary[id];
        else
            return $"<{id}>";
    }

    public static bool HasKey(string id)
    {
        return DownloadedAssets.LocalizationDictionary.ContainsKey(id);
    }

    //spell
    public static string GetSpellName(string id) => GetText(id + "_name");
    public static string GetSpellSpiritDescription(string id) => GetText(id + "_desc");
    public static string GetSpellPhyisicalDescription(string id) => GetText(id + "_desc_physical");
    public static string GetSpellLore(string id) => GetText(id + "_lore");

    //spirit
    public static string GetSpiritName(string id) => GetText(id + "_name");
    public static string GetSpiritBehavior(string id) => GetText(id + "_behavior");
    public static string GetSpiritDesc(string id) => GetText(id + "_desc");

    //collectable
    public static string GetCollectableName(string id) => GetText(id + "_name");
    public static string GetCollectableDesc(string id) => GetText(id + "_desc");

    //quests
    public static string GetExploreTitle(string id) => GetText(id + "_title");
    public static string GetExploreDesc(string id) => GetText(id + "_desc");
    public static string GetExploreLore(string id) => GetText(id + "_lore");

    //zones
    public static string GetZoneName(int id) => GetText("zone_" + id);
    public static string GetCountryName(string code) => GetText("country_" + code);

    //ftf dialogs
    public static string GetFtfDialog(int id) => GetText("ftf_" + id);

    //gardens
    public static string GetGardenName(string id) => GetText(id + "_name");
    public static string GetGardenDesc(string id) => GetText(id + "_desc");

    //condition
    public static string GetConditionDesc(string id) => GetText(id + "_condition");
    public static string GetConditionName(string id) => GetText(id + "_name");

    //Store
    public static string GetStoreTitle(string id) => GetText(id + "_title");
    public static string GetStoreSubtitle(string id) => (HasKey(id + "_subtitle") ? GetText(id + "_subtitle") : "");
    public static string GetStoreDesc(string id) => HasKey(id + "_desc") ? GetText(id + "_desc") : "";
    public static string GetStorePurchaseTitle(string id) => GetText(id + "_purchase");
    public static string GetStorePurchaseSuccess(string id) => GetText(id + "_consume");

}
