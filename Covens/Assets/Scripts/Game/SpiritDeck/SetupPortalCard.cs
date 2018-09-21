using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

public class SetupPortalCard : EnhancedScrollerCellView 
{
	public SpiritData sd;
	public Text title;
	public Text legend;
	public Text tier;
	public Text Energy;
	public Text SummonsSpirit;
	public Text SummonTime;
	public Image spirit;
	public Image spiritCopy;
	public GameObject FX;

	public void SetupCard(SpiritData data)
	{
		sd = data;
		Setup (sd);
		FX.SetActive (false);
	}

	void Setup(SpiritData sd)
	{
		SummonTime.text = "Summons in : " + Utilities.GetSummonTime (sd.summonOn); 
		SummonTime.text = "Summons in : ";
		Energy.text = "Energy : " + sd.energy.ToString (); 
		SummonsSpirit.text = "Summons : " + DownloadedAssets.spiritDictData [sd.spirit].spiritName; 
		spirit.sprite = DownloadedAssets.spiritArt [sd.spirit];
		spiritCopy.sprite = DownloadedAssets.spiritArt [sd.spirit]; 
	}

	public void OnClick()
	{
		SpiritDeckUIManager.Instance.Enter (transform);
	}
}

