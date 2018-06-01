using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerNotificationManager : MonoBehaviour
{
	//public Image[] icons;
	public GameObject[] notifications;
	public Text[] info;
	public static PlayerNotificationManager Instance { get; set;}
	public int currentNotification = 0;
	float minTimeGap = 2.5f;
	// Use this for initialization
	void Awake()
	{
		Instance = this;
	}
	
	public void showNotification(WebSocketResponse wb)
	{
		if (currentNotification < 4) {
			string s = wb.caster + " attacks you. You suffer " + wb.result.total.ToString () + " energy.";
			if (wb.result.critical) {
				s = "CRITICAL ATTACK! You suffer " + wb.result.total.ToString () + " energy from the " + wb.caster + "'s attack.";
			}
			info [currentNotification].text = s;
			notifications [currentNotification].SetActive (true);
			currentNotification++;
			Invoke ("DecreaseNotification", minTimeGap);
		}
	}

	void DecreaseNotification()
	{
		currentNotification--;
	}
}

