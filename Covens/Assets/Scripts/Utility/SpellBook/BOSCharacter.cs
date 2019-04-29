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
        title.text = "Witch Stats";
        coven.text = pData.covenName == "" ? "Coven: No Coven" : "Coven: " + pData.covenName;
        dominionRank.text = $"Rank {pData.dominionRank.ToString()} in the Dominion of {pData.dominion}";
        worldRank.text = $"Rank {pData.worldRank.ToString()} in the World";
        favoriteSpell.text = $"Favorite Spell: {(pData.favoriteSpell == "" ? "None" : pData.favoriteSpell)}";
        nemesis.text = $"Nemesis: {(pData.nemesis == "" ? "None" : pData.nemesis)}";
        benefactor.text = $"Benefactor: {(pData.benefactor == "" ? "None" : pData.benefactor)}";
        if (pData.degree > 0)
        {
            pathImage.sprite = pathSprites[1];
            pathText.text = "You are on the path of Light";
        }
        else if (pData.degree < 0)
        {
            pathImage.sprite = pathSprites[2];
            pathText.text = "You are on the path of Shadow";
        }
        else
        {
            pathImage.sprite = pathSprites[0];
            pathText.text = "";
        }
    }
}
