using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using TMPro;
using System.Collections.Generic;

public static class SpellcastingFX
{
    //generic
    private static SimplePool<Transform> m_BackfireAura = new SimplePool<Transform>("SpellFX/HitFX_Aura_Backfire");
    private static SimplePool<Transform> m_TextPopupPool = new SimplePool<Transform>("SpellFX/TextPopup");
    //public static SimplePool<Transform> DeathIconPool = new SimplePool<Transform>("SpellFX/DeathIcon");
    //public static SimplePool<Transform> ImmunityIconPool = new SimplePool<Transform>("SpellFX/ImmunityIcon");

    //spell damage ticks
    public static SimplePool<Transform> m_TickFxPool = new SimplePool<Transform>("SpellFX/SpellTickDamage");

    //banish
    private static SimplePool<Transform> m_BanishGlyph = new SimplePool<Transform>("SpellFX/SpellGlyph_Banish");
    private static SimplePool<Transform> m_BanishAura = new SimplePool<Transform>("SpellFX/HitFX_Aura_White");


    //
    private static List<int> m_TextTweenIds = new List<int>();
    private static bool m_PauseTween;

    public static void SpawnBanish(IMarker target)
    {
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
        SpawnText(target, f, 1);
    }

    public static void SpawnEnergyChange(IMarker target, int amount, float scale)
    {
        if (amount == 0)
            return;

        string color;
        if (amount > 0)
            color = "#ffffff";
        else
            color = "#FF0000";

        SpawnText(
            target,
            $"<color={color}>{amount.ToString("+#;-#")}</color>",
            scale
        );
    }

    public static void SpawnText(IMarker target, string text, float scale)
    {
        if (m_PauseTween) return;
        m_TextTweenIds.Clear();
        TextMeshPro textObj = m_TextPopupPool.Spawn(null, 3f).GetComponent<TextMeshPro>();
        textObj.text = text;
        textObj.fontSize = 45 * scale;
        textObj.transform.localScale = target.AvatarTransform.lossyScale;
        textObj.transform.rotation = target.AvatarRenderer.transform.rotation;

        if (LocationIslandController.isInBattle)
            textObj.transform.Translate(Random.Range(-7, -25), 61, 0);

        //animate the text
        textObj.transform.position = new Vector3(target.AvatarTransform.position.x, target.AvatarTransform.position.y + 42, target.AvatarTransform.position.z);
        var RandomSpacing = new Vector3(Random.Range(-7, 7), Random.Range(20, 24), 0);
        textObj.transform.Translate(RandomSpacing);
        m_TextTweenIds.Add(LeanTween.moveLocalY(textObj.gameObject, textObj.transform.localPosition.y + Random.Range(8, 11), 2f).setEaseOutCubic().id);
        m_TextTweenIds.Add(LeanTween.value(1f, 0f, 2f).setOnUpdate((float a) =>
       {
           if (textObj != null)
               textObj.alpha = a;
       }).id);
    }

    public static void StopTweening()
    {
        try
        {
            m_TextPopupPool.DespawnAll();
        }
        catch (System.Exception)
        {

        }
        m_PauseTween = true;
        foreach (var item in m_TextTweenIds)
        {
            LeanTween.cancel(item);
        }
        m_TextTweenIds.Clear();
    }

    public static void ResumeTweening()
    {
        m_PauseTween = false;
    }
}
