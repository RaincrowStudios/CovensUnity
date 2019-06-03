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

        StartCoroutine(CheckStatus());
    }

    IEnumerator Start()
    {
        if (Application.isEditor)
        {
            StartUpManager.Instance.Init();
            yield break;
        }

        //wait for gps to be enabled
        if (!Input.location.isEnabledByUser)
        {
            errorText.GetComponent<LocalizeLookUp>().id = "location_error";
            locationError.SetActive(true);
            GPSicon.SetActive(true);

            while (!Input.location.isEnabledByUser)
                yield return 0;

            locationError.SetActive(false);
            GPSicon.SetActive(false);
            errorText.text = "";
        }

        //wait fo rinternet to be available
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            locationError.SetActive(true);
            WifiIccon.SetActive(true);
            errorText.text = "Please check your internet connection and try again.";

            while (Application.internetReachability == NetworkReachability.NotReachable)
                yield return 0;

            locationError.SetActive(false);
            WifiIccon.SetActive(false);
            errorText.text = "";
        }

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        /////TO CHECK: THIS MAY BE CAUSING THE GAME TO GET STUCK IN LOADING SCREEN
        //// Service didn't initialize in 20 seconds
        //if (maxWait < 1)
        //{
        //    Debug.Log("Timed out");
        //    yield break;
        //}

        // Connection has failed
        while (Input.location.status != LocationServiceStatus.Running)
        {
            GPSicon.SetActive(true);

            //show error popup with Retry button
            bool waitingInput = true;
            UIGlobalErrorPopup.ShowError(() => 
            {
                Input.location.Start();
                waitingInput = false;
            },
            LocalizeLookUp.GetText("location_error"), "Retry");

            //wait for player's input
            while (waitingInput)
                yield return 0;

            maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }
        }

        StartUpManager.Instance.Init();
        lat = Input.location.lastData.latitude;
        lng = Input.location.lastData.longitude;

        // Access granted and location value could be retrieved
        Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
    }

    private IEnumerator CheckStatus()
    {
        LocationServiceStatus status;
        while (true)
        {
            status = Input.location.status;
            if(status != m_LastStatus)
            {
                m_LastStatus = status;
                statusChanged?.Invoke(status);

                if (status == LocationServiceStatus.Failed || status == LocationServiceStatus.Stopped)
                {
                    Debug.LogError("Location status changed: " + status +". Restarting.");
                    if (Application.isEditor == false)
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