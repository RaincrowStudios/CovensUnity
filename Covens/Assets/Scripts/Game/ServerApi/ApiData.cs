using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class ApiServerData
{
#if SERVER_RELEASE
    public bool UseLocal = false;
    public string hostAddressRaincrowLocal = "https://raincrow-pantheon.appspot.com/api/raincrow/";
    public string hostAddressLocal = "https://raincrow-pantheon.appspot.com/api/covens/";
    public string hostAddressRaincrow = "https://raincrow-pantheon.appspot.com/api/raincrow/";
    public string hostAddress = "https://raincrow-pantheon.appspot.com/api/covens/";
    public string wsAddress = "https://raincrowstudios.xyz:8080";
    public string wssAddress = "wss://raincrowstudios.xyz:8080?";
#else
    public bool UseLocal = false;
    public string hostAddressRaincrowLocal = "http://localhost:8080/api/raincrow/";
    public string hostAddressLocal = "http://localhost:8080/api/covens/";
    public string hostAddressRaincrow = "https://raincrow-pantheon.appspot.com/api/raincrow/";
    public string hostAddress = "https://raincrow-pantheon.appspot.com/api/covens/";
    public string wsAddress = "https://raincrowstudios.xyz:8080";
    public string wssAddress = "wss://raincrowstudios.xyz:8080?";
#endif

    const string FilePath = "GameSettings/GameConf";


    [UnityEditor.MenuItem("Raincrow/Test")]
    public static void PrintTest()
    {
        var a = new ApiServerData();
        Debug.Log(JsonUtility.ToJson(a, true));
    }
    [UnityEditor.MenuItem("Raincrow/Test 2")]
    public static void LoadTest()
    {
        LoadJsonFile<ApiServerData>(FilePath);
    }
    public static ApiServerData Load()
    {
        return LoadJsonFile<ApiServerData>(FilePath);
    }

    public static T LoadJsonFile<T>(string sPath)
    {
        string sFile = LoadFile(sPath);
        if (!string.IsNullOrEmpty(sFile))
        {
            try
            {
                return JsonUtility.FromJson<T>(sPath);
            }catch(Exception e)
            {
                Debug.Log("Error parsing json to class: " + sPath + " == " + e.Message);
            }
        }
        return default(T);
    }

    public static string LoadFile(string sPath)
    {
        TextAsset pText = Resources.Load<TextAsset>(sPath);

        string sResponse = null;
        if (pText != null)
        {
            sResponse = pText.text;
        }
        else
        {
            Debug.LogError("File not found: " + sPath);
        }

        // so we can save and use the text again
        Resources.UnloadAsset(pText);


        return sResponse;
    }
}
