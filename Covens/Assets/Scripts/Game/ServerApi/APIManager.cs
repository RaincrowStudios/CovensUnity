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

    private static readonly string PutMethod = "PUT";
    private static readonly string PostMethod = "POST";
    private static readonly string GetMethod = "GET";
    private static readonly string DeleteMethod = "DELETE";

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

    private static readonly string RaincrowEndpoint = "raincrow/";

    public void PostRaincrow(string endpoint, string data, Action<string, int> CallBack, bool bRequiresToken, bool bRequiresWssToken)
    {
        StartCoroutine(ServerApi.RequestServerRoutine(string.Concat(RaincrowEndpoint, endpoint), data, PostMethod, bRequiresToken, bRequiresWssToken, CallBack));
    }
    //public void Put(string endpoint, string data, Action<string, int> CallBack, bool bRequiresToken, bool bRequiresWssToken)
    //{
    //    StartCoroutine(ServerApi.RequestServerRoutine(string.Concat(RaincrowEndpoint, endpoint), data, PutMethod, bRequiresToken, bRequiresWssToken, CallBack));
    //}
    //public void Delete(string endpoint, string data, Action<string, int> CallBack)
    //{
    //    StartCoroutine(ServerApi.RequestServerRoutine(string.Concat(RaincrowEndpoint, endpoint), data, DeleteMethod, true, false, CallBack));
    //}
    public void GetRaincrow(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine(string.Concat(RaincrowEndpoint, endpoint), data, GetMethod, true, false, CallBack));
    }
    //public void GetDataRC(string endpoint, Action<string, int> CallBack)
    //{
    //    StartCoroutine(ServerApi.RequestServerRoutine(string.Concat(RaincrowEndpoint, endpoint), "", GetMethod, true, false, CallBack));
    //}
    #endregion


    #region covens requests

    private static readonly string CovensEndpoint = "";

    public void Post(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine(string.Concat(CovensEndpoint, endpoint), data, PostMethod, true, false, CallBack));
    }
    public void Put(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine(string.Concat(CovensEndpoint, endpoint), data, PutMethod, true, false, CallBack));
    }
    public void Delete(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine(string.Concat(CovensEndpoint, endpoint), data, DeleteMethod, true, false, CallBack));
    }
    public void Get(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestServerRoutine(string.Concat(CovensEndpoint, endpoint), data, GetMethod, true, false, CallBack));
    }
    //public void PostCovenSelect(string endpoint, string data, Action<string, int> CallBack)
    //{
    //    StartCoroutine(ServerApi.RequestServerRoutine(string.Concat(CovensEndpoint, endpoint), data, PostMethod, true, false, CallBack));
    //}
    //public void Post(string endpoint, string data, Action<string, int> CallBack)
    //{
    //    StartCoroutine(ServerApi.RequestServerRoutine(string.Concat(CovensEndpoint, endpoint), data, PostMethod, true, false, CallBack));
    //}
    //public void PutData(string endpoint, string data, Action<string, int> CallBack)
    //{
    //    StartCoroutine(ServerApi.RequestServerRoutine(string.Concat(CovensEndpoint, endpoint), data, PutMethod, true, false, CallBack));
    //}
    //public void Delete(string endpoint, Action<string, int> CallBack)
    //{
    //    StartCoroutine(ServerApi.RequestServerRoutine(string.Concat(CovensEndpoint, endpoint), "{}", DeleteMethod, true, false, CallBack));
    //}
    public void Get(string endpoint, Action<string, int> CallBack)
    {
        Get(endpoint, "", CallBack);
    }

    #endregion

    #region Analytics

    public void PostAnalytics(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(ServerApi.RequestAnalyticsRoutine(endpoint, data, PostMethod, true, false, CallBack));
    }

    #endregion        

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
