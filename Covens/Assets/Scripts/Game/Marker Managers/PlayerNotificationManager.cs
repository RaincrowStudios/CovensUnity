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
	public Image[] displayIcon;
	public Sprite spirit;
	public Sprite spellBookIcon;
	public Sprite whiteWitchFemale;
	public Sprite greyWitchFemale;
	public Sprite shadowWitchFemale;
	public Sprite whiteWitchMale;
	public Sprite greyWitchMale;
	public Sprite shadowWitchMale;

	public Sprite male;
	public Sprite female;
	// Use this for initialization
	void Awake()
	{
		Instance = this;
	}
	
	public void showNotification( string message = "", Sprite icon = null)
	{
		if (currentNotification < 4) {
			displayIcon[currentNotification].sprite = icon;
			info [currentNotification].text = message;
			notifications [currentNotification].SetActive (true);
			currentNotification++;
			Invoke ("DecreaseNotification", minTimeGap);
		}
	}

	void DecreaseNotification()
	{
		currentNotification--;
	}

	public  Sprite ReturnSprite(bool gender)
	{
//		if (gender) {
//			if (degree > 0)
//				return whiteWitchMale;
//			else if (degree < 0)
//				return shadowWitchMale;
//			else
//				return greyWitchMale;
//		} else {
//			if (degree > 0)
//				return whiteWitchFemale;
//			else if (degree < 0)
//				return shadowWitchFemale;
//			else
//				return greyWitchFemale;
//		}

		return(gender ? male : female);
	}
}

