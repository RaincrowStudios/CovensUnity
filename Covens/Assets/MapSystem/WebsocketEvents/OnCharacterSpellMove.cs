using UnityEngine;
using System.Collections;

public static class OnCharacterSpellMove
{
    public static void HandleEvent(WSData data)
    {
        if (data.spell == "spell_banish")
        {
            BanishManager.Instance.Banish(data.longitude, data.latitude, data.caster);
        } // handle magic dance;
    }
}
