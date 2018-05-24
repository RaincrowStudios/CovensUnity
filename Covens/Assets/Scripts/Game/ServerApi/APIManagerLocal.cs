using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;



public class APIManagerLocal
{
    public const float WaitDelay = 1f;

    public static IEnumerator postHelper(string endpoint, string data, Action<string, int> CallBack)
    {
        endpoint = "LocalApi/" + endpoint;
        // just to log in monitor
        UnityWebRequest www = BakeRequest(endpoint, data, "POST");
        APIManager.CallRequestEvent(www, data);
        yield return new WaitForSeconds(WaitDelay);

        TextAsset pText = Resources.Load<TextAsset>(endpoint);
        yield return null;

        // success callback
        string sResponse = null;
        if (pText != null)
        {
            sResponse = pText.text;
            CallBack(sResponse, 200);
        }
        else
        {
            CallBack("File not found", 400);
            Debug.LogError("File not found: " + endpoint);
        }

        // so we can save and use the text again
        Resources.UnloadAsset(pText);

        APIManager.CallOnResponseEvent(www, data, sResponse);
    }

    public static IEnumerator RequestCovenHelper(string endpoint, string data, string sMethod, Action<string, int> CallBack)
    {
        yield return null;
        endpoint = "LocalApi/" + endpoint;
        // just to log in monitor
        UnityWebRequest www = BakeRequest(endpoint, data, sMethod);
        APIManager.CallRequestEvent(www, data);
        yield return new WaitForSeconds(WaitDelay);

        TextAsset pText = Resources.Load<TextAsset>(endpoint);
        yield return null;

        // success callback
        string sResponse = null;
        if (pText != null)
        {
            sResponse = pText.text;
            CallBack(sResponse, 200);
        }
        else
        {
            CallBack("File not found", 400);
            Debug.LogError("File not found: " + endpoint);
        }

        // so we can save and use the text again
        Resources.UnloadAsset(pText);

        APIManager.CallOnResponseEvent(www, data, sResponse);
    }

    static  UnityWebRequest BakeRequest(string endpoint, string data, string sMethod)
    {
        UnityWebRequest www = UnityWebRequest.Put(endpoint, data);
        Debug.Log(endpoint);
        www.method = sMethod;
        string bearer = "Bearer " + LoginAPIManager.loginToken;
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", bearer);
        Debug.Log("Sending Data : " + data);
        return www;
    }

}