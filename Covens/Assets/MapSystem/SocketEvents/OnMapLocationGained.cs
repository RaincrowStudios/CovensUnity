using UnityEngine;
using System.Collections;

//sent to all coven members (both inside AND outside pops)
public static class OnMapLocationGained
{
    public static event System.Action<string> OnLocationGained;

    public static void HandleEvent(WSData data)
    {
        OnLocationGained?.Invoke(data.instance);
    }
}
