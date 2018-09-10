using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SpellCastAPI : MonoBehaviour
{
	 
	public static Dictionary<string, SpellData> spells = new Dictionary<string, SpellData>(); 

	public static void CastSummon(  )
	{
		Action<string,int> callback;
		callback = GetMarkersCallback;

		var data = new {latitude = SummonMapSelection.newMapPos.y,longitude =  SummonMapSelection.newMapPos.x, ingredients = GetIngredientsSummon()}; 
		ResetIngredients ();
		APIManager.Instance.PostCoven ("spirit/summon", JsonConvert.SerializeObject(data), callback);
	}

	public static void CastSummoningLocation( )
	{
		Action<string,int> callback;
		callback = CastSummoningLocationCallback;
		var data = new {ingredients = CalculateSpellData(0,false).ingredients}; 
		ResetIngredients ();
		APIManager.Instance.PostCoven ("spirit/summon", JsonConvert.SerializeObject(data), callback);
	}

	static void CastSummoningLocationCallback (string result, int response)
	{
		print ("Casting Response : " + response);
		print ("Casting Response : " + result);
		if (response == 200) {
			try{
				LocationUIManager.Instance.SummonClose();
			}catch(Exception e) {
				print (e.ToString());
			}
		}
	}

	static void GetMarkersCallback (string result, int response)
	{
		print (result + "," + response);
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
		print (SummonUIManager.selectedTool);
		print (IngredientsSpellManager.AddedHerb.Key);
		print (IngredientsSpellManager.AddedGem.Key);
		if (SummonUIManager.selectedTool != null) {
			var d = new spellIngredientsData ();
			d.id = SummonUIManager.selectedTool;
			d.count = 1;
			sd.Add (d);
		}
		if (IngredientsSpellManager.AddedHerb.Key != null) {
			var d = new spellIngredientsData ();
			d.id = IngredientsSpellManager.AddedHerb.Key;
			d.count = IngredientsSpellManager.AddedHerb.Value;
			sd.Add (d);
		}
		if (IngredientsSpellManager.AddedGem.Key != null) {
			var d = new spellIngredientsData ();
			d.id = IngredientsSpellManager.AddedGem.Key;
			d.count = IngredientsSpellManager.AddedGem.Value;
			sd.Add (d);
		}
		
		return sd;
	}

	public static void PortalCast(int en )
	{
//		var data = CalculateSpellData (energy);
		var data = new {target = MarkerSpawner.instanceID, energy = en}; 
		APIManager.Instance.PostCoven ("portal/cast", JsonConvert.SerializeObject (data), PortalCastCallBack);
//		SpellSpiralLoader.Instance.LoadingStart ();
	}

	public static void CastSpell( int energy = 0)
	{
		var data = CalculateSpellData (energy);
		Action<string,int> callback;
		callback = GetCastSpellCallback	;
		APIManager.Instance.PostCoven ("spell/targeted", JsonConvert.SerializeObject (data), callback);
		SpellSpiralLoader.Instance.LoadingStart ();
	}

	static void PortalCastCallBack (string result, int response)
	{
		print ("Casting Response : " + response);
		if (response == 200) {
			try{
				
			}catch(Exception e) {
				print (e.ToString());
			}
		}
	}

	static void ResetIngredients ()
	{
		IngredientsSpellManager.AddedHerb = new KeyValuePair<string, int> ();
		IngredientsSpellManager.AddedGem = new KeyValuePair<string, int> ();
		IngredientsSpellManager.AddedTool = new KeyValuePair<string, int> ();
	
	}

	static SpellTargetData CalculateSpellData (int energy, bool isSpell = true)
	{
		var data = new SpellTargetData ();
		data.ingredients = new List<spellIngredientsData> ();
		if (isSpell){
			data.spell = SpellCarouselManager.currentSpellData.id;
			data.target = MarkerSpawner.instanceID;
		}
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
			ResetIngredients ();

		IngredientUIManager.curType = IngredientType.none;
		IngredientUIManager.Instance.turnOffAddIcons ();
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

