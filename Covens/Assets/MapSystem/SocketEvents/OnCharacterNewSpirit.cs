using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class OnCharacterNewSpirit
{
    public static void HandleEvent(WSData data)
    {
        //add data.spirit, data.banishedOn, data.location to character's knownSpirits list

        //HitFXManager.Instance.titleSpirit.text = DownloadedAssets.spiritDictData[data.spirit].spiritName;
        //HitFXManager.Instance.titleDesc.text = "You now have the knowledge to summon " + DownloadedAssets.spiritDictData[data.spirit].spiritName;
        //HitFXManager.Instance.isSpiritDiscovered = true;

        UISpiritDiscovered.Instance.Show(data.spirit);

        var k = new KnownSpirits();
        k.banishedOn = data.banishedOn;
        k.spirit = data.spirit;
        k.dominion = data.location;
        //k.tags = new List<string>(data.tags);
        PlayerDataManager.playerData.knownSpirits.Add(k);
    }
}
