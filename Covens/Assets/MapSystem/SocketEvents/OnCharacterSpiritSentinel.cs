using UnityEngine;
using System.Collections;

public static class OnCharacterSpiritSentinel
{
    public static void HandleEvent(WSData data)
    {
        Debug.Log("character_spirit_sentinel\n" + data.json);
    }
}
