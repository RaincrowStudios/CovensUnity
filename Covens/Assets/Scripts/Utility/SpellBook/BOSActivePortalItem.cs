using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BOSActivePortalItem : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI summons;
    [SerializeField] TextMeshProUGUI timeLeft;
    [SerializeField] TextMeshProUGUI attackedBy;
    [SerializeField] TextMeshProUGUI spiritBehavior;
    [SerializeField] TextMeshProUGUI energy;
    [SerializeField] Image spiritIcon;

    public void Setup(SpiritData sp)
    {
        summons.text = $"Summons: <b>{DownloadedAssets.spiritDictData[sp.spirit].spiritName}";
        timeLeft.text = $"Time Left: <b>{Utilities.GetSummonTime(sp.summonOn)}";
        attackedBy.text = $"Last Attacked By: <b>None";
        spiritBehavior.text = DownloadedAssets.spiritDictData[sp.spirit].spriitBehavior;
		spiritBehavior.text = spiritBehavior.text.Replace ("<color=#FFFFFF99>", "")
			.Replace ("</color>", "");
        energy.text = $"Energy: <b>{sp.energy.ToString()}";
        DownloadedAssets.GetSprite(sp.spirit, spiritIcon);
    }
}
