using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class SpellCastAPI : MonoBehaviour
{
	 
	public static Dictionary<string, SpellData> spells = new Dictionary<string, SpellData>(); 



	public static void PortalCast(int en )
	{
		var data = new {target = MarkerSpawner.instanceID, energy = en}; 
		APIManager.Instance.PostCoven ("portal/cast", JsonConvert.SerializeObject (data), PortalCastCallBack);
	}


	static void PortalCastCallBack (string result, int response)
	{
		print ("Casting Response : " + response);
		if (response == 200) {
			try {
				SpellSpiralLoader.Instance.LoadingStart (true);

			} catch (Exception e) {
				print (e.ToString ());
			}
		} else {
			SpellSpiralLoader.Instance.LoadingStart (false);
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
//			data.spell = SpellCarouselManager.currentSpellData.id;
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

//		IngredientUIManager.curType = IngredientType.none;
//		IngredientUIManager.Instance.turnOffAddIcons ();
		return data;
	}


	static void GetCastSpellCallback (string result, int response)
	{
		print ("Casting Response : " + result);
//		SpellCastUIManager.Instance.SpellClose ();
		if (response == 200) {
			try {
				
			} catch (Exception e) {
				print (e.ToString ());
			}
		} else {
			if (result == "4301") {
				HitFXManager.Instance.TargetDead (true);
			} else if (result == "4700") {
				PlayerDataManager.playerData.state = "dead";
				PlayerDataManager.playerData.energy = 0;
//				SpellCastUIManager.Instance.Exit ();
			} if (result == "4704") {
				HitFXManager.Instance.Escape ();
			}
		}
	}

}

