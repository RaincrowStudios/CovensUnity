using Newtonsoft.Json;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class OnMapSpellcast
{
    public static System.Action<string, SpellDict, Result> OnSpellcastResult;
    public static System.Action<string, SpellDict, Result> OnPlayerTargeted;
    public static System.Action<string, string, SpellDict, Result> OnSpellCast;
    

    public static void HandleEvent(WSData data)
    {
        // Debug.Log("|||" + data.json);
        PlayerDataDetail player = PlayerDataManager.playerData;
        SpellDict spell = DownloadedAssets.GetSpell(data.spell);
        bool isCaster = data.casterInstance == player.instance;
        bool isTarget = data.targetInstance == player.instance;

        if (data.casterType == "spirit" && data.baseSpell == "")
        {
            data.baseSpell = "attack";
        }
        
        OnSpellCast?.Invoke(data.casterInstance, data.targetInstance, spell, data.result);

        if (isCaster)
            OnSpellcastResult?.Invoke(data.targetInstance, spell, data.result);
        if (isTarget)
            OnPlayerTargeted?.Invoke(data.casterInstance, spell, data.result);

        IMarker caster = isCaster ? PlayerManager.marker : MarkerSpawner.GetMarker(data.casterInstance);
        IMarker target = isTarget ? PlayerManager.marker : MarkerSpawner.GetMarker(data.targetInstance);
        string baseSpell = data.baseSpell;
        int damage = data.result.total;
        int casterNewEnergy = data.casterEnergy;
        int targetNewEnergy = data.targetEnergy;

        //if (data.result.effect == "fizzle" || data.result.effect == "fail")
        //    return;

        SpellcastingTrailFX.SpawnTrail(spell.spellSchool, caster, target,
            () =>
            {
                //trigger a map_energy_change event for the caster
                LeanTween.value(0, 0, 0.25f).setOnComplete(() => OnMapEnergyChange.ForceEvent(caster, casterNewEnergy, data.timestamp));

                //spell text for the energy lost casting the spell
                if (isCaster && caster != null)
                {
                    foreach (SpellData _spell in PlayerDataManager.playerData.spells)
                    {
                        if (_spell.id == spell.spellID)
                        {
                            SpellcastingFX.SpawnDamage(caster, -_spell.cost);
                            break;
                        }
                    }
                }

                if (data.result.effect == "backfire")
                    SpellcastingFX.SpawnBackfire(caster, Mathf.Abs(data.result.total), 0, isCaster);
            },
            () =>
            {
                //trigger a map_energy_change event for the target
                LeanTween.value(0, 0, 0.25f).setOnComplete(() => OnMapEnergyChange.ForceEvent(target, targetNewEnergy, data.timestamp));

                if (target != null)
                {
                    if (data.result.effect == "success")
                    {
                        if (isTarget)
                        {
                            if (data.spell == "spell_banish")
                                UISpellcasting.Instance.Hide();
                            else if (data.spell == "spell_bind")
                                BanishManager.Instance.ShowBindScreen(data);
                            else if (data.spell == "spell_silence")
                                BanishManager.Instance.Silenced(data);
                        }
                        else
                        {
                            //spawn the banish fx and remove the marker
                            if (data.spell == "spell_banish")
                            {
                                target.interactable = false;
                                SpellcastingFX.SpawnBanish(target, 0);
                                //make sure marker is removed in case the server doesnt send the map_token_remove
                                OnMapTokenRemove.ForceEvent(data.targetInstance);
                            }
                        }

                        //spawn the spell glyph
                        if (data.spell != "spell_banish")
                        {
                            SpellcastingFX.SpawnGlyph(target, spell, baseSpell);
                            SpellcastingFX.SpawnDamage(target, damage);
                        }
                    }
                    else if (data.result.effect == "fail")
                    {
                        SpellcastingFX.SpawnFail(target, 0);
                    }
                }

                if (isTarget || isCaster)
                {
                    //shake slightly if being healed
                    if (damage > 0) //healed
                    {
                        MapCameraUtils.ShakeCamera(
                            new Vector3(1, -5, 1),
                            0.05f,
                            0.6f,
                            2f
                        );
                    }
                    //shake more if taking damage
                    else if (damage < 0) //dealt damage
                    {
                        MapCameraUtils.ShakeCamera(
                            new Vector3(1, -5, 5),
                            0.2f,
                            0.3f,
                            1f
                        );
                    }
                }
                if (isTarget && !isCaster)
                {
                    if (caster is WitchMarker)
                    {
                        (caster as WitchMarker).GetPortrait(spr => {
                            PlayerNotificationManager.Instance.ShowNotification(SpellcastingTextFeedback.CreateSpellFeedback(caster, target, data), spr);
                        });
                    }
                    else if (caster is SpiritMarker)
                    {
                        PlayerNotificationManager.Instance.ShowNotification(SpellcastingTextFeedback.CreateSpellFeedback(caster, target, data));
                    }
                    else
                    {
                        Debug.LogError("something went wrong");
                    }
                }
            });
    }
}
