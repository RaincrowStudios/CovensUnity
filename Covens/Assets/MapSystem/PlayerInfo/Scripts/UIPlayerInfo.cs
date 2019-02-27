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

    [Header("Sigils")]
    [SerializeField] private Color m_ShadowSchoolColor;
    [SerializeField] private Color m_GreySchoolColor;
    [SerializeField] private Color m_WhiteSchoolColor;

    [SerializeField] private Sprite m_ShadowSigilSprite;
    [SerializeField] private Sprite m_GreySigilSprite;
    [SerializeField] private Sprite m_WhiteSigilSprite;

    [Header("Images")]
    [SerializeField] private Image m_Sigil;
    [SerializeField] private Image m_SeparatorA;
    [SerializeField] private Image m_SeparatorB;
    [SerializeField] private GameObject m_ImmunityOverlay;
    [SerializeField] private GameObject m_SilencedOverlay;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI m_DisplayNameText;
    [SerializeField] private TextMeshProUGUI m_DegreeSchoolText;
    [SerializeField] private TextMeshProUGUI m_CovenText;
    [SerializeField] private TextMeshProUGUI m_DominionRankText;
    [SerializeField] private TextMeshProUGUI m_WorldRankText;

    [Header("Buttons")]
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private Button m_CovenButton;
    [SerializeField] private Button m_CastButton;

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
        m_DegreeSchoolText.text = "degree: " + m_WitchData.degree;

        //sprite and color
        Color color;
        if (m_WitchData.degree < 0)
        {
            m_Sigil.sprite = m_ShadowSigilSprite;
            color = m_ShadowSchoolColor;
        }
        else if (m_WitchData.degree > 0)
        {
            m_Sigil.sprite = m_WhiteSigilSprite;
            color = m_WhiteSchoolColor;
        }
        else
        {
            m_Sigil.sprite = m_GreySigilSprite;
            color = m_GreySchoolColor;
        }
        m_Sigil.color = m_SeparatorA.color = m_SeparatorB.color = color;
        color.a = 0.7f;
        m_DegreeSchoolText.color = color;

        m_CovenButton.interactable = false;
        m_CovenText.text = "Coven: Loading...";
        m_DominionRankText.text = "Dominion Rank: Loading...";
        m_WorldRankText.text = "World Rank: Loading...";

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
        m_ImmunityOverlay.SetActive(isWitchImmune);

        bool isSilenced = BanishManager.isSilenced;
        m_SilencedOverlay.SetActive(isSilenced);

        m_CastButton.interactable = isWitchImmune == false && isSilenced == false;

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
        StreetMapUtils.FocusOnTarget(m_Witch, 9);
    }

    public void SetupDetails(MarkerDataDetail details)
    {
        m_Details = details;

        m_CovenButton.interactable = !string.IsNullOrEmpty(m_Details.covenName);
        m_CovenText.text = m_CovenButton.interactable ? "Coven: " + m_Details.coven : "No coven";
        m_DominionRankText.text = "Dominion Rank: " + details.dominionRank;
        m_WorldRankText.text = "World Rank: " + details.worldRank;
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
        StreetMapUtils.FocusOnTarget(PlayerManager.marker, 9);
    }
}
