using System.IO;
using UnityEngine;
using System;
using System.Net;
using Newtonsoft.Json;

public class DictionaryManager
{
    public static string version = "87";
    public static string baseURL = "https://storage.googleapis.com/raincrow-covens/dictionary/";
    static string[] lng = new string[] { "English", "Portuguese", "Spanish", "Japanese", "German", "Russian" };

    static int language
    {
        get { return PlayerPrefs.GetInt("Language", 0); }
        set { PlayerPrefs.SetInt("Language", value); }
    }

    public static void GetDictionary()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;
#endif
        string filename = "dict.text";
        string localDictionaryPath = Path.Combine(Application.persistentDataPath, filename);

        if (PlayerPrefs.HasKey("DataDict"))
        {
            string currentDictionary = PlayerPrefs.GetString("DataDict");
            if (currentDictionary == version)
            {
                if (System.IO.File.Exists(localDictionaryPath))
                {
                    Debug.Log($"\"{version}\" already downloaded.");
                    string json = System.IO.File.ReadAllText(localDictionaryPath);
                    DownloadAssetBundle.Instance.SaveDict(JsonConvert.DeserializeObject<DictMatrixData>(json));
                    return;
                }
                else
                {
                    Debug.Log($"Dictionary \"{version}\" is marked as download but not found.");
                }
            }
            else
            {
                Debug.Log($"Dictionary \"{currentDictionary}\" outdated.");
            }
        }
        else
        {
            Debug.Log("No dictionary found");
        }
        DownloadDictionary();
    }

    private async static void DownloadDictionary()
    {
        using (var webClient = new WebClient())
        {
            var url = new Uri(baseURL + version + "/" + lng[language] + ".json");
            Debug.Log(url);
            try
            {
                string result = await webClient.DownloadStringTaskAsync(url);
                Debug.Log("Loaded Dictionary");
                DownloadAssetBundle.Instance.SaveDict(JsonConvert.DeserializeObject<DictMatrixData>(result));
            }
            catch (Exception)
            {
                Debug.LogError("Error in getting dictionary");
                throw;
            }

        }
    }

}