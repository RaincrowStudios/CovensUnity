using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// request only responsible class
/// </summary>
public class APIManagerServer
{
    public const float RetryCooldown = 2f;
    public const int MaxRetries = 5;
    public const bool UseBackupServer = true;
    private const bool EnableAutoRetry = true;
    public const int MinBadGatewayErrors = 3;
    public const int BadGatewayErrorResponse = 502;

    public static IEnumerator RequestServerRoutine(string endpoint, string data, string sMethod, bool bRequiresToken, bool bRequiresWssToken, Action<string, int> CallBack)
    {
        string url = string.Concat(CovenConstants.hostAddress + endpoint);
        bool requestError = true;
        int retryCount = 0;
        int badGatewayErrorsCount = 0;
        UnityWebRequest www = null; // BakeRequest(url, data, sMethod, bRequiresToken, bRequiresWssToken);        

        while (requestError && retryCount < MaxRetries)
        {
            www = BakeRequest(url, data, sMethod, bRequiresToken, bRequiresWssToken);
            APIManager.CallRequestEvent(www, data);
            yield return www.SendWebRequest();
            APIManager.CallOnResponseEvent(www, data, www.isNetworkError ? www.error : www.downloadHandler.text);

            requestError = www.isNetworkError;
            retryCount += 1;

            if (requestError)
            {
                if (www.responseCode == BadGatewayErrorResponse)
                {
                    badGatewayErrorsCount += 1;
                }

                if (EnableAutoRetry)
                {
                    APIManager.ThrowRetryError(www, url, data);
                    LoadingOverlay.Show();
                    yield return new WaitForSeconds(RetryCooldown);
                }
            }
            else
            {
                break;
            }
        }

        if (!requestError)
        {
            CallBack(requestError ? www.error : www.downloadHandler.text, Convert.ToInt32(www.responseCode));

            LoadingOverlay.Hide();
        }
        else
        {
            // So, here's what this bit is doing right here:
            // If UseBackupServer is true, it means we will forward requests to the backup server if we have a lot
            // of bad gateway errors
            if (UseBackupServer && !CovenConstants.isBackUpServer && badGatewayErrorsCount >= MinBadGatewayErrors)
            {
                CovenConstants.isBackUpServer = true;
                Debug.LogWarningFormat("[APIManagerServer]: Switching to BACKUP SERVER: {0}", CovenConstants.hostAddress);

                yield return RequestServerRoutine(endpoint, data, sMethod, bRequiresToken, bRequiresWssToken, CallBack);
            }
            else
            {
                APIManager.ThrowCriticalError(www, url, data);
            }
        }
    }

    public static IEnumerator RequestAnalyticsRoutine(string endpoint, string data, string sMethod, bool bRequiresToken, bool bRequiresWssToken, Action<string, int> CallBack)
    {
        string url = string.Concat(CovenConstants.analyticsAddress + endpoint);
        int retryCount = 0;
        bool fail = true;

        UnityWebRequest www = null;

        while (fail && retryCount < MaxRetries)
        {
            www = BakeRequest(url, data, sMethod, bRequiresToken, bRequiresWssToken);
            APIManager.CallRequestEvent(www, data);
            yield return www.SendWebRequest();
            APIManager.CallOnResponseEvent(www, data, www.isNetworkError ? www.error : www.downloadHandler.text);

            fail = www.isNetworkError;
            retryCount += 1;

            if (fail && EnableAutoRetry)
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

        if (fail)
        {
            APIManager.ThrowCriticalError(www, url, data);
        }

        CallBack(fail ? www.error : www.downloadHandler.text, Convert.ToInt32(www.responseCode));

        LoadingOverlay.Hide();
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