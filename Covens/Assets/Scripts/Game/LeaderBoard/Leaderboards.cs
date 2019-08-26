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

    private bool isPlayer = true;

    public Transform container;
    public GameObject prefab;


    private static LeaderboardData[] m_Players;
    private static LeaderboardData[] m_Covens;

    private static float m_LastRequestTime = Mathf.NegativeInfinity;
    //5 minutes cooldown to request the leaderboard from the server again
    private const float m_RequestCooldown = 60 * 5;

    void Awake()
    {
        Instance = this;
        transform.localScale = Vector3.zero;

        ////cache the leaderboard as soon as the scene initializes
        //GetLeaderboards(null, null, false);

    }

    public static void GetLeaderboards(System.Action<LeaderboardData[], LeaderboardData[]> onSuccess, System.Action<int> onFailure, bool showLoading = true)
    {
        if (m_Covens != null && m_Players != null && Time.unscaledTime - m_LastRequestTime < m_RequestCooldown)
        {
            onSuccess?.Invoke(m_Players, m_Covens);
            return;
        }

        m_LastRequestTime = Time.unscaledTime;

        if (showLoading && Instance != null)
            Instance.loading.SetActive(true);

        APIManager.Instance.GetRaincrow(
            "leaderboard",
            "",
            (string s, int r) => OnReceiveLeaderboard(s, r, onSuccess, onFailure)
        );
    }

    //cache the results and get invoke the callback
    private static void OnReceiveLeaderboard(string response, int result, System.Action<LeaderboardData[], LeaderboardData[]> onSuccess, System.Action<int> onFailure)
    {
        if (Instance != null)
            Instance.loading.SetActive(false);

        if (result == 200)
        {
            var LR = JsonConvert.DeserializeObject<LeaderboardRoot>(response);
            m_Players = LR.players.OrderBy(p => p.rank).ToArray();
            m_Covens = LR.covens.OrderBy(p => p.rank).ToArray();
            onSuccess?.Invoke(m_Players, m_Covens);
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
            LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 1, .45f).setEase(LeanTweenType.easeOutSine).setOnComplete(() =>
            {
                UIStateManager.Instance.CallWindowChanged(false);
                MapsAPI.Instance.HideMap(true);
            });
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
        UIStateManager.Instance.CallWindowChanged(true);
        MapsAPI.Instance.HideMap(false);
        LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0, .45f).setEase(LeanTweenType.easeOutSine);
        LeanTween.scale(gameObject, Vector3.zero, .45f).setEase(LeanTweenType.easeOutSine).setOnComplete(() =>
        {
            Destroy(gameObject);
        });
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
            title.text = LocalizeLookUp.GetText("leaderboard_player");
            if (m_Players != null)
            {
                //This is to change score text to level when on top players
                transform.GetChild(2).GetChild(1).GetChild(0).GetChild(3).GetComponent<Text>().text = LocalizeLookUp.GetText("spell_xp").Replace("{{Number}} ", "");
                for (int i = 0; i < m_Players.Length; i++)
                {
                    var g = Utilities.InstantiateObject(prefab, container);
                    g.GetComponent<LeaderboardItemData>().Setup(m_Players[i], true);
                }
            }
        }
        else
        {
            if (m_Covens != null)
            {
                //This is to change score text to score when on top covens
                transform.GetChild(2).GetChild(1).GetChild(0).GetChild(3).GetComponent<Text>().text = LocalizeLookUp.GetText("leaderboard_score");
                for (int i = 0; i < m_Covens.Length; i++)
                {
                    var g = Utilities.InstantiateObject(prefab, container);
                    g.GetComponent<LeaderboardItemData>().Setup(m_Covens[i], false);
                }
            }
            topPlayersButton.GetComponent<Text>().color = Color.gray;
            topCovensButton.GetComponent<Text>().color = Color.white;
            title.text = LocalizeLookUp.GetText("leaderboard_coven");
        }
    }

    public void OnClickPlayer(string playerName)
    {
        TeamPlayerView.ViewCharacter(playerName, (character, error) => { }, true);
    }

    public void OnClickCoven(string covenName)
    {
        Hide();
        TeamManagerUI.OpenName(covenName);
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

public struct LeaderboardRoot
{
    public List<LeaderboardData> players;
    public List<LeaderboardData> covens;
}

public struct LeaderboardData
{
    public string name;
    public int rank;
    public string dominion;
    //public int dominionRank;
    public double score;
}
