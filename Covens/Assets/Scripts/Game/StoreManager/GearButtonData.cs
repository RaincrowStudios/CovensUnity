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

	public void Setup(ApparelData data)
	{
		apData = data;
		title.text = data.id;
		silver.text = data.silver.ToString ();
		gold.text = data.gold.ToString ();
		icon.sprite = DownloadedAssets.wardobePreviewArt [data.iconId];
		if (data.owned) {
			buttonText.text = "OWNED";
			button.image.sprite = unlockSprite;
		} else {
			buttonText.text = "UNLOCKED";
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

