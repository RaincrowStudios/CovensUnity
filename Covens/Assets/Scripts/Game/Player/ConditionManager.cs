using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Raincrow.GameEventResponses;
using Raincrow.Maps;

public static class ConditionManager
{
    public static System.Action<StatusEffect> OnPlayerApplyStatusEffect;
    public static System.Action<StatusEffect> OnPlayerExpireStatusEffect;

    private static Dictionary<string, System.Action<SpellCastHandler.SpellCastEventData, IMarker, IMarker>> m_StatusBehaviorDict = new Dictionary<string, System.Action<SpellCastHandler.SpellCastEventData, IMarker, IMarker>>()
    {
        { "bound",     BanishManager.Bind },
        { "silenced",  BanishManager.Silence }
    };

    public static void AddCondition(StatusEffect statusEffect, SpellCastHandler.SpellCastEventData data)
    {
        //remove old condition matching the same spell
        foreach (StatusEffect item in PlayerDataManager.playerData.effects)
        {
            if (item.spell == statusEffect.spell)
            {
                PlayerDataManager.playerData.effects.Remove(item);
                break;
            }
        }
        PlayerDataManager.playerData.effects.Add(statusEffect);
        OnPlayerApplyStatusEffect?.Invoke(statusEffect);

        //schedule expiration
        data.result.statusEffect.ScheduleExpiration();

        //TriggerStatusEffect(effect);

    }

    //public static void TriggerStatusEffect(StatusEffect effect)
    //{

    //    if (m_StatusBehaviorDict.ContainsKey(effect.modifiers))
    //    {
    //        m_StatusBehaviorDict[spell.id].Invoke(data, caster, target);
    //    }
    //}
}
