using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;


#if LOCAL_REQUEST
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

    [UnityEditor.MenuItem("Covens/Set Local", true)]
    public static void Test()
    {

    }
#endif
}

