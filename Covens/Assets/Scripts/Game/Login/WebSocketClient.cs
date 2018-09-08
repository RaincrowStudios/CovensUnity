using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
public class WebSocketClient : MonoBehaviour
{
	public static WebSocketClient Instance { get; set; }
    public static event Action<string> OnResponseEvt;
    public static event Action<WebSocketResponse> OnResponseParsedEvt;
	string curMessage;
	public GameObject shoutBox; 
	public int totalMessages = 0;
//	public List<string> ReceivedMessagesPriority = new List<string>();
//	public List<string> ReceivedMessagesPriority = new List<string>();

    // Use this for initialization
    MovementManager MM;
	WebSocket curSocket;
	bool canRun = true;
	Thread WebSocketProcessing;

	public bool ShowMessages = false;

	void Awake ()
	{
		Application.targetFrameRate = 80;
		Instance = this;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
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
		curSocket = new WebSocket (new Uri (Constants.wssAddress + LoginAPIManager.wssToken));

		yield return StartCoroutine (curSocket.Connect ());
		print (curSocket.RecvString ());
//		if (curSocket.RecvString () == "200") {
//			HandleThread ();
//		}
		while (true) {
			string reply = curSocket.RecvString ();
			if (reply != null) {
				if (reply != "200") {
					if (ShowMessages)
						print (reply);
					curMessage = reply;
                    ParseJson (reply);
				}
			}
			if (curSocket.error != null) {
				Debug.LogError ("Error: " + curSocket.error);
				break;
			}
			yield return 0;
		}
		curSocket.Close ();
	}

//	public void HandleThread( )
//	{
//		AbortThread ();
//		WebSocketProcessing= new Thread (()=> ReadCommands(curSocket));
//		WebSocketProcessing.Start ();
//	}
//
//	void ReadCommands(WebSocket w)
//	{
//		while (canRun) {
//			string reply = w.RecvString ();
//			if (reply != null) {
//				totalMessages++;
//			}
//		}
//	}
//
//	void AbortThread ()
//	{
//		if (WebSocketProcessing != null) {
//			canRun = false;
//			print ("AbortingThread");
//			WebSocketProcessing.Abort ();
//		}
//	}

//	void OnApplicationQuit()
//	{
//		AbortThread ();
//	}
//
	public void ParseJson(string json)
	{
		var data = JsonConvert.DeserializeObject<WSData> (json);
		try{
		ManageData (data);
		}catch(Exception e) {
			Debug.LogError (e);
			Debug.LogError (curMessage);
		}
	}

	 IEnumerator BootCharacterLocation (WSData data, float delay = 0)
	{
		yield return new WaitForSeconds (delay);
		var lm = LocationUIManager.Instance;
		if (LocationUIManager.isLocation) {
			if (lm.isSummon) {
				lm.SummonClose ();
			}
			PlayerManager.marker.position = new Vector2 ((float)data.longitude, (float)data.latitude);
			lm.ini = PlayerManager.marker.position;
			lm.Escape ();
		}
		yield return null;
	}

