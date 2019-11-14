using System.Security;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushManager : MonoBehaviour
{
    public static double latitude { get; private set; }
    public static double longitude { get; private set; }
    public static bool flyToPop { get; private set; }

    void Awake()
    {
        LoginAPIManager.OnCharacterReceived += InitPush;
    }

    public static void InitPush()
    {
        Debug.Log("INIT PUSH");
        // Enable line below to enable logging if you are having issues setting up OneSignal. (logLevel, visualLogLevel)
        // OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.INFO, OneSignal.LOG_LEVEL.INFO);
        try
        {
            OneSignal.StartInit(CovenConstants.pushNotificationKey)
                .HandleNotificationOpened(HandleNotificationOpened)
                .EndInit();

            OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
            OneSignal.SendTag("instance", PlayerDataManager.playerData.instance);
            OneSignal.SendTag("dominion", PlayerDataManager.playerData.dominion);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public static System.Action OnNotificationReceived;
    // Gets called when the player opens the notification.
    private static void HandleNotificationOpened(OSNotificationOpenedResult result)
    {
        OSNotificationPayload payload = result.notification.payload;
        Dictionary<string, object> additionalData = payload.additionalData;
        if (additionalData.ContainsKey("longitude") && additionalData.ContainsKey("latitude"))
        {
            latitude = (double)(additionalData["latitude"]);
            longitude = (double)(additionalData["longitude"]);
            flyToPop = true;
        }
        else
        {
            flyToPop = false;
        }
    }
}
