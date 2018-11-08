using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Linq;
public class Leaderboards : UIAnimationManager {
	public static Leaderboards Instance { get; set; }
	public Button topCovensButton;
	public Button topPlayersButton;

	public Text title;

	public GameObject loading;
	public Animator anim;

	bool isPlayer = true;

	public Transform container;
	public GameObject prefab;
	public List<LeaderboardData> players;
	public List<LeaderboardData> covens;
	void Awake()
	{
		Instance = this;
	}

	public void GetLeaderboards()
	{
		loading.SetActive (true);
		APIManager.Instance.GetData ("/leaderboards/get", (string s, int r) => {
			loading.SetActive(false);
			if(r == 200){
				print(s);
	 			var	LR = JsonConvert.DeserializeObject<LeaderboardRoot>(s); 
				players = LR.witch.OrderBy(p=> p.score).ToList();  
				players.Reverse();
				covens = LR.coven.OrderBy(p=> p.score).ToList();  
				covens.Reverse();
				Show();
			}	else{
				print(s);
			}
		});		
	}

	public void Show()
	{
		anim.gameObject.SetActive (true);
		anim.Play ("in");
		SetupUI ();
	}

	public void Hide()
	{
		anim.Play ("out");
		foreach (Transform item in container) {
			Destroy (item.gameObject);
		}
		Disable (anim.gameObject, 1f);
	}

	public void SetupUI(){
		foreach (Transform item in container) {
			Destroy (item.gameObject);
		}
		if (isPlayer) { 
			topPlayersButton.GetComponent<Text>().color = Color.white;	
			topCovensButton.GetComponent<Text>().color = Color.gray;
			title.text = "Playername";
			for (int i = 0; i < players.Count; i++) {
				var g = Utilities.InstantiateObject (prefab, container);
				g.GetComponent<LeaderboardItemData> ().Setup (players [i], i);
			}
		} else {
			for (int i = 0; i < covens.Count; i++) {
				var g = Utilities.InstantiateObject (prefab, container);
				g.GetComponent<LeaderboardItemData> ().Setup (covens [i], i);
			}
			topPlayersButton.GetComponent<Text>().color = Color.gray;	
			topCovensButton.GetComponent<Text>().color = Color.white;	
			title.text = "Coven";
		}
	}

	public void ToggleList(bool player)
	{
		if (player != isPlayer) {
			isPlayer = player;
			SetupUI ();
		}
	}
}

public class LeaderboardRoot {
	public List<LeaderboardData> witch { get; set;}
	public List<LeaderboardData> coven { get; set;}
}

public class LeaderboardData{
	public string displayName{ get; set;}
	public string dominion{ get; set; }
	public int score{ get; set;}
}
