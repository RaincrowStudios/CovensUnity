using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIKytelerInfo : MonoBehaviour
{
    public static UIKytelerInfo Instance { get; private set; }

    [SerializeField] private CanvasGroup m_Content;

    [SerializeField] private Image m_KytelerArt;
    [SerializeField] private Text m_LastLocationText;
    [SerializeField] private Text m_DiscoveryText;
    [SerializeField] private Text m_TitleText;
    [SerializeField] private Text m_DescriptionText;
    [SerializeField] private Button m_CloseButton;
    [SerializeField] private Button m_LastLocationButton;

    private int m_FadeTweenId;
    private int m_ScaleTweenId;
    private KytelerData m_Data;

    private void Awake()
    {
        Instance = this;
        m_CloseButton.onClick.AddListener(OnClickClose);
        m_LastLocationButton.onClick.AddListener(OnClickLastLocation);
        m_Content.gameObject.SetActive(false);
        m_Content.interactable = false;
        m_Content.alpha = 0;
        m_Content.transform.localScale = Vector3.zero;
    }

    public void Show(KytelerData data)
    {
        LeanTween.cancel(m_FadeTweenId);
        LeanTween.cancel(m_ScaleTweenId);

        m_Data = data;
        m_TitleText.text = data.id;
        //m_DescriptionText.text = 
        m_LastLocationText.text = "Last Location: <i>Owned by Bashelik</i>";
        m_DiscoveryText.text = "Date Discovered: <i>March 21, 2018</i>";

        m_Content.gameObject.SetActive(true);
        m_Content.interactable = true;

        m_ScaleTweenId = LeanTween.scale(m_Content.gameObject, Vector3.one, 0.25f)
            .uniqueId;
        m_FadeTweenId = LeanTween.value(m_Content.alpha, 1f, 0.5f)
            .setOnUpdate((float t) => { m_Content.alpha = t; })
            .setEaseOutCubic()
            .uniqueId;
    }

    public void Close()
    {
        LeanTween.cancel(m_FadeTweenId);
        LeanTween.cancel(m_ScaleTweenId);

        m_Content.interactable = false;

        System.Action onAnimComplete = () =>
        {
            m_Content.gameObject.SetActive(false);
        };

        m_ScaleTweenId = LeanTween.scale(m_Content.gameObject, Vector3.zero, 0.5f)
            .setOnComplete(onAnimComplete)
            .uniqueId;
        m_FadeTweenId = LeanTween.value(m_Content.alpha, 0f, 0.25f)
            .setOnUpdate((float t) => { m_Content.alpha = t; })
            .setEaseOutCubic()
            .uniqueId;
    }

    private void OnClickClose()
    {
        Close();
    }

    private void OnClickLastLocation()
    {
        Debug.Log("OnClickLastLocation");
    }
}
