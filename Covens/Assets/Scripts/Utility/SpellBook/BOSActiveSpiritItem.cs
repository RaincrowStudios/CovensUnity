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

    public void Setup(SpiritData sp)
    {
        Debug.Log($"spirit lat: {sp.lat} lng: {sp.lng}");

        flyToText.text = LocalizeLookUp.GetText("spirit_deck_fly_to");
        transform.GetChild(10).GetComponent<Button>().onClick.AddListener(() => {
            BOSController.Instance.Close();
            PlayerManager.Instance.FlyTo(sp.lng, sp.lat);
        });
        xpGained.text = $"XP gained for you: <b>0";
        expireOn.text = $"Expire On: <b>{Utilities.GetTimeStampBOS(sp.expiresOn)}";
        attacked.text = $"Attacked: <b>{sp.attacked.ToString()} Witches";
        collected.text = $"Collected: <b>{sp.gathered.ToString()} Items";
        spiritBehavior.text = DownloadedAssets.spiritDictData[sp.id].spriitBehavior;

        string r = "";
        if (DownloadedAssets.spiritDictData[sp.id].spiritTier == 1)
        {
            r = "Lesser Spirit";
        }
        else if (DownloadedAssets.spiritDictData[sp.id].spiritTier == 2)
        {
            r = "Greater Spirit";
        }
        else if (DownloadedAssets.spiritDictData[sp.id].spiritTier == 3)
        {
            r = "Superior Spirit";
        }
        else
        {
            r = "Legendary Spirit";
        }
        //spiritTier.text = $"({r})";
        spiritTitle.text = DownloadedAssets.spiritDictData[sp.id].spiritName + $" <b>({r})";
        DownloadedAssets.GetSprite(sp.id, spiritIcon);
    }
}
