using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryInfo : MonoBehaviour
{
	public static InventoryInfo Instance { get; set;}
	public GameObject info;
	public Image icon;
	public Text displayName;
	public Text rarity;
	public Text desc;
	public Text hint;
	bool isOn= false;

	void Awake()
	{
		Instance = this;
	}

	public void Show(string id, Sprite sp)
	{
		displayName.text = DownloadedAssets.ingredientDictData [id].name ;
		desc.text = DownloadedAssets.ingredientDictData [id].description;
		hint.text = DownloadedAssets.ingredientDictData [id].hint;
		rarity.text = "Rarity : " + DownloadedAssets.ingredientDictData [id].rarity.ToString ();
		icon.sprite = sp;
		info.SetActive (true);
	}

	void Update()
	{
		if(isOn && Input.GetMouseButtonUp(0)){
			info.SetActive (false);
			isOn = false;
		}
	}
}

