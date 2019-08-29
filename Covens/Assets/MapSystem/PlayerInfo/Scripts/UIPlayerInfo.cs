using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;
using Raincrow.GameEventResponses;
using Raincrow;

public class UIPlayerInfo : UIInfoPanel
{
    [SerializeField] private UIConditionList m_ConditionsList;
    [SerializeField] private ApparelView m_MaleView;
    [SerializeField] private ApparelView m_FemaleView;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI m_DisplayNameText;
    [SerializeField] private TextMeshProUGUI m_DegreeSchoolText;
    [SerializeField] private TextMeshProUGUI m_LevelText;
    [SerializeField] private TextMeshProUGUI m_EnergyText;
    [SerializeField] private TextMeshProUGUI m_CovenText;

    [Header("Buttons")]
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private Button m_PlayerButton;
    [SerializeField] private Button m_CovenButton;

    private static UIPlayerInfo m_Instance;

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

    public static void Show(WitchMarker witch, WitchToken data, System.Action onClose = null)
    {
        WitchMarker = witch;
        WitchToken = data;

        if (m_Instance != null)
        {
            m_Instance._Show(witch, data, onClose);
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.PLAYER_SELECT,
                UnityEngine.SceneManagement.LoadSceneMode.Additive,
                null,
                () =>
                {
                    m_Instance._Show(witch, data, onClose);
                    LoadingOverlay.Hide();
                });
        }
    }

    public static void SetupDetails(SelectWitchData_Map data)
    {
        m_WitchData = data;

        if (m_Instance == null)
            return;

        m_Instance._SetupDetails(data);
    }

    private float m_PreviousMapZoom;
    private System.Action m_OnClose;

    public static WitchToken WitchToken { get; private set; }
    public static WitchMarker WitchMarker { get; private set; }
    private static SelectWitchData_Map m_WitchData;

    protected override void Awake()
    {
        m_Instance = this;

        m_CloseButton.onClick.AddListener(OnClickClose);
        m_PlayerButton.onClick.AddListener(OnClickPlayer);
        m_CovenButton.onClick.AddListener(OnClickCoven);

        base.Awake();
    }


    public static void SetVisibility(bool isVisible)
    {
        if (m_Instance != null)
        {
            LeanTween.alphaCanvas(m_Instance.m_CanvasGroup, isVisible ? 1 : 0, .5f);
        }
    }



    private void _Show(WitchMarker witch, WitchToken data, System.Action onClose)
    {
        if (witch == null)
        {
            Debug.LogError("null witch");
            return;
        }

        m_OnClose = onClose;
        previousMapPosition = MapsAPI.Instance.GetWorldPosition();
        m_PreviousMapZoom = Mathf.Min(0.99f, MapsAPI.Instance.normalizedZoom);

        if (MarkerSpawner.GetMarker(witch.Token.Id) == null)
        {
            m_PreviousMapZoom = MapsAPI.Instance.normalizedZoom;
            Close();
            return;
        }

        
        // //setup the ui
        m_DisplayNameText.text = WitchToken.displayName;
        m_DegreeSchoolText.text = Utilities.WitchTypeControlSmallCaps(WitchToken.degree);
        m_LevelText.text = LocalizeLookUp.GetText("card_witch_level").ToUpper() + " <color=black>" + WitchToken.level.ToString() + "</color>";
        _OnEnergyChange(WitchToken.instance, WitchToken.energy);

        m_CovenText.text = LocalizeLookUp.GetText("chat_coven").ToUpper() + " <color=black>" + LocalizeLookUp.GetText("loading") + "</color>";

        m_ConditionsList.show = false;

        if (data.male)
        {
            m_FemaleView.ResetApparel();
            m_MaleView.InitializeChar(data.equipped);
        }
        else
        {
            m_MaleView.ResetApparel();
            m_FemaleView.InitializeChar(data.equipped);
        }

        if (!LocationIslandController.isInBattle)
        {
            MainUITransition.Instance.HideMainUI();
            MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, WitchMarker });

            MoveTokenHandler.OnTokenMove += _OnMapTokenMove;
        }

        RemoveTokenHandler.OnTokenRemove += _OnMapTokenRemove;
        SpellCastHandler.OnApplyStatusEffect += _OnStatusEffectApplied;
        BanishManager.OnBanished += Abort;
        OnMapEnergyChange.OnEnergyChange += _OnEnergyChange;
        OnMapEnergyChange.OnPlayerDead += _OnCharacterDead;

        if (m_WitchData != null)
            _SetupDetails(m_WitchData);

        //animate the ui
        Show();
    }

    public override void ReOpen()
    {
        base.ReOpen();

        if (!LocationIslandController.isInBattle)
        {
            MapsAPI.Instance.allowControl = false;

            IMarker marker = MarkerManager.GetMarker(WitchToken.instance);
            if (marker != null)
                MapCameraUtils.FocusOnMarker(marker.GameObject.transform.position);
            else
                Close();
        }
    }

    public override void Hide()
    {
        base.Hide();
    }

    public override void Close()
    {
        m_OnClose?.Invoke();
        m_OnClose = null;

        WitchMarker = null;
        WitchToken = null;
        m_WitchData = null;

        //aniamte the ui
        base.Close();

        //unsubscribe events
        MoveTokenHandler.OnTokenMove -= _OnMapTokenMove;
        RemoveTokenHandler.OnTokenRemove -= _OnMapTokenRemove;
        SpellCastHandler.OnApplyStatusEffect -= _OnStatusEffectApplied;
        BanishManager.OnBanished -= Abort;
        OnMapEnergyChange.OnEnergyChange -= _OnEnergyChange;
        OnMapEnergyChange.OnPlayerDead -= _OnCharacterDead;

        if (!LocationIslandController.isInBattle)
        {
            MainUITransition.Instance.ShowMainUI();
            MapsAPI.Instance.allowControl = true;
            MapCameraUtils.FocusOnPosition(previousMapPosition, m_PreviousMapZoom, true);
            MarkerSpawner.HighlightMarker(new List<IMarker> { });
        }
        else
        {
            LocationUnitSpawner.EnableMarkers();
        }
    }

    private void _SetupDetails(SelectWitchData_Map details)
    {
        if (string.IsNullOrEmpty(details.coven) == false)
            m_CovenText.text = LocalizeLookUp.GetText("chat_coven").ToUpper() + " <color=black>" + details.coven + "</color>";
        else
            m_CovenText.text = LocalizeLookUp.GetText("chat_screen_no_coven");

        m_ConditionsList.Setup(m_WitchData.effects);
    }

    private void OnClickClose()
    {
        Close();
    }

    private void OnClickCoven()
    {
        //show the witche's coven
        if (string.IsNullOrEmpty(m_WitchData.coven) == false)
        {
            TeamManagerUI.OpenName(m_WitchData.coven);
        }
    }

    private void OnClickPlayer()
    {
        TeamPlayerView.Instance.Show(m_WitchData);
    }


    private void _OnEnergyChange(string instance, int newEnergy)
    {
        if (instance == WitchToken.instance)
        {
            m_EnergyText.text = LocalizeLookUp.GetText("card_witch_energy").ToUpper() + " <color=black>" + WitchToken.energy + " / " + WitchToken.baseEnergy + "</color>";
        }
    }

    private void _OnMapTokenMove(string instance, Vector3 position)
    {
        if (WitchToken.instance == instance)
        {
            MapCameraUtils.FocusOnMarker(position);
        }
    }

    private void _OnMapTokenRemove(string instance)
    {
        if (instance == WitchToken.instance)
        {
            Abort();
            //UIGlobalPopup.ShowPopUp(null, LocalizeLookUp.GetText("spellbook_witch_is_gone").Replace("{{witch name}}", WitchToken.displayName));// + " is gone.");
        }
    }

    private void _OnCharacterDead()
    {
        Abort();
    }

    private void _OnStatusEffectApplied(string character, StatusEffect statusEffect)
    {
        if (character != WitchToken.instance)
            return;

        foreach (StatusEffect item in m_WitchData.effects)
        {
            if (item.spell == statusEffect.spell)
            {
                m_WitchData.effects.Remove(item);
                break;
            }
        }
        m_WitchData.effects.Add(statusEffect);
        m_ConditionsList.AddCondition(statusEffect);
    }

    private void Abort()
    {
        Close();
    }

    private void UISpellcasting_OnCastResult()
    {
        //if token is gone
        if (MarkerSpawner.GetMarker(WitchToken.instance) == null)
        {
            Close();
        }
    }

    private void UISpellcasting_OnClickClose()
    {
        //close this too
        Close();
    }
}
