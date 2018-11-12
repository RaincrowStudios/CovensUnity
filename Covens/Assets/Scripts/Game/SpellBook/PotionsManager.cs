using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionsManager : UIAnimationManager {

	public Text OnConsumeMsg;
	public GameObject[] HideObjects;
	public GameObject PotionsObject;
	public Transform container;
	public GameObject potionsItem;


	public void ShowPotions()
	{
		foreach (var item in HideObjects) {
			item.SetActive (false);
		}
		foreach (Transform item in container) {
			Destroy (item.gameObject);
		}
		PotionsObject.SetActive (true);
		foreach (var item in PlayerDataManager.playerData.inventory.consumables) {
			var g = Utilities.InstantiateObject (potionsItem, container);
			g.GetComponent<PotionsData> ().Setup (item,this);
		}
	}

	public void HidePotions()
	{
		foreach (var item in HideObjects) {
			item.SetActive (true);
		}
		PotionsObject.SetActive (false);
	}

	public void OnConsumeSuccess(string desc)
	{
		OnConsumeMsg.gameObject.SetActive (false);
		this.CancelInvoke ();
		Show (OnConsumeMsg.gameObject);
		OnConsumeMsg.text = desc;
		Invoke("HideConsumeMsg",6); 
		PlayerManagerUI.Instance.UpdateElixirCount ();
		SoundManagerOneShot.Instance.PlayReward ();
	}
	void HideConsumeMsg()
	{
		Hide (OnConsumeMsg.gameObject);
	}
}
