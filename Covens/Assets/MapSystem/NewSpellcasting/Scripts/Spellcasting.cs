using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Maps;
using Newtonsoft.Json;
using Raincrow.GameEventResponses;

public class Spellcasting
{
    public enum SpellState
    {
        /// <summary>
        /// spell is valid
        /// </summary>
        CanCast = 0,

        /// <summary>
        /// *not implemented*
        /// </summary>
        Locked,

        /// <summary>
        /// the target is immune to the player
        /// </summary>
        TargetImmune,

        /// <summary>
        /// the player is silenced
        /// </summary>
        PlayerSilenced,

        /// <summary>
        /// the player is missing ingredients
        /// </summary>
        MissingIngredients,

        /// <summary>
        /// the target's state does not match any of the spells required state
        /// </summary>
        InvalidState,

        /// <summary>
        /// the player is dead
        /// </summary>
        PlayerDead,

        /// <summary>
        /// the spell was not found
        /// </summary>
        InvalidSpell,

        /// <summary>
        /// the spell can only be used inside places of power
        /// </summary>
        NotInPop,

        /// <summary>
        /// the spell is under cooldown
        /// </summary>
        InCooldown,
    }
    
    private static Dictionary <string, System.Action<SpellData, IMarker, List<spellIngredientsData>, System.Action<Raincrow.GameEventResponses.SpellCastHandler.Result>, System.Action>> m_SpecialSpells = 
        new Dictionary<string, System.Action<SpellData, IMarker, List<spellIngredientsData>, System.Action<Raincrow.GameEventResponses.SpellCastHandler.Result>, System.Action>>
        {
            { "spell_channeling", SpellChanneling.CastSpell }
        };


    ///// <summary>
    ///// This is actually the callback <see cref="OnMapSpellcast.OnSpellcastResult"/>.
    ///// </summary>
    //public static System.Action<string, SpellData, DamageResult> OnSpellCast
    //{
    //    get { return OnMapSpellcast.OnSpellcastResult; }
    //    set { OnMapSpellcast.OnSpellcastResult = value; }
    //}

    public static SpellState CanCast(SpellData spell = null, IMarker target = null, CharacterMarkerData data = null)
    {
        //PLAYER        
        if (spell != null && DownloadedAssets.spellDictData.ContainsKey(spell.id) == false)
            return SpellState.InvalidSpell;

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

            if (target == PlayerManager.marker)
            {

            }
            else
            {
                if (token.Type == MarkerSpawner.MarkerType.SPIRIT)
                {
                    //temp fix: disable banish of spirits on pop
                    if (PlaceOfPower.IsInsideLocation && spell != null && spell.id == "spell_banish")
                        return SpellState.InvalidState;
                }
                else if (token.Type == MarkerSpawner.MarkerType.WITCH)
                {
                    //immunity
                    if (MarkerSpawner.IsTargetImmune(token as WitchToken))
                        return SpellState.TargetImmune;
                }
            }
        }


        //SPELL
        if (spell != null)
        {
            //unlocked?

            //is pop only?
            if (spell.pop && PlaceOfPower.IsInsideLocation == false)
                return SpellState.NotInPop;

            //in cooldown?
            if (CooldownManager.GetCooldown(spell.id) != null)
                return SpellState.InCooldown;

            //check ingredients
            if (spell.ingredients != null)
            {
                for (int i = 0; i < spell.ingredients.Length; i++)
                {
                    if (PlayerDataManager.playerData.GetIngredient(spell.ingredients[i]) == 0)
                        return SpellState.MissingIngredients;
                }
            }

            if (PlayerManager.marker == target)
            {
                //check player states
                if (spell.states.Contains(PlayerDataManager.playerData.state) == false)
                    return SpellState.InvalidState;
            }
            else
            {
                if (data != null)
                {
                    //check player states
                    if (spell.states.Contains(data.state) == false)
                        return SpellState.InvalidState;
                }
            }
        }

        return SpellState.CanCast;
    }

    public static SpellState CanCast(string spell = null, IMarker target = null, CharacterMarkerData data = null)
    {
        SpellData spellData = DownloadedAssets.GetSpell(spell);

        if (spellData != null)
            return CanCast(spellData, target, data);

        return SpellState.InvalidSpell;
    }

    public static void CastSpell(SpellData spell, 
                                 IMarker target, 
                                 List<spellIngredientsData> ingredients, 
                                 System.Action<Raincrow.GameEventResponses.SpellCastHandler.Result> onContinue, 
                                 System.Action onClose)
    {
        string targetId = target == PlayerManager.marker ? PlayerDataManager.playerData.instance : target.token.instance;

        var data = new
        {
            spell = spell.id,
            ingredients = ingredients
        };

        //slowly shake the screen while waiting for the cast response
        MapCameraUtils.ShakeCamera(
            new Vector3(1, -5, 5),
            0.02f,
            1f,
            10f
        );
        
        /// SPECIAL FLOW (SPELLS THAT HAVE THEIR OWN REQUESTS
        if (m_SpecialSpells.ContainsKey(spell.id))
        {
            m_SpecialSpells[spell.id].Invoke(
                spell,
                target,
                ingredients,
                (result) =>
                {
                    //on finish spell flow
                    onContinue?.Invoke(result);
                },
                () =>
                {
                    //on cancel spell flow
                    onClose?.Invoke();
                }
            );
        }
        /// DEFAULT FLOW (SEND A SPELL/TARGETED REQUEST)
        else
        {
            //show the animted UI
            UIWaitingCastResult.Instance.Show(target, spell, ingredients,
                (result) =>
                { // on click continue (after spellcast result)
                    onContinue?.Invoke(result);
                },
                () =>
                { //on click close
                    onClose?.Invoke();
                });
            
            //LoadingOverlay.Show();
            APIManager.Instance.Post(
                "character/cast/" + targetId,
                JsonConvert.SerializeObject(data),
                (response, result) =>
                {
                    if (result == 200)
                    {
                        Raincrow.GameEventResponses.SpellCastHandler.SpellCastEventData eventData = JsonConvert.DeserializeObject<Raincrow.GameEventResponses.SpellCastHandler.SpellCastEventData>(response);
                        SpellCastHandler.HandleEvent(eventData);
                        UIWaitingCastResult.Instance.ShowResults(spell, eventData.result);
                    }
                    else
                    {
                        UIWaitingCastResult.Instance.Close();
                           onContinue?.Invoke(new Raincrow.GameEventResponses.SpellCastHandler.Result());
                        UIGlobalErrorPopup.ShowError(null, LocalizeLookUp.GetText("error_" + response));
                    }
                }
            );
        }
    }

    //private static void CastSpellCallback(string response, int result)
    //{
    //    if (result == 200)
    //    {

    //    }
    //    else
    //    {
    //        if (response == "4301") //target dead
    //        {
    //        }
    //        else if (response == "4700") //you are dead
    //        {
    //        }
    //        else if (response == "4704") //target escaped
    //        {
    //        }
    //        else if (response == "4601") // target immune
    //        {

    //        }
    //        else
    //        {
    //            UIGlobalErrorPopup.ShowError(() => { }, "Error: " + response);
    //        }
    //    }
    //}
}
