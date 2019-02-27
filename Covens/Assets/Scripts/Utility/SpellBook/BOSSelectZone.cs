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




    void Start()
    {
        spawnRegion.text = "Spawn Region: " + DownloadedAssets.zonesIDS[BOSSpirit.currentZone];
        if (BOSSpirit.discoveredSpirits == 0)
        {
            discoveredButton.color = new Color(0, 0, 0, .55f);
            undiscoveredButton.fontStyle = FontStyles.Underline;
            undiscoveredButton.color = new Color(0, 0, 0, 1);
        }
        else
        {
            undiscoveredButton.GetComponent<Button>().onClick.AddListener(showUndiscoveredSpirits);
            discoveredButton.GetComponent<Button>().onClick.AddListener(showDiscoveredSpirits);
        }
        showUndiscoveredSpirits();
        backButton.onClick.AddListener(BOSSpirit.instance.ShowSpiritDeck);
    }

    void showUndiscoveredSpirits()
    {
        SetButtons(true);
        for (int i = 0; i < BOSSpirit.undiscoveredSpirits; i++)
        {
            var g = Utilities.InstantiateObject(undiscoveredCard, container);
        }
        spiritCountInfo.text = BOSSpirit.undiscoveredSpirits.ToString() + " Undiscovered Spirits";
    }

    void showDiscoveredSpirits()
    {
        SetButtons(false);
        foreach (var item in PlayerDataManager.playerData.knownSpirits)
        {
            if (PlayerDataManager.summonMatrixDict[item.id].zone.Contains(BOSSpirit.currentZone))
            {
                var g = Utilities.InstantiateObject(discoveredCard, container).transform;
                var sp = DownloadedAssets.spiritDictData[item.id];
                g.GetChild(1).GetComponent<TextMeshProUGUI>().text = sp.spiritTier.ToString();
                g.GetChild(2).GetComponent<TextMeshProUGUI>().text = sp.spiritName;
                DownloadedAssets.GetSprite(item.id, g.GetChild(3).GetComponent<Image>());
                g.GetChild(4).GetComponent<TextMeshProUGUI>().text = item.location + ", " + Utilities.GetTimeStampBOS(item.banishedOn) + ".";
            }
        }
        spiritCountInfo.text = BOSSpirit.discoveredSpirits.ToString() + " Discovered Spirits";

    }

    void SetButtons(bool undiscovered)
    {
        foreach (Transform item in container)
        {
            Destroy(item.gameObject);
        }
        undiscoveredButton.fontStyle = discoveredButton.fontStyle = FontStyles.Normal;
        undiscoveredButton.color = discoveredButton.color = new Color(0, 0, 0, .55f);
        if (undiscovered)
        {
            undiscoveredButton.fontStyle = FontStyles.Underline;
            undiscoveredButton.color = new Color(0, 0, 0, 1);
        }
        else
        {
            discoveredButton.fontStyle = FontStyles.Underline;
            discoveredButton.color = new Color(0, 0, 0, 1);
        }
    }
}