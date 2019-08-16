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
    [SerializeField] private UIQuickCast m_CastMenu;

    [Header("Images")]
    [SerializeField] private Image m_Sigil;

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

    private WitchMarker m_WitchMarker;
    private WitchToken m_WitchToken;
    private SelectWitchData_Map m_WitchDetails;
    private float m_PreviousMapZoom;
    public WitchToken WitchToken { get { return m_WitchToken; } }

    protected override void Awake()
    {
        m_Instance = this;

        m_BackButton.onClick.AddListener(OnClickBack);
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_PlayerButton.onClick.AddListener(OnClickPlayer);
        m_CovenButton.onClick.AddListener(OnClickCoven);

        m_CastMenu.OnClickCast = OnClickCast;
        m_CastMenu.OnQuickCast = (spell) => QuickCast(spell);

        base.Awake();
    }

    public void Show(WitchMarker witch, WitchToken data)
    {
        ShowHelper(witch, data);
        MainUITransition.Instance.HideMainUI();

        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_WitchMarker });

        SpellCastHandler.OnPlayerTargeted += _OnPlayerAttacked;
        MoveTokenHandler.OnTokenMove += _OnMapTokenMove;
        SpellCastHandler.OnApplyStatusEffect += _OnStatusEffectApplied;
        RemoveTokenHandler.OnTokenRemove += _OnMapTokenRemove;
        MarkerSpawner.OnImmunityChange += _OnImmunityChange;
        BanishManager.OnBanished += Abort;
        OnMapEnergyChange.OnEnergyChange += _OnEnergyChange;
        OnMapEnergyChange.OnPlayerDead += _OnCharacterDead;

        previousMapPosition = MapsAPI.Instance.GetWorldPosition();
        m_PreviousMapZoom = MapsAPI.Instance.normalizedZoom;

    }

    private void ShowHelper(WitchMarker witch, WitchToken data)
    {
        if (IsShowing)
            return;

        if (witch == null)
        {
            Debug.LogError("null witch");
            return;
        }

        m_WitchMarker = witch;
        m_WitchToken = data;
        m_WitchDetails = null;

        // //setup the ui
        m_DisplayNameText.text = m_WitchToken.displayName;
        m_DegreeSchoolText.text = Utilities.WitchTypeControlSmallCaps(m_WitchToken.degree);
        m_LevelText.text = LocalizeLookUp.GetText("card_witch_level").ToUpper() + " <color=black>" + m_WitchToken.level.ToString() + "</color>";
        _OnEnergyChange(m_WitchToken.instance, m_WitchToken.energy);

        // //sprite and color
        if (m_WitchToken.degree < 0)
        {
            m_Sigil.sprite = m_ShadowSigilSprite;
        }
        else if (m_WitchToken.degree > 0)
        {
            m_Sigil.sprite = m_WhiteSigilSprite;
        }
        else
        {
            m_Sigil.sprite = m_GreySigilSprite;
        }

        m_CovenText.text = LocalizeLookUp.GetText("chat_coven").ToUpper() + " <color=black>" + LocalizeLookUp.GetText("loading") + "</color>";

        m_ConditionsList.show = false;

        Show();
    }

    public override void ReOpen()
    {
        base.ReOpen();

        UpdateCanCast();
        if (!LocationIslandController.isInBattle)
        {
            MapsAPI.Instance.allowControl = false;

            IMarker marker = MarkerManager.GetMarker(m_WitchToken.instance);
            if (marker != null)
                MapCameraUtils.FocusOnMarker(marker.GameObject.transform.position);
            else
                Close();
        }
    }

    public override void Close()
    {
        CloseHelper();
        if (!LocationIslandController.isInBattle)
        {
            MainUITransition.Instance.ShowMainUI();
            MapsAPI.Instance.allowControl = true;
            MapCameraUtils.FocusOnPosition(previousMapPosition, m_PreviousMapZoom, true);

            //m_Witch.SetTextAlpha(NewMapsMarker.defaultTextAlpha);

            MarkerSpawner.HighlightMarker(new List<IMarker> { });

            SpellCastHandler.OnPlayerTargeted -= _OnPlayerAttacked;
            MoveTokenHandler.OnTokenMove -= _OnMapTokenMove;
            SpellCastHandler.OnApplyStatusEffect -= _OnStatusEffectApplied;
            RemoveTokenHandler.OnTokenRemove -= _OnMapTokenRemove;
            MarkerSpawner.OnImmunityChange -= _OnImmunityChange;
            BanishManager.OnBanished -= Abort;
        }
        else
        {
            LocationUnitSpawner.EnableMarkers();
        }
    }

    public void ShowPOP(WitchMarker witch, WitchToken data)
    {
        ShowHelper(witch, data);
    }

    private void CloseHelper()
    {
        base.Close();
        OnMapEnergyChange.OnEnergyChange -= _OnEnergyChange;
        OnMapEnergyChange.OnPlayerDead -= _OnCharacterDead;
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
            TeamManager.SendInvite(m_WitchToken.Id, false, (invite, error) =>
            {
                LoadingOverlay.Hide();
                if (string.IsNullOrEmpty(error))
                {
                    UIGlobalPopup.ShowPopUp(null, LocalizeLookUp.GetText("coven_invite_success"));
                }
                else
                {
                    UIGlobalPopup.ShowError(null, error);
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

        List<SpellData> spells = new List<SpellData>(PlayerDataManager.playerData.Spells);
        //spells.RemoveAll(spell => spell.target == SpellData.Target.SELF);

        UISpellcastBook.Open(
            m_WitchDetails,
            m_WitchMarker,
            spells,
            (spell, ingredients) =>
            {
                Spellcasting.CastSpell(spell, m_WitchMarker, ingredients,
                    (result) => ReOpen(),
                    () => OnClickClose()
                );
            },
            () =>
            {
                ReOpen();
            },
            () =>
            {
                Close();
            }
        );
    }

    private void QuickCast(string spellId)
    {
        SpellData spell = DownloadedAssets.GetSpell(spellId);

        if (spell == null)
            return;

        Hide();

        //send the cast
        Spellcasting.CastSpell(spell, m_WitchMarker, new List<spellIngredientsData>(), (result) =>
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
            m_CastMenu.UpdateCanCast(null, null);
            return;
        }

        m_CastMenu.UpdateCanCast(m_WitchDetails, m_WitchMarker);
    }

    private void _OnPlayerAttacked(string caster, SpellData spell, Raincrow.GameEventResponses.SpellCastHandler.Result result)
    {
        if (caster == m_WitchToken.instance)
        {
            UpdateCanCast();
        }
    }

    private void _OnEnergyChange(string instance, int newEnergy)
    {
        if (instance == m_WitchToken.instance)
        {
            m_EnergyText.text = LocalizeLookUp.GetText("card_witch_energy").ToUpper() + " <color=black>" + m_WitchToken.energy + " / " + m_WitchToken.baseEnergy + "</color>";
            UpdateCanCast();
        }
    }

    private void _OnMapTokenMove(string instance, Vector3 position)
    {
        if (m_WitchToken.instance == instance)
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
        if (instance == m_WitchToken.instance)
        {
            Abort();
            UIGlobalPopup.ShowPopUp(null, LocalizeLookUp.GetText("spellbook_witch_is_gone").Replace("{{witch name}}", m_WitchToken.displayName));// + " is gone.");
        }
    }

    private void _OnCharacterDead()
    {
        Abort();
    }

    private void _OnStatusEffectApplied(string character, StatusEffect statusEffect)
    {
        if (character != this.m_WitchToken.instance)
            return;

        foreach (StatusEffect item in m_WitchDetails.effects)
        {
            if (item.spell == statusEffect.spell)
            {
                m_WitchDetails.effects.Remove(item);
                break;
            }
        }
        m_WitchDetails.effects.Add(statusEffect);

        m_ConditionsList.AddCondition(statusEffect);
    }

    private void _OnImmunityChange(string caster, string target, bool immune)
    {
        if (caster == PlayerDataManager.playerData.instance && target == this.m_WitchToken.instance)
        {
            UpdateCanCast();
        }
        else if (target == PlayerDataManager.playerData.instance && caster == this.m_WitchToken.instance)
        {
            UpdateCanCast();
        }
    }

    private void Abort()
    {
        //wait for the result screen (UIspellcasting  will call OnFinishFlow)
        if (UIWaitingCastResult.isOpen)
            return;

        //if (UISpellcasting.isOpen)
        //    UISpellcasting.Instance.Close();

        Close();
    }

    private void UISpellcasting_OnCastResult()
    {
        //if token is gone
        if (MarkerSpawner.GetMarker(m_WitchToken.instance) == null)
        {
            Close();
            //UISpellcasting.Instance.Close();
        }
        //else stays at the spellcasting list
    }

    private void UISpellcasting_OnClickClose()
    {
        //close this too
        Close();
    }
}
