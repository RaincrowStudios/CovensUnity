using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBundlePopup : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private UIBundlePopupAnim m_Anim;

    private static UIBundlePopup m_Instance;
    public static bool IsOpen => m_Instance != null && m_Instance.m_InputRaycaster.enabled;

    public static void Open(string bundleId)
    {

    }

    private void _Show()
    {

    }

    public void _Close()
    {

    }
}
