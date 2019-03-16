using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using TMPro;
using System.Collections.Generic;

public class SpiritMarker : NewMapsMarker
{
    [SerializeField] private Transform m_AvatarGroup;
    [SerializeField] private Transform m_IconGroup;

    [SerializeField] private TextMeshPro m_Stats;
    [SerializeField] private TextMeshPro m_DisplayName;

    [SerializeField] private SpriteRenderer m_AvatarRenderer;
    [SerializeField] private SpriteRenderer m_IconRenderer;

    private int m_TweenId;

    public override Transform characterTransform { get { return m_AvatarRenderer.transform; } }
    public Sprite tierIcon { get { return m_IconRenderer.sprite; } }

    public override void Setup(Token data)
    {
        base.Setup(data);

        m_Stats.alpha = defaultTextAlpha;
        SetStats(0, data.energy);
        m_DisplayName.text = DownloadedAssets.spiritDictData[data.spiritId].spiritName;
        m_DisplayName.alpha = defaultTextAlpha;

        //todo: load icon and spirit avatar (currently implemented on marker spawner

        IsShowingAvatar = false;
        IsShowingIcon = false;

        m_IconRenderer.sprite = null;
        m_AvatarRenderer.sprite = null;

        m_IconRenderer.sprite = MarkerSpawner.GetSpiritTierSprite(data.spiritType);
    }

    public override void EnablePortait()
    {
        if (IsShowingIcon)
            return;
        
        m_Interactable = false;

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

        m_Interactable = true;

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

    public override void SetStats(int level, int energy)
    {
        m_Stats.text = $"Energy: <color=#4C80FD><b>{energy}</b></color>\n";
    }

    public void SetupAvatar()
    {
        //setup spirit sprite
        if (string.IsNullOrEmpty(m_Data.spiritId))
            Debug.LogError("spritid not sent [" + m_Data.instance + "]");
        else
        {
            DownloadedAssets.GetSprite(m_Data.spiritId, (sprite) =>
            {
                float spriteHeight = sprite.rect.height / sprite.pixelsPerUnit;
                m_AvatarRenderer.transform.localPosition = new Vector3(0, spriteHeight * 0.4f * m_AvatarRenderer.transform.localScale.x, 0);
                m_AvatarRenderer.sprite = sprite;
            });
        }
    }
}
