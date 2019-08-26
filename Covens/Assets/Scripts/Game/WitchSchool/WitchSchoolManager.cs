using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Raincrow;
using TMPro;

public class WitchSchoolManager : MonoBehaviour
{
    private static WitchSchoolManager m_Instance;
    public static string[] witchVideos = new string[0];

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [SerializeField] private RectTransform m_Window;
    [SerializeField] private witchSchoolData m_ItemPrefab;
    [SerializeField] private LayoutGroup m_ItemContainer;
    
    [Header("Player window")]
    [SerializeField] private CanvasGroup m_VideoWindow;
    [SerializeField] private RawImage m_VideoImage;
    [SerializeField] private MediaPlayerCtrl m_VideoPlayer;
    [SerializeField] private TextMeshProUGUI m_VideoTitle;
    [SerializeField] private Button m_PlayerButton;
    [SerializeField] private CanvasGroup m_Play;
    [SerializeField] private CanvasGroup m_Pause;
    [SerializeField] private Button m_CloseVideoButton;

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

        m_CloseVideoButton.onClick.AddListener(CloseVideo);
        m_PlayerButton.onClick.AddListener(OnClickPlayer);
    }

    private void Show()
    {
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
        m_InputRaycaster.enabled = false;
        MapsAPI.Instance.HideMap(false);

        float start = m_CanvasGroup.alpha;

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(start, 0, .6f)
            .setOnUpdate((float t) =>
            {
                float t2 = LeanTween.easeInQuad(start, 0, 1-t);
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
        m_VideoWindow.gameObject.SetActive(true);
        LeanTween.cancel(m_VideoTweenId);
        m_VideoTweenId = LeanTween.alphaCanvas(m_VideoWindow, 1f, 0.5f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                m_ItemContainer.gameObject.SetActive(false);
                string url = DownloadAssetBundle.baseURL + "witch-school-new/videos/" + id + ".mp4";
                StartCoroutine(LoadVideoCoroutine(url));
            })
            .uniqueId;

        PlayerDataManager.Instance.GetComponent<AudioSource>().Pause();
        m_VideoTitle.text = LocalizeLookUp.GetText(id + "_title").ToUpper();
        m_VideoImage.color = Color.black;
    }

    public void CloseVideo()
    {
        StopCoroutine("LoadVideoCoroutine");
        m_ItemContainer.gameObject.SetActive(true);
        LeanTween.cancel(m_VideoTweenId);
        m_VideoTweenId = LeanTween.alphaCanvas(m_VideoWindow, 0f, 0.5f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_VideoPlayer.SetVolume(t);
            })
            .setOnComplete(() =>
            {
                m_VideoWindow.gameObject.SetActive(false);
                m_VideoPlayer.UnLoad();
                m_VideoPlayer.OnReady = null;
                PlayerDataManager.Instance.GetComponent<AudioSource>().UnPause();
            })
            .uniqueId;
    }

    private IEnumerator LoadVideoCoroutine(string url)
    {
        m_VideoPlayer.m_bAutoPlay = true;
        bool ready = false;
        m_VideoPlayer.OnVideoFirstFrameReady = () => ready = true;
        //m_VideoPlayer.on
        m_VideoPlayer.Load(url);

        while (!ready)
            yield return 0;

        m_VideoTweenId = LeanTween.value(0, 1, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_VideoPlayer.SetVolume(t);
                m_VideoImage.color = Color.Lerp(Color.black, Color.white, t);
            })
            .uniqueId;
    }

    private void OnClickPlayer()
    {
        Debug.LogError(m_VideoPlayer.GetCurrentState() + "\n" + m_VideoPlayer.GetCurrentSeekPercent());
    }

    //IEnumerator FadeOutFocus(CanvasGroup cg)
    //{

    //    float t = 0;
    //    while (t <= 1)
    //    {
    //        t += Time.deltaTime * 2.8f;
    //        cg.alpha = Mathf.SmoothStep(1, 0, t);
    //        yield return 0;
    //    }
    //    cg.gameObject.SetActive(false);

    //}

    //IEnumerator FadeInFocus(CanvasGroup cg, float delay = 0)
    //{

    //    cg.gameObject.SetActive(true);
    //    float t = 0;
    //    while (t <= 1)
    //    {
    //        t += Time.deltaTime * 2;
    //        cg.alpha = Mathf.SmoothStep(0, 1, t);
    //        yield return 0;
    //    }

    //}

    //public void Disable(GameObject g, float delay = 1.5f)
    //{
    //    StartCoroutine(disableObject(g, delay));
    //}

    //IEnumerator disableObject(GameObject g, float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    g.SetActive(false);
    //}
}
