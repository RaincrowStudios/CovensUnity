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
        PlayerDead,
        InvalidSpell,
    }
    
    /// <summary>
    /// This is actually the callback <see cref="OnMapSpellcast.OnSpellcastResult"/>.
    /// </summary>
    public static System.Action<IMarker, SpellDict, Result> OnSpellCast
    {
        get { return OnMapSpellcast.OnSpellcastResult; }
        set { OnMapSpellcast.OnSpellcastResult = value; }
    }

    public static SpellState CanCast(SpellData spell = null, IMarker target = null, MarkerDataDetail data = null)
    {
        //PLAYER
        //silenced
        if (BanishManager.isSilenced)
            return SpellState.PlayerSilenced;

        //dead
        if (DeathState.IsDead)
            return SpellState.PlayerDead;

        //energy


        //TARGET
        if (target != null)
        {
            Token token = target.customData as Token;

            //immunity
            if (MarkerSpawner.IsPlayerImmune(token.instance))
                return SpellState.TargetImmune;
        }


        //SPELL
        if (spell != null)
        {
            //unlocked?

            //check ingredients
            if (spell.ingredients != null)
            {
                for (int i = 0; i < spell.ingredients.Length; i++)
                {
                    if (PlayerDataManager.playerData.ingredients.Amount(spell.ingredients[i]) == 0)
                        return SpellState.MissingIngredients;
                }
            }

            if (data != null)
            {
                //check player states
                if (spell.states.Contains(data.state) == false)
                    return SpellState.InvalidState;
            }
        }

        return SpellState.CanCast;
    }

    public static SpellState CanCast(string spell = null, IMarker target = null, MarkerDataDetail data = null)
    {
        foreach (SpellData _spell in PlayerDataManager.playerData.spells)
        {
            if (spell == _spell.id)
                return CanCast(_spell, target, data);
        }

        return SpellState.InvalidSpell;
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
        
        //despawn the aura and show the results UI
        System.Action<IMarker, SpellDict, Result> resultCallback = null;
        resultCallback = (_target, _spell, _result) =>
        {
            OnSpellCast -= resultCallback;
            
            LeanTween.value(0, 0, 0).setDelay(0.5f).setOnStart(() =>
            {
                UIWaitingCastResult.Instance.ShowResults(_spell, _result);
            });

            //update the ingredients
            PlayerDataManager.RemoveIngredients(ingredients); 
        };

        OnSpellCast += resultCallback;

        //LoadingOverlay.Show();
        APIManager.Instance.PostCoven(
            "spell/targeted",
            JsonConvert.SerializeObject(data),
            (_response, _result) =>
            {
                if (_result == 200 && _response != "OK")
                {
                    Debug.LogError("spell/target server error");

                    //force fail
                    SpellDict _spellData = DownloadedAssets.GetSpell(spell.id);
                    Result _spellResult = new Result
                    {
                        effect = "fail"
                    };
                    resultCallback.Invoke(target, _spellData, _spellResult);
                }
                CastSpellCallback(_response, _result);
            }
        );
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
                UIGlobalErrorPopup.ShowError(() => { }, "Error: " + response);
            }
        }
    }
}
