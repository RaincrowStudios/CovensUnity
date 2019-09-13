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
        { "bound",     BanishManager.Bind },
        { "silenced",  BanishManager.Silence }
    };

    private static Dictionary<string, System.Action<StatusEffect, IMarker>> m_StatusExpireBehavior = new Dictionary<string, System.Action<StatusEffect, IMarker>>()
    {
        { "bound",     BanishManager.Unbind },
        { "silenced",  BanishManager.Unsilence }
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
        
        TriggerStatusEffect(statusEffect, caster);

        Log(statusEffect.spell + " added");

        //schedule expiration
        statusEffect.ScheduleExpiration(() => ExpireStatusEffect(statusEffect, caster));
    }

    public static void TriggerStatusEffect(StatusEffect effect, IMarker caster)
    {
        foreach (string status in effect.modifiers.status)
        {
            if (m_StatusBehavior.ContainsKey(status))
            {
                Log(status + " triggered");
                m_StatusBehavior[status].Invoke(effect, caster);
            }
        }
    }

    private static void ExpireStatusEffect(StatusEffect statusEffect, IMarker caster)
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
    }

    private static void Log(string msg)
    {
        Debug.Log("[<color=cyan>ConditionManager</color>] " + msg);
    }
}
