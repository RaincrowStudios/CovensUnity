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
    
    private double[] tribunalStamps = new double[] { 1553040000, 1561075200, 1569196800, 1576972800, 1584662400, 1592697600 };
    private int[] tribunals = new int[] { 1, 2, 3, 4, 1, 2 };

    private float m_LogoSpeed = 1;
    private int m_SliderTweenId;
    private int m_HintTweenId;
    private Coroutine m_HintsCoroutine;

    void Awake()
    {
        Instance = this;

        progressBar.fillAmount = 0;
        progressBar.gameObject.SetActive(false);

        hintText.text = "";
        spiritName.text = "";
        spirit.overrideSprite = null;

        foreach (var logo in logos)
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

        if (Application.isEditor)
            m_LogoSpeed = 5f;
        else
            m_LogoSpeed = 1f;
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
        if (progress == 0)
        {
            slider.value = 0;
        }
        else
        {
            m_SliderTweenId = LeanTween.value(slider.value, progress, 0.2f)
                .setOnUpdate((float v) => { slider.value = v; })
                .uniqueId;
        }

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
            LeanTween.alphaCanvas(logos[idx], 1, fadeTime/m_LogoSpeed).setEaseOutCubic().setOnComplete(() =>
            {
                LeanTween.value(0, 0, logoTime/m_LogoSpeed).setOnComplete(() =>
                {
                    LeanTween.alphaCanvas(logos[idx], 0, fadeTime/m_LogoSpeed).setEaseOutCubic().setOnComplete(() =>
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
        if (!VideoPlayback.m_bAutoPlay)
            VideoPlayback.Play();
        VideoPlayback.SetSpeed(m_LogoSpeed);
        
        //wait for video to be ready to start
        bool videoReady = false;
        VideoPlayback.OnVideoFirstFrameReady += () => videoReady = true;
        while (!videoReady)
            yield return 0;

        VideoPlayback.GetComponent<RawImage>().color = Color.white;

        ////wait for video to finish playback
        //videoReady = false;
        //VideoPlayback.OnEnd += () => videoReady = true;
        //while (!videoReady)
        //    yield return 0;
        yield return new WaitForSeconds(splashTime / m_LogoSpeed);

        VideoPlayback.gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    public void ShowHints(System.Action onStart)
    {
        if (m_HintsCoroutine != null)
            StopCoroutine(m_HintsCoroutine);

        m_HintsCoroutine = StartCoroutine(HintsCoroutine(onStart));
    }

    public void HideHints(System.Action onComplete)
    {
        if (m_HintsCoroutine == null)
            return;

        m_HintTweenId = LeanTween.alphaCanvas(m_HintScreen, 1f, 0f)
            .setOnComplete(() =>
            {
                m_HintScreen.gameObject.SetActive(false);
                onComplete?.Invoke();
            })
            .uniqueId;
    }

    private IEnumerator HintsCoroutine(System.Action onStart)
    {
        ShowNewHint();

        yield return new WaitForSeconds(1f);
        onStart?.Invoke();

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
                ShowNewHint();

            yield return 0;
        }
    }

    private void ShowNewHint()
    {
        LeanTween.cancel(m_HintTweenId);

        m_HintScreen.alpha = 0;
        m_HintScreen.gameObject.SetActive(true);

        //get all available hints
        List<string> tips = new List<string>();
        int i = 0;
        while (DownloadedAssets.localizedText.ContainsKey("tip_" + i))
        {
            tips.Add("tip_" + i);
            i++;
        }

        //get a random hint
        hintText.text = LocalizeLookUp.GetText(tips[Random.Range(0, tips.Count)]);
        
        //get a random spirit
        SpiritData spiritData = DownloadedAssets.spiritDict.ElementAt(Random.Range(0, DownloadedAssets.spiritDict.Count)).Value;
        spiritName.text = spiritData.Name;
        spirit.overrideSprite = null;
        DownloadedAssets.GetSprite(spiritData.id, (spr) =>
        {
            spirit.overrideSprite = spr;
        });

        m_HintTweenId = LeanTween.alphaCanvas(m_HintScreen, 1f, 1f).uniqueId;
    }
}
