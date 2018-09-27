using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json;
public class ShowSelectionCard : UIAnimationManager
{
	public static ShowSelectionCard Instance { get; set;}
	public static bool isLocationCard = false;
	[Header("SpiritCard")]
	public GameObject SpiritCard;
	public Image spiritSprite;
	public Text title;
	public Text legend;
	public Text desc;
	public Text tier;

	[Header("WitchCard")]
	public GameObject WitchCard;
	public Text displayName;
	public Text level;
	public Text degree;
	public Text school;
	public Text energy;
	public GameObject Immune;
	public Text castButton;
	public ApparelView male;
	public ApparelView female;

	[Header("PortalCard")]
	public GameObject PortalCard;
	public GameObject[] portalType;
	public Text portalLevel;
	public Text creator;
	public Text portalTitle;
	public Text portalEnergy;
	public Text summonsIn;

	[Header("Location")]
	public GameObject LocationCard;
	public Text locationTitle;
	public Text locOwnedBy;
	public Text defendedBy;
	public Text timeToTreasure;
	public Button EnterLocation;
	public Text ExitLocation;
	public Text selfEnergy;
	private Animator anim;

	public GameObject InviteToCoven;
	public Button inviteButton;
	public Text InviteText;
	public GameObject inviteLoading;

	public Text[] castButtons;

	bool isCardShown  = false;

	

	void Awake ()
	{
		Instance = this;
	}

	public void ChangeSelfEnergy( ){
		selfEnergy.text ="Energy : " +PlayerDataManager.playerData.energy.ToString();
	}

	public void ChangeEnergy()
	{
		energy.text = "Energy : " +  MarkerSpawner.SelectedMarker.energy.ToString ();
	
	}

	public void ChangeLevel()
	{
		level.text =  MarkerSpawner.SelectedMarker.level.ToString ();
	}

	public void ChangeDegree()
	{
		degree.text = Utilities.GetDegree ( MarkerSpawner.SelectedMarker.degree);
		school.text = Utilities.GetSchool ( MarkerSpawner.SelectedMarker.degree);
	}

	public void ShowCard(MarkerSpawner.MarkerType Type)
	{
		this.CancelInvoke ();
		InviteToCoven.SetActive (false);
		ChangeSelfEnergy ();
		var data = MarkerSpawner.SelectedMarker;

		isCardShown = true;
		if (Type == MarkerSpawner.MarkerType.spirit) {
			SpiritCard.SetActive (true);
			anim = SpiritCard.GetComponent<Animator> ();
			var sData = DownloadedAssets.spiritDictData [data.id];
			title.text = sData.spiritName;
			tier.text = Utilities.ToRoman (sData.spiritTier);
			legend.text = sData.spiritLegend;
			desc.text = sData.spiritDescription;
			if( DownloadedAssets.spiritArt.ContainsKey( data.id))
				spiritSprite.sprite = DownloadedAssets.spiritArt [data.id];
			SpellCarouselManager.targetType = "spirit";

		} else if (Type == MarkerSpawner.MarkerType.portal ) {
			PortalCard.SetActive (true);
			anim = PortalCard.GetComponent<Animator> ();
				portalTitle.text = "Portal";
			foreach (var item in portalType) {
				item.SetActive (false);
			}
			if (data.degree > 0) {
				portalType [0].SetActive (true);
			} else if (data.degree == 0) {
				portalType [1].SetActive (true);
			} else {
				portalType [2].SetActive (false);
			}

			SpellCarouselManager.targetType = "portal";
			creator.text = data.owner;
			portalEnergy.text = "Energy : " + data.energy.ToString ();
			summonsIn.text = "Summon in " + Utilities.EpocToDateTime (data.summonOn);
			portalLevel.text = Utilities.ToRoman (data.level);
			
		} else if (Type == MarkerSpawner.MarkerType.witch) {
			WitchCard.SetActive (true);
			if (MarkerSpawner.SelectedMarker.male) {
				female.gameObject.SetActive (false);
				male.gameObject.SetActive (true);
				male.InitializeChar (MarkerSpawner.SelectedMarker.equipped);
			} else {
				female.gameObject.SetActive (true);
				male.gameObject.SetActive (false);
				female.InitializeChar (MarkerSpawner.SelectedMarker.equipped);
			}
			anim = WitchCard.GetComponent<Animator> ();
			displayName.text = data.displayName;
			level.text = data.level.ToString ();
			SpellCarouselManager.targetType = "witch";
			degree.text = Utilities.GetDegree (data.degree);
			school.text = Utilities.GetSchool (data.degree);
			energy.text = "Energy : " + data.energy.ToString ();

			Invoke ("SetupInviteToCoven", 1f);

			if(MarkerSpawner.ImmunityMap.ContainsKey(MarkerSpawner.instanceID)){
				if (MarkerSpawner.ImmunityMap [MarkerSpawner.instanceID].Contains (PlayerDataManager.playerData.instance)) {
					SetCardImmunity (true);
				} else {
					SetCardImmunity (false);
				}
			}
		}else if (Type == MarkerSpawner.MarkerType.location ) {
			isLocationCard = true;
			LocationCard.SetActive (true); 
			anim = LocationCard.GetComponent<Animator> (); 
			locationTitle.text = MarkerSpawner.SelectedMarker.displayName; 
			SetupLocationCard ();
				
		}  else {
			SpellCarouselManager.targetType = "none";
		}


		anim.SetTrigger ("in");
	}

