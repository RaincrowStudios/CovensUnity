using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;
using Newtonsoft.Json;

public class Spellcasting
{
    public enum SpellState
    {
        CanCast = 0,
        Locked,
        TargetImmune,
        PlayerSilenced,
        MissingIngredients,
        InvalidState,
    }
    
    /// <summary>
    /// This is actually the callback <see cref="OnMapSpellcast.OnSpellcastResult"/>.
    /// </summary>
    public static System.Action<IMarker, SpellDict, Result> OnSpellCast
    {
        get { return OnMapSpellcast.OnSpellcastResult; }
        set { OnMapSpellcast.OnSpellcastResult = value; }
    }

    public static SpellState CanCast(SpellData spell, IMarker target = null, MarkerDataDetail data = null)
    {
        Token token = target.customData as Token;
        //unlocked?

        //immunity
        if (MarkerSpawner.IsPlayerImmune(token.instance))
            return SpellState.TargetImmune;

        //silenced

        //check ingredients
        if (spell.ingredients != null)
        {
            for (int i = 0; i < spell.ingredients.Count; i++)
            {
                if (PlayerDataManager.playerData.ingredients.Amount(spell.ingredients[i].id) < spell.ingredients[i].count)
                    return SpellState.MissingIngredients;
            }
        }

        //check player states
        if (spell.states.Contains(data.state) == false)
            return SpellState.InvalidState;

        return SpellState.CanCast;
    }

    public static void CastSpell(SpellData spell, IMarker target, List<spellIngredientsData> ingredients, System.Action<Result> onContinue)
    {
        var data = new SpellTargetData();
        data.spell = spell.id;
        data.target = (target.customData as Token).instance;
        data.ingredients = ingredients;

        //slowly shake the screen while waiting for the cast response
        StreetMapUtils.ShakeCamera(
            new Vector3(1, -5, 5),
            0.02f,
            1f,
            10f
        );
        
        //show the animted UI
        UIWaitingCastResult.Instance.Show(target, spell, ingredients, (_result) =>
        {
            onContinue?.Invoke(_result);
        });

        //spawn the casting aura
        //SpellcastingFX.SpawnCastingAura(PlayerManager.marker, spell.school);

        //despawn the aura and show the results UI
        System.Action<IMarker, SpellDict, Result> resultCallback = null;
        resultCallback = (_target, _spell, _result) =>
        {
            OnSpellCast -= resultCallback;

            //SpellcastingFX.DespawnCastingAura(PlayerManager.marker);

            LeanTween.value(0, 0, 0).setDelay(0.5f).setOnStart(() =>
            {
                UIWaitingCastResult.Instance.CloseLoading();
                UIWaitingCastResult.Instance.ShowResults(_spell, _result);
            });
        };

        OnSpellCast += resultCallback;

        //LoadingOverlay.Show();
        APIManager.Instance.PostCoven(
            "spell/targeted",
            JsonConvert.SerializeObject(data),
            CastSpellCallback);
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
            else if (response == "4601") // target immune
            {

            }
            else
            {
                UIGlobalErrorPopup.ShowError(() => { }, "Unknown error [" + result + "] " + response);
            }
        }
    }
}
