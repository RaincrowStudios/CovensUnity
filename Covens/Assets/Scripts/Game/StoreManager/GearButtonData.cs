using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GearButtonData : MonoBehaviour
{
	public Text title;
	public Text silver;
	public Text gold;
	public Image icon;
	public Button button;
	public Text buttonText;
	public Sprite lockSprite;
	public Sprite unlockSprite;
	public ApparelData apData;
	public GearUIManager GM;
	public GameObject orText;

	public void Setup(ApparelData data)
	{
		apData = data;
		title.text = LocalizeLookUp.GetStoreTitle(data.id);
		if (data.silver > 0) {
			silver.transform.parent.gameObject.SetActive (true);
			silver.text = data.silver.ToString ();
		} else {
			silver.transform.parent.gameObject.SetActive (false);
		}
		if (data.gold > 0) {
			gold.transform.parent.gameObject.SetActive (true);
			gold.text = data.gold.ToString ();
		} else {
			gold.transform.parent.gameObject.SetActive (false);
		}

		orText.SetActive (data.silver > 0 && data.gold > 0);
	
		DownloadedAssets.GetSprite (data.iconId, icon, true);
		if (data.owned) {
			buttonText.text = "OWNED";
			button.image.sprite = unlockSprite;
		} else {
			buttonText.text = "PURCHASE";
			button.onClick.AddListener (OnClick);
			button.image.sprite = lockSprite;
		}
		button.interactable = true;
	}
	

	void OnClick()
	{
		GM.curButton = this; 
		GM.InitiateBuy (apData);
	}
}

