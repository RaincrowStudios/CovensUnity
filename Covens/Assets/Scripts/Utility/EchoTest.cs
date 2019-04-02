using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;

public class EchoTest : MonoBehaviour {


//	IEnumerator Start ()
//	{
//
//		string URL = "https://35.227.17.69:8080";
//		WWW www = new WWW (URL);
//		yield return www;
//		if (www.error == null) {
//			Debug.Log (www.text);
//		} else {
//			Debug.Log (www.error);
//		}
//
//	}
//

	// Use this for initialization
	IEnumerator Start () {
		
		WebSocket w = new WebSocket(new Uri("ws://localhost:1000/Chat"));

		yield return StartCoroutine(w.Connect());


		ChatData CD = new ChatData {
			Name = "grim",
			Coven = "asd",
			Dominion = "asdfgas",
			CommandRaw = Commands.Connected.ToString()
		};


		w.Send (System.Text.Encoding.UTF8.GetBytes( JsonConvert.SerializeObject (CD)));

		while (true) {
			string reply = w.RecvString ();
			if (reply != null) {
					Debug.Log (reply);
			}
			if (w.error != null) {
				Debug.LogError ("Error: " + w.error);
				break;
			}
			yield return 0;
		}
		w.Close ();
	}

	
}
