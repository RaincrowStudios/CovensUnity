using UnityEngine;
using System.Collections;

public static class OnMapLocationLost
{
    public static event System.Action<string> OnLocationLost;
    public static void HandleEvent(WSData data)
    {
        OnLocationLost?.Invoke(data.instance);
    }
}