	void ManageData ( WSData data)
	{
		var pData = PlayerDataManager.playerData; //markerdetaildata object 
		//CHARACTER COMMANDS
		if (data.command == character_cooldown_add) {
			var cooldown = new CoolDown ();
			cooldown.instance = data.instance;
			cooldown.expiresOn = data.expiresOn;
			cooldown.spell = data.spell;
			pData.cooldownDict [cooldown.instance] = cooldown;
			SpellCarouselManager.Instance.WSCooldownChange (pData.cooldownDict [data.instance], true);
		} else if (data.command == character_cooldown_remove) {
			if (pData.cooldownDict.ContainsKey (data.instance)) {
				SpellCarouselManager.Instance.WSCooldownChange (pData.cooldownDict [data.instance], false);
				pData.cooldownDict.Remove (data.instance);
			}
		} else if (data.command == character_new_signature) {
			PlayerDataManager.playerData.signatures.Add (data.signature);
			if (MapSelection.currentView == CurrentView.IsoView) {
				SpellCastUIManager.Instance.SetupSignature ();
//				print ("New Signature Discovered");
			}
		} else if (data.command == character_new_spirit) {
			//add data.spirit, data.banishedOn, data.location to character's knownSpirits list
		} else if (data.command == character_spirit_banished) {
			PlayerDataManager.playerData.signatures.Add (data.signature);
			if (MapSelection.currentView == CurrentView.IsoView) {
				SpellCastUIManager.Instance.SetupSignature ();
//				print ("New Signature Discovered");
			}
		} else if (data.command == character_coven_invited) {
			//inform player than they have been invited to data.covenName by data.displayName
			//send inviteToken with /coven/join POST request
		} else if (data.command == character_coven_kicked) {
			//add data.spirit, data.banishedOn, data.location to character's knownSpirits list
		} else if (data.command == character_coven_rejected) {
			//infrom player that their invite request to data.covenName has been rejected
		} else if (data.command == character_new_spirit) {
			//add data.spirit, data.banishedOn, data.location to character's knownSpirits list
		} else if (data.command == character_location_gained) {
			LocationUIManager.Instance.CharacterLocationGained (data.location);
			//inform character data.locationName has been gained by data.displayName
			//remove data.instance from controlled PoP if viewing
		} else if (data.command == character_location_lost) {
			LocationUIManager.Instance.CharacterLocationGained (data.location);
			//inform character the data.locationName has been lost
			//remove data.instance from controlled PoP if viewing
		}
		else if (data.command == map_location_lost) {
			LocationUIManager.Instance.LocationLost (data);
			if (ShowSelectionCard.isLocationCard && data.location == MarkerSpawner.instanceID) {
				var mData = MarkerSpawner.SelectedMarker;
				mData.controlledBy = data.controlledBy;
				mData.spiritCount = data.spiritCount;
				mData.isCoven = data.isCoven;
				ShowSelectionCard.Instance.SetupLocationCard ();
			}
		}
		else if (data.command == map_location_gained) {
			LocationUIManager.Instance.LocationGained (data);
			if (ShowSelectionCard.isLocationCard && data.location == MarkerSpawner.instanceID) {
				print ("In Location");
				var mData = MarkerSpawner.SelectedMarker;
				mData.controlledBy = data.controlledBy;
				mData.spiritCount = data.spiritCount;
				mData.isCoven = data.isCoven;
				ShowSelectionCard.Instance.SetupLocationCard ();
			}
		}
		else if (data.command == character_location_reward) {
			//inform character that data.locationName has rewarded them data.reward of gold
		} else if (data.command == character_spirit_banished) {
			//remove from active spirits if viewing
		} else if (data.command == character_spirit_expired) {
			//inform character that spirit (data.instance, data.spirit) has expired
			//remove from activie spirits if in view
		} else if (data.command == character_spirit_sentinel) {
			//inform character that spirit (data.instance, data.spirit) sees a new enemy
		} else if (data.command == character_spirit_summoned) {
			//inform character that spirit (data.instance, data.spirit) from portal (data.portalInstance) has been summoned
			//inform character of data.xpGain
			//remove portal from active portals if in view
			//add spirit to active spirts if in view
		} else if (data.command == character_location_boot) {
			print ("Booting");
			if (MapSelection.currentView == CurrentView.IsoView) {
				SpellCastUIManager.Instance.Exit ();
				StartCoroutine( BootCharacterLocation (data,1.8f)); 
				return;
			}
			StartCoroutine( BootCharacterLocation (data)); 
		}
		//MAP COMMANDS
		else if (data.command == map_condition_add) {
			if (data.instance == pData.instance) {
				Conditions cd = new Conditions ();
				cd.bearerInstance = data.instance;
				cd.id = data.condition;
				cd.instance = data.conditionInstance;
				cd.spellID = DownloadedAssets.conditionsDictData [cd.id].spellID;
				cd.status = data.status;
				ConditionsManager.Instance.WSAddCondition (cd);
				if (data.status == "silenced") {
					BanishManager.silenceTimeStamp = data.expiresOn;
					BanishManager.Instance.Silenced (data);
				}
				if (data.status == "bound") {
					BanishManager.bindTimeStamp = data.expiresOn;
					BanishManager.Instance.Bind (data); 
					if (LocationUIManager.isLocation) {
						LocationUIManager.Instance.Bind (true);
					}
				}
				if (MapSelection.currentView == CurrentView.IsoView) {
					ConditionsManagerIso.Instance.WSAddCondition (cd, true);
				}
				
			} else if (data.instance == MarkerSpawner.instanceID) {
				Conditions cd = new Conditions ();
				cd.bearerInstance = data.instance;
				cd.id = data.condition;
				cd.instance = data.conditionInstance;
				cd.spellID = DownloadedAssets.conditionsDictData [cd.id].spellID;
				if (MapSelection.currentView == CurrentView.IsoView) {
					ConditionsManagerIso.Instance.WSAddCondition (cd, false);
				}
			
			}
		} else if (data.command == map_condition_remove) {
			
			if (data.instance == pData.instance) {
		
				ConditionsManager.Instance.WSRemoveCondition (data.conditionInstance);
				if (MapSelection.currentView == CurrentView.IsoView) {
					ConditionsManagerIso.Instance.WSRemoveCondition (data.conditionInstance, true);
				}
				bool isSilenced = false;
				bool isBound = false;
				foreach (var item in PlayerDataManager.playerData.conditionsDict) {
					if (item.Value.status == "silenced") {
						BanishManager.bindTimeStamp = item.Value.expiresOn;
						isSilenced = true;
						break;
					} else
						isSilenced = false;
					
					if (item.Value.status == "bound") {
						BanishManager.bindTimeStamp = item.Value.expiresOn;
						isBound = true;
						break;
					} else
						isBound = false;
				}
				if (data.status == "silenced") {
					if (!isSilenced) {
						BanishManager.Instance.unSilenced ();
					}
				}
				if (!isBound && data.status == "bound") {
					BanishManager.Instance.Unbind ();
					if (LocationUIManager.isLocation) {
						LocationUIManager.Instance.Bind (false);
					}
				}
			} else if (data.instance == MarkerSpawner.instanceID) {
				if (MapSelection.currentView == CurrentView.IsoView) {
					ConditionsManagerIso.Instance.WSRemoveCondition (data.conditionInstance, false);
				}
			}
		} else if (data.command == map_condition_trigger) {
			if (data.instance == pData.instance && pData.conditionsDict.ContainsKey (data.conditionInstance)) {
				Conditions cd = new Conditions ();
				cd.bearerInstance = data.instance;
				cd.id = data.condition;
				cd.instance = data.conditionInstance;
				cd.spellID = pData.conditionsDict [cd.instance].spellID;
				ConditionsManager.Instance.ConditionTrigger (cd);
				if (MapSelection.currentView == CurrentView.IsoView) {
					ConditionsManagerIso.Instance.ConditionTrigger (cd, true);
				}
			} else if (data.instance == MarkerSpawner.instanceID) {
				if (MarkerSpawner.SelectedMarker.conditionsDict.ContainsKey (data.conditionInstance)) {
					Conditions cd = new Conditions ();
					cd.bearerInstance = data.instance;
					cd.id = data.condition;
					cd.instance = data.conditionInstance;
					cd.spellID = MarkerSpawner.SelectedMarker.conditionsDict [data.conditionInstance].spellID; 
					if (MapSelection.currentView == CurrentView.IsoView) {
						ConditionsManagerIso.Instance.ConditionTrigger (cd, false);
					}
				}
			}
		} else if (data.command == map_energy_change) {
			string logMessage = "<color=yellow> Map_Energy_Change</color>";
			logMessage += "\n <b> New Energy : " + data.newEnergy; 
			logMessage += " | New State : " + data.newState + "</b>"; 
//			Debug.Log (logMessage);

			if (data.instance == pData.instance) {
				if (LocationUIManager.isLocation) {
					pData.state = data.newState;
					pData.energy = data.newEnergy;
					return;
				}
				pData.energy = data.newEnergy;
				if (pData.state != "dead" && data.newState == "dead") {
					if (MapSelection.currentView == CurrentView.IsoView) {
						SpellCastUIManager.Instance.Exit ();
//						print ("dead");
					} else if (MapSelection.currentView == CurrentView.MapView) {
						DeathState.Instance.ShowDeath ();
					}
				} 
				if (pData.state == "dead" && data.newState != "dead") {
					DeathState.Instance.HideDeath ();
//					print ("undead");
				}
				pData.state = data.newState;
				SpellCarouselManager.Instance.WSStateChange ();
				PlayerManagerUI.Instance.UpdateEnergy ();
				if (MapSelection.currentView == CurrentView.IsoView) {
					ShowSelectionCard.Instance.ChangeSelfEnergy ();
				}
			}
			if (MarkerSpawner.instanceID == data.instance) {
				if (MapSelection.currentView == CurrentView.IsoView) {
					if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.portal) {
						if (data.newState != "dead") {
							print ("Energy Change Portal to " + data.newEnergy);
							IsoPortalUI.instance.PortalFX (data.newEnergy);
							MarkerSpawner.SelectedMarker.energy = data.newEnergy;
							IsoTokenSetup.Instance.ChangeEnergy ();
						} else {
							IsoPortalUI.instance.Destroyed ();
						}
						return;
					}
					if (MarkerSpawner.SelectedMarker.state != "dead" && data.newState == "dead") {
						HitFXManager.Instance.TargetDead ();
					} else if (MarkerSpawner.SelectedMarker.state == "dead" && data.newState != "dead") {
						HitFXManager.Instance.TargetRevive ();
					}
					MarkerSpawner.SelectedMarker.state = data.newState;
					MarkerSpawner.SelectedMarker.energy = data.newEnergy;
					SpellCarouselManager.Instance.WSStateChange ();
					IsoTokenSetup.Instance.ChangeEnergy ();
				}
			}
		} else if (data.command == map_immunity_add) {
			if (data.immunity == pData.instance) {
				string logMessage = "<color=#008bff> Map_immunity_add</color>";
				if (data.instance == MarkerSpawner.instanceID && data.immunity == pData.instance) {
					logMessage += "\n <b>" + MarkerSpawner.SelectedMarker.displayName + "<color=#008bff> is Immune to </color>" + pData.displayName + "</b>"; 
				}
//				Debug.Log (logMessage);
			
				if (MarkerSpawner.ImmunityMap.ContainsKey (data.instance))
					MarkerSpawner.ImmunityMap [data.instance].Add (data.immunity);
				else
					MarkerSpawner.ImmunityMap [data.instance] = new HashSet<string> (){ data.immunity };

				if (MapSelection.currentView == CurrentView.IsoView) {
					if (data.instance == MarkerSpawner.instanceID) {
						HitFXManager.Instance.SetImmune (true);
					}
				}
				if (MapSelection.currentView == CurrentView.MapView && MarkerSpawner.instanceID == data.instance) {
					ShowSelectionCard.Instance.SetCardImmunity (true);
				}
				MarkerManager.SetImmunity (true, data.instance);
			}
		} else if (data.command == map_immunity_remove) {
			if (data.immunity == pData.instance) {
				string logMessage = "<color=#008bff> Map_immunity_remove</color>";
				if (data.instance == MarkerSpawner.instanceID && data.immunity == pData.instance) {
					logMessage += "\n <b>" + MarkerSpawner.SelectedMarker.displayName + " <color=#008bff> is no longer Immune to </color> " + pData.displayName + "</b>"; 
				}
//				Debug.Log (logMessage);
				if (MarkerSpawner.ImmunityMap.ContainsKey (data.instance)) {
					if (MarkerSpawner.ImmunityMap [data.instance].Contains (data.immunity))
						MarkerSpawner.ImmunityMap [data.instance].Remove (data.immunity);
				}

				if (MapSelection.currentView == CurrentView.IsoView) { 
					if (data.instance == MarkerSpawner.instanceID) { 
						HitFXManager.Instance.SetImmune (false);  
					} 
				}
				if (MapSelection.currentView == CurrentView.MapView && MarkerSpawner.instanceID == data.instance) {
					ShowSelectionCard.Instance.SetCardImmunity (false);
				}
				MarkerManager.SetImmunity (false, data.instance);
			}
		} else if (data.command == map_level_up) {
			//change data.instance level to data.newLevel
			//for leveled up character, set baseEnergy to data.newBaseEnergy
		} else if (data.command == map_shout) {
			if (data.instance == pData.instance) {
				var g = Utilities.InstantiateObject (shoutBox, PlayerManager.marker.instance.transform);
				g.GetComponent<ShoutBoxData> ().Setup (data.displayName, data.shout);
			} else {
				if (MarkerManager.Markers.ContainsKey (data.instance)) {
					if (OnlineMaps.instance.zoom > 14) {
						if (MarkerManager.Markers [data.instance] [0].inMapView) {
							var g = Utilities.InstantiateObject (shoutBox, MarkerManager.Markers [data.instance] [0].instance.transform);
							g.GetComponent<ShoutBoxData> ().Setup (data.displayName, data.shout);
						}
					}
				}
			}
		} else if (data.command == map_spell_cast) {
			string logMessage = "<color=#00FF0C> Map_Spell_Cast</color> by " + data.caster + "<color=#00FF0C> => </color>" + data.target;
			logMessage += "\n <b> Result : " + data.result.effect; 
			logMessage += " | Damage : " + data.result.total; 
			logMessage += " | Spell : " + data.spell + "</b>"; 
//			Debug.Log (logMessage);

			if (data.casterInstance == pData.instance) {
				if (data.target == "portal")
					return;
				SpellSpiralLoader.Instance.LoadingDone ();

				if (data.spell == "spell_banish" && data.result.effect == "success") {
					HitFXManager.Instance.Banish ();
					return;
				}
				if (data.targetInstance == MarkerSpawner.instanceID && MapSelection.currentView == CurrentView.IsoView) {
					HitFXManager.Instance.Attack (data);
				}
				if (MapSelection.currentView == CurrentView.IsoView) {
					if (data.result.effect == "fail" || data.result.effect == "fizzle") {
						HitFXManager.Instance.Attack (data);
					}
					if (data.result.effect == "backfire") {
						HitFXManager.Instance.Attack (data);
					}
				}

			
			} 
			if (LocationUIManager.isLocation) {
				MovementManager.Instance.AttackFXOther (data);
				return;
			}
			if (data.targetInstance == pData.instance && MapSelection.currentView == CurrentView.MapView) {
				MovementManager.Instance.AttackFXSelf (data);
			}
			if (data.targetInstance != pData.instance && MapSelection.currentView == CurrentView.MapView) {
				MovementManager.Instance.AttackFXOther (data);
			}



	
			if (data.targetInstance == pData.instance) {

				if (data.spell == "spell_banish") {
					BanishManager.banishCasterID = data.caster;
				}
				
				if (data.result.total < 0) {
					MarkerManager.StanceDict [data.casterInstance] = true;
				} else if (data.result.total > 0) {
					MarkerManager.StanceDict [data.casterInstance] = false;
				}
				if (MarkerManager.Markers.ContainsKey (data.casterInstance)) {
					var tokenD = MarkerManager.Markers [data.casterInstance] [0].customData as Token;
					MarkerSpawner.Instance.SetupStance (MarkerManager.Markers [data.casterInstance] [0].instance.transform, tokenD);
				}

				if (MapSelection.currentView == CurrentView.MapView) {
					string msg = "";
					if (data.spell != "attack") {
						if (data.result.total > 0) { 
							msg = data.caster + " cast " + DownloadedAssets.spellDictData [data.spell].spellName + " on you. You gain " + data.result.total.ToString () + " Energy.";
						} else if (data.result.total < 0) {
							msg = data.caster + " cast " + DownloadedAssets.spellDictData [data.spell].spellName + " on you. You lose " + data.result.total.ToString () + " Energy.";
						} else {
							msg = data.caster + " cast " + DownloadedAssets.spellDictData [data.spell].spellName + " on you.";
						}
					} else {
						msg = data.caster + " attacked you, you lose " + data.result.total.ToString () + " Energy.";
					}
					if (MarkerManager.Markers.ContainsKey (data.casterInstance)) {
						var cData = MarkerManager.Markers [data.casterInstance] [0].customData as Token; 
						var Sprite = PlayerNotificationManager.Instance.ReturnSprite (cData.degree, cData.male);
						PlayerNotificationManager.Instance.showNotification (msg, Sprite);
					}
				}
			}

			if (data.casterInstance == MarkerSpawner.instanceID && data.targetInstance == pData.instance && MapSelection.currentView == CurrentView.IsoView) {
				if (data.result.effect == "backfire") {
					HitFXManager.Instance.BackfireEnemy (data);
				} else {
					HitFXManager.Instance.Hit (data);
				}
			}
		} else if (data.command == character_spell_move) {
			if (data.spell == "spell_banish") {
				if (!LocationUIManager.isLocation) {
					BanishManager.Instance.Banish (data.longitude, data.latitude);
				}
				StartCoroutine (BanishWaitTillLocationLeave (data));
			} // handle magic dance;
		} else if (data.command == map_portal_summon) {
			if (MarkerSpawner.instanceID == data.instance && MapSelection.currentView == CurrentView.IsoView) {
				IsoPortalUI.instance.Summoned ();
				MM.RemoveMarkerIso (data.instance);
			} else {
				if (MapSelection.currentView == CurrentView.IsoView || MapSelection.currentView == CurrentView.TransitionView)
					MM.RemoveMarkerIso (data.instance);
				else
					MM.RemoveMarker (data.instance);
			}

		} else if (data.command == map_token_add) {
			if (data.token.position == 0) {
				var updatedData = MarkerManagerAPI.AddEnumValueSingle (data.token);
				if (MapSelection.currentView == CurrentView.MapView)
					MM.AddMarker (updatedData);
				else
					MM.AddMarkerIso (updatedData);
			}else {
				LocationUIManager.Instance.AddToken (data.token);
			}

//			print (data.token);
		} else if (data.command == map_token_move) {
			
//			string logMessage = "Moving Player <color=#00FF0C>" + data.token.displayName + "</color>";
			if (data.token.position == 0) {
				if (MarkerManager.Markers.ContainsKey (data.token.instance)) {
					double distance = OnlineMapsUtils.DistanceBetweenPointsD (PlayerDataManager.playerPos, ReturnVector2 (data.token));
//				logMessage += " \n <b>Current Pos </b>" + MarkerManager.Markers[data.token.instance][0].position.x + "," + MarkerManager.Markers[data.token.instance][0].position.y + " |<b> New position </b>" + data.token.longitude + "," + data.token.latitude;
					if (distance < PlayerDataManager.DisplayRadius) {
						MM.UpdateMarkerPosition (data.token);	
						if (distance > (PlayerDataManager.attackRadius * 3.3f)) {
							if (MapSelection.currentView == CurrentView.IsoView) {
								if (data.token.instance == MarkerSpawner.instanceID) {
									HitFXManager.Instance.Escape ();
								}
							}
						} 
					} else {
						MM.RemoveMarker (data.token.instance);
					}
				} else {
					var updatedData = MarkerManagerAPI.AddEnumValueSingle (data.token);
					MM.AddMarker (updatedData);
				}
//			Debug.Log (logMessage);
			} 
		} else if (data.command == map_token_remove) {
			if (!LocationUIManager.isLocation) {
				if (MapSelection.currentView == CurrentView.MapView)
					MM.RemoveMarker (data.instance);
				else
					MM.RemoveMarkerIso (data.instance);
			}else {
				print ("Removing Token!");
				LocationUIManager.Instance.RemoveToken (data.instance);
			}
		} 
	}

	Vector2 ReturnVector2 (Token data)
	{
		return new Vector2 (data.longitude, data.latitude);
	}

	IEnumerator BanishWaitTillLocationLeave(WSData data)
	{
		yield return new WaitUntil (() => LocationUIManager.isLocation == false);
		BanishManager.Instance.Banish (data.longitude, data.latitude);

	}

	#region wsCommands
	 //CHARACTER
	 string character_cooldown_add= "character_cooldown_add";
	 string character_cooldown_remove= "character_cooldown_remove";
	 
	 string character_coven_invited= "character_coven_invited";
	 string character_coven_kicked = "character_coven_kicked";
	 string character_coven_rejected= "character_coven_rejected";

	 string character_location_gained= "character_location_gained";
	 string character_location_lost= "character_location_lost";
	string character_location_boot= "character_location_boot";
	 string character_location_reward= "character_location_reward";

	 string character_new_signature= "character_new_signature";
	 string character_new_spirit= "character_new_spirit";

	string character_portal_destroyed= "character_portal_destroyed";
	string character_spell_move= "character_spell_move";

	 string character_spirit_banished= "characer_spirit_banished";

	 string character_spirit_expired= "character_spirit_expired";
	 string character_spirit_sentinel= "character_spirit_sentinel";
	string character_spirit_summoned= "character_spirit_summoned";
