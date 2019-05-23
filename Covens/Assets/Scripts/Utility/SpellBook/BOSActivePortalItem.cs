using System;
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
    [SerializeField] TextMeshProUGUI flyToText;
    [SerializeField] Image spiritIcon;
    double spiritSummonOnTime;

    public void Setup(SpiritData sp)
    {
        Debug.Log($"spirit lat: {sp.lat} lng: {sp.lng}");

        flyToText.text = LocalizeLookUp.GetText("portal_fly_to");
        transform.GetChild(8).GetComponent<Button>().onClick.AddListener(() => {
            BOSController.Instance.Close();
            PlayerManager.Instance.FlyTo(sp.lng, sp.lat);
        });

        spiritSummonOnTime = sp.summonOn;
		summons.text = LocalizeLookUp.GetText ("portal_summon_spirit").Replace ("{{name of spirit}}", DownloadedAssets.spiritDictData [sp.spirit].spiritName);// $"Summons: <b>{DownloadedAssets.spiritDictData[sp.spirit].spiritName}";
		timeLeft.text = LocalizeLookUp.GetText("summoning_portal_summon_time").Replace("{{time}}", Utilities.GetSummonTime(sp.summonOn));//$"Time Left: <b>{Utilities.GetSummonTime(sp.summonOn)}";
		//attackedBy.text = LocalizeLookUp.GetText("portal_last_attacked").Replace("{{witch}}", DownloadedAssets.spiritDictData[sp.spirit]//$"Last Attacked By: <b>None";
        
		spiritBehavior.text = DownloadedAssets.spiritDictData[sp.spirit].spriitBehavior;
		spiritBehavior.text = spiritBehavior.text.Replace ("<color=#FFFFFF99>", "")
			.Replace ("</color>", "");
        //energy.text = $"Energy: <b>{sp.energy.ToString()}";
        DownloadedAssets.GetSprite(sp.spirit, spiritIcon);
        StartCoroutine(PortalCountDown());
    }

    IEnumerator PortalCountDown()
    {
		while (Utilities.TimespanFromJavaTime(spiritSummonOnTime).TotalSeconds > 0d)//timeLeft.text != "Time Left: <b>0 secs")
        {
			timeLeft.text = LocalizeLookUp.GetText("summoning_portal_summon_time").Replace("{{time}}", Utilities.GetSummonTime(spiritSummonOnTime));//$"Time Left: <b>{Utilities.GetSummonTime(spiritSummonOnTime)}";
            yield return new WaitForSeconds(1f);
        }

        Destroy(gameObject);
    }
	/*private string timeKill()
	//var i = Utilities.
		{
		var p = Utilities.GetSummonTime (spiritSummonOnTime);
		if (p != "0") {
			Destroy (gameObject);	
		} else {
			return p;
		}
	}*/
}
