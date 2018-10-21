using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeaderboardItemData : MonoBehaviour
{
	public Text Name;
	public Text Score;
	public Text Rank;
	public Text Dominion;
	public GameObject bg;

	public void Setup(LeaderboardData data, int rank){
		rank++;
		Name.text = data.displayName;
		Score.text = data.score.ToString();
		Rank.text = rank.ToString(); 
		Dominion.text = data.dominion;
		bg.SetActive (rank % 2 == 0);
	}
}

