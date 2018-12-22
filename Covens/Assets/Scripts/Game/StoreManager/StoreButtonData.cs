using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StoreButtonData : MonoBehaviour
{
	public Text title;
	public Text silverDrachs;
	public Text amount;
	public Button buy;
	public Text cost;
	public Text bonus;
	public Text subtitle;
	 StoreApiItem apiData;
	Sprite itemImage;

	public void Setup(StoreApiItem data){
		itemImage = transform.Find ("itemIcon").GetComponent<Image> ().sprite;
		apiData = data;
		apiData.pic = itemImage;
		try{
			
		title.text = DownloadedAssets.storeDict [data.id].title;
		if(data.type == "energy"){

			silverDrachs.text = data.silver.ToString();
			amount.text = data.amount.ToString ();
			subtitle.text = DownloadedAssets.storeDict [data.id].subtitle;
			buy.onClick.AddListener (OnClick);
		}
		if (data.type == "silver") { 
			cost.text = "$ " + data.cost.ToString (); 
			amount.text = data.amount.ToString (); 
			bonus.text = data.bonus; 
			buy.onClick.AddListener (delegate {
					OnClickDrachs(data.id);
			}); 
		}
		if (data.type == "xp" || data.type == "align" || data.type == "bundle") {
			silverDrachs.text = data.silver.ToString();
			subtitle.text = DownloadedAssets.storeDict [data.id].subtitle;
				buy.onClick.AddListener (OnClick);
			
		}
		}catch (System.Exception e){
			print (data.id);
			Debug.LogError (e);
		}
	}

	void OnClick()
	{
		StoreUIManager.SelectedStoreItem = apiData;
		StoreUIManager.Instance.InitiatePurchase (apiData, itemImage);
	}

	void OnClickDrachs(string id)
	{
		StoreUIManager.SelectedStoreItem = apiData;
		print ("CALLING BUY PRODUCT ID");
		IAPSilver.instance.BuyProductID (id);
	}
}

