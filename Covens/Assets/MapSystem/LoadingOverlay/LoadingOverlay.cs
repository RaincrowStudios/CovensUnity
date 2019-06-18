using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingOverlay : MonoBehaviour
{
    private static LoadingOverlay m_Instance;
    private static Canvas m_Canvas;
    private static UnityEngine.UI.GraphicRaycaster m_InputRaycaster;

    private void Awake()
    {
        m_Instance = this;
        m_Canvas = GetComponent<Canvas>();
        m_InputRaycaster = GetComponent<UnityEngine.UI.GraphicRaycaster>();

        DontDestroyOnLoad(this.gameObject);
    }

    private void _Show()
    {
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
    }

    private void _Hide()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
    }


    public static void Show()
    {
        if (m_Instance == null)
            return;

        m_Instance._Show();
    }

    public static void Hide()
    {
        if (m_Instance == null)
            return;

        m_Instance._Hide();
    }
}
