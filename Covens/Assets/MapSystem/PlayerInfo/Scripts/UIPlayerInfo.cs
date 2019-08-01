using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;
using Raincrow.GameEventResponses;

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
    [SerializeField] private Button m_BackButton;
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private Button m_PlayerButton;
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

    private WitchMarker m_Witch;
    private WitchToken m_WitchData;
    private SelectWitchData_Map m_WitchDetails;
    private float m_PreviousMapZoom;
    private string previousMarker = "";
    public WitchToken Witch { get { return m_WitchData; } }


    protected override void Awake()
    {
        m_Instance = this;

        m_BackButton.onClick.AddListener(OnClickBack);
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_PlayerButton.onClick.AddListener(OnClickPlayer);
        m_CovenButton.onClick.AddListener(OnClickCoven);
        m_CastButton.onClick.AddListener(OnClickCast);

        m_QuickBless.onClick.AddListener(() => QuickCast("spell_bless"));
        m_QuickHex.onClick.AddListener(() => QuickCast("spell_hex"));
        m_QuickSeal.onClick.AddListener(() => QuickCast("spell_seal"));

        base.Awake();
    }

    public void Show(WitchMarker witch, WitchToken data)
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
        m_DegreeSchoolText.text = Utilities.WitchTypeControlSmallCaps(m_WitchData.degree);
        m_LevelText.text = LocalizeLookUp.GetText("card_witch_level").ToUpper() + " <color=black>" + m_WitchData.level.ToString() + "</color>";
        _OnEnergyChange(m_WitchData.instance, m_WitchData.energy);

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

        m_CovenText.text = LocalizeLookUp.GetText("chat_coven").ToUpper() + " <color=black>" + LocalizeLookUp.GetText("loading") + "</color>";

        previousMapPosition = MapsAPI.Instance.GetWorldPosition();
        m_PreviousMapZoom = MapsAPI.Instance.normalizedZoom;

        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Witch }, true);

        //witch.SetTextAlpha(NewMapsMarker.highlightTextAlpha);

        OnMapEnergyChange.OnEnergyChange += _OnEnergyChange;
        SpellCastHandler.OnPlayerTargeted += _OnPlayerAttacked;
        MoveTokenHandler.OnTokenMove += _OnMapTokenMove;
        SpellCastHandler.OnApplyStatusEffect += _OnStatusEffectApplied;
        RemoveTokenHandler.OnTokenRemove += _OnMapTokenRemove;
        OnMapEnergyChange.OnPlayerDead += _OnCharacterDead;
        MarkerSpawner.OnImmunityChange += _OnImmunityChange;
        BanishManager.OnBanished += Abort;

        Show();
        m_ConditionsList.show = false;
    }

    public override void ReOpen()
    {
        base.ReOpen();

        UpdateCanCast();

        MapsAPI.Instance.allowControl = false;

        IMarker marker = MarkerManager.GetMarker(m_WitchData.instance);
        if (marker != null)
            MapCameraUtils.FocusOnMarker(marker.gameObject.transform.position);
        else
            Close();
    }

    public override void Close()
    {
        base.Close();

        MainUITransition.Instance.ShowMainUI();
        MapsAPI.Instance.allowControl = true;
        MapCameraUtils.FocusOnPosition(previousMapPosition, m_PreviousMapZoom, true);

        //m_Witch.SetTextAlpha(NewMapsMarker.defaultTextAlpha);

        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Witch }, false);

        OnMapEnergyChange.OnEnergyChange -= _OnEnergyChange;
        SpellCastHandler.OnPlayerTargeted -= _OnPlayerAttacked;
        MoveTokenHandler.OnTokenMove -= _OnMapTokenMove;
        SpellCastHandler.OnApplyStatusEffect -= _OnStatusEffectApplied;
        RemoveTokenHandler.OnTokenRemove -= _OnMapTokenRemove;
        OnMapEnergyChange.OnPlayerDead -= _OnCharacterDead;
        MarkerSpawner.OnImmunityChange -= _OnImmunityChange;
        BanishManager.OnBanished -= Abort;
    }

    public void SetupDetails(SelectWitchData_Map details)
    {
        if (details == null)
        {
            Abort();
            return;
        }

        m_WitchDetails = details;

        if (string.IsNullOrEmpty(details.coven) == false)
            m_CovenText.text = LocalizeLookUp.GetText("chat_coven").ToUpper() + " <color=black>" + details.coven + "</color>";
        else if (string.IsNullOrEmpty(TeamManager.MyCovenId) == false)
            m_CovenText.text = LocalizeLookUp.GetText("invite_coven").ToUpper();
        else
            m_CovenText.text = LocalizeLookUp.GetText("chat_screen_no_coven");

        UpdateCanCast();
        m_ConditionsList.Setup(m_WitchDetails.effects);
    }

    private void OnClickBack()
    {
        Close();
    }

    private void OnClickClose()
    {
        Close();
    }

    private void OnClickCoven()
    {
        //show the witche's coven
        if (string.IsNullOrEmpty(m_WitchDetails.coven) == false)
        {
            TeamManagerUI.OpenName(m_WitchDetails.covenId);
        }
        //invite to my coven
        else if (string.IsNullOrEmpty(TeamManager.MyCovenId) == false)
        {
            LoadingOverlay.Show();
            TeamManager.SendInvite(m_WitchData.Id, false, (invite, error) =>
            {
                LoadingOverlay.Hide();
                if (string.IsNullOrEmpty(error))
                {
                    UIGlobalErrorPopup.ShowPopUp(null, LocalizeLookUp.GetText("coven_invite_success"));
                }
                else
                {
                    UIGlobalErrorPopup.ShowError(null, error);
                }
            });
        }
    }

    private void OnClickPlayer()
    {
        TeamPlayerView.Instance.Show(m_WitchDetails);
    }

    private void OnClickCast()
    {
        this.Hide();

        UISpellcasting.Instance.Show(
            m_WitchDetails,
            m_Witch,
            PlayerDataManager.playerData.Spells,
            UISpellcasting_OnCastResult,
            ReOpen,
            UISpellcasting_OnClickClose);
    }

    private void QuickCast(string spellId)
    {
        SpellData spell = DownloadedAssets.GetSpell(spellId);

        if (spell == null)
            return;
        
        Hide();

        //send the cast
        Spellcasting.CastSpell(spell, m_Witch, new List<spellIngredientsData>(), (result) =>
        {
            ReOpen();
        },
        () =>
        {
            OnClickClose();
        });
        return;
    }

    private void UpdateCanCast()
    {
        if (m_WitchDetails == null)
        {
            m_QuickBless.interactable = m_QuickHex.interactable = m_QuickSeal.interactable = m_CastButton.interactable = false;
            m_CastText.text = LocalizeLookUp.GetText("spellbook_more_spells") + " (" + LocalizeLookUp.GetText("loading") + ")";
            return;
        }

        Spellcasting.SpellState canCast = Spellcasting.CanCast((SpellData)null, m_Witch, m_WitchDetails);

        m_CastButton.interactable = canCast == Spellcasting.SpellState.CanCast;
        m_QuickHex.interactable = Spellcasting.CanCast("spell_hex", m_Witch, m_WitchDetails) == Spellcasting.SpellState.CanCast;
        m_QuickSeal.interactable = Spellcasting.CanCast("spell_seal", m_Witch, m_WitchDetails) == Spellcasting.SpellState.CanCast;
        m_QuickBless.interactable = Spellcasting.CanCast("spell_bless", m_Witch, m_WitchDetails) == Spellcasting.SpellState.CanCast;

        if (canCast == Spellcasting.SpellState.TargetImmune)
        {
            m_CastText.text = LocalizeLookUp.GetText("spell_immune_to_you");// "Player is immune to you";
            if (previousMarker != m_WitchDetails.name)
            {
                SoundManagerOneShot.Instance.WitchImmune();
                previousMarker = m_WitchDetails.name;
            }
        }
        else if (canCast == Spellcasting.SpellState.PlayerSilenced)
            m_CastText.text = LocalizeLookUp.GetText("ftf_silenced");//) "You are silenced";
        else
        {
            m_CastText.text = LocalizeLookUp.GetText("spellbook_more_spells");
        }

        if (UISpellcasting.isOpen)
            UISpellcasting.Instance.UpdateCanCast();
    }


    private void _OnPlayerAttacked(string caster, SpellData spell, Raincrow.GameEventResponses.SpellCastHandler.Result result)
    {
        if (caster == m_WitchData.instance)
        {
            UpdateCanCast();
        }
    }

    private void _OnEnergyChange(string instance, int newEnergy)
    {
        if (instance == m_WitchData.instance)
        {
            m_EnergyText.text = LocalizeLookUp.GetText("card_witch_energy").ToUpper() + " <color=black>" + m_WitchData.energy + " / " + m_WitchData.baseEnergy + "</color>";
            UpdateCanCast();
        }
    }

    private void _OnMapTokenMove(string instance, Vector3 position)
    {
        if (m_WitchData.instance == instance)
        {
            MapCameraUtils.FocusOnMarker(position);
        }
    }

    //private void _OnMapTokenEscape(string instance)
    //{
    //    if (instance == m_WitchData.instance)
    //    {
    //        Abort();
    //        UIGlobalErrorPopup.ShowPopUp(null, m_WitchData.displayName + " disappeared.");
    //    }
    //}

    private void _OnMapTokenRemove(string instance)
    {
        if (instance == m_WitchData.instance)
        {
            Abort();
            UIGlobalErrorPopup.ShowPopUp(null, LocalizeLookUp.GetText("spellbook_witch_is_gone").Replace("{{witch name}}", m_WitchData.displayName));// + " is gone.");
        }
    }

    private void _OnCharacterDead()
    {
        Abort();
    }

    private void _OnStatusEffectApplied(string character, StatusEffect statusEffect)
    {
        if (character != this.m_WitchData.instance)
            return;

        m_ConditionsList.AddCondition(statusEffect);
    }

    private void _OnImmunityChange(string caster, string target, bool immune)
    {
        if (caster == PlayerDataManager.playerData.instance && target == this.m_WitchData.instance)
        {
            UpdateCanCast();
        }
        else if (target == PlayerDataManager.playerData.instance && caster == this.m_WitchData.instance)
        {
            UpdateCanCast();
        }
    }

    private void Abort()
    {
        //wait for the result screen (UIspellcasting  will call OnFinishFlow)
        if (UIWaitingCastResult.isOpen)
            return;

        if (UISpellcasting.isOpen)
            UISpellcasting.Instance.Close();

        Close();
    }

    private void UISpellcasting_OnCastResult()
    {
        //if token is gone
        if (MarkerSpawner.GetMarker(m_WitchData.instance) == null)
        {
            Close();
            UISpellcasting.Instance.Close();
        }
        //else stays at the spellcasting list
    }

    private void UISpellcasting_OnClickClose()
    {
        //close this too
        Close();
    }
}
