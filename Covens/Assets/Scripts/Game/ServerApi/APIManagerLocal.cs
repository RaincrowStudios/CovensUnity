using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;



public class APIManagerLocal
{
    public const float WaitDelay = 0.5f;

    public static IEnumerator RequestServerRoutine(string endpoint, string data, string sMethod, bool bRequiresToken, bool bRequiresWssToken, Action<string, int> CallBack)
    {
        endpoint = "LocalApi/" + endpoint;
        endpoint = endpoint.Replace($"{'/'}{'/'}", "/");
        endpoint = endpoint.Replace("?", "{63}");
        endpoint = endpoint.Replace(GetGPS.longitude.ToString(), "{physLon}").Replace(GetGPS.latitude.ToString(), "{physLat}");

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

    public static IEnumerator RequestAnalyticsRoutine(string endpoint, string data, string sMethod, bool bRequiresToken, bool bRequiresWssToken, Action<string, int> CallBack)
    {
        yield return null;
    }

    private static UnityWebRequest BakeRequest(string endpoint, string data, string sMethod)
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
        www.SetRequestHeader("Authorization", LoginAPIManager.loginToken);
        www.SetRequestHeader("Authorization", LoginAPIManager.wssToken);

        return www;
    }



    /// <summary>
    /// parse and send the websocket command
    /// </summary>
    /// <param name="sResponse"></param>
    /// <returns></returns>
    public static string ParseCommand(string sResponse)
    {
        if (!string.IsNullOrEmpty(sResponse) && sResponse.Contains("<#") && sResponse.Contains("#>"))
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
        if (!string.IsNullOrWhiteSpace(sContent))
        {
            CommandResponse commandReponse = new CommandResponse()
            {
                Command = sCommand,
                Data = sContent
            };
            SocketClient.Instance.AddMessage(commandReponse);
            return sContent;
        }
        return string.Empty;
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