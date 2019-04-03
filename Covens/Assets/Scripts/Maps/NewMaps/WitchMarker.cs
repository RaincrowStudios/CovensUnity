using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class WitchMarker : NewMapsMarker
{
    [SerializeField] private Transform m_AvatarGroup;
    [SerializeField] private Transform m_IconGroup;
    [SerializeField] private Transform m_Character;

    [SerializeField] private TextMeshPro m_DisplayName;
    [SerializeField] private TextMeshPro m_Level;

    [SerializeField] private SpriteRenderer m_AvatarRenderer;
    [SerializeField] private SpriteRenderer m_IconRenderer;


	[SerializeField] private SpriteRenderer m_ring1;

    private int m_TweenId;

    public override Transform characterTransform { get { return m_Character; } }
    
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

        IsShowingAvatar = false;
        IsShowingIcon = false;

        m_IconRenderer.sprite = null;
        m_AvatarRenderer.sprite = null;
        m_AvatarGroup.localScale = Vector3.zero;
        m_IconGroup.localScale = Vector3.zero;
        
        m_DisplayName.text = data.displayName;
        SetStats(data.level);
		SetRingAmount ();
        UpdateEnergy(data.energy);

        m_DisplayName.alpha = 0.3f + defaultTextAlpha;
        m_Level.alpha = 0.3f + defaultTextAlpha;
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

        string color = "";
		if (m_Data.degree < 0) color = "#FFFFFF";
		else if (m_Data.degree == 0) color = "#FFFFFF";
		else color = "#FFFFFF";

        m_Level.text = $"<color={color}><b>{level}</b></color>";
    }

    public void SetupAvatar(bool male, List<EquippedApparel> equips, System.Action<Sprite> callback = null)
    {
        //generate sprites for avatar and icon
        AvatarSpriteUtil.Instance.GenerateFullbodySprite(male, equips, spr =>
        {
            m_AvatarRenderer.transform.localPosition = Vector3.zero;
            m_AvatarRenderer.sprite = spr;
            callback?.Invoke(spr);
        });
    }

    public void SetupPortrait(bool male, List<EquippedApparel> equips, System.Action<Sprite> callback = null)
    {
        AvatarSpriteUtil.Instance.GeneratePortrait(male, equips, spr =>
        {
            m_IconRenderer.sprite = spr;
            callback?.Invoke(spr);
        });
    }

    public override void SetTextAlpha(float a)
    {
        LeanTween.value(m_DisplayName.alpha, a, 0.3f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_DisplayName.alpha = t;
                m_Level.alpha = t;
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

	public void SetRingAmount()
    {
		if (m_Data.degree < 0) {
			m_ring1.color = Utilities.Purple;
		}
		else if (m_Data.degree == 0) {
			m_ring1.color = Utilities.Blue;
		}
		else {
			m_ring1.color = Utilities.Orange;
		}
	}

    public override void UpdateEnergy(int energy)
    {
        var ind = Mathf.RoundToInt(MapUtils.scale(0, 12, 0, m_Data.baseEnergy, energy));
        ind = (int)Mathf.Clamp(ind, 0, 12);
        m_ring1.sprite = MarkerSpawner.Instance.EnergyRings[ind];
    }

    private void OnDestroy()
    {
        LeanTween.cancel(m_TweenId);
    }
}
