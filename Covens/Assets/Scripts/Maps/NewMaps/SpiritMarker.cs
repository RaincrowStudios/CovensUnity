using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using TMPro;
using System.Collections.Generic;

public class SpiritMarker : MuskMarker
{
    [Header("Spirit Marker")]
    [SerializeField] private Transform m_AvatarGroup;
    [SerializeField] private Transform m_IconGroup;
    [SerializeField] private Transform m_NamePlate;

    [SerializeField] private TextMeshPro m_DisplayName;
    [SerializeField] private TextMeshPro m_Tier;

    [SerializeField] private SpriteRenderer m_IconRenderer;

    public SpiritData spiritData { get; private set; }
    public SpiritToken spiritToken { get => Token as SpiritToken; }
    private int m_TweenId;

    public override Transform AvatarTransform
    {
        get
        {
            if (IsShowingIcon)
                return m_IconRenderer.transform;
            return m_AvatarRenderer.transform.parent;
        }
    }
    public Sprite tierIcon { get { return m_IconRenderer.sprite; } }

    public override void Setup(Token data)
    {
        base.Setup(data);

        spiritData = DownloadedAssets.GetSpirit(spiritToken.spiritId);

        m_DisplayName.text = LocalizeLookUp.GetSpiritName(spiritToken.spiritId);
        SetStats();

        UpdateNameplate(m_DisplayName.preferredWidth);
        UpdateEnergy();

        //todo: load icon and spirit avatar (currently implemented on marker spawner

        IsShowingAvatar = false;
        IsShowingIcon = false;

        m_IconRenderer.sprite = null;
        m_AvatarRenderer.sprite = null;

        m_IconRenderer.sprite = null;
    }

    public override void SetStats()
    {
        if (m_Tier == null)
            return;

        m_Tier.text = spiritData.tier.ToString();
    }

    public override void EnablePortait()
    {
        if (IsShowingIcon)
            return;

        if (m_IconRenderer.sprite == null)
            SetupIcon();

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

        if (m_AvatarRenderer.sprite == null)
            SetupAvatar();

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

    public override void EnablePopSorting()
    {
        base.EnablePopSorting();
        m_NamePlate.transform.localPosition = new Vector3(0, 23, 0);
        m_NamePlate.transform.localScale = Vector3.one * 2;
    }

    public void SetupAvatar()
    {
        //setup spirit sprite
        if (string.IsNullOrEmpty(spiritToken.spiritId))
            Debug.LogError("spritid not sent [" + Token.instance + "]");
        else
        {
            DownloadedAssets.GetSprite(spiritToken.spiritId, (sprite) =>
            {
                if (m_AvatarRenderer != null)
                {
                    float spriteHeight = sprite.rect.height / sprite.pixelsPerUnit;
                    m_AvatarRenderer.transform.localPosition = new Vector3(0, spriteHeight * 0.4f * m_AvatarRenderer.transform.localScale.x, 0);
                    m_AvatarRenderer.sprite = sprite;
                }
            });
        }
    }

    public void SetupIcon()
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

    public override void OnDespawn()
    {
        base.OnDespawn();
        LeanTween.cancel(m_TweenId);
    }


#if UNITY_EDITOR
    [ContextMenu("Update nameplate")]
    private void DebugNameplate()
    {
        UpdateNameplate(m_DisplayName.preferredWidth);
    }
#endif
}
