using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Raincrow.Maps;
using Raincrow.GameEventResponses;

public class UIPortalInfo : UIInfoPanel
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI m_Timer;
    [SerializeField] private TextMeshProUGUI m_EnergyText;
    [SerializeField] private TextMeshProUGUI m_AddText;
    [SerializeField] private TextMeshProUGUI m_RemoveText;
    [SerializeField] private TextMeshProUGUI o_CreatedBy;

    [Header("Buttons")]
    [SerializeField] private Button m_AddButton;
    [SerializeField] private Button m_RemoveButton;
    [SerializeField] private Button m_CastButton;
    [SerializeField] private Button m_BackButton;
    [SerializeField] private Button m_CloseButton;

    [Header("Other")]
    [SerializeField] private CanvasGroup m_BluePulse;
    [SerializeField] private CanvasGroup m_RedPulse;
    [SerializeField] private Color m_DefaultColor;
    [SerializeField] private Color m_AddColor;
    [SerializeField] private Color m_RemoveColor;


    private static UIPortalInfo m_Instance;
    public static UIPortalInfo Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UIPortalInfo>("UIPortalInfo"));
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

    private const float m_CycleDuration = 0.5f;
    private const float m_EnergyCostPerCycle = 1;
    private const float m_EnergyIncreasePerCycle = 25;
    private const float m_EnergyDecreasePerCycle = 25;

    private IMarker m_Marker;
    private Token m_MarkerData;
    private PortalMarkerData m_Data;

    //private Coroutine m_EnergyCoroutine;
    private float m_EnergyAcumulated;
    private float m_LastAddTime;
    private float m_LastRemoveTime;

    private int m_AddTweenId;
    private int m_RemoveTweenId;

    private float m_PreviousMapZoom;

    private bool m_WaitingResult = false;
    public Token token { get { return m_MarkerData; } }

    protected override void Awake()
    {
        base.Awake();

        m_BluePulse.alpha = 0;
        m_RedPulse.alpha = 0;

        m_BackButton.onClick.AddListener(OnClickClose);
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_CastButton.onClick.AddListener(OnClickCast);

        m_AddButton.onClick.AddListener(OnClickAddEnergy);
        m_RemoveButton.onClick.AddListener(OnClickRemoveEnergy);
    }

    public void Show(IMarker marker, Token data)
    {
        if (IsShowing)
            return;

        m_Marker = marker;
        m_MarkerData = data;
        m_Data = null;

        m_CastButton.interactable = false;
        m_EnergyText.text = "???";
        m_Timer.text = LocalizeLookUp.GetText("portal_summon_in").Replace("{{count down}}", "\n<size=55>???");//$"Summons in ???";
        m_EnergyText.color = m_DefaultColor;
        o_CreatedBy.text = string.Concat(LocalizeLookUp.GetText("summoning_created_by"), ": ???");//, data.owner);


        previousMapPosition = MapsAPI.Instance.GetWorldPosition();
        m_PreviousMapZoom = MapsAPI.Instance.normalizedZoom;

        MainUITransition.Instance.HideMainUI();
        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Marker }, true);

        OnMapPortalSummon.OnPortalSummoned += _OnMapPortalSummoned;
        RemoveTokenHandler.OnTokenRemove += _OnMapTokenRemove;
        SpellCastHandler.OnSpellCast += _OnMapSpellCast;
        OnMapEnergyChange.OnEnergyChange += _OnMapEnergyChange;
        OnMapEnergyChange.OnPlayerDead += _OnCharacterDead;
        BanishManager.OnBanished += _OnCharacterDead;

        Show();
    }

    public override void ReOpen()
    {
        base.ReOpen();

        UpdateCanCast();
        MapsAPI.Instance.allowControl = false;
        MapCameraUtils.FocusOnMarker(m_Marker.gameObject.transform.position);
        m_InputRaycaster.enabled = true;
    }

    public void SetupDetails(PortalMarkerData data)
    {
        m_Data = data;
        m_EnergyText.text = $"{data.energy}";
        o_CreatedBy.text = string.Concat(LocalizeLookUp.GetText("summoning_created_by"), ": ", data.owner);


        StartCoroutine(SummonTimerCoroutine());
    }

    private IEnumerator SummonTimerCoroutine()
    {
        while (true)
        {
            var span = System.DateTime.UtcNow.Subtract(Utilities.FromJavaTime(m_Data.summonOn));
            if (span.TotalSeconds > 0)
                break;
            m_Timer.text = LocalizeLookUp.GetText("portal_summon_in").Replace("{{count down}}", "\n<size=55>" + Utilities.GetSummonTime(m_Data.summonOn));
            yield return new WaitForSecondsRealtime(1.001f);
        }
    }

    private void OnClickAddEnergy()
    {
        if (Time.time - m_LastAddTime < m_CycleDuration)
            return;

        m_LastAddTime = Time.time;
        AddEnergy(m_EnergyIncreasePerCycle);

        LeanTween.cancel(m_AddTweenId);
        m_AddTweenId = LeanTween.value(0, 1, m_CycleDuration)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_AddText.alpha = t;
                m_AddButton.transform.localScale = Vector3.one * Mathf.Lerp(1.25f, 1, t);
            })
            .uniqueId;
    }

    private void OnClickRemoveEnergy()
    {
        if (Time.time - m_LastRemoveTime < m_CycleDuration)
            return;

        m_LastRemoveTime = Time.time;
        AddEnergy(-m_EnergyDecreasePerCycle);

        LeanTween.cancel(m_RemoveTweenId);
        m_RemoveTweenId = LeanTween.value(0, 1, m_CycleDuration)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_RemoveText.alpha = t;
                m_RemoveButton.transform.localScale = Vector3.one * Mathf.Lerp(1.25f, 1, t);
            })
            .uniqueId;
    }

    private void AddEnergy(float amount)
    {
        m_EnergyAcumulated += amount;

        if (m_EnergyAcumulated == 0)
            m_EnergyText.text = "" + m_Data.energy;
        else
            m_EnergyText.text = m_Data.energy + (m_EnergyAcumulated > 0 ? "+" + (int)m_EnergyAcumulated : "" + (int)m_EnergyAcumulated);

        UpdateCanCast();

        //animate
        CanvasGroup pulse = amount > 0 ? m_BluePulse : m_RedPulse;
        Color textColor;

        LeanTween.value(0, 1, m_CycleDuration / 1.25f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                pulse.transform.localScale = Vector3.one * 1.1f * t;
                pulse.alpha = (1 - t) * 1.5f;
            });

        textColor = m_EnergyAcumulated < 0 ? m_RemoveColor : m_AddColor;
        if (m_EnergyText.color != textColor)
        {
            Color start = m_EnergyText.color;
            LeanTween.value(0, 1, m_CycleDuration / 2f)
                .setOnUpdate((float t) =>
                {
                    m_EnergyText.color = Color.Lerp(start, textColor, t);
                });
        }
    }

    private void UpdateCanCast()
    {
        Spellcasting.SpellState canCast = Spellcasting.CanCast((SpellData)null, null, null);
        m_CastButton.interactable = m_EnergyAcumulated != 0;
    }

    private void OnClickCast()
    {
        m_WaitingResult = true;
        m_InputRaycaster.enabled = false;

        var data = new { target = m_MarkerData.instance, energy = (int)m_EnergyAcumulated };

        m_EnergyAcumulated = 0;
        Color start = m_EnergyText.color;

        APIManager.Instance.Post("portal/cast", Newtonsoft.Json.JsonConvert.SerializeObject(data), OnCastResponse);
    }

    private void OnCastResponse(string response, int result)
    {
        if (!m_WaitingResult)
            return;

        if (result == 200)
        {
            //wait for map_spell_cast to update portal energy
        }
        else
        {
            switch (response)
            {
                case "4301": //portal not found (spirit summoned or destroyed?)
                    //let the map_token_remove or map_portal_summoned events close the UI
                    break;
                case "4700": //player dead
                    m_WaitingResult = false;
                    OnClickClose();
                    break;
                case "4603": //player silenced
                    m_WaitingResult = false;
                    ReOpen();
                    break;
                default:
                    UIGlobalErrorPopup.ShowError(OnClickClose, "Error: " + response);
                    break;
            }
        }
        m_InputRaycaster.enabled = true;
    }

    private void OnClickBack()
    {
        Close();
    }

    private void OnClickClose()
    {
        m_InputRaycaster.enabled = true; //m_inputRaycaster was disabled after clicking cast
        StopAllCoroutines();

        MainUITransition.Instance.ShowMainUI();
        MapsAPI.Instance.allowControl = true;
        MapCameraUtils.FocusOnPosition(previousMapPosition, m_PreviousMapZoom, true);
        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Marker }, false);

        OnMapPortalSummon.OnPortalSummoned -= _OnMapPortalSummoned;
        RemoveTokenHandler.OnTokenRemove -= _OnMapTokenRemove;
        SpellCastHandler.OnSpellCast -= _OnMapSpellCast;
        OnMapEnergyChange.OnEnergyChange -= _OnMapEnergyChange;
        OnMapEnergyChange.OnPlayerDead -= _OnCharacterDead;
        BanishManager.OnBanished -= _OnCharacterDead;

        Close();
    }



    private void _OnMapSpellCast(string caster, string target, SpellData spell, Raincrow.GameEventResponses.SpellCastHandler.Result reuslt)
    {
        //someone attacked/buffed the portal
        if (target == m_MarkerData.instance)
        {
            if (caster == PlayerDataManager.playerData.instance)
            {
                m_WaitingResult = false;
                m_InputRaycaster.enabled = true;
            }

            //animate the attack in the UI?
        }
    }

    private void _OnMapTokenRemove(string tokenInstance)
    {
        //the portal was destroyed
        if (tokenInstance == m_MarkerData.instance)
        {
            m_WaitingResult = false;
            UIGlobalErrorPopup.ShowPopUp(() => OnClickClose(), LocalizeLookUp.GetText("portal_destroy"));//"The portal was destroyed.");
        }
    }

    private void _OnMapPortalSummoned(string portalInstance)
    {
        //the spirit was summoned
        if (portalInstance == m_MarkerData.instance)
        {
            m_WaitingResult = false;
            UIGlobalErrorPopup.ShowPopUp(() => OnClickClose(), LocalizeLookUp.GetText("summoning_success"));//"The spirit was summoned.");
        }
    }

    private void _OnMapEnergyChange(string instance, int newEnergy)
    {
        if (instance == m_MarkerData.instance)
        {
            if (m_EnergyAcumulated != 0)
                m_EnergyText.text = newEnergy + (m_EnergyAcumulated > 0 ? "+" + m_EnergyAcumulated : "" + m_EnergyAcumulated);
            else
            {
                if (m_EnergyText.color != m_DefaultColor)
                {
                    Color start = m_EnergyText.color;
                    LeanTween.value(0, 1, m_CycleDuration)
                        .setOnUpdate((float t) =>
                        {
                            m_EnergyText.color = Color.Lerp(start, m_DefaultColor, t);
                        });
                }

                m_EnergyText.text = newEnergy.ToString(); ;
            }
        }
    }

    private void _OnCharacterDead()
    {
        OnClickClose();
    }
}
