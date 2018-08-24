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

		var data = new {latitude = OnlineMaps.instance.position.x,longitude =  OnlineMaps.instance.position.y, ingredients = GetIngredientsSummon()}; 

		APIManager.Instance.PostCoven ("spirit/summon", JsonConvert.SerializeObject(data), callback);
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

	static List<spellIngredientsData> GetIngredientsSummon()
	{
		var sd = new List<spellIngredientsData> ();
		var d = new spellIngredientsData ();

		if (SummonUIManager.selectedTool != null) {
			d.id = SummonUIManager.selectedTool;
			d.count = 1;
			sd.Add (d);
		}
		if (IngredientsSpellManager.AddedHerb.Key != null) {
			d.id = IngredientsSpellManager.AddedHerb.Key;
			d.count = IngredientsSpellManager.AddedHerb.Value;
			sd.Add (d);
		}
		if (IngredientsSpellManager.AddedGem.Key != null) {
			d.id = IngredientsSpellManager.AddedGem.Key;
			d.count = IngredientsSpellManager.AddedGem.Value;
			sd.Add (d);
		}
	
		return sd;
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
//		print (JsonConvert.SerializeObject (data));
		SpellSpiralLoader.Instance.LoadingStart ();
	}

	static SpellTargetData CalculateSpellData (int energy)
	{
		var data = new SpellTargetData ();
		data.ingredients = new List<spellIngredientsData> ();
		data.spell = SpellCarouselManager.currentSpellData.id;
		data.target = MarkerSpawner.instanceID;
		if (!SignatureScrollManager.isActiveSig) {
			print ("Signature Not Active");
			if (IngredientsSpellManager.AddedHerb.Key != null) {
				data.ingredients.Add (new spellIngredientsData {
					id = IngredientsSpellManager.AddedHerb.Key,
					count = IngredientsSpellManager.AddedHerb.Value
				});
			}
			if (IngredientsSpellManager.AddedGem.Key != null) {
				data.ingredients.Add (new spellIngredientsData {
					id = IngredientsSpellManager.AddedGem.Key,
					count = IngredientsSpellManager.AddedGem.Value
				});
			}
			if (IngredientsSpellManager.AddedTool.Key != null) {
				data.ingredients.Add (new spellIngredientsData {
					id = IngredientsSpellManager.AddedTool.Key,
					count = IngredientsSpellManager.AddedTool.Value
				});
			}
			IngredientsSpellManager.AddedHerb = new KeyValuePair<string, int>();
			IngredientsSpellManager.AddedGem =   new KeyValuePair<string, int>();
			IngredientsSpellManager.AddedTool =  new KeyValuePair<string, int>();
			IngredientUIManager.curType = IngredientType.none;

		} else {
			print ("Signature Active!!");
			foreach (var item in SignatureScrollManager.currentSignature.ingredients) {
				data.ingredients.Add(new spellIngredientsData {
					id = item.id,
					count = item.count
				});
				if (item.type == "herb") {
					PlayerDataManager.playerData.ingredients.herbsDict [item.id].count -= item.count;
					if (PlayerDataManager.playerData.ingredients.herbsDict [item.id].count < 1)
						PlayerDataManager.playerData.ingredients.herbsDict.Remove (item.id);
				} else if (item.type == "gem") {
					PlayerDataManager.playerData.ingredients.gemsDict [item.id].count -= item.count;
					if (PlayerDataManager.playerData.ingredients.gemsDict [item.id].count < 1)
						PlayerDataManager.playerData.ingredients.gemsDict.Remove (item.id);
				} else {
					PlayerDataManager.playerData.ingredients.toolsDict [item.id].count -= item.count;
					if (PlayerDataManager.playerData.ingredients.toolsDict [item.id].count < 1)
						PlayerDataManager.playerData.ingredients.toolsDict.Remove (item.id);
				}
					
			}
		}
		return data;
	}

	static void GetCastSpellCallback (string result, int response)
	{
		print ("Casting Response : " + response);
		if (response == 200) {
			try{
				
			}catch(Exception e) {
				print (e.ToString());
			}
		}
	}

}

