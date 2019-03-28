using UnityEngine;
using System.Collections;

public static class OnCharacterSpiritBanished
{
    public static event System.Action<string, string> OnSpiritBanished;
    
    public static void HandleEvent(WSData data)
    {
        Debug.Log("character_spirit_banished\n" + data.json);
        OnSpiritBanished?.Invoke(data.instance, data.killer);
		Debug.Log ("banishing");
		UISpiritBanished.Instance.Show(data.spirit);
    }
}
