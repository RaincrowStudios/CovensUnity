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
        //tempfix while art is not loaded from assertbundle
        Sprite avatar = m_AvatarRenderer.sprite;
        Sprite icon = m_IconRenderer.sprite;
               
        base.OnDespawn();

        m_AvatarRenderer.sprite = avatar;
        m_IconRenderer.sprite = icon;
    }
}
