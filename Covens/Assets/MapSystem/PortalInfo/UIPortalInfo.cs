using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Raincrow.Maps;

public class UIPortalInfo : UIInfoPanel
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI m_Timer;
    [SerializeField] private TextMeshProUGUI m_EnergyText;
    [SerializeField] private TextMeshProUGUI m_AddText;
    [SerializeField] private TextMeshProUGUI m_RemoveText;

    [Header("Buttons")]
    [SerializeField] private EventTrigger m_AddButton;
    [SerializeField] private EventTrigger m_RemoveButton;
    [SerializeField] private Button m_CastButton;
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

    private const float m_CycleDuration = 0.5f;
    private const float m_EnergyCostPerCycle = 1;
    private const float m_EnergyIncreasePerCycle = 2;
    private const float m_EnergyDecreasePerCycle = 2;


    private IMarker m_Marker;
    private Token m_MarkerData;
    private MarkerDataDetail m_Data;

    private Coroutine m_EnergyCoroutine;
    private float m_EnergyAcumulated;
    
    private Vector3 m_PreviousMapPosition;
    private float m_PreviousMapZoom;

    private bool m_WaitingResult = false;

    protected override void Awake()
    {
        base.Awake();

        m_BluePulse.alpha = 0;
        m_RedPulse.alpha = 0;

        m_CloseButton.onClick.AddListener(OnClickClose);
        m_CastButton.onClick.AddListener(OnClickCast);

        m_AddButton.triggers = new List<EventTrigger.Entry>
        {
             new EventTrigger.Entry
             {
                eventID = EventTriggerType.PointerDown,
                callback = new EventTrigger.TriggerEvent()
            },
            new EventTrigger.Entry
             {
                eventID = EventTriggerType.PointerUp,
                callback = new EventTrigger.TriggerEvent()
            }
        };
        m_AddButton.triggers[0].callback.AddListener(data => OnStartAddEnergy());
        m_AddButton.triggers[1].callback.AddListener(data => OnStopAddEnergy());

        m_RemoveButton.triggers = new List<EventTrigger.Entry>
        {
             new EventTrigger.Entry
             {
                eventID = EventTriggerType.PointerDown,
                callback = new EventTrigger.TriggerEvent()
            },
            new EventTrigger.Entry
             {
                eventID = EventTriggerType.PointerUp,
                callback = new EventTrigger.TriggerEvent()
            }
        };
        m_RemoveButton.triggers[0].callback.AddListener(data => OnStartRemoveEnergy());
        m_RemoveButton.triggers[1].callback.AddListener(data => OnStopRemoveEnergy());
    }
    
    public void Show(IMarker marker)
    {
        if (IsShowing)
            return;

        m_Marker = marker;
        m_MarkerData = marker.customData as Token;
        m_Data = null;

        m_CastButton.interactable = false;
        m_EnergyText.text = "???";
        m_Timer.text = $"Summons in ???";
        m_EnergyText.color = m_DefaultColor;


        m_PreviousMapPosition = StreetMapUtils.CurrentPosition();
        m_PreviousMapZoom = MapController.Instance.zoom;
        
        MainUITransition.Instance.HideMainUI();
        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Marker }, true);

        OnMapPortalSummon.OnPortalSummoned += _OnMapPortalSummoned;
        OnMapTokenRemove.OnTokenRemove += _OnMapTokenRemove;
        OnMapSpellcast.OnSpellCast += _OnMapSpellCast;
        OnMapEnergyChange.OnEnergyChange += _OnMapEnergyChange;
        OnCharacterDeath.OnPlayerDead += _OnCharacterDead;

        Show();
    }

    public override void ReOpen()
    {
        base.ReOpen();
        
        UpdateCanCast();
        MapController.Instance.allowControl = false;
        StreetMapUtils.FocusOnTarget(m_Marker);
        m_InputRaycaster.enabled = true;
    }

    public void SetupDetails(MarkerDataDetail data)
    {
        m_Data = data;
        m_EnergyText.text = $"{data.energy}";
        StartCoroutine(SummonTimerCoroutine());
    }

    private IEnumerator SummonTimerCoroutine()
    {
        while (true)
        {
            m_Timer.text = $"Summons in {Utilities.GetSummonTime(m_Data.summonOn)}";
            yield return new WaitForSecondsRealtime(1.001f);
        }
    }

    private IEnumerator EnergyCoroutine(bool add)
    {
        CanvasGroup pulse = add ? m_BluePulse : m_RedPulse;
        Color textColor;

        while (true)
        {
            //wait
            yield return new WaitForSecondsRealtime(m_CycleDuration);

            //increment energy
            if (add)
                m_EnergyAcumulated += m_EnergyIncreasePerCycle;
            else
                m_EnergyAcumulated -= m_EnergyDecreasePerCycle;

            if (m_EnergyAcumulated == 0)
                m_EnergyText.text = "" + m_Data.energy;
            else
                m_EnergyText.text = m_Data.energy + (m_EnergyAcumulated > 0 ? "+" + m_EnergyAcumulated : "" + m_EnergyAcumulated);

            //animate
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

            UpdateCanCast();
        }
    }


    private void OnStartAddEnergy()
    {
        m_AddText.alpha = 0.6f;
        m_EnergyCoroutine = StartCoroutine(EnergyCoroutine(true));
    }
    private void OnStopAddEnergy()
    {
        m_AddText.alpha = 1f;
        StopCoroutine(m_EnergyCoroutine);
    }

    private void OnStartRemoveEnergy()
    {
        m_RemoveText.alpha = 0.6f;
        m_EnergyCoroutine = StartCoroutine(EnergyCoroutine(false));
    }
    private void OnStopRemoveEnergy()
    {
        m_RemoveText.alpha = 1f;
        StopCoroutine(m_EnergyCoroutine);
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

        var data = new { target = m_MarkerData.instance, energy = m_EnergyAcumulated };

        m_EnergyAcumulated = 0;
        Color start = m_EnergyText.color;

        APIManager.Instance.PostCoven("portal/cast", Newtonsoft.Json.JsonConvert.SerializeObject(data), OnCastResponse);
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

    private void OnClickClose()
    {
        m_InputRaycaster.enabled = true; //m_inputRaycaster was disabled after clicking cast
        StopAllCoroutines();

        MainUITransition.Instance.ShowMainUI();
        MapController.Instance.allowControl = true;
        StreetMapUtils.FocusOnPosition(m_PreviousMapPosition, true, m_PreviousMapZoom, true);
        MarkerSpawner.HighlightMarker(new List<IMarker> { PlayerManager.marker, m_Marker }, false);
        
        OnMapPortalSummon.OnPortalSummoned -= _OnMapPortalSummoned;
        OnMapTokenRemove.OnTokenRemove -= _OnMapTokenRemove;
        OnMapSpellcast.OnSpellCast -= _OnMapSpellCast;
        OnMapEnergyChange.OnEnergyChange -= _OnMapEnergyChange;
        OnCharacterDeath.OnPlayerDead -= _OnCharacterDead;

        Close();
    }

    

    private void _OnMapSpellCast(IMarker caster, IMarker target, SpellDict spell, Result reuslt)
    {
        //someone attacked/buffed the portal
        if (target == m_Marker)
        {
            if (caster == PlayerManager.marker)
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
            UIGlobalErrorPopup.ShowPopUp(() => OnClickClose(), "The portal was destroyed.");
        }
    }

    private void _OnMapPortalSummoned(string portalInstance)
    {
        //the spirit was summoned
        if (portalInstance == m_MarkerData.instance)
        {
            m_WaitingResult = false;
            UIGlobalErrorPopup.ShowPopUp(() => OnClickClose(), "The spirit was summoned.");
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

    private void _OnCharacterDead(string name, string spirit)
    {
        OnClickClose();
    }
}
