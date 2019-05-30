using UnityEngine;
using System.Collections;

public static class OnCharacterSpellMove
{
    public static void HandleEvent(WSData data)
    {
        if (data.spell == "spell_banish")
        {
            PlayerManager.Instance.StartCoroutine(BanishWaitTillLocationLeave(data));
        } // handle magic dance;
    }
    
    private static IEnumerator BanishWaitTillLocationLeave(WSData data)
    {
        yield return new WaitUntil(() => PlaceOfPower.IsInsideLocation == false);
        BanishManager.Instance.Banish(data.longitude, data.latitude, data.caster);
    }
}
