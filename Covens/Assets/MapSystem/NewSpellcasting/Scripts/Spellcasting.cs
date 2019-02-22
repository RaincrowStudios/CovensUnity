using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;
using Newtonsoft.Json;

public class Spellcasting
{
    public static void CastSpell(SpellData spell, IMarker target, List<spellIngredientsData> ingredients, System.Action<int, string> callback)
    {
        var data = new SpellTargetData();
        data.spell = spell.id;
        data.target = (target.customData as Token).instance;
        data.ingredients = ingredients;

        //if (IngredientManager.addedHerb != "")
        //{
        //    data.ingredients.Add(new spellIngredientsData
        //    {
        //        id = IngredientManager.addedHerb,
        //        count = IngredientManager.addedHerbCount
        //    });
        //}
        //if (IngredientManager.addedTool != "")
        //{
        //    data.ingredients.Add(new spellIngredientsData
        //    {
        //        id = IngredientManager.addedTool,
        //        count = IngredientManager.addedToolCount
        //    });
        //}
        //if (IngredientManager.addedGem != "")
        //{
        //    data.ingredients.Add(new spellIngredientsData
        //    {
        //        id = IngredientManager.addedGem,
        //        count = IngredientManager.addedGemCount
        //    });
        //}
        //IngredientManager.ClearIngredient();

        APIManager.Instance.PostCoven(
            "spell/targeted",
            JsonConvert.SerializeObject(data), 
            (response, result) => 
            {
                CastSpellCallback(response, result); callback?.Invoke(result, response);
            });
    }

    private static void CastSpellCallback(string response, int result)
    {
        if (result == 200)
        {
        }
        else
        {
            //if (response == "4301")
            //{
            //    HitFXManager.Instance.TargetDead(true);
            //}
            //else if (response == "4700")
            //{
            //    PlayerDataManager.playerData.state = "dead";
            //    PlayerDataManager.playerData.energy = 0;
            //}
            //else if (response == "4704")
            //{
            //    HitFXManager.Instance.Escape();
            //}
            //else
            //{
            //    UIGlobalErrorPopup.ShowError(() => { }, "Unknown error [" + result + "]");
            //}
        }
    }
}
