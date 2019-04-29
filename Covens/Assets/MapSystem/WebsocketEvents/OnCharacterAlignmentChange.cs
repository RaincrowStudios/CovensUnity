using UnityEngine;

public class OnCharacterAlignmentChange : MonoBehaviour
{
    public static void HandleEvent(WSData data)
    {
		Debug.Log ("test" + data.json);
        var pd = PlayerDataManager.playerData;
        pd.currentAlignment = data.currentAlignment;
        pd.maxAlignment = data.maxAlignment;
        pd.minAlignment = data.minAlignment;
    }
}