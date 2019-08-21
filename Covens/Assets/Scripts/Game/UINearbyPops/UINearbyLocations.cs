using Raincrow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINearbyLocations : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [SerializeField] private LayoutGroup m_Container;
    [SerializeField] private UINearbyLocationItem m_Prefab;

    [SerializeField] private Button m_CloseButton;

    private static UINearbyLocations m_Instance;
    
    public static void Open()
    {
        if (m_Instance != null)
        {
            m_Instance.Show();
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.SPIRIT_SELECT,
                UnityEngine.SceneManagement.LoadSceneMode.Additive,
                null,
                () =>
                {
                    m_Instance.Show();
                    LoadingOverlay.Hide();
                });
        }
    }

    private int m_AnimTweenId;

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
    }

    private void Show()
    {
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
        AnimShow(null);
    }

    private void Hide()
    {
        m_InputRaycaster.enabled = false;
        AnimHide(() =>
        {
            m_Canvas.enabled = false;
        });
    }

    private void AnimShow(System.Action onComplete)
    {
        LeanTween.cancel(m_AnimTweenId);
        m_AnimTweenId = LeanTween.value(m_CanvasGroup.alpha, 1, 1f)
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
            })
            .setOnComplete(onComplete)
            .setEaseOutCubic()
            .uniqueId;
    }

    private void AnimHide(System.Action onComplete)
    {
        LeanTween.cancel(m_AnimTweenId);
        m_AnimTweenId = LeanTween.value(m_CanvasGroup.alpha, 0, 1f)
            .setOnUpdate((float t) =>
            {
                m_CanvasGroup.alpha = t;
            })
            .setOnComplete(onComplete)
            .setEaseOutCubic()
            .uniqueId;
    }
}
