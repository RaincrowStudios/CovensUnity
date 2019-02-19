using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Linq;

//suggestion: use a cursor to get leaderboard pages
public class Leaderboards : UIAnimationManager
{
    public static Leaderboards Instance { get; set; }
    public Button topCovensButton;
    public Button topPlayersButton;
    public Button closeButton;

    public Text title;

    public GameObject loading;
    public GameObject loadingFullscreen;

    private bool isPlayer = true;

    public Transform container;
    public GameObject prefab;
    public LeaderboardData[] players { get; private set; }
    public LeaderboardData[] covens { get; private set; }

    private double lastRequestTime = Mathf.NegativeInfinity;
    //5 minutes cooldown to request the leaderboard from the server again
    private double requestCooldown = 60 * 5;

    void Awake()
    {
        loadingFullscreen.SetActive(false);
        Instance = this;
        transform.localScale = Vector3.zero;
        //cache the leaderboard as soon as the scene initializes
        GetLeaderboards(null, null, false);

    }

    public void GetLeaderboards(System.Action<LeaderboardData[], LeaderboardData[]> onSuccess, System.Action<int> onFailure, bool showLoading = true)
    {
        if (covens != null && players != null && Time.unscaledTime - lastRequestTime < requestCooldown)
        {
            onSuccess?.Invoke(players, covens);
            return;
        }

        lastRequestTime = Time.unscaledTime;

        if (showLoading)
            loading.SetActive(true);

        APIManager.Instance.GetData(
            "leaderboards/get",
            (string s, int r) => OnReceiveLeaderboard(s, r, onSuccess, onFailure)
        );
    }

    //cache the results and get invoke the callback
    private void OnReceiveLeaderboard(string response, int result, System.Action<LeaderboardData[], LeaderboardData[]> onSuccess, System.Action<int> onFailure)
    {
        loading.SetActive(false);

        if (result == 200)
        {
            var LR = JsonConvert.DeserializeObject<LeaderboardRoot>(response);
            players = LR.witch.OrderBy(p => p.score).Reverse().ToArray();
            covens = LR.coven.OrderBy(p => p.score).Reverse().ToArray();
            onSuccess?.Invoke(players, covens);
        }
        else
        {
            Debug.LogError($"[{result}] {response}");
            onFailure?.Invoke(result);
        }
    }


    void Start()
    {
        topCovensButton.onClick.AddListener(() => { isPlayer = false; Show(); });
        topPlayersButton.onClick.AddListener(() => { isPlayer = true; Show(); });
        closeButton.onClick.AddListener(Hide);
        GetComponent<CanvasGroup>().alpha = 0;
        transform.localScale = Vector3.zero;
        Show();
    }
    public void ShowCovens()
    {
        isPlayer = false;
        Show();
    }
    public void Show()
    {
        if (transform.localScale.x != 1)
        {
            LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1, .45f).setEase(LeanTweenType.easeOutSine);
            LeanTween.scale(gameObject, Vector3.one, .45f).setEase(LeanTweenType.easeOutSine);
        }
        GetLeaderboards(
            onSuccess: (players, covens) =>
            {
                SetupUI();
            },
            onFailure: (errorCode) =>
            {

            }
        );
    }

    void Hide()
    {
        LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0, .45f).setEase(LeanTweenType.easeOutSine);
        LeanTween.scale(gameObject, Vector3.zero, .45f).setEase(LeanTweenType.easeOutSine).setOnComplete(() => { Destroy(gameObject); });
    }



    public void SetupUI()
    {
        foreach (Transform item in container)
        {
            Destroy(item.gameObject);
        }
        if (isPlayer)
        {
            topPlayersButton.GetComponent<Text>().color = Color.white;
            topCovensButton.GetComponent<Text>().color = Color.gray;
            title.text = "Playername";
            if (players != null)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    var g = Utilities.InstantiateObject(prefab, container);
                    g.GetComponent<LeaderboardItemData>().Setup(players[i], i, true);
                }
            }
        }
        else
        {
            if (covens != null)
            {
                for (int i = 0; i < covens.Length; i++)
                {
                    var g = Utilities.InstantiateObject(prefab, container);
                    g.GetComponent<LeaderboardItemData>().Setup(covens[i], i, false);
                }
            }
            topPlayersButton.GetComponent<Text>().color = Color.gray;
            topCovensButton.GetComponent<Text>().color = Color.white;
            title.text = "Coven";
        }
    }

    public void OnClickPlayer(string playerName)
    {
        loadingFullscreen.SetActive(true);
        TeamManager.ViewCharacter(playerName,
            (character, resultCode) =>
            {
                if (resultCode == 200)
                {
                    TeamPlayerView.Instance.Setup(character);
                }
                loadingFullscreen.SetActive(false);
            });
    }

    public void OnClickCoven(string covenName)
    {
        Hide();
        TeamManagerUI.Instance.Show(covenName);
    }

    public void ToggleList(bool player)
    {
        if (player != isPlayer)
        {
            isPlayer = player;
            SetupUI();
        }
    }
}

public class LeaderboardRoot
{
    public List<LeaderboardData> witch { get; set; }
    public List<LeaderboardData> coven { get; set; }
}

public class LeaderboardData
{
    public string displayName { get; set; }
    public string dominion { get; set; }
    public int score { get; set; }
}
