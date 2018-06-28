using UnityEngine;
using System.Collections;
using System.Linq;

public class ActiveSpiritUI : MonoBehaviour
{
	public GameObject card;
	public int TotalCards = 20;
	public Transform container;
	public GameObject cardCollider;

	public GameObject[] turnOff;
	public GameObject[] turnOn;

	void Start()
	{
		Invoke ("SpawnCards", 2f);
	}
	void SpawnCards()
	{
		for (int i = 0; i < TotalCards; i++) {
			SpawnCard (i);
		}
		Invoke ("enableObject", 1f);

	}

	void SpawnCard (int i)
	{
		var g = Utilities.InstantiateObject (card, container);
		var setupCard = g.GetComponent<SetupSpiritCard> ();
		setupCard.SetupCard (DownloadedAssets.spiritArt.ElementAt (i).Key, "Latin America", Random.Range (1, 5), DownloadedAssets.spiritArt.ElementAt (i).Key);
	}

	public void OnClick()
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
	}
}

