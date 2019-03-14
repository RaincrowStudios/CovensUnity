using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OnSignatureDiscovered
{
    public static void HandleEvent(WSData data)
    {
        foreach (var ing in data.signature.ingredients)
        {
//            if (DownloadedAssets.ingredientDictData[ing.id].type == "herb")
//                data.signature.herb = ing.id;
//            else if (DownloadedAssets.ingredientDictData[ing.id].type == "gem")
//                data.signature.gem = ing.id;
//            else
//                data.signature.tool = ing.id;
        }

        SpellDict spellInfo = DownloadedAssets.spellDictData[data.signature.id];
        data.signature.displayName = spellInfo.spellName;
        data.signature.school = spellInfo.spellSchool;
        data.signature.description = spellInfo.spellDescription;
        data.signature.lore = spellInfo.spellLore;
        data.signature.unlocked = true;

        List<SpellData> spells = PlayerDataManager.playerData.spells;
        for (int i = 0; i < spells.Count; i++)
        {
            if (spells[i].id == data.signature.id)
            {
                spells[i] = data.signature;
                break;
            }
        }
        //PlayerDataManager.playerData.spells.Add(data.signature);
        //PlayerDataManager.playerData.spellsDict.Add(data.signature.id, data.signature);

        UISignatureUnlocked.Instance.Show(data);
    }
}
