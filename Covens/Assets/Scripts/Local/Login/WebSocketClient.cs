using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class WebSocketClient : MonoBehaviour
{
	public static WebSocketClient Instance { get; set;}
	// Use this for initialization

	void Awake()
	{
		Instance = this;
	}

	public void InitiateWSSCOnnection()
	{
		StartCoroutine (EstablishWSSConnection ());
	}

//	IEnumerator EstablishWSConnection () {
//		{
//
//			using (WWW www = new WWW (Constants.wsAddress)) {
//				yield return www;
//				if (www.error == null) {
//					print (www.text);
//					StartCoroutine (EstablishWSSConnection ());
//
//				}
//				print (www.error);
//
//			}
//		}
//	}

	IEnumerator EstablishWSSConnection () {
		WebSocket w = new WebSocket(new Uri(Constants.wssAddress + LoginAPIManager.wssToken));

		yield return StartCoroutine(w.Connect());

		while (true)
		{
			string reply = w.RecvString();
			if (reply != null)
			{
				Debug.Log (reply);
				ParseJson (reply);
			}
			if (w.error != null)
			{
				Debug.LogError ("Error: "+w.error);
				break;
			}
			yield return 0;
		}
		w.Close();
	}

	void ParseJson(string jsonText)
	{
		try {
	
		MarkerData data = JsonConvert.DeserializeObject<MarkerData> (jsonText);
		if (data.command == "map_add") {
			SpiritMovementFX.Instance.MoveSpirit (data);
		}
		} catch (Exception ex) {
			Debug.LogError (ex);
		}
	}
}



