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

	public static OnTapNPCUI Instance {get; set;}

	void Awake(){
		Instance = this;
	}

	public void ShowInfo(MarkerDataDetail MD)
	{
		energy.text = MD.energy.ToString() + " Energy.";
		displayName.text = MD.displayName;
		description.text = MD.description;
		weakness.text = "something";
		summonerName.text = MD.owner;
		alignment.text = Utilities.witchTypeControlSmallCaps (MD.degree);
		coven.text = MD.ownerCoven;
	}
	
}
