using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;
using Raincrow.GameEventResponses;
using Raincrow;

public class UISpiritInfo : UIInfoPanel
{
    [SerializeField] private UIConditionList m_ConditionList;
    [SerializeField] private Image m_SpiritArt;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI m_SpiritName;
    [SerializeField] private TextMeshProUGUI m_Tier;
    //[SerializeField] private TextMeshProUGUI m_Level;
    [SerializeField] private TextMeshProUGUI m_Energy;
    [SerializeField] private TextMeshProUGUI m_Desc;

    [Header("Buttons")]
    [SerializeField] private Button m_InfoButton;
    [SerializeField] private Button m_OwnerButton;
    [SerializeField] private Button m_CloseButton;

    private static UISpiritInfo m_Instance;

    // private int m_CurrentHexEffect;
    // private bool m_DisplayHexNotification;

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
        m_Tier.text = tier + " " + LocalizeLookUp.GetText("attacked_spirit");

        if (PlayerDataManager.playerData.level < 4 && m_SpiritData.tier > 1)
            PlayerNotificationManager.Instance.ShowNotification(("This is challenging spirit for your level, witch.").ToUpper());


        OnMapEnergyChange.OnPlayerDead += _OnCharacterDead;
        OnMapEnergyChange.OnEnergyChange += _OnMapEnergyChange;
        SpellCastHandler.OnApplyStatusEffect += _OnStatusEffectApplied;
        ExpireStatusEffectHandler.OnEffectExpire += _OnExpireEffect;
        RemoveTokenHandler.OnTokenRemove += _OnMapTokenRemove;
        ExpireSpiritHandler.OnSpiritExpire += _OnMapTokenRemove;
        BanishManager.OnBanished += Abort;

        if (!LocationIslandController.isInBattle)
        {
            MainUITransition.Instance.HideMainUI();
            MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, SpiritMarker });
            MoveTokenHandler.OnTokenMove += _OnMapTokenMove;
        }

        if (SpiritMarkerDetails != null)
            _SetupDetails(SpiritMarkerDetails);

        Show();
        m_ConditionList.show = false;
        SoundManagerOneShot.Instance.PlaySpiritSelectedSpellbook();
    }

    public override void ReOpen()
    {
        base.ReOpen();

        IMarker spirit = MarkerManager.GetMarker(SpiritToken.instance);

        if (!LocationIslandController.isInBattle)
        {
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
        else
        {
            if (spirit == null) Close();
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

        SpiritMarker = null;
        SpiritToken = null;
        SpiritMarkerDetails = null;

        base.Close();

        OnMapEnergyChange.OnPlayerDead -= _OnCharacterDead;
        OnMapEnergyChange.OnEnergyChange -= _OnMapEnergyChange;
        SpellCastHandler.OnApplyStatusEffect -= _OnStatusEffectApplied;
        ExpireStatusEffectHandler.OnEffectExpire -= _OnExpireEffect;
        RemoveTokenHandler.OnTokenRemove -= _OnMapTokenRemove;
        ExpireSpiritHandler.OnSpiritExpire -= _OnMapTokenRemove;

        if (!LocationIslandController.isInBattle)
        {
            MoveTokenHandler.OnTokenMove -= _OnMapTokenMove;
            BanishManager.OnBanished -= Abort;

            MapsAPI.Instance.allowControl = true;
            MapCameraUtils.FocusOnPosition(MapsAPI.Instance.mapCenter.position, m_PreviousMapZoom, true);
            MapCameraUtils.SetExtraFOV(0);
            MainUITransition.Instance.ShowMainUI();
            MarkerSpawner.HighlightMarker(new List<IMarker> { });
        }
        else
        {
            LocationUnitSpawner.EnableMarkers();
        }
    }

    private void _SetupDetails(SelectSpiritData_Map details)
    {
        if (details == null)
        {
            Abort();
            return;
        }

        if (string.IsNullOrEmpty(SpiritMarkerDetails.owner))
        {
            m_Desc.text = "";
            if (!LocationIslandController.isInBattle)
            { m_Desc.text = LocalizeLookUp.GetText("cast_spirit_knowledge"); }// "Defeating this spirit will give you the power to summon it.";
            if (m_SpiritData.tier == 1)
                m_Tier.text = LocalizeLookUp.GetText("ftf_wild_spirit") + " (" + LocalizeLookUp.GetText("cast_spirit_lesser") + ")";//"Wild Spirit (Lesser)";
            else if (m_SpiritData.tier == 2)
                m_Tier.text = LocalizeLookUp.GetText("ftf_wild_spirit") + " (" + LocalizeLookUp.GetText("cast_spirit_greater") + ")";//"Wild Spirit (Greater)";
            else if (m_SpiritData.tier == 3)
                m_Tier.text = LocalizeLookUp.GetText("ftf_wild_spirit") + " (" + LocalizeLookUp.GetText("cast_spirit_superior") + ")";//"Wild Spirit (Superior)";
            else
                m_Tier.text = LocalizeLookUp.GetText("ftf_wild_spirit") + " (" + LocalizeLookUp.GetText("cast_spirit_legendary") + ")";//"Wild Spirit (Legendary)";

            foreach (var item in PlayerDataManager.playerData.knownSpirits)
            {
                if (item.spirit.ToString() == m_SpiritData.id)
                {
                    m_Tier.text = m_Tier.text + "\n<color=#616161>" + LocalizeLookUp.GetText("spirit_known");
                    break;
                }
            }


        }
        else
        {
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

            Debug.Log(m_Energy.text);
            Debug.Log(m_Energy.text.Split('>')[1]);
            float currentEnergy = float.Parse(m_Energy.text.Split('>')[1]);
            //spirit at half health
            if (currentEnergy > SpiritToken.baseEnergy / 2 && newEnergy < SpiritToken.baseEnergy / 2)
            {
                PlayerNotificationManager.Instance.ShowNotification($"The <color=orange>{m_SpiritName.text}</color> is now at half health. Keep it up, witch!");
            }
            // spirit vulnerable
            if (currentEnergy > SpiritToken.baseEnergy * .2f && newEnergy < SpiritToken.baseEnergy * .2f)
            {
                PlayerNotificationManager.Instance.ShowNotification($"The <color=orange>{m_SpiritName.text}</color> is now <color=red>vulnerable!</color>");
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
                PlayerNotificationManager.Instance.ShowNotification($"The <color=orange>{m_SpiritName.text}</color> is fully Hexed and <color=red>vulnerable</color> to critical attacks.");

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
}
