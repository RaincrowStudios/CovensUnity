using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnTapNPCUI : MonoBehaviour {

	public Text energy;
	public Text displayName;
	public Text description;
	public Text weakness;
	public Text summonerName;
	public Text alignment;
	public Text coven;

	public Text SummonBy;
	public GameObject Weakness;
	public GameObject SummonIn;
	public Text summonInTime;

	public static OnTapNPCUI Instance {get; set;}

	void Awake(){
		Instance = this;
	}

	public void ShowInfo(MarkerDataDetail MD)
	{
		displayName.text = MD.displayName;

		if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.lesserPortal) {
			displayName.text = "Lesser Portal";
			SummonBy.text = "Created by";
			Weakness.SetActive (false);
			SummonIn.SetActive (true);
			summonInTime.text = Utilities.EpocToDateTime (MD.summonOn);
		} else if (MarkerSpawner.selectedType == MarkerSpawner.MarkerType.greaterPortal) {
			displayName.text = "Greater Portal";
			SummonBy.text = "Created by";
			Weakness.SetActive (false);
			SummonIn.SetActive (true);
			summonInTime.text = Utilities.EpocToDateTime (MD.summonOn);
		} else {
			SummonBy.text = "Summon by";
		}
		energy.text = MD.energy.ToString() + " Energy.";
		description.text = MD.description;
		weakness.text = "something";
		summonerName.text = MD.owner;
		alignment.text = Utilities.witchTypeControlSmallCaps (MD.degree);
		coven.text = MD.ownerCoven;
	}
	
}
