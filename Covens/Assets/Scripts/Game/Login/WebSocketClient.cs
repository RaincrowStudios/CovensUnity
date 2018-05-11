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
					if (ShowMessages)
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
//
			if (response.command != "map_spirit_move")
//				print (jsonText);
//			
			if(response.command == "character_spell_hit")
			{
				PlayerDataManager.playerData.energy = response.energy;
			}

			if (OnPlayerSelect.currentView == CurrentView.MapView) {
				
				if (response.command == "map_portal_remove" || response.command == "map_spirit_death" || response.command == "map_collectible_remove" || response.command == "map_spirit_expire") {
//					print(jsonText);
					MM.RemoveMarker (response.instance);
				} else if (response.command == "map_spirit_move" || response.command == "map_character_move") {
					
					if (MarkerManager.Markers.ContainsKey (response.token.instance)) {
//						print ("Contains Spirit");
						double distance = OnlineMapsUtils.DistanceBetweenPointsD (PlayerDataManager.playerPos, ReturnVector2 (response.token));
//						print ("Distance to Spirit: " + distance);
						if (distance < PlayerDataManager.DisplayRadius) {
							MM.UpdateMarkerPosition (response.token);	
//							print("Spirit In Range");

							if (distance > PlayerDataManager.attackRadius) {
//									print("SPIRIT ESCAPED!");
							} 

						} else {
//							print("spirit out of range. deleting");
							MM.RemoveMarker (response.token.instance);
						}
					} else {
//						print("Adding Spirit");
						var updatedData = MarkerManagerAPI.AddEnumValueSingle (response.token);
						MM.AddMarker (updatedData);
					}
				} else if (response.command == "map_spirit_action") {
						
				} else if (response.command == "map_portal_add" || response.command == "map_spirit_summon" || response.command == "map_collectible_add") {
//					print(jsonText);
					if(response.command =="map_spirit_summon"){
						string msg = response.token.displayName + " has been summoned near you by " + response.token.summoner + ".";
					NewsScroll.Instance.ShowText(msg);
					}

					if(response.command =="map_portal_add"){
						string msg = response.token.creator + " has created a " + response.token.subtype + " portal.";  ;
						NewsScroll.Instance.ShowText(msg);
					}
					var updatedData = MarkerManagerAPI.AddEnumValueSingle (response.token);
					MM.AddMarker (updatedData);
				} else if (response.command == "character_spell_hit") {
					PlayerDataManager.playerData.energy = response.energy;
				}
				
			} else if (OnPlayerSelect.currentView == CurrentView.IsoView) {
				{
					if (response.command == "map_portal_remove" || response.command == "map_spirit_death" || response.command == "map_collectible_remove" || response.command == "map_spirit_expire") {
						MM.RemoveMarker (response.instance);
					} else if (response.command == "map_spirit_move" || response.command == "map_character_move") {
						
						if (MarkerManager.Markers.ContainsKey (response.token.instance)) {
							double distance = OnlineMapsUtils.DistanceBetweenPointsD (PlayerDataManager.playerPos, ReturnVector2 (response.token));
							if (MarkerSpawner.instanceID != response.token.instance) {
								if (distance < PlayerDataManager.DisplayRadius) {
									MM.UpdateMarkerPositionIso (response.token);
								} else {
									MM.RemoveMarker (response.token.instance);
								}
							} else {
								print("SpiritEscape");
								AttackVisualFXManager.Instance.SpiritEscape();
								if (distance < PlayerDataManager.DisplayRadius) {
									MM.UpdateMarkerPositionIso (response.token);
									if (distance > PlayerDataManager.attackRadius) {
										if (response.token.instance == MarkerSpawner.instanceID) {
											//spirit escaped
										}
									} 
								}
							}
						} else {
							var updatedData = MarkerManagerAPI.AddEnumValueSingle (response.token);
							MM.AddMarkerIso (updatedData);
						}
					} else if (response.command == "map_portal_add" || response.command == "map_spirit_summon" || response.command == "map_collectible_add") {
						var updatedData = MarkerManagerAPI.AddEnumValueSingle (response.token);
						MM.AddMarkerIso (updatedData);
					} else if (response.command == "character_spell_success") {
//						print ("^^^CHARACTER SPELL SUCCESS");
//						print (jsonText);
						AttackVisualFXManager.Instance.Attack (response);
					} else if (response.command == "character_spell_fail") {
//						print ("^^^CHARACTER SPELL FAIL");
						AttackVisualFXManager.Instance.SpellUnsuccessful ();
					} else if (response.command == "character_spell_hit") {
						if (MarkerSpawner.instanceID != response.token.instance) {
//						print ("hit");
						AttackVisualFXManager.Instance.AddHitQueue (response);
						}
					}

				}
			} else {
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
				} else if (response.command == "character_spell_hit") {
					print(jsonText);
					if(response.instance == MarkerSpawner.instanceID){
					PlayerDataManager.playerData.energy = response.energy;
					}
				}
					
			}

		} catch (Exception ex) {
			print (jsonText);
			Debug.LogError (ex);
		}
	}
}



