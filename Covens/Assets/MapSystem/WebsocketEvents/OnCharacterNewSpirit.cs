using UnityEngine;
using System.Collections;

public static class OnCharacterNewSpirit
{
    public static void HandleEvent(WSData data)
    {
        //add data.spirit, data.banishedOn, data.location to character's knownSpirits list

        HitFXManager.Instance.titleSpirit.text = DownloadedAssets.spiritDictData[data.spirit].spiritName;
        HitFXManager.Instance.titleDesc.text = "You now have the knowledge to summon " + DownloadedAssets.spiritDictData[data.spirit].spiritName;
        HitFXManager.Instance.isSpiritDiscovered = true;
        PlayerDataManager.playerData.KnownSpiritsList.Add(data.spirit);
        var k = new KnownSpirits();
        k.banishedOn = data.banishedOn;
        k.id = data.spirit;
        k.location = data.location;
        PlayerDataManager.playerData.knownSpirits.Add(k);
    }
}
