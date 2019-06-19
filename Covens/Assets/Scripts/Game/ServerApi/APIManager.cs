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

    private const bool localAPI =
#if LOCAL_API
            true;
#else
            false;
#endif


    public override void Awake()
    {
        base.Awake();
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


    public void PostCovenSelect(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine("covens/" + endpoint, data, "POST", true, false, CallBack));
    }

    public void PostData(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine("covens/" + endpoint, data, "POST", true, false, CallBack));
    }

    public void PutData(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine("covens/" + endpoint, data, "PUT", true, false, CallBack));
    }

    public void DeleteData(string endpoint, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine("covens/" + endpoint, "{}", "DELETE", true, false, CallBack));
    }

    public void GetData(string endpoint, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine("covens/" + endpoint, "", "GET", true, false, CallBack));
    }


    public void GetDataRC(string endpoint, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine("raincrow/" + endpoint, "", "GET", true, false, CallBack));
    }

    public static void ThrowRetryError(UnityWebRequest www, string url, string data)
    {
        string debugString = $"{www.url}  (Error {www.responseCode}: {www.error})";
        Debug.LogError("Retring request.\n" + debugString);
    }

    public static void ThrowCriticalError(UnityWebRequest www, string url, string data)
    {
        string debugString = $"{www.url}  (Error {www.responseCode}: {www.error})";
        Debug.LogError("TODO: RETURN TO LOGIN SCREEN? " + debugString);
    }
}
