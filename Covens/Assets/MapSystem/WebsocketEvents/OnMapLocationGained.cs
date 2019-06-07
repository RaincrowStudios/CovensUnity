using UnityEngine;
using System.Collections;

public static class OnMapLocationGained
{
    public static event System.Action<string> OnLocationGained;
    public static void HandleEvent(WSData data)
    {
        OnLocationGained?.Invoke(data.instance);
    }
}
