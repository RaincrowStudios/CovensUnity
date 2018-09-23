using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
public class TestReq : MonoBehaviour {

	IEnumerator Start ()
	{
		var data = new PlayerLoginAPI ();   
		data.username = "asssd"; 
		data.password = "asssd"; 
		data.email = "asd"; 
		data.game = "covens";  
//		data.lat = 0;
//		data.lng = 0; 
		data.UID = SystemInfo.deviceUniqueIdentifier;


		UnityWebRequest www = new UnityWebRequest (Constants.hostAddress + "/raincrow/create-account", JsonConvert.SerializeObject(data)); 
		www.method = "PUT";
		www.SetRequestHeader("Content-Type", "application/json");
		print (JsonConvert.SerializeObject (data));
		yield return www.SendWebRequest();
		if (www.isNetworkError|| www.isHttpError) 
		{
			Debug.LogError(www.error + www.responseCode.ToString());
		}
		else
		{
			print("Received response : " + www.downloadHandler.text);
		}

	}

}
