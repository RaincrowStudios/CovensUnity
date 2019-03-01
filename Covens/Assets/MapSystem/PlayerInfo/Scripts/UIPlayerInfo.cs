using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;

public class UIPlayerInfo : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    
    [SerializeField] private Sprite m_ShadowSigilSprite;
    [SerializeField] private Sprite m_GreySigilSprite;
    [SerializeField] private Sprite m_WhiteSigilSprite;

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
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private Button m_CovenButton;
    [SerializeField] private Button m_CastButton;
    [SerializeField] private Button m_QuickBless;
    [SerializeField] private Button m_QuickSeal;
    [SerializeField] private Button m_QuickHex;

    [Header("Animation")]
    [SerializeField] private CanvasGroup m_MainCanvasGroup;
    [SerializeField] private RectTransform m_MainPanel;
    
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
                return m_Instance.m_InputRaycaster.enabled;
        }
    }

    private IMarker m_Witch;
    private Token m_WitchData;
    private MarkerDataDetail m_Details;
    private int m_TweenId;
    private Vector3 m_PreviousMapPosition;
    private float m_PreviousMapZoom;
    private List<IMarker> m_HighlightedMarkers = new List<IMarker>();

    public bool IsOpen { get { return m_Canvas.enabled; } }
    public Token Witch { get { return m_WitchData; } }


    private void Awake()
    {
        m_Instance = this;

        m_CloseButton.onClick.AddListener(OnClickClose);
        m_CovenButton.onClick.AddListener(OnClickCoven);
        m_CastButton.onClick.AddListener(OnClickCast);

        m_QuickBless.onClick.AddListener(() => QuickCast("spell_bless"));
        m_QuickHex.onClick.AddListener(() => QuickCast("spell_hex"));
        m_QuickSeal.onClick.AddListener(() => QuickCast("spell_seal"));

        m_MainPanel.anchoredPosition = new Vector2(m_MainPanel.sizeDelta.x, 0);
        m_MainCanvasGroup.alpha = 0;
        m_InputRaycaster.enabled = false;
        m_Canvas.enabled = false;
    }

    public void Show(IMarker witch, Token data)
    {
        if (m_Canvas.enabled)
            return;

        if (witch == null)
        {
            Debug.LogError("null witch");
            return;
        }

        MainUITransition.Instance.HideMainUI();

        m_Witch = witch;
        m_WitchData = data;
        m_Details = null;

        //setup the ui
        m_DisplayNameText.text = m_WitchData.displayName;
        m_DegreeSchoolText.text = "degree " + m_WitchData.degree;
        m_LevelText.text = $"LEVEL <color=black>{data.level}</color>";
        m_EnergyText.text = $"ENERGY <color=black>{data.energy}</color>";

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

        m_CovenButton.interactable = false;
        m_CovenText.text = $"COVEN <color=black>Loading...</color>";

        m_PreviousMapPosition = StreetMapUtils.CurrentPosition();
        m_PreviousMapZoom = MapController.Instance.zoom;

        ReOpen();

        if (m_HighlightedMarkers.Count == 0)
            m_HighlightedMarkers.Add(PlayerManager.marker);
        m_HighlightedMarkers.Add(witch);

        witch.SetTextAlpha(NewMapsMarker.highlightTextAlpha);
        StreetMapUtils.Highlight(m_HighlightedMarkers.ToArray());
    }

    public void ReOpen()
    {
        m_InputRaycaster.enabled = true;
        m_Canvas.enabled = true;

        bool isWitchImmune = MarkerSpawner.IsPlayerImmune(m_WitchData.instance);
        bool isSilenced = BanishManager.isSilenced;
        
        m_CastButton.interactable = isWitchImmune == false && isSilenced == false;
        m_QuickBless.interactable = m_QuickHex.interactable = m_QuickSeal.interactable = m_CastButton.interactable;

        if (isWitchImmune)
            m_CastText.text = "Player is immune to you";
        else if (isSilenced)
            m_CastText.text = "You are silenced";
        else
            m_CastText.text = "Spellbook";

        //animate
        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setOnUpdate((float t) =>
            {
                m_MainPanel.anchoredPosition = new Vector2((1 - t) * m_MainPanel.sizeDelta.x, 0);
                m_MainCanvasGroup.alpha = t;
            })
            .setEaseOutCubic()
            .uniqueId;

        MapController.Instance.allowControl = false;
        StreetMapUtils.FocusOnTarget(m_Witch);
    }

    public void SetupDetails(MarkerDataDetail details)
    {
        m_Details = details;

        m_CovenButton.interactable = !string.IsNullOrEmpty(m_Details.covenName);
        m_CovenText.text = m_CovenButton.interactable ? $"COVEN <color=black>{details.covenName}</color>" : "No coven";
    }

    public void Close()
    {
        if (m_InputRaycaster.enabled == false)
            return;

        m_InputRaycaster.enabled = false;
        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setOnUpdate((float t) =>
            {
                m_MainPanel.anchoredPosition = new Vector2(t * m_MainPanel.sizeDelta.x, 0);
                m_MainCanvasGroup.alpha = 1 - t;
            })
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
            })
            .setEaseOutCubic()
            .uniqueId;
    }

    private void OnClickClose()
    {
        MainUITransition.Instance.ShowMainUI();
        MapController.Instance.allowControl = true;
        StreetMapUtils.FocusOnPosition(m_PreviousMapPosition, true, m_PreviousMapZoom, true);

        m_Witch.SetTextAlpha(NewMapsMarker.defaultTextAlpha);
        StreetMapUtils.DisableHighlight(m_HighlightedMarkers.ToArray());
        m_HighlightedMarkers.Clear();

        Close();
    }

    private void OnClickCoven()
    {
        Debug.Log("TODO: Open coven");
    }

    private void OnClickCast()
    {
        this.Close();
        UISpellcasting.Instance.Show(m_Witch, PlayerDataManager.playerData.spells, () => { ReOpen(); });
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
                Spellcasting.CastSpell(spell, m_Witch, new List<spellIngredientsData>(), (result) =>
                {
                    //return to the witch UI no matter the result
                    ReOpen();
                });
                return;
            }
        }
    }
}
