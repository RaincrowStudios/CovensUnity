using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamCovenView : MonoBehaviour
{
    public static TeamCovenView Instance { get; private set; }


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
    public Button btnMotto;

    void Awake()
    {
        Instance = this;
        btnViewPOP.onClick.AddListener(() => TeamManagerUI.Instance.SetScreenType(TeamManagerUI.ScreenType.Locations));
        btnMotto.onClick.AddListener(() => TeamManagerUI.Instance.ChangeMotto());

        covenMotto.text = "";
        founder.text = "";
        worldRank.text = "";
        dominionRank.text = "";
        createdOn.text = "";
        POPControlled.text = "";
        covenType.text = "";
        creatorType.text = "";
    }

    public void Show(TeamData data)
    {
        canvasGroup.alpha = 0;
        container.SetActive(true);
        container.GetComponent<RectTransform>().localScale = Vector3.one;// Vector2.zero;
        LTDescr descrAlpha = LeanTween.alphaCanvas(canvasGroup, 1, .28f).setEase(LeanTweenType.easeInOutSine);
        //LTDescr descrScale = LeanTween.scale(container.GetComponent<RectTransform>(), Vector3.one, .4f).setEase(LeanTweenType.easeInOutSine);
        try
        {
			founder.text = LocalizeLookUp.GetText("coven_founder")/*"Founder:*/ + " " + data.createdBy;
			createdOn.text = LocalizeLookUp.GetText("coven_creation")/*"Created On:*/ + " " + TeamManagerUI.GetTimeStamp(data.createdOn);
			POPControlled.text = LocalizeLookUp.GetText("coven_pop_controlled")/*"Places of power controlled:*/ + " " + data.controlledLocations.Length;
			worldRank.text = LocalizeLookUp.GetText("lt_world_rank")/*"World Rank:*/ + " " + data.rank.ToString();
			dominionRank.text = LocalizeLookUp.GetText("lt_dominion_rank")/*"Dominion Rank:*/ + " " + data.dominionRank.ToString();
            btnViewPOP.gameObject.SetActive(data.controlledLocations.Length > 0);
            SetMotto(data);

            int covenDegree = data.Degree;
            int creatorDegree = data.CreatorDegree;
            SetDegreeCoven(covenDegree);
            creatorType.text = Utilities.witchTypeControlSmallCaps(creatorDegree);
            SetDegree(creatorDegree, PlayerSigil);
        }
        catch (System.Exception)
        {

        }

    }

    void SetDegreeCoven(int degree)
    {
        if (degree < 0)
        {
			covenType.text = " " + LocalizeLookUp.GetText ("coven_shadow");//" Shadow Coven";
        }
        else if (degree > 0)
        {
			covenType.text = " " + LocalizeLookUp.GetText ("coven_white");//" White Coven";
        }
        else
        {
			covenType.text = " " + LocalizeLookUp.GetText ("coven_grey");//"Grey Coven";
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

    public void SetMotto(TeamData data)
    {
        if (data.covenName == PlayerDataManager.playerData.covenName)
        {
			covenMotto.text = string.IsNullOrEmpty(data.motto) ? "\"" + LocalizeLookUp.GetText("coven_motto_here") + "\"" : "\"" + data.motto + "\"";
            btnMotto.interactable = TeamManager.CurrentRole >= TeamManager.CovenRole.Administrator;
        }
        else
        {
            covenMotto.text = string.IsNullOrEmpty(data.motto) ? "" : "\"" + data.motto + "\"";
            btnMotto.interactable = false;
        }
    }
}