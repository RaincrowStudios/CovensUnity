using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingOverlay : MonoBehaviour
{
    private static LoadingOverlay Instance;
    private Canvas m_Canvas;
    private CanvasGroup m_CanvasGroup;
    private TextMeshProUGUI m_Message;
    private UnityEngine.UI.GraphicRaycaster m_InputRaycaster;
    private int m_TweenId;

    private int m_Stack = 0;

    private void Awake()
    {
        m_Canvas = GetComponent<Canvas>();
        m_CanvasGroup = m_Canvas.GetComponent<CanvasGroup>();
        m_InputRaycaster = GetComponent<UnityEngine.UI.GraphicRaycaster>();
        m_Message = GetComponentInChildren<TextMeshProUGUI>();
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void _Show(string msg = null)
    {
        m_Stack += 1;

        if (m_Canvas.enabled && m_InputRaycaster.enabled)
            return;

        //Debug.Log("Show overlay load");
        if (string.IsNullOrEmpty(msg))
        {
            if (LocalizeLookUp.HasKey("generic_please_wait"))
                m_Message.text = LocalizeLookUp.GetText("generic_please_wait");
            else
                m_Message.text = "Please wait";
        }
        else
        {
            m_Message.text = msg;
        }

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, 0.5f)
            .setEaseOutCubic()
            .uniqueId;
    }

    private void _Hide()
    {
        m_Stack -= 1;

        if (m_Stack < 0)
            m_Stack = 0;

        if (m_Stack > 0)
            return;

        if (m_Canvas.enabled == false && m_InputRaycaster.enabled == false)
            return;
        
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

    public static void Show(string msg = null)
    {
        if (Instance == null)
        {
            return;
        }

        Instance._Show(msg);
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
