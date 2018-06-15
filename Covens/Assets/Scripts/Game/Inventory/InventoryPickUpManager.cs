using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPickUpManager : MonoBehaviour {
	public static InventoryPickUpManager Instance {get; set;}
	public GameObject CollectibleObject;
	public Text displayName;
	public Text disc;
	public GameObject BaseSprite;
	public GameObject Loading;
	GameObject loadingObject;
	public Sprite herb;
	public Sprite tool;
	public Sprite gem;
	public Animator anim;
	public GameObject GlowItem;
	public GameObject collecting;
	void Awake()
	{
		Instance = this;
	}
	void Start()
	{
		EventManager.OnInventoryDataReceived += OnDataReceived;
	}

	public void PickUp()
	{
		collecting.SetActive (false);
		print (MarkerSpawner.SelectedMarker3DT.name);
		loadingObject = Utilities.InstantiateObject (Loading, MarkerSpawner.SelectedMarker3DT,2.17f);
	}

	Sprite SetSprite() 
	{
		if (MarkerSpawner.SelectedMarker.type == "gem")
			return gem;
		else if (MarkerSpawner.SelectedMarker.type == "tool")
			return tool;
		else
			return herb;
	}

	void OnDataReceived()
	{
		Destroy (loadingObject);
		BaseSprite.SetActive (true);
		BaseSprite.GetComponent<Image>().sprite =SetSprite() ;
		disc.text = MarkerSpawner.SelectedMarker.description;
		displayName.text = MarkerSpawner.SelectedMarker.id;
		CollectibleObject.SetActive (true);
	}

	public void Collect()
	{
		PickUpCollectibleAPI.pickUp (MarkerSpawner.instanceID);
		anim.SetTrigger ("collect");
		collecting.SetActive (true);
	}

	public void OnCollectSuccess(MarkerDataDetail data)
	{
		MarkerManager.DeleteMarker (MarkerSpawner.instanceID);
		collecting.SetActive (false);
		GlowItem.SetActive (true);
		string msg = "Added " + data.count.ToString() + " " + data.id + " to the inventory. You gain " + data.xp.ToString() + " XP.";
		PlayerNotificationManager.Instance.showNotification (null,false,msg, SetSprite());
		CollectibleObject.SetActive (false);
	}

}
