using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBossMarker : CharacterMarker
{
    //public SpiritData spiritData { get; private set; }
    public BossToken bossToken { get => Token as BossToken; }

    public override void Setup(Token data)
    {
        //spiritData = DownloadedAssets.GetSpirit((data as SpiritToken).spiritId);
        m_EnergyRing.color = new Color(1, 0, 0.2f);

        base.Setup(data);

        m_DisplayName.text = LocalizeLookUp.GetSpiritName(bossToken.spiritId);
    }

    protected override void SetupAvatar()
    {
        //throw new System.NotImplementedException();
    }

    protected override void SetupIcon()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnWillDespawn()
    {
        base.OnWillDespawn();
        Interactable = false;
    }

    public override void OnDespawn()
    {
        //base.OnDespawn();
        //LeanTween.cancel(m_TweenId);
        //LeanTween.cancel(m_EnergyRingTweenId);

        IsShowingAvatar = false;
        IsShowingIcon = false;

        //m_AvatarRenderer.sprite = null;
        //m_IconRenderer.sprite = null;

        m_Animator.SetBool("avatar", IsShowingAvatar);
        m_Animator.SetBool("icon", IsShowingIcon);
    }
}
