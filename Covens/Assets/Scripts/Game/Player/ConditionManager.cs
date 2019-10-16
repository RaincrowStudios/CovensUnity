using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Raincrow.GameEventResponses;
using Raincrow.Maps;

public static class PlayerConditionManager
{
    public static System.Action<StatusEffect> OnPlayerApplyStatusEffect;
    public static System.Action<StatusEffect> OnPlayerExpireStatusEffect;

    private static Dictionary<string, System.Action<StatusEffect, IMarker>> m_StatusBehavior = new Dictionary<string, System.Action<StatusEffect, IMarker>>()
    {
        { SpellData.BOUND_STATUS,      BanishManager.Bind },
        { SpellData.SILENCED_STATUS,   BanishManager.Silence },
        //{ "channeling", SpellChanneling.SpawnPlayerFX },
        //{ "channeled",  SpellChanneling.SpawnFX }
    };

    private static Dictionary<string, System.Action<StatusEffect>> m_StatusExpireBehavior = new Dictionary<string, System.Action<StatusEffect>>()
    {
        { SpellData.BOUND_STATUS,          BanishManager.Unbind },
        { SpellData.SILENCED_STATUS,       BanishManager.Unsilence },
        //{ "channeling",     SpellChanneling.DespawnPlayerFX }
    };

    private static Dictionary<string, double> m_StatusDict = new Dictionary<string, double>();

    public static void OnApplyEffect(StatusEffect statusEffect, IMarker caster)
    {
        string debug = statusEffect.spell + " added";

        if (statusEffect.modifiers.status != null)
        {
            debug += "\n\t";
            foreach (string status in statusEffect.modifiers.status)
                debug += status + " ";
        }

        if (statusEffect.modifiers.status != null)
        {
            debug += "\n\t";
            foreach (string status in statusEffect.modifiers.status)
            {
                if (m_StatusBehavior.ContainsKey(status))
                {
                    debug += status + " triggered\n\t";
                    m_StatusBehavior[status].Invoke(statusEffect, caster);
                }
            }
        }

        Log(debug);
        
        OnPlayerApplyStatusEffect?.Invoke(statusEffect);
    }

    public static void ExpireEffect(string spell)
    {
        var effects = PlayerManager.witchMarker.witchToken.effects;
        foreach (StatusEffect item in effects)
        {
            if (item.spell == spell)
            {
                ExpireStatusEffectHandler.ExpireEffect(PlayerDataManager.playerData.instance, item);
                return;
            }
        }
    }

    public static void OnExpireEffect(StatusEffect statusEffect)
    {
        string debug = statusEffect.spell + " expired:\n";
        if (statusEffect.modifiers.status != null)
        {
            foreach (string s in statusEffect.modifiers.status)
            {
                debug += "\t" + s + "\n";
                if (m_StatusExpireBehavior.ContainsKey(s))
                    m_StatusExpireBehavior[s].Invoke(statusEffect);
            }
        }

        Log(debug);
        
        OnPlayerExpireStatusEffect?.Invoke(statusEffect);
    }

    private static void Log(string msg)
    {
        Debug.Log("[<color=cyan>ConditionManager</color>] " + msg);
    }
}
