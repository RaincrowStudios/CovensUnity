using UnityEngine;

public class OnCharacterAlignmentChange : MonoBehaviour
{
    public static void HandleEvent(WSData data)
    {
        var pd = PlayerDataManager.playerData;
        pd.currentAlignment = data.currentAlignment;
        pd.maxAlignment = data.maxAlignment;
        pd.minAlignment = data.minAlignment;
    }
}