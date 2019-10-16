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

        if (effect.spell == "spell_hex")
        {

        }
        else if (effect.spell == "spell_seal")
        {

        }
    }

    public override void OnExpireStatusEffect(StatusEffect effect)
    {
        base.OnExpireStatusEffect(effect);

        if (effect.spell == "spell_hex")
        {

        }
        else if (effect.spell == "spell_seal")
        {

        }
    }
}
