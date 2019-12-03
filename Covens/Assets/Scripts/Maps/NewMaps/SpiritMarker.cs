using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using TMPro;
using System.Collections.Generic;

public class SpiritMarker : CharacterMarker
{
    public SpiritData spiritData { get; private set; }
    public SpiritToken spiritToken { get => Token as SpiritToken; }
    
    public override string Name => spiritData.Name;

    public Sprite tierIcon { get { return m_IconRenderer.sprite; } }
    
    public override void Setup(Token data)
    {
        spiritData = DownloadedAssets.GetSpirit((data as SpiritToken).spiritId);

        base.Setup(data);

        m_DisplayName.text = LocalizeLookUp.GetSpiritName(spiritData.id);
    }

    public override void SetStats()
    {
        if (m_Level == null)
            return;

        m_Level.text = spiritData.tier.ToString();
    }
    
    protected override void SetupAvatar()
    {
        //setup spirit sprite
        if (string.IsNullOrEmpty(spiritToken.spiritId))
        {
            Debug.LogError("spritid not sent [" + Token.instance + "]");
        }
        else
        {
            m_AvatarRenderer.color = new Color(1, 1, 1, 0);
            DownloadedAssets.GetSprite(spiritToken.spiritId, (sprite) =>
            {
                if (m_AvatarRenderer != null && sprite != null)
                {
                    float spriteHeight = sprite.rect.height / sprite.pixelsPerUnit;
                    m_AvatarRenderer.transform.localPosition = new Vector3(0, spriteHeight * 0.4f * m_AvatarRenderer.transform.localScale.x, 0);
                    m_AvatarRenderer.sprite = sprite;
                    LeanTween.color(
                        m_AvatarRenderer.gameObject,
                        spiritToken.IsBossSummon ? new Color(1, 0, 0.2f) : Color.white, 1f
                    ).setEaseOutCubic();
                    m_EnergyRing.color = spiritToken.IsBossSummon ? new Color(1, 0, 0.2f) : Color.white;
                }
            });
        }
    }

    protected override void SetupIcon()
    {
        if (string.IsNullOrEmpty(spiritToken.spiritId))
        {
            Debug.LogError("spritid not sent [" + Token.instance + "]");
        }
        else
        {
            SpiritData spirit = DownloadedAssets.GetSpirit(spiritToken.spiritId);
            m_IconRenderer.sprite = MarkerSpawner.GetSpiritTierSprite(spirit.type);
        }
    }


#if UNITY_EDITOR
    [ContextMenu("Update nameplate")]
    private void DebugNameplate()
    {
        UpdateNameplate(m_DisplayName.preferredWidth);
    }
#endif
}
