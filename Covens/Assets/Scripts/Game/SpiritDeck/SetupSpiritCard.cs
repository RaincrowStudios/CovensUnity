using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SetupSpiritCard : MonoBehaviour
{
	public Text title;
	public Text legend;
	public Text tier;
	public Image spirit;

	public void SetupCard(string spiritName, string spiritlegend, int spirittier, string spiritID)
	{
		spirit.sprite = DownloadedAssets.spiritArt [spiritID];
		title.text = spiritName.Remove (0,7);
		tier.text = convertToRoman (spirittier);
		legend.text = spiritlegend;
	}

	string convertToRoman(int tier)
	{
		if (tier == 1)
			return "I";
		else if (tier == 2)
			return "II";
		else if (tier == 3)
			return "III";
		else
			return "IV";
	}
}

