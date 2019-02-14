using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RibbonController : MonoBehaviour
{
	public List<RibbonItem> ribbons = new List<RibbonItem> ();
	// Use this for initialization
	void Awake ()
	{
		foreach (Transform item in transform) {
			ribbons.Add( item.gameObject.AddComponent<RibbonItem> () as RibbonItem);
		}
		//ribbons[0].setState(true);
		
	}

	// public void setActiveRibbon(int index){
	// 	for (int i = 0; i < ribbons.Count; i++) {
	// 		if (i == index) {
	// 			ribbons [i].setState (true);
	// 		} else {
	// 			ribbons [i].setState (false);
	// 		}	
	// 	}	

		
	// }


}

