using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ScreenspaceUI : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private int m_Offset;

    private void Awake()
    {
        m_Canvas.worldCamera = Camera.main;
        m_Canvas.sortingOrder = 1000 + m_Offset;
        m_Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        m_Canvas.planeDistance = 100;
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        Debug.Log("validating ScreenspaceUI.cs");
        if (m_Canvas == null)
            m_Canvas = GetComponent<Canvas>();
        if (m_Canvas == null)
            Debug.LogError("CANVAS NOT FOUND");
        SetLayer(this.transform, 5);
#endif
    }

    private static void SetLayer(Transform transform, int layer)
    {
        transform.gameObject.layer = layer;
        foreach (Transform child in transform)
            SetLayer(child, layer);
    }
}
