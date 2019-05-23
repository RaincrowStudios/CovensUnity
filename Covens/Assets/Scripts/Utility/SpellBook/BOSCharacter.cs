using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BOSCharacter : BOSBase
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI witchType;
    [SerializeField] private TextMeshProUGUI coven;
    [SerializeField] private TextMeshProUGUI dominionRank;
    [SerializeField] private TextMeshProUGUI worldRank;
    [SerializeField] private TextMeshProUGUI favoriteSpell;
    [SerializeField] private TextMeshProUGUI nemesis;
    [SerializeField] private TextMeshProUGUI benefactor;
    [SerializeField] private TextMeshProUGUI pathText;
    [SerializeField] private Sprite[] pathSprites;
    [SerializeField] private Image pathImage;
	public static BOSCharacter Instance { get; set; }



    void Start()
    {
		Instance = this;
        var pData = PlayerDataManager.playerData;
        witchType.text = Utilities.witchTypeControlSmallCaps(pData.degree);
		witchType.text += " <b>" + PlayerDataManager.playerData.displayName + "</b>";
		title.text = LocalizeLookUp.GetText ("bos_title");
       // coven.text = pData.covenName == "" ? "Coven: No Coven" : "Coven: " + pData.covenName;
		coven.text = pData.covenName == "" ? LocalizeLookUp.GetText("lt_coven_none") : LocalizeLookUp.GetText("lt_coven") + pData.covenName;
		dominionRank.text = LocalizeLookUp.GetText ("generic_rank") + " " + pData.dominionRank.ToString() + " " + LocalizeLookUp.GetText ("dominion_location_short") + " " + pData.dominion;
		worldRank.text = LocalizeLookUp.GetText ("generic_rank") + " " + pData.worldRank.ToString() + " " + LocalizeLookUp.GetText ("dominion_world");
		favoriteSpell.text = LocalizeLookUp.GetText ("spell_favorite") + " " + (pData.favoriteSpell == null ? LocalizeLookUp.GetText ("lt_none") : DownloadedAssets.GetSpell(pData.favoriteSpell).spellName);
		nemesis.text = LocalizeLookUp.GetText("generic_nemesis") +": " + (pData.nemesis == "" ? LocalizeLookUp.GetText ("lt_none") : pData.nemesis);
		benefactor.text = LocalizeLookUp.GetText("generic_benefactor") +": " + (pData.benefactor == "" ? LocalizeLookUp.GetText ("lt_none") : pData.benefactor);
        if (pData.degree > 0)
        {
            pathImage.sprite = pathSprites[1];
			pathText.text = LocalizeLookUp.GetText("bos_path_light");
        }
        else if (pData.degree < 0)
        {
            pathImage.sprite = pathSprites[2];
			pathText.text = LocalizeLookUp.GetText("bos_path_shadow");
        }
        else
        {
            pathImage.sprite = pathSprites[0];
            pathText.text = "";
        }
    }
}
