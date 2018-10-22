using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;


public class GearUIManager : UIAnimationManager
{
	public CanvasGroup clothingFilter;
	public CanvasGroup accessories;
	public CanvasGroup skinArt;
	public CanvasGroup hairStyles;
	public GameObject left;
	public GameObject right;
	public GearButtonData[] buttonData;
	int displayCount = 6;
	int startPos = 0;
	int currentOnDisplay;
	bool isPreview = true;

	string currentFilter = "clothing";

	List<ApparelData> selectApparels = new List<ApparelData>(); 
	List<ApparelData> allApparels = new List<ApparelData>(); 

	public GameObject onSelectItemGold;
	public ApparelView male;
	public ApparelView female;
	public Text buyTitle;
	public Text goldCost;
	public Text silverCost;
	public Image buyIcon;
	ApparelView apparelView;
	ApparelData selectedApparelData;

	public GameObject NotEnoughGold;
	public GameObject NotEnoughtSilver;

	public GameObject BuySilverStore;
	public GameObject BuyWithSilverButton;
	public GameObject BuyWithGoldButton;

	public GameObject[] silverBuyObjects;
	public GameObject[] goldBuyObjects;
	public GameObject orText;
	public Text SilverText;
	public Text GoldText;

	public GearButtonData curButton;
	bool isGold;
	public void Init(List<ApparelData> apparelList)
	{
		foreach (var item in apparelList) {
			Utilities.SetCatagoryApparel (item);
		}
		if (PlayerDataManager.playerData.male) {
			female.gameObject.SetActive (false);
			male.gameObject.SetActive (true);
			apparelView = male;
		}else {
			female.gameObject.SetActive (true);
			male.gameObject.SetActive (false);
			apparelView = female;
		}
		allApparels = apparelList;
		ChangeFilter ("clothing");
	}

	public void moveLeft(){
		startPos -= (displayCount+currentOnDisplay);
		if (startPos < 0)
			startPos = 0;
		ShowItems ();
	}

	public void moveRight(){
		ShowItems ();
	}

	void ShowItems()
	{
		int curPos = startPos;
		currentOnDisplay = 0;

		foreach (var item in buttonData) {
			item.gameObject.SetActive (false);
		}
			if (selectApparels.Count > startPos + displayCount) {
				for (int i = 0; i < displayCount; i++) {
					buttonData [i].Setup (selectApparels [curPos + i]);
					buttonData [i].gameObject.SetActive (true);
					startPos++;
				currentOnDisplay++;
				}
				right.SetActive (true);
			} else {
				int k = selectApparels.Count - startPos;
				if (k > 0) {
					for (int i = 0; i < k; i++) {
						buttonData [i].Setup (selectApparels [curPos + i]);
						buttonData [i].gameObject.SetActive (true);
						startPos++;
					currentOnDisplay++;
					}
				}
				right.SetActive (false);
			}
		if (startPos == displayCount) {
			left.SetActive (false);
		} else {
			left.SetActive (true);
		}

	}

	public void ChangeFilter(string id){

		clothingFilter.alpha = .4f;
		accessories.alpha = .4f;
		skinArt.alpha = .4f;
		hairStyles.alpha = .4f;

		accessories.transform.GetChild (0).gameObject.SetActive (false);
		clothingFilter.transform.GetChild (0).gameObject.SetActive (false);
		skinArt.transform.GetChild (0).gameObject.SetActive (false);
		hairStyles.transform.GetChild (0).gameObject.SetActive (false);

		currentFilter = id;
		if (id == "clothing") {
			clothingFilter.alpha = 1;
			clothingFilter.transform.GetChild (0).gameObject.SetActive (true);
		} else if (id == "accessories") {
			accessories.alpha = 1;
			accessories.transform.GetChild (0).gameObject.SetActive (true);
		} else if (id == "hairstyles") {
			hairStyles.alpha = 1;
			hairStyles.transform.GetChild (0).gameObject.SetActive (true);
		} else {
			skinArt.alpha = 1;
			skinArt.transform.GetChild (0).gameObject.SetActive (true);
		}
		startPos = 0;
		filterList ();
	}

