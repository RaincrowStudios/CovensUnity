using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;

public class UISpiritInfo : UIInfoPanel
{
    [SerializeField] private UIConditionList m_ConditionList;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI m_SpiritName;
    [SerializeField] private TextMeshProUGUI m_Tier;
    [SerializeField] private TextMeshProUGUI m_Energy;
    [SerializeField] private TextMeshProUGUI m_Desc;
    [SerializeField] private TextMeshProUGUI m_CastText;

    [Header("Buttons")]
    [SerializeField] private Button m_InfoButton;
    [SerializeField] private Button m_DescButton;
    [SerializeField] private Button m_QuickBless;
    [SerializeField] private Button m_QuickSeal;
    [SerializeField] private Button m_QuickHex;
    [SerializeField] private Button m_CastButton;
    [SerializeField] private Button m_CloseButton;
    
    private static UISpiritInfo m_Instance;
    public static UISpiritInfo Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UISpiritInfo>("UISpiritInfo"));
            return m_Instance;
        }
    }

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

    private IMarker m_Spirit;
    private Token m_Token;
    private SpiritDict m_SpiritData;
    private MarkerDataDetail m_Details;

    private Vector3 m_PreviousMapPosition;
    private float m_PreviousMapZoom;

    public Token Spirit { get { return m_Token; } }

    protected override void Awake()
    {
        base.Awake();

        m_CastButton.onClick.AddListener(OnClickCast);
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_InfoButton.onClick.AddListener(OnClickInfo);

        m_QuickBless.onClick.AddListener(() => QuickCast("spell_bless"));
        m_QuickHex.onClick.AddListener(() => QuickCast("spell_hex"));
        m_QuickSeal.onClick.AddListener(() => QuickCast("spell_seal"));
    }

    public void Show(IMarker spirit, Token token)
    {
        if (isOpen)
            return;

        m_Spirit = spirit;
        m_Token = token;
        m_SpiritData = DownloadedAssets.spiritDictData[token.spiritId];
        m_Details = null;

        m_SpiritName.text = m_SpiritData.spiritName;

        m_DescButton.onClick.RemoveAllListeners();

        if (string.IsNullOrEmpty(token.owner))
        {
            if (m_SpiritData.spiritTier == 1)
                m_Tier.text = "Wild Spirit (Lesser)";
            else if (m_SpiritData.spiritTier == 2)
                m_Tier.text = "Wild Spirit (Greater)";
            else if (m_SpiritData.spiritTier == 3)
                m_Tier.text = "Wild Spirit (Superior)";
            else
                m_Tier.text = "Wild Spirit (Legendary)";

            m_Desc.text = "Defeating this spirit will give you the power to summon it.";
        }
        else
        {
            if (m_SpiritData.spiritTier == 1)
                m_Tier.text = "Lesser Spirit";
            else if (m_SpiritData.spiritTier == 2)
                m_Tier.text = "Greater Spirit";
            else if (m_SpiritData.spiritTier == 3)
                m_Tier.text = "Superior Spirit";
            else
                m_Tier.text = "Legendary Spirit";

            m_Desc.text = "Belongs to [Loading...]";
        }

        m_Energy.text = $"ENERGY <color=black>{token.energy}</color>";

        m_PreviousMapPosition = StreetMapUtils.CurrentPosition();
        m_PreviousMapZoom = MapController.Instance.zoom;

        spirit.SetTextAlpha(NewMapsMarker.highlightTextAlpha);
        MainUITransition.Instance.HideMainUI();

        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Spirit }, true);

        OnCharacterDeath.OnPlayerDead += _OnCharacterDead;
        OnMapEnergyChange.OnEnergyChange += _OnMapEnergyChange;
        OnMapConditionAdd.OnConditionAdded += _OnConditionAdd;
        OnMapConditionRemove.OnConditionRemoved += _OnConditionRemove;


        Show();
        m_ConditionList.show = false;
    }
    
    public override void ReOpen()
    {
        base.ReOpen();

        UpdateCanCast();
        
        MapController.Instance.allowControl = false;

        IMarker spirit = MarkerManager.GetMarker(m_Token.instance);

        //if the spirit was destroyed, close the ui
        if (spirit != null)
            StreetMapUtils.FocusOnTarget(m_Spirit);
        else
            OnClickClose();
    }

    public void SetupDetails(MarkerDataDetail details)
    {
        m_Details = details;

        if (string.IsNullOrEmpty(m_Token.owner) == false)
        {
            if (string.IsNullOrEmpty(details.ownerCoven))
            {
                m_Desc.text = $"Belongs to <color=black>{details.owner}</color>";
                m_DescButton.onClick.AddListener(OnClickOwner);
            }
            else
            {
                m_Desc.text = $"Belongs to Coven <color=black>{details.covenName}</color>";
                m_DescButton.onClick.AddListener(OnClickCoven);
            }
        }

        UpdateCanCast();
        m_ConditionList.Setup(m_Token, m_Details);
    }

    private void UpdateCanCast()
    {
        if (m_Details == null)
        {
            m_QuickBless.interactable = m_QuickHex.interactable = m_QuickSeal.interactable = m_CastButton.interactable = false;
            m_CastText.text = "Spellbook (Loading..)";
            return;
        }

        bool isSilenced = BanishManager.isSilenced;
        if (isSilenced)
            m_CastText.text = "You are silenced";
        else
            m_CastText.text = "Spellbook";

        m_QuickBless.interactable = m_QuickHex.interactable = m_QuickSeal.interactable = m_CastButton.interactable = isSilenced == false;

    }

    private void OnClickCast()
    {
        this.Hide();
        UISpellcasting.Instance.Show(m_Details, m_Spirit, PlayerDataManager.playerData.spells, () => { ReOpen(); });
        //StreetMapUtils.FocusOnTarget(PlayerManager.marker);
    }

    private void QuickCast(string spellId)
    {
        foreach (SpellData spell in PlayerDataManager.playerData.spells)
        {
            if (spell.id == spellId)
            {
                //StreetMapUtils.FocusOnTarget(PlayerManager.marker);

                Hide();

                //send the cast
                Spellcasting.CastSpell(spell, m_Spirit, new List<spellIngredientsData>(), (result) =>
                {
                    //return to the spirit UI no matter the result
                    ReOpen();
                });
                return;
            }
        }
    }

    private void OnClickClose()
    {
        OnCharacterDeath.OnPlayerDead -= _OnCharacterDead;
        OnMapEnergyChange.OnEnergyChange -= _OnMapEnergyChange;
        OnMapConditionAdd.OnConditionAdded -= _OnConditionAdd;
        OnMapConditionRemove.OnConditionRemoved -= _OnConditionRemove;
        //OnCharacterSpiritBanished.OnSpiritBanished -= _OnSpiritBanished;

        MainUITransition.Instance.ShowMainUI();
        MapController.Instance.allowControl = true;
        StreetMapUtils.FocusOnPosition(m_PreviousMapPosition, true, m_PreviousMapZoom, true);
        m_Spirit.SetTextAlpha(NewMapsMarker.defaultTextAlpha);
        MainUITransition.Instance.ShowMainUI();

        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Spirit }, false);

        Close();
    }

    private void OnClickInfo()
    {
        UIDetailedSpiritInfo.Instance.Show(m_SpiritData, m_Token);
    }

    private void OnClickOwner()
    {
        Debug.Log("TODO: go to player");
    }

    private void OnClickCoven()
    {
        Debug.Log("TODO: Open coven");
    }



    private void Abort()
    {
        if (UISpellcasting.isOpen)
            UISpellcasting.Instance.FinishSpellcastingFlow();

        if (UIWaitingCastResult.isOpen)
            UIWaitingCastResult.Instance.OnClickContinue();

        OnClickClose();
    }

    private void _OnCharacterDead(string name, string spirit)
    {
        Abort();
    }

    private void _OnMapEnergyChange(string instance, int newEnergy)
    {
        if (instance == m_Token.instance)
        {
            m_Energy.text = $"ENERGY <color=black>{newEnergy}</color>";

            if(newEnergy == 0)
            {
                //let the player see the result of his spellcasting
                if (UIWaitingCastResult.isOpen)
                    return;

                //if he is not waiting for the result, just close the ui
                Abort();
            }
        }
    }

    private void _OnConditionAdd(Conditions condition)
    {
        if (condition.bearer != this.m_Token.instance)
            return;

        m_ConditionList.AddCondition(condition);
    }

    private void _OnConditionRemove(Conditions condition)
    {
        if (condition.bearer != this.m_Token.instance)
            return;

        m_ConditionList.RemoveCondition(condition);
    }

    //private void _OnSpiritBanished(string instance, string killerName)
    //{
    //    Debug.Log("_onspiritbanished: " + instance + " < " + killerName + " > " + m_Token.instance + " : " + UIWaitingCastResult.isOpen);
    //    if (instance == m_Token.instance)
    //    {
    //        //let the player see the result of his spellcasting
    //        if (UIWaitingCastResult.isOpen)
    //            return;

    //        //if he is not waiting for the result, just close the ui
    //        Abort();
    //    }
    //}
}
