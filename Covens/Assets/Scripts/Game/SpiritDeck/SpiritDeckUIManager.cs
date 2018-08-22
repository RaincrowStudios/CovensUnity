using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Newtonsoft.Json;
public class SpiritDeckUIManager : UIAnimationManager {
	
	public GameObject spiritCard;
	public GameObject emptyCard;
	public GameObject undiscoveredCard;
	public GameObject portalWhite;
	public GameObject portalShadow;
	public GameObject portalGrey;
	public Transform container;
	public GameObject DeckObject;
	public GameObject[] buttonFX;
	public Text[] buttons;
	public GameObject[] descriptions;

	public ColliderScrollTrigger CS;

	[Header("Descriptions")]
	public Text title;
	public Text date;
	public Text hint;
	public GameObject[] flyPortalFlare;
	public Text lastAttackedByPortal;
	public Text addedIngredientsPortal;
	public GameObject[] flySpiritFlare;
	public Text xpGained;
	public Text expiresIn;
	public Text collectedIngredients;
	public Text noActiveItems;
	public GameObject Buttonleft;
	public GameObject ButtonRight;
	type currentType = type.known; 
	public enum type
	{
		active,known,portal
	}
	List<SpiritData> currentList = new List<SpiritData>();

	void Start () {
		CS.EnterAction = Enter;
		CS.ExitAction = Exit;
	}

	public void TurnOn()
	{
		Show (DeckObject);
		OnClick (currentType.ToString ());
	}

	public void TurnOff()
	{
		Hide (DeckObject);
		Invoke ("KillExtra", 1f);
	}

	void KillExtra()
	{
		foreach (Transform item in container) {
			Destroy (item.gameObject);
		}
	}

	void SetupUI()
	{
		this.CancelInvoke ();
		CS.GetComponent<BoxCollider>().enabled= false;
		foreach (Transform item in container) {
			Destroy (item.gameObject);
		}
		var sd = new SpiritData ();
		sd.instance = "empty";
		SpawnCard (sd, emptyCard);
		SpawnCard (sd, emptyCard);

		if (currentType == type.known) {
			noActiveItems.gameObject.SetActive (false);

			if (currentList.Count < 5) {
				int uCard = 5 - currentList.Count;
				foreach (var item in currentList) {
					SpawnCard (item, spiritCard);
				}
				for (int i = 0; i < uCard; i++) {
					var s = new SpiritData ();
					s.instance = "null";
					SpawnCard (s, undiscoveredCard);
				}
				Hide (Buttonleft);
				Hide (ButtonRight);
			} else {
				foreach (var item in currentList) {
					SpawnCard (item, spiritCard);
				}
				Show (Buttonleft);
				Show (ButtonRight);
			}
		} else if (currentType == type.active) {
			if (currentList.Count == 0) {
				Show (noActiveItems.gameObject);
				noActiveItems.text = "You do not have any spirits active at this moment.";
				Hide (Buttonleft);
				Hide (ButtonRight);
			} else {
				Hide (noActiveItems.gameObject);
				Show (Buttonleft);
				Show (ButtonRight);
			}
			foreach (var item in currentList) {
				SpawnCard (item, spiritCard);
			}
		} else {
			if (currentList.Count == 0) {
				Show (noActiveItems.gameObject);
				noActiveItems.text = "You do not have any portals active at this moment.";
				Hide (Buttonleft);
				Hide (ButtonRight);
			} else {
				Hide (noActiveItems.gameObject);
				Show (Buttonleft);
				Show (ButtonRight);
			}
			foreach (var item in currentList) {
				if (item.degree > 0) {
					SpawnCard (item, portalWhite);
				} else if (item.degree == 0) {
					SpawnCard (item, portalGrey);
				} else {
					SpawnCard (item, portalShadow);
				}
			}
		}

		SpawnCard (sd, emptyCard);
		SpawnCard (sd, emptyCard);
		Invoke ("colenable", .2f);
	}

	void colenable()
	{
		CS.GetComponent<BoxCollider>().enabled = true;

	}

	void SpawnCard (SpiritData sd,GameObject card )
	{
		var g = Utilities.InstantiateObject (card, container);
		g.GetComponent<SetupDeckCard> ().SetupCard (sd, currentType);
	}

