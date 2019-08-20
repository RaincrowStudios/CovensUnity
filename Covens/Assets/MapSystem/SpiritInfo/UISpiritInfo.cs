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
    [SerializeField] private TextMeshProUGUI m_Level;
    [SerializeField] private TextMeshProUGUI m_Energy;
    [SerializeField] private TextMeshProUGUI m_Desc;

    [Header("Buttons")]
    [SerializeField] private Button m_InfoButton;
    [SerializeField] private Button m_OwnerButton;
    [SerializeField] private Button m_CloseButton;

    private static UISpiritInfo m_Instance;

    public static bool isOpen
    {
        get
        {
            if (m_Instance == null)
                return false;
            else
                return m_Instance.IsShowing;
        }
    }

    public static void Show(SpiritMarker spirit, SpiritToken data, System.Action onClose = null)
    {
        if (m_Instance != null)
        {
            m_Instance._Show(spirit, data, onClose);
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
                    LoadingOverlay.Hide();
                });
        }
    }

    public static void SetupDetails(SelectSpiritData_Map data)
    {
        if (m_Instance == null)
            return;

        m_Instance._SetupDetails(data);
    }

    private SpiritData m_SpiritData;
    private SelectSpiritData_Map m_SpiritDetails;
    private System.Action m_OnClose;
    private float m_PreviousMapZoom;

    public static SpiritMarker SpiritMarker { get; private set; }
    public static SpiritToken SpiritToken { get; private set; }

    protected override void Awake()
    {
        m_Instance = this;

        base.Awake();

        m_CloseButton.onClick.AddListener(OnClickClose);
        m_InfoButton.onClick.AddListener(OnClickInfo);
    }

    public void _Show(IMarker spirit, Token token, System.Action onClose)
    {
        if (isOpen)
            return;

        m_OnClose = onClose;
        SpiritMarker = spirit as SpiritMarker;
        SpiritToken = token as SpiritToken;
        m_SpiritData = DownloadedAssets.spiritDict[SpiritToken.spiritId];
        m_SpiritDetails = null;
        m_OwnerButton.onClick.RemoveAllListeners();
        
        m_SpiritName.text = m_SpiritData.Name;
        m_Level.text = "";
        m_Energy.text = LocalizeLookUp.GetText("cast_energy").ToUpper() + " <color=black>" + SpiritToken.energy.ToString() + " / " + SpiritToken.baseEnergy.ToString() + "</color>";
        m_Desc.text = LocalizeLookUp.GetText("location_owned").Replace("{{Controller}}", "[" + LocalizeLookUp.GetText("loading") + "]");//"Belongs to [Loading...]";

        m_SpiritArt.overrideSprite = null;
        DownloadedAssets.GetSprite(SpiritToken.spiritId, m_SpiritArt);

        string tier;
        switch (m_SpiritData.tier)
        {
            case 1: tier = LocalizeLookUp.GetText("cast_spirit_lesser"); break;
            case 2: tier = LocalizeLookUp.GetText("cast_spirit_greater"); break;
            case 3: tier = LocalizeLookUp.GetText("cast_spirit_superior"); break;
            case 4: tier = LocalizeLookUp.GetText("cast_spirit_legendary"); break;
            default: tier = "?"; break;
        };
        m_Tier.text = tier;

        previousMapPosition = MapsAPI.Instance.GetWorldPosition();
        m_PreviousMapZoom = Mathf.Min(0.98f, MapsAPI.Instance.normalizedZoom);


        OnMapEnergyChange.OnPlayerDead += _OnCharacterDead;
        OnMapEnergyChange.OnEnergyChange += _OnMapEnergyChange;
        SpellCastHandler.OnApplyStatusEffect += _OnStatusEffectApplied;
        RemoveTokenHandler.OnTokenRemove += _OnMapTokenRemove;
        BanishManager.OnBanished += Abort;

        if (!LocationIslandController.isInBattle)
        {
            MainUITransition.Instance.HideMainUI();
            MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, SpiritMarker });
            MoveTokenHandler.OnTokenMove += _OnMapTokenMove;
        }

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
                MapCameraUtils.FocusOnMarker(spirit.GameObject.transform.position);
            else
                Close();
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

        base.Close();

        OnMapEnergyChange.OnPlayerDead -= _OnCharacterDead;
        OnMapEnergyChange.OnEnergyChange -= _OnMapEnergyChange;

        if (!LocationIslandController.isInBattle)
        {
            MoveTokenHandler.OnTokenMove -= _OnMapTokenMove;
            SpellCastHandler.OnApplyStatusEffect -= _OnStatusEffectApplied;
            RemoveTokenHandler.OnTokenRemove -= _OnMapTokenRemove;
            BanishManager.OnBanished -= Abort;

            MapsAPI.Instance.allowControl = true;
            MapCameraUtils.FocusOnPosition(MapsAPI.Instance.mapCenter.position, m_PreviousMapZoom, true);
            MainUITransition.Instance.ShowMainUI();
            MarkerSpawner.HighlightMarker(new List<IMarker> { });
        }
        else
        {
            LocationUnitSpawner.EnableMarkers();
        }

    }

    public void _SetupDetails(SelectSpiritData_Map details)
    {
        if (details == null)
        {
            Abort();
            return;
        }

        m_SpiritDetails = details;

        if (string.IsNullOrEmpty(m_SpiritDetails.owner))
        {
            if (m_SpiritData.tier == 1)
                m_Tier.text = LocalizeLookUp.GetText("ftf_wild_spirit") + " (" + LocalizeLookUp.GetText("cast_spirit_lesser") + ")";//"Wild Spirit (Lesser)";
            else if (m_SpiritData.tier == 2)
                m_Tier.text = LocalizeLookUp.GetText("ftf_wild_spirit") + " (" + LocalizeLookUp.GetText("cast_spirit_greater") + ")";//"Wild Spirit (Greater)";
            else if (m_SpiritData.tier == 3)
                m_Tier.text = LocalizeLookUp.GetText("ftf_wild_spirit") + " (" + LocalizeLookUp.GetText("cast_spirit_superior") + ")";//"Wild Spirit (Superior)";
            else
                m_Tier.text = LocalizeLookUp.GetText("ftf_wild_spirit") + " (" + LocalizeLookUp.GetText("cast_spirit_legendary") + ")";//"Wild Spirit (Legendary)";

            m_Desc.text = LocalizeLookUp.GetText("cast_spirit_knowledge");// "Defeating this spirit will give you the power to summon it.";
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

        m_ConditionList.Setup(m_SpiritDetails.effects);
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
        TeamManagerUI.OpenName(m_SpiritDetails.coven);
    }

    private void Abort()
    {
        Close();
    }

    private void UISpellcasting_OnCastResult()
    {
        //if token is gone
        if (MarkerSpawner.GetMarker(SpiritToken.instance) == null)
        {
            Close();
        }
    }

    private void UISpellcasting_OnClickClose()
    {
        //close this too
        Close();
    }

    private void _OnCharacterDead()
    {
        Abort();
    }

    private void _OnMapTokenMove(string instance, Vector3 position)
    {
        if (SpiritToken.instance == instance)
        {
            MapCameraUtils.FocusOnMarker(position);
        }
    }

    private void _OnMapEnergyChange(string instance, int newEnergy)
    {
        if (instance == SpiritToken.instance)
        {
            m_Energy.text = LocalizeLookUp.GetText("card_witch_energy").ToUpper() + " <color=black>" + newEnergy.ToString() + "</color>";

            if (newEnergy == 0)
            {
                Abort();
            }
        }
    }

    private void _OnStatusEffectApplied(string character, StatusEffect statusEffect)
    {
        if (character != SpiritToken.instance)
            return;

        foreach (StatusEffect item in m_SpiritDetails.effects)
        {
            if (item.spell == statusEffect.spell)
            {
                m_SpiritDetails.effects.Remove(item);
                break;
            }
        }
        m_SpiritDetails.effects.Add(statusEffect);
        m_ConditionList.AddCondition(statusEffect);
    }

    private void _OnMapTokenRemove(string instance)
    {
        if (instance == SpiritToken.instance)
        {
            Abort();
            //UIGlobalErrorPopup.ShowPopUp(null, LocalizeLookUp.GetText("spellbook_witch_is_gone").Replace("{{witch name}}", m_SpiritData.spiritName));// + " is gone.");
        }
    }
}
