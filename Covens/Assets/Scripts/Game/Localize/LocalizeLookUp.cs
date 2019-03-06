using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//[RequireComponent(typeof(Text))]
public class LocalizeLookUp : MonoBehaviour
{
    public string prefix = "";
    public string id = "";
    public string suffix = "";

    Text t;
    TextMeshProUGUI tmPro;
    bool isTmpro = true;
    void Start()
    {
        try
        {
            tmPro = GetComponent<TextMeshProUGUI>();
        }
        catch (System.Exception)
        {
            t = GetComponent<Text>();
            isTmpro = false;
        }
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
        if (tmPro)
            tmPro.text = prefix + GetText(id) + suffix;
        else
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
