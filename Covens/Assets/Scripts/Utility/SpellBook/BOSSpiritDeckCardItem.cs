using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class BOSSpiritDeckCardItem : BOSBase
{
    [SerializeField] private TextMeshProUGUI totalSpirits;
    [SerializeField] private TextMeshProUGUI undiscoverd;
    [SerializeField] private TextMeshProUGUI discovered;
    [SerializeField] private TextMeshProUGUI activeSpirits;
    [SerializeField] private TextMeshProUGUI spiritList;
    [SerializeField] private TextMeshProUGUI activePortals;
    [SerializeField] private TextMeshProUGUI cardZone;
    private HashSet<string> activeSpiritsList = new HashSet<string>();

    public void Setup(int zone)
    {
        var pData = PlayerDataManager.playerData;
        cardZone.text = LocalizeLookUp.GetZoneName(zone);
        int totalSpiritsCount = 0;
        int discoveredSpiritsCount = 0;
        int activePortalCount = 0;
        int activeSpiritCount = 0;


        Dictionary<string, KnownSpirits> knownSpiritsDict = new Dictionary<string, KnownSpirits>();
        foreach (KnownSpirits entry in pData.knownSpirits)
            knownSpiritsDict.Add(entry.spirit, entry);

        foreach (var spirit in DownloadedAssets.spiritDict.Values)
        {

            if (spirit.zones.Contains(zone))
            {
                totalSpiritsCount++;
                if (knownSpiritsDict.ContainsKey(spirit.id))
                    discoveredSpiritsCount++;
            }
        }

        foreach (var item in BOSSpirit.activePortalsData)
        {
            if (item.zone == zone)
            {
                activePortalCount++;
            }
        }


        foreach (var item in BOSSpirit.activeSpiritsData)
        {
            if (item.zone == zone)
            {
                activeSpiritsList.Add(item.id);
                activeSpiritCount++;
            }
        }

        totalSpirits.text = "Total Spirits: " + totalSpiritsCount.ToString();
        discovered.text = "Discovered: " + discoveredSpiritsCount.ToString();
        undiscoverd.text = "Undiscovered: " + (totalSpiritsCount - discoveredSpiritsCount).ToString();
        activeSpirits.text = activeSpiritCount > 0 ? "Active Spirits: " + activeSpiritCount.ToString() : "";
        activePortals.text = activePortalCount > 0 ? "Active Portals: " + activePortalCount.ToString() : "";

        spiritList.text = "";
        foreach (var item in activeSpiritsList)
        {
            spiritList.text += LocalizeLookUp.GetSpiritName(item) + " ";
        }
        if (totalSpiritsCount > 0)
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                BOSSpirit.undiscoveredSpirits = totalSpiritsCount - discoveredSpiritsCount;
                BOSSpirit.discoveredSpirits = discoveredSpiritsCount;
                BOSSpirit.currentZone = zone;
                BOSSpirit.instance.ShowSelectedZone();
            });
        }
    }

}