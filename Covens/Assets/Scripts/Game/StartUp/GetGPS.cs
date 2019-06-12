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

    private LocationServiceStatus m_LastStatus = LocationServiceStatus.Stopped;

    public static event System.Action<LocationServiceStatus> statusChanged;
    public static event System.Action OnInitialized;

    public static float longitude
    {
        get
        {
            if (Application.isEditor && instance != null)
                return instance.lng;

            return Input.location.lastData.longitude;
        }
        set
        {
            if (Application.isEditor && instance != null)
                instance.lng = value;
        }
    }
    public static float latitude
    {
        get
        {
            if (Application.isEditor && instance != null)
                return instance.lat;
            return Input.location.lastData.latitude;
        }
        set
        {
            if (Application.isEditor && instance != null)
                instance.lat = value;
        }
    }

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);

#if UNITY_ANDROID
        if (UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation) == false)
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);
#endif

        GPSicon.SetActive(false);
        WifiIccon.SetActive(false);
    }

    public void OnEnable()
    {
        if (Application.isEditor)
        {
            float range = 1f / 300f;
            lng = lng + Random.Range(-range, range);
            range = 1f / 450f;
            lat = lat + Random.Range(-range, range);
        }
    }

    IEnumerator Start()
    {
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            locationError.SetActive(true);
            GPSicon.SetActive(false);
            WifiIccon.SetActive(true);
            errorText.text = "Please check your internet connection and try again.";
            yield return new WaitForSeconds(0.1f);
        }

        if (Application.isEditor == false)
        {
            //if not enabled, wait for the user to enable it
            while (Input.location.isEnabledByUser == false)
            {
                errorText.GetComponent<LocalizeLookUp>().id = "location_error";
                locationError.SetActive(true);
                GPSicon.SetActive(true);
                WifiIccon.SetActive(false);
                errorText.text = "Please turn on your location and try again.";
                yield return new WaitForSeconds(1f);
            }

            // Start service before querying location
            Input.location.Start();
            yield return 0;

            // Wait until service initializes
            bool fail = Input.location.status != LocationServiceStatus.Running;
            while (fail)
            {
                int maxWait = 20;
                while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
                {
                    yield return new WaitForSeconds(1);
                    maxWait--;
                }

                // Service didn't initialize in 20 seconds
                fail = Input.location.status != LocationServiceStatus.Running;

                // Location init has failed or stopped
                if (fail)
                {
                    Debug.LogError("GPS init failed: " + Input.location.status);

                    errorText.GetComponent<LocalizeLookUp>().id = "location_error";
                    GPSicon.SetActive(true);
                    WifiIccon.SetActive(false);
                    errorText.text = "Please turn on your location and try again.";

                    //in case location was disabled
                    while (Input.location.isEnabledByUser == false)
                        yield return new WaitForSeconds(1f);

                    //try again
                    Input.location.Start();
                    yield return 0;
                }
            }

            lat = Input.location.lastData.latitude;
            lng = Input.location.lastData.longitude;
        }
        else
        {
            Debug.Log("[EDITOR] Skipping location check");
            yield return new WaitForSeconds(1f);
        }

        OnInitialized?.Invoke();
        StartCoroutine(CheckStatus());
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