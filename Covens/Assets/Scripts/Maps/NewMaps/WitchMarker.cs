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

    [SerializeField] private SpriteRenderer m_DeathIcon;
    [SerializeField] private SpriteRenderer m_ImmuneIcon;
    [SerializeField] private SpriteRenderer _battleIcon;

    public WitchToken witchToken;

    public override string Name => witchToken.displayName;
    
    private int m_AvatarColorTweenId;

    public override void GetPortrait(System.Action<Sprite> callback)
    {
        //if already despawned, return null
        if (Token == null)
        {
            callback?.Invoke(null);
            return;
        }

        //generate the portrait
        GeneratePortrait(callback, true);
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

    public static bool DisableSpriteGeneration => MarkerManagerAPI.WitchCount > WITCH_AVATAR_LIMIT * 2;

    private static Dictionary<string, Sprite> GeneratedAvatars = new Dictionary<string, Sprite>();
    private static Dictionary<string, Sprite> GeneratedPortraits = new Dictionary<string, Sprite>();

    public static int GeneratedAvatarCount => GeneratedAvatars.Count;
    public static int GeneratedPortraitCount => GeneratedPortraits.Count;

    public override void Setup(Token data)
    {
        witchToken = data as WitchToken;

        base.Setup(data);

        m_AvatarRenderer.transform.localPosition = Vector3.zero;

        m_DisplayName.text = witchToken.displayName;
        UpdateNameplate(m_DisplayName.preferredWidth);
        SetRingColor();

        //set immunity icon
        if (MarkerSpawner.IsTargetImmune(witchToken))
            OnAddImmunity();
        else
            OnRemoveImmunity();

        //set death icon
        if (witchToken.state == "dead" || witchToken.energy <= 0)
            OnDeath();
        else
            OnRevive();

        if (_battleIcon != null)
        {
            bool insideBattle = witchToken.insideBattle;
            _battleIcon.gameObject.SetActive(insideBattle);
        }        
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
        if (inMapView && !DisableSpriteGeneration)
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

        if (DisableSpriteGeneration)
            return;

        if (GeneratedAvatars.Count >= WITCH_AVATAR_LIMIT && !GeneratedAvatars.ContainsKey(Token.Id))
            return;
#endif

        GenerateAvatar();
    }

    protected override void SetupIcon()
    {
        m_IconRenderer.sprite = witchToken.male ? m_MalePortrait : m_FemalePortrait;

#if LIMIT_GENERATED_SPRITES

        if (DisableSpriteGeneration)
            return;

        if (GeneratedPortraits.Count >= WITCH_PORTRAIT_LIMIT && !GeneratedPortraits.ContainsKey(Token.Id))
            return;
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
        if (MarkerSpawner.GetMarker(Token.Id) != null)
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
        else //the gameobject was despawned
        {
            Destroy(spr.texture);
            callback?.Invoke(null);
        }
    }

    private void OnGeneratedPortrait(Sprite spr, System.Action<Sprite> callback)
    {
        if (MarkerSpawner.GetMarker(Token.Id) != null)
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

        //dont destroy in case its being used by another marker
        if (MarkerSpawner.GetMarker(Token.Id) == null)
        {
            DestroyGeneratedAvatar();
            DestroyGeneratedPortrait();
        }

        base.OnDespawn();
    }

    public void OnAddImmunity()
    {
        m_ImmuneIcon.gameObject.SetActive(true);
        UpdateAnimationState();
    }

    public void OnDeath()
    {
        m_DeathIcon.gameObject.SetActive(true);
        UpdateAnimationState();
    }

    public void OnRemoveImmunity()
    {
        m_ImmuneIcon.gameObject.SetActive(false);
        UpdateAnimationState();
    }

    public void OnRevive()
    {
        m_DeathIcon.gameObject.SetActive(false);
        UpdateAnimationState();
    }

    //public override void OnApplyStatusEffect(StatusEffect effect)
    //{
    //    base.OnApplyStatusEffect(effect);

        
    //}

    //public override void OnExpireStatusEffect(StatusEffect effect)
    //{
    //    base.OnExpireStatusEffect(effect);

    //}

    public override void OnEnterMapView()
    {
        base.OnEnterMapView();

        if (IsShowingAvatar)
            SetupAvatar();
        if (IsShowingIcon)
            SetupIcon();
    }

    public override void OnLeaveMapView()
    {
        base.OnLeaveMapView();

#if LIMIT_GENERATED_SPRITES
        DestroyGeneratedAvatar();
        DestroyGeneratedPortrait();
#endif

        IsShowingAvatar = false;
        IsShowingIcon = false;

        AvatarSpriteUtil.Instance.RemoveFromAvatarQueue(this);
        AvatarSpriteUtil.Instance.RemoveFromPortraitQueue(this);
    }

    public void DestroyGeneratedAvatar()
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

    public void DestroyGeneratedPortrait()
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
