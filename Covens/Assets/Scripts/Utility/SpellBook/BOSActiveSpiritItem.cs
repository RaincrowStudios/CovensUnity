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
    [SerializeField] TextMeshProUGUI spiritTier;
    [SerializeField] Image spiritIcon;

    public void Setup(SpiritData sp)
    {
        xpGained.text = $"XP gained for you: <b>0";
        expireOn.text = $"Expire On: <b>{Utilities.GetTimeStampBOS(sp.expiresOn)}";
        attacked.text = $"Attacked: <b>0 Witches";
        collected.text = $"Collected: <b>0 Items";
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
        spiritTier.text = $"({r})";
        spiritTitle.text = DownloadedAssets.spiritDictData[sp.id].spiritName;
        DownloadedAssets.GetSprite(sp.id, spiritIcon);
    }
}
