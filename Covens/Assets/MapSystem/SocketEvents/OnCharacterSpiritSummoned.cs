using UnityEngine;
using System.Collections;

public static class OnCharacterSpiritSummoned
{
    public static void HandleEvent(WSData data)
    {
        Debug.Log("character_spirit_summoned\n" + data.json);
    }
}
