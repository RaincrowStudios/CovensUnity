using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;
using Raincrow.GameEventResponses;
using Raincrow;
using Raincrow.FTF;

public class UISpiritInfo : UIInfoPanel
{
    [SerializeField] private UIConditionList m_ConditionList;
    [SerializeField] private Image m_SpiritArt;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI m_SpiritName;
    [SerializeField] private TextMeshProUGUI m_Tier;
    [SerializeField] private TextMeshProUGUI m_NewTier;
    //[SerializeField] private TextMeshProUGUI m_Level;
    [SerializeField] private TextMeshProUGUI m_Energy;
    [SerializeField] private TextMeshProUGUI m_Desc;
    [SerializeField] private TextMeshProUGUI m_ButtonChallengeText;

    [Header("Buttons")]
    [SerializeField] private Button m_InfoButton;
    [SerializeField] private Button m_OwnerButton;
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private Button m_ChallengeButton;

    private static UISpiritInfo m_Instance;

    // private int m_CurrentHexEffect;
    // private bool m_DisplayHexNotification;

    public static event System.Action OnOpen;
    public static event System.Action OnClose;

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

    public static void Show(SpiritMarker spirit, SpiritToken data, System.Action onClose = null, System.Action onLoad = null)
    {
        SpiritMarker = spirit;
        SpiritToken = data;

        if (m_Instance != null)
        {
            m_Instance._Show(spirit, data, onClose);
            onLoad?.Invoke();
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.SPIRIT_SELECT,
                UnityEngine.SceneManagement.LoadSceneMode.Additive,
                null,
                () =>
                {
                    m_Instance._Show(spirit, data, onClose);
                    onLoad?.Invoke();
                    LoadingOverlay.Hide();
                });
        }
    }
    public static void SetVisibility(bool isVisible)
    {
        if (m_Instance != null)
        {
            m_Instance.m_AlphaTweenId = LeanTween.alphaCanvas(m_Instance.m_CanvasGroup, isVisible ? 1 : 0, .5f).uniqueId;
        }
    }

    public static void SetupDetails(SelectSpiritData_Map data)
    {
        SpiritMarkerDetails = data;

        if (m_Instance == null)
            return;

        m_Instance._SetupDetails(data);
    }

    private SpiritData m_SpiritData;
    private System.Action m_OnClose;
    private float m_PreviousMapZoom;
    private int m_AlphaTweenId;

    public static SpiritMarker SpiritMarker { get; private set; }
    public static SpiritToken SpiritToken { get; private set; }
    public static SelectSpiritData_Map SpiritMarkerDetails { get; private set; }

    protected override void Awake()
    {
        m_Instance = this;

        base.Awake();

        m_CloseButton.onClick.AddListener(OnClickClose);
        m_InfoButton.onClick.AddListener(OnClickInfo);
        DownloadedAssets.OnWillUnloadAssets += OnWillUnloadAssets;
    }


    private void OnWillUnloadAssets()
    {
        if (IsShowing)
            return;

        DownloadedAssets.OnWillUnloadAssets -= OnWillUnloadAssets;
        LeanTween.cancel(m_TweenId);
        LeanTween.cancel(m_AlphaTweenId);
        SceneManager.UnloadScene(SceneManager.Scene.SPIRIT_SELECT, null, null);
    }

    private void _Show(IMarker spirit, Token token, System.Action onClose)
    {
        BackButtonListener.AddCloseAction(OnClickClose);

        m_OnClose = onClose;
        previousMapPosition = MapsAPI.Instance.GetWorldPosition();
        m_PreviousMapZoom = Mathf.Min(0.99f, MapsAPI.Instance.normalizedZoom);

        if (MarkerSpawner.GetMarker(spirit.Token.Id) == null)
        {
            m_PreviousMapZoom = MapsAPI.Instance.normalizedZoom;
            Close();
            return;
        }

        m_SpiritData = DownloadedAssets.spiritDict[SpiritToken.spiritId];
        m_OwnerButton.onClick.RemoveAllListeners();

        m_SpiritName.text = m_SpiritData.Name;
        m_Energy.text = LocalizeLookUp.GetText("cast_energy").ToUpper() + " <color=black>" + SpiritToken.energy.ToString();
        m_Desc.text = LocalizeLookUp.GetText("location_owned").Replace("{{Controller}}", "[" + LocalizeLookUp.GetText("loading") + "]");//"Belongs to [Loading...]";

        if (SpiritToken.effects == null)
        {
            SpiritToken.effects = new List<StatusEffect>();
        }
        m_ConditionList.Setup(SpiritToken.effects);

        m_SpiritArt.overrideSprite = null;
        DownloadedAssets.GetSprite(SpiritToken.spiritId, spr =>
        {
            if (m_SpiritArt != null)
                m_SpiritArt.overrideSprite = spr;
        });

        string tier;
        switch (m_SpiritData.tier)
        {
            case 1: tier = LocalizeLookUp.GetText("cast_spirit_lesser"); break;
            case 2: tier = LocalizeLookUp.GetText("cast_spirit_greater"); break;
            case 3: tier = LocalizeLookUp.GetText("cast_spirit_superior"); break;
            case 4: tier = LocalizeLookUp.GetText("cast_spirit_legendary"); break;
            default: tier = "?"; break;
        };
        m_NewTier.text = tier + " " + LocalizeLookUp.GetText("attacked_spirit");

        if (PlayerDataManager.playerData.level < 4 && m_SpiritData.tier > 1)
            PlayerNotificationManager.Instance.ShowNotification((LocalizeLookUp.GetText("spell_ui_notif_challenging")).ToUpper());//"This is challenging spirit for your level, witch.").ToUpper());


        OnMapEnergyChange.OnPlayerDead += _OnCharacterDead;
        OnMapEnergyChange.OnEnergyChange += _OnMapEnergyChange;
        SpellCastHandler.OnApplyEffect += _OnStatusEffectApplied;
        SpellCastHandler.OnExpireEffect += _OnExpireEffect;
        RemoveTokenHandler.OnTokenRemove += _OnMapTokenRemove;
        ExpireSpiritHandler.OnSpiritExpire += _OnMapTokenRemove;
        BanishManager.OnBanished += Abort;

        //MainUITransition.Instance.SetState(MainUITransition.State.MAPVIEW_SELECT);
        MarkerSpawner.HighlightMarkers(new List<MuskMarker> { PlayerManager.witchMarker, SpiritMarker });
        MoveTokenHandler.OnTokenMove += _OnMapTokenMove;

        if (SpiritMarkerDetails != null)
            _SetupDetails(SpiritMarkerDetails);

        Show();
        m_ConditionList.show = false;
        SoundManagerOneShot.Instance.PlaySpiritSelectedSpellbook();

        if (FTFManager.InFTF == false)
        {
            if (FirstTapManager.IsFirstTime("spellcasting"))
            {
                FirstTapManager.Show("spellcasting", () =>
                {
                    FirstTapManager.Show("quickcasting", () =>
                    {
                        FirstTapManager.Show("tier", null);
                    });
                });
            }
            else
            {
                if (FirstTapManager.IsFirstTime("tier"))
                {
                    FirstTapManager.Show("tier", null);
                }
            }
        }

        OnOpen?.Invoke();
    }

    public override void ReOpen()
    {
        base.ReOpen();

        IMarker spirit = MarkerSpawner.GetMarker(SpiritToken.instance);

        MapsAPI.Instance.allowControl = false;
        //if the spirit was destroyed, close the ui
        if (spirit != null)
        {
            MapCameraUtils.FocusOnMarker(spirit.GameObject.transform.position);
            MapCameraUtils.SetExtraFOV(-3);
        }
        else
        {
            Close();
        }
    }

    public override void Hide()
    {
        base.Hide();
    }

    public override void Close()
    {
        BackButtonListener.RemoveCloseAction();

        m_OnClose?.Invoke();
        m_OnClose = null;
        SpiritMarker = null;
        SpiritToken = null;
        SpiritMarkerDetails = null;
        m_ChallengeButton.interactable = false;
        m_ChallengeButton.onClick.RemoveAllListeners();

        base.Close();

        OnMapEnergyChange.OnPlayerDead -= _OnCharacterDead;
        OnMapEnergyChange.OnEnergyChange -= _OnMapEnergyChange;
        SpellCastHandler.OnApplyEffect -= _OnStatusEffectApplied;
        SpellCastHandler.OnExpireEffect -= _OnExpireEffect;
        RemoveTokenHandler.OnTokenRemove -= _OnMapTokenRemove;
        ExpireSpiritHandler.OnSpiritExpire -= _OnMapTokenRemove;

        MoveTokenHandler.OnTokenMove -= _OnMapTokenMove;
        BanishManager.OnBanished -= Abort;

        MapsAPI.Instance.allowControl = true;
        MapCameraUtils.FocusOnPosition(MapsAPI.Instance.mapCenter.position, m_PreviousMapZoom, true);
        MapCameraUtils.SetExtraFOV(0);
        //MainUITransition.Instance.SetState(MainUITransition.State.MAPVIEW);
        MarkerSpawner.HighlightMarkers(new List<MuskMarker> { });

        if (UISpellcastBook.IsOpen)
            UISpellcastBook.Close();

        OnClose?.Invoke();
    }

    private void _SetupDetails(SelectSpiritData_Map details)
    {
        if (details == null)
        {
            Abort();
            return;
        }

        m_ChallengeButton.interactable = string.IsNullOrEmpty(details.owner);
        m_ButtonChallengeText.text = details.token.insideBattle ? LocalizeLookUp.GetText("button_join") : LocalizeLookUp.GetText("button_challenge");

        if (details.token.insideBattle)
            m_ChallengeButton.onClick.AddListener(OnClickJoin);
        else
            m_ChallengeButton.onClick.AddListener(OnClickChallenge);


        if (string.IsNullOrEmpty(SpiritMarkerDetails.owner))
        {
            Debug.Log(SpiritMarkerDetails.owner + " owner");
            m_Desc.text = "";
            m_Tier.text = LocalizeLookUp.GetText("ftf_wild_spirit") + "\n";
            m_Desc.text = LocalizeLookUp.GetText("cast_spirit_knowledge");

            if (m_SpiritData.tier == 1)
                m_NewTier.text = LocalizeLookUp.GetText("cast_spirit_lesser");
            else if (m_SpiritData.tier == 2)
                m_NewTier.text = LocalizeLookUp.GetText("cast_spirit_greater");
            else if (m_SpiritData.tier == 3)
                m_NewTier.text = LocalizeLookUp.GetText("cast_spirit_superior");
            else
                m_NewTier.text = LocalizeLookUp.GetText("cast_spirit_legendary");

            /*foreach (var item in PlayerDataManager.playerData.knownSpirits)
            {
                if (item.spirit.ToString() == m_SpiritData.id)
                {
                    m_Tier.text = m_Tier.text + "\n<color=#616161>" + LocalizeLookUp.GetText("spirit_known");
                    break;
                }
            }*/


        }
        else
        {
            m_Tier.text = "";
            if (string.IsNullOrEmpty(details.coven))
            {
                m_Desc.text = LocalizeLookUp.GetText("location_owned").Replace("{{Controller}}", "<color=black>" + details.owner + "</color>");
                m_OwnerButton.onClick.AddListener(OnClickOwner);
            }
            else
            {
                m_Desc.text = LocalizeLookUp.GetText("location_owned").Replace("{{Controller}}", LocalizeLookUp.GetText("leaderboard_coven") + " <color=black>" + details.coven + "</color>");
                m_OwnerButton.onClick.AddListener(OnClickCoven);
            }
        }
        foreach (var item in PlayerDataManager.playerData.knownSpirits)
        {
            if (item.spirit.ToString() == m_SpiritData.id)
            {
                m_Tier.text = m_Tier.text + "<color=#616161>" + LocalizeLookUp.GetText("spirit_known");
                break;
            }
        }
    }

    private void OnClickClose()
    {
        Close();
    }

    private void OnClickInfo()
    {
        UIDetailedSpiritInfo.Instance.Show(m_SpiritData, SpiritToken);
    }

    private void OnClickOwner()
    {
        Debug.LogError("TODO: go to player");
    }

    private void OnClickCoven()
    {
        TeamManagerUI.OpenName(SpiritMarkerDetails.coven);
    }

    private void OnClickChallenge()
    {
        BattleArena.ChallengeRequests.Challenge(SpiritToken.Id);
        Close();
    }
    private void OnClickJoin()
    {
        BattleArena.ChallengeRequests.Join(SpiritToken.Id);
        Close();
    }

    private void Abort()
    {
        Close();
    }

    private void _OnCharacterDead()
    {
        Abort();
    }

    private void _OnMapTokenMove(string instance, Vector3 position)
    {
        if (SpiritToken?.instance == instance)
        {
            MapCameraUtils.FocusOnMarker(position);
        }
    }

    private void _OnMapEnergyChange(string instance, int newEnergy)
    {
        if (instance == SpiritToken?.instance)
        {
            float currentEnergy = float.Parse(m_Energy.text.Split('>')[1]);
            //spirit at half health
            if (currentEnergy > SpiritToken.baseEnergy / 2 && newEnergy < SpiritToken.baseEnergy / 2)
            {
                PlayerNotificationManager.Instance.ShowNotification(LocalizeLookUp.GetText("spell_ui_notif_half_health").Replace("{targetName}", "<color=orange>" + m_SpiritName.text + "</color>"));// $"The <color=orange>{m_SpiritName.text}</color> is now at half health. Keep it up, witch!");
            }
            // spirit vulnerable
            if (currentEnergy > SpiritToken.baseEnergy * .2f && newEnergy < SpiritToken.baseEnergy * .2f)
            {
                PlayerNotificationManager.Instance.ShowNotification(LocalizeLookUp.GetText("spell_ui_notif_vulnerable").Replace("{targetName}", "<color=orange>" + m_SpiritName.text + "</color>"));// $"The <color=orange>{m_SpiritName.text}</color> is now <color=red>vulnerable!</color>");
            }

            m_Energy.text = LocalizeLookUp.GetText("card_witch_energy").ToUpper() + " <color=black>" + newEnergy.ToString();


            if (newEnergy == 0)
            {
                Abort();
            }
        }
    }

    private void _OnStatusEffectApplied(string character, string caster, StatusEffect statusEffect)
    {
        if (character != SpiritToken?.instance)
            return;

        m_ConditionList.AddCondition(statusEffect);

        foreach (var item in SpiritToken.effects)
        {
            if (item.spell == "spell_hex" && item.stack == 3)
            {
                PlayerNotificationManager.Instance.ShowNotification(LocalizeLookUp.GetText("spell_ui_notif_hexed").Replace("{targetName}", "<color=orange>" + m_SpiritName.text + "</color>"));// $"The <color=orange>{m_SpiritName.text}</color> is fully Hexed and <color=red>vulnerable</color> to critical attacks.");

            }
        }
    }

    private void _OnExpireEffect(string character, StatusEffect effect)
    {
        if (character != SpiritToken?.instance)
            return;

        m_ConditionList.RemoveCondition(effect);
    }

    private void _OnMapTokenRemove(string instance)
    {
        if (instance == SpiritToken?.instance)
        {
            Abort();
            //UIGlobalErrorPopup.ShowPopUp(null, LocalizeLookUp.GetText("spellbook_witch_is_gone").Replace("{{witch name}}", m_SpiritData.spiritName));// + " is gone.");
        }
    }

    public static void HideCanvas(bool hide)
    {
        if (m_Instance == null)
            return;

        m_Instance.m_Canvas.enabled = !hide;
    }
}
