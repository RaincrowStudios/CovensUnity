using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizeLookUp : MonoBehaviour
{
    public string prefix = "";
    public string id = "";
    public string suffix = "";

    Text t;
    void Start()
    {
        t = GetComponent<Text>();
        if (DownloadAssetBundle.isDictLoaded)
            RefreshText();
        LocalizationManager.OnChangeLanguage += RefreshText;
    }

    void OnDestroy()
    {
        LocalizationManager.OnChangeLanguage -= RefreshText;
    }
    
    void RefreshText()
    {
        t.text = prefix + GetText(id) + suffix;
    }
    
    public static string GetText(string id)
    {
        if (DownloadedAssets.localizedText.ContainsKey(id))
            return DownloadedAssets.localizedText[id];
        else
            return $"<{id}>";
    }
}
