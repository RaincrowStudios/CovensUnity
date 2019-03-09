using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CosmeticPurchasePopup : MonoBehaviour {

	public ApparelData cosData;
	public TextMeshProUGUI title;
	public TextMeshProUGUI silver;
	public TextMeshProUGUI gold;
	public Image icon;
	public Image style;

	public Transform maleModel;
	public Transform femaleModel;

	public bool buyWithSilver;
	public bool previewEnabled = false;

	public List<EquippedApparel> previewData;

	// Use this for initialization
	void Start () {
		previewData = PlayerDataManager.playerData.equipped;
		if (PlayerDataManager.playerData.male) {
			maleModel.gameObject.SetActive (true);
			maleModel.GetComponent<ApparelView>().InitializeChar(previewData);
			if (cosData.assets.baseAsset.Count > 0) {
				cosData.apparelType = ApparelType.Base;
			} else if (cosData.assets.white.Count > 0) {
				cosData.apparelType = ApparelType.White;
			}else if (cosData.assets.shadow.Count > 0) {
				cosData.apparelType = ApparelType.Shadow;
			}else if (cosData.assets.grey.Count > 0) {
				cosData.apparelType = ApparelType.Grey;
			}
			maleModel.GetComponent<ApparelView>().EquipApparel (cosData);
		} else {
			femaleModel.gameObject.SetActive (true);
			femaleModel.GetComponent<ApparelView>().InitializeChar(previewData);
			if (cosData.assets.baseAsset.Count > 0) {
				cosData.apparelType = ApparelType.Base;
			} else if (cosData.assets.white.Count > 0) {
				cosData.apparelType = ApparelType.White;
			}else if (cosData.assets.shadow.Count > 0) {
				cosData.apparelType = ApparelType.Shadow;
			}else if (cosData.assets.grey.Count > 0) {
				cosData.apparelType = ApparelType.Grey;
			}
			femaleModel.GetComponent<ApparelView>().EquipApparel (cosData);
		}
		transform.GetChild (11).GetChild (0).GetComponent<TextMeshProUGUI> ().text = "Preview On";
	}

	public void Setup(ApparelData data)
	{
		previewData = PlayerDataManager.playerData.equipped;
		cosData = data;
		title.text = "Buy <color=#ffffff> " +  DownloadedAssets.storeDict[data.id].title + "</color>";
		silver.text = data.silver.ToString();
		gold.text = data.gold.ToString ();
		DownloadedAssets.GetSprite(data.iconId,icon,true);
		//icon.sprite = sp;

		if (PlayerDataManager.playerData.silver > cosData.silver) {
			transform.GetChild (7).GetChild (2).GetComponent<Button> ().onClick.AddListener (() => {
				NewStoreUIManager.Instance.ConfirmPurchase (cosData.id, true);
				buyWithSilver = true;
			});
		} else {
			// instead turn this off and turn the other one on and add functionality for IAP
//			transform.GetChild (7).GetChild (2).GetComponent<TextMeshProUGUI> ().color = new Color (.35f, .35f, .35f, 1);
			transform.GetChild (7).GetChild (2).gameObject.SetActive(false);
			transform.GetChild (12).gameObject.SetActive (true);
			transform.GetChild (12).GetComponent<TextMeshProUGUI> ().color = Color.red;
			transform.GetChild (12).GetComponent<Button> ().onClick.AddListener (() => {
				// some code to transfer to the silver screen here
				NewStoreUIManager.Instance.BuyMoreSilverTransition();
//				NewStoreUIManager.Instance.OpenCosmeticsScreen(NewStoreUIManager.WheelButtonType.silver);
//				Destroy(this.gameObject);
			});
		}
//		transform.GetChild (7).GetChild (2).GetComponent<Button> ().onClick.AddListener (() => {
//			NewStoreUIManager.Instance.ConfirmPurchase(cosData.id, true);
//			buyWithSilver = true;
//		});
		if (PlayerDataManager.playerData.gold > cosData.gold) {
			transform.GetChild (8).GetChild (3).GetComponent<Button> ().onClick.AddListener (() => {
				NewStoreUIManager.Instance.ConfirmPurchase (cosData.id, false);
				buyWithSilver = false;
			});
		} else {
			transform.GetChild (8).GetChild (3).GetComponent<TextMeshProUGUI> ().color = Color.red;
		}

		if (cosData.storeCatagory == "style") {
			//probably should just turn this off, but I don't think it will hurt anything as this is an instance
			Destroy (maleModel.parent.gameObject);
			transform.GetChild (11).gameObject.SetActive (false);
			style.gameObject.SetActive (true);
			DownloadedAssets.GetSprite (cosData.iconId, style, false);
		}

		transform.GetChild (11).GetComponent<Button> ().onClick.AddListener (() => {
			if (previewEnabled) {
				if (PlayerDataManager.playerData.male){
					maleModel.GetComponent<ApparelView>().InitializeChar(previewData);
					if (cosData.assets.baseAsset.Count > 0) {
						cosData.apparelType = ApparelType.Base;
					} else if (cosData.assets.white.Count > 0) {
						cosData.apparelType = ApparelType.White;
					}else if (cosData.assets.shadow.Count > 0) {
						cosData.apparelType = ApparelType.Shadow;
					}else if (cosData.assets.grey.Count > 0) {
						cosData.apparelType = ApparelType.Grey;
					}
					maleModel.GetComponent<ApparelView>().EquipApparel (cosData);
				} else {
					femaleModel.GetComponent<ApparelView>().InitializeChar(previewData);
					if (cosData.assets.baseAsset.Count > 0) {
						cosData.apparelType = ApparelType.Base;
					} else if (cosData.assets.white.Count > 0) {
						cosData.apparelType = ApparelType.White;
					}else if (cosData.assets.shadow.Count > 0) {
						cosData.apparelType = ApparelType.Shadow;
					}else if (cosData.assets.grey.Count > 0) {
						cosData.apparelType = ApparelType.Grey;
					}
					femaleModel.GetComponent<ApparelView>().EquipApparel (cosData);
				}
				transform.GetChild (11).GetChild (0).GetComponent<TextMeshProUGUI> ().text = "Preview On";
			} else {
				if (PlayerDataManager.playerData.male) {
					maleModel.GetComponent<ApparelView>().InitializeChar(previewData);
				} else {
					femaleModel.GetComponent<ApparelView>().InitializeChar(previewData);
				}
				transform.GetChild (11).GetChild (0).GetComponent<TextMeshProUGUI> ().text = "Preview Off";
			}
			previewEnabled = !previewEnabled;
		});
//		transform.GetChild (8).GetComponent<Button> ().onClick.AddListener (() => {
//			NewStoreUIManager.Instance.ConfirmPurchase (data.id);
//		});
		transform.GetChild (6).GetComponent<Button> ().onClick.AddListener (() => {
			NewStoreUIManager.Instance.CancelPurchase();
		});
	}


	public ApparelData PassDataForPurchaseSuccess()
	{
		return cosData;
	}
}
