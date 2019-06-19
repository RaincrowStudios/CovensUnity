using UnityEngine;
using System.Collections;
using Raincrow.Maps;

public class LocationMarker : MuskMarker
{
    [SerializeField] private Transform m_AvatarGroup;
    [SerializeField] private Transform m_IconGroup;
    [SerializeField] private SpriteRenderer m_Ring;
    [SerializeField] private Transform m_CharacterTransform;
    [SerializeField] private Transform m_IconTransform;

    public override Transform characterTransform
    {
        get
        {
            if (IsShowingIcon)
                return m_IconTransform;
            else
                return m_CharacterTransform;
        }
    }

    private int m_TweenId;

    public override void Setup(Token data)
    {
        base.Setup(data);

        m_CharacterRenderers = new SpriteRenderer[] { m_AvatarRenderer, m_Ring };
        
        IsShowingAvatar = false;
        IsShowingIcon = false;
    }

    public override void EnablePortait()
    {
        if (IsShowingIcon)
            return;
        
        IsShowingIcon = true;
        IsShowingAvatar = false;

        LeanTween.cancel(m_TweenId);

        m_TweenId = LeanTween.value(m_IconGroup.localScale.x, 1, 0.5f)
            .setEaseOutCubic()
            .setOnStart(() =>
            {
                m_IconGroup.gameObject.SetActive(true);
            })
            .setOnUpdate((float t) =>
            {
                m_IconGroup.localScale = new Vector3(t, t, t);
                m_AvatarGroup.localScale = new Vector3(1 - t, 1 - t, 1 - t);
            })
            .setOnComplete(() =>
            {
                m_AvatarGroup.gameObject.SetActive(false);
            })
            .uniqueId;
    }

    public override void EnableAvatar()
    {
        if (IsShowingAvatar)
            return;
        
        IsShowingAvatar = true;
        IsShowingIcon = false;

        LeanTween.cancel(m_TweenId);

        m_TweenId = LeanTween.value(m_AvatarGroup.localScale.x, 1, 0.5f)
            .setEaseOutCubic()
            .setOnStart(() =>
            {
                m_AvatarGroup.gameObject.SetActive(true);
            })
            .setOnUpdate((float t) =>
            {
                m_AvatarGroup.localScale = new Vector3(t, t, t);
                m_IconGroup.localScale = new Vector3(1 - t, 1 - t, 1 - t);
            })
            .setOnComplete(() =>
            {
                m_IconGroup.gameObject.SetActive(false);
            })
            .uniqueId;
    }

    public override void Destroy()
    {
        base.Destroy();
        LeanTween.cancel(m_TweenId);
    }
}
