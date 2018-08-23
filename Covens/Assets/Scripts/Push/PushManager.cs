using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushManager : MonoBehaviour {

	public static void InitPush () {
		// Enable line below to enable logging if you are having issues setting up OneSignal. (logLevel, visualLogLevel)
		// OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.INFO, OneSignal.LOG_LEVEL.INFO);
		try{
		OneSignal.StartInit("dfff330b-58b1-47dc-a287-a83b714ec95b")
			.HandleNotificationOpened(HandleNotificationOpened)
			.EndInit();
			
		OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
		OneSignal.SendTag ("instance", PlayerDataManager.playerData.instance);
		OneSignal.SendTag ("dominion", PlayerDataManager.playerData.dominion);
		}catch(System.Exception e) {
			Debug.LogError (e);
		}
	}

	// Gets called when the player opens the notification.
	private static void HandleNotificationOpened(OSNotificationOpenedResult result) {
	
	}
}
