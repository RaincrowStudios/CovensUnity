using UnityEngine;
using UnityEngine.UI;

public class TeamCovenView : MonoBehaviour
{
    public static TeamCovenView Instance { get; set; }


    public GameObject container;
    //public GameObject MainlistView;
    public Text covenName;
    public Text covenDominion;
    public Text covenMotto;
    public Text founder;
    public Text worldRank;
    public Text dominionRank;
    public Text createdOn;
    public Text POPControlled;
    public Text covenType;
    public Text creatorType;
    public Sprite whiteSchool;
    public Sprite shadowSchool;
    public Sprite greySchool;
    public Image CovenSigil;
    public Image PlayerSigil;
    public Button btnClose;
    public CanvasGroup canvasGroup;

    void Awake()
    {
        Instance = this;
        btnClose.onClick.AddListener(Close);
    }

    public void Show(TeamData data)
    {
        canvasGroup.alpha = 0;
        container.SetActive(true);
        container.GetComponent<RectTransform>().localScale = Vector2.zero;
        LTDescr descrAlpha = LeanTween.alphaCanvas(canvasGroup, 1, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(container.GetComponent<RectTransform>(), Vector2.one, .4f).setEase(LeanTweenType.easeInOutSine);

        covenName.text = data.covenName;
        covenDominion.text = data.dominion;
        covenMotto.text = data.motto;
        founder.text = "Founder: " + data.createdBy;
        createdOn.text = "Created On: " + TeamManagerUI.GetTimeStamp(data.createdOn);
        POPControlled.text = "Places of power controllerd: " + data.controlledLocations.Length;
        worldRank.text = "World Rank: " + data.rank.ToString();
        dominionRank.text = "Dominion Rank: " + data.dominionRank.ToString();
        SetDegreeCoven(data.covenDegree);
        creatorType.text = Utilities.witchTypeControlSmallCaps(data.creatorDegree);
        SetDegree(data.creatorDegree, PlayerSigil);
    }

    void SetDegreeCoven(int degree)
    {
        if (degree < 0)
        {
            covenType.text = " Shadow Coven";
        }
        else if (degree > 0)
        {
            covenType.text = " White Coven";
        }
        else
        {
            covenType.text = "Grey Coven";
        }
        SetDegree(degree, CovenSigil);
    }

    void SetDegree(int degree, Image sigil)
    {
        if (degree < 0)
        {
            sigil.sprite = shadowSchool;
            sigil.color = Utilities.Purple;
        }
        else if (degree > 0)
        {
            sigil.sprite = whiteSchool;
            sigil.color = Utilities.Orange;
        }
        else
        {
            sigil.sprite = greySchool;
            sigil.color = Utilities.Blue;
        }
    }



    public void Close()
    {
        LTDescr descrAlpha = LeanTween.alphaCanvas(canvasGroup, 0, .28f).setEase(LeanTweenType.easeInOutSine);
        LTDescr descrScale = LeanTween.scale(container.GetComponent<RectTransform>(), Vector3.zero, .4f).setEase(LeanTweenType.easeInOutSine);
        descrScale.setOnComplete(() => { container.SetActive(false); });
    }

    public bool IsVisible { get { return container.gameObject.activeSelf; } }
}