using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnhancedUI.EnhancedScroller;

public class SetupDeckCard :EnhancedScrollerCellView 
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

	public void SetupCard(SpiritData data)
	{
		GetComponent<Animator> ().Play("base");
		sd = data;
		if (sd.instance == "null" ||sd.instance == "empty" )
			return;
		if (sd.deckCardType != SpiritDeckUIManager.type.portal) {
			SetupSpiritCard (sd);
		} else {
			SetupPortalCard (sd);
		}
	}

	 void SetupSpiritCard(SpiritData sd)
	{
		try{
			DownloadedAssets.GetSprite(sd.id,spirit);
		title.text = DownloadedAssets.spiritDictData[sd.id].spiritName;
		tier.text = Utilities.ToRoman( DownloadedAssets.spiritDictData[sd.id].spiritTier);
		legend.text = DownloadedAssets.spiritDictData[sd.id].spiritLegend;
		tier.text = Utilities.ToRoman( DownloadedAssets.spiritDictData [sd.id].spiritTier);	 
		}catch{
			Debug.Log (" NOT EXIST " + sd.id);
		}
	}

	void SetupPortalCard(SpiritData sd)
	{
		SummonTime.text = "Summons in : " + Utilities.GetSummonTime (sd.summonOn);
		Energy.text = "Energy : " + sd.energy.ToString ();
		SummonsSpirit.text = "Summons : " + DownloadedAssets.spiritDictData [sd.spirit].spiritName;

		DownloadedAssets.GetSprite(sd.spirit,spirit);
		DownloadedAssets.GetSprite(sd.spirit,spiritCopy);
	

	}

	public void OnClick()
	{
		SpiritDeckUIManager.Instance.selectedcard = sd;
		SpiritDeckUIManager.Instance.Enter (transform);

	}

}

