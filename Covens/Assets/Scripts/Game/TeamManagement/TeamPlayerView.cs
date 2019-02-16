using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamPlayerView : MonoBehaviour
{
    public static TeamPlayerView Instance { get; set; }
    public GameObject WitchCard;
    [SerializeField] private TextMeshProUGUI _displayName;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _degree;
    [SerializeField] private TextMeshProUGUI _coven;
    [SerializeField] private TextMeshProUGUI _state;
    [SerializeField] private TextMeshProUGUI _dominion;
    [SerializeField] private TextMeshProUGUI _dominionRank;
    [SerializeField] private TextMeshProUGUI _worldRank;
    public Image schoolSigil;
    public Sprite whiteSchool;
    public Sprite shadowSchool;
    public Sprite greySchool;
    [SerializeField] private TextMeshProUGUI _energy;
    public Button flyToPlayerBtn;
    public ApparelView male;
    public ApparelView female;
    Vector2 playerPos = Vector2.zero;
    public CanvasGroup canvasGroup;
    public Button btnBack;

    void Awake()
    {
        Instance = this;
        btnBack.onClick.AddListener(Close);
    }

    public void Setup(MarkerDataDetail data)
    {
        canvasGroup.alpha = 0;
        WitchCard.SetActive(true);
        WitchCard.GetComponent<RectTransform>().localScale = Vector2.zero;
        LTDescr descrAlpha = LeanTween.alphaCanvas(canvasGroup, 1, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(WitchCard.GetComponent<RectTransform>(), Vector3.one, .4f).setEase(LeanTweenType.easeInOutSine);

        playerPos.x = data.longitude;
        playerPos.y = data.latitude;
        if (data.equipped[0].id.Contains("_m_"))
        {
            female.gameObject.SetActive(false);
            male.gameObject.SetActive(true);
            male.InitializeChar(data.equipped);
        }
        else
        {
            female.gameObject.SetActive(true);
            male.gameObject.SetActive(false);
            female.InitializeChar(data.equipped);
        }
        ChangeDegree(data.degree);
        _displayName.text = data.displayName;
        _level.text = "Level: " + data.level.ToString();
        _dominion.text = "Dominion: " + data.dominion;
        _dominionRank.text = "Dominion Rank: " + data.dominionRank;
        _worldRank.text = "World Rank: " + data.worldRank;
        _coven.text = (data.covenName == "" ? "Coven: None" : "Coven: " + data.covenName);
        _state.text = (data.state == "" ? "State: Normal" : "State: " + data.state);
        _energy.text = "Energy: " + data.energy.ToString();
        flyToPlayerBtn.onClick.AddListener(FlyToPlayer);
        flyToPlayerBtn.gameObject.SetActive(data.covenName == PlayerDataManager.playerData.covenName);
    }

    void ChangeDegree(int Degree)
    {
        _degree.text = Utilities.witchTypeControlSmallCaps(Degree);
        if (Degree < 0)
        {
            schoolSigil.sprite = shadowSchool;
            schoolSigil.color = Utilities.Purple;
        }
        else if (Degree > 0)
        {
            schoolSigil.sprite = whiteSchool;
            schoolSigil.color = Utilities.Orange;
        }
        else
        {
            schoolSigil.sprite = greySchool;
            schoolSigil.color = Utilities.Blue;
        }
    }

    public void FlyToPlayer()
    {
        if (PlayerDataManager.playerData.energy == 0)
            return;

        PlayerManager.Instance.Fly();
        MapsAPI.Instance.SetPosition(playerPos.x, playerPos.y);
        PlayerManager.inSpiritForm = false;
        PlayerManager.Instance.Fly();
        TeamManagerUI.Instance.Close();
        Close();
    }

    public void Close()
    {
        LTDescr descrAlpha = LeanTween.alphaCanvas(canvasGroup, 0, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(WitchCard.GetComponent<RectTransform>(), Vector3.zero, .4f).setEase(LeanTweenType.easeInOutSine);
        descrScale.setOnComplete(() => { WitchCard.SetActive(false); });
    }
}