	void SetupInviteToCoven()
	{
		if (PlayerDataManager.playerData.covenName != "") {
			if (MarkerSpawner.SelectedMarker.covenName != "") {
				StartCoroutine (FadeIn (InviteToCoven, 1));
				InviteText.text = "Invite to Coven";
				inviteLoading.SetActive (false);
				inviteButton.onClick.AddListener (SendInviteRequest);
				InviteText.color = Color.white;
			}
		} else {
			InviteToCoven.SetActive (false);
		}
	}

	public void SendInviteRequest()
	{
		var data = new {invited = MarkerSpawner.instanceID}; 
		inviteLoading.SetActive (true);
		APIManager.Instance.PostData ("coven/invite", JsonConvert.SerializeObject (data),requestResponse); 
	}
	public void requestResponse(string s , int r){
		inviteLoading.SetActive (false);
		if (r == 200) {
			inviteButton.onClick.RemoveListener (SendInviteRequest);
			InviteText.text = "Invitation Sent!";
		} else {
			Debug.Log (s);
			if (s == "4803") {
				InviteText.text = "Invitation Sent!";
			} else {
				InviteText.text = "Invite Failed...";
				InviteText.color = Color.red;
			}
			inviteButton.onClick.RemoveListener (SendInviteRequest);
		}
	}

	public void SetupLocationCard ( )
	{
		print ("Setting Stuff");
		var mData = MarkerSpawner.SelectedMarker;
		print (mData.controlledBy);
		if (mData.controlledBy != "") {
			if (mData.isCoven)
				locOwnedBy.text = "This Place of Power is owned by the coven <color=#ffffff> " + mData.controlledBy + "</color>.";
			else
				locOwnedBy.text = "This Place of Power is owned by <color=#ffffff> " + mData.controlledBy + "</color>.";
			if (mData.spiritCount == 1)
				defendedBy.text = "It is defended by " + mData.spiritCount.ToString () + " spirit.";
			else
				defendedBy.text = "It is defended by " + mData.spiritCount.ToString () + " spirits.";
		}
		else {
			locOwnedBy.text = "This Place of Power is unclaimed.";
			defendedBy.text = "You can own this Place of Power by summoning a spirit inside it.";
		}
		timeToTreasure.text = GetTime (mData.rewardOn) + "until this Place of Power yields treasure.";
		if (mData.full) {
			EnterLocation.GetComponent<Text> ().text = "Place of power is full.";
			ExitLocation.text = "Close";
		}
		else {
			ExitLocation.text = "Not Today";
			EnterLocation.GetComponent<Text> ().text = "Enter the Place of Power";
		}
	}

	public void SetCardImmunity(bool isImmune )
	{
		if (BanishManager.isSilenced)
			return;
		if (isImmune) {
			Immune.SetActive (true);
			castButton.text = "";
			castButton.GetComponent<Button> ().enabled = false;
		} else {
			castButton.gameObject.SetActive (true);
			castButton.text = "Cast";
			castButton.GetComponent<Button> ().enabled = true;
			Immune.SetActive (false);
		}
	}

	public void SetSilenced(bool isTrue)
	{
		if (isTrue) {
			foreach (var item in castButtons) {
				item.text = "You are silenced for " + Utilities.GetSummonTime (BanishManager.silenceTimeStamp);
				item.GetComponent<Button> ().enabled = false;
			}
			StartCoroutine (SilenceUpdateTimer ());
		} else {
			foreach (var item in castButtons) {
				item.text = "Cast";
				item.GetComponent<Button> ().enabled = true;
			}
		}
	}

	IEnumerator SilenceUpdateTimer()
	{
		while (BanishManager.isSilenced) {
			if (isCardShown) {
				foreach (var item in castButtons) {
					item.text = "You are silenced for " + Utilities.GetSummonTime (BanishManager.silenceTimeStamp);
				}
			}
			yield return new WaitForSeconds (1);
		}
	}

	public void Attack()
	{
		anim.SetTrigger ("out");
		Invoke ("disableObject", 1.2f);
		if (MarkerSpawner.selectedType != MarkerSpawner.MarkerType.location)
			MapSelection.Instance.OnSelect ();
		else {
			LocationUIManager.Instance.TryEnterLocation ();
			isLocationCard = false;
		}
	}

	public void close(){
		anim.SetTrigger ("out");
		Invoke ("disableObject", 1.2f);
	}

	void disableObject()
	{
		isCardShown = false;
		WitchCard.SetActive (false);
		PortalCard.SetActive (false);
		SpiritCard.SetActive (false);
		LocationCard.SetActive (false);
	}

	 string GetTime(double javaTimeStamp)
	{
		if (javaTimeStamp < 159348924)
		{
			string s = "unknown";
			return s;
		}

		System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
		dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToUniversalTime();
		var timeSpan = dtDateTime.Subtract(System.DateTime.UtcNow);
		string stamp = "";

		if (timeSpan.Days > 1) {
			stamp = timeSpan.Days.ToString () + " days, ";

		} else if (timeSpan.Days== 1) {
			stamp = timeSpan.Days.ToString () + " day, ";
		}
		if (timeSpan.Hours >1) {
			stamp += timeSpan.Hours.ToString () + " hours ";
		} else if (timeSpan.Hours ==1 ) {
			stamp += timeSpan.Hours.ToString () + " hour ";
		} else {
			if (timeSpan.Minutes > 1) {
				stamp += timeSpan.Minutes.ToString () + " minutes ";
			} else {
				stamp.Remove (4);
			}
		}
		return stamp;
	}

}

