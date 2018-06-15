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
		Action<string,int> callback;
		callback = GetMarkersCallback;
		APIManager.Instance.PostCoven ("portal/place", "{}", callback);
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

	public static void PortalAttack( int energy = 0)
	{
		var data = CalculateSpellData (energy);
		Action<string,int> callback;
		callback = GetCastSpellCallback	;
		APIManager.Instance.PostCoven ("portal/attack", JsonConvert.SerializeObject (data), callback);
		SpellSpiralLoader.Instance.LoadingStart ();
	}

	public static void PortalWard( int energy = 0)
	{
		var data = CalculateSpellData (energy);
		Action<string,int> callback;
		callback = GetCastSpellCallback	;
		APIManager.Instance.PostCoven ("portal/ward", JsonConvert.SerializeObject (data), callback);
		SpellSpiralLoader.Instance.LoadingStart ();
	}

	public static void CastSpell( int energy = 0)
	{
		var data = CalculateSpellData (energy);
		Action<string,int> callback;
		callback = GetCastSpellCallback	;
		APIManager.Instance.PostCoven ("spell/targeted", JsonConvert.SerializeObject (data), callback);
		SpellSpiralLoader.Instance.LoadingStart ();
	}

	static SpellTargetData CalculateSpellData (int energy)
	{
		var data = new SpellTargetData ();
		data.spell = SpellCarousel.currentSpell;
		data.target = MarkerSpawner.instanceID;
		data.channel = energy;
		data.energy = energy;
		data.ingredients = new List<spellIngredientsData> ();
		if (IngredientsManager.Instance.addedHerb != "") {
			var ingData = new spellIngredientsData ();
			print (IngredientsManager.Instance.addedHerb);
			ingData.id = IngredientsManager.Instance.addedHerb;
			ingData.count = IngredientsManager.herbCount;
			data.ingredients.Add (ingData);
		}
		if (IngredientsManager.Instance.addedGem != "") {
			var ingData = new spellIngredientsData ();
			print (IngredientsManager.Instance.addedGem);
			ingData.id = IngredientsManager.Instance.addedGem;
			ingData.count = IngredientsManager.gemCount;
			data.ingredients.Add (ingData);
		}
		if (IngredientsManager.Instance.addedTool != "") {
			var ingData = new spellIngredientsData ();
			print (IngredientsManager.Instance.addedTool);
			ingData.id = IngredientsManager.Instance.addedTool;
			ingData.count = IngredientsManager.toolCount;
			data.ingredients.Add (ingData);
		}
		IngredientsManager.Instance.addedHerb = IngredientsManager.Instance.addedTool = IngredientsManager.Instance.addedGem = "";
		IngredientsManager.herbCount = IngredientsManager.toolCount = IngredientsManager.gemCount = 0;
		return data;
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

