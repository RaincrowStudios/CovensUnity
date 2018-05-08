using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;

public class WebSocketClient : MonoBehaviour
{
	public static WebSocketClient Instance { get; set; }
	// Use this for initialization
	MovementManager MM;
	public bool ShowMessages = false;
	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{
		MM = MovementManager.Instance;
	}

	public void InitiateWSSCOnnection ()
	{
		StartCoroutine (EstablishWSConnection ());
	}

	IEnumerator EstablishWSConnection ()
	{
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
//		print (LoginAPIManager.wssToken);
		WebSocket w = new WebSocket (new Uri (Constants.wssAddress + LoginAPIManager.wssToken));

		yield return StartCoroutine (w.Connect ());

		while (true) {
			string reply = w.RecvString ();
			if (reply != null) {
				if (reply != "200") {
					if(ShowMessages)
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

	Vector2 ReturnVector2 (Token data)
	{
		return new Vector2 (data.longitude, data.latitude);
	}

	void ParseJson (string jsonText)
	{
		try {
			WebSocketResponse response = JsonConvert.DeserializeObject<WebSocketResponse> (jsonText);

			if(response.command!= "map_spirit_move")
				print(jsonText);

			if (OnPlayerSelect.currentView == CurrentView.MapView) {
				
				if (response.command == "map_portal_remove" || response.command == "map_spirit_death" || response.command == "map_collectible_remove" || response.command == "map_spirit_expire") {
					print(jsonText);
					MM.RemoveMarker (response.instance);
				} else if (response.command == "map_spirit_move" || response.command == "map_character_move") {
					if (MarkerManager.Markers.ContainsKey (response.token.instance)) {
//						print("Contains Spirit");
						double distance = OnlineMapsUtils.DistanceBetweenPointsD (PlayerDataManager.playerPos, ReturnVector2 (response.token));
						if (distance < PlayerDataManager.DisplayRadius) {
							MM.UpdateMarkerPosition (response.token);	
//							print("Spirit In Range");
						} else {
//							print("spirit out of range. deleting");
							MM.RemoveMarker (response.token.instance);
						}
					} else {
//						print("adding new spirit");
//						print(jsonText);
						var updatedData = MarkerManagerAPI.AddEnumValueSingle (response.token);
						MM.AddMarker (updatedData);
					}
				} else if (response.command == "map_spirit_action") {
						
				} else if (response.command == "map_portal_add" || response.command == "map_spirit_summon" || response.command == "map_collectible_add") {
//					print(jsonText);
					var updatedData = MarkerManagerAPI.AddEnumValueSingle (response.token);
					MM.AddMarker (updatedData);
				} 
				
			} 
			else if (OnPlayerSelect.currentView == CurrentView.IsoView) {
				{
					if (response.command == "map_portal_remove" || response.command == "map_spirit_death" || response.command == "map_collectible_remove" || response.command == "map_spirit_expire") {
						MM.RemoveMarker (response.instance);
					} else if (response.command == "map_spirit_move" || response.command == "map_character_move") {
						
						if (MarkerManager.Markers.ContainsKey (response.token.instance)) {
							double distance = OnlineMapsUtils.DistanceBetweenPointsD (PlayerDataManager.playerPos, ReturnVector2 (response.token));
							if (MarkerSpawner.instanceID != response.token.instance) {
								if (distance < PlayerDataManager.DisplayRadius) {
									MM.UpdateMarkerPosition (response.token);
								} else {
									MM.RemoveMarker (response.token.instance);
								}
							} else {
								if (distance < PlayerDataManager.DisplayRadius) {
									MM.UpdateMarkerPositionIso(response.token);
									if (distance > PlayerDataManager.attackRadius) {
										//spirit Escaped
									} 
								}
							}
						} else {
							var updatedData = MarkerManagerAPI.AddEnumValueSingle (response.token);
							MM.AddMarker (updatedData);
						}
					} else if(response.command == "map_portal_add" || response.command == "map_spirit_summon" || response.command == "map_collectible_add")
					{
						var updatedData = MarkerManagerAPI.AddEnumValueSingle (response.token);
						MM.AddMarker (updatedData);
					} else if(response.command == "character_spell_success")
					{
						print(jsonText);
							AttackVisualFXManager.Instance.Attack(response);
					}
						
				} 
			} 
			else{
				if (response.command == "map_portal_remove" || response.command == "map_spirit_death" || response.command == "map_collectible_remove" || response.command == "map_spirit_expire") {
					MM.RemoveMarkerIso (response.instance);
				} else if (response.command == "map_spirit_move" || response.command == "map_character_move") {
					if (MarkerManager.Markers.ContainsKey (response.token.instance)) {
						double distance = OnlineMapsUtils.DistanceBetweenPointsD (PlayerDataManager.playerPos, ReturnVector2 (response.token));
						if (distance < PlayerDataManager.DisplayRadius) {
							MM.UpdateMarkerPositionIso (response.token);	
						} else {
							MM.RemoveMarkerIso (response.token.instance);
						}
					} else {
						var updatedData = MarkerManagerAPI.AddEnumValueSingle (response.token);
						MM.AddMarkerIso (updatedData);
					}
				}
					
			}

		} catch (Exception ex) {
			print (jsonText);
			Debug.LogError (ex);
		}
	}
}



