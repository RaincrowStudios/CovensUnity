using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// request only responsible class
/// </summary>
public class APIManagerServer
{
    public static IEnumerator RequestRoutine(string endpoint, string data, string sMethod, bool bRequiresToken, bool bRequiresWssToken, Action<string, int> CallBack)
    {
        // build the request
        string sUrl = Constants.hostAddress + endpoint;
        UnityWebRequest www = BakeRequest(sUrl, data, sMethod, bRequiresToken, bRequiresWssToken);
        APIManager.CallRequestEvent(www, data);

        // request
        yield return www.SendWebRequest();

        // receive the response
        APIManager.CallOnResponseEvent(www, data, www.downloadHandler.text);
        if (www.isNetworkError)
        {
            Debug.LogError(www.responseCode.ToString());
            //PlayerManager.Instance.initStart();
        }
        else
        {
            //            Debug.Log(www.responseCode.ToString());
            //            Debug.Log("Received response : " + www.downloadHandler.text);
            CallBack(www.downloadHandler.text, Convert.ToInt32(www.responseCode));
        }
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
        Debug.Log(sRequest);
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