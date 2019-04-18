using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook.Unity.Example;
using Newtonsoft.Json;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; set; }

    public static string IsFb
    {
        get { return PlayerPrefs.GetString("fb", ""); }
        set { PlayerPrefs.SetString("fb", value); }
    }

    int currWitchButton;
    int currCollButton;
    int currSpiritButton;

    //	public GameObject loginButton;
    public GameObject profileObject;
    public Text playerFBName;
    public Image DisplayPic;

    public GameObject Credits;
    public GameObject creditsClone;

    [SerializeField]
    private TextMeshProUGUI m_AppVersion;

    int minWitch = 15;
    int minSpirits = 15;
    int minCollectibles = 15;

    // int moreWitch = 30;
    // int moreSpirits = 30;
    // int moreCollectibles = 30;

    int maxWitch = 50;
    int maxSpirits = 50;
    int maxCollectibles = 50;

    // int[] customSelection = new int[] { 15, 30, 50 };
    //Map Marker Settings
    public Button[] witchMarkers = new Button[3];
    public Button[] collectibleMarkers = new Button[3];
    public Button[] spiritMarkers = new Button[3];
    public Button[] buildingsOnOff = new Button[2];
    public Button[] soundOnOff = new Button[2];

    public Button tOS;
    public Button privacyPolicy;

    public Color buttonSelected;
    public Color buttonNotSelected;

    public Vector3 vectButtonSelected;
    public Vector3 vectButtonNotSel;

    public CanvasGroup CG;
    public GameObject container;
    public static MapMarkerAmount mapMarkerAmount;

    public static string audioConfig
    {
        get { return PlayerPrefs.GetString("configAudio", ""); }
        set { PlayerPrefs.SetString("configAudio", value); }
    }

    public static string buildingConfig
    {
        get { return PlayerPrefs.GetString("buildingConfig", ""); }
        set { PlayerPrefs.SetString("buildingConfig", value); }
    }


    void Awake()
    {
        Instance = this;
        int memory = (int)Mathf.Clamp(SystemInfo.systemMemorySize, 1500, 6000);
        int witches = (int)MapUtils.scale(minWitch, maxWitch, 1500, 6000, memory);
        vectButtonNotSel.Set(1f, 1f, 1f);
        vectButtonSelected.Set(1.1f, 1.1f, 1.1f);


        mapMarkerAmount = new MapMarkerAmount
        {
            witch = witches,
            collectible = witches,
            spirit = witches
        };

    }



    // Use this for initialization
    private void Start()
    {
        ToggleSound(audioConfig == "");
        //setting up listeners for buttons
        tOS.onClick.AddListener(LoginUIManager.Instance.openTOS);
        privacyPolicy.onClick.AddListener(LoginUIManager.Instance.openPP);
        soundOnOff[0].onClick.AddListener(() => { ToggleSound(true); });
        soundOnOff[1].onClick.AddListener(() => { ToggleSound(false); });

        buildingsOnOff[0].onClick.AddListener(() => { EnableDisableBuildings(true); });
        buildingsOnOff[1].onClick.AddListener(() => { EnableDisableBuildings(false); });


        if (buildingConfig == "" && SystemInfo.systemMemorySize < 3000)
        {
            //MapController.Instance.m_StreetMap.EnableBuildings(false);
            EnableDisableBuildings(false);
        }
        else if (buildingConfig == "true")
        {
            EnableDisableBuildings(true);
        }
        else
        {
            EnableDisableBuildings(false);

        }

        APIManager.Instance.PostData("character/configuration", JsonConvert.SerializeObject(mapMarkerAmount), (string s, int r) => { Debug.Log("sent"); });

#if UNITY_IOS || UNITY_ANDROID
        StartCoroutine(checkBatteryLevel());
        Debug.Log("BATTERY LEVEL AT " + GetBatteryLevel());
#endif

        // APIManager.Instance.PostData("character/configuration", JsonConvert.SerializeObject(mapMarkerAmount), (string s, int r) => Debug.Log("sent"));

    }

    IEnumerator checkBatteryLevel()
    {
        yield return new WaitForSeconds(40);
        if (GetBatteryLevel() < 15)
        {
            //make request
        }
        else
            StartCoroutine(checkBatteryLevel());
    }

    public static float GetBatteryLevel()
    {
        // #if UNITY_IOS
        //          UIDevice device = UIDevice.CurrentDevice();
        //          device.batteryMonitoringEnabled = true; // need to enable this first
        //          Debug.Log("Battery state: " + device.batteryState);
        //          Debug.Log("Battery level: " + device.batteryLevel);
        //          return device.batteryLevel*100;
        // #elif UNITY_ANDROID

        //         if (Application.platform == RuntimePlatform.Android)
        //         {
        //             try
        //             {
        //                 using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        //                 {
        //                     if (null != unityPlayer)
        //                     {
        //                         using (AndroidJavaObject currActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        //                         {
        //                             if (null != currActivity)
        //                             {
        //                                 using (AndroidJavaObject intentFilter = new AndroidJavaObject("android.content.IntentFilter", new object[] { "android.intent.action.BATTERY_CHANGED" }))
        //                                 {
        //                                     using (AndroidJavaObject batteryIntent = currActivity.Call<AndroidJavaObject>("registerReceiver", new object[] { null, intentFilter }))
        //                                     {
        //                                         int level = batteryIntent.Call<int>("getIntExtra", new object[] { "level", -1 });
        //                                         int scale = batteryIntent.Call<int>("getIntExtra", new object[] { "scale", -1 });

        //                                         // Error checking that probably isn't needed but I added just in case.
        //                                         if (level == -1 || scale == -1)
        //                                         {
        //                                             return 50f;
        //                                         }
        //                                         return ((float)level / (float)scale) * 100.0f;
        //                                     }

        //                                 }
        //                             }
        //                         }
        //                     }
        //                 }
        //             }
        //             catch (System.Exception ex)
        //             {

        //             }
        //         }

        //         return 100;
        // #endif
        return 100;
    }

    // void OnGUI()
    // {
    //     if (GUI.Button(new Rect(55, 200, 180, 40), "Logout"))
    //     {
    //         LogOut();
    //         StartCoroutine(RestartGame());
    //     }
    // }



    public void ToggleSound(bool soundOn)
    {
        if (soundOn)
        {
            soundOnOff[0].GetComponent<Image>().color = buttonSelected;
            LeanTween.scale(soundOnOff[0].gameObject, vectButtonSelected, 0.3f);
            soundOnOff[1].GetComponent<Image>().color = buttonNotSelected;
            LeanTween.scale(soundOnOff[1].gameObject, vectButtonNotSel, 0.3f);
            AudioListener.pause = false;
            audioConfig = "";
        }
        else
        {
            soundOnOff[1].GetComponent<Image>().color = buttonSelected;
            LeanTween.scale(soundOnOff[1].gameObject, vectButtonSelected, 0.3f);
            soundOnOff[0].GetComponent<Image>().color = buttonNotSelected;
            LeanTween.scale(soundOnOff[0].gameObject, vectButtonNotSel, 0.3f);
            AudioListener.pause = true;
            audioConfig = "false";
        }
    }

    public void EnableDisableBuildings(bool enableBuildings)
    {

        if (enableBuildings)
        {
            buildingsOnOff[0].GetComponent<Image>().color = buttonSelected;
            LeanTween.scale(buildingsOnOff[0].gameObject, vectButtonSelected, 0.3f);
            buildingsOnOff[1].GetComponent<Image>().color = buttonNotSelected;
            LeanTween.scale(buildingsOnOff[1].gameObject, vectButtonNotSel, 0.3f);
            if (container.activeInHierarchy)
                MapController.Instance.m_StreetMap.EnableBuildings(true);
            buildingConfig = "true";
        }
        else
        {
            buildingsOnOff[1].GetComponent<Image>().color = buttonSelected;
            LeanTween.scale(buildingsOnOff[1].gameObject, vectButtonSelected, 0.3f);
            buildingsOnOff[0].GetComponent<Image>().color = buttonNotSelected;
            LeanTween.scale(buildingsOnOff[0].gameObject, vectButtonNotSel, 0.3f);
            if (container.activeInHierarchy)
                MapController.Instance.m_StreetMap.EnableBuildings(false);
            buildingConfig = "false";

        }
    }

    public void ShowCredits()
    {
        creditsClone = Utilities.InstantiateObject(Credits, transform.GetChild(0));
        var rect = creditsClone.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
    }
    public void DestroyCredits()
    {
        if (this.transform.GetChild(0).GetChild(6) != null)
        {
            Destroy(creditsClone);
        }
    }

    public void ChangeSoundLevel(float value)
    {
        AudioListener.volume = value;
    }

    // public void FbLoginSetup()
    // {
    //     if (!FB.IsInitialized)
    //     {
    //         FB.Init(InitCallBack);
    //     }
    // }

    // void InitCallBack()
    // {
    //     //		loginButton.SetActive (true);
    //     profileObject.SetActive(false);
    //     if (IsFb != "")
    //     {
    //         if (!Application.isEditor)
    //             LoginFB();
    //     }
    // }

    // public void LoginFB()
    // {
    //     FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, HandleResult);
    // }

    // public void HandleResult(IResult result)
    // {
    //     if (result == null)
    //     {
    //         Debug.Log("Login Failed");
    //         return;
    //     }

    //     if (!string.IsNullOrEmpty(result.Error))
    //     {
    //         Debug.Log("Error - Check log for details");
    //         //			this.LastResponse = "Error Response:\n" + result.Error;
    //     }
    //     else if (result.Cancelled)
    //     {
    //         Debug.Log("Cancelled - Check log for details");
    //     }
    //     else if (!string.IsNullOrEmpty(result.RawResult))
    //     {
    //         Debug.Log("FB Logged in Success!");
    //         IsFb = "true";
    //         FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, FBPicCallBack);
    //         FB.API("/me?fields=first_name", HttpMethod.GET, FBNameCallBack);


    //     }
    //     else
    //     {
    //         Debug.Log("Empty Response\n");
    //     }
    // }

    // void FBPicCallBack(IGraphResult result)
    // {
    //     if (string.IsNullOrEmpty(result.Error) && result.Texture != null)
    //     {
    //         DisplayPic.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
    //         //			loginButton.SetActive (false);
    //         profileObject.SetActive(true);
    //     }
    // }

    // void FBNameCallBack(IGraphResult result)
    // {
    //     IDictionary<string, object> profile = result.ResultDictionary;
    //     playerFBName.text = profile["first_name"].ToString();
    //     Debug.Log(playerFBName.text);
    // }

    public void Show()
    {

        CG.alpha = 0;
        container.SetActive(true);
        container.transform.localScale = Vector3.zero;
        LeanTween.scale(container, Vector3.one, .35f).setEase(LeanTweenType.easeOutCirc).setOnComplete(() =>
        {
            UIStateManager.Instance.CallWindowChanged(false);
            MapController.Instance.SetVisible(false);
        });
        LeanTween.alphaCanvas(CG, 1, .35f);
        //Debug.Log("showing settings");
        //anim.SetBool("animate", true);

        m_AppVersion.text = string.Concat("App Version: ", DownloadedAssets.AppVersion);
    }

    public void Hide()
    {
        UIStateManager.Instance.CallWindowChanged(true);
        MapController.Instance.SetVisible(true);

        LeanTween.alphaCanvas(CG, 0, .35f).setOnComplete(() => container.SetActive(false));
        LeanTween.scale(container, Vector3.zero, .45f).setEase(LeanTweenType.easeInCubic);

        //anim.SetBool("animate", false);
        m_AppVersion.text = string.Empty;

    }
}

public class MapMarkerAmount
{
    public int witch { get; set; }
    public int collectible { get; set; }
    public int spirit { get; set; }
}