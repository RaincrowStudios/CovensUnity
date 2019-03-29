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

    public static void DelayedFeedback(float delay, IMarker target, SpellDict spell, string baseSpell, int damage, string textColor = null, bool shake = true)
    {
        LeanTween.value(0, 1, 0)
            .setOnStart(
            () =>
            {
                SpellcastingFX.SpawnGlyph(target, spell, baseSpell);
                SpellcastingFX.SpawnDamage(target, damage);

                if (shake)
                {
                    //shake slightly if being healed
                    if (damage > 0) //healed
                    {
                        StreetMapUtils.ShakeCamera(
                            new Vector3(1, -5, 1),
                            0.05f,
                            0.6f,
                            2f
                        );
                    }
                    //shake more if taking damage
                    else if (damage < 0) //dealt damage
                    {
                        StreetMapUtils.ShakeCamera(
                            new Vector3(1, -5, 5),
                            0.2f,
                            0.3f,
                            1f
                        );
                    }
                }
            })
            .setDelay(delay);
    }


    public static void HandleEvent(WSData data)
    {
        MarkerDataDetail player = PlayerDataManager.playerData;
        SpellDict spell = DownloadedAssets.GetSpell(data.spell);
        bool isCaster = data.casterInstance == player.instance;
        bool isTarget = data.targetInstance == player.instance;

        OnSpellCast?.Invoke(data.casterInstance, data.targetInstance, spell, data.result);

        if (isCaster)
            OnSpellcastResult?.Invoke(data.targetInstance, spell, data.result);
        if (isTarget)
            OnPlayerTargeted?.Invoke(data.casterInstance, spell, data.result);

        if (isCaster && !isTarget)
        {
            if (data.target == "portal")
                return;

            SoundManagerOneShot.Instance.PlayWhisperFX();
            if (data.result.effect == "success")
                SoundManagerOneShot.Instance.PlayCrit();

            IMarker targetMarker = MarkerManager.GetMarker(data.targetInstance);

            //marker dependant feedbacks
            if (targetMarker != null)
            {
                if (data.result.effect == "success")
                {
                    //spawn the banish fx
                    if (data.spell == "spell_banish")
                        SpellcastingFX.SpawnBanish(targetMarker, 0);
                    //spawn the spell glyph and aura
                    else
                        DelayedFeedback(0.6f, targetMarker, spell, data.baseSpell, data.result.total);
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
					Debug.Log ("spirit was casted on.");
                    PlayerNotificationManager.Instance.ShowNotification(
                        SpellcastingTextFeedback.CreateSpellFeedback(PlayerManager.marker, targetMarker, data),
                        (targetMarker as SpiritMarker).tierIcon
                    );
                }
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
                    BanishManager.banishCasterID = data.caster;
                    UISpellcasting.Instance.Hide();
                }
                else if (data.spell == "spell_bind")
                {
                    BanishManager.Instance.Bind(data);
                }
                else if (data.spell == "spell_silence")
                {
                    BanishManager.Instance.Silenced(data);
                    UISpellcasting.Instance.Hide();

                }
                else
                    DelayedFeedback(0, target, spell, data.baseSpell, data.result.total, null, false);
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

            return;
        }

        if (!isCaster && !isTarget)
        {
            IMarker caster = MarkerManager.GetMarker(data.casterInstance);
            IMarker target = MarkerManager.GetMarker(data.targetInstance);

            if (target != null)
            {
                if (data.result.effect == "success")
                {
                    if (data.spell == "spell_banish")
                        SpellcastingFX.SpawnBanish(target, 0);
                    else
                        DelayedFeedback(0, target, spell, data.baseSpell, data.result.total, null, false);
                }
            }
            //else if (data.result.effect == "backfire")
            //{
            //    int damage = (int)Mathf.Abs(data.result.total);
            //    SpellcastingFX.SpawnBackfire(caster, damage, 0.0f, false);
            //}
            //else if (data.result.effect == "fail")
            //{
            //    SpellcastingFX.SpawnFail(caster, 0);
            //}
        }
    }
}
