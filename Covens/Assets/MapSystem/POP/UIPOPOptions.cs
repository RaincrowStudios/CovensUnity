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

    [SerializeField] private GameObject CenterSummon;
    [Header("UI Anim")]
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_PanelRect;

    [Header("POP Info")]
    [SerializeField] private TextMeshProUGUI m_TitleText;
    [SerializeField] private TextMeshProUGUI m_LevelText;

    [Header("Buttons")]
    [SerializeField] private Button m_SummonButton;
    [SerializeField] private Button m_LeaveButton;
    //[SerializeField] private Button m_ParticlesButton;

    private IMarker m_Marker;
    private LocationMarkerData m_MarkerDetail;
    private PlaceOfPower.LocationData m_LocationData;

    public static UIPOPOptions Instance { get; set; }

    private int m_TweenId;

    //debug
    private bool m_ParticlesEnabled;

    private void Awake()
    {
        Instance = this;
        //this.GetComponent<CanvasGroup>().interactable = false;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_PanelRect.anchoredPosition = new Vector2(0, -m_PanelRect.sizeDelta.y);

        m_SummonButton.onClick.AddListener(OnClickSummon);
        m_LeaveButton.onClick.AddListener(OnClickLeave);
    }

    public void Show(IMarker marker, LocationMarkerData details, PlaceOfPower.LocationData locationData)
    {
        // m_SummonButton.interactable = false;
        // CenterSummon.SetActive(false);
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

            controlledBy = $"\n<size={m_TitleText.fontSize * 0.65f}>({controlledBy})</size>";
            m_TitleText.text = details.displayName + controlledBy;
        }
        m_LevelText.text = LocalizeLookUp.GetText("summoning_tier") + details.level;

        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
                m_PanelRect.anchoredPosition = new Vector2(0, Mathf.Lerp(-m_PanelRect.sizeDelta.y, 0, t));
            })
            .setOnComplete(() =>
            {
                ShowSummoning(locationData.spirit == null);
            })
            .setEaseOutCubic()
            .uniqueId;

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
        //LeanTween.value(0f, 1f, 1f).setOnComplete(() =>
        //  {
        //      this.GetComponent<CanvasGroup>().interactable = true;
        //  });
    }

    public void ShowSummoning(bool show)
    {
        CenterSummon.SetActive(show);
        m_SummonButton.transform.parent.gameObject.SetActive(show);
        m_SummonButton.interactable = show;
        if (UIWaitingCastResult.isOpen)
        {
            UIWaitingCastResult.Instance.OnClickContinue();
        }
        if (show == true)
        {
            LeanTween.alphaCanvas(m_PanelRect.GetChild(0).GetComponent<CanvasGroup>(), 1f, 1f);
            LeanTween.alpha(CenterSummon, 1f, 1f);
        }
        else
        {
            LeanTween.alphaCanvas(m_PanelRect.GetChild(0).GetComponent<CanvasGroup>(), 0f, 0.3f);
            LeanTween.alpha(CenterSummon, 0f, 0.3f);
        }
    }
    public void ShowUI()
    {
        var c = m_PanelRect.GetComponent<CanvasGroup>();
        LeanTween.alphaCanvas(c, 1f, 0.6f);
    }
    public void HideUI()
    {
        var c = m_PanelRect.GetComponent<CanvasGroup>();
        LeanTween.alphaCanvas(c, 0f, 0.6f);
    }

    public void Close()
    {
        // this.GetComponent<CanvasGroup>().interactable = false;

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
        ShowSummoning(false);
    }

    private void OnClickSummon()
    {
        SummoningController.Instance.Open();
        HideUI();
    }

    private void OnClickLeave()
    {
        if (BanishManager.isBind)
        {
            UIGlobalErrorPopup.ShowError(null, "You are bound");
            return;
        }

        PlaceOfPower.LeavePoP();
        Close();
    }
}
