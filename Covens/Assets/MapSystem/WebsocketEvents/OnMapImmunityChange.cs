using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OnMapImmunityChange
{
    public static System.Action<string, string, bool> OnImmunityChange;

    private static SimplePool<Transform> m_ImmunityShieldPool = new SimplePool<Transform>("SpellFX/ImmunityShield");
    private static SimplePool<Transform> m_ImmunityAuraPool = new SimplePool<Transform>("SpellFX/ImmunityAura");
        
    public static void AddImmunityFX(IMarker target)
    {
        if (target == null)
            return;

        Token token = target.customData as Token;

        if (token.Type != MarkerSpawner.MarkerType.witch)
            return;
        
        target.SetCharacterAlpha(0.38f, 1f);
    }

    public static void RemoveImmunityFX(IMarker target)
    {
        if (target == null)
            return;

        Token token = target.customData as Token;

        if (token.Type != MarkerSpawner.MarkerType.witch)
            return;

        target.SetCharacterAlpha(1f, 1f);
    }


    public static void OnAddImmunity(WSData data)
    {
        MarkerDataDetail player = PlayerDataManager.playerData;

        MarkerSpawner.AddImmunity(data.immunity, data.instance);
        OnImmunityChange?.Invoke(data.immunity, data.instance, true);

        if (data.immunity == player.instance)
        {
            //add the fx if the witch is now immune to me
            AddImmunityFX(MarkerManager.GetMarker(data.instance));

            return;
        }
    }

    public static void OnRemoveImmunity(WSData data)
    {
        MarkerDataDetail player = PlayerDataManager.playerData;

        MarkerSpawner.RemoveImmunity(data.immunity, data.instance);
        OnImmunityChange?.Invoke(data.immunity, data.instance, false);

        if (data.immunity == player.instance)
        {
            //remove the fx if the witch was immune to me
            RemoveImmunityFX(MarkerManager.GetMarker(data.instance));

            return;
        }
    }
}
