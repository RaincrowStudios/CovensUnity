using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class APIManagerServer
{
    public static IEnumerator postHelper(string endpoint, string data, Action<string, int> CallBack)
    {
        UnityWebRequest www = UnityWebRequest.Put(Constants.hostAddressRaincrowLocal + endpoint, data);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");
        APIManager.CallRequestEvent(www, data);

        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.LogError(www.responseCode.ToString());
        }
        else
        {
            Debug.Log(www.responseCode.ToString());
            Debug.Log(www.GetResponseHeader("date") + "11111");
            Debug.Log(www.GetRequestHeader("date"));
            Debug.Log("Received response : " + www.downloadHandler.text);
            CallBack(www.downloadHandler.text, Convert.ToInt32(www.responseCode));
        }
        APIManager.CallOnResponseEvent(www, data, www.downloadHandler.text);

    }

    public static IEnumerator RequestCovenHelper(string endpoint, string data, string sMethod, Action<string, int> CallBack)
    {
        UnityWebRequest www = BakeRequest(Constants.hostAddressLocal + endpoint, data, sMethod);
        Debug.Log("Sending Data : " + data);
        APIManager.CallRequestEvent(www, data);

        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.LogError(www.responseCode.ToString());
        }
        else
        {
            Debug.Log(www.responseCode.ToString());
            Debug.Log(www.GetRequestHeader("HTTP-date"));
            Debug.Log("Received response : " + www.downloadHandler.text);
            CallBack(www.downloadHandler.text, Convert.ToInt32(www.responseCode));
        }

         APIManager.CallOnResponseEvent(www, data, www.downloadHandler.text);
    }

    static UnityWebRequest BakeRequest(string endpoint, string data, string sMethod)
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