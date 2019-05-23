using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BOSActiveSpiritItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI xpGained;
    [SerializeField] TextMeshProUGUI collected;
    [SerializeField] TextMeshProUGUI attacked;
    [SerializeField] TextMeshProUGUI expireOn;
    [SerializeField] TextMeshProUGUI spiritBehavior;
    [SerializeField] TextMeshProUGUI spiritTitle;
    [SerializeField] TextMeshProUGUI flyToText;
    [SerializeField] Image spiritIcon;
	double spiritSummonOnTime;

    public void Setup(SpiritData sp)
    {
        Debug.Log($"spirit lat: {sp.lat} lng: {sp.lng}");
		spiritSummonOnTime = sp.expiresOn;
        flyToText.text = LocalizeLookUp.GetText("spirit_deck_fly_to");
        transform.GetChild(10).GetComponent<Button>().onClick.AddListener(() => {
            BOSController.Instance.Close();
            PlayerManager.Instance.FlyTo(sp.lng, sp.lat);
        });
        //xpGained.text = $"XP gained for you: <b>0";
		expireOn.text = LocalizeLookUp.GetText("spirit_deck_expire").Replace("{{time}}", Utilities.GetSummonTime(sp.expiresOn));//GetTimeStampBOS(sp.expiresOn));// $"Expire On: <b>{Utilities.GetTimeStampBOS(sp.expiresOn)}";
		attacked.text = LocalizeLookUp.GetText("spirit_deck_spirit_attacked").Replace("{{number}}", sp.attacked.ToString());//$"Attacked: <b>{sp.attacked.ToString()} Witches";
		collected.text = LocalizeLookUp.GetText("spirit_deck_spirit_collected").Replace("{{number}}", sp.gathered.ToString());//$"Collected: <b>{sp.gathered.ToString()} Items";
        spiritBehavior.text = DownloadedAssets.spiritDictData[sp.id].spriitBehavior;

        string r = "";
        if (DownloadedAssets.spiritDictData[sp.id].spiritTier == 1)
        {
			r = LocalizeLookUp.GetText ("cast_spirit_lesser");//"Lesser Spirit";
        }
        else if (DownloadedAssets.spiritDictData[sp.id].spiritTier == 2)
        {
			r = LocalizeLookUp.GetText ("cast_spirit_greater");//"Greater Spirit";
        }
        else if (DownloadedAssets.spiritDictData[sp.id].spiritTier == 3)
        {
			r = LocalizeLookUp.GetText ("cast_spirit_superior");//"Superior Spirit";
        }
        else
        {
			r = LocalizeLookUp.GetText ("cast_spirit_legendary");//"Legendary Spirit";
        }
        //spiritTier.text = $"({r})";
        spiritTitle.text = DownloadedAssets.spiritDictData[sp.id].spiritName + $" <b>({r})";
        DownloadedAssets.GetSprite(sp.id, spiritIcon);
		StartCoroutine (PortalCountDown());
    }
	IEnumerator PortalCountDown()
	{
		while (Utilities.TimespanFromJavaTime(spiritSummonOnTime).TotalSeconds > 0d)//timeLeft.text != "Time Left: <b>0 secs")
		{
			expireOn.text = LocalizeLookUp.GetText("spirit_deck_expire").Replace("{{time}}", Utilities.GetSummonTime(spiritSummonOnTime));//$"Time Left: <b>{Utilities.GetSummonTime(spiritSummonOnTime)}";
			yield return new WaitForSeconds(1f);
		}

		Destroy(gameObject);
	}
}
