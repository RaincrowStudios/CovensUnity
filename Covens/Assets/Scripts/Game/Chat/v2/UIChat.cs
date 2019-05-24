using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;


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


}