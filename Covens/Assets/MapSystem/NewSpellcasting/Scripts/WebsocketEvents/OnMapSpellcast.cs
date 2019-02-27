using Newtonsoft.Json;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class OnMapSpellcast
{
    private static SimplePool<Transform> m_ShadowGlyph = new SimplePool<Transform>("SpellFX/SpellGlyph_Shadow");
    private static SimplePool<Transform> m_GreyGlyph = new SimplePool<Transform>("SpellFX/SpellGlyph_Grey");
    private static SimplePool<Transform> m_WhiteGlyph = new SimplePool<Transform>("SpellFX/SpellGlyph_White");

    private static SimplePool<Transform> m_ShadowAura = new SimplePool<Transform>("SpellFX/HitFX_Aura_Shadow");
    private static SimplePool<Transform> m_GreyAura = new SimplePool<Transform>("SpellFX/HitFX_Aura_Grey");
    private static SimplePool<Transform> m_WhiteAura = new SimplePool<Transform>("SpellFX/HitFX_Aura_White");

    private static SimplePool<Transform> m_CastingShadow = new SimplePool<Transform>("SpellFX/CastingAura_Shadow");
    private static SimplePool<Transform> m_CastingGrey = new SimplePool<Transform>("SpellFX/CastingAura_Grey");
    private static SimplePool<Transform> m_CastingWhite = new SimplePool<Transform>("SpellFX/CastingAura_White");

    private static SimplePool<Transform> m_BackfireAura = new SimplePool<Transform>("SpellFX/CastingAura_Backfire");
    private static SimplePool<Transform> m_BackfireGlyph = new SimplePool<Transform>("SpellFX/SpellGlyph_Backfire");
        
    private static SimplePool<TextMeshPro> m_TextPopupPool = new SimplePool<TextMeshPro>("SpellFX/TextPopup");

    private static Dictionary<IMarker, Transform> m_CastingAuraDict = new Dictionary<IMarker, Transform>();
    public static void SpawnCastingAura(IMarker caster, int degree)
    {
        Transform aura;

        if (degree < 0)
            aura = m_CastingShadow.Spawn();
        else if (degree > 0)
            aura = m_CastingWhite.Spawn();
        else
            aura = m_CastingGrey.Spawn();

        //remove previous instance
        DespawnCastingAura(caster);
        m_CastingAuraDict.Add(caster, aura);

        aura.localScale = Vector3.zero;
        aura.position = caster.gameObject.transform.position;
        LeanTween.scale(aura.gameObject, Vector3.one, 0.8f)
            .setEaseOutCubic();
    }
    public static void DespawnCastingAura(IMarker marker)
    {
        if (m_CastingAuraDict.ContainsKey(marker))
        {
            Transform aura = m_CastingAuraDict[marker];
            m_CastingAuraDict.Remove(marker);
            LeanTween.scale(aura.gameObject, Vector3.zero, 1f)
                .setEaseOutCubic()
                .setOnComplete(() =>
                {
                    m_CastingShadow.Despawn(aura);
                    m_CastingGrey.Despawn(aura);
                    m_CastingWhite.Despawn(aura);
                });
        }
    }

    public static void SpawnBackfire(IMarker target, int damage, float delay)
    {
        LeanTween.value(0, 1, 0).setDelay(delay).setOnStart(() =>
        {
            Transform glyph = m_BackfireGlyph.Spawn();
            glyph.GetChild(5).GetComponent<TextMeshProUGUI>().text = damage.ToString();
            glyph.rotation = target.characterTransform.rotation;
            glyph.position = target.gameObject.transform.position + glyph.transform.up * 16.7f;
            glyph.SetParent(target.characterTransform);

            Transform aura = m_BackfireAura.Spawn();
            aura.position = target.characterTransform.position;
            aura.SetParent(target.gameObject.transform);

            LeanTween.value(0, 1, 0).setOnStart(() =>
            {
                m_BackfireAura.Despawn(aura);
                m_BackfireGlyph.Despawn(glyph);
            }).setDelay(3f);
        });
    }

    public static void SpawnFail(IMarker target, float delay)
    {
        LeanTween.value(0, 1, 0).setDelay(delay).setOnStart(() =>
        {
            Transform aura = m_BackfireAura.Spawn();
            aura.position = target.characterTransform.position;
            aura.SetParent(target.gameObject.transform);

            LeanTween.value(0, 1, 0).setOnStart(() =>
            {
                m_BackfireAura.Despawn(aura);

            }).setDelay(3f);

            SpawnText(target, "Spell failed!");
        });
    }
    
    public static void SpawnGlyph(IMarker target, SpellDict spell, string baseSpell)
    {
        Token token = target.customData as Token;
        SimplePool<Transform> glyphPool;
        SimplePool<Transform> auraPool;

        if (spell.spellSchool < 0)
        {
            auraPool = m_ShadowAura;
            glyphPool = m_ShadowGlyph;
        }
        else if (spell.spellSchool > 0)
        {
            auraPool = m_WhiteAura;
            glyphPool = m_WhiteGlyph;
        }
        else
        {
            auraPool = m_GreyAura;
            glyphPool = m_GreyGlyph;
        }

        Transform aura = auraPool.Spawn();
        Transform glyph = glyphPool.Spawn();

        LeanTween.value(0, 1, 0).setOnStart(() =>
        {
            glyphPool.Despawn(glyph);
            auraPool.Despawn(aura);
        }).setDelay(3f);

        aura.position = target.gameObject.transform.position;
        glyph.rotation = target.characterTransform.rotation;
        glyph.position = target.gameObject.transform.position + glyph.transform.up * 16.7f;
        glyph.GetChild(5).GetComponent<TextMeshProUGUI>().text = spell.spellName;

        if (string.IsNullOrEmpty(baseSpell))
            baseSpell = spell.spellID;
        DownloadedAssets.GetSprite(baseSpell, (spr) => { glyph.GetChild(4).GetComponent<UnityEngine.UI.Image>().sprite = spr; });

        aura.gameObject.SetActive(true);
        glyph.gameObject.SetActive(true);
    }

    public static void SpawnDamage(IMarker target, int amount, string color = null)
    {
        if (amount == 0)
            return;

        if (color == null)
            color = "#ffffff";

        Token token = target.customData as Token;
        SpawnText(target, $"<color={color}>{amount.ToString("+#;-#")}</color> Energy");
    }

    public static void SpawnText(IMarker target, string text)
    {
        TextMeshPro textObject = m_TextPopupPool.Spawn(target.characterTransform);
        textObject.transform.localRotation = Quaternion.identity;
        textObject.text = text;
        Vector2 pos = new Vector2();

        LeanTween.value(0, 1, 2f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                textObject.alpha = (1 - t) * 1.25f;
                pos.y = 30 + t * 10;
                textObject.transform.localPosition = pos;
            })
            .setOnComplete(() =>
            {
                m_TextPopupPool.Despawn(textObject);
            });
    }

    public static void DelayedFeedback(float delay, IMarker target, SpellDict spell, string baseSpell, int damage, string textColor = null)
    {
        LeanTween.value(0, 1, 0)
            .setOnStart(
            () =>
            {
                SpawnGlyph(target, spell, baseSpell);
                SpawnDamage(target, damage);
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

            if (data.result.effect == "success")
            {
                //focus on the target when the spell is succesfully cast
                StreetMapUtils.FocusOnTarget(target, 9);

                //spawn the spell glyph and aura
                DelayedFeedback(0.6f, target, spell, data.baseSpell, data.result.total);

                //shake slightly if being healed
                if(data.result.total > 0) //healed
                {
                    StreetMapUtils.ShakeCamera(
                        new Vector3(1, -5, 1),
                        0.05f,
                        0.6f,
                        2f
                    );
                }
                //shake more if taking damage
                else if (data.result.total < 0) //dealt damage
                {
                    StreetMapUtils.ShakeCamera(
                        new Vector3(1, -5, 5),
                        0.2f,
                        0.3f,
                        1f
                    );
                }

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

                SpawnBackfire(PlayerManager.marker, damage, 0);

                //shake a little more than normal on backfire
                StreetMapUtils.ShakeCamera(
                    new Vector3(1, -5, 5),
                    0.3f,
                    0.3f,
                    1f
                );
            }
            else if (data.result.effect == "fail")
            {
                SpawnFail(PlayerManager.marker, 0);

                StreetMapUtils.ShakeCamera(
                    new Vector3(1, -5, 5),
                    0.1f,
                    0.3f,
                    1f
                );
            }

            //update the UI
            //waiting 2seconds while the spell is animated
            UIWaitingCastResult.Instance.Close(2f, () =>
            {
                DespawnCastingAura(PlayerManager.marker);
                //if failed, dont close the spellcasting UI so the player can retry casting
                if (data.result.effect != "fail" && data.result.effect != "backfire")
                {
                    UISpellcasting.Instance.FinishSpellcastingFlow();
                }
            });

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
