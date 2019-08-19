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

    public static SimplePool<Transform> m_TickFxPool = new SimplePool<Transform>("SpellFX/SpellTickDamage");

    public static SimplePool<Transform> DeathIconPool = new SimplePool<Transform>("SpellFX/DeathIcon");
    public static SimplePool<Transform> ImmunityIconPool = new SimplePool<Transform>("SpellFX/ImmunityIcon");

    private static bool m_QueueGlyphs = false;

    //public static void SpawnBackfire(IMarker target, int damage, float delay, bool shake = true)
    //{
    //    LeanTween.value(0, 1, delay).setOnComplete(() =>
    //    {
    //        target.SpawnFX(m_BackfireGlyph, true, 3f, m_QueueGlyphs, (glyph) =>
    //        {
    //            glyph.GetChild(5).GetComponent<TextMeshProUGUI>().text = damage.ToString();
    //            glyph.position = target.gameObject.transform.position + glyph.transform.up * 21.7f - target.characterTransform.forward;
    //        });

    //        target.SpawnFX(m_BackfireAura, false, 3f, m_QueueGlyphs, null);

    //        if (shake)
    //        {
    //            //shake a little more than normal on backfire
    //            MapCameraUtils.ShakeCamera(
    //                new Vector3(1, -5, 5),
    //                0.3f,
    //                0.3f,
    //                1f
    //            );
    //        }
    //    });
    //}

    public static void SpawnBanish(IMarker target)
    {
        //LeanTween.value(0, 1, delay).setOnComplete(() =>
        //{
        Transform glyph = m_BanishGlyph.Spawn();
        Transform aura = m_BanishAura.Spawn();

        glyph.localScale = target.AvatarTransform.lossyScale;
        glyph.rotation = target.AvatarTransform.rotation;
        glyph.position = target.GameObject.transform.position + target.AvatarTransform.up * 37.30935f - target.AvatarTransform.forward;

        aura.position = target.GameObject.transform.position;
        aura.localScale = target.GameObject.transform.lossyScale;

        glyph.gameObject.SetActive(true);
        aura.gameObject.SetActive(true);

        LeanTween.value(0, 0, 3f).setOnComplete(() =>
        {
            m_BanishGlyph.Despawn(glyph);
            m_BanishAura.Despawn(aura);
        });
        //});
    }

    public static void SpawnFail(IMarker target, bool shake = true)
    {
        Transform aura = m_BackfireAura.Spawn(target.GameObject.transform, 3f);
        aura.localPosition = new Vector3(0, 0, 0f);
        aura.localScale = Vector3.one;
        aura.localRotation = Quaternion.identity;

        if (shake)
        {
            MapCameraUtils.ShakeCamera(
                new Vector3(1, -5, 5),
                0.1f,
                0.3f,
                1f
            );
        }
        var f = LocalizeLookUp.GetText("spell_fail");
        SpawnText(target, f, false, m_QueueGlyphs);
    }

    public static void SpawnGlyph(IMarker target, SpellData spell, string baseSpell)
    {
        if (string.IsNullOrEmpty(baseSpell))
            baseSpell = spell.id;

        string[] array = { "spell_bind", "spell_silence", "spell_seal", "spell_invisibility", "spell_dispel", "spell_clarity", "spell_sealBalance", "spell_sealLight", "spell_sealShadow", "spell_reflectiveWard", "spell_rageWard", "spell_greaterSeal", "spell_greaterDispel", "spell_banish", "spell_mirrors", "spell_trueSight", "spell_crowsEye", "spell_shadowMark", "spell_channeling", "spell_transquility", "spell_addledMind", "spell_whiteRain" }; //list of all nondamaging spells to display in oblique text
        if (array.Contains(spell.id))
        {
            var s = LocalizeLookUp.GetSpellName(spell.id);

            LeanTween.value(0f, 1f, 0.25f).setOnComplete(() => //slight delay
              {
                  SpawnText(
                      target,
                      s,
                      false,
                      m_QueueGlyphs
                  );
              });
        }
        /* Token token = target.Token;
        SimplePool<Transform> glyphPool;
        SimplePool<Transform> auraPool;

        if (spell.school < 0)
        {
            auraPool = m_ShadowAura;
            glyphPool = m_ShadowGlyph;
        }
        else if (spell.school > 0)
        {
            auraPool = m_WhiteAura;
            glyphPool = m_WhiteGlyph;
        }
        else
        {
            auraPool = m_GreyAura;
            glyphPool = m_GreyGlyph;
        }

        //spawn n setup glyph
        Transform glyph = glyphPool.Spawn(target.AvatarTransform, 3f);
        glyph.localScale = Vector3.one;
        glyph.localRotation = Quaternion.identity;
        glyph.position = target.AvatarTransform.position + new Vector3(0, 0, -0.5f) + glyph.up * 40.7f;
        glyph.GetChild(0).GetChild(6).GetComponent<TextMeshProUGUI>().text = LocalizeLookUp.GetSpellName(spell.id);

        if (string.IsNullOrEmpty(baseSpell))
            baseSpell = spell.id;
        DownloadedAssets.GetSprite(baseSpell, (spr) => { glyph.GetChild(0).GetChild(5).GetComponent<UnityEngine.UI.Image>().overrideSprite = spr; });

        //spawn aura
        Transform aura = auraPool.Spawn(target.GameObject.transform, 3f);
        aura.localPosition = new Vector3(0, 0, 0f);
        aura.localScale = Vector3.one;
        aura.localRotation = Quaternion.identity;
        */
    }

    public static void SpawnDamage(IMarker target, int amount, bool crit, string color = null)
    {
        if (amount == 0)
            return;

        if (color == null)
            color = "#ffffff";

        SpawnText(
            target,
            amount.ToString("+#;-#"),
            //LocalizeLookUp.GetText("moon_energy").Replace("{{Amount}}", "<color=" + color + ">" + amount.ToString("+#;-#") + "</color>"),
            crit,
            m_QueueGlyphs
        );//$"<color={color}>{amount.ToString("+#;-#")}</color> Energy");
    }

    public static void SpawnText(IMarker target, string text, bool crit, bool queued)
    {
        TextMeshPro textObj = m_TextPopupPool.Spawn(target.AvatarTransform, 3f).GetComponent<TextMeshPro>();
        textObj.text = text;
        textObj.fontSize = 45;
        textObj.transform.localScale = Vector3.one;
        textObj.transform.localRotation = Quaternion.identity;

        if (LocationIslandController.isInBattle)
            textObj.transform.Translate(Random.Range(-7, -25), 61, 0);
        if (text.Contains("-") == true)
        {
            textObj.color = Utilities.Red; //make it red for damage
        }
        if (crit == true)
        {
            textObj.fontSize = 65; //big text for crit
        }
        //animate the text
        textObj.transform.position = new Vector3(target.AvatarTransform.position.x, target.AvatarTransform.position.y + 22, target.AvatarTransform.position.z);
        var RandomSpacing = new Vector3(Random.Range(-7, 7), textObj.transform.localPosition.y + Random.Range(20, 24), textObj.transform.localPosition.z);
        textObj.transform.Translate(RandomSpacing);
        LeanTween.moveLocalY(textObj.gameObject, textObj.transform.localPosition.y + Random.Range(8, 11), 2f).setEaseOutCubic();
        LeanTween.value(1f, 0f, 2f).setOnUpdate((float a) =>
        {
            textObj.alpha = a;
        });
        /*Vector3 pos;
        LeanTween.value(0, 1, 2f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                textObj.alpha = (1 - t) * 2f;
                pos = textObj.transform.up * (40 + t * 10);
                textObj.transform.position = target.AvatarTransform.position + pos;
            });*/
    }
}
