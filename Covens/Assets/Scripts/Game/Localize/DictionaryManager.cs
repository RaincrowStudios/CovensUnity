using UnityEngine;
using UnityEngine.CrashReportHandler;

public class DictionaryManager
{
    private enum LocalFileState
    {
        /// <summary>
        /// file is at player prefs and on disc
        /// </summary>
        FILE_AVAILABLE = 0,

        /// <summary>
        /// key was not found on player prefs
        /// </summary>
        KEY_NOT_FOUND,

        /// <summary>
        /// key was found, but is outdated
        /// </summary>
        VERSION_OUTDATED,

        /// <summary>
        /// key was at player prefs, but not on disc
        /// </summary>
        FILE_NOT_FOUND,
    }

    public const string LanguageIndexPlayerPrefsKey = "LanguageIndex";
    public static readonly string[] Languages = new string[] { "English", "Portuguese", "Spanish", "Japanese", "German", "Russian" };
    public static readonly string[] Cultures = new string[] { "en", "pt", "es", "jp", "de", "ru" };

    private const string baseURL = "https://storage.googleapis.com/raincrow-covens/dictionary_v3/";

    private const string LOCALISATION_DICT_KEY = "LocalisationDict";
    private const string GAME_DICT_KEY = "GameDict";
    private const string STORE_DICT_KEY = "StoreDict";

    public const string GAME_DICT_FILENAME = "game.text";
    public const string STORE_DICT_FILENAME = "store.text";

    public static int languageIndex
    {
        get { return PlayerPrefs.GetInt(LanguageIndexPlayerPrefsKey, 0); }
        set { PlayerPrefs.SetInt(LanguageIndexPlayerPrefsKey, value); }
    }

    public static string GetCurrentCultureName()
    {
        if (languageIndex < 0 || languageIndex >= Cultures.Length)
        {
            return "en";
        }
        return Cultures[languageIndex];
    }

    public static event System.Action<string,float,float> OnDownloadProgress;


    public static void GetLocalisationDictionary(string version, System.Action onDicionaryReady, System.Action<int, string> onDownloadError, System.Action onParseError)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;

        //TextAsset english = (TextAsset)UnityEditor.EditorGUIUtility.Load("english.json");
        //if (DownloadManager.DeserializeLocalisationDictionary(version, english.text))
        //    onDicionaryReady?.Invoke();
        //else
        //    onParseError?.Invoke();
        //return;
#endif

        string json;
        string language = Languages[languageIndex];
        string localPath = System.IO.Path.Combine(Application.persistentDataPath, language + ".text");
        CrashReportHandler.SetUserMetadata("localisation", language + version);

        LocalFileState result = TryGetLocalFile(LOCALISATION_DICT_KEY + language, version, localPath, out json);

        if (result == LocalFileState.FILE_AVAILABLE && json != null)
        {
            Debug.Log($"\"{language + version}\" already downloaded.");
            if (DownloadManager.DeserializeLocalisationDictionary(json))
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
            if (result == LocalFileState.FILE_NOT_FOUND)
                Debug.Log($"Dictionary \"{language + version}\" is marked as download but no file was found.");
            else if (result == LocalFileState.KEY_NOT_FOUND)
                Debug.Log("No dictionary found");
            else if (result == LocalFileState.VERSION_OUTDATED)
                Debug.Log($"Dictionary outdated.");
        }

        DownloadFile(language, version, (resultCode, response) =>
        {
            if (resultCode == 200)
            {
                if (DownloadManager.DeserializeLocalisationDictionary(response))
                {
                    PlayerPrefs.SetString(LOCALISATION_DICT_KEY + language, version);
                    System.IO.File.WriteAllText(localPath, response);
                    onDicionaryReady?.Invoke();
                }
                else
                {
                    onParseError?.Invoke();
                }
            }
            else
            {
                onDownloadError?.Invoke(resultCode, response);
            }
        },
        5, 0);
    }

    public static void GetGameDictionary(string version, System.Action onDicionaryReady, System.Action<int, string> onDownloadError, System.Action onParseError)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;

        //TextAsset gamedata = (TextAsset)UnityEditor.EditorGUIUtility.Load("gamedata.json");
        //if (DownloadManager.DeserializeGameDictionary(version, gamedata.text))
        //    onDicionaryReady?.Invoke();
        //else
        //    onParseError?.Invoke();
        //return;
