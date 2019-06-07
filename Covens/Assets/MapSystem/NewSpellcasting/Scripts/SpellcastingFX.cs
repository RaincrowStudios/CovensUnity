using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using TMPro;
using System.Collections.Generic;

public static class SpellcastingFX
{
    private static SimplePool<Transform> m_ShadowGlyph = new SimplePool<Transform>("SpellFX/SpellGlyph_Shadow");
    private static SimplePool<Transform> m_GreyGlyph = new SimplePool<Transform>("SpellFX/SpellGlyph_Grey");
    private static SimplePool<Transform> m_WhiteGlyph = new SimplePool<Transform>("SpellFX/SpellGlyph_White");
    private static SimplePool<Transform> m_BackfireGlyph = new SimplePool<Transform>("SpellFX/SpellGlyph_Backfire");
    private static SimplePool<Transform> m_BanishGlyph = new SimplePool<Transform>("SpellFX/SpellGlyph_Banish");
    private static SimplePool<Transform> m_EscapedGlyph = new SimplePool<Transform>("SpellFX/SpellGlyph_Escaped");

    private static SimplePool<Transform> m_ShadowAura = new SimplePool<Transform>("SpellFX/HitFX_Aura_Shadow");
    private static SimplePool<Transform> m_GreyAura = new SimplePool<Transform>("SpellFX/HitFX_Aura_Grey");
    private static SimplePool<Transform> m_WhiteAura = new SimplePool<Transform>("SpellFX/HitFX_Aura_White");
    private static SimplePool<Transform> m_BackfireAura = new SimplePool<Transform>("SpellFX/HitFX_Aura_Backfire");
    private static SimplePool<Transform> m_BanishAura = new SimplePool<Transform>("SpellFX/HitFX_Aura_White");

    private static SimplePool<Transform> m_TextPopupPool = new SimplePool<Transform>("SpellFX/TextPopup");

    private static SimplePool<Transform> m_DeadIconPool = new SimplePool<Transform>("SpellFX/DeathIcon");

    private static Dictionary<string, Transform> m_DeathIcons = new Dictionary<string, Transform>();

    public static void SpawnDeathFX(string instance, IMarker marker)
    {
        if (m_DeathIcons.ContainsKey(instance))
            return;

        Transform icon = m_DeadIconPool.Spawn();
        marker.AddChild(icon, marker.characterTransform, m_DeadIconPool);

        m_DeathIcons.Add(instance, icon);
        marker.SetCharacterAlpha(0.45f);
    }

    public static void DespawnDeathFX(string instance, IMarker marker)
    {
        if (!m_DeathIcons.ContainsKey(instance))
            return;

        Transform icon = m_DeathIcons[instance];
        m_DeathIcons.Remove(instance);
        marker.RemoveChild(icon);
        marker.SetCharacterAlpha(1f);
    }

    public static void SpawnBackfire(IMarker target, int damage, float delay, bool shake = true)
    {
        LeanTween.value(0, 1, delay).setOnComplete(() =>
        {
            target.SpawnFX(m_BackfireGlyph, true, 3f, true, (glyph) =>
            {
                glyph.GetChild(5).GetComponent<TextMeshProUGUI>().text = damage.ToString();
                glyph.position = target.gameObject.transform.position + glyph.transform.up * 21.7f - target.characterTransform.forward;
            });

            target.SpawnFX(m_BackfireAura, false, 3f, true, null);

            if (shake)
            {
                //shake a little more than normal on backfire
                MapCameraUtils.ShakeCamera(
                    new Vector3(1, -5, 5),
                    0.3f,
                    0.3f,
                    1f
                );
            }
        });
    }

    public static void SpawnBanish(IMarker target, float delay)
    {
        LeanTween.value(0, 1, delay).setOnComplete(() =>
        {
            Transform glyph = m_BanishGlyph.Spawn();
            Transform aura = m_BanishAura.Spawn();

            glyph.localScale = target.characterTransform.lossyScale;
            glyph.rotation = target.characterTransform.rotation;
            glyph.position = target.gameObject.transform.position + target.characterTransform.up * 37.30935f - target.characterTransform.forward;

            aura.position = target.gameObject.transform.position;
            aura.localScale = target.gameObject.transform.lossyScale;

            glyph.gameObject.SetActive(true);
            aura.gameObject.SetActive(true);

            LeanTween.value(0, 0, 3f).setOnComplete(() =>
            {
                m_BanishGlyph.Despawn(glyph);
                m_BanishAura.Despawn(aura);
            });
        });
    }

    public static void SpawnFail(IMarker target, float delay, bool shake = true)
    {
        LeanTween.value(0, 1, delay).setOnComplete(() =>
        {
            target.SpawnFX(m_BackfireAura, false, 3f, true, null);

            if (shake)
            {
                MapCameraUtils.ShakeCamera(
                    new Vector3(1, -5, 5),
                    0.1f,
                    0.3f,
                    1f
                );
            }

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

        target.SpawnFX(glyphPool, true, 3f, true, (glyph) =>
        {
            glyph.position = target.gameObject.transform.position + glyph.transform.up * 40.7f - target.characterTransform.forward;

            glyph.GetChild(0).GetChild(5).GetComponent<TextMeshProUGUI>().text = spell.spellName;

            if (string.IsNullOrEmpty(baseSpell))
                baseSpell = spell.spellID;
            DownloadedAssets.GetSprite(baseSpell, (spr) => { glyph.GetChild(0).GetChild(4).GetComponent<UnityEngine.UI.Image>().overrideSprite = spr; });
        });

        target.SpawnFX(auraPool, false, 3f, true, null);
    }

    public static void SpawnDamage(IMarker target, int amount, string color = null)
    {
        if (amount == 0)
            return;

        if (color == null)
            color = "#ffffff";

        SpawnText(target, LocalizeLookUp.GetText("moon_energy").Replace("{{Amount}}", "<color=" + color + ">" + amount.ToString("+#;-#") + "</color>"));//$"<color={color}>{amount.ToString("+#;-#")}</color> Energy");
    }

    public static void SpawnText(IMarker target, string text)
    {
        target.SpawnFX(m_TextPopupPool, true, 3f, false, (textTransform) =>
        {
            TextMeshPro textObject = textTransform.GetComponent<TextMeshPro>();
            textObject.text = text;

            Vector3 pos = textObject.transform.localPosition;

            LeanTween.value(0, 1, 2f)
                .setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    textObject.alpha = (1 - t) * 2f;
                    pos.y = 20 + t * 10;
                    textTransform.localPosition = pos;
                });
        });
    }
}
