using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using TMPro;
using System.Collections.Generic;

public class WitchMarker : NewMapsMarker
{
    [SerializeField] private Transform m_AvatarGroup;
    [SerializeField] private Transform m_IconGroup;
    [SerializeField] private Transform m_Character;
    [SerializeField] GameObject[] m_IconSchools;
    [SerializeField] GameObject[] m_AvatarSchools;

    [SerializeField] private TextMeshPro m_Stats;
    [SerializeField] private TextMeshPro m_DisplayName;

    [SerializeField] private SpriteRenderer m_AvatarRenderer;
    [SerializeField] private SpriteRenderer m_IconRenderer;

    private int m_TweenId;

    public override Transform characterTransform { get { return m_Character; } }

    public override void Setup(Token data)
    {
        base.Setup(data);

        IsShowingAvatar = false;
        IsShowingIcon = false;

        m_IconRenderer.sprite = null;
        m_AvatarRenderer.sprite = null;

        m_AvatarGroup.localScale = Vector3.zero;
        m_IconGroup.localScale = Vector3.zero;
        
        m_DisplayName.text = data.displayName;
        SetStats(data.level, data.energy);

        m_DisplayName.alpha = defaultTextAlpha;
        m_Stats.alpha = defaultTextAlpha;

        //setup school fx
        for (int i = 0; i < m_AvatarSchools.Length; i++)
            m_AvatarSchools[i].SetActive(false);
        for (int i = 0; i < m_IconSchools.Length; i++)
            m_IconSchools[i].SetActive(false);

        if (data.degree < 0)
        {
            m_AvatarSchools[0].gameObject.SetActive(true);
            m_IconSchools[0].gameObject.SetActive(true);
        }
        else if (data.degree == 0)
        {
            m_AvatarSchools[1].gameObject.SetActive(true);
            m_IconSchools[1].gameObject.SetActive(true);
        }
        else
        {
            m_AvatarSchools[2].gameObject.SetActive(true);
            m_IconSchools[2].gameObject.SetActive(true);
        }
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

    public override void SetStats(int level, int energy)
    {
        if (m_Stats == null)
            return;

        string color = "";
        if (m_Data.degree < 0) color = m_ShadowColor;
        else if (m_Data.degree == 0) color = m_GreyColor;
        else color = m_WhiteColor;

        m_Stats.text =
            $"Energy: <color={color}><b>{energy}</b></color>\n" +
            $"lvl: <color={color}><b>{level}</b></color>";
    }

    public void SetupAvatar(bool male, List<EquippedApparel> equips)
    {
        //shadow scale
        //m_AvatarGroup.GetChild(2).localScale = male ? new Vector3(8, 8, 8) : new Vector3(6, 6, 6);

        //generate sprites for avatar and icon
        AvatarSpriteUtil.Instance.GenerateFullbodySprite(male, equips, spr =>
        {
            m_AvatarRenderer.transform.localPosition = Vector3.zero;
            m_AvatarRenderer.sprite = spr;
        });
    }

    public void SetupPortrait(bool male, List<EquippedApparel> equips)
    {
        AvatarSpriteUtil.Instance.GeneratePortrait(male, equips, spr =>
        {
            m_IconRenderer.sprite = spr;
        });
    }

    //public void SetupAvatarAndPortrait(bool male, List<EquippedApparel> equips)
    //{
    //    AvatarSpriteUtil.Instance.GeneratePortraitAndFullbody(male, equips,
    //        portrait =>
    //        {
    //            //portrait
    //            m_IconRenderer.sprite = portrait;
    //        },
    //        avatar =>
    //        {
    //            m_AvatarRenderer.transform.localPosition = Vector3.zero;
    //            m_AvatarRenderer.sprite = avatar;
    //        });
    //}

    public override void SetTextAlpha(float a)
    {
        LeanTween.value(m_DisplayName.alpha, a, 0.3f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_DisplayName.alpha = t;
                m_Stats.alpha = t;
            });
    }

    public override void SetAlpha(float a)
    {
        Color aux = m_AvatarRenderer.color;
        LeanTween.value(aux.a, a, 0.3f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                aux.a = t;
                m_AvatarRenderer.color = aux;
            });
    }
}
