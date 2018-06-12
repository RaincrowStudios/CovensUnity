using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;

public class WebSocketClient : MonoBehaviour
{
	public static WebSocketClient Instance { get; set; }
    public static event Action<string> OnResponseEvt;
    public static event Action<WebSocketResponse> OnResponseParsedEvt;

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

	public void ParseJson (string jsonText)
	{
        if (OnResponseEvt != null)
            OnResponseEvt(jsonText);

        try
        {
			WebSocketResponse response = JsonConvert.DeserializeObject<WebSocketResponse> (jsonText);

            if (OnResponseParsedEvt != null)
                OnResponseParsedEvt(response);


            if (response.command == "character_spell_hit") {
				PlayerDataManager.playerData.energy = response.energy;
				PlayerManagerUI.Instance.UpdateEnergy ();
				MM.AttackFXSelf (response);
				if (OnPlayerSelect.currentView == CurrentView.MapView)
					PlayerNotificationManager.Instance.showNotification (response);
			}

			if (response.command == "character_condition_add") {
				Conditions cd = new Conditions ();
				cd.caster = response.caster;
				cd.instance = response.instance;
				cd.isBuff = false;
				cd.Description = "some other condition";
				cd.displayName = "Hex";
				ConditionsManager.Instance.AddCondition (cd);
			}

			if (response.command == "character_condition_remove") {
				print (jsonText);
//				ConditionsManager.Instance.RemoveCondition(response.instance);
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
					if (response.action == "Attack") {
						MM.AttackFXOther (response);
					}
						
				} else if (response.command == "map_portal_add" || response.command == "map_spirit_summon" || response.command == "map_collectible_add") {
//					print(jsonText);
					if (response.command == "map_spirit_summon") {
						string msg = response.token.displayName + " has been summoned near you by " + response.token.summoner + ".";
						NewsScroll.Instance.ShowText (msg);
					}

					if (response.command == "map_portal_add") {
						string msg = response.token.creator + " has created a " + response.token.subtype + " portal.";
						;
						NewsScroll.Instance.ShowText (msg);
					}
					var updatedData = MarkerManagerAPI.AddEnumValueSingle (response.token);
					MM.AddMarker (updatedData);
				} else if (response.command == "character_spell_hit") {
					PlayerDataManager.playerData.energy = response.energy;
				} else if (response.command == "character_spirit_action") {
					int xpDef = response.xp - PlayerDataManager.playerData.xp;
					string msg = "Your " + response.spirit + " attacked " + response.target + ". You gain " + xpDef.ToString () + " XP.";
					NewsScroll.Instance.ShowText (msg);
				} else if (response.command == "map_collectible_drop") {
					var updatedData = MarkerManagerAPI.AddEnumValueSingle (response.token);
					MM.AddMarkerInventory (updatedData);
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
//								AttackVisualFXManager.Instance.SpiritEscape();
								if (distance < PlayerDataManager.DisplayRadius) {
									MM.UpdateMarkerPositionIso (response.token);
									if (distance > PlayerDataManager.attackRadius) {
										if (response.token.instance == MarkerSpawner.instanceID) {
											//AttackVisualFXManager.Instance.SpiritEscape();
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
						AttackVisualFXManager.Instance.Attack (response);
					} else if (response.command == "character_spell_fail") {
//						print ("^^^CHARACTER SPELL FAIL");
						AttackVisualFXManager.Instance.SpellUnsuccessful ();
					} else if (response.command == "character_spell_hit") {
						if (MarkerSpawner.instanceID == response.instance) {
							print ("hit");
//						AttackVisualFXManager.Instance.AddHitQueue (response);
						}
					} else if (response.command == "map_condition_add") {
						if (MarkerSpawner.instanceID != response.instance) {
							
						}
					} else if (response.command == "map_collectible_drop") {
						var updatedData = MarkerManagerAPI.AddEnumValueSingle (response.token);
						MM.AddMarkerInventoryIso (updatedData);
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
				} else if (response.command == "map_collectible_drop") {
					var updatedData = MarkerManagerAPI.AddEnumValueSingle (response.token);
					MM.AddMarkerInventoryIso (updatedData);
				}
					
			}

		} catch (Exception ex) {
			print (jsonText);
			Debug.LogError (ex);
		}
	}
}



