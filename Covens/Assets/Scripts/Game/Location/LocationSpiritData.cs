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
        SpiritData spirit = DownloadedAssets.GetSpirit(id);

        title.text = spirit.Name;
        tier.text = spirit.tier.ToString();
        legend.text = spirit.Location;
    }
}

