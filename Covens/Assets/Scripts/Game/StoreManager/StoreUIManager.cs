using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;


public class StoreUIManager : UIAnimationManager
{

	public static StoreUIManager Instance { get; set; }

	public GameObject loadingButton;
	public Animator storeAnim;

	public GameObject wheelContainer;
	public GameObject silverContainer;
	public GameObject elixirContainer;
	public GameObject gearContainer;
	public GearUIManager gearUIM;
	StoreApiObject storeItems;

	public StoreButtonData[] Energy;
	public StoreButtonData[] xp;
	public StoreButtonData[] align;
	public StoreButtonData[] bundle;
	public StoreButtonData[] Silver;

	//silver PurchaseA
	public GameObject selectSilver;
	public Text selectSilverTitle;
	public Text selectSilverDesc;
	public Image selectSilverImg;
	public Text selectSilverAmount;
	public GameObject selectBuy;
	public GameObject selectBuySilver;
	public GameObject SelectNoSilver;
	public Text silverDrachs;

	public GameObject purchaseSuccess;
	public Text purchaseSuccessTitle;
	public Text purchaseAmount;
	public Image purchaseSuccessDisplayImage;

	public static StoreApiItem SelectedStoreItem;

	void Awake ()
	{
		Instance = this;
	}

	public void GetStore ()
	{
		StoreManagerAPI.GetShopItems (Callback);
		loadingButton.SetActive (true);
	}

	public void Callback (string result, int code)
	{
		if (code == 200) {
			storeItems = JsonConvert.DeserializeObject<StoreApiObject> (result); 
			HandleResult ();
			InitStore ();
		} else {
			
			Debug.LogError (code + " | Couldnt Get Store Data : " + result);
		}
		loadingButton.SetActive (false);

	}

	void HandleResult ()
	{
		foreach (var item in storeItems.silver) {
			item.type = "silver"; 
			Silver [(int.Parse ((item.id [6]).ToString ())) - 1].Setup (item); 
		}

		foreach (var item in storeItems.consumables) {
			if (item.id.Contains ("energy")) {
				item.type = "energy";
				if (item.id.Contains ("1")) {
					Energy [0].Setup (item);
				} else if (item.id.Contains ("3")) {
					Energy [1].Setup (item);
				} else if (item.id.Contains ("5")) {
					Energy [2].Setup (item);
				}
			} else if (item.id.Contains ("xpBooster")) {
				item.type = "xp";
				if (item.id.Contains ("Smaller")) {
					xp [0].Setup (item);
				} else if (item.id.Contains ("Medium")) {
					xp [1].Setup (item);
				} else if (item.id.Contains ("Greater")) {
					xp [2].Setup (item);
				}
			} else if (item.id.Contains ("alignmentBooster")) {
				item.type = "align";
				if (item.id.Contains ("Smaller")) {
					align [0].Setup (item);
				} else if (item.id.Contains ("Medium")) {
					align [1].Setup (item);
				} else if (item.id.Contains ("Greater")) {
					align [2].Setup (item);
				}
			} else if (item.type == "cosmetics") {

			}
		}

		foreach (var item in storeItems.bundles) {
			item.type = "bundle";
			if (item.id == "bundle_abondiasBest") {
				bundle [0].Setup (item);
			}
			if (item.id == "bundle_sapphosChoice") {
				bundle [1].Setup (item);
			}
			if (item.id == "bundle_hermeticCollection") {
				bundle [2].Setup (item);
			}
		}
	}

	void InitStore ()
	{
		this.CancelInvoke ();
		storeAnim.gameObject.SetActive (true);
		storeAnim.Play ("enter");
		Show (wheelContainer);
	}

	public void Exit ()
	{
		storeAnim.Play ("exit");
		Invoke ("DisableDelay", .9f);
	}

	void DisableDelay ()
	{
		storeAnim.gameObject.SetActive (false);
	}

	public void ShowSilver (bool isShow)
	{
		if (isShow) {
			Hide (wheelContainer);
			Show (silverContainer);
		} else {
			Show (wheelContainer);
			Hide (silverContainer);
		}
	}

	public void ShowGear (bool isShow)
	{
		if (isShow) {
			Hide (wheelContainer);
			Show (gearContainer);
			gearUIM.Init (storeItems.cosmetics);
		} else {
			Show (wheelContainer);
			Hide (gearContainer);
		}
	}

	public void ShowElixir (bool isShow)
	{
		if (isShow) {
			Hide (wheelContainer);
			Show (elixirContainer);
		} else {
			Show (wheelContainer);
			Hide (elixirContainer);
		}
	}

	public void InitiatePurchase (StoreApiItem data, Sprite sp)
	{
		Show (selectSilver);  
		if (PlayerDataManager.playerData.silver > data.silver) {
			selectBuy.SetActive (true);
			selectBuySilver.SetActive (false);
			SelectNoSilver.SetActive (false);
			silverDrachs.color = Color.white;
			selectSilverTitle.text = "Buy <color=ffffff>" + DownloadedAssets.storeDict [data.id].title + "</color>";
			selectSilverDesc.text = DownloadedAssets.storeDict [data.id].onBuyDescription;
			selectSilverImg.sprite = sp;
			selectSilverAmount.text = data.silver.ToString ();
		} else {
			selectBuy.SetActive (false);
			selectBuySilver.SetActive (true);
			SelectNoSilver.SetActive (true);
			silverDrachs.color = Color.red;
		}
	}

