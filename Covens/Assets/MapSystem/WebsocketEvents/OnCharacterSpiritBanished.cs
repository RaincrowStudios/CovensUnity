using UnityEngine;
using System.Collections;

public static class OnCharacterSpiritBanished
{
    public static void HandleEvent(WSData data)
    {
        Debug.Log("character_spirit_banished\n" + data.json);
    }
}
