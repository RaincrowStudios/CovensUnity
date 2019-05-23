using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;


#if LOCAL_API
using ServerApi = APIManagerLocal;
#else
using ServerApi = APIManagerServer;
#endif

/// <summary>
/// TODO: check if we need thos functionalities:
///     - Timeout
///     - crypt or gzip the data
/// </summary>
public class APIManager : Patterns.SingletonComponent<APIManager>
{
    public static event Action<UnityWebRequest, string> OnRequestEvt;
    public static event Action<UnityWebRequest, string, string> OnResponseEvt;

    public ApiServerData ServerData;

    private const bool localAPI =
#if LOCAL_API
            true;
#else
            false;
#endif

    public override void Awake()
    {
        base.Awake();

        //ServerData = ApiServerData.Load();
    }


    public static void CallRequestEvent(UnityWebRequest pReq, string sRequestData)
    {
        if (OnRequestEvt != null)
            OnRequestEvt(pReq, sRequestData);
    }
    public static void CallOnResponseEvent(UnityWebRequest pRequest, string sRequestData, string sResponseData)
    {
        if (OnResponseEvt != null)
            OnResponseEvt(pRequest, sRequestData, sResponseData);
    }


    #region raincrow requests

    public void Post(string endpoint, string data, Action<string, int> CallBack, bool bRequiresToken, bool bRequiresWssToken)
    {
        StartCoroutine(ServerApi.RequestServerRoutine("raincrow/" + endpoint, data, "POST", bRequiresToken, bRequiresWssToken, CallBack));
    }
    public void Put(string endpoint, string data, Action<string, int> CallBack, bool bRequiresToken, bool bRequiresWssToken)
    {
        StartCoroutine(ServerApi.RequestServerRoutine("raincrow/" + endpoint, data, "PUT", bRequiresToken, bRequiresWssToken, CallBack));
    }
    public void Delete(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine("raincrow/" + endpoint, data, "DELETE", true, false, CallBack));
    }
    public void Get(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine("raincrow/" + endpoint, data, "GET", true, false, CallBack));
    }
    #endregion


    #region covens requests

    public void PostCoven(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine("covens/" + endpoint, data, "POST", true, false, CallBack));
    }
    public void PutCoven(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine("covens/" + endpoint, data, "PUT", true, false, CallBack));
    }
    public void DeleteCoven(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine("covens/" + endpoint, data, "DELETE", true, false, CallBack));
    }
    public void GetCoven(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine("covens/" + endpoint, data, "GET", true, false, CallBack));
    }

    #endregion

    #region Analytics

    public void PostAnalytics(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestAnalyticsRoutine(endpoint, data, "POST", true, false, CallBack));
    }

    #endregion

    // 

    public void PostCovenSelect(string endpoint, string data, Action<string, int, MarkerSpawner.MarkerType> CallBack, MarkerSpawner.MarkerType type)
    {
        if (localAPI)
            StartCoroutine(ServerApi.RequestServerRoutine("covens/" + endpoint, data, "POST", true, false, (s, i) => CallBack(s, i, type)));
        else
            StartCoroutine(PostCovenSelectHelper(endpoint, data, CallBack, type));
    }

    IEnumerator PostCovenSelectHelper(string endpoint, string data, Action<string, int, MarkerSpawner.MarkerType> CallBack, MarkerSpawner.MarkerType type)
    {
        UnityWebRequest www = UnityWebRequest.Put(CovenConstants.hostAddress + "covens/" + endpoint, data);
        www.method = "POST";
        string bearer = "Bearer " + LoginAPIManager.loginToken;
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", bearer);
        //        Debug.Log("Sending Data : " + data);
        //		Debug.Log (CovenConstants.hostAddress + endpoint);
        if (OnRequestEvt != null)
            OnRequestEvt(www, data);

        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.LogError(www.error + www.responseCode.ToString());
        }
        else
        {
            if (www.downloadHandler.text == "4700")
            {
                PlayerManager.Instance.initStart();
            }
            //            Debug.Log(www.GetRequestHeader("HTTP-date"));
            //            Debug.Log(www.responseCode.ToString());
            //            Debug.Log("Received response : " + www.downloadHandler.text);
            CallBack(www.downloadHandler.text, Convert.ToInt32(www.responseCode), type);
        }

        if (OnResponseEvt != null)
            OnResponseEvt(www, data, www.downloadHandler.text);

    }

    public void PostData(string endpoint, string data, Action<string, int> CallBack)
    {
        if (localAPI)
            StartCoroutine(ServerApi.RequestServerRoutine("covens/" + endpoint, data, "POST", true, false, CallBack));
        else
            StartCoroutine(PostDataHelper(endpoint, data, CallBack));
    }

    IEnumerator PostDataHelper(string endpoint, string data, Action<string, int> CallBack)
    {
        UnityWebRequest www = UnityWebRequest.Put(CovenConstants.hostAddress + "covens/" + endpoint, data);

        www.method = "POST";

        string bearer = "Bearer " + LoginAPIManager.loginToken;
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", bearer);
        string sRequest = "==> BakeRequest for: " + endpoint;
        sRequest += "\n  endpoint: " + CovenConstants.hostAddress + "covens/" + endpoint;
        sRequest += "\n  method: " + ("POST");
        sRequest += "\n  data: " + data;
        sRequest += "\n  loginToken: " + LoginAPIManager.loginToken;
        Debug.Log(sRequest);

        if (OnRequestEvt != null)
            OnRequestEvt(www, data);

        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.LogError(www.error + www.responseCode.ToString());
            CallBack(www.error, Convert.ToInt32(www.responseCode));
        }
        else
        {
            if (www.downloadHandler.text == "4700")
            {
                PlayerManager.Instance.initStart();
            }
            //Debug.Log("Received response : " + www.downloadHandler.text);
            CallBack(www.downloadHandler.text, Convert.ToInt32(www.responseCode));
        }

        if (OnResponseEvt != null)
            OnResponseEvt(www, data, www.downloadHandler.text);

    }

    public void PutData(string endpoint, string data, Action<string, int> CallBack)
    {
        if (localAPI)
            StartCoroutine(ServerApi.RequestServerRoutine("covens/" + endpoint, data, "PUT", true, false, CallBack));
        else
            StartCoroutine(PutDataHelper(endpoint, data, CallBack));
    }

    IEnumerator PutDataHelper(string endpoint, string data, Action<string, int> CallBack)
    {
        UnityWebRequest www = UnityWebRequest.Put(CovenConstants.hostAddress + "covens/" + endpoint, data);


        string bearer = "Bearer " + LoginAPIManager.loginToken;
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", bearer);
        string sRequest = "==> BakeRequest for: " + endpoint;
        //		sRequest += "\n  endpoint: " + CovenConstants.hostAddress + "covens/" + endpoint;
        //		sRequest += "\n  method: " + ("POST");
        //		sRequest += "\n  data: " + data;
        //		sRequest += "\n  loginToken: " + LoginAPIManager.loginToken;
        //		Debug.Log(sRequest);

        if (OnRequestEvt != null)
            OnRequestEvt(www, data);

        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.LogError(www.error + www.responseCode.ToString());
        }
        else
        {
            if (OnResponseEvt != null)
                OnResponseEvt(www, "", www.downloadHandler.text);

            if (www.downloadHandler.text == "4700")
            {
                PlayerManager.Instance.initStart();
            }
            CallBack(www.downloadHandler.text, Convert.ToInt32(www.responseCode));
        }


    }

    public void DeleteData(string endpoint, Action<string, int> CallBack)
    {
        if (localAPI)
            StartCoroutine(ServerApi.RequestServerRoutine("covens/" + endpoint, "", "DELETE", true, false, CallBack));
        else
            StartCoroutine(DeleteDataHelper(endpoint, CallBack));
    }

    IEnumerator DeleteDataHelper(string endpoint, Action<string, int> CallBack)
    {
        UnityWebRequest www = UnityWebRequest.Delete(CovenConstants.hostAddress + "covens/" + endpoint);
        www.downloadHandler = new DownloadHandlerBuffer();
        //www.method = "DELETE";

        string bearer = "Bearer " + LoginAPIManager.loginToken;
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", bearer);
        string sRequest = "==> BakeRequest for: " + endpoint;
        //		sRequest += "\n  endpoint: " + CovenConstants.hostAddress + "covens/" + endpoint;
        //		sRequest += "\n  method: " + ("POST");
        //		sRequest += "\n  data: " + data;
        //		sRequest += "\n  loginToken: " + LoginAPIManager.loginToken;
        //		Debug.Log(sRequest);

        if (OnRequestEvt != null)
            OnRequestEvt(www, "");

        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.LogError(www.error + www.responseCode.ToString());
        }
        else
        {
            //			Debug.Log("Received response : " + www.downloadHandler.text);
            CallBack(www.downloadHandler.text, Convert.ToInt32(www.responseCode));
        }

        if (OnResponseEvt != null)
            OnResponseEvt(www, "", www.downloadHandler.text);
    }

    public void GetData(string endpoint, Action<string, int> CallBack)
    {
        if (localAPI)
            StartCoroutine(ServerApi.RequestServerRoutine("covens/" + endpoint, "", "GET", true, false, CallBack));
        else
            StartCoroutine(GetDataHelper(endpoint, CallBack));
    }

    IEnumerator GetDataHelper(string endpoint, Action<string, int> CallBack)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(CovenConstants.hostAddress + "covens/" + endpoint))
        {
            string bearer = "Bearer " + LoginAPIManager.loginToken;
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", bearer);

            string sRequest = "==> BakeRequest for: " + endpoint;
            sRequest += "\n  endpoint: " + CovenConstants.hostAddress + "covens/" + endpoint;
            sRequest += "\n  loginToken: " + LoginAPIManager.loginToken;
            //			Debug.Log(sRequest);

            if (OnRequestEvt != null)
                OnRequestEvt(www, "");

            yield return www.Send();
            //
            //			if (www.isNetworkError || www.isHttpError)
            //			{
            //				Debug.Log(www.error);
            //			}
            //			else
            //			{
            if (www.downloadHandler.text == "4700")
            {
                PlayerManager.Instance.initStart();
            }
            //				Debug.Log(www.downloadHandler.text);
            CallBack(www.downloadHandler.text, Convert.ToInt32(www.responseCode));

            //			}

            if (OnResponseEvt != null)
                OnResponseEvt(www, "", www.downloadHandler.text);
        }
    }


    public void GetDataRC(string endpoint, Action<string, int> CallBack)
    {
        StartCoroutine(GetDataRCHelper(endpoint, CallBack));
    }

    IEnumerator GetDataRCHelper(string endpoint, Action<string, int> CallBack)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(CovenConstants.hostAddress + "raincrow/" + endpoint))
        {
            string bearer = "Bearer " + LoginAPIManager.loginToken;
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", bearer);

            string sRequest = "==> BakeRequest for: " + endpoint;
            sRequest += "\n  endpoint: " + CovenConstants.hostAddress + "raincrow/" + endpoint;
            sRequest += "\n  loginToken: " + LoginAPIManager.loginToken;
            Debug.Log(sRequest);

            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                CallBack(www.downloadHandler.text, Convert.ToInt32(www.responseCode));

                //			}
            }
        }

    }
}
