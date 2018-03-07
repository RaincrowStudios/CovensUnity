using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleSelect : MonoBehaviour {
	
	public static CollectibleSelect Instance {get; set;}

	public GameObject CollectibleObject;
	public GameObject collectibleOnTapObject;
	public GameObject collectibleOnCollect;

	public Text displayName;
	public Text disc;
	public static string instanceID; 

	public Text collectInfo;
	public Text xpGained;


	void Awake(){
		Instance = this;
	}

	void Start()
	{
		EventManager.OnInventoryDataReceived += OnDataReceived;
	}

	void OnDataReceived()
	{
		disc.text = MarkerSpawner.SelectedMarker.description;
		displayName.text = MarkerSpawner.SelectedMarker.displayName;
	}

	public void pickUp ( ) {  
		collectibleOnTapObject.SetActive (true);
		collectibleOnCollect.SetActive (false);
		CollectibleObject.SetActive (true);  
		displayName.text = "...";
		disc.text = "...Loading...";
	}

	public void Collect()
	{
		PickUpCollectibleAPI.pickUp (instanceID);
	}

	public void OnCollectSuccess(MarkerDataDetail data)
	{
		if (CollectibleObject.activeInHierarchy) {
			collectibleOnTapObject.SetActive (false);
			collectibleOnCollect.SetActive (true);
			collectInfo.text = data.count.ToString ()+ " " + data.displayName + " added to Inventory.";
			xpGained.text = "+" + data.xp.ToString () + " xp";
		}
	}
	
}
