using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using System.Collections.Generic;

public class LocationMarker : MuskMarker
{
    [Header("PoP Marker")]
    [SerializeField] private Transform m_AvatarGroup;
    [SerializeField] private SpriteRenderer m_Ring;
    [SerializeField] private Transform m_CharacterTransform;

    public override Transform AvatarTransform => m_CharacterTransform;

    private int m_TweenId;

    public override void Setup(Token data)
    {
        base.Setup(data);

        m_CharacterRenderers = new List<SpriteRenderer> { m_AvatarRenderer, m_Ring };

        IsShowingAvatar = true;
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        LeanTween.cancel(m_TweenId);
    }
}