//	 string character_spirit_summoned= "character_spirit_summoned";

	 //MAP
	string map_location_lost = "map_location_lost";
	string map_location_gained = "map_location_gained";
	 string map_condition_add= "map_condition_add";			
	 string map_condition_remove= "map_condition_remove";
	 string map_condition_trigger= "map_condition_trigger";

	string map_portal_summon= "map_portal_summon";
	 string map_degree_change= "map_degree_change";

	string map_shout= "map_shout";

 	 string map_energy_change= "map_energy_change";
	 
	 string map_equipment_change= "map_equipment_change";

	 string map_immunity_add= "map_immunity_add";
	 string map_immunity_remove= "map_immunity_remove";

	 string map_level_up= "map_level_up";

	 string map_location_offer= "map_location_offer";

	 string map_spell_cast= "map_spell_cast";

	 string map_token_add= "map_token_add";
	 string map_token_move= "map_token_move";
	 string map_token_remove= "map_token_remove";

	 //COVEN
	 string coven_invite_requested= "coven_invite_requested";
	 string coven_member_invited= "coven_member_invited";
	 string coven_member_joined= "coven_member_joined";
	 string coven_member_left= "coven_member_left";
	 string coven_allied= "coven_allied";
	 string coven_unallied= "coven_unallied";
	 string coven_member_promoted= "coven_member_promoted";
	 string coven_member_titled= "coven_member_titled";
	 string coven_was_allied= "coven_was_allied";
	 string coven_was_unallied= "coven_was_unallied";
	 string coven_disbanded= "coven_disbanded";
	#endregion

}



