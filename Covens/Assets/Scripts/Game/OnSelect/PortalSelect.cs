using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PortalSelect : MonoBehaviour
{
	public static PortalSelect Instance { get; set; }
	public GameObject PortalSelectScreen;
	public Animator anim;
	public Button continueButton;
	public Text owner;
	public Text energy;
	public Text title;
	public Text summonsIn;
	public GameObject[] portals;
	void Awake()
	{
		Instance = this;
	}

	public void ShowLoading(int degree)
	{
		continueButton.interactable = false;

		if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.lesserPortal)
			title.text = "LESSER PORTAL";
		else
			title.text = "GREATER PORTAL";

		foreach (var item in portals) {
			item.SetActive (false);
		}

		if (degree == 0)
			portals [0].SetActive (true);
		else if (degree > 0)
				portals [1].SetActive (true);
		else
			portals [2].SetActive (true);

		summonsIn.text = "Summons in : ...";
		energy.text = "Energy : ...";
		owner.text = "...";

		PortalSelectScreen.SetActive (true);
		anim.SetTrigger ("in");
	}

	void OnEnable()
	{
		EventManager.OnPortalDataReceived += DataReceived;
	}

	void OnDisable()
	{
		EventManager.OnPortalDataReceived -= DataReceived;
	}



	void DataReceived()
	{
		continueButton.interactable = true;
		PortalSelectScreen.SetActive (true);
		summonsIn.text = "Summons in : " + Utilities.EpocToDateTime(MarkerSpawner.SelectedMarker.summonOn);
		energy.text = "Energy : " + MarkerSpawner.SelectedMarker.energy.ToString ();
		owner.text =  MarkerSpawner.SelectedMarker.owner;
	}

	public void Continue()
	{
		OnPlayerSelect.Instance.OnClick (MarkerSpawner.SelectedMarkerPos);
		anim.SetTrigger ("out");
		Invoke ("disableObject", 1.2f);
	}

	void disableObject()
	{
		PortalSelectScreen.SetActive (false);
	}
}

