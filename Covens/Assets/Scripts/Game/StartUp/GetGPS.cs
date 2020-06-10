using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;
using System.Collections.Generic;
using TMPro;
public class GetGPS : MonoBehaviour
{
    [SerializeField] private float lat;
    [SerializeField] private float lng;

    public static GetGPS instance { get; set; }

    public GameObject locationError;
    public GameObject WifiIccon;
    public GameObject GPSicon;
    public TextMeshProUGUI errorText;
    public GameObject askUserGPSPermission;
    public GameObject permissionDeniedAndroid;
    public Button goToAppSettingsBtn;

    public static int hasAskedPermission
    {
        get
        {
#if UNITY_ANDROID
            return PlayerPrefs.GetInt("gps", 0);
#endif
            return 0;
        }
        set
        {
#if UNITY_ANDROID
            PlayerPrefs.SetInt("gps", value);
            return;
#endif
        }
    }

    public Button continueToPermission;
    public Button declineToPermission;
    private LocationServiceStatus m_LastStatus = LocationServiceStatus.Stopped;

    public static event System.Action<LocationServiceStatus> statusChanged;
    public static event System.Action OnInitialized;

    public static Vector2 noise { get; private set; }

    public static bool IsReady => Input.location.status == LocationServiceStatus.Running;

    public static float LastLongitude { get; private set; }
    public static float LastLatitude { get; private set; }

    public static float longitude
    {
        get
        {
            if (Application.isEditor)
            {
                if (instance)
                {
                    LastLongitude = instance.lng;
                    return instance.lng;
                }
                else
                {
                    return LastLongitude;
                }
            }
            else
            {
                LastLongitude = Input.location.lastData.longitude;
                return LastLongitude;
            }
        }
        set
        {
            if (Application.isEditor && instance != null)
            {
                LastLongitude = instance.lng = value;
            }
        }
    }
    public static float latitude
    {
        get
        {
            if (Application.isEditor)
            {
                if (instance)
                {
                    LastLatitude = instance.lat;
                    return instance.lat;
                }
                else
                {
                    return LastLatitude;
                }
            }
            else
            {
                LastLatitude = Input.location.lastData.latitude;
                return LastLatitude;
            }
        }
        set
        {
            if (Application.isEditor && instance != null)
            {
                LastLatitude = instance.lat = value;
            }
        }
    }

    public static Vector2 coordinates
    {
        get { return new Vector2(longitude, latitude); }
    }

    public static void SetNoise()
    {
        Debug.Log("adding gps noise");
        Vector2 randCircle = Random.insideUnitCircle.normalized;
        noise = new Vector2(randCircle.x * 0.0005f, randCircle.y * 0.0005f);
    }

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);

#if UNITY_ANDROID
        if (UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation) == false)
        {
            askUserGPSPermission.SetActive(true);
            continueToPermission.onClick.AddListener(() =>
            {
                askUserGPSPermission.SetActive(false);
                hasAskedPermission = 1;
                UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);
            });

            declineToPermission.onClick.AddListener(() => {
                Application.Quit();
            });

            goToAppSettingsBtn.onClick.AddListener(() =>
            {
                RedirectToSettings();
            });
        }
#endif

        GPSicon.SetActive(false);
        WifiIccon.SetActive(false);
    }

    IEnumerator Start()
    {
        yield return null;

#if !LOCAL_API
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            locationError.SetActive(true);
            GPSicon.SetActive(false);
            WifiIccon.SetActive(true);
            errorText.text = "Please check your internet connection and try again.";
            yield return new WaitForSeconds(0.1f);
        }

        locationError.SetActive(false);
        GPSicon.SetActive(false);
        WifiIccon.SetActive(false);
#endif

        if (Application.isEditor == false)
        {
            Debug.Log("1. Location enabled by user: " + Input.location.isEnabledByUser);

            //if not enabled, wait for the user to enable it
            while (Input.location.isEnabledByUser == false)
            {
                errorText.GetComponent<LocalizeLookUp>().id = "location_error";
                locationError.SetActive(true);



                GPSicon.SetActive(true);
                WifiIccon.SetActive(false);
#if UNITY_ANDROID
                locationError.SetActive(false);
                GPSicon.SetActive(false);
                Debug.Log("LOCATION PERMISSION " + hasAskedPermission);
                if (hasAskedPermission == 1)
                    permissionDeniedAndroid.SetActive(true);
                else
                {
                    permissionDeniedAndroid.SetActive(false);
                    askUserGPSPermission.SetActive(true);
                }
#endif
                errorText.text = "Please turn on your location and try again.";
                yield return new WaitForSeconds(1f);
            }

#if UNITY_ANDROID
            permissionDeniedAndroid.SetActive(false);
#endif
            locationError.SetActive(false);

            // Start service before querying location
            Debug.Log("2. Starting location service");
            Input.location.Start();
            yield return 0;

            // Wait until service initializes
            bool fail = Input.location.status != LocationServiceStatus.Running;
            while (fail)
            {
                Debug.Log("3. Location status: " + Input.location.status.ToString());

                int maxWait = 5;
                while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
                {
                    yield return new WaitForSeconds(1);
                    maxWait--;
                    Debug.Log("4. Waiting for location service: " + Input.location.status.ToString() + "(" + maxWait + ")");
                }

                // Service didn't initialize in 20 seconds
                fail = Input.location.status != LocationServiceStatus.Running;

                // Location init has failed or stopped
                if (fail)
                {
                    Debug.LogError("GPS init failed: " + Input.location.status);

                    errorText.GetComponent<LocalizeLookUp>().id = "location_error";
                    GPSicon.SetActive(true);
                    locationError.SetActive(true);
                    WifiIccon.SetActive(false);
                    errorText.text = "Please turn on your location and try again.";

                    //in case location was disabled
                    Debug.Log("5. Location enabled by user: " + Input.location.isEnabledByUser);
                    while (Input.location.isEnabledByUser == false)
                        yield return new WaitForSeconds(1f);

                    //try again
                    Debug.Log("6. Starting location server.");
                    Input.location.Start();
                    yield return new WaitForSeconds(1);
                }
            }
            locationError.SetActive(false);

            lat = Input.location.lastData.latitude;
            lng = Input.location.lastData.longitude;
        }
        else
        {
            Debug.Log("[EDITOR] Skipping location check");
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("7. Location status: " + Input.location.status.ToString());
        Debug.Log(longitude + " : " + latitude);

        SetNoise();

        OnInitialized?.Invoke();
        StartCoroutine(CheckStatus());
    }

    private void RedirectToSettings()
    {
        try
        {
#if UNITY_ANDROID
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                string packageName = currentActivityObject.Call<string>("getPackageName");

                using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                {
                    intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                    intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                    currentActivityObject.Call("startActivity", intentObject);
                }
            }
#endif
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private IEnumerator CheckStatus()
    {
        if (Application.isEditor == false)
            yield break;

        LocationServiceStatus status;
        while (true)
        {
            status = Input.location.status;
            if (status != m_LastStatus)
            {
                m_LastStatus = status;
                statusChanged?.Invoke(status);

                if (status == LocationServiceStatus.Failed || status == LocationServiceStatus.Stopped)
                {
                    Debug.LogError("Location status changed: " + status + ". Restarting.");
                    Input.location.Start();
                }
                else
                {
                    Debug.Log("Location status changed: " + status);
                }
            }
            yield return new WaitForSeconds(2);
        }
    }
}