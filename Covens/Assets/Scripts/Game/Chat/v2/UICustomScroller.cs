using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICustomScroller : MonoBehaviour
{
    [SerializeField] private RectTransform m_RectTransform;
    [SerializeField] private VerticalLayoutGroup m_Layout;

    [Header("Settings")]
    [SerializeField] private float m_Sensivity;
}
