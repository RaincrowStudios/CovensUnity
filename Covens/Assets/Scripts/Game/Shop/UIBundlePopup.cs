using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow;

public class UIBundlePopup : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private UIBundlePopupAnim m_Anim;

    [Space]

    private static UIBundlePopup m_Instance;
    public static bool IsOpen => m_Instance != null && m_Instance.m_InputRaycaster.enabled;

    public static void Open(string bundleId)
    {

        if (m_Instance != null)
        {
            m_Instance._Show();
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.STORE_BUNDLE, UnityEngine.SceneManagement.LoadSceneMode.Additive, null, () =>
            {
                LoadingOverlay.Hide();
                m_Instance._Show();
            });
        }
    }

    private void Awake()
    {
        m_Instance = this;
        m_Canvas.enabled = this;
        m_Instance.enabled = this;
    }

    private void _Show()
    {
    }

    public void _Close()
    {

    }
}
