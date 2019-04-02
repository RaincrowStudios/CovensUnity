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

    public void OnEnable()
    {
        instance = this;

        if (Application.isEditor)
        {
            float range = 1f / 300f;
            lng = lng + Random.Range(-range, range);
            range = 1f / 450f;
            lat = lat + Random.Range(-range, range);
        }
    }

    public static float longitude
    {
        get
        {
            if (Application.isEditor)
                return instance.lng;

            return Input.location.lastData.longitude;
        }
    }
    public static float latitude
    {
        get
        {
            if (Application.isEditor)
                return instance.lat;
            return Input.location.lastData.latitude;
        }
    }

    IEnumerator Start()
    {
        //		foreach (var item in DoGetHostEntry("https://raincrowstudios.xyz/manager")) {
        //			Ping p = new Ping (item.ToString ());
        //			yield return p;
        //
        //		}
        // First, check if user has location service enabled
        //		Debug.Log(Application.systemLanguage);

        if (Application.isEditor)
        {
            PlayerDataManager.playerPos = new Vector2(lng, lat);
            StartUpManager.Instance.Init();
            yield break;
        }

        if (!Input.location.isEnabledByUser)
        {
            locationError.SetActive(true);
            GPSicon.SetActive(true);
            WifiIccon.SetActive(false);
            errorText.text = "Please turn on your location and try again.";
            yield break;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            locationError.SetActive(true);
            GPSicon.SetActive(false);
            WifiIccon.SetActive(true);
            errorText.text = "Please check your internet connection and try again.";
            yield break;
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

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            GPSicon.SetActive(true);
            WifiIccon.SetActive(false);
            errorText.text = "Please turn on your location and try again.";
            yield break;
        }
        else
        {
            StartUpManager.Instance.Init();
            lat = Input.location.lastData.latitude;
            lng = Input.location.lastData.longitude;

            //PlayerDataManager.playerPos.y = Input.location.lastData.latitude;
            //PlayerDataManager.playerPos.x = Input.location.lastData.longitude;
            PlayerDataManager.playerPos = new Vector2(lng, lat);

            // Access granted and location value could be retrieved
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }

    //	public static IPAddress[] DoGetHostEntry(string hostName )
    //	{
    //		
    //		IPHostEntry host = Dns.GetHostEntry(hostName);
    //		return host.AddressList;
    //
    //	}
}