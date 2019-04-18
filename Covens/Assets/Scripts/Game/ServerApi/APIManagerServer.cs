using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// request only responsible class
/// </summary>
public class APIManagerServer
{
    private static IEnumerator RequestRoutine(string url, string data, string sMethod, bool bRequiresToken, bool bRequiresWssToken, Action<string, int> CallBack)
    {
        // build the request
        //string sUrl = CovenConstants.hostAddress + endpoint;
        UnityWebRequest www = BakeRequest(url, data, sMethod, bRequiresToken, bRequiresWssToken);
        APIManager.CallRequestEvent(www, data);

        // request
        yield return www.SendWebRequest();

        // receive the response
        APIManager.CallOnResponseEvent(www, data, www.downloadHandler.text);
        if (www.isNetworkError)
        {
            string debugString = www.responseCode.ToString() + "\n" + url;
            Debug.LogError(debugString);
            //PlayerManager.Instance.initStart();
            CallBack("", (int)www.responseCode);
        }
        else
        {
            //            Debug.Log(www.responseCode.ToString());
            //            Debug.Log("Received response : " + www.downloadHandler.text);
            CallBack(www.downloadHandler.text, Convert.ToInt32(www.responseCode));
        }
    }

    public static IEnumerator RequestServerRoutine(string endpoint, string data, string sMethod, bool bRequiresToken, bool bRequiresWssToken, Action<string, int> CallBack)
    {
        string url = string.Concat(CovenConstants.hostAddress + endpoint);
        return RequestRoutine(url, data, sMethod, bRequiresToken, bRequiresWssToken, CallBack);
    }

    public static IEnumerator RequestAnalyticsRoutine(string endpoint, string data, string sMethod, bool bRequiresToken, bool bRequiresWssToken, Action<string, int> CallBack)
    {
        string url = string.Concat(CovenConstants.analyticsAddress + endpoint);
        return RequestRoutine(url, data, sMethod, bRequiresToken, bRequiresWssToken, CallBack);
    }

    static UnityWebRequest BakeRequest(string endpoint, string data, string sMethod, bool bRequiresLoginToken, bool bRequiresWssToken)
    {
        // log it
        string sRequest = "==> BakeRequest for: " + endpoint;
        sRequest += "\n  endpoint: " + endpoint;
        sRequest += "\n  method: " + sMethod;
        sRequest += "\n  data: " + data;
        sRequest += "\n  bRequiresLoginToken: " + bRequiresLoginToken;
        sRequest += "\n  bRequiresWssToken: " + bRequiresWssToken;
        if (bRequiresLoginToken)
            sRequest += "\n  loginToken: " + LoginAPIManager.loginToken;
        if (bRequiresWssToken)
            sRequest += "\n  wssToken: " + LoginAPIManager.wssToken;
        //Debug.Log(sRequest);
        UnityWebRequest www;
        if (sMethod == "GET")
        {
            www = UnityWebRequest.Get(endpoint);
        }
        else
        {
            www = UnityWebRequest.Put(endpoint, data);
            www.method = sMethod;
        }
        www.SetRequestHeader("Content-Type", "application/json");
        if (bRequiresLoginToken)
        {
            www.SetRequestHeader("Authorization", "Bearer " + LoginAPIManager.loginToken);
        }
        if (bRequiresWssToken)
        {
            www.SetRequestHeader("Authorization", "Bearer " + LoginAPIManager.wssToken);
        }

        return www;
    }



}