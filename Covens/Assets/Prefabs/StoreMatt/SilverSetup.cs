using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SilverSetup : MonoBehaviour {

	public StoreApiItem silverData;
	public Image itemIcon;
	public TextMeshProUGUI itemName;
	public TextMeshProUGUI amount;
	public TextMeshProUGUI costCurrency;
	public TextMeshProUGUI bonus;

	public LeanTweenType easy;
	public float inTime;

	public void SetupSilverInstance(StoreApiItem data)
	{
		silverData = data;
		DownloadedAssets.GetSprite (data.id, itemIcon, true);
		itemName.text = DownloadedAssets.storeDict [data.id].title;
		costCurrency.text = "$" + data.cost.ToString();
		amount.text = data.amount.ToString ();
		if (data.bonus == "") {
			this.transform.GetChild (0).GetChild (0).gameObject.SetActive (false);
		}
		bonus.text = data.bonus;

		this.transform.GetChild (0).GetChild (5).gameObject.GetComponent<Button>().onClick.AddListener(() => {
			//will have to put function here for google play store
			//NewStoreUIManager.Instance.InitiateSilverPurchase(this.silverData, this.itemIcon.sprite);
			if (data.id.Contains("1")) {
				NewStoreUIManager.Instance.buySilverID = 1;
			} else if (data.id.Contains("2")) {
				NewStoreUIManager.Instance.buySilverID = 2;
			} else if (data.id.Contains("3")) {
				NewStoreUIManager.Instance.buySilverID = 3;
			} else if (data.id.Contains("4")) {
				NewStoreUIManager.Instance.buySilverID = 4;
			} else if (data.id.Contains("5")) {
				NewStoreUIManager.Instance.buySilverID = 5;
			} else {
				NewStoreUIManager.Instance.buySilverID = 6;
			}
			IAPSilver.instance.BuyProductID(data.id);

		});

		this.gameObject.transform.localScale = Vector3.zero;
		LeanTween.scale (this.gameObject, Vector3.one, inTime).setEase (easy).setOnComplete(() => {print("done");});
	}

	//insert buying logic here... will have to add a listener in awake most likely
}
