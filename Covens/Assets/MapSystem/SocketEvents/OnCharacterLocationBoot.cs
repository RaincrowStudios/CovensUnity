using UnityEngine;
using System.Collections;

public static class OnCharacterLocationBoot
{
    public static void HandleEvent(WSData data)
    {
        //PlaceOfPower.LeavePoP(false);
        float lng = PlayerDataManager.playerData.longitude;
        float lat = PlayerDataManager.playerData.latitude;

        BanishManager.Instance.Banish(lng, lat, "");
    }
}
