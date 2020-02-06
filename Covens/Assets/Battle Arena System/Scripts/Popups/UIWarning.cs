using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIWarning : MonoBehaviour
{
    [SerializeField] private GameObject m_Content;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Button m_CloseButton;
    [SerializeField] protected TextMeshProUGUI m_Title;
    [SerializeField] protected TextMeshProUGUI m_Description;
    [SerializeField] protected TextMeshProUGUI m_Button;
    [SerializeField] private Animator m_Animator;

    private static UIWarning m_Instance;

    public static UIWarning GetInstance()
    {
        if (m_Instance == null)
            m_Instance = Instantiate(Resources.Load<UIWarning>("UIWarning"));
        return m_Instance;
    }

    public static bool IsOpen
    {
        get
        {
            return m_Instance != null;
        }
    }

    private int m_TweenId;
    private System.Action m_OnClose;

    private void Awake()
    {
        m_CloseButton.onClick.AddListener(Close);
        m_Content.SetActive(false);
        m_CanvasGroup.alpha = 0;
        m_Animator.enabled = false;
    }

    public void Show(string title, string message, string button, System.Action onClose = null)
    {
        LeanTween.cancel(m_TweenId);

        m_OnClose = onClose;

        BackButtonListener.AddCloseAction(null);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0, 0.5f).uniqueId;
        StartCoroutine(ShowCoroutine(title, message, button));
    }

    private void Close()
    {
        BackButtonListener.RemoveCloseAction();

        m_Animator.enabled = false;
        m_CloseButton.interactable = false;
        m_OnClose?.Invoke();
        m_OnClose = null;
        m_Instance = null;

        LeanTween.cancel(m_TweenId);
        LeanTween.scale(m_Content.gameObject, m_Content.transform.localScale * 0.8f, 0.5f).setEaseOutCubic(); ;
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0, 0.25f).setOnComplete(() => Destroy(this.gameObject)).uniqueId;
    }

    private IEnumerator ShowCoroutine(string title, string message, string button)
    {
        Setup(title, message, button);

        m_Animator.enabled = true;
        m_Content.SetActive(true);

        m_CloseButton.interactable = false;

        yield return new WaitForSeconds(0.2f);

        m_CloseButton.interactable = true;

        BackButtonListener.RemoveCloseAction();
        BackButtonListener.AddCloseAction(Close);
    }

    void Setup(string title, string message, string button)
    {
        m_Title.text = title;
        m_Description.text = message;
        m_Button.text = button;
    }
}
