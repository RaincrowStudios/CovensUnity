using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

public class GetMarkerDetailAPI : MonoBehaviour
{
	static string charName;
	public static void GetData(string characterName, MarkerSpawner.MarkerType type)
	{
		charName = characterName;
		var data = new MapAPI ();
		if (type == MarkerSpawner.MarkerType.greaterPortal || type == MarkerSpawner.MarkerType.lesserPortal)
			data.type = "portal";
		else if(type == MarkerSpawner.MarkerType.greaterSpirit || type == MarkerSpawner.MarkerType.lesserSpirit)
			data.type = "spirit";
		else
		data.type = type.ToString ();
		data.target = characterName;
		Action<string,int, MarkerSpawner.MarkerType> callback;
		callback = SendResetCodeCallback;
		APIManager.Instance.PostCovenSelect ("map/select", JsonConvert.SerializeObject(data), callback,type);
	}

	static void SendResetCodeCallback (string result, int response, MarkerSpawner.MarkerType type)
	{
		if (response == 200) {
			try{
				var data = JsonConvert.DeserializeObject<MarkerDataDetail> (result);
				MarkerSpawner.SelectedMarker = data;
				try{
					if(data.validSpells!=null){
					SpellCastAPI.validSpells = data.validSpells;
					SpellCastAPI.validSpells.Insert(0,"null");
					SpellCastAPI.validSpells.Insert(1,"null");
					SpellCastAPI.validSpells.Add("null");
					SpellCastAPI.validSpells.Add("null");
					}

					if(data.conditions!=null)
					{
//						ConditionsManager.ConditionsTarget = data.conditions;
					}

				}catch{
					
				}


				if(type == MarkerSpawner.MarkerType.witch ){
					MarkerSpawner.SelectedMarker.displayName = charName;
					EventManager.Instance.CallPlayerDataReceivedEvent();
				}else if(type == MarkerSpawner.MarkerType.gem || type == MarkerSpawner.MarkerType.herb || type == MarkerSpawner.MarkerType.tool){
					EventManager.Instance.CallInventoryDataReceived();
				}else if(type == MarkerSpawner.MarkerType.lesserPortal || type == MarkerSpawner.MarkerType.greaterPortal ){
					SpellCastAPI.validSpells =  new List<string>();
					SpellCastAPI.validSpells.Add("spell_attack");
					SpellCastAPI.validSpells.Add("spell_ward");
					SpellCastAPI.validSpells.Insert(0,"null");
					SpellCastAPI.validSpells.Insert(1,"null");
					SpellCastAPI.validSpells.Add("null");
					SpellCastAPI.validSpells.Add("null");
					EventManager.Instance.CallPortalDataReceivedEvent();

				}else{
					EventManager.Instance.CallNPCDataReceivedEvent();
				}
			}catch(Exception e) {
				Debug.LogError (e);
			}
		}
	}
}

