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
    
    [SerializeField] private double m_latitude;
    [SerializeField] private double m_longitude;

    public WitchToken witchToken { get => m_Data as WitchToken; }

    private int m_TweenId;
    private Transform m_DeathIcon;
    private Transform m_ImmunityIcon;

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
        
        m_latitude = data.latitude;
        m_longitude = data.longitude;

        m_CharacterRenderers = new SpriteRenderer[] { m_AvatarRenderer };

        //if (IsShowingAvatar == false && IsShowingIcon == false)
        //{
        //    m_AvatarGroup.localScale = Vector3.zero;
        //    m_IconGroup.localScale = Vector3.zero;
        //}

        m_DisplayName.text = witchToken.displayName;
        SetStats(witchToken.level);
        UpdateNameplate(m_DisplayName.preferredWidth);
        SetRingAmount();
        UpdateEnergy(witchToken.energy, witchToken.baseEnergy);

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

    public override void SetStats(int level)
    {
        if (m_Level == null)
            return;
        
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
        if (witchToken.degree < 0)
            color = Utilities.Purple;
        else if (witchToken.degree == 0)
            color = Utilities.Blue;
        else
            color = new Color(0.97f, 0.67f, 0.18f, 1f);// Utilities.Orange;

        color.a = characterAlpha * alpha;
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

        base.OnDespawn();
    }

    public void AddImmunityFX()
    {
        if (m_ImmunityIcon == null)
        {
            m_ImmunityIcon = SpellcastingFX.ImmunityIconPool.Spawn();
            LeanTween.alpha(m_ImmunityIcon.GetChild(0).gameObject, 0f, 0.01f);
            m_ImmunityIcon.SetParent(characterTransform);
            m_ImmunityIcon.localPosition = new Vector3(0, 0, -0.5f);
            m_ImmunityIcon.localScale = Vector3.one;
            m_ImmunityIcon.localRotation = Quaternion.identity;
            LeanTween.alpha(m_ImmunityIcon.GetChild(0).gameObject, 1f, 0.5f);
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
        m_DeathIcon.SetParent(characterTransform);
        m_DeathIcon.localPosition = new Vector3(0, 0, -0.5f);
        m_DeathIcon.localScale = Vector3.one;
        m_DeathIcon.localRotation = Quaternion.identity;

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
        UpdateCharacterAlphaMul();
    }

    public void RemoveDeathFX()
    {
        if (m_DeathIcon == null)
        {
            UpdateCharacterAlphaMul();
            return;
        }

        SpellcastingFX.DeathIconPool.Despawn(m_DeathIcon);
        UpdateCharacterAlphaMul();
    }

    private void UpdateCharacterAlphaMul()
    {
        if (m_Data == null)
            return;

        float prevValue = m_CharacterAlphaMul;

        if (witchToken.energy <= 0 || witchToken.state == "dead")
            m_CharacterAlphaMul = 0.45f;
        else if (MarkerSpawner.IsTargetImmune(witchToken))
            m_CharacterAlphaMul = 0.38f;
        else
            m_CharacterAlphaMul = 1f;

        m_Renderers = GetComponentsInChildren<SpriteRenderer>(true);
        m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);

        if (m_CharacterAlphaMul != prevValue)
            SetCharacterAlpha(characterAlpha, 1f);
    }
}
