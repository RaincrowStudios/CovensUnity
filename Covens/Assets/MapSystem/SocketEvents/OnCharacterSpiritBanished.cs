using UnityEngine;
using System.Collections;

public static class OnCharacterSpiritBanished
{
    public static event System.Action<string, string> OnSpiritBanished;
    
    public static void HandleEvent(WSData data)
    {
        
        OnSpiritBanished?.Invoke(data.instance, data.killer);
		Debug.Log ("banishing");
        if (!PlaceOfPower.IsInsideLocation)
            UISpiritBanished.Instance.Show(data.spirit);
        else
            UISpiritBanished.Instance.Show(data.spirit, true);
    }
}
