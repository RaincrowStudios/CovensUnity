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
    public Button btn;

    private bool isPlayer;

    public void Setup(LeaderboardData data, bool isPlayer)
    {
        Name.text = data.name;
        Score.text = data.score != 0 ? data.score.ToString() : data.score.ToString();
        Rank.text = data.rank.ToString();
        Dominion.text = $"{data.dominion}";// ({data.dominionRank})";
        bg.SetActive(data.rank % 2 == 0);
        this.isPlayer = isPlayer;

        if (btn != null)
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

