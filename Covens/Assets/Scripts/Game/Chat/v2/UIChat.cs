using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private UICustomScroller m_Scroller;

    [Header("Animation")]
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private CanvasGroup m_ContainerCanvasGroup;
    [SerializeField] private RectTransform m_WindowTransform;

    [Header("Header")]
    [SerializeField] private Button m_NewsButton;
    [SerializeField] private Button m_WorldButton;
    [SerializeField] private Button m_CovenButton;
    [SerializeField] private Button m_DominionButton;
    [SerializeField] private Button m_HelpButton;

    [Header("Prefabs")]
    [SerializeField] private UIChatItem m_ChatMessagePrefab;
    [SerializeField] private UIChatItem m_ChatLocationPrefab;
    [SerializeField] private UIChatItem m_ChatCovenPrefab;
    [SerializeField] private UIChatItem m_ChatHelpPlayerPrefab;
    [SerializeField] private UIChatItem m_ChatHelpCrowPrefab;

    [Header("Settings")]
    [SerializeField] private int m_MaxItems = 10;

    private SimplePool<UIChatItem> m_ChatMessagePool;
    private SimplePool<UIChatItem> m_ChatLocationPool;
    private SimplePool<UIChatItem> m_ChatCovenPool;
    private SimplePool<UIChatItem> m_ChatHelpPlayerPool;
    private SimplePool<UIChatItem> m_ChatHelpCrowPool;

    private List<UIChatItem> m_Visible = new List<UIChatItem>();

    public static void Show()
    {

    }
    

    private void Awake()
    {
        m_Scroller.OnBotChildExitView += Scroller_OnBotChildExitView;
        m_Scroller.OnTopChildExitView += Scroller_OnTopChildExitView;

        m_ChatMessagePool = new SimplePool<UIChatItem>(m_ChatMessagePrefab, 1);
        m_ChatLocationPool = new SimplePool<UIChatItem>(m_ChatLocationPrefab, 1);
        m_ChatCovenPool = new SimplePool<UIChatItem>(m_ChatCovenPrefab, 1);
        m_ChatHelpPlayerPool = new SimplePool<UIChatItem>(m_ChatHelpPlayerPrefab, 1);
        m_ChatHelpCrowPool = new SimplePool<UIChatItem>(m_ChatHelpCrowPrefab, 1);
    }

    private void Scroller_OnTopChildExitView(RectTransform obj)
    {

    }

    private void Scroller_OnBotChildExitView(RectTransform obj)
    {

    }

    private void AnimateShow()
    {

    }

    private void AnimateHide()
    {

    }


}