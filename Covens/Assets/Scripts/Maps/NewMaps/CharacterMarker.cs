using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using TMPro;

public abstract class CharacterMarker : MuskMarker
{
    [Header("Character Marker")]
    [SerializeField] protected Transform m_AvatarGroup;
    [SerializeField] protected Transform m_IconGroup;
    [SerializeField] protected Transform m_Character;
    [SerializeField] protected SpriteRenderer m_IconRenderer;

    [SerializeField] protected TextMeshPro m_DisplayName;
    [SerializeField] protected TextMeshPro m_Level;
    
    public override Transform AvatarTransform
    {
        get
        {
            if (IsShowingIcon)
                return m_IconRenderer.transform;
            return m_Character;
        }
    }

    private int m_TweenId;
    private Transform m_HexFX;
    private Transform m_SealFX;
    private Transform m_CovenBuffFX;

    protected abstract void SetupIcon();
    protected abstract void SetupAvatar();

    public override void Setup(Token data)
    {
        base.Setup(data);

        CharacterToken character = data as CharacterToken;

        SetStats();
        UpdateEnergy();
        IsShowingAvatar = false;
        IsShowingIcon = false;
        m_IconRenderer.sprite = null;
        m_AvatarRenderer.sprite = null;
        
        if (character.effects != null)
        {
            foreach (var effect in character.effects)
                OnApplyStatusEffect(effect);
        }
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

    public override void OnDespawn()
    {
        base.OnDespawn();
        LeanTween.cancel(m_TweenId);

        IsShowingAvatar = false;
        IsShowingIcon = false;

        m_AvatarRenderer.sprite = null;
        m_IconRenderer.sprite = null;
    }

    public override void OnApplyStatusEffect(StatusEffect effect)
    {
        base.OnApplyStatusEffect(effect);

        Transform fx = null;

        if (effect.stack == effect.stackable)
        {
            if (effect.spell == "spell_hex")
            {
                if (m_HexFX == null)
                    m_HexFX = fx = StatusEffectFX.SpawnHexFX();
            }
            else if (effect.spell == "spell_seal")
            {
                if (m_SealFX == null)
                    m_SealFX = fx = StatusEffectFX.SpawnSealFX();
            }
        }

        if (effect.spell == "spell_covenBuff")
        {
            if (m_CovenBuffFX != null)
                StatusEffectFX.DespawnCovenBuff(m_CovenBuffFX);
            m_CovenBuffFX = fx = StatusEffectFX.SpawnCovenBuff(effect);
        }

        if (fx)
        {
            fx.SetParent(AvatarTransform);
            fx.localPosition = Vector3.zero;
            fx.localScale = Vector3.one;
        }
    }

    public override void OnExpireStatusEffect(StatusEffect effect)
    {
        base.OnExpireStatusEffect(effect);

        if (effect.spell == "spell_hex")
        {
            if (m_HexFX)
            {
                StatusEffectFX.DespawnHexFX(m_HexFX);
                m_HexFX = null;
            }
        }
        else if (effect.spell == "spell_seal")
        {
            if (m_SealFX)
            {
                StatusEffectFX.DespawnHexFX(m_SealFX);
                m_SealFX = null;
            }
        }
        else if (effect.spell == "spell_covenBuff")
        {
            if (m_CovenBuffFX)
                StatusEffectFX.DespawnCovenBuff(m_CovenBuffFX);
        }
    }
}
