using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BOSSpirit : BOSBase
{
    public static BOSSpirit instance { get; set; }
    [SerializeField] private GameObject spiritDeck;
    [SerializeField] private GameObject activeSpirits;
    [SerializeField] private GameObject activePortals;
    [SerializeField] private GameObject selectZone;
    [SerializeField] private GameObject header;

    [SerializeField] private TextMeshProUGUI deckButton;
    [SerializeField] private TextMeshProUGUI portalsButton;
    [SerializeField] private TextMeshProUGUI spiritsButton;


    public static List<SpiritData> activeSpiritsData = new List<SpiritData>();
    public static List<SpiritData> activePortalsData = new List<SpiritData>();
    public static int currentZone;
    public static int undiscoveredSpirits;
    public static int discoveredSpirits;

    private CanvasGroup CG;
    private GameObject currentObject;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        CG = GetComponent<CanvasGroup>();
        CG.alpha = 0;
        deckButton.GetComponent<Button>().onClick.AddListener(ShowSpiritDeck);
        portalsButton.GetComponent<Button>().onClick.AddListener(ShowActivePortals);
        spiritsButton.GetComponent<Button>().onClick.AddListener(ShowActiveSpirits);
        LeanTween.alphaCanvas(CG, 1, .7f);
        ShowSpiritDeck();
    }

    public void CheckDisableButton()
    {
        if (activeSpiritsData.Count == 0)
            spiritsButton.GetComponent<Button>().enabled = false;
        else
            spiritsButton.GetComponent<Button>().enabled = true;

        if (activePortalsData.Count == 0)
            portalsButton.GetComponent<Button>().enabled = false;
        else
            portalsButton.GetComponent<Button>().enabled = true;
    }

    public void ShowSelectedZone()
    {
        header.SetActive(false);
        DestroyPrevious(currentObject);
        currentObject = CreateScreen(selectZone);
    }

    public void ShowSpiritDeck()
    {
        SetButtons(0);
        DestroyPrevious(currentObject);
        currentObject = CreateScreen(spiritDeck);
    }

    void ShowActiveSpirits()
    {
        SetButtons(1);
        DestroyPrevious(currentObject);
        currentObject = CreateScreen(activeSpirits);
    }

    void ShowActivePortals()
    {
        SetButtons(2);
        DestroyPrevious(currentObject);
        currentObject = CreateScreen(activePortals);
    }

    void SetButtons(int index)
    {
        header.SetActive(true);
        deckButton.fontStyle = spiritsButton.fontStyle = portalsButton.fontStyle = FontStyles.Normal;
        deckButton.color = spiritsButton.color = portalsButton.color = new Color(0, 0, 0, .55f);
        if (index == 0)
        {
            deckButton.fontStyle = FontStyles.Underline;
            deckButton.color = new Color(0, 0, 0, 1);
        }
        else if (index == 1)
        {
            spiritsButton.fontStyle = FontStyles.Underline;
            spiritsButton.color = new Color(0, 0, 0, 1);
        }
        else
        {
            portalsButton.fontStyle = FontStyles.Underline;
            portalsButton.color = new Color(0, 0, 0, 1);
        }
    }
}