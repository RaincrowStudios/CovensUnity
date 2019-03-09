using UnityEngine;
using System.Collections;

public static class OnCharacterLocationGained 
{
    public static void HandleEvent(WSData data)
    {
        LocationUIManager.Instance.CharacterLocationGained(data.instance);
    }
}
