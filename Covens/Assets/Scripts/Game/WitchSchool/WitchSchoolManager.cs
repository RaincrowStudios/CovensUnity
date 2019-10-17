using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Raincrow;
using TMPro;

public class WitchSchoolManager : MonoBehaviour
{
    private static WitchSchoolManager m_Instance;
    public static string[] witchVideos = witchVideos = new string[]
    {
        "mainui",
        "fly",
        "summoning",
        "spellcasting",
        "path",
        "energy",
        "money",
        "tribunal"
    };

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [SerializeField] private RectTransform m_Window;
    [SerializeField] private witchSchoolData m_ItemPrefab;
    [SerializeField] private LayoutGroup m_ItemContainer;

    private int m_TweenId;
    private int m_VideoTweenId;

    public static void Open()
    {
        if (m_Instance != null)
        {
            m_Instance.Show();
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.WITCH_SCHOOL, UnityEngine.SceneManagement.LoadSceneMode.Additive,
                (progress) => { },
                () =>
                {
                    LoadingOverlay.Hide();
                    m_Instance.Show();
                });
        }
    }

    void Awake()
    {
        m_Instance = this;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CanvasGroup.alpha = 0;
        m_ItemPrefab.gameObject.SetActive(false);

        foreach (var item in witchVideos)
        {
            var g = Instantiate(m_ItemPrefab, m_ItemContainer.transform);
            g.gameObject.SetActive(true);
            g.Setup(item, () => PlayVideo(item));
        }
    }

    [ContextMenu("Open")]
    private void Show()
    {
        BackButtonListener.AddCloseAction(Close);
        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;

        float start = m_CanvasGroup.alpha;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(start, 1, .6f)
            .setOnUpdate((float t) =>
            {
                float t2 = LeanTween.easeOutQuad(start, 1, t);
                m_CanvasGroup.alpha = t;
                m_Window.localScale = Vector3.one * t2;
            })
            .setOnComplete(() => MapsAPI.Instance.HideMap(true))
            .uniqueId;
    }

    public void Close()
    {
        BackButtonListener.RemoveCloseAction();

        m_InputRaycaster.enabled = false;
        MapsAPI.Instance.HideMap(false);

        float start = m_CanvasGroup.alpha;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(start, 0, .6f)
            .setOnUpdate((float t) =>
            {
                float t2 = LeanTween.easeInQuad(start, 0, 1 - t);
                m_CanvasGroup.alpha = t;
                m_Window.localScale = Vector3.one * t2;
            })
            .setOnComplete(() =>
            {
                m_Canvas.enabled = false;
                m_TweenId = LeanTween.value(0, 0, 5f)
                    .setOnComplete(() => SceneManager.UnloadScene(SceneManager.Scene.WITCH_SCHOOL, null, null))
                    .uniqueId;

            })
            .uniqueId;
    }

    public void PlayVideo(string id)
    {
        WitchSchoolPlayer.Open(
            LocalizeLookUp.GetText(id + "_title").ToUpper(),
            DownloadAssetBundle.baseURL + "witch-school-new/videos/" + id + ".mp4",
            () => m_ItemContainer.gameObject.SetActive(false),
            () => m_ItemContainer.gameObject.SetActive(true));
    }
}
