using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;
using Newtonsoft.Json;

public class Spellcasting
{
    public static event System.Action<SpellData, IMarker> OnSpellCast;

    public static void CastSpell(SpellData spell, IMarker target, List<spellIngredientsData> ingredients, System.Action<int, string> callback)
    {
        var data = new SpellTargetData();
        data.spell = spell.id;
        data.target = (target.customData as Token).instance;
        data.ingredients = ingredients;
                
        LoadingOverlay.Show();
        APIManager.Instance.PostCoven(
            "spell/targeted",
            JsonConvert.SerializeObject(data), 
            (response, result) => 
            {
                CastSpellCallback(response, result);
                callback?.Invoke(result, response);
                LoadingOverlay.Hide();

                if (result == 200)
                    OnSpellCast?.Invoke(spell, target);
            });
    }

    private static void CastSpellCallback(string response, int result)
    {
        if (result == 200)
        {
        }
        else
        {
            if (response == "4301") //target dead
            {
            }
            else if (response == "4700") //you are dead
            {
            }
            else if (response == "4704") //target escaped
            {
            }
            else
            {
                UIGlobalErrorPopup.ShowError(() => { }, "Unknown error [" + result + "] " + response);
            }
        }
    }
}
