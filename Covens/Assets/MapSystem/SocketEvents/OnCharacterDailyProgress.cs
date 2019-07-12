using UnityEngine;
using System.Collections;

public static class OnCharacterDailyProgress
{
    public static void HandleEvent(WSData data)
    {
        QuestsController.instance.OnProgress(data.daily, data.count, data.silver);
        
        PlayerDataManager.playerData.silver += data.silver;
        PlayerManagerUI.Instance.UpdateDrachs();
    }
}
