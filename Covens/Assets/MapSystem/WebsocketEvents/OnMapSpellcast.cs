using Newtonsoft.Json;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class OnMapSpellcast
{
    public static System.Action<IMarker, SpellDict, Result> OnSpellcastResult;

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

        if (data.casterInstance == player.instance) //I am the caster
        {
            if (data.target == "portal")
                return;

            IMarker target = MarkerManager.GetMarker(data.targetInstance);
            Token token = target.customData as Token;
            SpellDict spell = DownloadedAssets.spellDictData[data.spell];

            if (target == null)
            {
                Debug.LogError("NULL TARGET? " + data.targetInstance);
                return;
            }
            SoundManagerOneShot.Instance.PlayWhisperFX();

            if (data.result.effect == "success")
            {
                ////focus on the target only if the spell is succesfully cast
                //StreetMapUtils.FocusOnTarget(target);
                SoundManagerOneShot.Instance.PlayCrit();
                //spawn the spell glyph and aura
                DelayedFeedback(0.6f, target, spell, data.baseSpell, data.result.total);

                //add the immunity in case the map_immunity_add did not arrive yet
                MarkerSpawner.AddImmunity(player.instance, token.instance);

                //update the witch's energy
                if (data.result.total != 0)
                {
                    token.energy += data.result.total;
                    target.SetStats(token.level, token.energy);
                }
            }
            else if (data.result.effect == "backfire")
            {
                int damage = (int)Mathf.Abs(data.result.total);
                PlayerDataManager.playerData.energy -= damage;
                PlayerManagerUI.Instance.UpdateEnergy();

                StreetMapUtils.FocusOnTarget(PlayerManager.marker);
                SpellcastingFX.SpawnBackfire(PlayerManager.marker, damage, 0.6f, true);
            }
            else if (data.result.effect == "fail")
            {
                SpellcastingFX.SpawnFail(PlayerManager.marker, 0);
            }

            OnSpellcastResult?.Invoke(target, spell, data.result);

            return;
            //				SpellSpiralLoader.Instance.LoadingDone ();
            SpellManager.Instance.loadingFX.SetActive(false);
            SpellManager.Instance.mainCanvasGroup.interactable = true;
            if (data.spell == "spell_banish" && data.result.effect == "success")
            {
                HitFXManager.Instance.Banish();
                return;
            }
            if (data.targetInstance == MarkerSpawner.instanceID && MapSelection.currentView == CurrentView.IsoView)
            {
                HitFXManager.Instance.Attack(data);
            }
            if (MapSelection.currentView == CurrentView.IsoView)
            {
                if (data.result.effect == "fail" || data.result.effect == "fizzle")
                {
                    HitFXManager.Instance.Attack(data);
                }
                if (data.result.effect == "backfire")
                {
                    HitFXManager.Instance.Attack(data);
                }
            }
        }
        if (LocationUIManager.isLocation && MapSelection.currentView != CurrentView.IsoView)
        {
            if (data.result.effect == "success")
                MovementManager.Instance.AttackFXOther(data);
            return;
        }
        if (data.targetInstance == player.instance && MapSelection.currentView == CurrentView.MapView)
        {
            MovementManager.Instance.AttackFXSelf(data);
        }
        if (data.targetInstance != player.instance && MapSelection.currentView == CurrentView.MapView)
        {
            MovementManager.Instance.AttackFXOther(data);
        }

        if (data.targetInstance == player.instance)
        {

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

            if (MapSelection.currentView == CurrentView.MapView)
            {
                string msg = "";
                if (data.spell != "attack")
                {
                    if (data.result.total > 0)
                    {
                        msg = " cast " + DownloadedAssets.spellDictData[data.spell].spellName + " on you. You gain " + data.result.total.ToString() + " Energy.";
                    }
                    else if (data.result.total < 0)
                    {
                        msg = " cast " + DownloadedAssets.spellDictData[data.spell].spellName + " on you. You lose " + data.result.total.ToString() + " Energy.";
                    }
                    else
                    {
                        msg = " cast " + DownloadedAssets.spellDictData[data.spell].spellName + " on you.";
                    }
                }
                else
                {
                    msg = " attacked you, you lose " + data.result.total.ToString() + " Energy.";
                }

                if (data.casterType == "witch")
                {
                    msg = data.caster + msg;
                }
                else if (data.casterType == "spirit")
                {
                    msg = "Spirit " + DownloadedAssets.spiritDictData[data.caster].spiritName + msg;
                }
                else
                {
                    return;
                }
                if (MarkerManager.Markers.ContainsKey(data.casterInstance))
                {
                    var cData = MarkerManager.Markers[data.casterInstance][0].customData as Token;
                    var Sprite = PlayerNotificationManager.Instance.ReturnSprite(cData.male);
                    PlayerNotificationManager.Instance.showNotification(msg, Sprite);
                }
            }
        }

        if (data.casterInstance == MarkerSpawner.instanceID && data.targetInstance == player.instance && MapSelection.currentView == CurrentView.IsoView)
        {
            if (data.result.effect == "backfire")
            {
                HitFXManager.Instance.BackfireEnemy(data);
            }
            else
            {
                HitFXManager.Instance.Hit(data);
            }
        }
    }
}
