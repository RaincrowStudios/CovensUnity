using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SpellCastAPI : MonoBehaviour
{
	 
	public static Dictionary<string, SpellData> spells = new Dictionary<string, SpellData>(); 
	public static List<string> validSpells = new List<string>();

	public static void CastSummon( )
	{
		var data = new MapAPI ();
		data.characterName = PlayerDataManager.playerData.displayName; 
		Action<string,int> callback;
		callback = GetMarkersCallback;
		APIManager.Instance.PostCoven ("portal/place", JsonConvert.SerializeObject (data), callback);
	}

	static void GetMarkersCallback (string result, int response)
	{
		if (response == 200) {
			try{
			}catch(Exception e) {
				print (e.ToString());
			}
		}
	}

	public static void CastSpell( )
	{
		var data = new SpellTargetData ();
		data.spell = SpellCarousel.currentSpell;
		data.target = MarkerSpawner.instanceID;
		data.ingredients = new List<string> ();
		Action<string,int> callback;
		callback = GetCastSpellCallback	;
		APIManager.Instance.PostCoven ("spell/targeted", JsonConvert.SerializeObject (data), callback);
	}

	static void GetCastSpellCallback (string result, int response)
	{
		if (response == 200) {
			try{

			}catch(Exception e) {
				print (e.ToString());
			}
		}
	}

}

