using UnityEngine;
using UnityEngine.CrashReportHandler;

public class DictionaryManager
{
    public static readonly string DictionaryVersionPlayerPrefsKey = "DictionaryVersion";
    public static readonly string LanguageIndexPlayerPrefsKey = "LanguageIndex";
    public static readonly string[] Languages = new string[] { "English", "Portuguese", "Spanish", "Japanese", "German", "Russian" };

    //public static string version = "87";
    private const string baseURL = "https://storage.googleapis.com/raincrow-covens/dictionary/";
    static int tries = 0;
    public static string filename = "dict.text";
    static string localDictionaryPath;

    public static bool DictionaryReady { get; private set; }

    public static int language
    {
        get { return PlayerPrefs.GetInt(LanguageIndexPlayerPrefsKey, 0); }
        set { PlayerPrefs.SetInt(LanguageIndexPlayerPrefsKey, value); }
    }

    public static void GetDictionary(string version, System.Action onDicionaryReady, System.Action<int, string> onDownloadError, System.Action onParseError)
    {
        DictionaryReady = false;
        tries = 0;

#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;
#endif

        localDictionaryPath = System.IO.Path.Combine(Application.persistentDataPath, filename);
        if (PlayerPrefs.HasKey("DataDict"))
        {
            string currentDictionary = PlayerPrefs.GetString("DataDict");
            if (currentDictionary == version)
            {
                if (System.IO.File.Exists(localDictionaryPath))
                {
                    Debug.Log($"\"{version}\" already downloaded.");
                    string json = System.IO.File.ReadAllText(localDictionaryPath);
                    if (DownloadManager.SaveDict(version, json))
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
        DownloadDictionary(version, onDicionaryReady, onDownloadError, onParseError);
    }

    private async static void DownloadDictionary(string version, System.Action onComplete, System.Action<int, string> onDownloadError, System.Action onParseError)
    {
        Debug.Log("Download dictionary " + version);
        using (var webClient = new System.Net.WebClient())
        {
            var url = new System.Uri(baseURL + version + "/" + Languages[0] + ".json");

            CrashReportHandler.SetUserMetadata("dictionary", version + "/" + Languages[language]);

            try
            {
                string result = await webClient.DownloadStringTaskAsync(url);
                System.IO.File.WriteAllText(localDictionaryPath, result);

                if (DownloadManager.SaveDict(version, result))
                {
                    PlayerPrefs.SetString("DataDict", version);
                    onComplete?.Invoke();
                }
                else
                {
                    onParseError?.Invoke();
                }
            }
            catch (System.Net.WebException e)
            {
                Debug.LogError("Error in getting dictionary: " + e.Message + "\nStacktrace: " + e.StackTrace);
                tries++;
                if (tries < 5)
                {
                    LeanTween.value(0, 0, 2f).setOnComplete(() => DownloadDictionary(version, onComplete, onDownloadError, onParseError));
                }
                else
                {
                    var response = e.Response as System.Net.HttpWebResponse;

                    if (response == null)
                        onDownloadError?.Invoke(0, "null response");
                    else
                        onDownloadError?.Invoke((int)response.StatusCode, response.StatusDescription);
                }
            }
        }
    }
}