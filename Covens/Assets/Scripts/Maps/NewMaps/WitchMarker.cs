using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class WitchMarker : MuskMarker
{
    [Header("Witch Marker")]
    [SerializeField] private Transform m_AvatarGroup;
    [SerializeField] private Transform m_IconGroup;
    [SerializeField] private Transform m_Character;


    [SerializeField] private TextMeshPro m_DisplayName;
    [SerializeField] private TextMeshPro m_Level;

    [SerializeField] private SpriteRenderer m_IconRenderer;
    
    public WitchToken witchToken { get => Token as WitchToken; }

    public override string Name => witchToken.displayName;

    private int m_TweenId;

    private Transform m_DeathIcon;
    private Transform m_ImmunityIcon;
    private Transform m_ChannelingFX;

    public override Transform AvatarTransform
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
            SetupAvatar(witchToken.male, witchToken.equipped, callback);
    }

    public void GetPortrait(System.Action<Sprite> callback)
    {
        if (m_IconRenderer.sprite != null)
            callback.Invoke(m_IconRenderer.sprite);
        else
            SetupPortrait(witchToken.male, witchToken.equipped, callback);
    }

    public override void Setup(Token data)
    {
        base.Setup(data);
        m_DisplayName.text = witchToken.displayName;
        SetStats();
        UpdateNameplate(m_DisplayName.preferredWidth);
        SetRingColor();
        UpdateEnergy();

        //set immunity icon
        if (MarkerSpawner.IsTargetImmune(witchToken))
            AddImmunityFX();
        else
            RemoveImmunityFX();

        //set death icon
        if (witchToken.state == "dead" || witchToken.energy <= 0)
            AddDeathFX();
        else
            RemoveDeathFX();

        //setup effects
        foreach (var effect in witchToken.effects)
            OnApplyStatusEffect(effect);
    }

    public override void EnablePopSorting()
    {
        base.EnablePopSorting();
        //m_AvatarRenderer.sortingOrder = 10;
    }

    public override void EnablePortait()
    {
        if (IsShowingIcon)
            return;

        if (m_IconRenderer.sprite == null)
            SetupPortrait(witchToken.male, witchToken.equipped);

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
            SetupAvatar(witchToken.male, witchToken.equipped);

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

    public override void SetStats()
    {
        if (m_Level == null)
            return;

        m_Level.text = witchToken.level.ToString();
    }

    public void UpdateEquips(List<EquippedApparel> equips)
    {
        //update equip list and generate new textures if visible
        witchToken.equipped = equips;

        if (m_AvatarRenderer != null)
        {
            if (IsShowingAvatar)
                SetupAvatar(witchToken.male, witchToken.equipped);
            else if (m_AvatarRenderer.sprite != null && m_AvatarRenderer.sprite.texture != null)
                Destroy(m_AvatarRenderer.sprite.texture);
        }
        if (m_IconRenderer != null)
        {
            if (IsShowingIcon)
                SetupPortrait(witchToken.male, witchToken.equipped);
            else if (m_IconRenderer.sprite != null && m_IconRenderer.sprite.texture != null)
                Destroy(m_IconRenderer.sprite.texture);
        }
    }

    public void SetupAvatar(bool male, List<EquippedApparel> equips, System.Action<Sprite> callback = null)
    {
        //generate sprites for avatar and icon
        AvatarSpriteUtil.Instance.GenerateFullbodySprite(male, equips, spr =>
        {
            if (m_AvatarRenderer != null)
            {
                if (m_AvatarRenderer.sprite != null && m_AvatarRenderer.sprite.texture != null)
                    Destroy(m_AvatarRenderer.sprite.texture);

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
            {
                if (m_IconRenderer.sprite != null && m_IconRenderer.sprite.texture != null)
                    Destroy(m_IconRenderer.sprite.texture);

                m_IconRenderer.sprite = spr;
            }
            callback?.Invoke(spr);
        });
    }

    public void SetRingColor()
    {
        if (witchToken.degree < 0)
            m_SchoolColor = Utilities.Purple;
        else if (witchToken.degree == 0)
            m_SchoolColor = Utilities.Blue;
        else
            m_SchoolColor = new Color(0.97f, 0.67f, 0.18f, 1f);// Utilities.Orange;

        m_SchoolColor.a = m_EnergyRing.color.a;
        m_EnergyRing.color = m_SchoolColor;
    }

    public override void OnDespawn()
    {
        LeanTween.cancel(m_TweenId);

        IsShowingAvatar = false;
        IsShowingIcon = false;

        if (m_AvatarRenderer.sprite != null)
        {
            Destroy(m_AvatarRenderer.sprite.texture);
            m_AvatarRenderer.sprite = null;
        }

        if (m_IconRenderer.sprite != null)
        {
            Destroy(m_IconRenderer.sprite.texture);
            m_IconRenderer.sprite = null;
        }

        if (m_DeathIcon != null)
        {
            SpellcastingFX.DeathIconPool.Despawn(m_DeathIcon);
            m_DeathIcon = null;
        }

        if (m_ImmunityIcon != null)
        {
            SpellcastingFX.ImmunityIconPool.Despawn(m_ImmunityIcon);
            m_ImmunityIcon = null;
        }

        if (m_ChannelingFX != null)
        {
            SpellChanneling.DespawnFX(m_ChannelingFX);
            m_ChannelingFX = null;
        }

        base.OnDespawn();
    }

    public void AddImmunityFX()
    {
        if (m_ImmunityIcon == null)
        {
            m_ImmunityIcon = SpellcastingFX.ImmunityIconPool.Spawn();
            m_ImmunityIcon.SetParent(AvatarTransform);
            m_ImmunityIcon.localPosition = new Vector3(0, 0, -0.5f);
            m_ImmunityIcon.localScale = Vector3.one;
            m_ImmunityIcon.localRotation = Quaternion.identity;
            UpdateRenderers();
        }

        UpdateCharacterAlphaMul();
    }

    public void AddDeathFX()
    {
        if (m_DeathIcon != null)
        {
            UpdateCharacterAlphaMul();
            return;
        }

        m_DeathIcon = SpellcastingFX.DeathIconPool.Spawn();
        m_DeathIcon.SetParent(AvatarTransform);
        m_DeathIcon.localPosition = new Vector3(0, 0, -0.5f);
        m_DeathIcon.localScale = Vector3.one;
        m_DeathIcon.localRotation = Quaternion.identity;

        UpdateRenderers();
        UpdateCharacterAlphaMul();
    }

    public void RemoveImmunityFX()
    {
        if (m_ImmunityIcon == null)
        {
            UpdateCharacterAlphaMul();
            return;
        }

        SpellcastingFX.ImmunityIconPool.Despawn(m_ImmunityIcon);
        m_ImmunityIcon = null;

        UpdateRenderers();
        UpdateCharacterAlphaMul();
    }

    public void RemoveDeathFX()
    {
        if (m_DeathIcon != null)
        {
            SpellcastingFX.DeathIconPool.Despawn(m_DeathIcon);
            UpdateRenderers();
        }
        UpdateCharacterAlphaMul();
    }

    private void UpdateCharacterAlphaMul()
    {
        if (Token == null)
            return;

        float prevValue = m_CharacterAlphaMul;

        if (witchToken.energy <= 0 || witchToken.state == "dead")
            m_CharacterAlphaMul = 0.45f;
        else if (MarkerSpawner.IsTargetImmune(witchToken))
            m_CharacterAlphaMul = 0.38f;
        else
            m_CharacterAlphaMul = 1f;

        if (m_CharacterAlphaMul != prevValue)
            SetCharacterAlpha(AvatarAlpha, 1f);
    }

    public override void OnApplyStatusEffect(StatusEffect effect)
    {
        if (effect.HasStatus(SpellData.CHANNELING_STATUS) && m_ChannelingFX == null)
        {
            m_ChannelingFX = SpellChanneling.SpawnFX(this, witchToken);
            m_ChannelingFX.SetParent(AvatarTransform);
            m_ChannelingFX.localPosition = Vector3.zero;
            m_ChannelingFX.localScale = Vector3.one;

            ParticleSystem[] particles = m_ChannelingFX.GetComponentsInChildren<ParticleSystem>();
            particles[0].Play();
            particles[1].Play();
        }

        if (effect.HasStatus(SpellData.CHANNELED_STATUS))
        {
            if (m_ChannelingFX == null)
            {
                m_ChannelingFX = SpellChanneling.SpawnFX(this, witchToken);
                m_ChannelingFX.SetParent(AvatarTransform);
                m_ChannelingFX.localPosition = Vector3.zero;
                m_ChannelingFX.localScale = Vector3.one;
            }

            ParticleSystem[] particles = m_ChannelingFX.GetComponentsInChildren<ParticleSystem>();
            particles[0].Stop();
            particles[1].Stop();
        }
    }

    public override void OnExpireStatusEffect(StatusEffect effect)
    {
        if (effect.HasStatus(SpellData.CHANNELING_STATUS) || effect.HasStatus(SpellData.CHANNELED_STATUS))
        {
            if (m_ChannelingFX != null)
            {
                SpellChanneling.DespawnFX(m_ChannelingFX);
                m_ChannelingFX = null;
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Update nameplate")]
    private void DebugNameplate()
    {
        UpdateNameplate(m_DisplayName.preferredWidth);
    }
#endif
}
