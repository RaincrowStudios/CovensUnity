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

	Sprite SetSprite() 
	{
		if (MarkerSpawner.SelectedMarker.type == "gem")
			return gem;
		else if (MarkerSpawner.SelectedMarker.type == "tool")
			return tool;
		else
			return herb;
	}

	public void OnDataReceived()
	{
		Destroy (loadingObject);
		BaseSprite.SetActive (true);
		BaseSprite.GetComponent<Image>().sprite =SetSprite() ;
		disc.text = DownloadedAssets.ingredientDictData [MarkerSpawner.SelectedMarker.id].description; 
		displayName.text = DownloadedAssets.ingredientDictData [MarkerSpawner.SelectedMarker.id].name;
		CollectibleObject.SetActive (true);
		CollectibleObject.transform.GetChild(0).gameObject.SetActive (true);
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
		string msg = "Added " + data.count.ToString() + " " + DownloadedAssets.ingredientDictData[data.id].name + " to the inventory";
		PlayerNotificationManager.Instance.showNotification (msg, SetSprite());
		CollectibleObject.SetActive (false);
	}

	public void OnCollectFail( )
	{
		collecting.SetActive (false);
		string msg = "Failed to collect the item.";
		PlayerNotificationManager.Instance.showNotification (msg, SetSprite());
	}

}