public class WSData{
	public string command { get; set;}
	public string instance { get; set;}
	// map commands
	public string caster { get; set;}
	public bool isCoven{ get; set;}
	public int spiritCount{ get; set;}
	public string status { get; set;}
	public string casterInstance { get; set;}
	public string controlledBy { get; set;}
	public string target { get; set;}
	public string targetInstance { get; set;}
	public string spell { get; set;}
	public string baseSpell { get; set;}
	public string baseEffect { get; set;}
	public Result result { get; set;}
	public int newEnergy { get; set;}
	public string newState { get; set;}
	public string shout { get; set;}
	public string conditionInstance { get; set;}
	public string condition { get; set;}
	public int newLevel { get; set;}
	public int newBaseEnergy { get; set;}
	public int newDegree { get; set;}
	public Token token { get;set;}
	public string immunity { get; set; }
	//char commands
	public string covenName { get; set;}
	public string locationName { get; set;}
	public string displayName { get; set;}
	public int reward { get; set;}
	public string portalInstance { get; set;}
	public int xpGain { get;set;}
	public string location { get;set;}
	public double banishedOn { get;set;}
	public double createdOn { get;set;}
	public double latitude { get;set;}
	public double longitude { get;set;}
	public double expiresOn { get;set;}
	public string spirit { get; set;}
	public string killer { get; set;}
	public string type { get; set;}
	public string owner { get; set;}
	public string inviteToken { get; set;}
	public Signature signature { get; set;}
}