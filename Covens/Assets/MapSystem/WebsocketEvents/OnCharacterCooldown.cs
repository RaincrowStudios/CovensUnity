using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCharacterCooldown
{
    public static void OnStart(WSData data)
    {
        string id = data.spell;
        double timestamp = data.timeStamp;

        //PlayerManager.m_CooldownDictionary[id]
    }

    public static void OnFinish(WSData data)
    {
        //if (PlayerManager.m_CooldownDictionary.ContainsKey)
    }
}
