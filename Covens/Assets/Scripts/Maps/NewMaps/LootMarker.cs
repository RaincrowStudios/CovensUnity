using Raincrow.GameEventResponses;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootMarker : MuskMarker
{
    [SerializeField] private ParticleSystem m_Particles;

    public LootToken LootToken { get; private set; }

    public bool IsEligible
    {
        get
        {
            //if (string.IsNullOrEmpty(TeamManager.MyCovenId))
                return LootToken.eligibleCharacters.Contains(PlayerDataManager.playerData.instance);
            //else
            //    return LootToken.eligibleCovens.Contains(TeamManager.MyCovenId);
        }
    }

    public override void Setup(Token data)
    {
        base.Setup(data);

        LootToken = data as LootToken;

        if (IsEligible)
            SetClosed();
        else
            SetOpened();
    }

    public void OpenChest(CollectLootHandler.EventData data)
    {
        LootToken.eligibleCharacters.Remove(PlayerDataManager.playerData.instance);
        SetOpened();
    }

    public void SetOpened()
    {
        m_Particles.Stop();
    }

    public void SetClosed()
    {
        m_Particles.Play();
    }
}
