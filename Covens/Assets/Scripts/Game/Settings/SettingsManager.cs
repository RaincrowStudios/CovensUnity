using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook.Unity.Example;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; set; }

    public static string IsFb
    {
        get { return PlayerPrefs.GetString("fb", ""); }
        set { PlayerPrefs.SetString("fb", value); }
    }
    public Animator anim;

    int currWitchButton;
    int currCollButton;
    int currSpiritButton;

    //	public GameObject loginButton;
    public GameObject profileObject;
    public Text playerFBName;
    public Image DisplayPic;

    [SerializeField]
    private TextMeshProUGUI m_AppVersion;

    int minWitch = 15;
    int minSpirits = 15;
    int minCollectibles = 15;

    int moreWitch = 30;
    int moreSpirits = 30;
    int moreCollectibles = 30;

    int maxWitch = 50;
    int maxSpirits = 50;
    int maxCollectibles = 50;

    //Map Marker Settings
    public Button[] witchMarkers = new Button[3];
    public Button[] collectibleMarkers = new Button[3];
    public Button[] spiritMarkers = new Button[3];
    public Button[] buildingsOnOff = new Button[2];
    public Button[] soundOnOff = new Button[2];

    public Color buttonSelected;
    public Color buttonNotSelected;

    public Vector3 unselectedButtonSize;
    public Vector3 selectedButtonSize;

    public static MapMarkerAmount mapMarkerAmount;

    void Awake()
    {
        Instance = this;
        int memory = (int)Mathf.Clamp(SystemInfo.systemMemorySize, 1500, 6000);
        //Debug.Log(memory);
        int witches = (int)MapUtils.scale(minWitch, maxWitch, 1500, 6000, memory);
        //Debug.Log(witches);
        mapMarkerAmount = new MapMarkerAmount
        {
            witch = witches,
            collectible = witches,
            spirit = witches
        };

        if (SystemInfo.systemMemorySize < 3000)
        {
            //MapController.Instance.m_StreetMap.EnableBuildings(false);
            EnableDisableBuildings(false);
        }

        APIManager.Instance.PostData("character/configuration", JsonConvert.SerializeObject(mapMarkerAmount), MarkerPostCallback);
    }
    // Use this for initialization
    private void Start()
    {
        //setting up listeners for buttons
        {
            soundOnOff[0].onClick.AddListener(() => { ToggleSound(true); });
            soundOnOff[1].onClick.AddListener(() => { ToggleSound(false); });

            buildingsOnOff[0].onClick.AddListener(() => { EnableDisableBuildings(true); });
            buildingsOnOff[1].onClick.AddListener(() => { EnableDisableBuildings(false); });

            witchMarkers[0].onClick.AddListener(() => { ToggleWitches(0); });
            witchMarkers[1].onClick.AddListener(() => { ToggleWitches(1); });
            witchMarkers[2].onClick.AddListener(() => { ToggleWitches(2); });

            collectibleMarkers[0].onClick.AddListener(() => { ToggleCollectibles(0); });
            collectibleMarkers[1].onClick.AddListener(() => { ToggleCollectibles(1); });
            collectibleMarkers[2].onClick.AddListener(() => { ToggleCollectibles(2); });

            spiritMarkers[0].onClick.AddListener(() => { ToggleSpirits(0); });
            spiritMarkers[1].onClick.AddListener(() => { ToggleSpirits(1); });
            spiritMarkers[2].onClick.AddListener(() => { ToggleSpirits(2); });
        }

        ToggleSound(true);
        EnableDisableBuildings(true);
        ToggleWitches(0);
        ToggleCollectibles(0);
        ToggleSpirits(0);



#if UNITY_IOS || UNITY_ANDROID
        StartCoroutine(checkBatteryLevel());
        Debug.Log("BATTERY LEVEL AT " + GetBatteryLevel());
#endif
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
        //restart put here by matt
        RestartGame();
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

    public void ToggleWitches(int buttonClicked)
    {
        witchMarkers[currWitchButton].interactable = true;
        //change color or sprite of previous button here
        witchMarkers[currWitchButton].GetComponent<Image>().color = buttonNotSelected;
        witchMarkers[buttonClicked].interactable = false;
        currWitchButton = buttonClicked;
        witchMarkers[currWitchButton].GetComponent<Image>().color = buttonSelected;
        //Need to add logic for min/more/max

        //this is for posting marker data
    }

    public void ToggleCollectibles(int buttonClicked)
    {
        collectibleMarkers[currCollButton].interactable = true;
        collectibleMarkers[currCollButton].GetComponent<Image>().color = buttonNotSelected;
        //change color or sprite of previous button here
        collectibleMarkers[buttonClicked].interactable = false;
        currCollButton = buttonClicked;
        collectibleMarkers[currCollButton].GetComponent<Image>().color = buttonSelected;
        //Need to add logic for min/more/max


    }

    public void ToggleSpirits(int buttonClicked)
    {
        spiritMarkers[currSpiritButton].interactable = true;
        //change color or sprite of previous button here
        spiritMarkers[currSpiritButton].GetComponent<Image>().color = buttonNotSelected;
        spiritMarkers[buttonClicked].interactable = false;
        currSpiritButton = buttonClicked;
        spiritMarkers[currSpiritButton].GetComponent<Image>().color = buttonSelected;
        //Need to add logic for min/more/max


    }

    public void ToggleSound(bool soundOn)
    {
        if (soundOn)
        {
            soundOnOff[0].interactable = false;
            soundOnOff[0].GetComponent<Image>().color = buttonSelected;
            soundOnOff[1].interactable = true;
            soundOnOff[1].GetComponent<Image>().color = buttonNotSelected;
            AudioListener.pause = false;
        }
        else
        {
            soundOnOff[1].interactable = false;
            soundOnOff[1].GetComponent<Image>().color = buttonSelected;
            soundOnOff[0].interactable = true;
            soundOnOff[0].GetComponent<Image>().color = buttonNotSelected;
            AudioListener.pause = true;

        }
        //AudioListener.pause = !AudioListener.pause;
    }

    public void EnableDisableBuildings(bool enableBuildings)
    {
        
        if (enableBuildings)
        {
            //Enables Buildings
            buildingsOnOff[0].interactable = false;
            buildingsOnOff[0].GetComponent<Image>().color = buttonSelected;
            buildingsOnOff[1].interactable = true;
            buildingsOnOff[1].GetComponent<Image>().color = buttonNotSelected;
            //MapController.Instance.m_StreetMap.EnableBuildings(true);
        }
        else
        {
            //Disables Buildings
            buildingsOnOff[1].interactable = false;
            buildingsOnOff[1].GetComponent<Image>().color = buttonSelected;
            buildingsOnOff[0].interactable = true;
            buildingsOnOff[0].GetComponent<Image>().color = buttonNotSelected;
            //MapController.Instance.m_StreetMap.EnableBuildings(false);
        }
    }

    public void MarkerPostCallback(string result, int code)
    {
        Debug.Log(result);
        if (code == 200)
        {
            Debug.Log("data posted");
        }
        else
        {
            Debug.LogError("Code: " + code);
        }
    }

    public void ChangeSoundLevel(float value)
    {
        AudioListener.volume = value;
    }

    //push 

    // credits (low)

    // legal stuff (low)

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
        gameObject.SetActive(true);
        //Debug.Log("showing settings");
        //anim.SetBool("animate", true);

        m_AppVersion.text = string.Concat("App Version: ", DownloadedAssets.AppVersion);
    }

    public void Hide()
    {
        gameObject.SetActive(false);

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