using System.IO;
using UnityEngine;
using System;
using System.Net;
using Newtonsoft.Json;
public class DictionaryManager
{
    public static string version = "87";
    private const string baseURL = "https://storage.googleapis.com/raincrow-covens/dictionary/";
    static string[] lng = new string[] { "English", "Portuguese", "Spanish", "Japanese", "German", "Russian" };
    static int tries = 0;
    static string filename = "dict.text";
    static string localDictionaryPath;

    public static bool DictionaryReady { get; private set; }

    static int language
    {
        get { return PlayerPrefs.GetInt("Language", 0); }
        set { PlayerPrefs.SetInt("Language", value); }
    }

    public static void GetDictionary(System.Action onDicionaryReady, System.Action<int, string> onDownloadError, System.Action onParseError)
    {
        DictionaryReady = false;
        tries = 0;

#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;
#endif

        localDictionaryPath = Path.Combine(Application.persistentDataPath, filename);
        if (PlayerPrefs.HasKey("DataDict"))
        {
            string currentDictionary = PlayerPrefs.GetString("DataDict");
            if (currentDictionary == version)
            {
                if (System.IO.File.Exists(localDictionaryPath))
                {
                    Debug.Log($"\"{version}\" already downloaded.");
                    string json = System.IO.File.ReadAllText(localDictionaryPath);
                    if (DownloadManager.SaveDict(json))
                    {
                        onDicionaryReady?.Invoke();
                        return;
                    }
                    else
                    {
                        Debug.LogError($"Failed to parse the existing dictionary. Redownloading it");
                    }
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
        DownloadDictionary(onDicionaryReady, onDownloadError, onParseError);
    }

    private async static void DownloadDictionary(System.Action onComplete, System.Action<int, string> onDownloadError, System.Action onParseError)
    {
        using (var webClient = new WebClient())
        {
            var url = new Uri(baseURL + version + "/" + lng[language] + ".json");

            try
            {
                string result = await webClient.DownloadStringTaskAsync(url);
                File.WriteAllText(localDictionaryPath, result);

                if (DownloadManager.SaveDict(result))
                    onComplete?.Invoke();
                else
                    onParseError?.Invoke();
            }
            catch (WebException e)
            {
                Debug.LogError("Error in getting dictionary: " + e.Message + "\nStacktrace: " + e.StackTrace);
                tries++;
                if (tries < 5)
                {
                    DownloadDictionary(onComplete, onDownloadError, onParseError);
                }
                else
                {
                    var response = e.Response as HttpWebResponse;
                    onDownloadError?.Invoke((int)response.StatusCode, response.StatusDescription);
                }
            }
        }
    }
}