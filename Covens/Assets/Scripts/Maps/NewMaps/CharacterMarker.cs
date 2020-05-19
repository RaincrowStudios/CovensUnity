using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using TMPro;
using System.Collections.Generic;

public abstract class CharacterMarker : MuskMarker
{
    private static int DEAD_STATE_ID = Animator.StringToHash("dead");
    private static int IMMUNE_STATE_ID = Animator.StringToHash("immune");
    private static int AVATAR_STATE_ID = Animator.StringToHash("avatar");
    private static int ICON_STATE_ID = Animator.StringToHash("icon");

    [Header("Character Marker")]
    [SerializeField] protected Transform m_AvatarGroup;
    [SerializeField] protected Transform m_IconGroup;
    [SerializeField] protected Transform m_Character;
    [SerializeField] protected SpriteRenderer m_IconRenderer;
    [Space]
    [SerializeField] protected SpriteRenderer m_NameBanner;
    [SerializeField] protected Transform m_StatsContainer;
    [Space]
    [SerializeField] protected SpriteRenderer m_EnergyRing;
    [SerializeField] protected SpriteRenderer m_EnergyRingEmpty;
    [Space]
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

    public SpriteRenderer EnergyRing => m_EnergyRing;

    private CharacterToken characterToken;
    private float m_EnergyFill = 0;
    private int m_EnergyRingTweenId;
    private Transform m_HexFX;
    private Transform m_SealFX;
    private Transform m_CovenBuffFX;
    private Transform m_ChannelingFX;

    //Elixirs effects
    private Dictionary<string, Transform> m_ElixirsEffect = new Dictionary<string, Transform>();

    protected abstract void SetupIcon();
    protected abstract void SetupAvatar();

    public override void Setup(Token data)
    {
        characterToken = data as CharacterToken;

        base.Setup(data);

        UpdateEnergy(0);

        SetStats();

        IsShowingAvatar = false;
        IsShowingIcon = false;

        //m_IconRenderer.sprite = null;
        //m_AvatarRenderer.sprite = null;

        if (characterToken.effects != null)
        {
            foreach (var effect in characterToken.effects)
                OnApplyStatusEffect(effect);
        }
    }

    public virtual void GetPortrait(System.Action<Sprite> callback)
    {
        callback?.Invoke(m_IconRenderer.sprite);
    }

    public override void EnablePortait()
    {
        if (IsShowingIcon)
            return;

        SetupIcon();

        IsShowingIcon = true;
        IsShowingAvatar = false;

        m_Animator.SetBool(AVATAR_STATE_ID, IsShowingAvatar);
        m_Animator.SetBool(ICON_STATE_ID, IsShowingIcon);
    }

    public override void EnableAvatar()
    {
        if (IsShowingAvatar)
            return;

        SetupAvatar();

        IsShowingAvatar = true;
        IsShowingIcon = false;

        m_Animator.SetBool(AVATAR_STATE_ID, IsShowingAvatar);
        m_Animator.SetBool(ICON_STATE_ID, IsShowingIcon);
    }

    public override void OnDespawn()
    {
        base.OnDespawn();

        //LeanTween.cancel(m_TweenId);
        LeanTween.cancel(m_EnergyRingTweenId);

        IsShowingAvatar = false;
        IsShowingIcon = false;

        m_AvatarRenderer.sprite = null;
        m_IconRenderer.sprite = null;

        m_Animator.SetBool("avatar", IsShowingAvatar);
        m_Animator.SetBool("icon", IsShowingIcon);
    }

    protected void UpdateAnimationState()
    {
        if (Token == null)
            return;

        bool dead = characterToken.energy <= 0 || characterToken.state == "dead";
        bool immune = MarkerSpawner.IsTargetImmune(characterToken);

        m_Animator.SetBool(DEAD_STATE_ID, dead);
        m_Animator.SetBool(IMMUNE_STATE_ID, immune);
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
            else if (effect.spell.Contains("elixir"))
            {
                if (!m_ElixirsEffect.ContainsKey(effect.spell))
                {
                    SimplePool<Transform> elixirEffect = StatusEffectFX.GetElixirEffect(effect.spell);

                    if (elixirEffect != null)
                    {
                        fx = StatusEffectFX.Spawn(elixirEffect);
                        m_ElixirsEffect.Add(effect.spell, fx);
                    }
                }
            }
        }

