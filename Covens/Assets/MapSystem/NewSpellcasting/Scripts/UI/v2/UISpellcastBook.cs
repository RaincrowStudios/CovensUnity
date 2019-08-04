using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow;
using Raincrow.Maps;

public class UISpellcastBook : MonoBehaviour, IEnhancedScrollerDelegate
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private EnhancedScroller m_Scroller;
    [SerializeField] private UISpellcard m_CardPrefab;

    [Header("Buttons")]
    [SerializeField] private Button m_PortraitButton;
    [SerializeField] private Button m_InventoryButton;

    [Header("Portrait")]
    [SerializeField] private Image m_TargetPortrait;
    [SerializeField] private TextMeshProUGUI m_TargetName;
    [SerializeField] private Image m_TargetEnergy;

    private static UISpellcastBook m_Instance;

    private List<SpellData> m_Spells;
    private List<SpellData> m_ScrollerSpells = new List<SpellData>();    private int? m_SelectedSchool = null;    private SpellData m_SelectedSpell = null;    private System.Action<SpellData, List<spellIngredientsData>> m_OnConfirmSpell;    private System.Action m_OnBack;        public static bool IsOpen
    {
        get
        {
            if (m_Instance == null)
                return false;

            return m_Instance.m_InputRaycaster.enabled;
        }
    }    public static void Open(
        CharacterMarkerData target, 
        IMarker marker, 
        List<SpellData> spells, 
        System.Action<SpellData, List<spellIngredientsData>> onConfirm, 
        System.Action onClickBack = null)
    {
        if (m_Instance == null)
        {
            SceneManager.LoadSceneAsync(
                SceneManager.Scene.SPELLCAST_BOOK,
                UnityEngine.SceneManagement.LoadSceneMode.Additive,
                (progress) => { },
                () =>
                {
                    m_Instance.Show(target, marker, spells, onConfirm, onClickBack);
                });
        }
        else
        {
            m_Instance.Show(target, marker, spells, onConfirm, onClickBack);
        }
    }    public static void Close()
    {
        if (m_Instance == null)
            return;

        m_Instance.Hide();
    }    private void Awake()
    {
        m_Instance = this;
        m_InventoryButton.gameObject.SetActive(false);
        m_Scroller.Delegate = this;

        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        m_InventoryButton.onClick.AddListener(OnClickInventory);
        m_PortraitButton.onClick.AddListener(OnClickPortrait);

        DownloadedAssets.OnWillUnloadAssets += DownloadedAssets_OnWillUnloadAssets;
    }

    private void DownloadedAssets_OnWillUnloadAssets()
    {
        if (IsOpen)
            return;
        
        DownloadedAssets.OnWillUnloadAssets -= DownloadedAssets_OnWillUnloadAssets;

        m_Scroller.ClearAll();
        SceneManager.UnloadScene(SceneManager.Scene.SPELLCAST_BOOK, null, null);
    }

    private void Show(
        CharacterMarkerData target, 
        IMarker marker, 
        List<SpellData> spells, 
        System.Action<SpellData, List<spellIngredientsData>> onConfirm, 
        System.Action onBack = null)
    {
        m_OnConfirmSpell = onConfirm;
        m_OnBack = onBack;

        m_Spells = spells;
        SetSchool(m_SelectedSchool);

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
    }

    private void Hide()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        m_OnConfirmSpell = null;
        m_OnBack = null;
    }

    private void SetSchool(int? school)
    {
        if (school == m_SelectedSchool && m_ScrollerSpells.Count > 0)
            return;

        m_SelectedSchool = school;

        m_ScrollerSpells.Clear();
        foreach(SpellData spell in m_Spells)
        {
            if (school == null || school == spell.school)
                m_ScrollerSpells.Add(spell);
        }

        m_Scroller.ReloadData();
    }

    private void OnSelectCard(UISpellcard card)
    {
        if (m_SelectedSpell == null || m_SelectedSpell.id != card.Spell.id)
            m_SelectedSpell = card.Spell;
        else
            m_SelectedSpell = null;

        UISpellcard[] cards = m_Scroller.GetComponentsInChildren<UISpellcard>();
        foreach (UISpellcard _card in cards)
        {
            if (m_SelectedSpell == null || _card.Spell.id == m_SelectedSpell.id)
                _card.SetAlpha(1, 1);
            else
                _card.SetAlpha(0.65f, 1);
        }
    }

    private void OnClickCast(UISpellcard card)
    {
        Debug.LogError("cast " + card.Spell.Name);
        m_OnConfirmSpell?.Invoke(card.Spell, new List<spellIngredientsData>());
        Hide();
    }

    private void OnSelectSchool(int school)
    {
        if (m_SelectedSchool == null || m_SelectedSchool != school)
            SetSchool(school);
        else
            SetSchool(null);
    }

    private void OnClickInventory()
    {
        UIGlobalPopup.ShowError(null, "not implemented");
    }

    private void OnClickPortrait()
    {
        m_OnBack?.Invoke();
        Hide();
    }

    #region ENHANCED SCROLLER

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        UISpellcard cellView = scroller.GetCellView(m_CardPrefab) as UISpellcard;
        cellView.SetData(m_ScrollerSpells[dataIndex], OnSelectSchool, OnSelectCard, OnClickCast);

        if (m_SelectedSpell == null || m_ScrollerSpells[dataIndex].id == m_SelectedSpell.id)
            cellView.SetAlpha(1);
        else
            cellView.SetAlpha(0.5f);

        return cellView;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 300;
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return m_ScrollerSpells.Count;
    }

    #endregion
}
