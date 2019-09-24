using Raincrow;
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
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private LayoutGroup m_SpellContainer;
    [SerializeField] private UIQuickcastButton m_ButtonPrefab;
    [SerializeField] private Button m_MoreSpells;
    [SerializeField] private CanvasGroup m_Panel;

    [Header("others")]
    [SerializeField] private UIQuickCastPicker m_Picker;

    private static UIQuickCast m_Instance;
    private static event System.Action m_OnClose;
    public static bool IsOpen { get; private set; }

    private List<UIQuickcastButton> m_Buttons = new List<UIQuickcastButton>();
    private static IMarker m_Target;
    private static CharacterMarkerData m_TargetData;
    private string m_PreviousMarker = "";

    private int m_AnimTweenId;

    public static void Open(System.Action onLoaded = null)
    {
        m_Target = null;
        m_TargetData = null;

        if (m_Instance != null)
        {
            m_Instance._Open();
            onLoaded?.Invoke();
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.QUICKCAST,
                UnityEngine.SceneManagement.LoadSceneMode.Additive,
                null,
                () =>
                {
                    m_Instance._Open();
                    onLoaded?.Invoke();
                    LoadingOverlay.Hide();
                });
        }
    }

    public static void Close()
    {
        m_Instance._Close();
    }

    public static void UpdateCanCast(IMarker marker, CharacterMarkerData details)
    {
        if (m_Instance == null)
        {
            m_Target = marker;
            m_TargetData = details;
            return;
        }
        m_Instance._UpdateCanCast(marker, details);
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
        m_Canvas.enabled = false;
        m_ButtonPrefab.gameObject.SetActive(false);
        m_MoreSpells.onClick.AddListener(OnClickMoreSpells);

        DownloadedAssets.OnWillUnloadAssets += DownloadedAssets_OnWillUnloadAssets;
    }

    private void DownloadedAssets_OnWillUnloadAssets()
    {
        if (IsOpen)
            return;

        LeanTween.cancel(m_AnimTweenId);

        DownloadedAssets.OnWillUnloadAssets -= DownloadedAssets_OnWillUnloadAssets;
        SceneManager.UnloadScene(SceneManager.Scene.QUICKCAST, null, null);
    }


    private void _Open()
    {
        //in case the marker was rmeoved while loading the scene
        if (m_Target != null)
        {
            if (MarkerSpawner.GetMarker(m_Target.Token.Id) == null)
            {
                _Close();
                return;
            }
        }

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
            m_MoreSpells.transform.SetAsLastSibling();
        }

        UpdateCanCast(m_Target, m_TargetData);

        SpellCastHandler.OnPlayerTargeted += _OnPlayerAttacked;
        MarkerSpawner.OnImmunityChange += _OnImmunityChange;
        SpellCastHandler.OnApplyStatusEffect += _OnStatusEffectApplied;
        ConditionManager.OnPlayerExpireStatusEffect += _OnStatusEffectExpired;
        BanishManager.OnBanished += _OnBanished;
        OnMapEnergyChange.OnEnergyChange += _OnEnergyChange;
        RemoveTokenHandler.OnTokenRemove += _OnMapTokenRemove;

        AnimOpen();
    }

    private void _Close()
    {
        SpellCastHandler.OnPlayerTargeted -= _OnPlayerAttacked;
        MarkerSpawner.OnImmunityChange -= _OnImmunityChange;
        SpellCastHandler.OnApplyStatusEffect -= _OnStatusEffectApplied;
        ConditionManager.OnPlayerExpireStatusEffect -= _OnStatusEffectExpired;
        BanishManager.OnBanished -= _OnBanished;
        OnMapEnergyChange.OnEnergyChange -= _OnEnergyChange;
        RemoveTokenHandler.OnTokenRemove -= _OnMapTokenRemove;

        IsOpen = false;
        AnimHide();
        try
        {
            m_OnClose?.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.InnerException + "\n" + e.StackTrace);
        }
        m_OnClose = null;
        m_Target = null;
        m_TargetData = null;
    }

    private void _Hide(bool hide)
    {
        if (IsOpen == false)
            return;

        if (hide)
            AnimHide();
        else
            AnimOpen();

        UpdateCanCast(m_Target, m_TargetData);
    }

    private void AnimOpen()
    {
        LeanTween.cancel(m_AnimTweenId);

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        //m_AnimTweenId = LeanTween.moveLocalY(m_SpellContainer.gameObject, 0f, 0.6f)
        m_AnimTweenId = LeanTween.value(m_Panel.alpha, 1, 0.6f)
            .setOnUpdate((float t) =>
            {
                m_Panel.transform.localPosition = new Vector3(0, Mathf.Lerp(-172, 0, t), 0);
                m_Panel.alpha = t;
            })
            .setEaseOutCubic()
            .uniqueId;
    }

    private void AnimHide()
    {
        LeanTween.cancel(m_AnimTweenId);

        m_InputRaycaster.enabled = false;

        //m_AnimTweenId = LeanTween.moveLocalY(m_SpellContainer.gameObject, -138f, 0.4f)
        m_AnimTweenId = LeanTween.value(m_Panel.alpha, 0, 0.4f)
            .setOnUpdate((float t) =>
            {
                m_Panel.transform.localPosition = new Vector3(0, Mathf.Lerp(-172, 0, t), 0);
                m_Panel.alpha = t;
            })
            .setOnComplete(() => m_Canvas.enabled = false)
            .setEaseOutCubic()
            .uniqueId;
    }

    private void _UpdateCanCast(IMarker marker, CharacterMarkerData data)
    {
        m_Target = marker;
        m_TargetData = data;

        foreach (UIQuickcastButton item in m_Buttons)
            item.UpdateCanCast(marker, data);
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

        Spellcasting.CastSpell(spell, m_Target, new List<spellIngredientsData>(),
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

                button.UpdateCanCast(m_Target, m_TargetData);
            },
            () => button.Hightlight(false)
        );
    }

    private void OnClickMoreSpells()
    {
        if (m_Target == null || m_TargetData == null)
            return;

        this._Hide(true);

        UISpellcastBook.Open(
            m_TargetData,
            m_Target,
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



    //GAME EVENTS

    private void _OnPlayerAttacked(string caster, SpellData spell, Raincrow.GameEventResponses.SpellCastHandler.Result result)
    {
        if (m_Target == null || m_TargetData == null)
            return;

        if (caster != m_Target.Token.instance)
            return;

        UpdateCanCast(m_Target, m_TargetData);
    }

    private void _OnCharacterDead()
    {
        _UpdateCanCast(null, null);
    }

    private void _OnStatusEffectExpired(StatusEffect effect)
    {
        UpdateCanCast(m_Target, m_TargetData);
    }

    private void _OnStatusEffectApplied(string instance, StatusEffect statusEffect)
    {
        if (m_Target == null || m_TargetData == null)
            return;

        if (instance != m_Target.Token.instance)
            return;

        UpdateCanCast(m_Target, m_TargetData);
    }

    private void _OnImmunityChange(string caster, string target, bool immune)
    {
        if (m_Target == null || m_TargetData == null)
            return;

        if (caster == PlayerDataManager.playerData.instance && target == m_Target.Token.instance)
            UpdateCanCast(m_Target, m_TargetData);
        else if (target == PlayerDataManager.playerData.instance && caster == m_Target.Token.instance)
            UpdateCanCast(m_Target, m_TargetData);
    }

    private void _OnEnergyChange(string instance, int newEnergy)
    {
        if (m_Target == null || m_TargetData == null)
            return;

        if (instance != m_Target.Token.instance)
            return;

        if (newEnergy == 0 && m_Target.Type == MarkerManager.MarkerType.SPIRIT)
        {
            if (LocationIslandController.isInBattle)
            {

            }
            else
            {
                _Close();
            }
        }
        else
        {
            UpdateCanCast(m_Target, m_TargetData);
        }
    }

    private void _OnBanished()
    {
        if (LocationIslandController.isInBattle)
        {

        }
        else
        {
            _Close();
        }
    }

    private void _OnPlayerDead()
    {
        UpdateCanCast(null, null);
    }

    private void _OnMapTokenRemove(string instance)
    {
        if (m_Target == null)
            return;

        if (instance != m_Target.Token.instance)
            return;

        if (LocationIslandController.isInBattle)
        {

        }
        else
        {
            _Close();
        }
    }
}
