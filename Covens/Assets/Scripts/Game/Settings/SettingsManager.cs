using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook.Unity.Example;
using Newtonsoft.Json;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; set; }

    public static string IsFb
    {
        get { return PlayerPrefs.GetString("fb", ""); }
        set { PlayerPrefs.SetString("fb", value); }
    }
    public Animator anim;
    //	public GameObject loginButton;
    public GameObject profileObject;
    public Text playerFBName;
    public Image DisplayPic;

    [SerializeField]
    private Text m_AppVersion;

    int minWitch = 15;
    int minSpirits = 15;
    int minCollectibles = 15;

    int maxWitch = 50;
    int maxSpirits = 50;
    int maxCollectibles = 50;

    public static MapMarkerAmount mapMarkerAmount;

    void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    private void Start()
    {
        int memory = (int)Mathf.Clamp(SystemInfo.systemMemorySize, 1500, 6000);
        int witches = (int)MapUtils.scale(minWitch, maxWitch, 1500, 6000, memory);
        //    Debug.Log(witches);
        mapMarkerAmount = new MapMarkerAmount
        {
            witch = witches,
            collectible = witches,
            spirit = witches,
        };
        if (SystemInfo.systemMemorySize < 3000)
        {
            MapController.Instance.m_StreetMap.EnableBuildings(false);
        }
#if UNITY_IOS || UNITY_ANDROID
        StartCoroutine(checkBatteryLevel());
        Debug.Log("BATTERY LEVEL AT " + GetBatteryLevel());
#endif

        APIManager.Instance.PostData("character/configuration", JsonConvert.SerializeObject(mapMarkerAmount), (string s, int r) => Debug.Log("sent"));
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
#if UNITY_IOS
         UIDevice device = UIDevice.CurrentDevice();
         device.batteryMonitoringEnabled = true; // need to enable this first
         Debug.Log("Battery state: " + device.batteryState);
         Debug.Log("Battery level: " + device.batteryLevel);
         return device.batteryLevel*100;
#elif UNITY_ANDROID

        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    if (null != unityPlayer)
                    {
                        using (AndroidJavaObject currActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                        {
                            if (null != currActivity)
                            {
                                using (AndroidJavaObject intentFilter = new AndroidJavaObject("android.content.IntentFilter", new object[] { "android.intent.action.BATTERY_CHANGED" }))
                                {
                                    using (AndroidJavaObject batteryIntent = currActivity.Call<AndroidJavaObject>("registerReceiver", new object[] { null, intentFilter }))
                                    {
                                        int level = batteryIntent.Call<int>("getIntExtra", new object[] { "level", -1 });
                                        int scale = batteryIntent.Call<int>("getIntExtra", new object[] { "scale", -1 });

                                        // Error checking that probably isn't needed but I added just in case.
                                        if (level == -1 || scale == -1)
                                        {
                                            return 50f;
                                        }
                                        return ((float)level / (float)scale) * 100.0f;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {

            }
        }

        return 100;
#endif
    }

    // void OnGUI()
    // {
    //     if (GUI.Button(new Rect(55, 200, 180, 40), "Logout"))
    //     {
    //         LogOut();
    //         StartCoroutine(RestartGame());
    //     }
    // }

    public void LogOut()
    {
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.DeleteKey("Password");
    }

    IEnumerator RestartGame()
    {

        /*will add restarting scene functionality later */

        // LoginAPIManager.isInFTF = false;
        // LoginAPIManager.sceneLoaded = false;
        // LoginAPIManager.hasCharacter = false;
        // LoginAPIManager.FTFComplete = false;
        // LoginAPIManager.loggedIn = false;
        // MarkerManager.Markers.Clear();
        // WebSocketClient.Instance.AbortThread();
        // yield return SceneManager.UnloadSceneAsync(1);
        // SceneManager.LoadScene(0);

        /*show restart game message to log back in */
        Application.Quit();
        yield return 0;
    }

    public void ToggleSound()
    {
        AudioListener.pause = !AudioListener.pause;
    }

    public void ChangeSoundLevel(float value)
    {
        AudioListener.volume = value;

    }

    //push 

    // credits

    // legal stuff

    public void FbLoginSetup()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallBack);
        }
    }

    void InitCallBack()
    {
        //		loginButton.SetActive (true);
        profileObject.SetActive(false);
        if (IsFb != "")
        {
            if (!Application.isEditor)
                LoginFB();
        }
    }

    public void LoginFB()
    {
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, HandleResult);
    }

    public void HandleResult(IResult result)
    {
        if (result == null)
        {
            Debug.Log("Login Failed");
            return;
        }

        if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Error - Check log for details");
            //			this.LastResponse = "Error Response:\n" + result.Error;
        }
        else if (result.Cancelled)
        {
            Debug.Log("Cancelled - Check log for details");
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            Debug.Log("FB Logged in Success!");
            IsFb = "true";
            FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, FBPicCallBack);
            FB.API("/me?fields=first_name", HttpMethod.GET, FBNameCallBack);


        }
        else
        {
            Debug.Log("Empty Response\n");
        }
    }

    void FBPicCallBack(IGraphResult result)
    {
        if (string.IsNullOrEmpty(result.Error) && result.Texture != null)
        {
            DisplayPic.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
            //			loginButton.SetActive (false);
            profileObject.SetActive(true);
        }
    }

    void FBNameCallBack(IGraphResult result)
    {
        IDictionary<string, object> profile = result.ResultDictionary;
        playerFBName.text = profile["first_name"].ToString();
        Debug.Log(playerFBName.text);
    }

    public void Show()
    {
        Debug.Log("showing settings");
        anim.SetBool("animate", true);

        m_AppVersion.text = string.Concat("App Version: ", DownloadedAssets.AppVersion);
    }

    public void Hide()
    {
        anim.SetBool("animate", false);
        m_AppVersion.text = string.Empty;
    }
}

public class MapMarkerAmount
{
    public int witch { get; set; }
    public int collectible { get; set; }
    public int spirit { get; set; }
}