using UnityEngine;
using System.Collections;

public static class OnCharacterSpiritExpired
{
    public static void HandleEvent(WSData data)
    {
        Debug.Log("character_spirit_expired\n" + data.json);
    }
}
