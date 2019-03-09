using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class NewStoreUIManager : MonoBehaviour {

	public static NewStoreUIManager Instance { get; set; }

	public enum WheelButtonType
	{
		gear, charms, ingredients, silver, style, wheel
	}

	private enum FilterButtonType
	{
		clothing, accessories, skinart, hairstyles
	}

	public Sprite[] nonCosmetics;

	public GameObject cosmeticPrefab;
	public GameObject silverPrefab;
	public GameObject ingredientCharmPrefab;
	public GameObject wheelContainer;
	public GameObject gearFilter;
	public GameObject drachs;
	public TextMeshProUGUI silverCoin;
	public TextMeshProUGUI goldCoin;
	public int silverNum;
	public int goldNum;
	public GameObject darken;
	public GameObject fortuna;
	public CanvasGroup fortunaCG;
	public GameObject itemMask;
	public GameObject stylePrefab;
	private GameObject styleInstance;
	public GameObject close;

	public TextMeshProUGUI purchaseInfoText;

	public Transform screenHeader;
	public GameObject headerChild;

	public GameObject itemSelectedGold;
	public GameObject itemSelectedSilver;
	public GameObject purchaseSuccess;

	public GameObject purchaseScreen;
	public GameObject purchaseSuccessScreen;
	public int buySilverID;

	//Orry's stuff
	public GameObject wheelGear;
	public GameObject wheelCharms;
	public GameObject wheelIngredients;
	public GameObject wheelSilver;
	public GameObject wheelBG;
	public CanvasGroup wheelBGCG;
	public CanvasGroup wheelGearCG;
	public CanvasGroup wheelCharmsCG;
	public CanvasGroup wheelIngredientsCG;
	public CanvasGroup wheelSilverCG;

	public float moveFortuna;
	public float inTime;
	//end orry's stuff
	private Vector2 squareGrid = new Vector2(60,60);
	private Vector2 cosGrid = new Vector2(120,60);

	private WheelButtonType currScreenType;
	private FilterButtonType currFilterType = FilterButtonType.hairstyles;

	StoreApiObject storeItems;

	//public LeanTweenType easy;

	private List<StoreApiItem> ingredients = new List<StoreApiItem>();
	private List<StoreApiItem> charms = new List<StoreApiItem>();
	private List<StoreApiItem> silver = new List<StoreApiItem>();
	private List<ApparelData> gear = new List<ApparelData>();
	private List<ApparelData> styles = new List<ApparelData>();

	// Use this for initialization
	void Start () {
		Instance = this;
		StoreManagerAPI.GetShopItems (Callback);
		wheelContainer.transform.GetChild (0).GetChild (0).GetComponent<Button> ().onClick.AddListener (() => OpenCosmeticsScreen(WheelButtonType.gear));
		wheelContainer.transform.GetChild (1).GetChild (0).GetComponent<Button> ().onClick.AddListener (() => OpenCosmeticsScreen(WheelButtonType.charms));
		wheelContainer.transform.GetChild (2).GetChild (0).GetComponent<Button> ().onClick.AddListener (() => OpenCosmeticsScreen(WheelButtonType.ingredients));
		wheelContainer.transform.GetChild (3).GetChild (0).GetComponent<Button> ().onClick.AddListener (() => OpenCosmeticsScreen(WheelButtonType.silver));
		this.transform.GetChild (9).GetComponent<Button> ().onClick.AddListener (() => {
			CloseCosmeticsScreen ();
		});
		silverCoin = drachs.transform.GetChild (0).GetChild (2).GetComponent<TextMeshProUGUI> ();
		goldCoin = drachs.transform.GetChild (1).GetChild (2).GetComponent<TextMeshProUGUI> ();
		silverNum = PlayerDataManager.playerData.silver;
		goldNum = PlayerDataManager.playerData.gold;
		silverCoin.text = silverNum.ToString ();
		goldCoin.text = goldNum.ToString ();

		gearFilter.transform.GetChild (1).GetComponent<Button> ().onClick.AddListener (() => ResetGearScreen (FilterButtonType.clothing));
		gearFilter.transform.GetChild (2).GetComponent<Button> ().onClick.AddListener (() => ResetGearScreen (FilterButtonType.accessories));
		gearFilter.transform.GetChild (3).GetComponent<Button> ().onClick.AddListener (() => ResetGearScreen (FilterButtonType.skinart));
		gearFilter.transform.GetChild (4).GetComponent<Button> ().onClick.AddListener (() => ResetGearScreen (FilterButtonType.hairstyles));
	}

	public void Callback (string result, int code)
	{
		if (code == 200) {
			LeanTween.alphaCanvas (transform.GetComponent<CanvasGroup> (), 1f, .5f).setEase(LeanTweenType.easeInOutQuad);
			print (result);

			storeItems = JsonConvert.DeserializeObject<StoreApiObject> (result);

		} else {
			print ("failure");
			Debug.LogError (code + " | Couldn't get store data : " + result);
			//probably want to destroy store Instance here
		}
		WheelSpinIn();
		FortunaSlideIn();

		for (int i = 0; i < storeItems.cosmetics.Count; i++) {
			Utilities.SetCatagoryApparel (storeItems.cosmetics [i]);
			if (storeItems.cosmetics[i].storeCatagory == "style") {
				styles.Add (storeItems.cosmetics[i]);
			} else {
				gear.Add (storeItems.cosmetics[i]);
			}
		}
		ingredients = storeItems.bundles;
		charms = storeItems.consumables;
		silver = storeItems.silver;
	}

	public void OpenCosmeticsScreen(WheelButtonType buttonPressed)
	{
		
		itemMask.transform.localScale = Vector3.one;
		itemMask.GetComponent<CanvasGroup> ().alpha = 1;
		WheelFadeOut ();
		LeanTween.alphaCanvas (darken.GetComponent<CanvasGroup>(), 1f, inTime).setEase(LeanTweenType.easeInOutQuad);
		if (buttonPressed != WheelButtonType.style) {
			FortunaSlideOut ();
		} else {
			for (int i = 0; i < screenHeader.childCount; i++) {
				Destroy (screenHeader.GetChild(i).gameObject);
			}
		}
		if (buttonPressed != WheelButtonType.silver) {
			itemMask.transform.GetChild (0).GetComponent<GridLayoutGroup> ().startAxis = GridLayoutGroup.Axis.Vertical;
		} else {
			itemMask.transform.GetChild (0).GetComponent<GridLayoutGroup> ().startAxis = GridLayoutGroup.Axis.Horizontal;
		}
		itemMask.transform.GetChild (0).GetComponent<GridLayoutGroup> ().spacing = squareGrid;
		itemMask.transform.GetChild (0).GetComponent<GridLayoutGroup> ().padding.top = 0;
		currScreenType = buttonPressed;
		if (styleInstance == null) {
			StartCoroutine(DelayInstantiation(.2f));
		}
		itemMask.SetActive (true);
		darken.SetActive (true);
		drachs.SetActive (true);

		GameObject[] o = new GameObject[2];
		switch (currScreenType) {
		case WheelButtonType.style:
			Transform maskContainer = itemMask.transform.GetChild (0);
			for (int i = 0; i < maskContainer.childCount; i++) {
				Destroy (maskContainer.GetChild (i).gameObject);
			}
			itemMask.SetActive (false);
			gearFilter.SetActive (false);
			darken.SetActive (false);
			FortunaSlideIn ();
			o [0] = Instantiate (headerChild, screenHeader);
			o [0].GetComponent<CanvasGroup> ().alpha = 1;
			o [0].GetComponent<TextMeshProUGUI> ().text = "Gear";
			o [0].AddComponent<Button> ().onClick.AddListener (() => OpenCosmeticsScreen (WheelButtonType.gear));
			o [0].GetComponent<TextMeshProUGUI> ().color = new Color (.35f, .35f, .35f, 1);

			o [1] = Instantiate (headerChild, screenHeader);
			o [1].GetComponent<CanvasGroup> ().alpha = 1;
			o [1].GetComponent<TextMeshProUGUI> ().text = "Styles";
			o [1].GetComponent<TextMeshProUGUI> ().color = Color.white;

			styleInstance = Instantiate (stylePrefab, this.transform);
			styleInstance.GetComponent<StyleScreenSetup> ().SetupStyleScreen (styles);
			break;
		case WheelButtonType.gear:
			for (int i = 0; i < screenHeader.childCount; i++) {
				Destroy (screenHeader.GetChild(i).gameObject);
			}
			itemMask.transform.GetChild (0).GetComponent<GridLayoutGroup> ().spacing = cosGrid;
			o [0] = Instantiate (headerChild, screenHeader);
			o [0].GetComponent<TextMeshProUGUI> ().text = "Gear";
			o [1] = Instantiate (headerChild, screenHeader);
			o [1].GetComponent<TextMeshProUGUI> ().text = "Styles";
			o [1].GetComponent<TextMeshProUGUI> ().color = new Color (.35f, .35f, .35f, 1);
			o [1].AddComponent<Button> ().onClick.AddListener (() => OpenCosmeticsScreen (WheelButtonType.style));
			if (styleInstance != null) {
				if (currFilterType == FilterButtonType.clothing) {
					currFilterType = FilterButtonType.hairstyles;
				}
				o [0].GetComponent<CanvasGroup> ().alpha = 1;
				o [1].GetComponent<CanvasGroup> ().alpha = 1;
				Destroy (styleInstance);
			} else {
				HeaderFadeIn ();
			}
			if (fortunaCG.alpha == 1) {
				FortunaSlideOut ();
			}
			gearFilter.SetActive (true);
			ResetGearScreen (FilterButtonType.clothing);

			break;
		case WheelButtonType.charms:
			o [0] = Instantiate (headerChild, screenHeader);
			o [0].GetComponent<TextMeshProUGUI> ().text = "Charms";
			HeaderFadeIn ();
			break;
		case WheelButtonType.ingredients:
			//here I'm centering the children in ingredients
			itemMask.transform.GetChild (0).GetComponent<GridLayoutGroup> ().padding.top = 200;
			o [0] = Instantiate (headerChild, screenHeader);
			o [0].GetComponent<TextMeshProUGUI> ().text = "Ingredients";
			HeaderFadeIn ();
			break;
		case WheelButtonType.silver:
			o [0] = Instantiate (headerChild, screenHeader);
			o [0].GetComponent<TextMeshProUGUI> ().text = "Silver";
			HeaderFadeIn ();
			break;
		default:
			o = null;
			break;
		}
	}

	private void CloseCosmeticsScreen()
	{
	
		LeanTween.scale (itemMask, Vector3.zero, inTime).setEase (LeanTweenType.easeInOutQuad).setOnStart(() => {
			HeaderFadeOut ();
			LeanTween.alphaCanvas (darken.GetComponent<CanvasGroup>(), 0f, inTime).setEase(LeanTweenType.easeInOutQuad);
			if (styleInstance != null)
			{
				LeanTween.scale (styleInstance, Vector3.zero, inTime).setEase (LeanTweenType.easeInOutQuad);
			}
		}).setOnComplete(() => {
			//LeanTween.alphaCanvas (darken.GetComponent<CanvasGroup>(), 0f, inTime).setEase(LeanTweenType.easeInOutQuad);
			WheelFadeIn ();
			//HeaderFadeOut ();
			itemMask.SetActive (false);

			drachs.SetActive (false);
			if (styleInstance == null)
			{
				FortunaSlideIn ();
			}
			if (currScreenType == WheelButtonType.gear) {
				gearFilter.SetActive (false);
			}
			for (int i = 0; i < screenHeader.childCount; i++) {
				Destroy (screenHeader.GetChild(i).gameObject);
			}
			Transform maskContainer = itemMask.transform.GetChild (0);
			for (int i = 0; i < maskContainer.childCount; i++) {
				Destroy (maskContainer.GetChild (i).gameObject);
			}
		});

		LeanTween.alphaCanvas (itemMask.GetComponent<CanvasGroup>(), 0f, inTime).setEase (LeanTweenType.easeInOutQuad).setOnComplete(()=>{
			if (currScreenType != WheelButtonType.wheel) {
				
				if (styleInstance != null) {
					Destroy (styleInstance);
				}

				int instanceCount = itemMask.transform.GetChild (0).childCount;
				for (int i = 0; i < instanceCount; i++) {
					Destroy (itemMask.transform.GetChild (0).GetChild (i).gameObject);
				}
				currScreenType = WheelButtonType.wheel;
			} else {
				DestroyStoreInstance ();
			}
		});
	}
		
	private void ResetGearScreen(FilterButtonType filterType)
	{

		if (filterType != currFilterType) {
			int instanceCount = itemMask.transform.GetChild (0).childCount;

			for (int i = 0; i < instanceCount; i++) {
				Destroy (itemMask.transform.GetChild (0).GetChild (i).gameObject);
			}
			switch (currFilterType) {
			case FilterButtonType.clothing:
				gearFilter.transform.GetChild (1).GetComponent<TextMeshProUGUI> ().color = new Color(.35f, .35f, .35f, 1);
				gearFilter.transform.GetChild (1).GetComponent<TextMeshProUGUI> ().fontStyle = FontStyles.Normal;
				break;
			case FilterButtonType.accessories:
				gearFilter.transform.GetChild (2).GetComponent<TextMeshProUGUI> ().color = new Color(.35f, .35f, .35f, 1);
				gearFilter.transform.GetChild (2).GetComponent<TextMeshProUGUI> ().fontStyle = FontStyles.Normal;
				break;
			case FilterButtonType.skinart:
				gearFilter.transform.GetChild (3).GetComponent<TextMeshProUGUI> ().color = new Color(.35f, .35f, .35f, 1);
				gearFilter.transform.GetChild (3).GetComponent<TextMeshProUGUI> ().fontStyle = FontStyles.Normal;
				break;
			case FilterButtonType.hairstyles:
				gearFilter.transform.GetChild (4).GetComponent<TextMeshProUGUI> ().color = new Color(.35f, .35f, .35f, 1);
				gearFilter.transform.GetChild (4).GetComponent<TextMeshProUGUI> ().fontStyle = FontStyles.Normal;
				break;
			default:
				break;
			}
			currFilterType = filterType;
			switch (currFilterType) {
			case FilterButtonType.clothing:
				gearFilter.transform.GetChild (1).GetComponent<TextMeshProUGUI> ().color = Color.white;
				gearFilter.transform.GetChild (1).GetComponent<TextMeshProUGUI> ().fontStyle = FontStyles.Underline;
				break;
			case FilterButtonType.accessories:
				gearFilter.transform.GetChild (2).GetComponent<TextMeshProUGUI> ().color = Color.white;
				gearFilter.transform.GetChild (2).GetComponent<TextMeshProUGUI> ().fontStyle = FontStyles.Underline;
				break;
			case FilterButtonType.skinart:
				gearFilter.transform.GetChild (3).GetComponent<TextMeshProUGUI> ().color = Color.white;
				gearFilter.transform.GetChild (3).GetComponent<TextMeshProUGUI> ().fontStyle = FontStyles.Underline;
				break;
			case FilterButtonType.hairstyles:
				gearFilter.transform.GetChild (4).GetComponent<TextMeshProUGUI> ().color = Color.white;
				gearFilter.transform.GetChild (4).GetComponent<TextMeshProUGUI> ().fontStyle = FontStyles.Underline;
				break;
			default:
				break;
			}
			//SetupContainer ();
			StartCoroutine(DelayInstantiation(.2f));
		} else {
			print ("no sir");
		}
	}

	IEnumerator DelayInstantiation(float time)
	{
		Transform maskContainer = itemMask.transform.GetChild (0);
		if (currScreenType == WheelButtonType.gear) {
			for (int i = 0; i < gear.Count; i++) {
				
				float tempTime = 0;
				if (gear [i].storeCatagory == currFilterType.ToString ()) {
					CosmeticSetup cos = Instantiate (cosmeticPrefab, maskContainer).GetComponent<CosmeticSetup> ();
					cos.SetupGearInstance (gear [i]);
//					while (tempTime < time) {
//						tempTime += Time.deltaTime;
//					}
					yield return null;
				}
			}
		} else if (currScreenType == WheelButtonType.charms) {

			for (int i = 0; i < charms.Count; i++) {
				float tempTime = 0;
				IngredientCharmSetup ing = Instantiate (ingredientCharmPrefab, maskContainer).GetComponent<IngredientCharmSetup> ();
				ing.SetupConsumableInstance (charms[i]);
				if (charms [i].id.Contains ("energy")) {
					if (charms [i].id.Contains ("1")) {
						ing.itemIcon.sprite = nonCosmetics [11];
					} else if (charms [i].id.Contains ("3")) {
						ing.itemIcon.sprite = nonCosmetics [12];
					} else {
						ing.itemIcon.sprite = nonCosmetics [13];
					}
				} else if (charms [i].id.Contains ("align")) {
					if (charms [i].id.Contains ("Smaller")) {
						ing.itemIcon.sprite = nonCosmetics [10];
					} else if (charms [i].id.Contains ("Medium")) {
						ing.itemIcon.sprite = nonCosmetics [9];
					} else {
						ing.itemIcon.sprite = nonCosmetics [8];
					}
				} else if (charms [i].id.Contains ("xp")) {
					if (charms [i].id.Contains ("Smaller")) {
						ing.itemIcon.sprite = nonCosmetics [6];
					} else if (charms [i].id.Contains ("Medium")) {
						ing.itemIcon.sprite = nonCosmetics [5];
					} else {
						ing.itemIcon.sprite = nonCosmetics [4];
					}
				} else if (charms [i].id.Contains ("invisible")) {
					ing.itemIcon.sprite = nonCosmetics [3];
				} else if (charms [i].id.Contains ("truesight")) {
					ing.itemIcon.sprite = nonCosmetics [7];
				}
				while (tempTime < time) {
					tempTime += Time.deltaTime;
				}

				yield return null;
			}
		} else if (currScreenType == WheelButtonType.ingredients) {
			
			if (ingredients.Count <= 3) {
				for (int i = 0; i < ingredients.Count; i++) {
					
					float tempTime = 0;
					IngredientCharmSetup ing = Instantiate (ingredientCharmPrefab, maskContainer).GetComponent<IngredientCharmSetup> ();
					ing.SetupConsumableInstance (ingredients[i]);
					ing.itemIcon.sprite = nonCosmetics [i];
					while (tempTime < time) {
						tempTime += Time.deltaTime;
					}
					GameObject newObj2 = Instantiate (ingredientCharmPrefab, maskContainer);
					newObj2.transform.GetChild (0).gameObject.SetActive (false);
					yield return null;
				}
			} else {
				for (int i = 0; i < ingredients.Count; i++) {
					float tempTime = 0;
					IngredientCharmSetup ing = Instantiate (ingredientCharmPrefab, maskContainer).GetComponent<IngredientCharmSetup> ();
					ing.SetupConsumableInstance (ingredients[i]);
					while (tempTime < time) {
						tempTime += Time.deltaTime;
					}
					yield return null;
				}
			}
		} else if (currScreenType == WheelButtonType.silver) {

			for (int i = 0; i < silver.Count; i++) {
				float tempTime = 0;
				SilverSetup sil = Instantiate (silverPrefab, maskContainer).GetComponent<SilverSetup> ();
				sil.SetupSilverInstance (silver[i]);
				sil.itemIcon.sprite = nonCosmetics [i + 14];
				while (tempTime < time) {
					tempTime += Time.deltaTime;
				}
				yield return null;
			}
		}
	}

	public void InitiateSilverPurchase(StoreApiItem data, Sprite sp)
	{
		for (int i = 0; i < screenHeader.childCount; i++) {
			screenHeader.GetChild(i).gameObject.SetActive (false);
		}
		close.SetActive (false);
		purchaseScreen = Instantiate (itemSelectedSilver, transform);
		purchaseScreen.GetComponent<PurchasePopup> ().Setup (data, sp);
	}

	public void InitiateCosmeticPurchase (ApparelData data)
	{
		for (int i = 0; i < screenHeader.childCount; i++) {
			screenHeader.GetChild(i).gameObject.SetActive (false);
		}
		close.SetActive (false);
		purchaseScreen = Instantiate (itemSelectedGold, transform);
		purchaseScreen.GetComponent<CosmeticPurchasePopup> ().Setup (data);
	}

	public void ConfirmPurchase(string id, bool isSilver)
	{
		var data = new {purchaseItem = id, currency = (isSilver ? "silver" : "gold")};
		APIManager.Instance.PostData ("shop/purchase", JsonConvert.SerializeObject (data), PurchaseCallback);
	}

	public void PurchaseCallback(string result, int code)
	{
		if (code == 200)
		{
			//I don't remember why I commented this line out
			if (purchaseScreen.GetComponent<PurchasePopup> () != null) {
				Raincrow.Analytics.Events.PurchaseAnalytics.PurchaseItem (purchaseScreen.GetComponent<PurchasePopup> ().itemData.id, false);
			} else {
				Raincrow.Analytics.Events.PurchaseAnalytics.PurchaseItem (purchaseScreen.GetComponent<CosmeticPurchasePopup> ().cosData.id, true);
			}

			PurchaseSuccess (); 
		} else {
			Debug.LogError ("Something Went Wrong in Purchase : " + result);
			//display error message popup here or something instead
			Destroy (purchaseScreen);
			for (int i = 0; i < screenHeader.childCount; i++) {
				screenHeader.GetChild(i).gameObject.SetActive (true);
			}
			close.SetActive (true);
		}
	}

	public void PurchaseSuccess()
	{
		SoundManagerOneShot.Instance.PlayReward ();

		if (purchaseScreen.GetComponent<PurchasePopup> () != null) {
			StoreApiItem item = new StoreApiItem ();
			Sprite itemPic;
			item = purchaseScreen.GetComponent<PurchasePopup> ().PassDataForPurchaseSuccess ();
			itemPic = purchaseScreen.GetComponent<PurchasePopup> ().icon.sprite;
			if (item.type != "silver") {
				//PlayerDataManager.playerData.silver -= item.silver;
				CountSilver (item.silver);
			} else {
				//needs double checking
				PlayerDataManager.playerData.silver += item.silver;
				CountSilver (-item.silver);
			}

			Destroy (purchaseScreen);
			purchaseSuccessScreen = Instantiate (purchaseSuccess, transform);
			purchaseSuccessScreen.GetComponent<PurchaseSuccessPopup> ().SetupSuccessScreen (item, itemPic);

		} else if (purchaseScreen.GetComponent<CosmeticPurchasePopup> () != null) {
			bool isSilver = purchaseScreen.GetComponent<CosmeticPurchasePopup> ().buyWithSilver;
			ApparelData data = purchaseScreen.GetComponent<CosmeticPurchasePopup> ().PassDataForPurchaseSuccess ();
			Destroy (purchaseScreen);
			purchaseSuccessScreen = Instantiate (purchaseSuccess, transform);
			purchaseSuccessScreen.GetComponent<PurchaseSuccessPopup> ().SetupSuccessScreen (data);

			for (int i = 0; i < itemMask.transform.GetChild (0).childCount; i++) {
				CosmeticSetup currCos = itemMask.transform.GetChild (0).GetChild (i).GetComponent<CosmeticSetup> ();
				if (currCos.cosmeticData.id == data.id) {
					currCos.button.transform.GetComponentInParent<Image> ().sprite = currCos.greenButton;
					currCos.button.text = "OWNED";
				}
			}

			if (!isSilver) {
				CountGold (data.gold);
			} else {
				CountSilver (data.silver);
			}
		} else {
			switch (buySilverID)
			{
			case 1:
				CountSilver (-100);
				break;
			case 2:
				CountSilver (-550);
				break;
			case 3:
				CountSilver (-1200);
				break;
			case 4:
				CountSilver (-2500);
				break;
			case 5:
				CountSilver (-5200);
				break;
			case 6:
				CountSilver (-14500);
				break;
			default:
				print ("wrong purchaseID?");
				break;
			}
		}
	}

	public void CancelPurchase()
	{
		for (int i = 0; i < screenHeader.childCount; i++) {
			screenHeader.GetChild(i).gameObject.SetActive (true);
		}
		close.SetActive (true);
		if (currScreenType == WheelButtonType.style) {
			styleInstance.SetActive (true);
		}
		Destroy (purchaseScreen);
	}

	//not used yet
	public void DestroyPurchaseSuccessScreen()
	{
		close.SetActive (true);
		Destroy (purchaseSuccessScreen);
		for (int i = 0; i < screenHeader.childCount; i++) {
			screenHeader.GetChild(i).gameObject.SetActive (true);
		}
	}

	private IEnumerator ShowPurchaseLocationInfo()
	{
		//set which purchase info text it is based on the item data
		yield return new WaitForSeconds (3f);
		FadeOutText ();
	}

	public void FadeInText(ApparelData data)
	{
		purchaseInfoText.text = "Your purchase has been sent to your wardrobe.";
		//purchaseInfoText.text = DownloadedAssets.localizedText[LocalizationManager.store_purchase_info_inventory];
		LeanTween.alphaCanvas (purchaseInfoText.GetComponent<CanvasGroup> (), 1f, .5f)
			.setEase (LeanTweenType.easeInOutQuad)
			.setOnComplete(() => {
				StartCoroutine(ShowPurchaseLocationInfo());
			});
	}

	public void FadeInText(StoreApiItem data)
	{
		purchaseInfoText.text = "Your purchase has been sent to your inventory.";
		//purchaseInfoText.text = DownloadedAssets.localizedText[LocalizationManager.store_purchase_info_wardrobe];
		LeanTween.alphaCanvas (purchaseInfoText.GetComponent<CanvasGroup> (), 1f, .5f)
			.setEase (LeanTweenType.easeInOutQuad)
			.setOnComplete(() => {
				StartCoroutine(ShowPurchaseLocationInfo());
			});
	}

	private void FadeOutText()
	{
		LeanTween.alphaCanvas (purchaseInfoText.GetComponent<CanvasGroup> (), 0f, .5f)
			.setEase (LeanTweenType.easeInOutQuad);
	}

	public void BuyMoreSilverTransition()
	{
		CloseCosmeticsScreen ();
		Destroy (purchaseScreen);
		close.SetActive (true);
		StartCoroutine (SilverTransition ());
	}

	private IEnumerator SilverTransition()
	{
		yield return new WaitForSeconds (1f);
		OpenCosmeticsScreen (WheelButtonType.silver);
	}

	public void WheelSpinIn()
	{
		LeanTween.rotateZ (wheelBG, 0f, inTime).setEase(LeanTweenType.easeOutCubic);
		LeanTween.alphaCanvas (wheelBGCG, 1f, inTime*0.8f).setEase (LeanTweenType.easeOutCubic);
		LeanTween.rotateZ(wheelContainer, 0f, inTime).setEase (LeanTweenType.easeOutCubic);
		LeanTween.alphaCanvas (wheelGearCG, 1f, inTime*0.4f);
		LeanTween.alphaCanvas (wheelSilverCG, 1f, inTime*0.6f);
		LeanTween.alphaCanvas (wheelCharmsCG, 1f, inTime*0.8f);
		LeanTween.alphaCanvas (wheelIngredientsCG, 1f, inTime*1f);
		LeanTween.rotateZ (wheelGear, 0f, inTime * 1.1f).setEase (LeanTweenType.easeOutCubic);
		LeanTween.rotateZ (wheelSilver, 0f, inTime * 1.1f).setEase (LeanTweenType.easeOutCubic);
		LeanTween.rotateZ (wheelCharms, 0f, inTime * 1.1f).setEase (LeanTweenType.easeOutCubic);
		LeanTween.rotateZ (wheelIngredients, 0f, inTime * 1.1f).setEase (LeanTweenType.easeOutCubic);
	}

	public void WheelFadeOut()
	{
		LeanTween.alphaCanvas (wheelBGCG, 0f, inTime).setEase (LeanTweenType.easeInOutQuad).setOnComplete(() => {
			wheelBG.SetActive(false);
			wheelContainer.SetActive(false);
		});
		LeanTween.alphaCanvas (wheelGearCG, 0f, inTime).setEase (LeanTweenType.easeInOutQuad);
		LeanTween.alphaCanvas (wheelSilverCG, 0f, inTime).setEase (LeanTweenType.easeInOutQuad);
		LeanTween.alphaCanvas (wheelIngredientsCG, 0f, inTime).setEase (LeanTweenType.easeInOutQuad);
		LeanTween.alphaCanvas (wheelCharmsCG, 0f, inTime).setEase (LeanTweenType.easeInOutQuad);
	}

	public void WheelFadeIn()
	{
		wheelBG.SetActive(true);
		wheelContainer.SetActive(true);
		LeanTween.alphaCanvas (wheelBGCG, 1f, inTime).setEase (LeanTweenType.easeInOutQuad);
		LeanTween.alphaCanvas (wheelGearCG, 1f, inTime).setEase (LeanTweenType.easeInOutQuad);
		LeanTween.alphaCanvas (wheelSilverCG, 1f, inTime).setEase (LeanTweenType.easeInOutQuad);
		LeanTween.alphaCanvas (wheelIngredientsCG, 1f, inTime).setEase (LeanTweenType.easeInOutQuad);
		LeanTween.alphaCanvas (wheelCharmsCG, 1f, inTime).setEase (LeanTweenType.easeInOutQuad);
	}

	public void HeaderFadeIn()
	{
		for (int i = 0; i < screenHeader.childCount; i++) {
			LeanTween.alphaCanvas (screenHeader.GetChild (i).GetComponent<CanvasGroup> (), 1f, inTime);
		}
	}

	public void HeaderFadeOut()
	{
		for (int i = 0; i < screenHeader.childCount; i++) {
			LeanTween.alphaCanvas (screenHeader.GetChild (i).GetComponent<CanvasGroup> (), 0f, inTime);
		}
	}

	//need wheel spin out

	public void CountSilver(int itemCost)
	{
		PlayerDataManager.playerData.silver -= itemCost;
		LeanTween.value (silverNum, PlayerDataManager.playerData.silver, 1f).setOnUpdate((float f) => {
			f = (int)f;
			silverCoin.text = f.ToString();
		});
		silverNum = PlayerDataManager.playerData.silver;
	}

	public void CountGold(int itemCost)
	{
		PlayerDataManager.playerData.gold -= itemCost;
		LeanTween.value (goldNum, PlayerDataManager.playerData.gold, 1f).setOnUpdate((float f) => {
			f = (int)f;
			silverCoin.text = f.ToString();
		});
		goldNum = PlayerDataManager.playerData.gold;
	}


	public void FortunaSlideIn()
	{
		LeanTween.moveLocalX(fortuna, moveFortuna, inTime).setEase(LeanTweenType.easeOutCubic);
		LeanTween.alphaCanvas(fortunaCG, 1f, inTime).setEase(LeanTweenType.easeOutCubic);
	}

	public void FortunaSlideOut()
	{
		LeanTween.moveLocalX (fortuna, (moveFortuna + 500f), inTime).setEase (LeanTweenType.easeInCubic);
		LeanTween.alphaCanvas(fortunaCG, 0f, inTime).setEase(LeanTweenType.easeInCubic);
	}
		

	//not sure if this is right yet
	private void DestroyStoreInstance()
	{
		Destroy (this.gameObject);
	}
}
