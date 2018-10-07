using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BanishManager : MonoBehaviour
{
	public static BanishManager Instance { get; set;}
	public GameObject banishObject;
	public Text banishInfoText;
	public static string banishCasterID;

	public GameObject bindObject;
	public Text bindInfoText;
	public GameObject flyButton;
	public GameObject bindLock;
	public Text countDown;

	public GameObject silencedObject;
	public Text silencedInfo;

	public static double bindTimeStamp;
	public static double silenceTimeStamp;

	public static bool isSilenced;
	public static bool isBind;

	bool underBanish;

	public void Awake()
	{
		Instance = this;
	}

	public void FakeBanish()
	{
		Banish (Random.Range (-180, 180), Random.Range (-89, 89));
	}

	public void Banish(double lng, double lat)
	{
		if (MapSelection.currentView == CurrentView.IsoView) {
			SpellCastUIManager.Instance.Exit ();
			StartCoroutine (IsoStateCheckBanish (lng, lat));
			return;
		} else {
			banishObject.SetActive (true);
			banishInfoText.text = "You have been banished by " + banishCasterID;
			StartCoroutine (BanishHelper (lng, lat));
		}
	}

	IEnumerator IsoStateCheckBanish(double lng,double lat)
	{
		yield return new WaitForSeconds (1.2f);
		banishObject.SetActive (true);
		StartCoroutine (BanishHelper (lng, lat));

	}

	IEnumerator BanishHelper(double lng, double lat)
	{
		yield return new WaitForSeconds (2.5f);
		if (PlayerManager.Instance.fly) {
			PlayerManager.Instance.Fly ();
		}
		OnlineMaps.instance.SetPosition (lng, lat);
		OnlineMaps.instance.zoom = 15;
		PlayerManager.Instance.Fly ();
		yield return new WaitForSeconds (2f);
		banishObject.SetActive (false);
	}

	public void BindLogin()
	{
		flyButton.SetActive (false);
		bindLock.SetActive (true);
	}

	public void Bind( WSData data )
	{
		flyButton.SetActive (false);
		bindLock.SetActive (true);
		if(MapSelection.currentView == CurrentView.MapView)
		{
			ShowBindFX (data); 
		} 
		PlayerManager.Instance.CancelFlight ();
	}

	public void ShowBindFX( WSData data )
	{
		bindObject.SetActive (true);
		if (data.type == "witch") {
			bindInfoText.text = "You have been bound by " + data.caster + " for " + Utilities.GetSummonTime (data.expiresOn); 
		} else 	if (data.type == "spirit") {
			bindInfoText.text = "You have been bound by " + DownloadedAssets.spiritDictData[data.caster].spiritName + " for " + Utilities.GetSummonTime (data.expiresOn); 
		}
		this.CancelInvoke ();
		Invoke("DisableBind",3.5f);
	}

	void DisableBind()
	{
		bindObject.SetActive (false);
	}

	public void Unbind()
	{
		flyButton.SetActive (true);
		bindLock.SetActive (false);
		PlayerNotificationManager.Instance.showNotification ("You are no longer bound. You are now able to fly.", PlayerNotificationManager.Instance.spellBookIcon);
	}

	public void Silenced(WSData data )
	{
		if (silencedObject.activeInHierarchy) {
			silencedObject.SetActive (false);
		}
		isSilenced = true;
		ShowSelectionCard.Instance.SetSilenced (true);
		if (data.type == "witch") {
			silencedInfo.text = "You have been silenced by " + data.caster + " for " + Utilities.GetSummonTime (data.expiresOn); 
		} else 	if (data.type == "spirit") {
			silencedInfo.text = "You have been silenced by " + DownloadedAssets.spiritDictData[data.caster].spiritName + " for " + Utilities.GetSummonTime (data.expiresOn); 
		}
		if (MapSelection.currentView == CurrentView.IsoView) {
			SpellCastUIManager.Instance.Exit ();
			StartCoroutine (IsoStateCheckSilenced ());
		} else {
			silencedObject.SetActive (true);
		}
	}

	IEnumerator IsoStateCheckSilenced()
	{
		yield return new WaitForSeconds (1f);
		silencedObject.SetActive (true);
	}

	public void unSilenced()
	{
		print ("Not Silenced");
		isSilenced = false;
		PlayerNotificationManager.Instance.showNotification ("You have been unsilenced. You are now able to cast spells.", PlayerNotificationManager.Instance.spellBookIcon);
		ShowSelectionCard.Instance.SetSilenced (false);
		silencedObject.SetActive (false);

	}

//	public static int getSeconds (double javaTimeStamp)
//	{
//		if (javaTimeStamp < 159348924)
//		{
//			string s = "unknown";
//			return 0;
//		}
//
//		System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
//		dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToUniversalTime();
//		var timeSpan = dtDateTime.Subtract( System.DateTime.UtcNow);
//		string stamp = "";
//	
//				
//		stamp = (Mathf.Abs((int)timeSpan.TotalSeconds)).ToString() + " secs";
//
//		return stamp;
//	}

}