        if (effect.spell == "spell_covenBuff")
        {
            if (m_CovenBuffFX != null)
                StatusEffectFX.DespawnCovenBuff(m_CovenBuffFX);
            m_CovenBuffFX = fx = StatusEffectFX.SpawnCovenBuff(effect);
        }

        if (effect.HasStatus(SpellData.CHANNELING_STATUS))
        {
            if (m_ChannelingFX == null)
                fx = m_ChannelingFX = SpellChanneling.SpawnFX(this, characterToken);

            ParticleSystem[] particles = m_ChannelingFX.GetComponentsInChildren<ParticleSystem>();
            particles[0].Play();
            particles[1].Play();
        }

        if (effect.HasStatus(SpellData.CHANNELED_STATUS))
        {
            if (m_ChannelingFX == null)
                fx = m_ChannelingFX = SpellChanneling.SpawnFX(this, characterToken);

            ParticleSystem[] particles = m_ChannelingFX.GetComponentsInChildren<ParticleSystem>();
            particles[0].Stop();
            particles[1].Stop();
        }

        if (fx)
        {
            fx.SetParent(m_AvatarGroup);
            fx.localPosition = Vector3.zero;
            fx.localScale = Vector3.one;
        }
    }

    public override void OnExpireStatusEffect(StatusEffect effect)
    {
        base.OnExpireStatusEffect(effect);

        DespawnFX(effect.spell);

        //if (effect.HasStatus(SpellData.CHANNELING_STATUS) || effect.HasStatus(SpellData.CHANNELED_STATUS))
        //{
        //    if (m_ChannelingFX != null)
        //    {
        //        SpellChanneling.DespawnFX(m_ChannelingFX);
        //        m_ChannelingFX = null;
        //    }
        //}
    }

    private void DespawnFX(string id)
    {
        if (id == "spell_hex")
        {
            if (m_HexFX)
            {
                StatusEffectFX.DespawnHexFX(m_HexFX);
                m_HexFX = null;
            }
        }
        else if (id == "spell_seal")
        {
            if (m_SealFX)
            {
                StatusEffectFX.DespawnHexFX(m_SealFX);
                m_SealFX = null;
            }
        }
        else if (id == "spell_covenBuff")
        {
            if (m_CovenBuffFX)
            {
                StatusEffectFX.DespawnCovenBuff(m_CovenBuffFX);
                m_CovenBuffFX = null;
            }
        }
        else if (id == "spell_channeling")
        {
            if (m_ChannelingFX != null)
            {
                SpellChanneling.DespawnFX(m_ChannelingFX);
                m_ChannelingFX = null;
            }
        }
        else if (id.Contains("elixir"))
        {
            if (m_ElixirsEffect.ContainsKey(id))
            {
                SimplePool<Transform> elixirEffect = StatusEffectFX.GetElixirEffect(id);

                if (elixirEffect != null)
                {
                    Transform effect = m_ElixirsEffect[id];

                    elixirEffect.Despawn(effect);
                    m_ElixirsEffect.Remove(id);
                }
            }
        }
    }

    public override void UpdateEnergy(float time = 1f)
    {
        LeanTween.cancel(m_EnergyRingTweenId);

        m_EnergyFill = (float)(Token as CharacterToken).energy;
        m_EnergyFill /= (Token as CharacterToken).baseEnergy;

        if (time == 0)
            m_EnergyRing.color = new Color(1, 1, 1, m_EnergyFill);
        else
            m_EnergyRingTweenId = LeanTween.alpha(m_EnergyRing.gameObject, m_EnergyFill, time).uniqueId;
    }

    public override void UpdateNameplate(float preferredWidth)
    {
        Vector2 bannerSize = new Vector2(MapUtils.scale(2.2f, 9.5f, .86f, 8f, preferredWidth), m_NameBanner.size.y);
        m_NameBanner.size = bannerSize;

        if (m_StatsContainer == null)
            return;

        Vector3 statPos = new Vector3(
            -MapUtils.scale(0f, 3.6f, 2.2f, 9.5f, m_NameBanner.size.x),
            m_StatsContainer.localPosition.y,
            m_StatsContainer.localPosition.z
        );
        m_StatsContainer.localPosition = statPos;
    }

    public override void ScaleNamePlate(bool scaleUp, float time = 1)
    {
        LeanTween.scale(m_NameBanner.gameObject, scaleUp ? Vector3.one * 4.5f : Vector3.zero, time);
    }
}
