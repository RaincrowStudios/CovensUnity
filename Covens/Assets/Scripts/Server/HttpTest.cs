using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;

public class HttpTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (Upload ());
	}
	
	IEnumerator Upload() {
		var log = new Login ();
		log.username = "lame!";
		var s = JsonUtility.ToJson (log);
		UnityWebRequest www = UnityWebRequest.Put("http://raincrow-pantheon.appspot.com/api/raincrow/check-username", s);
		www.method = "POST";
		www.SetRequestHeader ("Content-Type", "application/json");
		yield return www.SendWebRequest();
		if(www.isNetworkError || www.isHttpError) {
			Debug.Log(www.error);
		}
		else {
			print (www.downloadHandler.text);
		
		}
			
	}

}

[Serializable]
public class Login
{
	public string username;
}