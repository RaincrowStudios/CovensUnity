using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class WebSocketClient : MonoBehaviour
{
	public static WebSocketClient Instance { get; set; }
	// Use this for initialization

	void Awake ()
	{
		Instance = this;
	}

	public void InitiateWSSCOnnection ()
	{
		StartCoroutine (EstablishWSConnection ());
	}

	IEnumerator EstablishWSConnection () {
		{

			using (WWW www = new WWW (Constants.wsAddress)) {
				yield return www;
				if (www.error == null) {
					print (www.text + "From Web Socket HTTP");
					StartCoroutine (EstablishWSSConnection ());

				}
				print (www.error);

			}
		}
	}

	IEnumerator EstablishWSSConnection ()
	{
		print (LoginAPIManager.wssToken);
		WebSocket w = new WebSocket (new Uri (Constants.wssAddress + LoginAPIManager.wssToken));

		yield return StartCoroutine (w.Connect ());

		while (true) {
			string reply = w.RecvString ();
			if (reply != null) {
				if (reply != "200") {
					print (reply);
					ParseJson (reply);
				}
			}
			if (w.error != null) {
				Debug.LogError ("Error: " + w.error);
				break;
			}
			yield return 0;
		}
		w.Close ();
	}

	void ParseJson (string jsonText)
	{
		try {
			WebSocketResponse response = JsonConvert.DeserializeObject<WebSocketResponse> (jsonText);
			if (response.command == "map_spirit_remove") {
				print("Removing ID : " + response.instance); 
				SpiritMovementFX.Instance.SpiritRemove (response.instance); 
				return;
			}
			else if(response.command == "map_portal_remove" || response.command == "map_character_remove" )
			{
				if(MarkerSpawner.instanceID == response.instance)
					return;
				MarkerManager.DeleteMarker (response.instance);
				return;
			}
			if(response.tokens != null){
			foreach (var data in response.tokens) {
				if (response.command == "map_spirit_add") {
						var updatedData = MarkerManagerAPI.AddEnumValueSingle(data);
						print("Moving ID : " + updatedData.instance);
						SpiritMovementFX.Instance.MoveSpirit (updatedData);
				}  else if (response.command == "map_spirit_action"){
					SpiritMovementFX.Instance.SpiritAttack(data.instance,data.target,data.dead);
					} else if(response.command == "map_portal_add" || response.command == "map_character_add")
				{
					var updatedData = MarkerManagerAPI.AddEnumValueSingle(data);
					MarkerSpawner.Instance.AddMarker(updatedData);
				} 
			}
			}

	
			
		} catch (Exception ex) {
			Debug.LogError (ex);
		}
	}
}



