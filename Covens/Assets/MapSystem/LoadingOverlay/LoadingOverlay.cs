using System.Collections.Generic;
using UnityEngine;

public class LoadingOverlay : MonoBehaviour
{
    private static LoadingOverlay Instance;
    private Canvas m_Canvas;
    private CanvasGroup m_CanvasGroup;
    private UnityEngine.UI.GraphicRaycaster m_InputRaycaster;
    private int m_TweenId;

    private void Awake()
    {
        m_Canvas = GetComponent<Canvas>();
        m_CanvasGroup = m_Canvas.GetComponent<CanvasGroup>();
        m_InputRaycaster = GetComponent<UnityEngine.UI.GraphicRaycaster>();
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void _Show()
    {
        if (m_Canvas.enabled && m_InputRaycaster.enabled)
            return;

        Debug.Log("Show overlay load");

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, 0.5f)
            .setEaseOutCubic()
            .uniqueId;
    }

    private void _Hide()
    {
        if (m_Canvas.enabled == false && m_InputRaycaster.enabled == false)
            return;

        Debug.Log("Hide overlay load");

        m_InputRaycaster.enabled = false;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0f, 1.5f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
            })
            .uniqueId;
    }

    public static void Show()
    {
        if (Instance == null)
        {
            return;
        }

        Instance._Show();
    }

    public static void Hide()
    {
        if (Instance == null)
        {
            return;
        }

        Instance._Hide();
    }
}
