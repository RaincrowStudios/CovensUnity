using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;


public class StoreUIManager : UIAnimationManager {

	public static StoreUIManager Instance { get; set;}

	public GameObject loadingButton;
	public Animator storeAnim;

	public GameObject wheelContainer;
	public GameObject silverContainer;
	public GameObject elixirContainer;
	public GameObject gearContainer;

	List<StoreApiItem> storeItems = new List<StoreApiItem>();

	public StoreButtonData[] Energy;
	public StoreButtonData[] xp;
	public StoreButtonData[] align;
	public StoreButtonData[] bundle;
	public StoreButtonData[] Silver;

	public GameObject purchaseConfirm;
	public Text purchaseTitle;
	public Text purchaseDesc;
	public Image purchaseDisplayImage;

	public GameObject purchaseSuccess;
	public Text purchaseSuccessTitle;
	public Text purchaseSuccessDesc;
	public Image purchaseSuccessDisplayImage;

	public static StoreApiItem SelectedStoreItem ;

	void Awake()
	{
		Instance = this;
	}

	public void GetStore () {
		StoreManagerAPI.GetShopItems (Callback);
		loadingButton.SetActive (true);
	}

	public void Callback(string result, int code){
		if (code == 200) {
			storeItems = JsonConvert.DeserializeObject<List<StoreApiItem>> (result); 
			HandleResult ();
			InitStore ();
		} else {
			Debug.LogError ("Couldnt Get Store Data : " + result);
		}
		loadingButton.SetActive (false);

	}

	void HandleResult()
	{
		foreach (var item in storeItems) {
			if (item.id.Contains ("silver")) {
				item.type = "silver"; 
				Silver [(int.Parse ((item.id[6]).ToString()))-1].Setup (item); 
			} else if (item.id.Contains ("energy")) {
				if (item.id.Contains ("1")) {
					Energy [0].Setup (item);
				}else if (item.id.Contains ("3")) {
					Energy [1].Setup (item);
				}else if (item.id.Contains ("5")) {
					Energy [2].Setup (item);
				}
			} else if (item.id.Contains ("bundle")) {
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
			} else if (item.id.Contains ("xpBooster")) {
				item.type = "xp";
				if (item.id.Contains ("Smaller")) {
					xp [0].Setup (item);
				}else if (item.id.Contains ("Medium")) {
					xp [1].Setup (item);
				}else if (item.id.Contains ("Greater")) {
					xp [2].Setup (item);
				}
			} else if (item.id.Contains ("alignmentBooster")) {
				item.type = "align";
				if (item.id.Contains ("Smaller")) {
					align [0].Setup (item);
				}else if (item.id.Contains ("Medium")) {
					align [1].Setup (item);
				}else if (item.id.Contains ("Greater")) {
					align [2].Setup (item);
				}
			} else if (item.type == "cosmetics") {
				
			}
		}
	}

	void InitStore()
	{
		this.CancelInvoke ();
		storeAnim.gameObject.SetActive (true);
		storeAnim.Play ("enter");
		Show (wheelContainer);
	}

	public void Exit()
	{
		storeAnim.Play ("exit");
		Invoke ("DisableDelay", .9f);
	}

	void DisableDelay()
	{
		storeAnim.gameObject.SetActive (false);
	}

	public void ShowSilver (bool isShow){
		if (isShow) {
			Hide (wheelContainer);
			Show (silverContainer);
		} else {
			Show (wheelContainer);
			Hide (silverContainer);
		}
	}

	public void ShowGear(bool isShow){
		if (isShow) {
			Hide (wheelContainer);
			Show (gearContainer);
		} else {
			Show (wheelContainer);
			Hide (gearContainer);
		}
	}

	public void ShowElixir(bool isShow){
		if (isShow) {
			Hide (wheelContainer);
			Show (elixirContainer);
		} else {
			Show (wheelContainer);
			Hide (elixirContainer);
		}
	}

	public void InitiatePurchase(StoreApiItem data, Sprite sp)
	{
		Show(purchaseConfirm); 
		string t = DownloadedAssets.storeDict [data.id].onBuyTitle;
		if (t.Contains ("{{Energy}}")) {
			t = t.Replace ("{{Energy}}", data.amount.ToString ());
		}
		if (t.Contains ("{{Amount}}")) {
			t = t.Replace ("{{Amount}}", data.amount.ToString ());
		}
		if (t.Contains ("{{Price}}")) {
			t = t.Replace ("{{Price}}", data.silver.ToString ());
		}
		if (t.Contains ("{{Cost}}")) {
			t = t.Replace ("{{Cost}}", data.cost.ToString ());
		}
		t =  t.ToUpper ();
		purchaseTitle.text = t;
		purchaseDesc.text = DownloadedAssets.storeDict [data.id].onBuyDescription;
		purchaseDisplayImage.sprite = sp;
	}

	public void CancelPurchase(){
		Hide(purchaseConfirm); 
	}


	public void PuchaseSuccess( ){
		Show (purchaseSuccess,false);
		purchaseSuccessTitle.text = "You bought " + SelectedStoreItem.amount.ToString() + " Silver Drachs!";
		purchaseDisplayImage.sprite = SelectedStoreItem.pic;
	}
}


