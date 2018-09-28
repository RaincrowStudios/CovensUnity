using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; 

public class SpiritDeckUIManager : UIAnimationManager {
	public static SpiritDeckUIManager Instance{ get; set; }
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
	public DeckScroller DS;
	type currentType = type.known; 
	public Transform previousTransform;
	public SpiritData selectedcard;
	public GameObject loading;
	public enum type
	{
		active,known,portal
	}
	List<SpiritData> currentList = new List<SpiritData>();

	void Awake()
	{
		Instance = this;
	}

	void Start () {
		
	}

	public void TurnOn()
	{
		Show (DeckObject);
		OnClick (currentType.ToString ());
	}

	public void TurnOff()
	{
		Hide (DeckObject);
	}

	void SetupUI()
	{
		this.CancelInvoke ();
		if (currentType == type.known ) {
			Hide (noActiveItems.gameObject);
			Show (Buttonleft);
			Show (ButtonRight);
		} 
		else if (currentType == type.active) {
			if (currentList.Count == 0) {
				Show (noActiveItems.gameObject);
				noActiveItems.text = "You do not have any spirits active at this moment.";
				Hide (Buttonleft);
				Hide (ButtonRight);
			} else {
				Hide (noActiveItems.gameObject);
				Show (Buttonleft);
				Show (ButtonRight);
//				DS.data = currentList;
			}

		} 
		else {
			if (currentList.Count == 0) {
				Show (noActiveItems.gameObject);
				noActiveItems.text = "You do not have any portals active at this moment.";
				Hide (Buttonleft);
				Hide (ButtonRight);
			} else {
				Hide (noActiveItems.gameObject);
				Show (Buttonleft);
				Show (ButtonRight);
//				DS.data = currentList;
			}
	
		}
		DS.data = currentList;
		DS.InitScroll ();
		Invoke ("colenable", .2f);
	}

	void colenable()
	{

	}
		
	public void OnClick(string t)
	{
		DisablePrevious (previousTransform);
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
		TurnOffDesc ();
		Get ();
	}

	void Get()
	{
		if (currentType == type.known) 
			APIManager.Instance.GetData ("/character/spirits/known",  ReceiveData );
		else if(currentType == type.active) 
			APIManager.Instance.GetData ("/character/spirits/active", ReceiveData);
		else
			APIManager.Instance.GetData ("/character/portals/active", ReceiveData);

	}

	public void ReceiveData(string response, int code)
	{
		if (code == 200) {
			print (response);
			currentList = JsonConvert.DeserializeObject<List<SpiritData>> (response);
			foreach (var item in currentList) {
				item.deckCardType = currentType; 
			}
//			if (currentType == type.known) {
//
//		
//			}
//			foreach (var item in currentList) {
//				print (item.id);
//			}
			SetupUI ();
		}
	}

	public void Enter(Transform t){
		var data = t.GetComponent<SetupDeckCard> ().sd;
		TurnOffDesc ();
		DisablePrevious (previousTransform);
		if (currentType == type.known) {
			t.GetComponent<Animator> ().Play("glow");
			if(data.instance!="empty" && data.instance!="null"){
				descriptions [0].SetActive (true);
				if( DownloadedAssets.spiritDictData.ContainsKey(data.id))
				title.text = "You have gained power over the " + DownloadedAssets.spiritDictData [data.id].spiritName; 
				date.text = "Discovered on " + GetTimeStamp(data.banishedOn) + ", in " + data.location +"."; 
			}
		}else if (currentType == type.active) {
			if(data.instance != "empty"){
				t.GetComponent<Animator> ().Play("glow");
		
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
				t.GetComponent<Animator> ().Play("glow");

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
		previousTransform = t;
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

	void DisablePrevious(Transform t){
		if (t == null)
			return;
		if(t.GetComponent<Animator>()!=null)
			t.GetComponent<Animator> ().Play("hide");

		if (currentType == type.portal) {
			try{
				if(t.GetChild (0).gameObject.activeInHierarchy)
				t.GetChild (0).gameObject.SetActive (false);
				if(!t.GetChild (0).gameObject.activeInHierarchy)
				t.GetChild (1).gameObject.SetActive (true);
			}catch{
			
			}
		}
//		TurnOffDesc ();
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

	public void FlyToItem()
	{
		if (selectedcard.instance != null) {
			loading.SetActive (true);
			var data = new {target = selectedcard.instance};
			if(selectedcard.deckCardType == type.active)
			APIManager.Instance.PostData ("/character/spirits/location", JsonConvert.SerializeObject (data), FlyResponse);
			if(selectedcard.deckCardType == type.portal)
				APIManager.Instance.PostData ("/character/portals/location", JsonConvert.SerializeObject (data), FlyResponse);
		}
	}

	public void FlyResponse(string result,int response){
		loading.SetActive (false);
		if (response == 200) {
			var data = JObject.Parse (result); 
//			PlayerManager.Instance.Fly ();
			OnlineMaps.instance.SetPosition (double.Parse(data ["longitude"].ToString()), double.Parse(data ["latitude"].ToString()));
//			PlayerManager.inSpiritForm = false;
//			PlayerManager.Instance.Fly ();
			TurnOff ();
		} else {
			Debug.LogError (result);
		}
	}

}
