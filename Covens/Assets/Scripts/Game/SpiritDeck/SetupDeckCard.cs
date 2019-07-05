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
	public SpiritInstance sd;

	public void SetupCard(SpiritInstance data)
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

    void SetupSpiritCard(SpiritInstance sd)
    {
        try
        {
            DownloadedAssets.GetSprite(sd.id, spirit);
            title.text = DownloadedAssets.spiritDictData[sd.id].Name;
            tier.text = Utilities.ToRoman(DownloadedAssets.spiritDictData[sd.id].tier);
            legend.text = DownloadedAssets.spiritDictData[sd.id].Location;
            tier.text = Utilities.ToRoman(DownloadedAssets.spiritDictData[sd.id].tier);
        }
        catch
        {
            Debug.Log(" NOT EXIST " + sd.id);
        }
    }

	void SetupPortalCard(SpiritInstance sd)
	{
		SummonTime.text = "Summons in : " + Utilities.GetSummonTime (sd.summonOn);
		Energy.text = "Energy : " + sd.energy.ToString ();
		SummonsSpirit.text = "Summons : " + LocalizeLookUp.GetSpiritName(sd.spirit);

		DownloadedAssets.GetSprite(sd.spirit,spirit);
		DownloadedAssets.GetSprite(sd.spirit,spiritCopy);
	

	}

	public void OnClick()
	{
		SpiritDeckUIManager.Instance.selectedcard = sd;
		SpiritDeckUIManager.Instance.Enter (transform);

	}

}

