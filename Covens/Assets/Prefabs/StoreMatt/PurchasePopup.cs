using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchasePopup : MonoBehaviour {

	public StoreApiItem itemData;
	public TextMeshProUGUI title;
	public TextMeshProUGUI description;
	public TextMeshProUGUI cost;
	public Image icon;

	void Start()
	{

//
//		transform.GetChild(9).GetComponent<Button>().onClick.AddListener(() =>{
//			//NewStoreUIManager.Instance.CancelPurchase(transform.gameObject);
//			print("clicked close");
//		});
//
	}

	public void Setup(StoreApiItem data, Sprite sp)
	{
		Vector3 spScale = new Vector3(1.25f, 1.25f, 1.25f);
		if (!data.id.Contains("energy") && data.type != "bundles") {
			spScale = new Vector3 (2, 2, 2);
		}
		itemData = data;
		title.text = "Buy <color=#ffffff> " +  DownloadedAssets.storeDict[data.id].title + "</color>";
		description.text = DownloadedAssets.storeDict [data.id].onBuyDescription;
		cost.text = data.silver.ToString();
		icon.sprite = sp;
		icon.transform.localScale = spScale;
		transform.GetChild (8).GetComponent<Button> ().onClick.AddListener (() => {
			if (PlayerDataManager.playerData.silver > itemData.silver) {
				NewStoreUIManager.Instance.ConfirmPurchase (data.id, true);
			} else {
				transform.GetChild (8).GetComponent<TextMeshProUGUI>().color = Color.red;
			}
			//put else statement here about buying more silver / not having enough or something
		});
		transform.GetChild (9).GetComponent<Button> ().onClick.AddListener (() => {
			NewStoreUIManager.Instance.CancelPurchase();
		});
	}

	public StoreApiItem PassDataForPurchaseSuccess()
	{
		return itemData;
	}
}
