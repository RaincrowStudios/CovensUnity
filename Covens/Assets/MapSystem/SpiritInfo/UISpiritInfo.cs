using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;

public class UISpiritInfo : UIInfoPanel
{
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

        ReOpen();
        spirit.SetTextAlpha(NewMapsMarker.highlightTextAlpha);
        MainUITransition.Instance.HideMainUI();

        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Spirit }, true);
    }
    
    public override void ReOpen()
    {
        base.ReOpen();

        UpdateCanCast();

        m_QuickBless.interactable = m_QuickHex.interactable = m_QuickSeal.interactable = m_CastButton.interactable;
        
        MapController.Instance.allowControl = false;
        StreetMapUtils.FocusOnTarget(m_Spirit);
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

        m_CastButton.interactable = isSilenced == false;
    }

    private void OnClickCast()
    {
        this.Close();
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

                Close();

                //send the cast
                Spellcasting.CastSpell(spell, m_Spirit, new List<spellIngredientsData>(), (result) =>
                {
                    //return to the witch UI no matter the result
                    ReOpen();
                });
                return;
            }
        }
    }

    private void OnClickClose()
    {
        MainUITransition.Instance.ShowMainUI();
        MapController.Instance.allowControl = true;
        StreetMapUtils.FocusOnPosition(m_PreviousMapPosition, true, m_PreviousMapZoom, true);
        m_Spirit.SetTextAlpha(NewMapsMarker.defaultTextAlpha);
        Close();
        MainUITransition.Instance.ShowMainUI();

        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Spirit }, false);
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
}