	public void CancelSilverPurchase ()
	{
		Hide (selectSilver); 
		silverDrachs.color = Color.white;
		SelectNoSilver.SetActive (false);
	}

	public void buyWithGold ()
	{
		ConfirmPurchase (false);
	}

	public void buyWithSilver ()
	{
		ConfirmPurchase ();
	}

	void ConfirmPurchase (bool isSilver = true)
	{
		var data = new{purchaseItem = SelectedStoreItem.id,currency = (isSilver ? "silver" : "gold")};  
		APIManager.Instance.PostData ("shop/purchase", JsonConvert.SerializeObject (data), PurchaseCallback); 
	}

	public void PurchaseCallback (string result, int code)
	{
		if (code == 200) {
			PuchaseSuccess (); 
		} else {
			Debug.LogError ("Something Went Wrong in Purchase : " + result);
		}
	}

	public void PuchaseSuccess (bool isCosmetic = false, ApparelData apData = null)
	{
		print ("purchase Success!");
		Hide (selectSilver);
		purchaseSuccess.SetActive (true);
		if (!isCosmetic) {
			if (SelectedStoreItem.type == "silver") {
				purchaseSuccessTitle.text = SelectedStoreItem.amount.ToString () + " SILVER DRACHS";
				purchaseAmount.text = "";
				StartCoroutine (Countup (PlayerDataManager.playerData.silver, PlayerDataManager.playerData.silver + SelectedStoreItem.amount));
				PlayerDataManager.playerData.silver += SelectedStoreItem.amount;
			}
			if (SelectedStoreItem.type == "energy") {
				purchaseSuccessTitle.text = DownloadedAssets.storeDict [SelectedStoreItem.id].title;
				purchaseAmount.text = DownloadedAssets.storeDict [SelectedStoreItem.id].subtitle;
				StartCoroutine (Countup (PlayerDataManager.playerData.silver, PlayerDataManager.playerData.silver - SelectedStoreItem.silver));
				PlayerDataManager.playerData.silver -= SelectedStoreItem.amount;
				foreach (var item in PlayerDataManager.playerData.inventory.consumables) {
					if (item.id == SelectedStoreItem.contents[0].id) {
						item.count+=SelectedStoreItem.contents[0].count ;
						return;
					}
				}
				ConsumableItem ci = new ConsumableItem ();
				ci.count = SelectedStoreItem.contents[0].count;
				ci.id = SelectedStoreItem.contents[0].id;
				PlayerDataManager.playerData.inventory.consumables.Add(ci);
			}
			if (SelectedStoreItem.type == "bundle") {
				purchaseSuccessTitle.text = DownloadedAssets.storeDict [SelectedStoreItem.id].title;
				purchaseAmount.text = DownloadedAssets.storeDict [SelectedStoreItem.id].subtitle;
				StartCoroutine (Countup (PlayerDataManager.playerData.silver, PlayerDataManager.playerData.silver - SelectedStoreItem.silver));
				PlayerDataManager.playerData.silver -= SelectedStoreItem.amount;
				foreach (var item in SelectedStoreItem.contents) {
					var type = DownloadedAssets.ingredientDictData [item.id].type;
					if (type == "herb") {
						PlayerDataManager.playerData.ingredients.herbsDict [item.id].count += item.count;
					}else if(type == "gem")
						PlayerDataManager.playerData.ingredients.gemsDict [item.id].count += item.count;
					else
						PlayerDataManager.playerData.ingredients.toolsDict [item.id].count += item.count;


				}
			}
			if (SelectedStoreItem.type == "xp" || SelectedStoreItem.type == "align") {
				purchaseSuccessTitle.text = DownloadedAssets.storeDict [SelectedStoreItem.id].title;
				purchaseAmount.text = DownloadedAssets.storeDict [SelectedStoreItem.id].subtitle;
				StartCoroutine (Countup (PlayerDataManager.playerData.silver, PlayerDataManager.playerData.silver - SelectedStoreItem.silver));
				PlayerDataManager.playerData.silver -= SelectedStoreItem.amount;
			

				purchaseSuccessDisplayImage.sprite = SelectedStoreItem.pic; 

				foreach (var item in PlayerDataManager.playerData.inventory.consumables) {
					if (item.id == SelectedStoreItem.id) {
						item.count++;
						return;
					}
				}
				ConsumableItem ci = new ConsumableItem ();
				ci.count = 1;
				ci.id = SelectedStoreItem.id;
				PlayerDataManager.playerData.inventory.consumables.Add(ci);
			}
			purchaseSuccessDisplayImage.sprite = SelectedStoreItem.pic; 

		} else {
			gearUIM.curButton.buttonText.text = "OWNED";
			gearUIM.curButton.button.interactable = false;
			gearUIM.curButton.button.image.sprite = 	gearUIM.curButton.unlockSprite;
			purchaseSuccessTitle.text = apData.id;
			purchaseSuccessDisplayImage.sprite = DownloadedAssets.wardobePreviewArt [apData.iconId];
		}
	}

	public void OpenSilverStore ()
	{
		Hide (elixirContainer);
		Hide (gearContainer);
		Hide (selectSilver);
		Show (silverContainer);
	}

	IEnumerator Countup (int before, int after)
	{
		float t = 0;
		while (t < 1) {
			t += Time.deltaTime;
			silverDrachs.text = ((int)Mathf.Lerp (before, after, t)).ToString (); 
			yield return null;
		}
	}
}


