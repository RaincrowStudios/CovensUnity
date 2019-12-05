using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootMarker : MuskMarker
{
    public LootToken LootToken { get; private set; }
    public bool IsEligible { get; private set; }

    public override void Setup(Token data)
    {
        base.Setup(data);

        LootToken = data as LootToken;

        if (string.IsNullOrEmpty(TeamManager.MyCovenId))
            IsEligible = LootToken.eligibleCharacters.Contains(PlayerDataManager.playerData.instance);
        else
            IsEligible = LootToken.eligibleCovens.Contains(TeamManager.MyCovenId);
    }

}
