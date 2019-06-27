using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCharacterCooldown
{
    public static event System.Action<string> OnCooldownEnd;

    public static void OnStart(WSData data)
    {
        string id = data.spell;
        double timestamp = data.cooldownTime;

        PlayerManager.m_CooldownDictionary[id] = timestamp;
    }

    public static void OnFinish(WSData data)
    {
        if (PlayerManager.m_CooldownDictionary.ContainsKey(data.spell))
            PlayerManager.m_CooldownDictionary.Remove(data.spell);

        OnCooldownEnd?.Invoke(data.spell);
    }
}
