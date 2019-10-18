using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow;

public class UIDailyBlessing : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [SerializeField] private TextMeshProUGUI m_BlessingText;
    [SerializeField] private TextMeshProUGUI m_PopBonusText;

    [SerializeField] private Button m_CloseButton;

    private static UIDailyBlessing m_Instance;
    private static System.Action m_OnClose;
    private int m_TweenId;

    public static void Show(int energyGained)
    {
        if (m_Instance != null)
        {
            m_Instance.Open(energyGained);
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.DAILY_BLESSING,
                UnityEngine.SceneManagement.LoadSceneMode.Additive,
                (progress) => { },
                () =>
                {
                    LoadingOverlay.Hide();
                    m_Instance.Open(energyGained);
                });
        }
    }

    private void Awake()
    {
        m_Instance = this;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;

        m_CloseButton.onClick.AddListener(() => Close());
    }

    private void Open(int energyGained)
    {
        BackButtonListener.AddCloseAction(Close);

        m_BlessingText.text = LocalizeLookUp.GetText("blessing_grant").Replace("{{amount}}", "<color=#FF9900>" + energyGained.ToString() + "</color>");
        m_PopBonusText.text = "";
        
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(0, 1, 0.25f)
            .setEaseOutCubic()
            .setOnUpdate((float t) => m_CanvasGroup.alpha = t)
            .uniqueId;
        SoundManagerOneShot.Instance.MenuSound();
    }

    private void Close()
    {
        BackButtonListener.RemoveCloseAction();

        m_InputRaycaster.enabled = false;
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(1, 0, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) => m_CanvasGroup.alpha = t)
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
                SceneManager.UnloadScene(SceneManager.Scene.DAILY_BLESSING, null, null);
            })
            .uniqueId;

        LeanTween.value(0, 0, 0.5f).setOnComplete(() => PlayerManagerUI.Instance.UpdateEnergy());
    }
}
