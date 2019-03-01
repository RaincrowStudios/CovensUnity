using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;

public class UISpiritInfo : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_Panel;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI m_SpiritName;
    [SerializeField] private TextMeshProUGUI m_Owner;
    [SerializeField] private TextMeshProUGUI m_Tier;
    [SerializeField] private TextMeshProUGUI m_Energy;
    [SerializeField] private TextMeshProUGUI m_Coven;
    [SerializeField] private TextMeshProUGUI m_CastText;
    
    [Header("Buttons")]
    [SerializeField] private Button m_InfoButton;
    [SerializeField] private Button m_PlayerButton;
    [SerializeField] private Button m_CovenButton;
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
                return m_Instance.m_InputRaycaster.enabled;
        }
    }

    private IMarker m_Spirit;
    private Token m_Token;
    private SpiritDict m_SpiritData;
    private MarkerDataDetail m_Details;

    private Vector3 m_PreviousMapPosition;
    private float m_PreviousMapZoom;
    private int m_TweenId;

    public Token Spirit { get { return m_Token; } }

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        m_CastButton.onClick.AddListener(OnClickCast);
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_InfoButton.onClick.AddListener(OnClickInfo);
        m_PlayerButton.onClick.AddListener(OnClickOwner);
        m_CovenButton.onClick.AddListener(OnClickCoven);

        m_QuickBless.onClick.AddListener(() => QuickCast("spell_bless"));
        m_QuickHex.onClick.AddListener(() => QuickCast("spell_hex"));
        m_QuickSeal.onClick.AddListener(() => QuickCast("spell_seal"));
    }

    public void Show(IMarker spirit, Token token)
    {
        if (m_InputRaycaster.enabled)
            return;
        
        m_Spirit = spirit;
        m_Token = token;
        m_SpiritData = DownloadedAssets.spiritDictData[token.spiritId];
        m_Details = null;
        
        m_SpiritName.text = m_SpiritData.spiritName;

        if (m_SpiritData.spiritTier == 1)
            m_Tier.text = "LESSER SPIRIT";
        else if (m_SpiritData.spiritTier == 2)
            m_Tier.text = "GREATER SPIRIT";
        else if (m_SpiritData.spiritTier == 3)
            m_Tier.text = "SUPERIOR SPIRIT";
        else
            m_Tier.text = "LEGENDARY SPIRIT";

        m_Energy.text = $"ENERGY <color=black>{token.energy}</color>";

        m_PlayerButton.interactable = false;
        m_CovenButton.interactable = false;

        if (string.IsNullOrEmpty(token.owner))
        {
            m_Owner.text = "WILD";
            m_Coven.text = "";
        }
        else
        {
            m_Owner.text = "Owner: Loading...";
            m_Coven.text = "Coven: Loading...";
        }
        
        m_PreviousMapPosition = StreetMapUtils.CurrentPosition();
        m_PreviousMapZoom = MapController.Instance.zoom;

        ReOpen();

        MainUITransition.Instance.HideMainUI();
    }

    private void Close()
    {
        if (m_InputRaycaster.enabled == false)
            return;
        
        m_InputRaycaster.enabled = false;
        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setOnUpdate((float t) =>
            {
                m_Panel.anchoredPosition = new Vector2(t * m_Panel.sizeDelta.x, 0);
                m_CanvasGroup.alpha = 1 - t;
            })
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
            })
            .setEaseOutCubic()
            .uniqueId;
    }

    public void ReOpen()
    {
        m_InputRaycaster.enabled = true;
        m_Canvas.enabled = true;

        bool isSilenced = BanishManager.isSilenced;
        if (isSilenced)
            m_CastText.text = "You are silenced";
        else
            m_CastText.text = "Spellbook";

        m_CastButton.interactable = isSilenced == false;
        m_QuickBless.interactable = m_QuickHex.interactable = m_QuickSeal.interactable = m_CastButton.interactable;

        //animate
        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setOnUpdate((float t) =>
            {
                m_Panel.anchoredPosition = new Vector2((1 - t) * m_Panel.sizeDelta.x, 0);
                m_CanvasGroup.alpha = t;
            })
            .setEaseOutCubic()
            .uniqueId;

        MapController.Instance.allowControl = false;
        StreetMapUtils.FocusOnTarget(m_Spirit);
    }

    public void SetupDetails(MarkerDataDetail details)
    {
        m_Details = details;

        if(string.IsNullOrEmpty(m_Token.owner) == false)
        {
            m_Owner.text = $"Summoned by <color=black>{details.owner}</color>";
            m_Coven.text = string.IsNullOrEmpty(details.ownerCoven) ? "NO COVEN" : $"COVEN <color=black>{details.covenName}</color>";

            m_PlayerButton.interactable = false;
            m_CovenButton.interactable = !string.IsNullOrEmpty(details.ownerCoven);
        }
    }

    private void OnClickCast()
    {
        this.Close();
        UISpellcasting.Instance.Show(m_Spirit, PlayerDataManager.playerData.spells, () => { ReOpen(); });
        StreetMapUtils.FocusOnTarget(PlayerManager.marker);
    }

    private void QuickCast(string spellId)
    {
        foreach (SpellData spell in PlayerDataManager.playerData.spells)
        {
            if (spell.id == spellId)
            {
                StreetMapUtils.FocusOnTarget(PlayerManager.marker);

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

        Close();
        MainUITransition.Instance.ShowMainUI();
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
