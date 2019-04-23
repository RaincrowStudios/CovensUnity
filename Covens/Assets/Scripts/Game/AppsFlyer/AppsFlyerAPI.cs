using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppsFlyerAPI : MonoBehaviour {

    public static AppsFlyerAPI Instance { get; set; }

    AppsFlyerTrackerCallbacks AFTrackerCallbacks;



	// Use this for initialization
	void Start () {
        AppsFlyer.trackRichEvent(AFInAppEvents.LEVEL_ACHIEVED, new Dictionary<string, string>() {

        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TrackStorePurchaseEvent(string contentID, string currency = "USD")
    {
        string contentAddress = "com.raincrow.covens." + contentID;
        string revenue = "";
        switch (contentID)
        {
            case "silver1":
                revenue = "0.99";
                break;
            case "silver2":
                revenue = "4.99";
                break;
            case "silver3":
                revenue = "9.99";
                break;
            case "silver4":
                revenue = "19.99";
                break;
            case "silver5":
                revenue = "39.99";
                break;
            case "silver6":
                revenue = "89.99";
                break;
            default:
                revenue = "error";
                break;
        }
        //contentID is intended to be one of the strings initialized in IAPSilver.cs
        AppsFlyer.trackRichEvent(AFInAppEvents.PURCHASE, new Dictionary<string, string>() {
            {AFInAppEvents.CONTENT_ID, contentAddress},
            {AFInAppEvents.REVENUE, revenue},
            {AFInAppEvents.CURRENCY, currency},
            {AFInAppEvents.QUANTITY, "1"}
        });
    }
}
