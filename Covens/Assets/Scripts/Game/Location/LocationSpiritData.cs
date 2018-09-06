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
		sprite.sprite = DownloadedAssets.spiritArt [id];
		title.text = DownloadedAssets.spiritDictData [id].spiritName;
		tier.text = DownloadedAssets.spiritDictData [id].spiritTier.ToString();
		legend.text = DownloadedAssets.spiritDictData [id].spiritLegend;
	}
}

