using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BOSSelectZone : BOSBase
{
    [SerializeField] private TextMeshProUGUI undiscoveredButton;
    [SerializeField] private TextMeshProUGUI discoveredButton;
    [SerializeField] private TextMeshProUGUI spiritCountInfo;
    [SerializeField] private TextMeshProUGUI spawnRegion;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject undiscoveredCard;
    [SerializeField] private GameObject discoveredCard;
    [SerializeField] private Transform container;
    public CanvasGroup image;
    public CanvasGroup image1;





    void Start()
    {
        this.GetComponent<CanvasGroup>().alpha = 0f;
        LeanTween.alphaCanvas(this.GetComponent<CanvasGroup>(), 1f, 0.3f);
        spawnRegion.text = LocalizeLookUp.GetText("ftf_spawn_region").Replace("{{region}}", LocalizeLookUp.GetZoneName(BOSSpirit.currentZone));

        if (BOSSpirit.discoveredSpirits == 0)
        {
            discoveredButton.color = new Color(0, 0, 0, .55f);
            undiscoveredButton.fontStyle = FontStyles.Underline;
            undiscoveredButton.color = new Color(0, 0, 0, 1);
            showUndiscoveredSpirits();
        }
        else
        {
            undiscoveredButton.GetComponent<Button>().onClick.AddListener(showUndiscoveredSpirits);
            discoveredButton.GetComponent<Button>().onClick.AddListener(showDiscoveredSpirits);
            showDiscoveredSpirits();
        }

        //  backButton.onClick.AddListener(BOSSpirit.instance.ShowSpiritDeck);
    }

    void showUndiscoveredSpirits()
    {
        SetButtons(true);
        for (int i = 0; i < BOSSpirit.undiscoveredSpirits; i++)
        {
            var g = Utilities.InstantiateObject(undiscoveredCard, container);
        }
        spiritCountInfo.text = BOSSpirit.undiscoveredSpirits.ToString() + " " + LocalizeLookUp.GetText("spirit_undiscovered");// " Undiscovered Spirits";
    }

    void showDiscoveredSpirits()
    {
        SetButtons(false);
        SpiritData spirit;
        foreach (var item in PlayerDataManager.playerData.knownSpirits)
        {
            spirit = DownloadedAssets.GetSpirit(item.spirit);
            if (spirit.zones.Contains(BOSSpirit.currentZone))
            {
                var g = Utilities.InstantiateObject(discoveredCard, container).transform;
                g.GetChild(1).GetComponent<TextMeshProUGUI>().text = spirit.tier.ToString();
                g.GetChild(2).GetComponent<TextMeshProUGUI>().text = spirit.Name;
                DownloadedAssets.GetSprite(item.spirit, g.GetChild(3).GetComponent<Image>());
                g.GetChild(4).GetComponent<TextMeshProUGUI>().text = item.dominion + ",\n" + Utilities.GetTimeStampBOS(item.banishedOn);
            }
        }
        spiritCountInfo.text = BOSSpirit.discoveredSpirits.ToString() + " " + LocalizeLookUp.GetText("spirit_discovered");// Discovered Spirits";

    }

    void SetButtons(bool undiscovered)
    {
        foreach (Transform item in container)
        {
            //LeanTween.alphaCanvas (item.gameObject.GetComponent<CanvasGroup> (), 0f, 0.3f).setOnComplete (() => {
            Destroy(item.gameObject);
            //});
        }
        undiscoveredButton.fontStyle = discoveredButton.fontStyle = FontStyles.Normal;
        undiscoveredButton.color = discoveredButton.color = new Color(0, 0, 0, .55f);
        if (undiscovered)
        {
            LeanTween.alphaCanvas(image1, 1f, 0.3f);
            LeanTween.alphaCanvas(image, 0f, 0.3f);
            //undiscoveredButton.fontStyle = FontStyles.Underline;
            undiscoveredButton.color = new Color(0, 0, 0, 1);
        }
        else
        {
            LeanTween.alphaCanvas(image1, 0f, 0.3f);
            LeanTween.alphaCanvas(image, 1f, 0.3f);
            //discoveredButton.fontStyle = FontStyles.Underline;
            discoveredButton.color = new Color(0, 0, 0, 1);
        }
    }
}