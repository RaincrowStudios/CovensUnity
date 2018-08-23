using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowSelectionCard : MonoBehaviour
{
	public static ShowSelectionCard Instance { get; set;}
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

	[Header("PortalCard")]
	public GameObject PortalCard;
	public GameObject[] portalType;
	public Text portalLevel;
	public Text creator;
	public Text portalTitle;
	public Text portalEnergy;
	public Text summonsIn;

	public Text selfEnergy;
	private Animator anim;

	public Text[] castButtons;

	bool isCardShown  = false;

	void Awake ()
	{
		Instance = this;
	}

	public void ChangeSelfEnergy( ){
		selfEnergy.text ="Energy : " +PlayerDataManager.playerData.energy.ToString();
	}

	public void ShowCard(MarkerSpawner.MarkerType Type)
	{
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
			anim = WitchCard.GetComponent<Animator> ();
			displayName.text = data.displayName;
			level.text = data.level.ToString ();
			SpellCarouselManager.targetType = "witch";
			degree.text = Utilities.GetDegree (data.degree);
			school.text = Utilities.GetSchool (data.degree);
			energy.text = "Energy : " + data.energy.ToString ();
			if(MarkerSpawner.ImmunityMap.ContainsKey(MarkerSpawner.instanceID)){
				if (MarkerSpawner.ImmunityMap [MarkerSpawner.instanceID].Contains (PlayerDataManager.playerData.instance)) {
					SetCardImmunity (true);
				} else {
					SetCardImmunity (false);
				}
			}
		} else {
			SpellCarouselManager.targetType = "none";
		}
		anim.SetTrigger ("in");
	}

	public void SetCardImmunity(bool isImmune)
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
		MapSelection.Instance.OnSelect ();
	}

	void disableObject()
	{
		isCardShown = false;
		WitchCard.SetActive (false);
		PortalCard.SetActive (false);
		SpiritCard.SetActive (false);
	}

}

