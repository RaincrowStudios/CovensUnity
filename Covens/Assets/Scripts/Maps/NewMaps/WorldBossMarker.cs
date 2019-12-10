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

        Interactable = true;
    }

    protected override void SetupAvatar()
    {
        if (m_AvatarRenderer.sprite != null)
            return;

        DownloadedAssets.GetSprite(bossToken.spiritId, spr => m_AvatarRenderer.sprite = spr);
    }

    protected override void SetupIcon()
    {
        if (m_IconRenderer.sprite != null)
            return;

        DownloadedAssets.GetSprite(bossToken.spiritId + "_portrait", spr => m_IconRenderer.sprite = spr);
    }

    public override void OnWillDespawn()
    {
        base.OnWillDespawn();
        Interactable = false;
    }
}
