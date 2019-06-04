using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
public class StartUpManager : MonoBehaviour
{

    public static StartUpManager Instance { get; set; }

    public GameObject LoadingImage;
    public GameObject StatScreen;
    public GameObject continueButton;
    public MediaPlayerCtrl VideoPlayback;
    public float splashTime = 15f;
    public Image progressBar;
    public CanvasGroup[] logos;
    public float logoTime;
    public float fadeTime;
    int i = 0;
    public GameObject HintObject;
    public TextMeshProUGUI hintText;
    public GameObject splash;
    public static Config config;
    public TextMeshProUGUI tribunalTimer;
    public TextMeshProUGUI tribunalTitle;

    public TextMeshProUGUI currentDominion;
    public TextMeshProUGUI strongestWitch;
    public TextMeshProUGUI strongestCoven;
    bool hasTriedLogin = false;

    public TextMeshProUGUI tip;
    public Image spirit;
    public TextMeshProUGUI spiritName;
    public RectTransform Hint;
    AsyncOperation SceneAO;

    public GameObject hint2;
    public TextMeshProUGUI tip2;
    public Image spirit2;
    public TextMeshProUGUI spiritName2;

    // public GameObject ServerDown;


    public GameObject OutdatedBuild;

    void Awake()
    {
        Instance = this;
        //  Application.targetFrameRate = 60;
    }

    private void Start()
    {
        //Setting up AppsFlyerStuff
        AppsFlyer.setAppsFlyerKey("Wdx4jw7TTNEEJYUh5UnaDB");
#if UNITY_IOS
        AppsFlyer.setAppID("com.raincrow.covens");
        //above is same as the android one
        AppsFlyer.trackAppLaunch();
#elif UNITY_ANDROID
        AppsFlyer.setAppID("com.raincrow.covens");
        AppsFlyer.init("Wdx4jw7TTNEEJYUh5UnaDB", "AppsFlyerTrackerCallbacks");
#endif
    }

    public void Init()
    {
        StartCoroutine(FadeIn(0));
        continueButton.SetActive(false);
        StatScreen.SetActive(false);
        LoadingImage.SetActive(false);
        VideoPlayback.gameObject.SetActive(false);
        DownloadAssetBundle.Instance.BeginDownload();
        //Setting up AppsFlyerStuff
        AppsFlyer.setAppsFlyerKey("1b2d8406-dec9-49ac-bace-5928f5c0f391");
#if UNITY_IOS
        AppsFlyer.setAppID("com.raincrow.covens");
        //above is same as the android one
        AppsFlyer.trackAppLaunch();
#elif UNITY_ANDROID
        AppsFlyer.setAppID("com.raincrow.covens");
        AppsFlyer.init("1b2d8406-dec9-49ac-bace-5928f5c0f391", "AppsFlyerTrackerCallbacks");
#endif
    }

