using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class WitchMarker : MuskMarker
{
    [SerializeField] private Transform m_AvatarGroup;
    [SerializeField] private Transform m_IconGroup;
    [SerializeField] private Transform m_Character;


    [SerializeField] private TextMeshPro m_DisplayName;
    [SerializeField] private TextMeshPro m_Level;

    [SerializeField] private SpriteRenderer m_IconRenderer;

    [SerializeField] private Transform m_StatContainer;
    [SerializeField] private SpriteRenderer m_NameBanner;

    [SerializeField] private SpriteRenderer m_ring1;
    [SerializeField] private double m_latitude;
    [SerializeField] private double m_longitude;



    private int m_TweenId;

    public override Transform characterTransform
    {
        get
        {
            if (IsShowingIcon)
                return m_IconRenderer.transform;
            return m_Character;
        }
    }

    public void GetAvatar(System.Action<Sprite> callback)
    {
        if (m_AvatarRenderer.sprite != null)
            callback.Invoke(m_AvatarRenderer.sprite);
        else
            SetupAvatar(m_Data.male, new List<EquippedApparel>(m_Data.equipped.Values), callback);
    }

    public void GetPortrait(System.Action<Sprite> callback)
    {
        if (m_IconRenderer.sprite != null)
            callback.Invoke(m_IconRenderer.sprite);
        else
            SetupPortrait(m_Data.male, new List<EquippedApparel>(m_Data.equipped.Values), callback);
    }

    public override void Setup(Token data)
    {
        base.Setup(data);

        m_latitude = data.latitude;
        m_longitude = data.longitude;

        IsShowingAvatar = false;
        IsShowingIcon = false;

        m_IconRenderer.sprite = null;
        m_AvatarRenderer.sprite = null;

        m_CharacterRenderers = new SpriteRenderer[] { m_AvatarRenderer, m_ring1 };

        m_AvatarGroup.localScale = Vector3.zero;
        m_IconGroup.localScale = Vector3.zero;

        m_DisplayName.text = data.displayName;
        SetStats(data.level);
        SetRingAmount();
        UpdateEnergy(data.energy, data.baseEnergy);

        //SetTextAlpha(0.3f + defaultTextAlpha);
    }

    public override void EnablePortait()
    {
        if (IsShowingIcon)
            return;

        if (m_IconRenderer.sprite == null)
            SetupPortrait(m_Data.male, new List<EquippedApparel>(m_Data.equipped.Values));

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
            SetupAvatar(m_Data.male, new List<EquippedApparel>(m_Data.equipped.Values));

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

    public override void SetStats(int level)
    {
        if (m_Level == null)
            return;

        Vector2 bannerSize = new Vector2(MapUtils.scale(2.2f, 9.5f, .86f, 8f, m_DisplayName.preferredWidth), m_NameBanner.size.y);
        m_NameBanner.size = bannerSize;
        Vector3 statPos = new Vector3(-MapUtils.scale(0f, 3.6f, 2.2f, 9.5f, m_NameBanner.size.x), m_StatContainer.localPosition.y, m_StatContainer.localPosition.z);
        m_StatContainer.localPosition = statPos;

        m_Level.text = level.ToString();
    }

    public void SetupAvatar(bool male, List<EquippedApparel> equips, System.Action<Sprite> callback = null)
    {
        //generate sprites for avatar and icon
        AvatarSpriteUtil.Instance.GenerateFullbodySprite(male, equips, spr =>
        {
            if (m_AvatarRenderer != null)
            {
                m_AvatarRenderer.transform.localPosition = Vector3.zero;
                m_AvatarRenderer.sprite = spr;
            }
            callback?.Invoke(spr);
        });
    }

    public void SetupPortrait(bool male, List<EquippedApparel> equips, System.Action<Sprite> callback = null)
    {
        AvatarSpriteUtil.Instance.GeneratePortrait(male, equips, spr =>
        {
            if (m_IconRenderer != null)
                m_IconRenderer.sprite = spr;
            callback?.Invoke(spr);
        });
    }

    public void SetRingAmount()
    {
        Color color;
        if (m_Data.degree < 0)
            color = Utilities.Purple;
        else if (m_Data.degree == 0)
            color = Utilities.Blue;
        else
            color = Utilities.Orange;

        color.a = characterAlpha * alpha;
        m_ring1.color = color;
    }

    public override void UpdateEnergy(int energy, int baseEnergy)
    {
        var ind = Mathf.RoundToInt(MapUtils.scale(0, 12, 0, baseEnergy, energy));
        ind = 12 - (int)Mathf.Clamp(ind, 0, 12);
        m_ring1.sprite = MarkerSpawner.Instance.EnergyRings[ind];
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        LeanTween.cancel(m_TweenId);
    }
}
