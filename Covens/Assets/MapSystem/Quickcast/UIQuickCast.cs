using BattleArena;
using Raincrow;
using Raincrow.BattleArena.Controller;
using Raincrow.GameEventResponses;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuickCast : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private LayoutGroup m_SpellContainer;
    [SerializeField] private UIQuickcastButton m_ButtonPrefab;
    [SerializeField] private Button m_MoreSpells;
    [SerializeField] private Button m_Button;
    [SerializeField] private Canvas m_ContainerCanvas;

    [Header("Anim")]
    [SerializeField] private RectTransform m_RectTransform;
    [SerializeField] private CanvasGroup m_ContentPanel;

    [Header("others")]
    [SerializeField] private UIQuickCastPicker m_Picker;

    private static UIQuickCast m_Instance;
    private static event System.Action m_OnClose;

    private List<UIQuickcastButton> m_Buttons = new List<UIQuickcastButton>();
    private static IMarker m_Target;
    private static CharacterMarkerData m_TargetData;
    
    public static bool IsOpen { get; private set; }
    public static IMarker target => m_Target != null ? m_Target : PlayerManager.marker;
    public static CharacterMarkerData targetData => m_TargetData != null ? m_TargetData : PlayerDataManager.playerData;

    private int m_AnimTweenId;
    private bool m_WasOpen = false;

    public static void Close()
    {
    }

    public static void Hide()
    {
        m_Instance?._Hide();
    }

    public static void UpdateTarget(IMarker marker, CharacterMarkerData details)
    {
        if (m_Instance == null)
        {
            m_Target = marker;
            m_TargetData = details;
            return;
        }
        m_Instance._UpdateTarget(marker, details);
        //Spellcasting.SpellState canCast = Spellcasting.CanCast((SpellData)null, marker, data);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="go">The gameobject that will be added along the quickcast buttons</param>
    /// <param name="index">The sibling index the gameobject should be added to</param>
    /// <param name="onClose">This will be called when the QuickCast menu is closed. Use this to remove your gameObject.</param>
    public static void AddItem(GameObject go, int index, System.Action onClose)
    {
        if (m_Instance == null)
        {
            Debug.LogError("UIQuickCast not initialized");
            return;
        }

        go.transform.SetParent(m_Instance.m_SpellContainer.transform);
        go.transform.localScale = Vector3.one;
        go.transform.SetSiblingIndex(index);
        m_OnClose += onClose;
    }

    public static void EnableQuickcastButtons(bool enable)
    {
        if (m_Instance == null)
            return;

        for (int i = 0; i < m_Instance.m_Buttons.Count; i++)
        {
            m_Instance.m_Buttons[i].Show(enable);
        }

        m_Instance.m_MoreSpells.gameObject.SetActive(enable);
    }

    private void Awake()
    {
        m_Instance = this;
        m_ButtonPrefab.gameObject.SetActive(false);
        m_ContainerCanvas.enabled = false;
        m_MoreSpells.onClick.AddListener(OnClickMoreSpells);
        
        m_Button.onClick.AddListener(() =>
        {
            if (IsOpen)
                _Hide();
            else
                _Show();
        });

        UISpiritInfo.OnOpen += OnSelectTarget;
        UISpiritInfo.OnClose += OnUnselectTarget;

        UIPlayerInfo.OnOpen += OnSelectTarget;
        UIPlayerInfo.OnClose += OnUnselectTarget;

        UIWorldBoss.OnSelectBoss += OnSelectTarget;
        UIWorldBoss.OnUnselectBoss += OnUnselectTarget;
    }

    private void Start()
    {
        _Hide();
    }

    private void OnSelectTarget()
    {
        m_Button.interactable = false;
        _Show();
    }

    private void OnUnselectTarget()
    {
        m_Button.interactable = true;
        UpdateTarget(null, null);

        if (m_WasOpen == false)
            _Hide();
    }


    private void _Show()
    {
        m_WasOpen = IsOpen;

        if (IsOpen)
            return;

        IsOpen = true;

        int quickcastCount = 4;
        for (int i = m_Buttons.Count; i < quickcastCount; i++)
        {
            UIQuickcastButton aux = Instantiate(m_ButtonPrefab, m_SpellContainer.transform);
            aux.Setup(i, () => OnClickSpell(aux), () => OnHoldSpell(aux));
            aux.Hightlight(false);
            aux.transform.localScale = Vector3.one;
            aux.gameObject.SetActive(true);
            m_Buttons.Add(aux);
        }
        m_MoreSpells.transform.SetAsLastSibling();

        UpdateTarget(target, targetData);

        SpellCastHandler.OnPlayerTargeted += _OnPlayerAttacked;
        MarkerSpawner.Instance.OnImmunityChange += _OnImmunityChange;
        SpellCastHandler.OnApplyEffect += _OnStatusEffectApplied;
        PlayerConditionManager.OnPlayerExpireStatusEffect += _OnStatusEffectExpired;
        BanishManager.OnBanished += _OnBanished;
        OnMapEnergyChange.OnEnergyChange += _OnEnergyChange;
        RemoveTokenHandler.OnTokenRemove += _OnMapTokenRemove;

        AnimOpen();
    }

    private void _Hide()
    {
        SpellCastHandler.OnPlayerTargeted -= _OnPlayerAttacked;
        MarkerSpawner.Instance.OnImmunityChange -= _OnImmunityChange;
        SpellCastHandler.OnApplyEffect -= _OnStatusEffectApplied;
        PlayerConditionManager.OnPlayerExpireStatusEffect -= _OnStatusEffectExpired;
        BanishManager.OnBanished -= _OnBanished;
        OnMapEnergyChange.OnEnergyChange -= _OnEnergyChange;
        RemoveTokenHandler.OnTokenRemove -= _OnMapTokenRemove;

        IsOpen = false;
        AnimHide();

        if (m_Picker.IsOpen)
            m_Picker.Hide();

        m_OnClose?.Invoke();
        m_OnClose = null;
    }

    private void _Hide(bool hide)
    {
        if (IsOpen == false)
            return;

        if (hide)
        {
            m_Picker.Hide();
            AnimHide();
        }
        else
        {
            AnimOpen();
        }

        UpdateTarget(target, targetData);
    }

    private void AnimOpen()
    {
        LeanTween.cancel(m_AnimTweenId);

        m_Button.image.transform.localEulerAngles = new Vector3(0, 0, -90);

        m_AnimTweenId = LeanTween.value(m_ContentPanel.alpha, 1, 0.5f)
            .setOnUpdate((float t) =>
            {
                m_RectTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(-200, 0, t));
                m_ContentPanel.alpha = t;
            })
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
            })
            .uniqueId;

        m_ContainerCanvas.enabled = true;
    }

    private void AnimHide()
    {
        LeanTween.cancel(m_AnimTweenId);

        m_Button.image.transform.localEulerAngles = new Vector3(0, 0, 90);

        m_AnimTweenId = LeanTween.value(m_ContentPanel.alpha, 0, 0.3f)
            .setOnUpdate((float t) =>
            {
                m_RectTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(-200, 0, t));
                m_ContentPanel.alpha = t;
            })
            .setOnComplete(() =>
            {
                m_ContainerCanvas.enabled = false;
            })
            .setEaseOutCubic()
            .uniqueId;
    }

    private void _UpdateTarget(IMarker marker, CharacterMarkerData data)
    {
        m_Target = marker;
        m_TargetData = data;

        foreach (UIQuickcastButton item in m_Buttons)
            item.UpdateCanCast(target, targetData);
    }

    private void OnClickSpell(UIQuickcastButton button)
    {
        if (m_Picker.IsOpen)
        {
            OnHoldSpell(button);
            return;
        }

        if (string.IsNullOrEmpty(button.Spell))
        {
            UIGlobalPopup.ShowPopUp(null, LocalizeLookUp.GetText("quickcast_tap_hold"));//"hold to set a spell");
            return;
        }

        if (button.CastStatus != Spellcasting.SpellState.CanCast)
            return;

        SpellData spell = DownloadedAssets.GetSpell(button.Spell);

        this._Hide(true);

        Spellcasting.CastSpell(spell, target, new List<spellIngredientsData>(),
            (result) => this._Hide(false),
            () => this._Hide(false));
    }

    private void OnHoldSpell(UIQuickcastButton button)
    {
        foreach (UIQuickcastButton _item in m_Buttons)
        {
            _item.Hightlight(_item == button);
        }

        m_Picker.Show(
            button.Spell,
            spell =>
            {
                PlayerManager.SetQuickcastSpell(button.QuickcastIndex, spell);

                button.Setup(
                    button.QuickcastIndex,
                    () => OnClickSpell(button),
                    () => OnHoldSpell(button));

                button.UpdateCanCast(target, targetData);
            },
            () => button.Hightlight(false)
        );
    }

    private void OnClickMoreSpells()
    {
        if (target == null || targetData == null)
            return;

        UIMainScreens.PushEventAnalyticUI(UIMainScreens.Map, UIMainScreens.SpellBook);

        this._Hide(true);
        
        UISpellcastBook.Open(
            targetData,
            target,
            PlayerDataManager.playerData.UnlockedSpells,
            (spell, ingredients) =>
            {
                Spellcasting.CastSpell(spell, m_Target, ingredients,
                    (result) => this._Hide(false),
                    () => this._Hide(false)
                );
            },
            () => this._Hide(false),
            () => this._Hide(false)
        );
    }

    public void OnClickChallenge()
    {
        ChallengeManager.Challenge(target);
    }

    //GAME EVENTS

    private void _OnPlayerAttacked(string caster, SpellData spell, Raincrow.GameEventResponses.SpellCastHandler.Result result)
    {
        UpdateTarget(target, targetData);
    }
    
    private void _OnStatusEffectExpired(StatusEffect effect)
    {
        UpdateTarget(target, targetData);
    }

    private void _OnStatusEffectApplied(string instance, string caster, StatusEffect statusEffect)
    {
        if (instance != target.Token.instance)
            return;

        UpdateTarget(target, targetData);
    }

    private void _OnImmunityChange(string _caster, string _target, bool immune)
    {
        if (_caster == PlayerDataManager.playerData.instance && _target == target.Token.instance)
            UpdateTarget(target, targetData);
        else if (_target == PlayerDataManager.playerData.instance && _caster == target.Token.instance)
            UpdateTarget(target, targetData);
    }

    private void _OnEnergyChange(string instance, int newEnergy)
    {
        if (instance != target.Token.instance)
            return;

        UpdateTarget(target, targetData);
    }

    private void _OnBanished()
    {
        UpdateTarget(null, null);
    }

    private void _OnPlayerDead()
    {
        UpdateTarget(null, null);
    }

    private void _OnMapTokenRemove(string instance)
    {
        if (instance != target.Token.instance)
            return;

        UpdateTarget(null, null);
    }
}
