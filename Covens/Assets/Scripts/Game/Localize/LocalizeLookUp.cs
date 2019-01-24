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

    Text t;
    TextMeshProUGUI tmp;

    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            t = GetComponent<Text>();
            if (t == null)
                Destroy(this.gameObject);
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
        if (tmp)
            tmp.text = prefix + GetText(id) + suffix;
        else if (t)
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
