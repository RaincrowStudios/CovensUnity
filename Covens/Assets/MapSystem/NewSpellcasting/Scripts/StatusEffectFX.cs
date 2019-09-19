using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Raincrow.Maps;

public static class StatusEffectFX
{
    private static Dictionary<string, System.Action<IMarker, StatusEffect>> m_SpawnDict = new Dictionary<string, System.Action<IMarker, StatusEffect>>()
    {
        { "spell_channeling", SpellChanneling.SpawnFX }
    };
    private static Dictionary<string, System.Action<IMarker, StatusEffect>> m_DespawnDict = new Dictionary<string, System.Action<IMarker, StatusEffect>>()
    {
        { "spell_channeling", SpellChanneling.DespawnFX }
    };

    public static void SpawnFX(IMarker marker, StatusEffect effect)
    {
        if (effect.modifiers.status == null)
            return;

        if (m_SpawnDict.ContainsKey(effect.spell))
            m_SpawnDict[effect.spell].Invoke(marker, effect);
    }

    public static void DespawnFX(IMarker marker, StatusEffect effect)
    {
        if (effect.modifiers.status == null)
            return;

        if (m_DespawnDict.ContainsKey(effect.spell))
            m_DespawnDict[effect.spell].Invoke(marker, effect);
    }
}