#endif

        CrashReportHandler.SetUserMetadata("gamedata", version);

        string json;
        string localPath = System.IO.Path.Combine(Application.persistentDataPath, GAME_DICT_FILENAME);
        LocalFileState result = TryGetLocalFile(GAME_DICT_KEY, version, localPath, out json);

        if (result == LocalFileState.FILE_AVAILABLE && json != null)
        {
            Debug.Log($"\"game{version}\" already downloaded.");
            if (DownloadManager.DeserializeGameDictionary(json))
            {
                onDicionaryReady?.Invoke();
                return;
            }
            else
            {
                Debug.LogError($"Failed to parse the game dictionary. Redownloading it");
            }
        }
        else
        {
            if (result == LocalFileState.FILE_NOT_FOUND)
                Debug.Log($"game dict \"{version}\" is marked as download but no file was found.");
            else if (result == LocalFileState.KEY_NOT_FOUND)
                Debug.Log("No gamedict found");
            else if (result == LocalFileState.VERSION_OUTDATED)
                Debug.Log($"gamedict outdated.");
        }

        DownloadFile("game", version, (resultCode, response) =>
        {
            if (resultCode == 200)
            {
                if (DownloadManager.DeserializeGameDictionary(response))
                {
                    PlayerPrefs.SetString(GAME_DICT_KEY, version);
                    System.IO.File.WriteAllText(localPath, response);
                    onDicionaryReady?.Invoke();
                }
                else
                {
                    onParseError?.Invoke();
                }
            }
            else
            {
                onDownloadError?.Invoke(resultCode, response);
            }
        },
        5, 0);
    }

    public static void GetStoreDictionary(string version, System.Action onDicionaryReady, System.Action<int, string> onDownloadError, System.Action onParseError)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;

        //TextAsset gamedata = (TextAsset)UnityEditor.EditorGUIUtility.Load("gamedata.json");
        //if (DownloadManager.DeserializeGameDictionary(version, gamedata.text))
        //    onDicionaryReady?.Invoke();
        //else
        //    onParseError?.Invoke();
        //return;
#endif

        CrashReportHandler.SetUserMetadata("store", version);

        string json;
        string localPath = System.IO.Path.Combine(Application.persistentDataPath, STORE_DICT_FILENAME);
        LocalFileState result = TryGetLocalFile(STORE_DICT_KEY, version, localPath, out json);

        if (result == LocalFileState.FILE_AVAILABLE && json != null)
        {
            Debug.Log($"\"store{version}\" already downloaded.");
            if (DownloadManager.DeserializeStoreDictionary(json))
            {
                onDicionaryReady?.Invoke();
                return;
            }
            else
            {
                Debug.LogError($"Failed to parse the store dictionary. Redownloading it");
            }
        }
        else
        {
            if (result == LocalFileState.FILE_NOT_FOUND)
                Debug.Log($"store dict \"{version}\" is marked as download but no file was found.");
            else if (result == LocalFileState.KEY_NOT_FOUND)
                Debug.Log("No store found");
            else if (result == LocalFileState.VERSION_OUTDATED)
                Debug.Log($"store outdated.");
        }

        DownloadFile("store", version, (resultCode, response) =>
        {
            if (resultCode == 200)
            {
                if (DownloadManager.DeserializeStoreDictionary(response))
                {
                    PlayerPrefs.SetString(STORE_DICT_KEY, version);
                    System.IO.File.WriteAllText(localPath, response);
                    onDicionaryReady?.Invoke();
                }
                else
                {
                    onParseError?.Invoke();
                }
            }
            else
            {
                onDownloadError?.Invoke(resultCode, response);
            }
        },
        5, 0);
    }



    private static LocalFileState TryGetLocalFile(string key, string version, string filepath, out string content)
    {
        content = null;
        if (PlayerPrefs.HasKey(key))
        {
            string currentVersion = PlayerPrefs.GetString(key);
            if (currentVersion == version)
            {
                if (System.IO.File.Exists(filepath))
                {
                    content = System.IO.File.ReadAllText(filepath);
                    return LocalFileState.FILE_AVAILABLE;
                }
                else
                {
                    return LocalFileState.FILE_NOT_FOUND;
                }
            }
            else
            {
                return LocalFileState.VERSION_OUTDATED;
            }
        }
        else
        {
            return LocalFileState.KEY_NOT_FOUND;
        }
    }

    private async static void DownloadFile(string name, string version, System.Action<int, string> onComplete, int maxRetries, int tryCount = 0)
    {
        var url = new System.Uri(baseURL + version + "/" + name + ".json");
        Debug.Log("Downloading " + url.ToString());
        using (var webClient = new System.Net.WebClient())
        {
            try
            {
                //get header
                var openReadTask = await webClient.OpenReadTaskAsync(url);
                long size = size = System.Convert.ToInt64(webClient.ResponseHeaders["Content-Length"]);
                openReadTask.Close();

                //get file
                webClient.DownloadProgressChanged += (sender, args) => DownloadProgressChanged(name, size, args); 
                string result = await webClient.DownloadStringTaskAsync(url);

                onComplete(200, result);
            }
            catch (System.Net.WebException e)
            {
                Debug.LogError("Error in getting dictionary: " + e.Message + "\nStacktrace: " + e.StackTrace);
                tryCount++;
                if (tryCount <= maxRetries)
                {
                    LeanTween.value(0, 0, 2f).setOnComplete(() => DownloadFile(name, version, onComplete, maxRetries, tryCount));
                }
                else
                {
                    var response = e.Response as System.Net.HttpWebResponse;
                    
                    if (response == null)
                        onComplete(0, "null response");
                    else
                        onComplete((int)response.StatusCode, response.StatusDescription);
                }
            }
        }
    }

    private static void DownloadProgressChanged(string name, long size, System.Net.DownloadProgressChangedEventArgs e)
    {
        OnDownloadProgress?.Invoke(name, size * 0.000001f, e.ProgressPercentage/100f);
    }
}