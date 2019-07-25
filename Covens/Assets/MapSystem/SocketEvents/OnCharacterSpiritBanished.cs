using UnityEngine;
using System.Collections;

public static class OnCharacterSpiritBanished
{
    public static event System.Action<string, string> OnSpiritBanished;
    
    public static void HandleEvent(WSData data)
    {
        
        OnSpiritBanished?.Invoke(data.instance, data.killer);
        UISpiritBanished.Instance.Show(data.spirit);
    }
}
