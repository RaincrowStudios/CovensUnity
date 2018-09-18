using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetupDeckCard : MonoBehaviour
{

	public Text title;
	public Text legend;
	public Text tier;
	public Text Energy;
	public Text SummonsSpirit;
	public Text SummonTime;
	public Image spirit;
	public Image spiritCopy;
	public SpiritData sd;


	public void SetupCard(SpiritData data, SpiritDeckUIManager.type type)
	{
		sd = data;
		if (sd.instance == "null" ||sd.instance == "empty" )
			return;
		if (type != SpiritDeckUIManager.type.portal) {
			SetupSpiritCard (sd);
		} else {
			SetupPortalCard (sd);
		}
	}

	 void SetupSpiritCard(SpiritData sd)
	{
		if( DownloadedAssets.spiritArt.ContainsKey(sd.id))
			spirit.sprite = DownloadedAssets.spiritArt [sd.id];
		if (DownloadedAssets.spiritDictData.ContainsKey (sd.id)) {
			title.text = DownloadedAssets.spiritDictData [sd.id].spiritName;
			tier.text = Utilities.ToRoman (DownloadedAssets.spiritDictData [sd.id].spiritTier);
			legend.text = DownloadedAssets.spiritDictData [sd.id].spiritLegend;
			tier.text = Utilities.ToRoman (DownloadedAssets.spiritDictData [sd.id].spiritTier);	 
		}
	}

	void SetupPortalCard(SpiritData sd)
	{
		SummonTime.text = "Summons in : " + Utilities.GetSummonTime (sd.summonOn);
		Energy.text = "Energy : " + sd.energy.ToString ();
		SummonsSpirit.text = "Summons : " + DownloadedAssets.spiritDictData [sd.spirit].spiritName;
		spirit.sprite = DownloadedAssets.spiritArt [sd.spirit];
		spiritCopy.sprite = DownloadedAssets.spiritArt [sd.spirit];
	}
}

