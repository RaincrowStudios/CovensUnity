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
        summons.text = $"Summons: <b>{DownloadedAssets.spiritDictData[sp.spirit].spiritName}";
        timeLeft.text = $"Time Left: <b>{Utilities.GetSummonTime(sp.summonOn)}";
        attackedBy.text = $"Last Attacked By: <b>None";
        spiritBehavior.text = DownloadedAssets.spiritDictData[sp.spirit].spriitBehavior;
		spiritBehavior.text = spiritBehavior.text.Replace ("<color=#FFFFFF99>", "")
			.Replace ("</color>", "");
        //energy.text = $"Energy: <b>{sp.energy.ToString()}";
        DownloadedAssets.GetSprite(sp.spirit, spiritIcon);
        StartCoroutine(PortalCountDown());
    }

    IEnumerator PortalCountDown()
    {
        while (timeLeft.text != "Time Left: <b>0 secs")
        {
            timeLeft.text = $"Time Left: <b>{Utilities.GetSummonTime(spiritSummonOnTime)}";
            yield return new WaitForSeconds(1f);
        }

        Destroy(gameObject);
    }
}
