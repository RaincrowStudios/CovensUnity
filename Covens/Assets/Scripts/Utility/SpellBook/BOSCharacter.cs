using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Raincrow.FTF;

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

    private void Awake()
    {
        Instance = this;

        title.text = LocalizeLookUp.GetText("bos_title");

        var pData = PlayerDataManager.playerData;

        witchType.text = Utilities.WitchTypeControlSmallCaps(pData.degree);
        witchType.text += " <b>" + PlayerDataManager.playerData.name + "</b>";
        coven.text = string.IsNullOrEmpty(pData.covenInfo.name) ? LocalizeLookUp.GetText("lt_coven_none") : LocalizeLookUp.GetText("lt_coven") + " " + pData.covenInfo.name;

        dominionRank.text = LocalizeLookUp.GetText("generic_rank") + " " + pData.dominionRank.ToString() + " " + LocalizeLookUp.GetText("dominion_location_short") + " " + pData.dominion;
        //dominionRank.text = LocalizeLookUp.GetText("lt_dominion") + " " + pData.dominion; ;
        worldRank.text = LocalizeLookUp.GetText("generic_rank") + " " + pData.worldRank.ToString() + " " + LocalizeLookUp.GetText("dominion_world");

        favoriteSpell.text = LocalizeLookUp.GetText("spell_favorite") + " " + (string.IsNullOrEmpty(pData.favoriteSpell) ? LocalizeLookUp.GetText("lt_none") : LocalizeLookUp.GetSpellName(pData.favoriteSpell));

        nemesis.text = LocalizeLookUp.GetText("generic_nemesis") + ": " + (string.IsNullOrEmpty(pData.nemesis) ? LocalizeLookUp.GetText("lt_none") : pData.nemesis);
        benefactor.text = LocalizeLookUp.GetText("generic_benefactor") + ": " + (string.IsNullOrEmpty(pData.benefactor) ? LocalizeLookUp.GetText("lt_none") : pData.benefactor);

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
        if (FirstTapManager.IsFirstTime("bos"))
        {
            FirstTapManager.Show("bos", null);
            return;
        }
    }
}