	public void OnClick(string t)
	{
		if (t == "known") {
			currentType = type.known;
			buttonFX [0].SetActive (true);
			buttonFX [1].SetActive (false);
			buttonFX [2].SetActive (false);
			buttons [0].color = Color.white;
			buttons [1].color = new Color (1, 1, 1, .35f);
			buttons [2].color = new Color (1, 1, 1, .35f);
		} else if (t == "active") {
			currentType = type.active;
			buttonFX [1].SetActive (true);
			buttonFX [0].SetActive (false);
			buttonFX [2].SetActive (false);
			buttons [1].color = Color.white;
			buttons [0].color = new Color (1, 1, 1, .35f);
			buttons [2].color = new Color (1, 1, 1, .35f);
		} else {
			currentType = type.portal;
			buttonFX [2].SetActive (true);
			buttonFX [1].SetActive (false);
			buttonFX [0].SetActive (false);
			buttons [2].color = Color.white;
			buttons [1].color = new Color (1, 1, 1, .35f);
			buttons [0].color = new Color (1, 1, 1, .35f);
		}
		Get ();
	}

	void Get()
	{
		if (currentType == type.known) 
			APIManager.Instance.PostData ("/character/spirits/known", "null", ReceiveData, true);
		else if(currentType == type.active) 
			APIManager.Instance.PostData ("/character/spirits/active", "null", ReceiveData, true);
		else
			APIManager.Instance.PostData ("/character/portals/active", "null", ReceiveData, true);

	}

	public void ReceiveData(string response, int code)
	{
		if (code == 200) {
			print (response);
			currentList = JsonConvert.DeserializeObject<List<SpiritData>>(response);
			SetupUI ();
		}
	}

	void Enter(Transform t){
		var data = t.GetComponent<SetupDeckCard> ().sd;
		TurnOffDesc ();
		if (currentType == type.known) {
			t.GetComponent<Animator> ().SetBool ("animate", true);
			if(data.instance!="empty" && data.instance!="null"){
				descriptions [0].SetActive (true);
				title.text = "You have gained power over the " + DownloadedAssets.spiritDictData [data.id].spiritName; 
				date.text = "Discovered on " + GetTimeStamp(data.banishedOn) + ", in " + data.location +"."; 
			}
		}else if (currentType == type.active) {
			if(data.instance != "empty"){
				t.GetComponent<Animator> ().SetBool ("animate", true);
				if (data.degree > 0) {
					flySpiritFlare [0].SetActive (true);
				} else if (data.degree == 0) {
					flySpiritFlare [1].SetActive (true);
				} else {
					flySpiritFlare [2].SetActive (true);
				}
				descriptions [1].SetActive (true);
				xpGained.text = "XP Gained : " + data.xpGained.ToString();
				expiresIn.text = "Expires in : " + Utilities.GetSummonTime(data.expireOn);
				collectedIngredients.text = "Ingredients Collected : None"; 
			}
		}else  {
			if(data.instance != "empty"){
				t.GetComponent<Animator> ().SetBool ("animate", true);
				t.GetChild (0).gameObject.SetActive (true);
				t.GetChild (1).gameObject.SetActive (false);
				if (data.degree > 0) {
					flyPortalFlare [0].SetActive (true);
				} else if (data.degree == 0) {
					flyPortalFlare [1].SetActive (true);
				} else {
					flyPortalFlare [2].SetActive (true);
				}
				descriptions [2].SetActive (true);
				lastAttackedByPortal.text = "Last Attacked By : None";
				addedIngredientsPortal.text = "Ingredients Added : None"; 
			}
		}
	}

	void TurnOffDesc()
	{
		foreach (var item in descriptions) {
			item.SetActive (false);
		}
		foreach (var item in flyPortalFlare) {
			item.SetActive (false);
		}
		foreach (var item in flySpiritFlare) {
			item.SetActive (false);
		}
	}

	void Exit(Transform t){
		if(t.GetComponent<Animator>()!=null)
			t.GetComponent<Animator> ().SetBool ("animate", false);
		if (currentType == type.portal) {
			try{
				t.GetChild (0).gameObject.SetActive (false);
				t.GetChild (1).gameObject.SetActive (true);
			}catch{
			
			}
		}
		TurnOffDesc ();
	}

	string GetTimeStamp(double javaTimeStamp)
	{
		if (javaTimeStamp < 159348924)
		{
			string s = "unknown";
			return s;
		}
		System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local); 
		dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime(); 

		return dtDateTime.ToString("D");
	}
}
