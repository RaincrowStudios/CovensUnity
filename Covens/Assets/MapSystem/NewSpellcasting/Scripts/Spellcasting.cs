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

        /// <summary>
        /// player does not meet the spell's levev requirement
        /// </summary>
        NoLevel,
        
        /// <summary>
        /// the spell cant be casted on yourself or cant be casted on someone else
        /// </summary>
        InvalidTarget,

        /// <summary>
        /// the player's school does not match the required school to cast this spell
        /// </summary>
        InvalidCasterSchool,
        
        /// <summary>
        /// the target's school does not match the required school to target this spell
        /// </summary>
        InvalidTargetSchool,

        /// <summary>
        /// this spell can only be casted on witches or only on spirits
        /// </summary>
        InvalidTargetType,

        /// <summary>
        /// the target does not have the required status effects to cast this spell
        /// </summary>
        InvalidTargetStatus,
    }
    
    private static Dictionary <string, System.Action<SpellData, IMarker, List<spellIngredientsData>, System.Action<Raincrow.GameEventResponses.SpellCastHandler.Result>, System.Action>> m_SpecialSpells = 
        new Dictionary<string, System.Action<SpellData, IMarker, List<spellIngredientsData>, System.Action<Raincrow.GameEventResponses.SpellCastHandler.Result>, System.Action>>
        {
            { "spell_channeling", SpellChanneling.CastSpell }
        };

    public static SpellState CanCast(SpellData spell = null, IMarker target = null, CharacterMarkerData targetData = null)
    {
        //silenced
        if (BanishManager.isSilenced)
            return SpellState.PlayerSilenced;

        //dead
        if (DeathState.IsDead)
            return SpellState.PlayerDead;
        
        //SPELL
        if (spell != null)
        {
            if (DownloadedAssets.spellDictData.ContainsKey(spell.id) == false)
                return SpellState.InvalidSpell;

            if (spell.level > PlayerDataManager.playerData.level)
                return SpellState.NoLevel;

            //caster school
            if (spell.casterSchool != null && spell.casterSchool.Count > 0 && spell.casterSchool.Contains((int)PlayerDataManager.playerData.school) == false)
                return SpellState.InvalidCasterSchool;
                        
            //check ingredients
            if (spell.ingredients != null)
            {
                for (int i = 0; i < spell.ingredients.Count; i++)
                {
                    if (PlayerDataManager.playerData.GetIngredient(spell.ingredients[i]) == 0)
                        return SpellState.MissingIngredients;
                }
            }

            //in cooldown?
            if (CooldownManager.GetCooldown(spell.id) != null)
                return SpellState.InCooldown;
            
            if (target != null)
            {
                Token token = target.Token;

                if (target.IsPlayer)
                {
                    //cant be casted on self
                    if (spell.target == SpellData.Target.OTHER)
                        return SpellState.InvalidTarget;
                }
                else
                {
                    //cant be casted on others
                    if (spell.target == SpellData.Target.SELF)
                        return SpellState.InvalidTarget;

                    //check immunity
                    if (target.Type == MarkerManager.MarkerType.WITCH && MarkerSpawner.IsTargetImmune(target.Token as WitchToken))
                        return SpellState.TargetImmune;
                }

                //targetType
                if (spell.targetType != SpellData.TargetType.ANY)
                {
                    if (target.Type == MarkerManager.MarkerType.WITCH && spell.targetType != SpellData.TargetType.WITCH)
                        return SpellState.InvalidTargetType;

                    if (target.Type == MarkerManager.MarkerType.SPIRIT && spell.targetType != SpellData.TargetType.SPIRIT)
                        return SpellState.InvalidTargetType;
                }

                //targetschool
                if (spell.targetSchool != null && spell.targetSchool.Count > 0 && spell.targetSchool.Contains((int)targetData.school) == false)
                    return SpellState.InvalidTargetSchool;

                //check target state
                if (spell.states.Contains(targetData.state) == false)
                    return SpellState.InvalidState;

                ////target status effect
                //if (spell.targetStatus != null && spell.targetStatus.Count > 0)
                //{
                //    bool statusValid = false;
                //    foreach (string requiredStatus in spell.targetStatus)
                //    {
                //        foreach (var statusEffect in targetData.effects)
                //        {
                //            if (statusEffect.spell == requiredStatus)
                //            {
                //                statusValid = true;
                //                break;
                //            }
                //        }
                //    }
                //    if (statusValid == false)
                //        return SpellState.InvalidTargetStatus;
                //}
            }
        }
        else if (target != null)
        {
            //check immunity
            if (target.Type == MarkerManager.MarkerType.WITCH && MarkerSpawner.IsTargetImmune(target.Token as WitchToken))
                return SpellState.TargetImmune;
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
        string targetId = target == PlayerManager.marker ? PlayerDataManager.playerData.instance : target.Token.instance;

        List<spellIngredientsData> toRemove = new List<spellIngredientsData>();
        List<spellIngredientsData> auxIngr = new List<spellIngredientsData>();

        foreach (spellIngredientsData _ing in ingredients)
        {
            toRemove.Add(new spellIngredientsData(_ing.collectible, _ing.count));

            if (spell.ingredients.Contains(_ing.collectible))
                _ing.count -= 1;
            if (_ing.count > 0)
                auxIngr.Add(_ing);
        }

        var data = new
        {
            spell = spell.id,
            ingredients = auxIngr
        };

        //slowly shake the screen while waiting for the cast response
        MapCameraUtils.ShakeCamera(
            new Vector3(1, -5, 5),
            0.02f,
            1f,
            10f
        );

        //simulate cooldown localy
        CooldownManager.AddCooldown(spell.id, Utilities.GetUnixTimestamp(System.DateTime.UtcNow.AddSeconds(100)), spell.cooldown);

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

            System.Action<string, int> handleResult = (a, b) => { };
            handleResult = (response, result) =>
            {
                if (result == 200)
                {
                    foreach (spellIngredientsData _ing in toRemove)
                        PlayerDataManager.playerData.SubIngredient(_ing.collectible, _ing.count);

                    Raincrow.GameEventResponses.SpellCastHandler.SpellCastEventData eventData = JsonConvert.DeserializeObject<Raincrow.GameEventResponses.SpellCastHandler.SpellCastEventData>(response);
                    SpellCastHandler.HandleEvent(
                        eventData,
                        null,
                        () => UIWaitingCastResult.Instance.ShowResults(spell, eventData.result));
                }
                else
                {
                //retry
                if (response == "2016")
                    {
                        APIManager.Instance.Post(
                            "character/cast/" + targetId,
                            JsonConvert.SerializeObject(data),
                            handleResult
                        );
                        return;
                    }

                //remove the local cooldown if there was an error
                CooldownManager.RemoveCooldown(spell.id);

                    UIWaitingCastResult.Instance.Close();

                //force a remove token event just in case the marker stayed onthe game
                if (response == "1002")
                    {
                        RemoveTokenHandler.ForceEvent(targetId);
                        return;
                    }

                    onContinue?.Invoke(new Raincrow.GameEventResponses.SpellCastHandler.Result());
                    UIGlobalPopup.ShowError(null, APIManager.ParseError(response));
                }
            };

            //LoadingOverlay.Show();
            APIManager.Instance.Post(
                    "character/cast/" + targetId,
                    JsonConvert.SerializeObject(data),
                    handleResult
                );
        }
    }
}
