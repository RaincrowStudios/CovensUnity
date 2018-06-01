using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class APIManagerServer
{
    public static IEnumerator RequestRoutine(string endpoint, string data, string sMethod, bool bRequiresToken, Action<string, int> CallBack)
    {
        // build the request
        string sUrl = Constants.hostAddress + endpoint;
        UnityWebRequest www = BakeRequest(sUrl, data, sMethod, bRequiresToken);
        APIManager.CallRequestEvent(www, data);

        // request
        yield return www.SendWebRequest();

        // receive the response
        APIManager.CallOnResponseEvent(www, data, www.downloadHandler.text);
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
    }
    static UnityWebRequest BakeRequest(string endpoint, string data, string sMethod, bool bRequiresToken)
    {
        // log it
        string sRequest = "==> BakeRequest for: " + endpoint;
        sRequest += "\n  endpoint: " + endpoint;
        sRequest += "\n  method: " + sMethod;
        sRequest += "\n  data: " + data;
        Debug.Log(sRequest);

        UnityWebRequest www = UnityWebRequest.Put(endpoint, data);
        www.method = sMethod;
        www.SetRequestHeader("Content-Type", "application/json");
        if (bRequiresToken)
        {
            string bearer = "Bearer " + LoginAPIManager.loginToken;
            www.SetRequestHeader("Authorization", bearer);
        }

        return www;
    }






    /*
    public static IEnumerator RequestHelper(string endpoint, string data, string sMethod, bool bRequiresToken, Action<string, int> CallBack)
    {
        UnityWebRequest www = BakeRequest(Constants.hostAddressLocal + endpoint, data, sMethod, bRequiresToken);
        APIManager.CallRequestEvent(www, data);

        yield return www.SendWebRequest();

        APIManager.CallOnResponseEvent(www, data, www.downloadHandler.text);

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
    }

    public static IEnumerator RequestCovenHelper(string endpoint, string data, string sMethod, bool bRequiresToken, Action<string, int> CallBack)
    {
        yield return RequestRoutine("covens/" + endpoint, data, sMethod, bRequiresToken, CallBack);
        UnityWebRequest www = BakeRequest(Constants.hostAddressLocal + endpoint, data, sMethod, bRequiresToken);
        APIManager.CallRequestEvent(www, data);

        yield return www.SendWebRequest();

        APIManager.CallOnResponseEvent(www, data, www.downloadHandler.text);


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
        }*/
    


}