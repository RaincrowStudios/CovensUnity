using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;


public class TeamPlayerView : MonoBehaviour
{
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

    public void Show(string playerID, bool canFly)
    {
        var data = new { target = playerID };
        APIManager.Instance.PostData("display/character", JsonConvert.SerializeObject(data), (string s, int r) =>
        {
            if (r == 200)
            {
                Setup(JsonConvert.DeserializeObject<MarkerDataDetail>(s), canFly);
            }
        });
    }

    void Setup(MarkerDataDetail data, bool canFly)
    {
        canvasGroup.alpha = 0;
        GetComponent<RectTransform>().localScale = Vector2.zero;
        LTDescr descrAlpha = LeanTween.alphaCanvas(canvasGroup, 1, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(GetComponent<RectTransform>(), Vector2.one, .4f).setEase(LeanTweenType.easeInOutSine);

        WitchCard.SetActive(true);
        playerPos.x = data.longitude;
        playerPos.y = data.latitude;
        if (MarkerSpawner.SelectedMarker.equipped[0].id.Contains("_m_"))
        {
            female.gameObject.SetActive(false);
            male.gameObject.SetActive(true);
            male.InitializeChar(MarkerSpawner.SelectedMarker.equipped);
        }
        else
        {
            female.gameObject.SetActive(true);
            male.gameObject.SetActive(false);
            female.InitializeChar(MarkerSpawner.SelectedMarker.equipped);
        }
        ChangeDegree();
        displayName.text = data.displayName;
        level.text = "Level: " + data.level.ToString();
        dominion.text = "Dominion: " + data.dominion;
        dominionRank.text = "Dominion Rank: " + data.dominionRank;
        worldRank.text = "World Rank: " + data.worldRank;
        coven.text = (data.coven == "" ? "None" : data.coven);
        state.text = (data.state == "" ? "Normal" : data.state);
        energy.text = "Energy: " + data.energy.ToString();
        flyToPlayerBtn.onClick.AddListener(FlyToPlayer);
        flyToPlayerBtn.gameObject.SetActive(canFly);
    }

    void ChangeDegree()
    {
        degree.text = Utilities.witchTypeControlSmallCaps(MarkerSpawner.SelectedMarker.degree);
        if (MarkerSpawner.SelectedMarker.degree < 0)
        {
            schoolSigil.sprite = shadowSchool;
            schoolSigil.color = Utilities.Purple;
        }
        else if (MarkerSpawner.SelectedMarker.degree > 0)
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
        LTDescr descrScale = LeanTween.scale(GetComponent<RectTransform>(), Vector3.zero, .4f).setEase(LeanTweenType.easeInOutSine);
        descrScale.setOnComplete(() => { gameObject.SetActive(false); });
    }
}