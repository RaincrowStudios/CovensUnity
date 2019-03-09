using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CosmeticSetup : MonoBehaviour {

	public ApparelData cosmeticData;
	public Image itemIcon;
	public Sprite greenButton;
	public TextMeshProUGUI itemName;
	public TextMeshProUGUI costGold;
	public TextMeshProUGUI costSilver;
	public TextMeshProUGUI button;

	public LeanTweenType easy;
	public float inTime;

	void Awake()
	{
		//add a listener for unlocking
		//transform.GetChild (0).GetChild (5).GetComponent<Button> ().onClick.AddListener ();
		//
	}


	public void SetupGearInstance(ApparelData data)
	{
		cosmeticData = data;
		DownloadedAssets.GetSprite(data.iconId,itemIcon,true);
		try {
			itemName.text = DownloadedAssets.storeDict [data.id].title;
		}
		catch {
			print ("fuck " + data.id);
		}
		costGold.text = data.gold.ToString();
		costSilver.text = data.silver.ToString ();
		if (data.owned) {
			button.text = "OWNED";
			this.button.transform.GetComponentInParent<Image> ().sprite = greenButton;
		} else {
			button.text = "BUY";
			this.transform.GetChild (0).GetChild (5).gameObject.AddComponent<Button>().onClick.AddListener(() => {
				NewStoreUIManager.Instance.InitiateCosmeticPurchase(data);
			});
		}

		this.gameObject.transform.localScale = Vector3.zero;
		LeanTween.scale (this.gameObject, Vector3.one, inTime).setEase (easy);
	}

}
