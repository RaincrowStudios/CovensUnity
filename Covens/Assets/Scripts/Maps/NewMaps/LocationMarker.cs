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

    public override Transform AvatarTransform => m_CharacterTransform;

    private int m_TweenId;

    public override void Setup(Token data)
    {
        base.Setup(data);
        var popData = (PopToken)data;

        if (popData != null && popData.lastOwnedBy != null && popData.lastOwnedBy.displayName != null)
        {
            m_OwnedBy.text = popData.lastOwnedBy.displayName;
        }

        m_CharacterRenderers = new List<SpriteRenderer> { m_AvatarRenderer, m_Ring };

        IsShowingAvatar = true;
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        LeanTween.cancel(m_TweenId);
    }
}
