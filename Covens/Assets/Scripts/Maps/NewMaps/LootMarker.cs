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

        SetDisable(!IsEligible);
    }

    public void OpenChest(CollectLootHandler.EventData data)
    {
        LootToken.eligibleCharacters.Remove(PlayerDataManager.playerData.instance);
        SetDisable(true);
    }

    public void SetLoading(bool value)
    {
        m_Animator.SetBool("loading", value);
    }

    public void SetDisable(bool value)
    {
        if (value)
            m_Particles.Stop();
        else
            m_Particles.Play();

        Interactable = !value;
        m_Animator.SetBool("disabled", value);
    }

    //public void SetDespawn()
    //{
    //    m_Particles.Stop();
    //    m_Animator.SetTrigger("despawn");
    //}
}
