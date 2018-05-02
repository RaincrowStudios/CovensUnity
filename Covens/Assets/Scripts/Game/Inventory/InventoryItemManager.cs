using UnityEngine;
using System.Collections;

public class InventoryItemManager : MonoBehaviour
{

	public string itemName;

	public void OnClick()
	{
		InventorySrollManager.Instance.OnClick (itemName);
	}

	public void onRelease()
	{
		InventorySrollManager.Instance.OnRelease ( );
	}
}

