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

        string sContent = LoadFile(endpoint);
        if(sContent != null)
        {
            CallBack(sContent, 200);
        }
        else
        {
            CallBack("File not found", 400);
        }
        sContent = ParseCommand(sContent);
        APIManager.CallOnResponseEvent(www, data, sContent);
    }


    public static IEnumerator RequestCovenHelper(string endpoint, string data, string sMethod, Action<string, int> CallBack)
    {
        yield return null;
        endpoint = "LocalApi/" + endpoint;
        // just to log in monitor
        UnityWebRequest www = BakeRequest(endpoint, data, sMethod);
        APIManager.CallRequestEvent(www, data);
        yield return new WaitForSeconds(WaitDelay);

        string sContent = LoadFile(endpoint);
        if (sContent != null)
        {
            CallBack(sContent, 200);
        }
        else
        {
            CallBack("File not found", 400);
        }
        sContent = ParseCommand(sContent);
        APIManager.CallOnResponseEvent(www, data, sContent);
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



    /// <summary>
    /// parse and send the websocket command
    /// </summary>
    /// <param name="sResponse"></param>
    /// <returns></returns>
    public static string ParseCommand(string sResponse)
    {
        if (sResponse.Contains("<#") && sResponse.Contains("#>"))
        {
            int iStart = sResponse.IndexOf("<#") + 2;
            int iEnd = sResponse.IndexOf("#>");
            string sCommand = sResponse.Substring(iStart, iEnd - iStart);
            sResponse = sResponse.Replace(sCommand, "");
            SendCommand(sCommand);
        }
        return sResponse;
    }


    public static string SendCommand(string sCommand)
    {
        string sFile = string.Format("LocalApi/websocket/{0}", sCommand);

        string sContent = LoadFile(sFile);
        if (sContent != null)
        {
            WebSocketClient.Instance.ParseJson(sContent);
            return sContent;
        }
        else
        {

        }
        return "";
    }


    public static string LoadFile(string sPath)
    {
        TextAsset pText = Resources.Load<TextAsset>(sPath);

        string sResponse = null;
        if (pText != null)
        {
            sResponse = pText.text;
        }
        else
        {
            Debug.LogError("File not found: " + sPath);
        }

        // so we can save and use the text again
        Resources.UnloadAsset(pText);


        return sResponse;
    }

}