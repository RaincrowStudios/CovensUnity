using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
public class APIManager : MonoBehaviour
{
	public static APIManager Instance { get; set;}

    public static event Action<UnityWebRequest> OnRequestEvt;
    public static event Action<UnityWebRequest, string> OnResponseEvt;

	void Awake()
	{
		Instance = this;
	}

	#region Login /raincrowEndpoint

	public void Post(string endpoint, string data , Action<string,int> CallBack)
	{
		StartCoroutine(postHelper(endpoint,data,CallBack));
	}

	IEnumerator postHelper(string endpoint, string data , Action<string,int> CallBack )
	{
		UnityWebRequest www = UnityWebRequest.Put(Constants.hostAddressRaincrow + endpoint, data);
		www.method = "POST";
		www.SetRequestHeader ("Content-Type", "application/json");
		print ("Sending Data : " + data);
		if (OnRequestEvt != null)
			OnRequestEvt(www);

		yield return www.SendWebRequest();
		if(www.isNetworkError) {
			Debug.LogError (www.responseCode.ToString());
		}
		else {
			print (www.responseCode.ToString());
			print("Received response : " + www.downloadHandler.text);
			CallBack (www.downloadHandler.text, Convert.ToInt32(www.responseCode));
		}
		if (OnResponseEvt != null)
			OnResponseEvt(www, data);
	}

	public void Put(string endpoint, string data , Action<string,int> CallBack, bool isToken = false)
	{
		StartCoroutine(putHelper(endpoint,data,CallBack,isToken));
	}

	IEnumerator putHelper(string endpoint, string data , Action<string,int> CallBack , bool isToken)
	{
		UnityWebRequest www = UnityWebRequest.Put(Constants.hostAddressRaincrow + endpoint, data);
		www.SetRequestHeader ("Content-Type", "application/json");
		if (isToken) {
			string bearer = "Bearer " + LoginAPIManager.loginToken;
			www.SetRequestHeader ("Authorization", bearer);
		}
		print ("Sending Data : " + data);
		if (OnRequestEvt != null)
			OnRequestEvt(www);
		yield return www.SendWebRequest();
		if(www.isNetworkError) {
			Debug.LogError (www.responseCode.ToString());
		}
		else {
			print("Received response : " + www.downloadHandler.text);
			if (www.responseCode == 400) {
				try{
				CallBack (www.downloadHandler.text, Convert.ToInt32 (www.downloadHandler.text));
				}catch{
				CallBack (www.downloadHandler.text, 000);
				}
			} else {
				CallBack (www.downloadHandler.text, 200);
			}
		}
		if (OnResponseEvt != null)
			OnResponseEvt(www, data);
	}

	#endregion

	#region Coven /CovenEndpoint

	public void PostCoven(string endpoint, string data , Action<string,int> CallBack)
	{
		StartCoroutine(PostCovenHelper(endpoint,data,CallBack));
	}

	IEnumerator PostCovenHelper(string endpoint, string data , Action<string,int> CallBack )
	{
		UnityWebRequest www = UnityWebRequest.Put(Constants.hostAddress	 + endpoint, data);
		www.method = "POST";
		string bearer = "Bearer " + LoginAPIManager.loginToken;
		www.SetRequestHeader ("Content-Type", "application/json");
		www.SetRequestHeader ("Authorization", bearer);
		print ("Sending Data : " + data);
		if (OnRequestEvt != null)
			OnRequestEvt(www);

		yield return www.SendWebRequest();
		if(www.isNetworkError) {
			Debug.LogError (www.responseCode.ToString());
		}
		else {
			print (www.responseCode.ToString());
			print ("Received response : " + www.downloadHandler.text);
			CallBack (www.downloadHandler.text, Convert.ToInt32(www.responseCode));
		}

		if (OnResponseEvt != null)
			OnResponseEvt(www, data);
	}

    public void PutCoven(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(PutCovenHelper(endpoint, data, CallBack));
    }

    IEnumerator PutCovenHelper(string endpoint, string data, Action<string, int> CallBack)
    {
        UnityWebRequest www = UnityWebRequest.Put(Constants.hostAddressLocal + endpoint, data);
        print(Constants.hostAddressLocal + endpoint);
        www.method = "PUT";
        string bearer = "Bearer " + LoginAPIManager.loginToken;
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", bearer);
        print("Sending Data : " + data);
        if (OnRequestEvt != null)
            OnRequestEvt(www);

        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.LogError(www.responseCode.ToString());
        }
        else
        {
            print(www.responseCode.ToString());
            print(www.GetRequestHeader("HTTP-date"));
            print("Received response : " + www.downloadHandler.text);
            CallBack(www.downloadHandler.text, Convert.ToInt32(www.responseCode));
        }

        if (OnResponseEvt != null)
            OnResponseEvt(www, data);
    }

    public void GetCoven(string endpoint, string data, Action<string, int> CallBack)
    {
        StartCoroutine(GetCovenHelper(endpoint, data, CallBack));
    }

    IEnumerator GetCovenHelper(string endpoint, string data, Action<string, int> CallBack)
    {
        UnityWebRequest www = UnityWebRequest.Put(Constants.hostAddressLocal + endpoint, data);
        print(Constants.hostAddressLocal + endpoint);
        www.method = "GET";
        string bearer = "Bearer " + LoginAPIManager.loginToken;
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", bearer);
        print("Sending Data : " + data);
        if (OnRequestEvt != null)
            OnRequestEvt(www);

        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.LogError(www.responseCode.ToString());
        }
        else
        {
            print(www.responseCode.ToString());
            print(www.GetRequestHeader("HTTP-date"));
            print("Received response : " + www.downloadHandler.text);
            CallBack(www.downloadHandler.text, Convert.ToInt32(www.responseCode));
        }

        if (OnResponseEvt != null)
            OnResponseEvt(www, data);
    }

    public void PostCovenSelect(string endpoint, string data , Action<string,int,MarkerSpawner.MarkerType> CallBack ,  MarkerSpawner.MarkerType type)
	{
		StartCoroutine(PostCovenSelectHelper(endpoint,data,CallBack,type));
	}

	IEnumerator PostCovenSelectHelper(string endpoint, string data , Action<string,int,MarkerSpawner.MarkerType> CallBack, MarkerSpawner.MarkerType type )
	{
		UnityWebRequest www = UnityWebRequest.Put(Constants.hostAddress	 + endpoint, data);
		www.method = "POST";
		string bearer = "Bearer " + LoginAPIManager.loginToken;
		www.SetRequestHeader ("Content-Type", "application/json");
		www.SetRequestHeader ("Authorization", bearer);
		print ("Sending Data : " + data);
		if (OnRequestEvt != null)
			OnRequestEvt(www);

		yield return www.SendWebRequest();
		if(www.isNetworkError) {
			Debug.LogError(www.error + www.responseCode.ToString());
		}
		else {
			print( www.GetRequestHeader ("HTTP-date"));
			print (www.responseCode.ToString());
			print ("Received response : " + www.downloadHandler.text);
			CallBack (www.downloadHandler.text, Convert.ToInt32(www.responseCode),type);
		}

		if (OnResponseEvt != null)
			OnResponseEvt(www, data);

	}
	#endregion


}

