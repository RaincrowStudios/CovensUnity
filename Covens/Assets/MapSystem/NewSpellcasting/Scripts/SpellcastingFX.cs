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

    private static SimplePool<TextMeshPro> m_TextPopupPool = new SimplePool<TextMeshPro>("SpellFX/TextPopup");

    private static SimplePool<Transform> m_DeadIconPool = new SimplePool<Transform>("SpellFX/DeathIcon");

    //private static Dictionary<IMarker, Transform> m_CastingAuraDict = new Dictionary<IMarker, Transform>();
    //public static void SpawnCastingAura(IMarker caster, int degree)
    //{
    //    Transform aura;

    //    if (degree < 0)
    //        aura = m_ShadowAura.Spawn();
    //    else if (degree > 0)
    //        aura = m_WhiteAura.Spawn();
    //    else
    //        aura = m_GreyAura.Spawn();

    //    //remove previous instance
    //    DespawnCastingAura(caster);
    //    m_CastingAuraDict.Add(caster, aura);

    //    aura.position = caster.gameObject.transform.position;
    //}
    //public static void DespawnCastingAura(IMarker marker)
    //{
    //    if (m_CastingAuraDict.ContainsKey(marker))
    //    {
    //        Transform aura = m_CastingAuraDict[marker];
    //        m_CastingAuraDict.Remove(marker);
    //        LeanTween.scale(aura.gameObject, Vector3.zero, 1f)
    //            .setEaseOutCubic()
    //            .setOnComplete(() =>
    //            {
    //                m_ShadowAura.Despawn(aura);
    //                m_GreyAura.Despawn(aura);
    //                m_ShadowAura.Despawn(aura);
    //            });
    //    }
    //}

    private static Dictionary<string, Transform> m_DeathIcons = new Dictionary<string, Transform>();

    public static void SpawnDeathFX(string instance, IMarker marker)
    {
        if (m_DeathIcons.ContainsKey(instance))
            return;

        Transform icon = m_DeadIconPool.Spawn();
        icon.position = marker.characterTransform.position;
        //icon.SetParent(marker.characterTransform);
        //icon.localScale = Vector3.one * 1.4f;
        //icon.localPosition = Vector3.zero;

        m_DeathIcons.Add(instance, icon);
        marker.SetAlpha(0.45f);
    }

    public static void DespawnDeathFX(string instance, IMarker marker)
    {
        if (!m_DeathIcons.ContainsKey(instance))
            return;

        Transform icon = m_DeathIcons[instance];
        m_DeathIcons.Remove(instance);
        m_DeadIconPool.Despawn(icon);
        marker.SetAlpha(1f);
    }

    public static void SpawnBackfire(IMarker target, int damage, float delay, bool shake = true)
    {
        LeanTween.value(0, 1, 0).setDelay(delay).setOnStart(() =>
        {
            Transform glyph = m_BackfireGlyph.Spawn();
            glyph.GetChild(5).GetComponent<TextMeshProUGUI>().text = damage.ToString();
            glyph.rotation = target.characterTransform.rotation;
            glyph.position = target.gameObject.transform.position + glyph.transform.up * 21.7f;
            //glyph.SetParent(target.characterTransform);

            Transform aura = m_BackfireAura.Spawn();
            aura.position = target.characterTransform.position;
            //aura.SetParent(target.gameObject.transform);

            if (shake)
            {
                //shake a little more than normal on backfire
                StreetMapUtils.ShakeCamera(
                    new Vector3(1, -5, 5),
                    0.3f,
                    0.3f,
                    1f
                );
            }

            LeanTween.value(0, 1, 0).setOnStart(() =>
            {
                m_BackfireAura.Despawn(aura);
                m_BackfireGlyph.Despawn(glyph);
            }).setDelay(3f);
        });
    }

    public static void SpawnBanish(IMarker target, float delay)
    {
        LeanTween.value(0, 1, 0).setDelay(delay).setOnStart(() =>
        {
            Transform glyph = m_BanishGlyph.Spawn();
            glyph.rotation = target.characterTransform.rotation;
            glyph.position = target.gameObject.transform.position + glyph.transform.up * 21.7f;
            //glyph.SetParent(target.characterTransform);

            Transform aura = m_BanishAura.Spawn();
            aura.position = target.characterTransform.position;
            //aura.SetParent(target.gameObject.transform);

            LeanTween.value(0, 1, 0).setOnStart(() =>
            {
                m_BanishAura.Despawn(aura);
                m_BanishGlyph.Despawn(glyph);
            }).setDelay(3f);
        });
    }

    public static void SpawnEscaped(IMarker target, float delay)
    {
        LeanTween.value(0, 1, 0).setDelay(delay).setOnStart(() =>
        {
            Transform glyph = m_EscapedGlyph.Spawn();
            glyph.rotation = target.characterTransform.rotation;
            glyph.position = target.gameObject.transform.position + glyph.transform.up * 21.7f;
            //glyph.SetParent(target.characterTransform);

            Transform aura = m_BanishAura.Spawn();
            aura.position = target.characterTransform.position;
            //aura.SetParent(target.gameObject.transform);
            
            LeanTween.value(0, 1, 0).setOnStart(() =>
            {
                m_BanishAura.Despawn(aura);
                m_EscapedGlyph.Despawn(glyph);
            }).setDelay(3f);
        });
    }

    public static void SpawnFail(IMarker target, float delay, bool shake = true)
    {
        LeanTween.value(0, 1, 0).setDelay(delay).setOnStart(() =>
        {
            Transform aura = m_BackfireAura.Spawn();
            aura.position = target.characterTransform.position;
            //aura.SetParent(target.gameObject.transform);

            if (shake)
            {
                StreetMapUtils.ShakeCamera(
                    new Vector3(1, -5, 5),
                    0.1f,
                    0.3f,
                    1f
                );
            }

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
        glyph.position = target.gameObject.transform.position + glyph.transform.up * 21.7f;
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
        TextMeshPro textObject = m_TextPopupPool.Spawn(null);
        textObject.transform.position = target.characterTransform.position;
        textObject.transform.rotation = target.characterTransform.rotation;
        //textObject.transform.localRotation = Quaternion.identity;

        textObject.text = text;
        Vector2 pos = new Vector2();

        LeanTween.value(0, 1, 2f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                textObject.alpha = (1 - t) * 2f;
                pos.y = 30 + t * 5;
                textObject.transform.localPosition = pos;
            })
            .setOnComplete(() =>
            {
                m_TextPopupPool.Despawn(textObject);
            });
    }
}
