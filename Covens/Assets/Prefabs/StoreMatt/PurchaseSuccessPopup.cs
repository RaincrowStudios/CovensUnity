using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchaseSuccessPopup : MonoBehaviour {

	public Image itemIcon;
	public TextMeshProUGUI itemTitle;
	public TextMeshProUGUI amountBought;

	void Start()
	{
		transform.GetChild (3).GetChild(9).GetComponent<Button> ().onClick.AddListener (() => {
			NewStoreUIManager.Instance.DestroyPurchaseSuccessScreen();
		});
	}

	public void SetupSuccessScreen(StoreApiItem data, Sprite sp)
	{
		if (data.type == "consumables" && (data.count > 0)) {
			amountBought.text = "x" + data.count;
		}
		itemIcon.sprite = sp;
		//DownloadedAssets.GetSprite(data.id,itemIcon,true);
		itemTitle.text = DownloadedAssets.storeDict [data.id].title;

		//check to make sure that this doesn't overwrite previous accept button listener in start
		transform.GetChild (3).GetChild(9).GetComponent<Button> ().onClick.AddListener (() => {
			NewStoreUIManager.Instance.FadeInText(data);
		});
	}

	public void SetupSuccessScreen(ApparelData data)
	{
		if (data.storeCatagory == "style")
			DownloadedAssets.GetSprite (data.iconId, itemIcon, false);
		else
			DownloadedAssets.GetSprite (data.iconId, itemIcon, true);
		itemTitle.text = DownloadedAssets.storeDict [data.id].title;

		//check to make sure that this doesn't overwrite previous accept button listener in start
		transform.GetChild (3).GetChild(9).GetComponent<Button> ().onClick.AddListener (() => {
			NewStoreUIManager.Instance.FadeInText(data);
		});
	}
}
