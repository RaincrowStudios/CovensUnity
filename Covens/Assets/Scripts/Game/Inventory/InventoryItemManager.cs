using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class InventoryItemManager : MonoBehaviour
{
	public string itemID;
	public Text title;
	public Text rarity;
	public Text Count;
	public Sprite icon;
	public float rot;
	public int Index;
	public void Setup(int count, string id , int index )
	{
		Index = index;

		if (id == "null") {
			itemID = "null";
			title.text = "Empty";
			Count.transform.parent.gameObject.SetActive (false);
			rarity.gameObject.SetActive (false);
			return;
		}
		Count.transform.parent.gameObject.SetActive (true);
		rarity.gameObject.SetActive (true);

		rarity.text = "Rarity (" + DownloadedAssets.ingredientDictData[id].rarity.ToString()+")";
		itemID = id;
		title.text = DownloadedAssets.ingredientDictData[id].name;
		Count.text = count.ToString ();
	}
	void Update()
	{
		rot = transform.rotation.eulerAngles.y;
	}
	public void OnClick()
	{
			InventoryInfo.Instance.Show (itemID, icon);
	}

}

