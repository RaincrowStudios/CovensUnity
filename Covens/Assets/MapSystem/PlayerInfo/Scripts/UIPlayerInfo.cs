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

    public static bool IsShowing
    {
        get
        {
            if (m_Instance == null)
                return false;
            else
                return m_Instance.m_IsShowing;
        }
    }

    public static void Show(WitchMarker witch, WitchToken data, System.Action onClose = null, System.Action onLoad = null)
    {
        WitchMarker = witch;
        WitchToken = data;

        if (m_Instance != null)
        {
            m_Instance._Show(witch, data, onClose);
            onLoad?.Invoke();
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
                    onLoad?.Invoke();
                    LoadingOverlay.Hide();
                });
        }
    }

    public static void SetupDetails(SelectWitchData_Map data)
    {
        WitchMarkerDetails = data;

        if (m_Instance == null)
            return;

        m_Instance._SetupDetails(data);
    }

    private float m_PreviousMapZoom;
    private System.Action m_OnClose;
    private int m_AlphaTweenId;

    public static WitchToken WitchToken { get; private set; }
    public static WitchMarker WitchMarker { get; private set; }
    public static SelectWitchData_Map WitchMarkerDetails { get; private set; }

    protected override void Awake()
    {
        m_Instance = this;

        m_CloseButton.onClick.AddListener(OnClickClose);
        m_PlayerButton.onClick.AddListener(OnClickPlayer);
        m_CovenButton.onClick.AddListener(OnClickCoven);

        base.Awake();

        DownloadedAssets.OnWillUnloadAssets += OnWillUnloadAssets;
    }

    private void OnWillUnloadAssets()
    {
        if (IsShowing)
            return;

        DownloadedAssets.OnWillUnloadAssets -= OnWillUnloadAssets;
        LeanTween.cancel(m_TweenId);
        LeanTween.cancel(m_AlphaTweenId);
        SceneManager.UnloadScene(SceneManager.Scene.PLAYER_SELECT, null, null);
    }

    public static void SetVisibility(bool isVisible)
    {
        if (m_Instance != null)
        {
            m_Instance.m_AlphaTweenId = LeanTween.alphaCanvas(m_Instance.m_CanvasGroup, isVisible ? 1 : 0, .5f).uniqueId;
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

        m_ConditionsList.Setup(data.effects);

        if (PlayerDataManager.playerData.level < 4 && WitchToken.level > 4)
            PlayerNotificationManager.Instance.ShowNotification(($"{WitchToken.displayName} is challenging target for your level, witch.").ToUpper());

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
        ExpireStatusEffectHandler.OnEffectExpire += _OnExpireEffect;
        BanishManager.OnBanished += Abort;
        OnMapEnergyChange.OnEnergyChange += _OnEnergyChange;
        OnMapEnergyChange.OnPlayerDead += _OnCharacterDead;

        if (WitchMarkerDetails != null)
            _SetupDetails(WitchMarkerDetails);

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
            {
                MapCameraUtils.FocusOnMarker(marker.GameObject.transform.position);
                MapCameraUtils.SetExtraFOV(-3);
            }
            else
            {
                Close();
            }
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
        WitchMarkerDetails = null;

        //aniamte the ui
        base.Close();

        //unsubscribe events
        MoveTokenHandler.OnTokenMove -= _OnMapTokenMove;
        RemoveTokenHandler.OnTokenRemove -= _OnMapTokenRemove;
        SpellCastHandler.OnApplyStatusEffect -= _OnStatusEffectApplied;
        ExpireStatusEffectHandler.OnEffectExpire -= _OnExpireEffect;
        BanishManager.OnBanished -= Abort;
        OnMapEnergyChange.OnEnergyChange -= _OnEnergyChange;
        OnMapEnergyChange.OnPlayerDead -= _OnCharacterDead;

        if (!LocationIslandController.isInBattle)
        {
            MainUITransition.Instance.ShowMainUI();
            MapsAPI.Instance.allowControl = true;
            MapCameraUtils.FocusOnPosition(previousMapPosition, m_PreviousMapZoom, true);
            MapCameraUtils.SetExtraFOV(0);
            MarkerSpawner.HighlightMarker(new List<IMarker> { });
        }
        else
        {
            LocationUnitSpawner.EnableMarkers();
        }
        
        if (UISpellcastBook.IsOpen)
            UISpellcastBook.Close();
    }

    private void _SetupDetails(SelectWitchData_Map details)
    {
        if (string.IsNullOrEmpty(details.coven) == false)
            m_CovenText.text = LocalizeLookUp.GetText("chat_coven").ToUpper() + " <color=black>" + details.coven + "</color>";
        else
            m_CovenText.text = LocalizeLookUp.GetText("chat_screen_no_coven");
    }

    private void OnClickClose()
    {
        Close();
    }

    private void OnClickCoven()
    {
        //show the witche's coven
        if (string.IsNullOrEmpty(WitchMarkerDetails.coven) == false)
        {
            TeamManagerUI.OpenName(WitchMarkerDetails.coven);
        }
    }

    private void OnClickPlayer()
    {
        TeamPlayerView.Instance.Show(WitchMarkerDetails);
    }


    private void _OnEnergyChange(string instance, int newEnergy)
    {
        if (instance == WitchToken.instance)
        {
            float currentEnergy = float.Parse(m_EnergyText.text.Split('>')[1].Split(' ')[0]);
            //spirit at half health
            if (currentEnergy > WitchToken.baseEnergy / 2 && newEnergy < WitchToken.baseEnergy / 2)
            {
                PlayerNotificationManager.Instance.ShowNotification($"The witch <color=orange>{WitchToken.displayName}</color> is now at half health. Keep it up!");
            }
            // spirit vulnerable
            if (currentEnergy > WitchToken.baseEnergy * .2f && newEnergy < WitchToken.baseEnergy * .2f)
            {
                PlayerNotificationManager.Instance.ShowNotification($"The witch <color=orange>{WitchToken.displayName}</color> is now <color=red>vulnerable!</color>");
            }

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

    private void _OnStatusEffectApplied(string character, string caster, StatusEffect statusEffect)
    {
        if (character != WitchToken.instance)
            return;

        m_ConditionsList.AddCondition(statusEffect);

        foreach (var item in WitchToken.effects)
        {
            if (item.spell == "spell_hex" && item.stack == 3)
            {
                PlayerNotificationManager.Instance.ShowNotification($"The <color=orange>{WitchToken.displayName}</color> is fully Hexed and <color=red>vulnerable</color> to critical attacks.");
            }
        }
    }

    private void _OnExpireEffect(string character, StatusEffect effect)
    {
        if (character != WitchToken.instance)
            return;

        m_ConditionsList.RemoveCondition(effect);
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
