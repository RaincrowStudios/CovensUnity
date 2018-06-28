using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class SpiritDeckUIManager : MonoBehaviour {
	public GameObject card;
	public GameObject emptyCard;
	public GameObject undiscoveredCard;
	public Transform container;
	public GameObject cardCollider;
	public GameObject[] turnOff;
	public GameObject[] turnOn;
	public int TotalCards = 10;
	
	void Start () {
		cardCollider.SetActive (false);

		var e1 = Utilities.InstantiateObject (emptyCard, container);
		var e2 = Utilities.InstantiateObject (emptyCard, container);

		if (TotalCards > 4) {
			for (int i = 0; i < TotalCards; i++) {
				SpawnCard (i);
			}
		} else if(TotalCards == 4){
			for (int i = 0; i < TotalCards; i++) {
				SpawnCard (i);
			}
			var u1 = Utilities.InstantiateObject (undiscoveredCard, container);
		} else if(TotalCards == 3){
			var u1 = Utilities.InstantiateObject (undiscoveredCard, container);
			for (int i = 0; i < TotalCards; i++) {
				SpawnCard (i);
			}
			var u2 = Utilities.InstantiateObject (undiscoveredCard, container);
		}  else if(TotalCards == 2){
			var u1 = Utilities.InstantiateObject (undiscoveredCard, container);
			for (int i = 0; i < TotalCards; i++) {
				SpawnCard (i);
			}
			var u2 = Utilities.InstantiateObject (undiscoveredCard, container);
			var u3 = Utilities.InstantiateObject (undiscoveredCard, container);
		} else if(TotalCards == 1){
			var u1 = Utilities.InstantiateObject (undiscoveredCard, container);
			var u4 = Utilities.InstantiateObject (undiscoveredCard, container);
			SpawnCard (0);
			var u2 = Utilities.InstantiateObject (undiscoveredCard, container);
			var u3 = Utilities.InstantiateObject (undiscoveredCard, container);
		}
		var e3 = Utilities.InstantiateObject (emptyCard, container);
		var e4 = Utilities.InstantiateObject (emptyCard, container);
		Invoke ("enableObject", .5f);

	}

	void SpawnCard (int i)
	{
		var g = Utilities.InstantiateObject (card, container);
		var setupCard = g.GetComponent<SetupSpiritCard> ();
		setupCard.SetupCard (DownloadedAssets.spiritArt.ElementAt (i).Key, "Latin America", Random.Range (1, 5), DownloadedAssets.spiritArt.ElementAt (i).Key);
	}

	public void OnClickKnown()
	{
		foreach (var item in turnOn) {
			item.SetActive (true);
		}

		foreach (var item in turnOff) {
			item.SetActive (false);
		}
	}

	void enableObject()
	{
		cardCollider.SetActive (true);
		OnClickKnown ();
	}


}
