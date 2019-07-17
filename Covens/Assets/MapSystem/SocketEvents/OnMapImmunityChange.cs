using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.GameEventResponses;

public static class OnMapImmunityChange
{
    public static System.Action<string, string, bool> OnImmunityChange;

    private static SimplePool<Transform> m_ImmunityShieldPool = new SimplePool<Transform>("SpellFX/ImmunityShield");
    private static SimplePool<Transform> m_ImmunityAuraPool = new SimplePool<Transform>("SpellFX/ImmunityAura");



    //public static void AddImmunityFX(IMarker target)
    //{
    //    if (target == null)
    //        return;

    //    Token token = target.customData as Token;

    //    if (token.Type != MarkerSpawner.MarkerType.witch)
    //        return;

    //    target.SetCharacterAlpha(0.38f, 1f);
    //    LeanTween.value(0f,1f,2.5f).setOnComplete(() => {
    //        var i = target.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
    //        LeanTween.alpha(i, 0f, 0.01f).setOnComplete(() => {
    //                i.SetActive(true);
    //            });
    //    LeanTween.alpha(i, 1f, 0.6f);
    //    });

    //}

    //public static void RemoveImmunityFX(IMarker target)
    //{
    //    if (target == null)
    //        return;

    //    Token token = target.customData as Token;

    //    if (token.Type != MarkerSpawner.MarkerType.witch)
    //        return;

    //    target.SetCharacterAlpha(1f, 1f);
    //    LeanTween.value(0f,1f,2.5f).setOnComplete(() => {
    //        var i = target.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
    //        LeanTween.alpha(i, 0f, 0.3f).setOnComplete(() => {
    //            i.SetActive(false);
    //        });
    //    });
    //}


    public static void OnAddImmunity(AddImmunityHandler.AddImmunityEventData data)
    {
        PlayerData player = PlayerDataManager.playerData;

        MarkerSpawner.AddImmunity(data.caster, data.target);
        OnImmunityChange?.Invoke(data.caster, data.target, true);
        if (data.caster == player.instance)
        {
            //add the fx if the witch is now immune to me
            IMarker marker = MarkerManager.GetMarker(data.target);
            if (marker != null && marker is WitchMarker)
                (marker as WitchMarker).AddImmunityFX();

            return;
        }
    }

    public static void OnRemoveImmunity(RemoveImmunityHandler.RemoveImmunityEventData data)
    {
        PlayerData player = PlayerDataManager.playerData;

        MarkerSpawner.RemoveImmunity(data.caster, data.target);
        OnImmunityChange?.Invoke(data.caster, data.target, false);

        if (data.caster == player.instance)
        {
            //remove the fx if the witch was immune to me
            IMarker marker = MarkerManager.GetMarker(data.target);
            if (marker != null && marker is WitchMarker)
                (marker as WitchMarker).RemoveImmunityFX();

            return;
        }
    }
}
