using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenSilverPurchase : MonoBehaviour {
	//public Button button;
	// Use this for initialization
	void Start () {
		
		GetComponent<Button>().onClick.AddListener (() => {
			ShopManager.Instance.Open ();
			ShopManager.Instance.ShowSilver ();
		});

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
