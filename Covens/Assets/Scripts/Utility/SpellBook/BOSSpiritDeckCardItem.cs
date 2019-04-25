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
        var pSData = PlayerDataManager.summonMatrixDict;
        var pData = PlayerDataManager.playerData;
        cardZone.text = DownloadedAssets.zonesIDS[zone];
        int totalSpiritsCount = 0;
        int discoveredSpiritsCount = 0;
        int activePortalCount = 0;
        int activeSpiritCount = 0;


        foreach (var item in pSData)
        {

            if (item.Value.zone.Contains(zone))
                totalSpiritsCount++;
            if (pData.knownSpiritsDict.ContainsKey(item.Key) && (item.Value.zone.Contains(zone)))
                discoveredSpiritsCount++;
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
            spiritList.text += DownloadedAssets.spiritDictData[item].spiritName + " ";
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