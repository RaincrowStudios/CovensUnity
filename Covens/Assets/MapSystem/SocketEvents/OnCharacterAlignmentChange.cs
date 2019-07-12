using UnityEngine;

public class OnCharacterAlignmentChange : MonoBehaviour
{
    public static void HandleEvent(WSData data)
    {
		
        var pd = PlayerDataManager.playerData;
        pd.alignment = data.currentAlignment;
    }
}