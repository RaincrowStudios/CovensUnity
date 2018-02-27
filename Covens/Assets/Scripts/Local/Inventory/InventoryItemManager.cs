using UnityEngine;
using System.Collections;

public class InventoryItemManager : MonoBehaviour
{

	public void swapScrollStart()
	{
		GetComponentInParent<InventoryScroll> ().canScroll = true;
		GetComponent<Animator> ().SetBool ("animate", true);
	}

	public void swapScrollStop()
	{
		GetComponentInParent<InventoryScroll> ().canScroll = true;
		GetComponent<Animator> ().SetBool ("animate", false);
	}
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

