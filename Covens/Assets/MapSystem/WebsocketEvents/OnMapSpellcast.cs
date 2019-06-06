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

    public static void DelayedFeedback(IMarker caster, IMarker target, SpellDict spell, string baseSpell, int damage, string textColor = null, bool shake = true)
    {
        SpellcastingTrailFX.SpawnTrail(spell.spellSchool, caster, target, 
            () =>
            {
                foreach(SpellData _spell in PlayerDataManager.playerData.spells)
                {
                    if (_spell.id == spell.spellID)
                    {
                        if (caster != null)
                        {
                            OnMapEnergyChange.ForceEvent(caster, -_spell.cost);
                            SpellcastingFX.SpawnDamage(caster, -_spell.cost);
                        }
                        break;
                    }
                }
            },
            () =>
            {
                if (target != null)
                {
                    SpellcastingFX.SpawnGlyph(target, spell, baseSpell);
                    SpellcastingFX.SpawnDamage(target, damage);
                    LeanTween.value(0, 0, 0.25f).setOnComplete(() => OnMapEnergyChange.ForceEvent(target, damage));
                }

                if (shake)
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
        });
    }


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
        
        //IMarker d_Caster = MarkerSpawner.GetMarker(data.casterInstance);
        //IMarker d_Target = MarkerSpawner.GetMarker(data.targetInstance);
        //string d_CasterName = "", d_TargetName = "";
        //if (isCaster)
        //    d_CasterName = PlayerDataManager.playerData.displayName;
        //if (isTarget)
        //    d_TargetName = PlayerDataManager.playerData.displayName;
        //if (!isCaster)
        //    d_CasterName = d_Caster == null ? "null" : (d_Caster.type == MarkerSpawner.MarkerType.witch ? d_Caster.token.displayName : d_Caster.token.spiritId);
        //if (!isTarget)
        //    d_TargetName = d_Target == null ? "null" : (d_Target.type == MarkerSpawner.MarkerType.witch ? d_Target.token.displayName : d_Target.token.spiritId);
        //Debug.LogError("caster:" + d_CasterName + "\ntarget:" + d_TargetName);

        if (isCaster && !isTarget)
        {
            if (data.target == "portal")
                return;

            SoundManagerOneShot.Instance.PlayWhisperFX();
            if (data.result.effect == "success")
                SoundManagerOneShot.Instance.PlayCrit();

            if (data.targetType == "witch" && data.targetState == "dead")
                SoundManagerOneShot.Instance.PlayWitchDead();
            // if(data.)

            IMarker targetMarker = MarkerManager.GetMarker(data.targetInstance);

            //marker dependant feedbacks
            if (targetMarker != null)
            {
                if (data.result.effect == "success")
                {
                    //spawn the banish fx
                    if (data.spell == "spell_banish")
                    {
                        //force onmaptokenremove
                        SpellcastingFX.SpawnBanish(targetMarker, 0);
                        OnMapTokenRemove.ForceEvent(data.targetInstance);
                    }
                    //spawn the spell glyph and aura
                    else
                        DelayedFeedback(PlayerManager.marker, targetMarker, spell, data.baseSpell, data.result.total, null, true);
                }
                else if (data.result.effect == "backfire")
                {
                    SpellcastingFX.SpawnBackfire(PlayerManager.marker, Mathf.Abs(data.result.total), 0.6f, true);
                }
                else if (data.result.effect == "fail")
                {
                    SpellcastingFX.SpawnFail(PlayerManager.marker, 0.6f);
                }

                if (targetMarker is WitchMarker)
                {
                    (targetMarker as WitchMarker).GetPortrait(spr =>
                    {
                        PlayerNotificationManager.Instance.ShowNotification(
                           SpellcastingTextFeedback.CreateSpellFeedback(PlayerManager.marker, targetMarker, data),
                           spr
                       );
                    });
                }
                else if (targetMarker is SpiritMarker)
                {
                    PlayerNotificationManager.Instance.ShowNotification(
                        SpellcastingTextFeedback.CreateSpellFeedback(PlayerManager.marker, targetMarker, data),
                        (targetMarker as SpiritMarker).tierIcon
                    );
                }
            }
            else
            {
                //just update the palyer energy
                DelayedFeedback(PlayerManager.marker, null, spell, data.baseSpell, data.result.total, null, true);
            }
            return;
        }

        // i am the target
        if (!isCaster && isTarget)
        {
            IMarker caster = MarkerManager.GetMarker(data.casterInstance);
            IMarker target = PlayerManager.marker;

            if (data.result.effect == "success")
            {
                if (data.spell == "spell_banish")
                {
                    UISpellcasting.Instance.Hide();
                }
                else if (data.spell == "spell_bind")
                {
                    BanishManager.Instance.ShowBindScreen(data);
                }
                else if (data.spell == "spell_silence")
                {
                    BanishManager.Instance.Silenced(data);
                    if (UISpellcasting.isOpen)
                        UISpellcasting.Instance.UpdateCanCast();
                }
                else
                {
                    DelayedFeedback(caster, target, spell, data.baseSpell, data.result.total, null, false);
                }
            }
            //else if (data.result.effect == "backfire")
            //{                
            //    SpellcastingFX.SpawnBackfire(caster, Mathf.Abs(data.result.total), 0.0f, false);
            //}
            //else if (data.result.effect == "fail")
            //{
            //    SpellcastingFX.SpawnFail(caster, 0);
            //}

            if (data.result.total < 0)
                MarkerManager.StanceDict[data.casterInstance] = true;
            else if (data.result.total > 0)
                MarkerManager.StanceDict[data.casterInstance] = false;

            if (caster != null)
            {
                if (caster is WitchMarker)
                {
                    (caster as WitchMarker).GetPortrait(spr =>
                    {
                        PlayerNotificationManager.Instance.ShowNotification(
                             SpellcastingTextFeedback.CreateSpellFeedback(caster, target, data),
                             spr
                         );
                    });
                }
                else if (caster is SpiritMarker)
                {
                    PlayerNotificationManager.Instance.ShowNotification(
                        SpellcastingTextFeedback.CreateSpellFeedback(caster, target, data),
                        (caster as SpiritMarker).tierIcon
                    );
                }
            }
            else
            {
                //just update the player energy
                DelayedFeedback(null, PlayerManager.marker, spell, data.baseSpell, data.result.total, null, true);
            }

            return;
        }

        if (!isCaster && !isTarget)
        {
            IMarker caster = MarkerManager.GetMarker(data.casterInstance);
            IMarker target = MarkerManager.GetMarker(data.targetInstance);

            if (data.spell == "spell_banish")
            {
                if (data.result.effect == "success" && target != null)
                {
                    SpellcastingFX.SpawnBanish(target, 0);
                    OnMapTokenRemove.ForceEvent(data.targetInstance);
                }
            }
            else
            {
                if (data.result.effect == "success")
                {
                    DelayedFeedback(caster, target, spell, data.baseSpell, data.result.total, null, false);
                }
            }
        }
    }
}
