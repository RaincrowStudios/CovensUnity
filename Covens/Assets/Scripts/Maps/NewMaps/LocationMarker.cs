using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using System.Collections.Generic;
using TMPro;

public class LocationMarker : MuskMarker
{
    [Header("PoP Marker")]
    [SerializeField] private Transform m_AvatarGroup;
    [SerializeField] private SpriteRenderer m_Ring;
    [SerializeField] private TextMeshPro m_OwnedBy;
    [SerializeField] private Transform m_CharacterTransform;
    [SerializeField] private Sprite[] m_SchoolBanners;
    [SerializeField] private Sprite[] m_SchoolSeals;
    [SerializeField] private SpriteRenderer m_BannerSprite;
    [SerializeField] private SpriteRenderer m_SchoolSprite;

    public override Transform AvatarTransform => m_CharacterTransform;

    private int m_TweenId;

    public override void Setup(Token data)
    {
        base.Setup(data);
        var popData = (PopToken)data;

        if (popData != null && popData.lastOwnedBy != null && popData.lastOwnedBy.displayName != null)
        {
            m_OwnedBy.text = popData.lastOwnedBy.displayName;
            {
                m_SchoolSprite.gameObject.SetActive(true);
                if (popData.lastOwnedBy.degree > 0)
                {
                    m_BannerSprite.sprite = m_SchoolBanners[0];
                    m_SchoolSprite.sprite = m_SchoolSeals[0];
                    m_SchoolSprite.color = Utilities.Orange;
                }
                else if (popData.lastOwnedBy.degree < 0)
                {
                    m_BannerSprite.sprite = m_SchoolBanners[1];
                    m_SchoolSprite.sprite = m_SchoolSeals[1];
                    m_SchoolSprite.color = Utilities.Purple;

                }
                else
                {
                    m_BannerSprite.sprite = m_SchoolBanners[2];
                    m_SchoolSprite.sprite = m_SchoolSeals[2];
                    m_SchoolSprite.color = Utilities.Blue;

                }
            }
        }
        else
        {
            m_SchoolSprite.gameObject.SetActive(false);
            m_BannerSprite.sprite = m_SchoolBanners[3];
            m_OwnedBy.text = "Unclaimed";
        }

        //m_CharacterRenderers = new List<SpriteRenderer> { m_AvatarRenderer, m_Ring };

        IsShowingAvatar = true;
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        LeanTween.cancel(m_TweenId);
    }
}