	void filterList()
	{
		selectApparels.Clear ();
		foreach (var item in allApparels) {
			if (item.storeCatagory == currentFilter) {
				selectApparels.Add (item);
			}
		}
		ShowItems ();
	}

	public void InitiateBuy(ApparelData data)
	{
		isPreview = true;
		selectedApparelData = data;
		Show (onSelectItemGold);
		TogglePreview ();

		buyIcon.sprite = DownloadedAssets.wardobePreviewArt [data.iconId];
		buyTitle.text = "Buy <color=ffffff>" + DownloadedAssets.storeDict[data.id].title + "</color>";
		silverCost.text = data.silver.ToString ();
		goldCost.text = data.gold.ToString ();

		foreach (var item in goldBuyObjects) {
			item.SetActive (data.gold != 0);
		}

		foreach (var item in silverBuyObjects) {
			item.SetActive (data.silver != 0);
		}

		if (PlayerDataManager.playerData.silver < data.silver) {
			BuyWithSilverButton.SetActive (false);
			SilverText.color = Color.red;
			BuySilverStore.SetActive (true);
			NotEnoughtSilver.SetActive (true);
		} else {
			NotEnoughtSilver.SetActive (false);
			SilverText.color = Color.white;
			BuySilverStore.SetActive (false);
			BuyWithSilverButton.SetActive (true);
		}

		if (PlayerDataManager.playerData.gold < data.gold) {
			BuyWithGoldButton.SetActive (false);
			NotEnoughGold.SetActive (true);
			GoldText.color = Color.red;
		} else {
			GoldText.color = Color.white;
			NotEnoughGold.SetActive (false);
			BuyWithGoldButton.SetActive (true);
		}

	
	

		orText.SetActive (data.silver > 0 && data.gold > 0);
	}

	public void CloseBuy()
	{
		Hide(onSelectItemGold);
	}

	public void TogglePreview()
	{
		if (isPreview) {
			apparelView.InitializeChar (PlayerDataManager.playerData.equipped);
			if (selectedApparelData.assets.baseAsset.Count > 0) {
				selectedApparelData.apparelType = ApparelType.Base;
			} else if (selectedApparelData.assets.white.Count > 0) {
				selectedApparelData.apparelType = ApparelType.White;
			}else if (selectedApparelData.assets.shadow.Count > 0) {
				selectedApparelData.apparelType = ApparelType.Shadow;
			}else if (selectedApparelData.assets.grey.Count > 0) {
				selectedApparelData.apparelType = ApparelType.Grey;
			}
			apparelView.EquipApparel (selectedApparelData);

		} else {
			apparelView.InitializeChar (PlayerDataManager.playerData.equipped);
		}
		isPreview = !isPreview;
	}

	public void ConfirmPurchase (bool isSilver = true)
	{
		isGold = !isSilver;
		var data = new{purchaseItem = selectedApparelData.id,currency = (isSilver ? "silver" : "gold")};  
		APIManager.Instance.PostData ("shop/purchase", JsonConvert.SerializeObject (data), PurchaseCallback); 
	}

	public void PurchaseCallback (string result, int code)
	{
		if (code == 200) {
			selectedApparelData.isNew = true;
			PlayerDataManager.playerData.inventory.cosmetics.Add (selectedApparelData);
			Hide (onSelectItemGold);
			StoreUIManager.Instance.PuchaseSuccess(true,selectedApparelData,isGold); 
			foreach (var item in allApparels) {
				if (item.id == selectedApparelData.id) {
					item.owned = true;
				}
			}
			foreach (var item in selectApparels) {
				if (item.id == selectedApparelData.id) {
					item.owned = true;
				}
			}
		} else {
			Debug.LogError ("Something Went Wrong in Purchase : " + result);
		}
	}
}

