﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeaderboardItemData : MonoBehaviour
{
	public Text Name;
	public Text Score;
	public Text Rank;
	public Text Dominion;
	public GameObject bg;
    public Button btn;

    private bool isPlayer;

	public void Setup(LeaderboardData data, int rank, bool isPlayer)
    {
		rank++;
		Name.text = data.displayName;
		Score.text = data.score.ToString();
		Rank.text = rank.ToString(); 
		Dominion.text = data.dominion;
		bg.SetActive (rank % 2 == 0);
        this.isPlayer = isPlayer;

        if(btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
        }
	}

    private void OnClick()
    {
        if (isPlayer)
        {
            Leaderboards.Instance.OnClickPlayer(Name.text);
        }
        else
        {
            Leaderboards.Instance.OnClickCoven(Name.text);
        }
    }
}

