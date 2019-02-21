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
    [SerializeField] private Vector2 m_FocusOffsetPosition = new Vector2(0.5f, 0.5f);

    public static UIPlayerInfo Instance { get; private set; }

    private IMarker m_Witch;
    private Token m_WitchData;
    private MarkerDataDetail m_Details;
    private int m_TweenId;

    public bool IsOpen { get { return m_Canvas.enabled; } }
    public Token Witch { get { return m_WitchData; } }

    private void Awake()
    {
        Instance = this;

        m_CloseButton.onClick.AddListener(Close);
        m_CovenButton.onClick.AddListener(OnClickCoven);
        m_CastButton.onClick.AddListener(OnClickCast);

        m_MainPanel.anchoredPosition = new Vector2(m_MainPanel.sizeDelta.x, 0);
        m_MainCanvasGroup.alpha = 0;
        m_InputRaycaster.enabled = false;
        m_Canvas.enabled = false;
    }

    public void Show(IMarker witch)
    {
        if (witch == null)
        {
            Debug.LogError("null witch");
            return;
        }

        m_Witch = witch;
        m_WitchData = witch.customData as Token;
        m_Details = null;

        //setup the ui
        m_DisplayNameText.text = m_WitchData.displayName;
        m_DegreeSchoolText.text = "degree: " + m_WitchData.degree;

        //sprite and color
        Color color;
        if(m_WitchData.degree < 0)
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

        m_InputRaycaster.enabled = true;
        m_Canvas.enabled = true;

        //animate
        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setOnUpdate((float t) =>
            {
                m_MainPanel.anchoredPosition = new Vector2((1 - t) * m_MainPanel.sizeDelta.x, 0);
                m_MainCanvasGroup.alpha = t;
            })
            .setEaseOutCubic()
            .uniqueId;
        StreetMapUtils.FocusOnTarget(witch, m_FocusOffsetPosition, 0);
    }

    public void SetupDetails(MarkerDataDetail details)
    {
        m_Details = details;

        m_CovenButton.interactable = !string.IsNullOrEmpty(m_Details.coven);
        m_CovenText.text = m_CovenButton.interactable ? "Coven: " + m_Details.coven : "No coven";
        m_DominionRankText.text = "Dominion Rank: " + details.dominionRank;
        m_WorldRankText.text = "World Rank: " + details.worldRank;
    }

    public void Close()
    {
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

    private void OnClickCoven()
    {
    }

    private void OnClickCast()
    {
        this.Close();
        UISpellcasting.Instance.Show(m_Witch, PlayerDataManager.playerData.spells);
    }
}
