using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPOPOffering : MonoBehaviour
{
    private static UIPOPOffering m_Instance;
    public static UIPOPOffering Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UIPOPOffering>("UIPOPOffering"));
            return m_Instance;
        }
    }

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private LayoutGroup m_Layout;

    [SerializeField] private TextMeshProUGUI m_TitleText;
    [SerializeField] private TextMeshProUGUI m_CancelText;

    [SerializeField] private Button m_OfferingButton;
    [SerializeField] private Button m_CancelButton;

    private int m_TweenId;

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_Layout.enabled = false;
        m_CanvasGroup.alpha = 0;
    }

    public void Show()
    {
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(0, 1, 1f)
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
            })
            .setEaseOutCubic()
            .uniqueId;

        m_Layout.enabled = true;
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
    }

    public void Close()
    {
        m_Layout.enabled = false;
        m_InputRaycaster.enabled = false;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(1, 0, 1f)
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
            })
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
            })
            .setEaseOutCubic()
            .uniqueId;
    }
}
