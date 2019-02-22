using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingOverlay : MonoBehaviour
{
    private static LoadingOverlay m_Instance;
    private static Canvas m_Canvas;
    private static UnityEngine.UI.GraphicRaycaster m_InputRaycaster;

    public static void Show()
    {
        if (m_Instance == null)
            Instantiate();

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
    }

    public static void Hide()
    {
        if (m_Instance == null)
            Instantiate();

        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
    }

    private static void Instantiate()
    {
        LoadingOverlay prefab = Resources.Load<LoadingOverlay>("LoadingOverlay");
        m_Instance = GameObject.Instantiate(prefab);
        m_Canvas = m_Instance.GetComponent<Canvas>();
        m_InputRaycaster = m_Instance.GetComponent<UnityEngine.UI.GraphicRaycaster>();
    }
}