    IEnumerator FadeIn(int i)
    {
        if (!Application.isEditor)
        {
            float t = 0;
            while (t <= 1f)
            {
                t += Time.deltaTime * fadeTime;
                logos[i].alpha = Mathf.SmoothStep(0, 1, t);
                yield return null;
            }

            yield return new WaitForSeconds(logoTime);

            while (t >= 0f)
            {
                t -= Time.deltaTime * fadeTime;
                logos[i].alpha = Mathf.SmoothStep(0, 1, t);
                yield return null;
            }
            i++;

            if (i < logos.Length)
            {
                StartCoroutine(FadeIn(i));
            }
            else
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
                StartCoroutine(ShowHint());
            }
        }
        else
        {
            StartCoroutine(ShowHint());
            yield return null;
        }
    }

    public void OutDatedBuild()
    {
        OutdatedBuild.SetActive(true);
        this.StopAllCoroutines();
    }

    IEnumerator ShowHint()
    {
        // DownloadAssetBundle.Instance.BeginDownload();

        HintObject.SetActive(true);
        yield return new WaitUntil(() => DownloadAssetBundle.isDictLoaded == true);
        tip.text = DownloadedAssets.tips[Random.Range(0, DownloadedAssets.tips.Count)].value;


        var k = DownloadedAssets.spiritDictData.ElementAt(Random.Range(0, DownloadedAssets.spiritDictData.Count));
        //spirit.sprite = k.Value;
        spiritName.text = k.Value.spiritName;
        WWW www = new WWW(DownloadAssetBundle.baseURL + "spirit/" + k.Key + ".png");
        yield return www;

        spirit.color = new Color(0, 0, 0, 0);
        if (www.texture != null)
        {
            spirit.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
            spirit.color = Color.white;
        }
        else
        {
            spirit.color = new Color(0, 0, 0, 0);
            Debug.LogError("error loading hint spirit sprite: \"" + k.Key + "\"\nerror: " + www.error);
        }

        yield return new WaitUntil(() => DownloadAssetBundle.isAssetBundleLoaded == true);

        Hint.anchoredPosition = new Vector2(0, -92);
        Hint.localScale = Vector3.one * 1.05f;

        tip2.text = tip.text;
        spirit2.sprite = spirit.sprite;
        spirit2.color = spirit.color;

        spiritName2.text = spiritName.text;
        LoginAPIManager.AutoLogin();
    }

    public void ShowTribunalTimer()
    {
        Debug.Log(config.tribunal);

        if (config.tribunal == 1)
        {
            tribunalTitle.text = LocalizeLookUp.GetText("summer_tribunal_upper");
        }
        else if (config.tribunal == 2)
        {
            tribunalTitle.text = LocalizeLookUp.GetText("spring_tribunal_upper");
        }
        else if (config.tribunal == 3)
        {
            tribunalTitle.text = LocalizeLookUp.GetText("autumn_tribunal_upper");
        }
        else
        {
            tribunalTitle.text = LocalizeLookUp.GetText("winter_tribunal_upper");
        }

        tribunalTimer.text = config.daysRemaining.ToString();
        currentDominion.text = LocalizeLookUp.GetText("dominion_location") + " " + config.dominion;
        strongestWitch.text = LocalizeLookUp.GetText("strongest_witch_dominion") + " " + config.strongestWitch;
        strongestCoven.text = LocalizeLookUp.GetText("strongest_coven_dominion") + " " + config.strongestCoven;
        HintObject.SetActive(false);
        splash.SetActive(true);

        Invoke("EnableStats", 3f);

    }

    public void DoSceneLoading()
    {
        StartCoroutine(LoadMainScene());
        DownloadAssetBundle.Instance.DownloadUI.SetActive(false);
        LoadingImage.SetActive(true);
        hasTriedLogin = true;
    }

    void EnableStats()
    {
        splash.GetComponent<Fade>().FadeOutHelper();
        StatScreen.SetActive(true);
        LoadingImage.SetActive(true);
        hasTriedLogin = true;
        StartCoroutine(LoadMainScene());
        Invoke("ShowHint2", 5);
    }

    void ShowHint2()
    {
        hint2.SetActive(true);
    }

    IEnumerator LoadMainScene()
    {
        yield return new WaitForSeconds(splashTime + 1);
        SceneAO = SceneManager.LoadSceneAsync("MainScene");
        //	SceneAO.allowSceneActivation = false;
        while (!SceneAO.isDone)
        {
            //progressBar.fillAmount = SceneAO.progress;
            // if (SceneAO.progress >= .9f) {
            // 	progressBar.fillAmount = 1;
            // 	continueButton.SetActive (true);
            // 	LoadingImage.SetActive (false);
            // }
            yield return null;
        }

    }

    void StartDelayedLoop()
    {
        StartCoroutine(DelayedLoop());
    }

    IEnumerator DelayedLoop()
    {

        for (int i = 0; i < 5; i++)
        {
            //your stuff
            yield return new WaitForSeconds(1);  // delay in seconds
        }

    }

    public void ContinueToMain()
    {
        SceneAO.allowSceneActivation = true;
    }

    public void UpdateAppAndroid()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.raincrow.covens");
    }

    public void updateApple()
    {
        Application.OpenURL("https://testflight.apple.com/join/dusXyBlR");
    }
}
