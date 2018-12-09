using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;


public class TeamPlayerView : MonoBehaviour
{
    public static TeamPlayerView Instance { get; set; }
    public GameObject WitchCard;
    public Text displayName;
    public Text level;
    public Text degree;
    public Text coven;
    public Text state;
    public Text dominion;
    public Text dominionRank;
    public Text worldRank;
    public Image schoolSigil;
    public Sprite whiteSchool;
    public Sprite shadowSchool;
    public Sprite greySchool;
    public Text energy;
    public Button flyToPlayerBtn;
    public ApparelView male;
    public ApparelView female;
    Vector2 playerPos = Vector2.zero;
    public CanvasGroup canvasGroup;

    void Awake()
    {
        Instance = this;
    }

    public void Setup(MarkerDataDetail data)
    {
        canvasGroup.alpha = 0;
        WitchCard.SetActive(true);
        WitchCard.GetComponent<RectTransform>().localScale = Vector2.zero;
        LTDescr descrAlpha = LeanTween.alphaCanvas(canvasGroup, 1, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(WitchCard.GetComponent<RectTransform>(), Vector2.one, .4f).setEase(LeanTweenType.easeInOutSine);

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
        displayName.text = data.displayName;
        level.text = "Level: " + data.level.ToString();
        dominion.text = "Dominion: " + data.dominion;
        dominionRank.text = "Dominion Rank: " + data.dominionRank;
        worldRank.text = "World Rank: " + data.worldRank;
        coven.text = (data.covenName == "" ? "Coven: None" : "Coven: " + data.covenName);
        state.text = (data.state == "" ? "State: Normal" : "State: " + data.state);
        energy.text = "Energy: " + data.energy.ToString();
        flyToPlayerBtn.onClick.AddListener(FlyToPlayer);
        flyToPlayerBtn.gameObject.SetActive(data.covenName == PlayerDataManager.playerData.covenName);
    }

    void ChangeDegree(int Degree)
    {
        degree.text = Utilities.witchTypeControlSmallCaps(Degree);
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
        OnlineMaps.instance.SetPosition(playerPos.x, playerPos.y);
        PlayerManager.inSpiritForm = false;
        PlayerManager.Instance.Fly();
        TeamManagerUI.Instance.Close();
    }

    public void Close()
    {
        LTDescr descrAlpha = LeanTween.alphaCanvas(canvasGroup, 0, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(WitchCard.GetComponent<RectTransform>(), Vector3.zero, .4f).setEase(LeanTweenType.easeInOutSine);
        descrScale.setOnComplete(() => { WitchCard.SetActive(false); });
    }
}