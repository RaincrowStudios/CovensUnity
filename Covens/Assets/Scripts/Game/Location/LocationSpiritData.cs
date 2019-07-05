using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocationSpiritData : MonoBehaviour
{
    public Image sprite;
    public Text title;
    public Text tier;
    public Text legend;

    public void Setup(string id)
    {
        DownloadedAssets.GetSprite(id, sprite);

        title.text = DownloadedAssets.spiritDictData[id].spiritName;
        tier.text = DownloadedAssets.spiritDictData[id].tier.ToString();
        legend.text = DownloadedAssets.spiritDictData[id].spiritLegend;
    }
}

