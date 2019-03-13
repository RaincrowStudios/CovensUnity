using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;

public class UIPlayerInfo : UIInfoPanel
{
    [SerializeField] private Sprite m_ShadowSigilSprite;
    [SerializeField] private Sprite m_GreySigilSprite;
    [SerializeField] private Sprite m_WhiteSigilSprite;

    [SerializeField] private UIConditionList m_ConditionsList;

    [Header("Images")]
    [SerializeField] private Image m_Sigil;
    [SerializeField] private TextMeshProUGUI m_CastText;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI m_DisplayNameText;
    [SerializeField] private TextMeshProUGUI m_DegreeSchoolText;
    [SerializeField] private TextMeshProUGUI m_LevelText;
    [SerializeField] private TextMeshProUGUI m_EnergyText;
    [SerializeField] private TextMeshProUGUI m_CovenText;

    [Header("Buttons")]
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private Button m_CovenButton;
    [SerializeField] private Button m_CastButton;
    [SerializeField] private Button m_QuickBless;
    [SerializeField] private Button m_QuickSeal;
    [SerializeField] private Button m_QuickHex;
    
    private static UIPlayerInfo m_Instance;
    public static UIPlayerInfo Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UIPlayerInfo>("UIPlayerInfo"));
            return m_Instance;
        }
    }

    public static bool isShowing
    {
        get
        {
            if (m_Instance == null)
                return false;
            else
                return m_Instance.IsShowing;
        }
    }

    private IMarker m_Witch;
    private Token m_WitchData;
    private MarkerDataDetail m_WitchDetails;
    private Vector3 m_PreviousMapPosition;
    private float m_PreviousMapZoom;

    public Token Witch { get { return m_WitchData; } }


    protected override void Awake()
    {
        m_Instance = this;

        m_CloseButton.onClick.AddListener(OnClickClose);
        m_CovenButton.onClick.AddListener(OnClickCoven);
        m_CastButton.onClick.AddListener(OnClickCast);

        m_QuickBless.onClick.AddListener(() => QuickCast("spell_bless"));
        m_QuickHex.onClick.AddListener(() => QuickCast("spell_hex"));
        m_QuickSeal.onClick.AddListener(() => QuickCast("spell_seal"));

        base.Awake();
    }

    public void Show(IMarker witch, Token data)
    {
        if (IsShowing)
            return;

        if (witch == null)
        {
            Debug.LogError("null witch");
            return;
        }

        MainUITransition.Instance.HideMainUI();

        m_Witch = witch;
        m_WitchData = data;
        m_WitchDetails = null;

        //setup the ui
        m_DisplayNameText.text = m_WitchData.displayName;
        m_DegreeSchoolText.text = "degree " + m_WitchData.degree;
        m_LevelText.text = $"LEVEL <color=black>{data.level}</color>";
        m_EnergyText.text = $"ENERGY <color=black>{data.energy}</color>";

        //sprite and color
        if (m_WitchData.degree < 0)
        {
            m_Sigil.sprite = m_ShadowSigilSprite;
        }
        else if (m_WitchData.degree > 0)
        {
            m_Sigil.sprite = m_WhiteSigilSprite;
        }
        else
        {
            m_Sigil.sprite = m_GreySigilSprite;
        }

        m_CovenButton.interactable = false;
        m_CovenText.text = $"COVEN <color=black>Loading...</color>";

        m_PreviousMapPosition = StreetMapUtils.CurrentPosition();
        m_PreviousMapZoom = MapController.Instance.zoom;
        
        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Witch }, true);

        witch.SetTextAlpha(NewMapsMarker.highlightTextAlpha);

        OnMapEnergyChange.OnEnergyChange += _OnEnergyChange;
        OnMapSpellcast.OnPlayerTargeted += _OnPlayerAttacked;
        OnMapTokenMove.OnTokenFinishMove += _OnMapTokenMove;
        OnMapTokenMove.OnTokenEscaped += _OnMapTokenEscape;
        OnMapTokenRemove.OnTokenRemove += _OnMapTokenRemove;
        OnCharacterDeath.OnPlayerDead += _OnCharacterDead;
        OnMapConditionAdd.OnConditionAdded += _OnConditionAdd;
        OnMapConditionRemove.OnConditionRemoved += _OnConditionRemove;

        StreetMapUtils.FocusOnTarget(m_Witch);

        Show();
        m_ConditionsList.Hide();
    }

    public override void ReOpen()
    {
        base.ReOpen();

        UpdateCanCast();

        MapController.Instance.allowControl = false;
    }

    public void SetupDetails(MarkerDataDetail details)
    {
        m_WitchDetails = details;

        m_CovenButton.interactable = !string.IsNullOrEmpty(m_WitchDetails.covenName);
        m_CovenText.text = m_CovenButton.interactable ? $"COVEN <color=black>{details.covenName}</color>" : "No coven";

        UpdateCanCast();
        m_ConditionsList.Setup(m_WitchData, m_WitchDetails);
    }

    private void OnClickClose()
    {
        MainUITransition.Instance.ShowMainUI();
        MapController.Instance.allowControl = true;
        StreetMapUtils.FocusOnPosition(m_PreviousMapPosition, true, m_PreviousMapZoom, true);

        m_Witch.SetTextAlpha(NewMapsMarker.defaultTextAlpha);

        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Witch }, false);

        OnMapEnergyChange.OnEnergyChange -= _OnEnergyChange;
        OnMapSpellcast.OnPlayerTargeted -= _OnPlayerAttacked;
        OnMapTokenMove.OnTokenFinishMove -= _OnMapTokenMove;
        OnMapTokenMove.OnTokenEscaped -= _OnMapTokenEscape;
        OnMapTokenRemove.OnTokenRemove -= _OnMapTokenRemove;
        OnCharacterDeath.OnPlayerDead -= _OnCharacterDead;
        OnMapConditionAdd.OnConditionAdded -= _OnConditionAdd;
        OnMapConditionRemove.OnConditionRemoved -= _OnConditionRemove;

        Close();
    }

    private void OnClickCoven()
    {
        Debug.Log("TODO: Open coven");
    }

    private void OnClickCast()
    {
        this.Hide();

        UISpellcasting.Instance.Show(m_WitchDetails, m_Witch, PlayerDataManager.playerData.spells, () => { ReOpen(); });
    }

    private void QuickCast(string spellId)
    {
        foreach (SpellData spell in PlayerDataManager.playerData.spells)
        {
            if (spell.id == spellId)
            {
                Hide();

                //send the cast
                Spellcasting.CastSpell(spell, m_Witch, new List<spellIngredientsData>(), (result) =>
                {
                    if (result != null)
                    {
                        //if the spell backfired, the camera is focusing on the player
                        if (result.effect == "backfire")
                            StreetMapUtils.FocusOnTarget(m_Witch);
                    }
                    ReOpen();
                });
                return;
            }
        }
    }

    private void UpdateCanCast()
    {
        if (m_WitchDetails == null)
        {
            m_QuickBless.interactable = m_QuickHex.interactable = m_QuickSeal.interactable = m_CastButton.interactable = false;
            m_CastText.text = "Spellbook (Loading..)";
            return;
        }

        Spellcasting.SpellState canCast = Spellcasting.CanCast((SpellData)null, m_Witch, m_WitchDetails);

        m_CastButton.interactable = canCast == Spellcasting.SpellState.CanCast;
        //m_QuickBless.interactable = m_QuickHex.interactable = m_QuickSeal.interactable = m_CastButton.interactable;

        if (canCast == Spellcasting.SpellState.TargetImmune)
            m_CastText.text = "Player is immune to you";
        else if (canCast == Spellcasting.SpellState.PlayerSilenced)
            m_CastText.text = "You are silenced";
        else
        {
            m_CastText.text = "Spellbook";

            m_QuickHex.interactable = Spellcasting.CanCast("spell_hex", m_Witch, m_WitchDetails) == Spellcasting.SpellState.CanCast;
            m_QuickSeal.interactable = Spellcasting.CanCast("spell_seal", m_Witch, m_WitchDetails) == Spellcasting.SpellState.CanCast;
            m_QuickBless.interactable = Spellcasting.CanCast("spell_bless", m_Witch, m_WitchDetails) == Spellcasting.SpellState.CanCast;
        }
    }
    

    private void _OnPlayerAttacked(IMarker caster, SpellDict spell, Result result)
    {
        if (caster == m_Witch)
        {
            UpdateCanCast();
        }
    }

    private void _OnEnergyChange(string instance, int newEnergy)
    {
        if (instance == m_WitchData.instance)
        {
            m_EnergyText.text = $"ENERGY <color=black>{newEnergy}</color>";
            UpdateCanCast();
        }
    }

    private void _OnMapTokenMove(string instance, Vector3 position)
    {
        if (m_WitchData.instance == instance)
        {
            StreetMapUtils.FocusOnTarget(m_Witch);
            m_Witch.EnableAvatar();
        }
    }

    private void _OnMapTokenEscape(string instance)
    {
        UIGlobalErrorPopup.ShowPopUp(Abort, m_WitchData.displayName + " disappeared.");
    }

    private void _OnMapTokenRemove(string instance)
    {
        UIGlobalErrorPopup.ShowPopUp(Abort, m_WitchData.displayName + " is gone.");
    }

    private void _OnCharacterDead(string name, string spirit)
    {
        Abort();
    }

    private void _OnConditionAdd(Conditions condition)
    {
        if (condition.bearer != this.m_WitchData.instance)
            return;

        m_ConditionsList.AddCondition(condition);
    }

    private void _OnConditionRemove(Conditions condition)
    {
        if (condition.bearer != this.m_WitchData.instance)
            return;

        m_ConditionsList.RemoveCondition(condition);
    }

    private void Abort()
    {
        if (UISpellcasting.isOpen)
            UISpellcasting.Instance.FinishSpellcastingFlow();

        if (UIWaitingCastResult.isOpen)
            UIWaitingCastResult.Instance.OnClickContinue();

        OnClickClose();
    }
}
