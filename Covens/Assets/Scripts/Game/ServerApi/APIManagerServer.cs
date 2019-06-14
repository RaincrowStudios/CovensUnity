using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// request only responsible class
/// </summary>
public class APIManagerServer
{
    private const float RETRY_COOLDOWN = 2F;
    private const int MAX_RETRIES = 3;
    public static bool ENABLE_AUTO_RETRY = true;

    private static IEnumerator RequestRoutine(string url, string data, string sMethod, bool bRequiresToken, bool bRequiresWssToken, Action<string, int> CallBack)
    {
        int retryCount = 0;
        bool fail = true;

        UnityWebRequest www = null;// BakeRequest(url, data, sMethod, bRequiresToken, bRequiresWssToken);

        while (fail && retryCount < MAX_RETRIES)
        {
            www = BakeRequest(url, data, sMethod, bRequiresToken, bRequiresWssToken);
            APIManager.CallRequestEvent(www, data);
            yield return www.SendWebRequest();
            APIManager.CallOnResponseEvent(www, data, www.downloadHandler.text);

            fail = www.isNetworkError;
            retryCount += 1;

            if (fail && ENABLE_AUTO_RETRY)
            {
                APIManager.ThrowRetryError(www, url, data);
                LoadingOverlay.Show();
                yield return new WaitForSeconds(RETRY_COOLDOWN);
            }
            else
            {               
                break;
            }
        }

        if (fail)
            APIManager.ThrowCriticalError(www, url, data);

        CallBack(fail ? www.error : www.downloadHandler.text, Convert.ToInt32(www.responseCode));

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