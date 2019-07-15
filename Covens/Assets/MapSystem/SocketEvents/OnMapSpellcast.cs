using Raincrow.GameEventResponses;
using Raincrow.Maps;
using UnityEngine;

public static class OnMapSpellcast
{
    public static System.Action<string, SpellData, SpellCastResult> OnSpellcastResult;
    public static System.Action<string, SpellData, SpellCastResult> OnPlayerTargeted;
    public static System.Action<string, string, SpellData, SpellCastResult> OnSpellCast;
    

    public static void HandleEvent(SpellCastResponse response)
    {
        // Debug.Log("|||" + data.json);
        PlayerDataDetail player = PlayerDataManager.playerData;
        SpellData spell = DownloadedAssets.GetSpell(response.Spell);
        bool playerIsCaster = response.Caster.Id == player.instance;
        bool playerIsTarget = response.Target.Id == player.instance;
        
        OnSpellCast?.Invoke(response.Caster.Id, response.Target.Id, spell, response.Result);

        if (playerIsCaster)
        {
            OnSpellcastResult?.Invoke(response.Target.Id, spell, response.Result);
        }
        if (playerIsTarget)
        {
            OnPlayerTargeted?.Invoke(response.Caster.Id, spell, response.Result);
        }

        IMarker caster = playerIsCaster ? PlayerManager.marker : MarkerManager.GetMarker(response.Caster.Id);
        IMarker target = playerIsTarget ? PlayerManager.marker : MarkerManager.GetMarker(response.Target.Id);
        int damage = response.Result.Damage;
        int casterNewEnergy = response.Caster.Energy;
        int targetNewEnergy = response.Target.Energy;

        //if (data.result.effect == "fizzle" || data.result.effect == "fail")
        //    return;

        SpellcastingTrailFX.SpawnTrail(spell.school, caster, target,
            () =>
            {
                //trigger a map_energy_change event for the caster
                LeanTween.value(0, 0, 0.25f).setOnComplete(() => OnMapEnergyChange.ForceEvent(caster, casterNewEnergy, response.Timestamp));

                //spell text for the energy lost casting the spell
                if (playerIsCaster && caster != null)
                {
                    SpellcastingFX.SpawnDamage(caster, -spell.cost);
                }

                //if (response.result.effect == "backfire")
                //    SpellcastingFX.SpawnBackfire(caster, Mathf.Abs(response.result.total), 0, playerIsCaster);
            },
            () =>
            {
                //trigger a map_energy_change event for the target
                LeanTween.value(0, 0, 0.25f).setOnComplete(() => OnMapEnergyChange.ForceEvent(target, targetNewEnergy, response.Timestamp));

                if (target != null)
                {
                    if (response.Result.IsSuccess)
                    {
                        if (playerIsTarget)
                        {
                            if (response.Spell == "spell_banish")
                            {
                                UISpellcasting.Instance.Hide();
                            }
                            else if (response.Spell == "spell_bind")
                            {
                                BanishManager.Instance.ShowBindScreen(response);
                            }
                            else if (response.Spell == "spell_silence")
                            {
                                BanishManager.Instance.Silenced(response);
                            }
                        }
                        else
                        {
                            //spawn the banish fx and remove the marker
                            if (response.Spell == "spell_banish")
                            {
                                target.interactable = false;
                                SpellcastingFX.SpawnBanish(target, 0);
                                //make sure marker is removed in case the server doesnt send the map_token_remove
                                OnMapTokenRemove.ForceEvent(response.Target.Id);
                            }
                        }

                        //spawn the spell glyph
                        if (response.Spell != "spell_banish")
                        {
                            SpellcastingFX.SpawnGlyph(target, spell, response.Spell);
                            SpellcastingFX.SpawnDamage(target, damage);
                        }
                    }
                    else
                    {
                        SpellcastingFX.SpawnFail(target, 0);
                    }
                }

                if (playerIsTarget || playerIsCaster)
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
                if (playerIsTarget && !playerIsCaster)
                {
                    if (caster is WitchMarker)
                    {
                        (caster as WitchMarker).GetPortrait(spr => {
                            PlayerNotificationManager.Instance.ShowNotification(SpellcastingTextFeedback.CreateSpellFeedback(caster, target, response), spr);
                        });
                    }
                    else if (caster is SpiritMarker)
                    {
                        PlayerNotificationManager.Instance.ShowNotification(SpellcastingTextFeedback.CreateSpellFeedback(caster, target, response));
                    }
                    else
                    {
                        Debug.LogError("something went wrong");
                    }
                }
            });
    }
}
