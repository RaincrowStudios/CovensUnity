using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIStorePurchaseSuccess : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Animator m_Animator;

    [Space()]
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Subtitle;
    [SerializeField] private Image m_Icon;
    [SerializeField] private Button m_ContinueButton;

    private static UIStorePurchaseSuccess m_Instance;

    private System.Action m_OnClose;
    private int m_AlphaTweenId;

    public static void Show(string title, string subtitle, Sprite icon, System.Action onClose = null)
    {
        if (m_Instance == null)
        {
            onClose?.Invoke();
            return;
        }
        m_Instance._Show(title, subtitle, icon, onClose);
    }

    private void Awake()
    {
        m_Instance = this;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;

        m_ContinueButton.onClick.AddListener(_Close);
        m_Animator.gameObject.SetActive(false);
    }

    private void _Show(string title, string subtitle, Sprite icon, System.Action onClose)
    {
        m_Title.text = title;
        m_Subtitle.text = subtitle;
        m_Icon.overrideSprite = icon;
        m_OnClose = onClose;

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        LeanTween.cancel(m_AlphaTweenId);
        m_Animator.gameObject.SetActive(true);
        m_AlphaTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.2f).setEaseOutCubic().uniqueId;
    }

    private void _Close()
    {
        m_InputRaycaster.enabled = false;

        LeanTween.cancel(m_AlphaTweenId);
        Debug.Log("a. " + Time.time);
        m_AlphaTweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0, 0.5f).setEaseOutCubic().setOnComplete(() =>
        {
            m_Canvas.enabled = false;
            m_Animator.gameObject.SetActive(false);
            Debug.Log("b. " + Time.time);
        }).uniqueId;
        m_OnClose?.Invoke();
    }
}
