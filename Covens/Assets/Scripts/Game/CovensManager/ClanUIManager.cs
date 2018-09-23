using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json;

public class ClanUIManager : MonoBehaviour
{
	public Text clanTitle;
	public GameObject clanTitleBorder;
	public Text dominionTitle;
	APIManager API;
	// Use this for initialization
	void Start ()
	{
		API = APIManager.Instance;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void PlayerNoClan()
	{
		clanTitle.text = "No Coven";
		clanTitleBorder.SetActive (true);
		dominionTitle.text = "Requests";

		

	}
}



