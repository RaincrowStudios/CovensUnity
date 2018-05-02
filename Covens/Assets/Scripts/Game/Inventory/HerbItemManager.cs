using UnityEngine;
using System.Collections;

public class HerbItemManager : MonoBehaviour
{

	public string itemName;

	public void OnClick()
	{
		ScrollManagerHerb.Instance.OnClick (itemName);
	}

	public void onRelease()
	{
		ScrollManagerHerb.Instance.OnRelease ( );
	}
}

