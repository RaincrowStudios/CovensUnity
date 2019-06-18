using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppsFlyerAPI : MonoBehaviour {

    AppsFlyerTrackerCallbacks AFTrackerCallbacks;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void TrackStorePurchaseEvent(string contentID, string currency = "USD")
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

    public static void CreatedAccount()
    {
        Debug.Log("Created account in afAPI is being called");

        AppsFlyer.trackRichEvent(AFInAppEvents.ACCOUNT_REGISTRATION, new Dictionary<string, string>() {
            {AFInAppEvents.QUANTITY, "1"}
        });
    }

    public static void CreatedWitch()
    {
        Debug.Log("Created witch in afAPI is being called");

        AppsFlyer.trackRichEvent(AFInAppEvents.WITCH_CREATION, new Dictionary<string, string>() {
            {AFInAppEvents.QUANTITY, "1"}
        });
    }

    public static void CreatedAvatar()
    {
        Debug.Log("Created avatar in afAPI is being called");

        AppsFlyer.trackRichEvent(AFInAppEvents.AVATAR_CHOSEN, new Dictionary<string, string>() {
            {AFInAppEvents.QUANTITY, "1"}
        });
    }

    public static void CompletedFTUE()
    {
        Debug.Log("Completed ftue in afAPI is being called");

        AppsFlyer.trackRichEvent(AFInAppEvents.TUTORIAL_COMPLETION, new Dictionary<string, string>() {
            {AFInAppEvents.QUANTITY, "1"}
        });
    }

    public static void ReachedLevelThree()
    {
        Debug.Log("Reached level three in afAPI is being called");

        AppsFlyer.trackRichEvent(AFInAppEvents.LEVEL3_ACHIEVED, new Dictionary<string, string>() {
            {AFInAppEvents.QUANTITY, "1"}
        });
    }
}
