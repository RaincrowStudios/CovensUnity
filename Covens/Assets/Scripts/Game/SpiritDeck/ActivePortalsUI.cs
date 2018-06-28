using UnityEngine;
using System.Collections;
using System.Linq;

public class ActivePortalsUI : MonoBehaviour
{
	public GameObject[] card;
	public Transform container;
	public GameObject cardCollider;
	public int totalPortals = 10;

	public GameObject[] turnOff;
	public GameObject[] turnOn;
	void Start()
	{
		Invoke ("SpawnCards", 2f);
	}
	void SpawnCards()
	{
		for (int i = 0; i < totalPortals; i++) {
			SpawnCard (i);
		}
		Invoke ("enableObject", 1f);

	}

	void SpawnCard (int i)
	{
		var g = Utilities.InstantiateObject (card[Random.Range(1,2)], container);
		PortalSetup setupCard = g.GetComponent<PortalSetup> (); 
		setupCard.id = DownloadedAssets.spiritArt.ElementAt (i).Key; 
		setupCard.spiritIcon.sprite = DownloadedAssets.spiritArt [setupCard.id];
	}
	void enableObject()
	{
		cardCollider.SetActive (true);
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
}

