using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using UnityEngine.Networking;

public class SplashManager : MonoBehaviour
{
    public static SplashManager Instance { get; set; }
    public static event System.Action OnFinish;

    [Header("General")]
    [SerializeField] private GameObject LoadingImage;
    [SerializeField] private Image progressBar;

    [Header("Logos")]
    [SerializeField] private float logoTime;
    [SerializeField] private float splashTime = 15f;
    [SerializeField] private float fadeTime;
    [SerializeField] private CanvasGroup[] logos;
    [SerializeField] private MediaPlayerCtrl VideoPlayback;

    [Header("Download")]
    [SerializeField] private CanvasGroup m_DownloadScreen;
    [SerializeField] private TextMeshProUGUI downloadingTitle;
    [SerializeField] private TextMeshProUGUI downloadingInfo;
    [SerializeField] private Slider slider;

    [Header("Dominion")]
    [SerializeField] private CanvasGroup m_DominionScreen;
    [SerializeField] private TextMeshProUGUI currentDominion;
    [SerializeField] private TextMeshProUGUI strongestWitch;
    [SerializeField] private TextMeshProUGUI strongestCoven;

    [Header("Hints")]
    [SerializeField] private CanvasGroup m_HintScreen;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private Image spirit;
    [SerializeField] private TextMeshProUGUI spiritName;

    [Header("Tribunal")]
    [SerializeField] private CanvasGroup m_TribualScreen;
    [SerializeField] private TextMeshProUGUI tribunalTimer;
    [SerializeField] private TextMeshProUGUI tribunalTitle;

    [Header("Outdated build")]
    [SerializeField] private GameObject OutdatedBuild;
    [SerializeField] private GameObject playstoreIcon;
    [SerializeField] private GameObject appleIcon;
    
    bool hasTriedLogin = false;

    AsyncOperation SceneAO;
    double[] tribunalStamps = new double[] { 1553040000, 1561075200, 1569196800, 1576972800, 1584662400, 1592697600 };
    int[] tribunals = new int[] { 1, 2, 3, 4, 1, 2 };
    
    public static bool loginCheck = false;
    public static bool loginStatus = false;

    private int m_SliderTweenId;
    
    void Awake()
    {
        Instance = this;

        progressBar.fillAmount = 0;
        foreach(var logo in logos)
        {
            if (logo == null)
                continue;
            logo.alpha = 0;
            logo.gameObject.SetActive(false);
        }

        slider.value = 0;
        slider.gameObject.SetActive(false);
        VideoPlayback.gameObject.SetActive(false);
        LoadingImage.gameObject.SetActive(false);
    }

    public void ShowLoading(float progress)
    {
        LoadingImage.SetActive(progress < 1);

        LeanTween.cancel(m_SliderTweenId);
        m_SliderTweenId = LeanTween.value(slider.value, progress, 0.2f)
                .setOnUpdate((float v) => { progressBar.fillAmount = v; })
                .uniqueId;
    }

    public void OutDatedBuild()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
            appleIcon.SetActive(true);
        else if (Application.platform == RuntimePlatform.Android)
            playstoreIcon.SetActive(true);
    }

    public void SetDownloadProgress(string fileName, int fileIndex, int totalFiles, float fileSize, float progress)
    {
        string title = "";
        string msg = "";

        //downloading title
        title = "Downloading";
        int points = (int)(Time.time) % 3;
        for (int i = 0; i <= points; i++)
            title += " .";

        //downloading description
        msg =
        "Downloading: " +
        (fileIndex).ToString() +
        " out of " +
        totalFiles.ToString() +
        " (" + (progress * fileSize).ToString("F2") + "/" + fileSize.ToString("F2") + "MB)";

        slider.gameObject.SetActive(true);
        LeanTween.cancel(m_SliderTweenId);
        m_SliderTweenId = LeanTween.value(slider.value, progress, 0.2f)
            .setOnUpdate((float v) => { slider.value = v; })
            .uniqueId;

        downloadingTitle.text = title;
        downloadingInfo.text = msg;
    }

    public void ShowDownloadSlider(bool show)
    {
        slider.gameObject.SetActive(show);
    }

    public void SetDownloadMessage(string message, string submessage)
    {
        downloadingTitle.overflowMode = TextOverflowModes.Overflow;
        downloadingTitle.text = message;
        downloadingInfo.text = submessage;
    }

    public void ShowLogos(System.Action onComplete)
    {
        System.Action<int> showLogo = (idx) => { };
        showLogo = (idx) =>
        {
            if (idx >= logos.Length)
            {
                StartCoroutine(CovenLogoCoroutine(onComplete));
                return;
            }

            if (logos[idx] == null)
            {
                showLogo(idx + 1);
                return;
            }

            logos[idx].gameObject.SetActive(true);
            LeanTween.alphaCanvas(logos[idx], 1, fadeTime).setEaseOutCubic().setOnComplete(() =>
            {
                LeanTween.value(0, 0, logoTime).setOnComplete(() =>
                {
                    LeanTween.alphaCanvas(logos[idx], 0, fadeTime).setEaseOutCubic().setOnComplete(() =>
                    {
                        logos[idx].gameObject.SetActive(false);
                        showLogo(idx + 1);
                    });
                });
            });
        };

        showLogo(0);
    }
    
    private IEnumerator CovenLogoCoroutine(System.Action onComplete)
    {
        VideoPlayback.gameObject.SetActive(true);
        VideoPlayback.Load("Splash.mp4");

        bool videoReady = false;
        VideoPlayback.OnVideoFirstFrameReady += () => videoReady = true;

        while (!videoReady)
            yield return 0;

        VideoPlayback.GetComponent<RawImage>().color = Color.white;
        yield return new WaitForSeconds(splashTime);
        VideoPlayback.gameObject.SetActive(false);

        onComplete?.Invoke();
    }
}
