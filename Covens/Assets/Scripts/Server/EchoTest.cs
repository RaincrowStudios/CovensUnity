using UnityEngine;
using System.Collections;
using System;

public class EchoTest : MonoBehaviour {


//	IEnumerator Start ()
//	{
//
//		string URL = "https://35.227.17.69:8080";
//		WWW www = new WWW (URL);
//		yield return www;
//		if (www.error == null) {
//			print (www.text);
//		} else {
//			print (www.error);
//		}
//
//	}
//

	// Use this for initialization
	IEnumerator Start () {





		WebSocket w = new WebSocket(new Uri("wss://35.227.17.69:8080/path?param=2"));




		yield return StartCoroutine(w.Connect());
		w.SendString("Hi there");
		int i=0;
		while (true)
		{
			string reply = w.RecvString();
			if (reply != null)
			{
				Debug.Log ("Received: "+reply);
				w.SendString("Hi there"+i++);
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
}
