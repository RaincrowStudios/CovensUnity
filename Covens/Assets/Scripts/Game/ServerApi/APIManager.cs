using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;


#if SERVER_FAKE
using ServerApi = APIManagerLocal;
#else
using ServerApi = APIManagerServer;
#endif

/// <summary>
/// TODO: check if we need thos functionalities:
///     - Timeout
///     - crypt or gzip the data
/// </summary>
public class APIManager : Patterns.SingletonComponent<APIManager>
{
    public static event Action<UnityWebRequest, string> OnRequestEvt;
    public static event Action<UnityWebRequest, string, string> OnResponseEvt;

    public ApiServerData ServerData;

    public override void Awake()
    {
        base.Awake();

        ServerData = ApiServerData.Load();
    }


    public static void CallRequestEvent(UnityWebRequest pReq, string sRequestData)
    {
        if (OnRequestEvt != null)
            OnRequestEvt(pReq, sRequestData);
    }
    public static void CallOnResponseEvent(UnityWebRequest pRequest, string sRequestData, string sResponseData)
    {
        if (OnResponseEvt != null)
            OnResponseEvt(pRequest, sRequestData, sResponseData);
    }



    public void Post(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.postHelper(endpoint, data, CallBack));
    }


    public void PostCoven(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestCovenHelper(endpoint, data, "POST", CallBack));
    }
    public void PutCoven(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestCovenHelper(endpoint, data, "PUT", CallBack));
    }
    public void DeleteCoven(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestCovenHelper(endpoint, data, "DELETE", CallBack));
    }
    public void GetCoven(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestCovenHelper(endpoint, data, "GET", CallBack));
    }



    UnityWebRequest BakeRequest(string endpoint, string data, string sMethod)
    {
        UnityWebRequest www = UnityWebRequest.Put(endpoint, data);
        print(endpoint);
        www.method = sMethod;
        string bearer = "Bearer " + LoginAPIManager.loginToken;
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", bearer);
        print("Sending Data : " + data);
        return www;
    }


    public void PostCovenSelect(string endpoint, string data, Action<string, int, MarkerSpawner.MarkerType> CallBack, MarkerSpawner.MarkerType type)
    {
        StartCoroutine(PostCovenSelectHelper(endpoint, data, CallBack, type));
    }

    IEnumerator PostCovenSelectHelper(string endpoint, string data, Action<string, int, MarkerSpawner.MarkerType> CallBack, MarkerSpawner.MarkerType type)
    {
        UnityWebRequest www = UnityWebRequest.Put(Constants.hostAddressLocal + endpoint, data);
        www.method = "POST";
        string bearer = "Bearer " + LoginAPIManager.loginToken;
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", bearer);
        print("Sending Data : " + data);
        if (OnRequestEvt != null)
            OnRequestEvt(www, data);

        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.LogError(www.error + www.responseCode.ToString());
        }
        else
        {
            print(www.GetRequestHeader("HTTP-date"));
            print(www.responseCode.ToString());
            print("Received response : " + www.downloadHandler.text);
            CallBack(www.downloadHandler.text, Convert.ToInt32(www.responseCode), type);
        }

        if (OnResponseEvt != null)
            OnResponseEvt(www, data, www.downloadHandler.text);

    }

#if UNITY_EDITOR


    private const string SetServerRelease = "Raincrow/Server/Set Server Release";
    private const string SetServerLocal = "Raincrow/Server/Set Server Local";
    private const string SetServerFake = "Raincrow/Server/Set Server Fake";




    public static void RemoveDefine(string sDef, UnityEditor.BuildTargetGroup eBuildGroup)
    {
        string sDefs = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(eBuildGroup);
        sDefs = sDefs.Replace(";" + sDef, "");
        sDefs = sDefs.Replace(sDef + ";", "");
        sDefs = sDefs.Replace(sDef, "");
        UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(eBuildGroup, sDefs);
    }
    public static void AddDefine(string sDef, UnityEditor.BuildTargetGroup eBuildGroup)
    {
        string sDefs = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(eBuildGroup);
        sDefs = sDefs + ";" + sDef;
        UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(eBuildGroup, sDefs);
    }
    public static void AddDefine(string sDef)
    {
        AddDefine(sDef, UnityEditor.BuildTargetGroup.Android);
        AddDefine(sDef, UnityEditor.BuildTargetGroup.iOS);
        AddDefine(sDef, UnityEditor.BuildTargetGroup.Standalone);
    }
    public static void RemoveAll()
    {
        RemoveDefine("SERVER_RELEASE", UnityEditor.BuildTargetGroup.Android);
        RemoveDefine("SERVER_RELEASE", UnityEditor.BuildTargetGroup.iOS);
        RemoveDefine("SERVER_RELEASE", UnityEditor.BuildTargetGroup.Standalone);
        RemoveDefine("SERVER_LOCAL", UnityEditor.BuildTargetGroup.Android);
        RemoveDefine("SERVER_LOCAL", UnityEditor.BuildTargetGroup.iOS);
        RemoveDefine("SERVER_LOCAL", UnityEditor.BuildTargetGroup.Standalone);
        RemoveDefine("SERVER_FAKE", UnityEditor.BuildTargetGroup.Android);
        RemoveDefine("SERVER_FAKE", UnityEditor.BuildTargetGroup.iOS);
        RemoveDefine("SERVER_FAKE", UnityEditor.BuildTargetGroup.Standalone);
    }

    #region SERVER_RELEASE

    [UnityEditor.MenuItem(SetServerRelease, false, 0)]
    public static void SetFake()
    {
        RemoveAll();
        AddDefine("SERVER_RELEASE");
    }

    [UnityEditor.MenuItem(SetServerRelease, true, 0)]
    public static bool CheckToggleFake()
    {
#if SERVER_RELEASE
        UnityEditor.Menu.SetChecked(SetServerRelease, true);
#else
        UnityEditor.Menu.SetChecked(SetServerRelease, false);
#endif
        return true;
    }

    #endregion


    #region SERVER_LOCAL

    [UnityEditor.MenuItem(SetServerLocal, false, 0)]
    public static void ServerLocal()
    {
        RemoveAll();
        AddDefine("SERVER_LOCAL");
    }
    [UnityEditor.MenuItem(SetServerLocal, true, 0)]
    public static bool CheckServerLocal()
    {
#if SERVER_LOCAL
        UnityEditor.Menu.SetChecked(SetServerLocal, true);
#else
        UnityEditor.Menu.SetChecked(SetServerLocal, false);
#endif
        return true;
    }

    #endregion


    #region SERVER_FAKE

    [UnityEditor.MenuItem(SetServerFake, false, 0)]
    public static void ServerFake()
    {
        RemoveAll();
        AddDefine("SERVER_FAKE");
    }
    [UnityEditor.MenuItem(SetServerFake, true, 0)]
    public static bool CheckServerFake()
    {
#if SERVER_FAKE
        UnityEditor.Menu.SetChecked(SetServerFake, true);
#else
        UnityEditor.Menu.SetChecked(SetServerFake, false);
#endif
        return true;
    }

    #endregion



#endif
}

