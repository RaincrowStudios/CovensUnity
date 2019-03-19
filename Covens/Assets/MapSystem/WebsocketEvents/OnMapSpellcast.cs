using Newtonsoft.Json;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class OnMapSpellcast
{
    public static System.Action<IMarker, SpellDict, Result> OnSpellcastResult;
    public static System.Action<IMarker, SpellDict, Result> OnPlayerTargeted;
    public static System.Action<IMarker, IMarker, SpellDict, Result> OnSpellCast;

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

        IMarker target;
        SpellDict spell = DownloadedAssets.GetSpell(data.spell);

        if (data.casterInstance == player.instance) //I am the caster
        {
            if (data.target == "portal")
                return;

            target = MarkerManager.GetMarker(data.targetInstance);

            if (target == null)
            {
                Debug.LogError("NULL TARGET? " + data.targetInstance);
                return;
            }
            SoundManagerOneShot.Instance.PlayWhisperFX();
            Token token = target.customData as Token;

            if (data.result.effect == "success")
            {
                ////focus on the target only if the spell is succesfully cast
                SoundManagerOneShot.Instance.PlayCrit();

                if (data.spell == "spell_banish")
                {
                    //spawn the banish glyph and remove the marker
                    SpellcastingFX.SpawnBanish(target, 0);
                    LeanTween.value(0, 1, 0).setOnStart(() => MarkerSpawner.DeleteMarker(token.instance)).setDelay(1.5f);
                }
                else
                {
                    //spawn the spell glyph and aura
                    DelayedFeedback(0.6f, target, spell, data.baseSpell, data.result.total);

                    //add the immunity in case the map_immunity_add did not arrive yet
                    if (token.Type == MarkerSpawner.MarkerType.witch)
                        MarkerSpawner.AddImmunity(player.instance, token.instance);
                }
            }
            else if (data.result.effect == "backfire")
            {
                int damage = (int)Mathf.Abs(data.result.total);
                PlayerManagerUI.Instance.UpdateEnergy();

                StreetMapUtils.FocusOnTarget(PlayerManager.marker);
                SpellcastingFX.SpawnBackfire(PlayerManager.marker, damage, 0.6f, true);
            }
            else if (data.result.effect == "fail")
            {
                StreetMapUtils.FocusOnTarget(PlayerManager.marker);
                SpellcastingFX.SpawnFail(PlayerManager.marker, 0.6f);
            }

            if (target is WitchMarker)
            {
                (target as WitchMarker).GetPortrait(spr =>
                {
                    PlayerNotificationManager.Instance.ShowNotification(
                       SpellcastingTextFeedback.CreateSpellDescription_Caster(data),
                       spr
                   );
                });
            }
            else if (target is SpiritMarker)
            {
                PlayerNotificationManager.Instance.ShowNotification(
                       SpellcastingTextFeedback.CreateSpellDescription_Caster(data),
                       (target as SpiritMarker).tierIcon
                   );
            }

            

            OnSpellcastResult?.Invoke(target, spell, data.result);
            OnSpellCast?.Invoke(PlayerManager.marker, target, spell, data.result);
            return;
        }


        if (string.IsNullOrEmpty(data.targetInstance))
        {
            Debug.LogError("EMPTY TARGET ON MAP_SPELL_CAST\n" + data.json);
            return;
        }


        IMarker caster = MarkerManager.GetMarker(data.casterInstance);
        Token casterToken = caster.customData as Token;

        // i am the target
        if (data.targetInstance == player.instance)
        {
            target = PlayerManager.marker;

            if (data.result.effect == "success")
            {
                if (data.spell == "spell_banish")
                {
                    //spawn the banish glyph and remove the marker
                    SpellcastingFX.SpawnBanish(PlayerManager.marker, 0);
                }
                else
                {
                    //spawn the spell glyph and aura
                    DelayedFeedback(0, target, spell, data.baseSpell, data.result.total, null, false);
                }

                MarkerSpawner.RemoveImmunity(player.instance, casterToken.instance);
            }
            else if (data.result.effect == "backfire")
            {
                int damage = (int)Mathf.Abs(data.result.total);
                
                SpellcastingFX.SpawnBackfire(caster, damage, 0.0f, false);
            }
            else if (data.result.effect == "fail")
            {
                SpellcastingFX.SpawnFail(caster, 0);
            }

            if (data.spell == "spell_banish")
            {
                BanishManager.banishCasterID = data.caster;
            }

            if (data.result.total < 0)
            {
                MarkerManager.StanceDict[data.casterInstance] = true;
            }
            else if (data.result.total > 0)
            {
                MarkerManager.StanceDict[data.casterInstance] = false;
            }
            if (MarkerManager.Markers.ContainsKey(data.casterInstance))
            {
                var tokenD = MarkerManager.Markers[data.casterInstance][0].customData as Token;
                MarkerSpawner.Instance.SetupStance(MarkerManager.Markers[data.casterInstance][0].gameObject.transform, tokenD);
            }

            //if (MapSelection.currentView == CurrentView.MapView)
            //{
            //    string msg = "";
            //    if (data.spell != "attack")
            //    {
            //        if (data.result.total > 0)
            //        {
            //            msg = " cast " + DownloadedAssets.spellDictData[data.spell].spellName + " on you. You gain " + data.result.total.ToString() + " Energy.";
            //        }
            //        else if (data.result.total < 0)
            //        {
            //            msg = " cast " + DownloadedAssets.spellDictData[data.spell].spellName + " on you. You lose " + data.result.total.ToString() + " Energy.";
            //        }
            //        else
            //        {
            //            msg = " cast " + DownloadedAssets.spellDictData[data.spell].spellName + " on you.";
            //        }
            //    }
            //    else
            //    {
            //        msg = " attacked you, you lose " + data.result.total.ToString() + " Energy.";
            //    }

            //    if (data.casterType == "witch")
            //    {
            //        msg = data.caster + msg;
            //    }
            //    else if (data.casterType == "spirit")
            //    {
            //        msg = "Spirit " + DownloadedAssets.spiritDictData[data.caster].spiritName + msg;
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}

            if (caster is WitchMarker)
            {
                (caster as WitchMarker).GetPortrait(spr =>
                {
                    PlayerNotificationManager.Instance.ShowNotification(
                       SpellcastingTextFeedback.CreateSpellDescription_Target(data),
                       spr
                   );
                });
            }
            else if (caster is SpiritMarker)
            {
                PlayerNotificationManager.Instance.ShowNotification(
                       SpellcastingTextFeedback.CreateSpellDescription_Target(data),
                       (caster as SpiritMarker).tierIcon
                   );
            }


            OnPlayerTargeted?.Invoke(caster, spell, data.result);
            OnSpellCast?.Invoke(caster, target, spell, data.result);
        }
        else //other witches are fighting
        {
            target = MarkerManager.GetMarker(data.targetInstance);
            Token targetToken = target.customData as Token;

            if (data.result.effect == "success")
            {
                if (data.spell == "spell_banish")
                {
                    //spawn the banish glyph and remove the marker
                    SpellcastingFX.SpawnBanish(target, 0);
                    LeanTween.value(0, 1, 0).setOnStart(() => MarkerSpawner.DeleteMarker(data.targetInstance)).setDelay(1.5f);
                }
                else
                {
                    //spawn the spell glyph and aura
                    DelayedFeedback(0, target, spell, data.baseSpell, data.result.total, null, false);
                }
            }
            else if (data.result.effect == "backfire")
            {
                int damage = (int)Mathf.Abs(data.result.total);

                SpellcastingFX.SpawnBackfire(caster, damage, 0.0f, false);
            }
            else if (data.result.effect == "fail")
            {
                SpellcastingFX.SpawnFail(caster, 0);
            }

            OnSpellCast?.Invoke(caster, target, spell, data.result);
        }
    }
}
