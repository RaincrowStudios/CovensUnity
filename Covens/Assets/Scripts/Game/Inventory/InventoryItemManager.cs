using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class InventoryItemManager : MonoBehaviour
{
	public string itemID;
	public Text title;
	public Text Count;
	public GameObject countContainer;
	public CanvasGroup cg;
	bool canClick = false;
	public void Setup(string name, int count, string id, bool isClick)
	{
		itemID = id;
		title.text = name;
		canClick = isClick;
		if (count > 0) {
			countContainer.SetActive (true);
			cg.alpha = 1;
			Count.text = count.ToString ();
		} else {
			countContainer.SetActive (false);
			cg.alpha = .35f;
		}
	}

	public void OnClick()
	{
		if(canClick)
		InventorySrollManager.Instance.OnClick (itemID);
	}

}

