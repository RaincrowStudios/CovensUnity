using UnityEngine;
using System.Collections;

public static class OnCharacterLocationBoot
{
    public static void HandleEvent(WSData data)
    {
        PlaceOfPower.LeavePoP(false);
    }
}
