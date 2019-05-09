using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using TMPro;
using System.Collections.Generic;

public class SpiritMarker : MuskMarker
{
    [SerializeField] private Transform m_AvatarGroup;
    [SerializeField] private Transform m_IconGroup;

    //[SerializeField] private TextMeshPro m_Stats;
    [SerializeField] private TextMeshPro m_DisplayName;
	[SerializeField] private SpriteRenderer m_NameBanner;
	[SerializeField] private Transform m_StatContainer;

    [SerializeField] private SpriteRenderer m_IconRenderer;
	[SerializeField] private SpriteRenderer o_Ring;

    private int m_TweenId;

    public override Transform characterTransform
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

        SetTextAlpha(defaultTextAlpha);
        UpdateEnergy(data.energy, data.baseEnergy);
        m_DisplayName.text = DownloadedAssets.GetSpirit(data.spiritId).spiritName;

        //todo: load icon and spirit avatar (currently implemented on marker spawner

        IsShowingAvatar = false;
        IsShowingIcon = false;

        m_IconRenderer.sprite = null;
        m_AvatarRenderer.sprite = null;

        m_IconRenderer.sprite = MarkerSpawner.GetSpiritTierSprite(data.spiritType);


		Vector2 bannerSize = new Vector2(MapUtils.scale(1.4f, 5.2f, 1.23f, 4.8f, m_DisplayName.preferredWidth), m_NameBanner.size.y);
		m_NameBanner.size = bannerSize;
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

    public override void UpdateEnergy(int energy, int baseEnergy)
    {
		
        //m_Stats.text = $"Energy: <color=#4C80FD><b>{energy}</b></color>\n";
		var ind = Mathf.RoundToInt (MapUtils.scale (0, 12, 0, baseEnergy, energy));
		ind = 12 - (int)Mathf.Clamp (ind, 0, 12);
		o_Ring.sprite = MarkerSpawner.Instance.EnergyRings [ind];

		//Vector3 statPos = new Vector3(-MapUtils.scale(0f, 3.6f, 2.2f, 9.5f, m_NameBanner.size.x), m_StatContainer.localPosition.y, m_StatContainer.localPosition.z);
		//m_StatContainer.localPosition = statPos;
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
                if (m_AvatarRenderer != null)
                {
                    float spriteHeight = sprite.rect.height / sprite.pixelsPerUnit;
                    m_AvatarRenderer.transform.localPosition = new Vector3(0, spriteHeight * 0.4f * m_AvatarRenderer.transform.localScale.x, 0);
                    m_AvatarRenderer.sprite = sprite;
                }
            });
        }
    }

    public override void SetTextAlpha(float a)
    {
        base.SetTextAlpha(a);

        //m_DisplayName.alpha = textAlpha * multipliedAlpha;
       // m_Stats.alpha = textAlpha * multipliedAlpha;
    }

    private void OnDestroy()
    {
        LeanTween.cancel(m_TweenId);
    }
}
