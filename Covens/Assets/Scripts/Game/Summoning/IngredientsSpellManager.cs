using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IngredientsSpellManager : MonoBehaviour
{
	public static IngredientsSpellManager Instance { get; set;}

	public static KeyValuePair<string,int> AddedHerb = new KeyValuePair<string, int>();
	public static KeyValuePair<string,int> AddedTool = new KeyValuePair<string, int>();
	public static KeyValuePair<string,int> AddedGem  = new KeyValuePair<string, int>();

	static int maxItem = 5;

	public static void ClearCachedItems(IngredientType type)
	{
		//if (type == IngredientType.gem) {
		//	if (AddedGem.Key != null) {
		//		PlayerDataManager.playerData.ingredients.gemsDict [AddedGem.Key].count += AddedGem.Value; 
		//		AddedGem = new KeyValuePair<string, int> ();
		//		return;
		//	}
		//} else if (type == IngredientType.tool) {
		//	if (AddedTool.Key != null) { 
		//		PlayerDataManager.playerData.ingredients.toolsDict [AddedTool.Key].count += AddedTool.Value;  
		//		AddedTool = new KeyValuePair<string, int> ();
		//		return;
		//	}
		//}else{
		//	if (AddedHerb.Key != null) {
		//		PlayerDataManager.playerData.ingredients.herbsDict [AddedHerb.Key].count += AddedHerb.Value; 
		//		AddedHerb = new KeyValuePair<string, int> ();
		//		return;
		//	}
		//}
	}

	public static void CastSpell()
	{
		AddedGem = AddedTool = AddedHerb = new KeyValuePair<string, int> ();
	}

	public static KeyValuePair<string,int> GetCurrentItem(IngredientType type)
	{
		if (type == IngredientType.gem)
			return AddedGem;
		else if (type == IngredientType.herb)
			return AddedHerb;
		else
			return AddedTool;
	}

	/* 
	 0 = can add item
	 1 = insufficientitem
	 2 = max capacity reached
	 3 = different item adding
	 */

	public static int AddItem( string id,IngredientType type, int count =1){
		//if (type == IngredientType.gem) {
		//	if (AddedGem.Key != null) {
		//		if (AddedGem.Key != id) {
		//			return 3;
		//		}
		//		if (AddedGem.Value < maxItem) {
		//			if (CheckAvailability (type, id)) {
		//				PlayerDataManager.playerData.ingredients.gemsDict [id].count--;
		//				AddedGem = new KeyValuePair<string, int>(AddedGem.Key,AddedGem.Value+count);
		//				return 0;
		//			} else
		//				return 1;
		//		} else
		//			return 2;
		//	} else {
		//		if (CheckAvailability (type, id)) {
		//			PlayerDataManager.playerData.ingredients.gemsDict [id].count--;
		//			AddedGem = new KeyValuePair<string, int>(id,count);
		//			return 0;
		//		}
		//	}
		//} 
		//else if (type == IngredientType.tool) {
		//	if (AddedTool.Key != null) {
		//		if (AddedTool.Key != id) {
		//			return 3;
		//		}
		//		if (AddedTool.Value < maxItem) {
		//			if (CheckAvailability (type, id)) {
		//				PlayerDataManager.playerData.ingredients.toolsDict [id].count--;
		//				AddedTool = new KeyValuePair<string, int>(AddedTool.Key,AddedTool.Value+count);
		//				return 0;
		//			} else
		//				return 1;
		//		} else
		//			return 2;
		//	} else {
		//		if (CheckAvailability (type, id)) {
		//			PlayerDataManager.playerData.ingredients.toolsDict [id].count--;
		//			AddedTool = new KeyValuePair<string, int>(id,count);
		//			return 0;
		//		}
		//	}
		//} 
		//else {
		//	if (AddedHerb.Key != null) {
		//		if (AddedHerb.Key != id) {
		//			return 3;
		//		}
		//		if (AddedHerb.Value < maxItem) {
		//			if (CheckAvailability (type, id)) {
		//				PlayerDataManager.playerData.ingredients.herbsDict [id].count--;
		//				AddedHerb = new KeyValuePair<string, int>(AddedHerb.Key,AddedHerb.Value+count);
		//				return 0;
		//			} else
		//				return 1;
		//		} else
		//			return 2;
		//	} else {
		//		if (CheckAvailability (type, id)) {
		//			PlayerDataManager.playerData.ingredients.herbsDict [id].count--;
		//			AddedHerb = new KeyValuePair<string, int>(id,count);
		//			return 0;
		//		}
		//	}
		//}
		return 1;
	}

	static bool CheckAvailability(IngredientType type, string id){
        //if (type == IngredientType.gem) {
        //	if (PlayerDataManager.playerData.ingredients.gemsDict [id].count > 0)
        //		return true;
        //	else
        //		return false;
        //} else if (type == IngredientType.tool) {
        //	if (PlayerDataManager.playerData.ingredients.toolsDict [id].count > 0)
        //		return true;
        //	else
        //		return false;
        //} else {
        //	if (PlayerDataManager.playerData.ingredients.herbsDict [id].count > 0)
        //		return true;
        //	else
        //		return false;
        //}
        return false;
	}
}

public enum IngredientType{
	gem,tool,herb,none
}
