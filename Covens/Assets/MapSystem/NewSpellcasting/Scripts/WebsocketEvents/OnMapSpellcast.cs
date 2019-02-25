using Newtonsoft.Json;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class OnMapSpellcast
{
    private static GameObject m_ShadowGlyph;
    private static GameObject m_GreyGlyph;
    private static GameObject m_WhiteGlyph;

    private static GameObject m_ShadowAura;
    private static GameObject m_GreyAura;
    private static GameObject m_WhiteAura;

    public static GameObject shadowGlyph
    {
        get
        {
            if (m_ShadowGlyph == null) m_ShadowGlyph = GameObject.Instantiate(Resources.Load<GameObject>("SpellFX/SpellGlyph_Shadow"));
            return m_ShadowGlyph;
        }
    }
    public static GameObject greyGlyph
    {
        get
        {
            if (m_GreyGlyph == null) m_GreyGlyph = GameObject.Instantiate(Resources.Load<GameObject>("SpellFX/SpellGlyph_Grey"));
            return m_GreyGlyph;
        }
    }
    public static GameObject whiteGlyph
    {
        get
        {
            if (m_WhiteGlyph == null) m_WhiteGlyph = GameObject.Instantiate(Resources.Load<GameObject>("SpellFX/SpellGlyph_White"));
            return m_WhiteGlyph;
        }
    }

    public static GameObject shadowAura
    {
        get
        {
            if (m_ShadowAura == null) m_ShadowAura = GameObject.Instantiate(Resources.Load<GameObject>("SpellFX/HitFX_Aura_Shadow"));
            return m_ShadowAura;
        }
    }
    public static GameObject greyAura
    {
        get
        {
            if (m_GreyAura == null) m_GreyAura = GameObject.Instantiate(Resources.Load<GameObject>("SpellFX/HitFX_Aura_Grey"));
            return m_GreyAura;
        }
    }
    public static GameObject whiteAura
    {
        get
        {
            if (m_WhiteAura == null) m_WhiteAura = GameObject.Instantiate(Resources.Load<GameObject>("SpellFX/HitFX_Aura_White"));
            return m_WhiteAura;
        }
    }

    private static SimplePool<TextMeshPro> m_TextPopupPool = new SimplePool<TextMeshPro>("SpellFX/TextPopup");

    
    public static void SpawnGlyph(IMarker target, SpellDict spell, string baseSpell)
    {
        Token token = target.customData as Token;
        GameObject aura;
        GameObject glyph;

        if (spell.spellSchool < 0)
        {
            aura = shadowAura;
            glyph = shadowGlyph;
        }
        else if (spell.spellSchool > 0)
        {
            aura = whiteAura;
            glyph = whiteGlyph;
        }
        else
        {
            aura = greyAura;
            glyph = greyGlyph;
        }
        
        aura.transform.position = target.gameObject.transform.position;
        glyph.transform.rotation = target.characterTransform.rotation;
        glyph.transform.position = target.gameObject.transform.position + glyph.transform.up * 16.7f;
        glyph.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = spell.spellName;

        if (string.IsNullOrEmpty(baseSpell))
            baseSpell = spell.spellID;
        DownloadedAssets.GetSprite(baseSpell, (spr) => { glyph.transform.GetChild(4).GetComponent<UnityEngine.UI.Image>().sprite = spr; });

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
        TextMeshPro textObject = m_TextPopupPool.Spawn(target.characterTransform);
        textObject.transform.localRotation = Quaternion.identity;
        textObject.text = $"<color={color}>{amount.ToString("+#;-#")}</color> Energy";
        Vector2 pos = new Vector2();

        LeanTween.value(0, 1, 2f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                textObject.alpha = 1 - t;
                pos.y = 30 + t * 15;
                textObject.transform.localPosition = pos;
            })
            .setOnComplete(() =>
            {
                m_TextPopupPool.Despawn(textObject);
            });
    }




    public static void HandleEvent(WSData data)
    {
        MarkerDataDetail player = PlayerDataManager.playerData;

        if (data.casterInstance == player.instance) //I am the caster
        {
            if (data.target == "portal")
                return;

            IMarker target = MarkerManager.GetMarker(data.targetInstance);
            SpellDict spell = DownloadedAssets.spellDictData[data.spell];

            if (target == null)
            {
                Debug.LogError("NULL TARGET? " + data.targetInstance);
                return;
            }

            if (data.result.effect == "success")
            {
                SpawnGlyph(target, spell, data.baseSpell);
                SpawnDamage(target, data.result.total);
                if(data.result.total > 0) //healed
                {
                    StreetMapUtils.ShakeCamera(
                        new Vector3(1, -5, 1),
                        0.05f,
                        0.6f,
                        2f
                    );
                }
                else if (data.result.total < 0) //dealt damage
                {
                    StreetMapUtils.ShakeCamera(
                        new Vector3(1, -5, 5),
                        0.2f,
                        0.3f,
                        1f
                    );
                }
            }

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
