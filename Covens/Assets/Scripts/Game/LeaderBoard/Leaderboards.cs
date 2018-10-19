using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class Leaderboards : UIAnimationManager {
	public static Leaderboards Instance { get; set; }
	public Button topCovensButton;
	public Button topPlayersButton;

	public Text title;

	public GameObject loading;
	public Animator anim;
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
			}	
		});		
	}

	public void Show()
	{
		anim.gameObject.SetActive (true);
		anim.Play ("in");
	}

	public void Hide()
	{
		anim.Play ("out");
		Disable (anim.gameObject, 1f);
	}



}


