using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientCharmSetup : MonoBehaviour {

	StoreApiItem consumableData;
	public Image itemIcon;
	public TextMeshProUGUI itemName;
	public TextMeshProUGUI cost;
	public TextMeshProUGUI subtitle;
	public TextMeshProUGUI button;

	public LeanTweenType easy;
	public float inTime;

	public void SetupConsumableInstance(StoreApiItem data)
	{
		consumableData = data;
		//DownloadedAssets.GetSprite (data.id, itemIcon, true);
		itemName.text = DownloadedAssets.storeDict [data.id].title;
		cost.text = data.silver.ToString ();

		subtitle.text = "";
		if (data.id.Contains ("Smaller")) {
			subtitle.text = "Smaller";
		} else if (data.id.Contains ("Medium")) {
			subtitle.text = "Medium";
		} else if (data.id.Contains ("Greater")) {
			subtitle.text = "Greater";
		}

		this.gameObject.transform.localScale = Vector3.zero;
		LeanTween.scale (this.gameObject, Vector3.one, inTime).setEase (easy).setOnComplete(() => {print("done");});
		//this.transform.GetChild (0).GetChild (5).GetComponent<Button>().onClick.AddListener(() =>
			//InitiateConsumablePurchase(consumableData, itemIcon.sprite));

		//set button text for localization here
		//subtitle.text


		this.transform.GetChild (0).GetChild (3).gameObject.AddComponent<Button> ().onClick.AddListener (() => {
			NewStoreUIManager.Instance.InitiateSilverPurchase (this.consumableData, this.itemIcon.sprite);
		});


//		this.transform.GetChild (0).GetChild (2).GetComponent<Button> ().onClick.AddListener (() => {
//			print("want to buy");
//		});
	}

}
