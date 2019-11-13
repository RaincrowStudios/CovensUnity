#define LIMIT_GENERATED_SPRITES
using UnityEngine;
using System.Collections;
using Raincrow.Maps;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class WitchMarker : CharacterMarker
{
    [Header("Witch Marker")]
    [SerializeField] private Sprite m_MaleMannequin;
    [SerializeField] private Sprite m_FemaleMannequin;

    [SerializeField] private Sprite m_MalePortrait;
    [SerializeField] private Sprite m_FemalePortrait;

    public WitchToken witchToken { get => Token as WitchToken; }

    public override string Name => witchToken.displayName;
    
    private Transform m_DeathIcon;
    private Transform m_ImmunityIcon;
    private Transform m_ChannelingFX;

    private int m_AvatarColorTweenId;
    
    public void GetPortrait(System.Action<Sprite> callback)
    {
        //if already despawned, return null
        if (Token == null)
        {
            callback?.Invoke(null);
            return;
        }

        //portrait was already generated
        if (GeneratedPortraits.ContainsKey(Token.Id) && GeneratedPortraits[Token.Id] != null)
        {
            callback.Invoke(GeneratedPortraits[Token.Id]);
            return;
        }

        //generate the portrait
        GeneratePortrait(callback);
    }

    public static int WITCH_AVATAR_LIMIT
    {
        get => PlayerPrefs.GetInt("Settings.WitchAvatarLimit", 50);
        set => PlayerPrefs.SetInt("Settings.WitchAvatarLimit", value);
    }

    public static int WITCH_PORTRAIT_LIMIT
    {
        get => PlayerPrefs.GetInt("Settings.WitchPortraitLimit", 50);
        set => PlayerPrefs.SetInt("Settings.WitchPortraitLimit", value);
    }

    private static Dictionary<string, Sprite> GeneratedAvatars = new Dictionary<string, Sprite>();
    private static Dictionary<string, Sprite> GeneratedPortraits = new Dictionary<string, Sprite>();

    public static int GeneratedAvatarCount => GeneratedAvatars.Count;
    public static int GeneratedPortraitCount => GeneratedPortraits.Count;

    public override void Setup(Token data)
    {
        base.Setup(data);

        m_AvatarRenderer.transform.localPosition = Vector3.zero;

        m_DisplayName.text = witchToken.displayName;
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
    }
      

    public override void SetStats()
    {
        if (m_Level == null)
            return;

        m_Level.text = witchToken.level.ToString();
    }

    public void UpdateEquips(List<EquippedApparel> equips)
    {
        //update equip list
        witchToken.equipped = equips;

        //destroy old sprites
        DestroyGeneratedAvatar();
        DestroyGeneratedPortrait();

        //generate new sprites
        if (inMapView)
        {
            if (IsShowingAvatar)
                GenerateAvatar();
            if (IsShowingIcon)
                GeneratePortrait();
        }
    }

    protected override void SetupAvatar()
    {
        LeanTween.cancel(m_AvatarColorTweenId);
        m_AvatarRenderer.sprite = witchToken.male ? m_MaleMannequin : m_FemaleMannequin;
        m_AvatarRenderer.color = new Color(0, 0, 0, m_AvatarRenderer.color.a);

#if LIMIT_GENERATED_SPRITES
        if (GeneratedAvatars.Count >= WITCH_AVATAR_LIMIT)
        {
            return;
        }
#endif

        GenerateAvatar();
    }

    protected override void SetupIcon()
    {
        m_IconRenderer.sprite = witchToken.male ? m_MalePortrait : m_FemalePortrait;

#if LIMIT_GENERATED_SPRITES
        if (GeneratedPortraits.Count >= WITCH_PORTRAIT_LIMIT)
        {
            return;
        }
#endif

        GeneratePortrait();
    }

    public void GenerateAvatar(System.Action<Sprite> callback = null, bool ignoreQueue = false)
    {
        //check if it already exists
        if (GeneratedAvatars.ContainsKey(Token.Id) && GeneratedAvatars[Token.Id] != null)
        {
            m_AvatarRenderer.sprite = GeneratedAvatars[Token.Id];

            //fade avatar
            LeanTween.cancel(m_AvatarColorTweenId);
            m_AvatarColorTweenId = LeanTween.value(m_AvatarRenderer.color.r, 1, 1f)
              .setOnUpdate((float t) => m_AvatarRenderer.color = new Color(t, t, t, m_AvatarRenderer.color.a))
              .uniqueId;

            callback?.Invoke(m_AvatarRenderer.sprite);
            return;
        }

        bool male = witchToken.male;
        List<EquippedApparel> equips = witchToken.equipped;
        GeneratedAvatars[Token.Id] = null;

        //generate without using the avatar queue
        if (ignoreQueue)
        {
            AvatarSpriteUtil.Instance.GenerateAvatar(
                male,
                equips,
                spr => OnGenerateAvatar(spr, callback));
        }
        //use the avatar queue, which is aborted when landing on a new area
        else
        {
            AvatarSpriteUtil.Instance.AddToAvatarQueue(this, (spr) => OnGenerateAvatar(spr, callback));
        }
    }

    public void GeneratePortrait(System.Action<Sprite> callback = null, bool ignoreQueue = false)
    {
        if (GeneratedPortraits.ContainsKey(Token.Id) && GeneratedPortraits[Token.Id] != null)
        {
            m_IconRenderer.sprite = GeneratedPortraits[Token.Id];
            callback?.Invoke(m_IconRenderer.sprite);
            return;
        }

        bool male = witchToken.male;
        List<EquippedApparel> equips = witchToken.equipped;
        GeneratedPortraits[Token.Id] = null;

        if (ignoreQueue)
        {
            AvatarSpriteUtil.Instance.GeneratePortrait(
                male,
                equips,
                spr => OnGeneratedPortrait(spr, callback));
        }
        else
        {
            AvatarSpriteUtil.Instance.AddToPortraitQueue(this, spr => OnGeneratedPortrait(spr, callback));
        }
    }

    private void OnGenerateAvatar(Sprite spr, System.Action<Sprite> callback)
    {
        if (Token != null)
        {
            //destroy old avatar in case the generation started without destroying it
            DestroyGeneratedAvatar();
            //save it
            GeneratedAvatars[Token.Id] = spr;
            m_AvatarRenderer.sprite = spr;
            //fade avatar
            LeanTween.cancel(m_AvatarColorTweenId);
            m_AvatarColorTweenId = LeanTween.value(m_AvatarRenderer.color.r, 1, 1f)
              .setOnUpdate((float t) => m_AvatarRenderer.color = new Color(t, t, t, m_AvatarRenderer.color.a))
              .uniqueId;

            callback?.Invoke(spr);
        }
        else
        {
            Destroy(spr.texture);
            callback?.Invoke(null);
        }
    }

    private void OnGeneratedPortrait(Sprite spr, System.Action<Sprite> callback)
    {
        if (Token != null)
        {
            DestroyGeneratedPortrait();
            GeneratedPortraits[Token.Id] = spr;
            m_IconRenderer.sprite = spr;
            callback?.Invoke(spr);
        }
        else
        {
            Destroy(spr.texture);
            callback?.Invoke(null);
        }
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
        LeanTween.cancel(m_AvatarColorTweenId);

        DestroyGeneratedAvatar();
        DestroyGeneratedPortrait();

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
        base.OnApplyStatusEffect(effect);
        
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
                m_ChannelingFX.SetParent(m_AvatarGroup);
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
        base.OnExpireStatusEffect(effect);

        if (effect.HasStatus(SpellData.CHANNELING_STATUS) || effect.HasStatus(SpellData.CHANNELED_STATUS))
        {
            if (m_ChannelingFX != null)
            {
                SpellChanneling.DespawnFX(m_ChannelingFX);
                m_ChannelingFX = null;
            }
        }
    }

    public override void OnEnterMapView()
    {
        base.OnEnterMapView();

        if (IsShowingAvatar && m_AvatarRenderer.sprite == null)
            SetupAvatar();
        if (IsShowingIcon && m_IconRenderer.sprite == null)
            SetupIcon();
    }

    public override void OnLeaveMapView()
    {
        base.OnLeaveMapView();

        if (GeneratedAvatarCount >= WITCH_AVATAR_LIMIT/2)
            DestroyGeneratedAvatar();

        if (GeneratedPortraitCount >= WITCH_PORTRAIT_LIMIT/2)
            DestroyGeneratedPortrait();

        IsShowingAvatar = false;
        IsShowingIcon = false;
    }

    private void DestroyGeneratedAvatar()
    {
        if (GeneratedAvatars.ContainsKey(Token.Id) && GeneratedAvatars[Token.Id] != null)
        {
            Sprite spr = GeneratedAvatars[Token.Id];
            Destroy(spr.texture);
            Destroy(spr);
        }

        GeneratedAvatars.Remove(Token.Id);
        m_AvatarRenderer.sprite = null;
    }

    private void DestroyGeneratedPortrait()
    {
        if (GeneratedPortraits.ContainsKey(Token.Id) && GeneratedPortraits[Token.Id] != null)
        {
            Sprite spr = GeneratedPortraits[Token.Id];
            Destroy(spr.texture);
            Destroy(spr);
        }

        GeneratedPortraits.Remove(Token.Id);
        m_IconRenderer.sprite = null;
    }

#if UNITY_EDITOR
    [ContextMenu("Update nameplate")]
    private void DebugNameplate()
    {
        UpdateNameplate(m_DisplayName.preferredWidth);
    }
#endif
}
