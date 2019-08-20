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

    [Header("others")]
    [SerializeField] private UIQuickCastPicker m_Picker;

    private static UIQuickCast m_Instance;
    private static event System.Action m_OnClose;
    public static bool IsOpen { get; private set; }
    
    private List<UIQuickcastButton> m_Buttons = new List<UIQuickcastButton>();
    private static IMarker m_Target;
    private static CharacterMarkerData m_TargetData;
    private string m_PreviousMarker = "";

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
        if (m_Instance == null)
            return;
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


    private void Awake()
    {
        m_Instance = this;
        m_ButtonPrefab.gameObject.SetActive(false);
        m_MoreSpells.onClick.AddListener(OnClickMoreSpells);

        DownloadedAssets.OnWillUnloadAssets += DownloadedAssets_OnWillUnloadAssets;
    }

    private void DownloadedAssets_OnWillUnloadAssets()
    {
        DownloadedAssets.OnWillUnloadAssets -= DownloadedAssets_OnWillUnloadAssets;
        SceneManager.UnloadScene(SceneManager.Scene.QUICKCAST, null, null);
    }


    private void _Open()
    {
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
        BanishManager.OnBanished += _OnBanished;
        OnMapEnergyChange.OnEnergyChange += _OnEnergyChange;

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
    }

    private void _Close()
    {
        SpellCastHandler.OnPlayerTargeted -= _OnPlayerAttacked;
        MarkerSpawner.OnImmunityChange -= _OnImmunityChange;
        SpellCastHandler.OnApplyStatusEffect -= _OnStatusEffectApplied;
        BanishManager.OnBanished -= _OnBanished;
        OnMapEnergyChange.OnEnergyChange -= _OnEnergyChange;

        IsOpen = false;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        m_OnClose?.Invoke();
        m_OnClose = null;
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
        Debug.Log("on click spell " + button.Spell);
        if (string.IsNullOrEmpty(button.Spell))
            return;

        SpellData spell = DownloadedAssets.GetSpell(button.Spell);

        this._Close();
        Spellcasting.CastSpell(spell, m_Target, new List<spellIngredientsData>(), 
            (result) => this._Open(),
            null);
    }

    private void OnHoldSpell(UIQuickcastButton button)
    {
        Debug.Log("on hold spell " + button.Spell);

        button.Hightlight(true);

        m_Picker.Show(
            button.Spell,
            spell =>
            {
                //SpellData data = DownloadedAssets.GetSpell(spell);

                //if (data == null)
                //    return;

                //if (data.level > PlayerDataManager.playerData.level)
                //    return;

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

        this._Close();

        UISpellcastBook.Open(
            m_TargetData,
            m_Target,
            PlayerDataManager.playerData.UnlockedSpells,
            (spell, ingredients) =>
            {
                Spellcasting.CastSpell(spell, m_Target, ingredients,
                    (result) => this._Open(),
                    null//() => this._Close()
                );
            },
            () => this._Open(),
            null //() => this._Close()
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

        UpdateCanCast(m_Target, m_TargetData);
    }

    private void _OnBanished()
    {
        UpdateCanCast(null, null);
    }

    private void _OnPlayerDead()
    {
        UpdateCanCast(null, null);
    }
}
