using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Maps;

public class UIPlayerInfo : UIInfoPanel
{
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
                return m_Instance.IsShowing;
        }
    }

    private IMarker m_Witch;
    private Token m_WitchData;
    private MarkerDataDetail m_Details;
    private Vector3 m_PreviousMapPosition;
    private float m_PreviousMapZoom;

    public Token Witch { get { return m_WitchData; } }


    protected override void Awake()
    {
        m_Instance = this;

        m_CloseButton.onClick.AddListener(OnClickClose);
        m_CovenButton.onClick.AddListener(OnClickCoven);
        m_CastButton.onClick.AddListener(OnClickCast);

        m_QuickBless.onClick.AddListener(() => QuickCast("spell_bless"));
        m_QuickHex.onClick.AddListener(() => QuickCast("spell_hex"));
        m_QuickSeal.onClick.AddListener(() => QuickCast("spell_seal"));

        base.Awake();
    }

    public void Show(IMarker witch, Token data)
    {
        if (IsShowing)
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

        MarkerSpawner.Instance.SetMarkersScale(true);
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

        //if (m_HighlightedMarkers.Count == 0)
        //    m_HighlightedMarkers.Add(PlayerManager.marker);
        //m_HighlightedMarkers.Add(witch);

        //StreetMapUtils.Highlight(m_HighlightedMarkers.ToArray());
        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Witch }, true);

        witch.SetTextAlpha(NewMapsMarker.highlightTextAlpha);

        OnMapSpellcast.OnPlayerTargeted += OnPlayerAttacked;
    }

    public override void ReOpen()
    {
        base.ReOpen();

        UpdateCanCast();

        MapController.Instance.allowControl = false;
        StreetMapUtils.FocusOnTarget(m_Witch);
    }

    public void SetupDetails(MarkerDataDetail details)
    {
        m_Details = details;

        m_CovenButton.interactable = !string.IsNullOrEmpty(m_Details.covenName);
        m_CovenText.text = m_CovenButton.interactable ? $"COVEN <color=black>{details.covenName}</color>" : "No coven";
    }

    private void OnClickClose()
    {
        MainUITransition.Instance.ShowMainUI();
        MapController.Instance.allowControl = true;
        StreetMapUtils.FocusOnPosition(m_PreviousMapPosition, true, m_PreviousMapZoom, true);

        m_Witch.SetTextAlpha(NewMapsMarker.defaultTextAlpha);
        MarkerSpawner.Instance.SetMarkersScale(false);

        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Witch }, false);

        OnMapSpellcast.OnPlayerTargeted -= OnPlayerAttacked;

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
                Spellcasting.CastSpell(spell, m_Witch, new List<spellIngredientsData>(), (result) =>
                {
                    //if the spell backfired, the camera is focusing on the player
                    if (result.effect == "backfire")
                        StreetMapUtils.FocusOnTarget(m_Witch);
                    ReOpen();
                });
                return;
            }
        }
    }

    private void UpdateCanCast()
    {
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
    }

    private void OnPlayerAttacked(IMarker caster, SpellDict spell, Result result)
    {
        if (caster == m_Witch)
        {
            UpdateCanCast();
        }
    }
}
