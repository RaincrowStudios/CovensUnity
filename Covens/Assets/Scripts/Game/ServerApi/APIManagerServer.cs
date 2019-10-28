using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

/// <summary>
/// request only responsible class
/// </summary>
public class APIManagerServer
{
    public const float RetryCooldown = 2f;
    public const int MaxRetries = 5;
    public const bool UseBackupServer = true;
    public static bool EnableAutoRetry = true;
    public const int MinBadGatewayErrors = 3;
    public const int BadGatewayErrorResponse = 502;

    public static IEnumerator RequestServerRoutine(string endpoint, string data, string sMethod, bool bRequiresToken, bool bRequiresWssToken, Action<string, int> CallBack)
    {
        string url = string.Concat(CovenConstants.hostAddress + endpoint);
        yield return RequestCoroutine(url, data, sMethod, bRequiresToken, bRequiresWssToken, CallBack);
    }

    public static IEnumerator RequestCoroutine(string url, string data, string sMethod, bool bRequiresToken, bool bRequiresWssToken, Action<string, int> CallBack)
    {
        bool retry = true;
        int retryCount = 0;
        //int badGatewayErrorsCount = 0;
        UnityWebRequest www = null;

        while (retry && retryCount < MaxRetries)
        {
            www = BakeRequest(url, data, sMethod, bRequiresToken, bRequiresWssToken);

            APIManager.CallRequestEvent(www, data);
            yield return www.SendWebRequest();
            APIManager.CallOnResponseEvent(www, data, www.isNetworkError ? www.error : www.downloadHandler.text);

            retry = www.isNetworkError || (www.isHttpError && www.responseCode > 500);
            retryCount += 1;
            
            if (retryCount > 0)
            {
                LoadingOverlay.Hide();
                LoadingOverlay.Show();
            }

            if (www.isHttpError && www.responseCode == 412 && www.downloadHandler.text == "2016")
            {
                Debug.LogError("2016 - retry");
                retryCount = 0;
                retry = true;
                yield return new WaitForSeconds(1f);
            }
            else if (www.isHttpError && (www.responseCode == 401 || www.downloadHandler.text == "1006"))
            {
                //refresh auth tokens and repeat the request
                bool waitingTokens = true;
                LoginAPIManager.RefreshTokens((success) =>
                {
                    if (success)
                    {
                        //reset the retry count and set retry to true
                        retryCount = 0;
                        retry = true;
                    }
                    else
                    {
                        //abort everything, throw critical unauthenticated should return to login screen
                        APIManager.ThrowCriticalUnauthenticated();
                        retryCount = MaxRetries;
                    }

                    waitingTokens = false;
                });

                while (waitingTokens)
                    yield return 0;
            }
            else if (retry && EnableAutoRetry)
            {
                APIManager.ThrowRetryError(www, url, data);
                LoadingOverlay.Show();
                yield return new WaitForSeconds(RetryCooldown);
            }
            else
            {
                break;
            }
        }

        Dictionary<string, string> responseHeaders = www.GetResponseHeaders();

        if (retryCount > 0)
            LoadingOverlay.Hide();

        CallBack(retry ? www.error : www.downloadHandler.text, Convert.ToInt32(www.responseCode));

        if (retry)
            APIManager.ThrowCriticalError(www, url, data);
    }

    public static IEnumerator RequestAnalyticsRoutine(string endpoint, string data, string sMethod, bool bRequiresToken, bool bRequiresWssToken, Action<string, int> CallBack)
    {
        string url = string.Concat(CovenConstants.analyticsAddress + endpoint);
        bool fail = true;
        int retryCount = 0;

        UnityWebRequest www = null;

        while (fail)
        {
            www = BakeRequest(url, data, sMethod, bRequiresToken, bRequiresWssToken);
            APIManager.CallRequestEvent(www, data);
            yield return www.SendWebRequest();
            APIManager.CallOnResponseEvent(www, data, www.isNetworkError ? www.error : www.downloadHandler.text);

            fail = www.isNetworkError || www.isHttpError;

            if (fail && retryCount < 10)
            {
                APIManager.ThrowRetryError(www, url, data);
                yield return new WaitForSeconds(10);
                retryCount += 1;
            }
            else
            {
                break;
            }
        }

        CallBack(fail ? www.error : www.downloadHandler.text, Convert.ToInt32(www.responseCode));
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

        www.timeout = 20;

        www.SetRequestHeader("Content-Type", "application/json");
        if (bRequiresLoginToken)
        {
            www.SetRequestHeader("Authorization", LoginAPIManager.loginToken == null ? "" : LoginAPIManager.loginToken);
        }
        if (bRequiresWssToken)
        {
            www.SetRequestHeader("Authorization", LoginAPIManager.wssToken == null ? "" : LoginAPIManager.wssToken);
        }
        //if (PlayerPrefs.HasKey("cookie"))
        //{
        //    Debug.LogError(endpoint + "\n>>>>>>>>>" + PlayerPrefs.GetString("cookie"));
        //    www.SetRequestHeader("Cookie", PlayerPrefs.GetString("cookie"));
        //}

        return www;
    }
}