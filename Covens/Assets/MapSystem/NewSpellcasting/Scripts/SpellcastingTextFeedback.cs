using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellcastingTextFeedback
{
    public static string CreateSpellFeedback(IMarker caster, IMarker target, WSData data)
    {
        //basic caster/target info
        string casterName, targetName;
        string casterColor, targetColor;
        string casterDegree, targetDegree;
        int damage = Mathf.Abs(data.result.total);
        string intensityModifier = "";
        if (data.hexCount == 0) intensityModifier = "";
        else if (data.hexCount == 1) intensityModifier = "slightly more";
        else if (data.hexCount == 2) intensityModifier = "more";
        else if (data.hexCount >= 3) intensityModifier = "significantly more";

        //setup spirit/witch specific info
        if (caster == PlayerManager.marker)
        {
            casterName = casterColor = casterDegree = "";
        }
        else if (caster.type == MarkerSpawner.MarkerType.spirit)
        {
            casterName = DownloadedAssets.GetSpirit(caster.token.spiritId).spiritName;
            casterColor = "spirit";
            casterDegree = "";
        }
        else
        {
            casterName = caster.token.displayName;
            casterColor = Utilities.GetSchool(caster.token.degree);
            casterDegree = Utilities.GetDegree(caster.token.degree);
        }

        if (target == PlayerManager.marker)
        {
            targetName = targetColor = targetDegree = "";
        }
        else if (target.type == MarkerSpawner.MarkerType.spirit)
        {
            targetName = DownloadedAssets.GetSpirit(target.token.spiritId).spiritName;
            targetColor = "spirit";
            targetDegree = "";
        }
        else
        {
            targetName = target.token.displayName;
            targetColor = Utilities.GetSchool(target.token.degree);
            targetDegree = Utilities.GetDegree(target.token.degree);
        }


        if (DownloadedAssets.spellFeedbackDictData.ContainsKey(data.spell))
        {
            string str = caster == PlayerManager.marker ? DownloadedAssets.spellFeedbackDictData[data.spell].asCaster : DownloadedAssets.spellFeedbackDictData[data.spell].asTarget;

            if (str == null)
            {
                Debug.LogError($"empty feedback string for {data.spell}");
                return null;
            }

            return string.Format(
                str,
                casterName,
                casterColor,
                casterDegree,
                targetName,
                targetColor,
                targetDegree,
                damage,
                0,//power
                0,//resilience
                0,//spell success change
                intensityModifier,
                0//condition duration
            );
        }
        else //default feedback texts
        {
            Debug.LogError($"spell feedback not found for spell \"{data.spell}\"");

            SpellDict spellData = DownloadedAssets.GetSpell(data.spell);

            if (spellData != null)
            {
                if (target == PlayerManager.marker)
                {
                    if (data.result.total > 0)
                        return $"{casterName} cast {spellData.spellName} on you. You gain <color=yellow>{damage}</color> Energy.";
                    else if (data.result.total < 0)
                        return $"{casterName} cast {spellData.spellName} on you. You lose <color=red>{damage}</color> Energy.";
                    else
                        return $"{casterName} cast {spellData.spellName} on you.";
                }
                else
                {
                    if (data.result.total > 0)
                        return $"You cast {spellData.spellName} on {targetName}. {targetName} gained <color=yellow>{damage}</color> Energy.";
                    else if (data.result.total < 0)
                        return $"You cast {spellData.spellName} on {targetName}. {targetName} lost <color=red>{damage}</color> Energy.";
                    else
                        return $"You cast {spellData.spellName} on {targetName}.";
                }
            }
            else
            {
                if (target == PlayerManager.marker)
                {
                    if (data.result.total > 0)
                        return $"{casterName} buffed you. You gained {damage} energy.";
                    else if (data.result.total < 0)
                        return $"{targetName} attacked you. You lost {damage} energy.";
                    else return null;

                }
                else
                {
                    if (data.result.total > 0)
                        return $"You buffed {targetName}. {targetName} gained {damage} energy.";
                    else if (data.result.total < 0)
                        return $"You attacked {targetName}. {targetName} lost {damage} energy.";
                    else return null;
                }
            }
        }
    }
}
