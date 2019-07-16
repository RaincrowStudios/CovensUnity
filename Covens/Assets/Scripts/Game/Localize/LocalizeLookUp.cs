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

    void Start()
    {
        if (DownloadAssetBundle.isDictLoaded)
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
        Text text = GetComponent<Text>();
        if (text != null)
        {
            text.text = prefix + GetText(id) + suffix;
        }
        else
        {
            TMPro.TextMeshProUGUI textPro = GetComponent<TMPro.TextMeshProUGUI>();
            if (textPro != null)
                textPro.text = prefix + GetText(id) + suffix;
        }
    }

    public static string GetText(string id)
    {
        if (DownloadedAssets.localizedText.ContainsKey(id))
            return DownloadedAssets.localizedText[id];
        else
            return $"<{id}>";
    }

    //spell
    public static string GetSpellName(string id) => GetText(id + "_name");
    public static string GetSpellSpiritDescription(string id) => GetText(id + "_desc");
    public static string GetSpellPhyisicalDescription(string id) => GetText(id + "_desc_physical");
    public static string GetSpellLore(string id) => GetText(id + "_lore");

    //spirit
    public static string GetSpiritName(MarkerSpawner.MarkerType id) => GetText(id.ToString().ToLower() + "_name");
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

    //Store
    public static string GetStoreTitle(string id) => GetText(id + "_title");
    public static string GetStoreSubtitle(string id) => GetText(id + "_subtitle");
    public static string GetStoreDesc(string id) => GetText(id + "_desc");
    public static string GetStorePurchaseTitle(string id) => GetText(id + "_purchase");
    public static string GetStorePurchaseSuccess(string id) => GetText(id + "_consume");

}
