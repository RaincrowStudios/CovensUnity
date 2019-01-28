using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamCovenView : MonoBehaviour
{
    public static TeamCovenView Instance { get; set; }


    public GameObject container;
    public TextMeshProUGUI covenMotto;
    public TextMeshProUGUI founder;
    public TextMeshProUGUI worldRank;
    public TextMeshProUGUI dominionRank;
    public TextMeshProUGUI createdOn;
    public TextMeshProUGUI POPControlled;
    public TextMeshProUGUI covenType;
    public TextMeshProUGUI creatorType;
    public Sprite whiteSchool;
    public Sprite shadowSchool;
    public Sprite greySchool;
    public Image CovenSigil;
    public Image PlayerSigil;
    public Button btnViewPOP;
    public CanvasGroup canvasGroup;

    void Awake()
    {
        Instance = this;
        btnViewPOP.onClick.AddListener(() => TeamManagerUI.Instance.SetScreenType(TeamManagerUI.ScreenType.Locations));
    }

    public void Show(TeamData data)
    {
        canvasGroup.alpha = 0;
        container.SetActive(true);
        container.GetComponent<RectTransform>().localScale = Vector2.one;// Vector2.zero;
        LTDescr descrAlpha = LeanTween.alphaCanvas(canvasGroup, 1, .28f).setEase(LeanTweenType.easeInOutSine);
        //LTDescr descrScale = LeanTween.scale(container.GetComponent<RectTransform>(), Vector2.one, .4f).setEase(LeanTweenType.easeInOutSine);

        covenMotto.text = string.IsNullOrEmpty(data.motto) ? "" : "\"" + data.motto + "\"";
        founder.text = "Founder: " + data.createdBy;
        createdOn.text = "Created On: " + TeamManagerUI.GetTimeStamp(data.createdOn);
        POPControlled.text = "Places of power controllerd: " + data.controlledLocations.Length;
        worldRank.text = "World Rank: " + data.rank.ToString();
        dominionRank.text = "Dominion Rank: " + data.dominionRank.ToString();
        btnViewPOP.gameObject.SetActive(data.controlledLocations.Length > 0);

        int covenDegree = data.Degree;
        int creatorDegree = data.CreatorDegree;
        SetDegreeCoven(covenDegree);
        creatorType.text = Utilities.witchTypeControlSmallCaps(creatorDegree);
        SetDegree(creatorDegree, PlayerSigil);
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
        //LTDescr descrScale = LeanTween.scale(container.GetComponent<RectTransform>(), Vector3.zero, .4f).setEase(LeanTweenType.easeInOutSine);
        descrAlpha.setOnComplete(() => { container.SetActive(false); });
    }

    public bool IsOpen { get { return container.gameObject.activeSelf; } }
}