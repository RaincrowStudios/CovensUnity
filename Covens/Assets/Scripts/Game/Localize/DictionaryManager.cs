using UnityEngine;
using BestHTTP;
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
    public static readonly string[] Cultures = new string[] { "en", "pt", "es", "ja", "de", "ru" };

    private static string baseURL
    {
        get
        {
            string url = "https://storage.googleapis.com/raincrow-covens/dictionary/";

#if UNITY_EDITOR
            switch (UnityEditor.EditorPrefs.GetString("game"))
            {
                case "Local": url += "staging/"; break;
                case "Release": url += "release/"; break;
                case "Gustavo": url += "staging/"; break;
                default: url += "staging/"; break;
            }
#elif PRODUCTION
            url += "release/";
#else
            url +=  "staging/";
#endif

            return url;
        }
    }
    private static string LOCALIZATION_URL => baseURL + "localization/";
    private static string STORE_URL => baseURL + "store/";
    private static string GAME_URL => baseURL + "game/";

    private const string LOCALISATION_DICT_KEY = "covens.localization";
    private const string GAME_DICT_KEY = "covens.game";
    private const string STORE_DICT_KEY = "covens.store";

    public const string GAME_DICT_FILENAME = "game.text";
    public const string STORE_DICT_FILENAME = "store.text";

    public static int languageIndex
    {
        get
        {
            if (PlayerPrefs.HasKey(LanguageIndexPlayerPrefsKey))
            {
                return PlayerPrefs.GetInt(LanguageIndexPlayerPrefsKey, 0);
            }
            else
            {
                var t = Application.systemLanguage.ToString();
                for (int i = 0; i < Languages.Length; i++)
                {
                    if (Languages[i] == t)
                    {
                        PlayerPrefs.SetInt(LanguageIndexPlayerPrefsKey, i);
                        return i;
                    }
                }

                return 0;
            }
        }
        set
        {
            PlayerPrefs.SetInt(LanguageIndexPlayerPrefsKey, value);
        }
    }

    public static string GetCurrentCultureName()
    {
        if (languageIndex < 0 || languageIndex >= Cultures.Length)
        {
            return "en";
        }
        return Cultures[languageIndex];
    }

    //public static event System.Action<string, float, float> OnDownloadProgress;


    public static void GetLocalisationDictionary(string version, System.Action onDicionaryReady, System.Action<int, string> onDownloadError, System.Action onParseError)
    {
        string json;
        string language = Languages[languageIndex];
        string localPath = System.IO.Path.Combine(Application.persistentDataPath, language + ".text");
        CrashReportHandler.SetUserMetadata("localisation", language + version);

        LocalFileState result = TryGetLocalFile(LOCALISATION_DICT_KEY + language, version, localPath, out json);

        if (result == LocalFileState.FILE_AVAILABLE && json != null)
        {
            Debug.Log($"\"{language + version}\" already downloaded.");
            if (DownloadManager.DeserializeLocalisationDictionary(json, null))
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
                Debug.Log("No localization dictionary found");
            else if (result == LocalFileState.VERSION_OUTDATED)
                Debug.Log($"Localization dictionary outdated.");
        }

        DownloadFile(LOCALIZATION_URL + version + "/" + language + ".json", language, version, (resultCode, response) =>
        {
            if (resultCode == 200 || resultCode == 304)
            {
                 if (DownloadManager.DeserializeLocalisationDictionary(response, null))
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
        5);
    }

    public static void GetGameDictionary(string version, System.Action onDicionaryReady, System.Action<int, string> onDownloadError, System.Action onParseError)
    {

#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;

        if (UnityEditor.EditorPrefs.GetBool("DebugUtils.UseLocalGameDict", false))
        {
            TextAsset gamedata = (TextAsset)UnityEditor.EditorGUIUtility.Load("game.json");
            if (DownloadManager.DeserializeGameDictionary(gamedata.text, null))
                onDicionaryReady?.Invoke();
            else
                onParseError?.Invoke();
            return;
        }
#endif

        CrashReportHandler.SetUserMetadata("gamedata", version);

        string json;
        string localPath = System.IO.Path.Combine(Application.persistentDataPath, GAME_DICT_FILENAME);
        LocalFileState result = TryGetLocalFile(GAME_DICT_KEY, version, localPath, out json);

        if (result == LocalFileState.FILE_AVAILABLE && json != null)
        {
            Debug.Log($"\"game{version}\" already downloaded.");
            if (DownloadManager.DeserializeGameDictionary(json, null))
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

        DownloadFile(GAME_URL + version + ".json", "game", version, (resultCode, response) =>
        {
            if (resultCode == 200 || resultCode == 304)
            {
                if (DownloadManager.DeserializeGameDictionary(response, null))
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
        5);
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
            if (DownloadManager.DeserializeStoreDictionary(json, null))
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

        DownloadFile(STORE_URL + version + ".json", "store", version, (resultCode, response) =>
        {
            if (resultCode == 200 || resultCode == 304)
            {
                if (DownloadManager.DeserializeStoreDictionary(response, null))
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
        5);
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

    private static void DownloadFile(string url, string name, string version, System.Action<int, string> onComplete, int maxRetries)
    {
        //LeanTween.value(0, 0, 0).setDelay(10).setOnComplete(() => { 
        DownloadManager.DownloadFile(
            url,
            (downloaded, length) =>
            {
                //OnDownloadProgress?.Invoke(name, size * 0.000001f, e.ProgressPercentage / 100f);
            },
            (status, response) =>
            {
                onComplete?.Invoke(status, response);
            });
        //});
    }
}