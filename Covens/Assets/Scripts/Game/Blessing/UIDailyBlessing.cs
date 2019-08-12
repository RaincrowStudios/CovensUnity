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
        m_BlessingText.text = LocalizeLookUp.GetText("blessing_grant").Replace("{{amount}}", "<color=#FF9900>" + energyGained.ToString() + "</color>");
        m_PopBonusText.text = "";

        //if (PlayerDataManager.playerData.blessing.daily != 0)
        //{
        //    blessingText.text = LocalizeLookUp.GetText("blessing_grant");
        //    blessingText.text = blessingText.text.Replace("{{amount}}", "<color=#FF9900>" + PlayerDataManager.playerData.blessing.daily.ToString() + "</color>");
        //}
        //else
        //    blessingText.text = LocalizeLookUp.GetText("blessing_full");
        ////blessingText.text = "The Dea Savannah Grey has granted you her daily gift of <color=#FF9900>" + PlayerDataManager.playerData.blessing.daily.ToString() + "</color> energy";
        //if (PlayerDataManager.playerData.blessing.locations > 0)
        //{
        //    locationEn.text = LocalizeLookUp.GetText("blessing_pop").Replace("{{amount}}", PlayerDataManager.playerData.blessing.locations.ToString());// "You also gained " + PlayerDataManager.playerData.blessing.locations.ToString() + " energy from your Places of Power";
        //}
        //else
        //{
        //    locationEn.text = "";
        //}

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
