using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using UnityEngine.Networking;

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
    double[] tribunalStamps = new double[] { 1553040000, 1561075200, 1569196800, 1576972800, 1584662400, 1592697600 };
    int[] tribunals = new int[] { 1, 2, 3, 4, 1, 2 };

    public GameObject ServerDown;
    public GameObject OutdatedBuild;

    bool showLogos = false;
    bool showVideo = false;
    bool ShowSplash = false;
    bool showStats = false;
    bool activateScene = false;
    public static bool loginCheck = false;
    public static bool loginStatus = false;

    void Awake()
    {
        Instance = this;
        //  Application.targetFrameRate = 60;
    }

    public void Init()
    {
        if (Application.isEditor)
        {
            activateScene = true;
        }
        StartCoroutine(FadeIn(0));
        continueButton.SetActive(false);
        StatScreen.SetActive(false);
        LoadingImage.SetActive(false);
        VideoPlayback.gameObject.SetActive(false);
        StartCoroutine(SetSceneActivation());
        StartCoroutine(ShowHint());
    }

    IEnumerator FadeIn(int i)
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
            showLogos = true;
            // Debug.Log("Showing video playback");
            VideoPlayback.gameObject.SetActive(true);
            VideoPlayback.Load("Splash.mp4");

            bool videoReady = false;
            VideoPlayback.OnVideoFirstFrameReady += () => videoReady = true;

            while (!videoReady)
                yield return 0;

            VideoPlayback.GetComponent<RawImage>().color = Color.white;
            yield return new WaitForSeconds(splashTime);
            showVideo = true;
            VideoPlayback.gameObject.SetActive(false);
            StartCoroutine(ShowTribunalTimer());
            // StartCoroutine(ShowHint());
        }

    }

    public void OutDatedBuild()
    {
        OutdatedBuild.SetActive(true);
        this.StopAllCoroutines();
    }

    IEnumerator ShowHint()
    {
        // Debug.Log("Showing Hint");
        // HintObject.SetActive(true);
        yield return new WaitUntil(() => DownloadAssetBundle.isDictLoaded == true);
        tip.text = DownloadedAssets.tips[Random.Range(0, DownloadedAssets.tips.Count)].value;


        var k = DownloadedAssets.spiritDictData.ElementAt(Random.Range(0, DownloadedAssets.spiritDictData.Count));
        //spirit.sprite = k.Value;
        spiritName.text = k.Value.Name;
        spirit.color = new Color(0, 0, 0, 0);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(DownloadAssetBundle.baseURL + "spirit/" + k.Key + ".png");
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            spirit.color = new Color(0, 0, 0, 0);
            Debug.LogError("error loading hint spirit sprite: \"" + k.Key + "\"\nerror: " + www.error);
            Debug.Log(www.error);
        }
        else
        {
            // Debug.Log("got Texture");
            var tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            spirit.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
            spirit.color = Color.white;
        }


        yield return new WaitUntil(() => DownloadAssetBundle.isAssetBundleLoaded == true);

        // Hint.anchoredPosition = new Vector2(0, -92);
        // Hint.localScale = Vector3.one * 1.05f;

    }

    IEnumerator ShowTribunalTimer()
    {

        double currentTime = (double)System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        // Debug.Log(currentTime);
        int currentI = 0;
        for (int i = 0; i < tribunalStamps.Length; i++)
        {
            if (tribunalStamps[i] > currentTime)
            {
                currentI = --i;
                break;
            }
        }
        // log
        int tribunal = tribunals[currentI];


        if (tribunal == 2)
        {
            tribunalTitle.text = LocalizeLookUp.GetText("summer_tribunal_upper");
        }
        else if (tribunal == 1)
        {
            tribunalTitle.text = LocalizeLookUp.GetText("spring_tribunal_upper");
        }
        else if (tribunal == 3)
        {
            tribunalTitle.text = LocalizeLookUp.GetText("autumn_tribunal_upper");
        }
        else
        {
            tribunalTitle.text = LocalizeLookUp.GetText("winter_tribunal_upper");
        }

        System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        // Debug.Log(tribunalStamps[currentI + 1]);
        dtDateTime = dtDateTime.AddSeconds(tribunalStamps[currentI + 1]).ToUniversalTime();
        var timeSpan = dtDateTime.Subtract(System.DateTime.UtcNow);

        tribunalTimer.text = timeSpan.TotalDays.ToString("N0");

        // HintObject.SetActive(false);
        splash.SetActive(true);
        // if()
        yield return new WaitUntil(() => loginCheck);
        if (loginStatus)
        {
            currentDominion.text = LocalizeLookUp.GetText("dominion_location") + " " + config.dominion;
            strongestWitch.text = LocalizeLookUp.GetText("strongest_witch_dominion") + " " + config.strongestWitch;
            strongestCoven.text = LocalizeLookUp.GetText("strongest_coven_dominion") + " " + config.strongestCoven;
            Invoke("EnableStats", 3f);
        }
        else
        {
            yield return new WaitForSeconds(3);
            ShowSplash = true;
            StartCoroutine(showHintAgain(false));
            showStats = true;
        }

    }

    public void DoSceneLoading()
    {
        // Debug.Log("||||||loading scene");
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
        // StartCoroutine(LoadMainScene());
        //SHow hint again
        ShowSplash = true;
        StartCoroutine(showHintAgain());
    }

    IEnumerator showHintAgain(bool setStats = true)
    {
        yield return new WaitForSeconds(3);

        if (setStats)
            showStats = true;
        StatScreen.SetActive(false);
        HintObject.SetActive(true);
    }

    IEnumerator SetSceneActivation()
    {
        yield return new WaitUntil(() => showLogos);
        yield return new WaitUntil(() => showVideo);
        yield return new WaitUntil(() => showStats);
        yield return new WaitUntil(() => ShowSplash);
        yield return new WaitForSeconds(2);
        // Debug.Log("Scene Load Active");
        activateScene = true;
    }

    IEnumerator LoadMainScene()
    {
        yield return new WaitForSeconds(splashTime + 1);
        SceneAO = SceneManager.LoadSceneAsync("MainScene");
        SceneAO.allowSceneActivation = false;
        while (!SceneAO.isDone)
        {
            if (activateScene) SceneAO.allowSceneActivation = true;
            progressBar.fillAmount = SceneAO.progress;
            yield return null;
        }

    }

    public void UpdateAppAndroid()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.raincrow.covens");
    }

    public void updateApple()
    {
        Application.OpenURL("https://apps.apple.com/us/app/covens/id1456181456");
    }

}
