using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// request only responsible class
/// </summary>
public class APIManagerServer
{
    private  const int MAX_RETRIES = 3;

    private static IEnumerator RequestRoutine(string url, string data, string sMethod, bool bRequiresToken, bool bRequiresWssToken, Action<string, int> CallBack)
    {
        int retryCount = 0;
        bool fail = true;
        // build the request
        UnityWebRequest www = BakeRequest(url, data, sMethod, bRequiresToken, bRequiresWssToken);

        while (fail && retryCount < MAX_RETRIES)
        {
            APIManager.CallRequestEvent(www, data);
            // request
            yield return www.SendWebRequest();
            // receive the response
            APIManager.CallOnResponseEvent(www, data, www.downloadHandler.text);

            fail = www.isNetworkError;
            retryCount += 1;

            if (fail)
            {
                APIManager.ThrowRetryError(www, url, data);
                LoadingOverlay.Show();
                www = BakeRequest(url, data, sMethod, bRequiresToken, bRequiresWssToken);
            }
            else
            {
                CallBack(www.downloadHandler.text, Convert.ToInt32(www.responseCode));
            }
        }

        if (fail)
        {
            APIManager.ThrowCriticalError(www, url, data);
            CallBack("", (int)www.responseCode);
        }

        LoadingOverlay.Hide();
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