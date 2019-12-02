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
}
