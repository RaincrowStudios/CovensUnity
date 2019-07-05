using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

public class SetupPortalCard : EnhancedScrollerCellView 
{
	public SpiritInstance sd;
	public Text title;
	public Text legend;
	public Text tier;
	public Text Energy;
	public Text SummonsSpirit;
	public Text SummonTime;
	public Image spirit;
	public Image spiritCopy;
	public GameObject FX;

	public void SetupCard(SpiritInstance data)
	{
		sd = data;
		Setup (sd);
		FX.SetActive (false);
	}

	void Setup(SpiritInstance sd)
	{
		SummonTime.text = "Summons in : " + Utilities.GetSummonTime (sd.summonOn); 
		SummonTime.text = "Summons in : ";
		Energy.text = "Energy : " + sd.energy.ToString (); 
		SummonsSpirit.text = "Summons : " + DownloadedAssets.spiritDictData [sd.spirit].spiritName; 

		DownloadedAssets.GetSprite(sd.spirit,spirit);

		DownloadedAssets.GetSprite(sd.spirit,spiritCopy);


	}

	public void OnClick()
	{
		SpiritDeckUIManager.Instance.Enter (transform);
	}
}

