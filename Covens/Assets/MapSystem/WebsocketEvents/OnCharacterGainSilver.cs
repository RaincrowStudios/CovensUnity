using UnityEngine;
using System.Collections;

public static class OnCharacterGainSilver
{
    public static void HandleEvent(WSData data)
    {
        //ayerDataManager.playerData.silver
        PlayerDataManager.playerData.silver += data.silver;
		PlayerManagerUI.Instance.UpdateDrachs ();
    }
}