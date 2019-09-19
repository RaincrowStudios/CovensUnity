using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Raincrow.GameEventResponses;
using Raincrow.Maps;

public static class ConditionManager
{
    public static System.Action<StatusEffect> OnPlayerApplyStatusEffect;
    public static System.Action<StatusEffect> OnPlayerExpireStatusEffect;

    private static Dictionary<string, System.Action<StatusEffect, IMarker>> m_StatusBehavior = new Dictionary<string, System.Action<StatusEffect, IMarker>>()
    {
        { "bound",      BanishManager.Bind },
        { "silenced",   BanishManager.Silence },
        { "channeling", SpellChanneling.SpawnFX },
        { "channeled",     SpellChanneling.DespawnFX }
    };

    private static Dictionary<string, System.Action<StatusEffect, IMarker>> m_StatusExpireBehavior = new Dictionary<string, System.Action<StatusEffect, IMarker>>()
    {
        { "bound",          BanishManager.Unbind },
        { "silenced",       BanishManager.Unsilence },
    };

    private static Dictionary<string, double> m_StatusDict = new Dictionary<string, double>();

    public static void AddCondition(StatusEffect statusEffect, IMarker caster)
    {
        //remove old condition matching the same spell
        foreach (StatusEffect item in PlayerDataManager.playerData.effects)
        {
            if (item.spell == statusEffect.spell)
            {
                PlayerDataManager.playerData.effects.Remove(item);
                item.CancelExpiration();
                break;
            }
        }
        PlayerDataManager.playerData.effects.Add(statusEffect);
        OnPlayerApplyStatusEffect?.Invoke(statusEffect);

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

        //schedule expiration
        statusEffect.ScheduleExpiration(() => ExpireStatusEffect(statusEffect, caster));

        //StatusEffectFX.SpawnFX(PlayerManager.marker, statusEffect);
    }

    public static void TriggerStatusEffect(StatusEffect effect, IMarker caster)
    {
        
    }

    public static void ExpireStatusEffect(StatusEffect statusEffect, IMarker caster)
    {
        foreach (StatusEffect item in PlayerDataManager.playerData.effects)
        {
            if (item.spell == statusEffect.spell)
            {
                PlayerDataManager.playerData.effects.Remove(item);
                break;
            }
        }

        string debug = statusEffect.spell + " expired:\n";
        if (statusEffect.modifiers.status != null)
        {
            foreach (string s in statusEffect.modifiers.status)
            {
                debug += "\t" + s + "\n";
                if (m_StatusExpireBehavior.ContainsKey(s))
                    m_StatusExpireBehavior[s].Invoke(statusEffect, caster);
            }
        }

        Log(debug);
        
        OnPlayerExpireStatusEffect?.Invoke(statusEffect);

        //StatusEffectFX.DespawnFX(PlayerManager.marker, statusEffect);
    }

    private static void Log(string msg)
    {
        Debug.Log("[<color=cyan>ConditionManager</color>] " + msg);
    }
}
