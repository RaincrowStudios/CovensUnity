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
}
