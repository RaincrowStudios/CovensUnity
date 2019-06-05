using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Newtonsoft.Json;
using Raincrow.Maps;

public class UIPOPOptions : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;

    [Header("UI Anim")]
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_PanelRect;
    
    [Header("POP Info")]
    [SerializeField] private TextMeshProUGUI m_TitleText;
    [SerializeField] private TextMeshProUGUI m_LevelText;

    [Header("Buttons")]
    [SerializeField] private Button m_SummonButton;
    [SerializeField] private Button m_LeaveButton;
    [SerializeField] private Button m_ParticlesButton;

    private IMarker m_Marker;
    private LocationMarkerDetail m_MarkerDetail;
    private PlaceOfPower.LocationData m_LocationData;

    private int m_TweenId;

    //debug
    private bool m_ParticlesEnabled;

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_PanelRect.anchoredPosition = new Vector2(0, -m_PanelRect.sizeDelta.y);

        m_SummonButton.onClick.AddListener(OnClickSummon);
        m_LeaveButton.onClick.AddListener(OnClickLeave);
        m_ParticlesButton.onClick.AddListener(() =>
        {
            m_ParticlesEnabled = !m_ParticlesEnabled;
            ParticleSystem[] particles = transform.parent.gameObject.GetComponentsInChildren<ParticleSystem>(true);
            foreach (ParticleSystem _particle in particles)
                _particle.gameObject.SetActive(m_ParticlesEnabled);
        });
    }

    public void Show(IMarker marker, LocationMarkerDetail details, PlaceOfPower.LocationData locationData)
    {
        LeanTween.cancel(m_TweenId);

        this.m_Marker = marker;
        this.m_MarkerDetail = details;
        this.m_LocationData = locationData;

        if (string.IsNullOrEmpty(details.controlledBy))
        {
            m_TitleText.text = details.displayName + $" <size={m_TitleText.fontSize * 0.65f}>(" + LocalizeLookUp.GetText("location_unclaimed") + ")</size>";
        }
        else
        {
            string controlledBy;
            if (details.isCoven)
                controlledBy = LocalizeLookUp.GetText("pop_owner_coven").Replace("{{coven}}", details.controlledBy);
            else
                controlledBy = LocalizeLookUp.GetText("pop_owner_player").Replace("{{player}}", details.controlledBy);

            controlledBy = $" <size={m_TitleText.fontSize * 0.65f}>({controlledBy})</size>";
            m_TitleText.text = details.displayName + controlledBy;
        }
        m_LevelText.text = LocalizeLookUp.GetText("lt_level") + details.level;

        //summoning only available if the coven is controller by the player or its guild
        m_SummonButton.interactable = 
            ( details.isCoven && details.controlledBy == PlayerDataManager.playerData.covenName) || 
            (!details.isCoven && details.controlledBy == PlayerDataManager.playerData.displayName);

        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
                m_PanelRect.anchoredPosition = new Vector2(0, Mathf.Lerp(-m_PanelRect.sizeDelta.y, 0, t));
            })
            .setEaseOutCubic()
            .uniqueId;

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
    }

    public void Close()
    {
        m_InputRaycaster.enabled = false;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(1, 0, 0.5f)
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
                m_PanelRect.anchoredPosition = new Vector2(0, Mathf.Lerp(-m_PanelRect.sizeDelta.y, 0, t));
            })
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
                m_InputRaycaster.enabled = false;
            })
            .setEaseOutCubic()
            .uniqueId;
    }

    private void OnClickSummon()
    {
        SummoningController.Instance.Open();
    }

    private void OnClickLeave()
    {
        PlaceOfPower.LeavePoP();
        Close();
    }
}
