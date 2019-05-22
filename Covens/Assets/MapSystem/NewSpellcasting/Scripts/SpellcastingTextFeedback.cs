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
		if (data.hexCount == 0)
			intensityModifier = "";
		else if (data.hexCount == 1)
			intensityModifier = LocalizeLookUp.GetText ("spell_intensity_slight");// "slightly more";
		else if (data.hexCount == 2) intensityModifier = LocalizeLookUp.GetText ("spell_intensity_more");//"more";
		else if (data.hexCount >= 3) intensityModifier = LocalizeLookUp.GetText ("spell_intensity_sig");//"significantly more";

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
			targetColor = LocalizeLookUp.GetText ("lt_spirit_s");//"spirit";
            targetDegree = "";
        }
        else
        {
            targetName = target.token.displayName;
            targetColor = Utilities.GetSchool(target.token.degree);
            targetDegree = Utilities.GetDegree(target.token.degree);
        }

        if (data.result.effect != "success")
        {
            SpellDict spellData = DownloadedAssets.GetSpell(data.spell);

            if (target == PlayerManager.marker)
            {
				if (spellData != null)
					return LocalizeLookUp.GetText ("spell_caster_tried_spell_failed").Replace ("{{Color}}", casterColor).Replace ("{{Target Name}}", targetName).Replace ("{{Spell Name}}", spellData.spellName);//"The " + casterColor + " " + targetName + " tried to cast " + spellData.spellName + " on you but failed.";
                else
					return LocalizeLookUp.GetText ("spell_caster_tried_failed").Replace ("{{Color}}", casterColor).Replace ("{{Target Name}}", targetName);//"The " + casterColor + " " + targetName + " tried to cast a spell on you but failed.";
            }
            else
            {
				if (spellData != null)
					return LocalizeLookUp.GetText ("spell_you_cast_spell_failed").Replace ("{{Spell Name}}", spellData.spellName).Replace ("{{Color}}", casterColor).Replace ("{{Target Name}}", targetName);//"You tried to cast " + spellData.spellName + " on the " + casterColor + " " + targetName + " and failed.";
                else
					return LocalizeLookUp.GetText ("spell_you_cast_failed").Replace ("{{Color}}", casterColor).Replace ("{{Target Name}}", targetName);//"You tried to cast a spell on the " + casterColor + " " + targetName + " and failed.";
            }
        }

        if (DownloadedAssets.spellFeedbackDictData.ContainsKey(data.spell))
        {
            string str = caster == PlayerManager.marker ? DownloadedAssets.spellFeedbackDictData[data.spell].asCaster : DownloadedAssets.spellFeedbackDictData[data.spell].asTarget;

            if (str == null)
            {
                Debug.LogError($"empty feedback string for {data.spell}");
                return null;
            }
            Debug.Log(str);
            return string.Format(
                str,
                casterName,
                casterColor,
                casterDegree,
                targetName,
                targetColor,
                targetDegree,
                damage,
                damage,
                data.result.resilienceChanged.ToString(),//power
                data.result.successChance.ToString(),//resilience
                intensityModifier,
                "1 min",
                data.result.newResilience.ToString(),
                data.result.newPower.ToString(),
                data.result.powerChanged.ToString(),
                data.result.selfEnergy.ToString()
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
						return LocalizeLookUp.GetText ("spell_caster_spell_gain").Replace ("{{Caster Name}}", casterName).Replace ("{{Spell Name}}", spellData.spellName).Replace ("{{Amount}}", "<color=yellow>" + damage + "</color>");//$"{casterName} cast {spellData.spellName} on you. You gain <color=yellow>{damage}</color> Energy.";
                    else if (data.result.total < 0)
						return LocalizeLookUp.GetText ("spell_caster_spell_lose").Replace ("{{Caster Name}}", casterName).Replace ("{{Spell Name}}", spellData.spellName).Replace ("{{Amount}}", "<color=red>" + damage + "</color>");//$"{casterName} cast {spellData.spellName} on you. You lose <color=red>{damage}</color> Energy.";
                    else
						return LocalizeLookUp.GetText ("spell_caster_spell").Replace ("{{Caster Name}}", casterName).Replace ("{{Spell Name}}", spellData.spellName);//$"{casterName} cast {spellData.spellName} on you.";
                }
                else
                {
                    if (data.result.total > 0)
						return LocalizeLookUp.GetText ("spell_you_target_gain").Replace ("{{Spell Name}}", spellData.spellName).Replace("{{Target Name}}", targetName).Replace ("{{Amount}}", "<color=yellow>" + damage + "</color>");//$"You cast {spellData.spellName} on {targetName}. {targetName} gained <color=yellow>{damage}</color> Energy.";
                    else if (data.result.total < 0)
						return LocalizeLookUp.GetText ("spell_you_target_lost").Replace ("{{Spell Name}}", spellData.spellName).Replace("{{Target Name}}", targetName).Replace ("{{Amount}}", "<color=red>" + damage + "</color>");//$"You cast {spellData.spellName} on {targetName}. {targetName} lost <color=red>{damage}</color> Energy.";
                    else
						return LocalizeLookUp.GetText ("spell_you_target").Replace ("{{Spell Name}}", spellData.spellName).Replace("{{Target Name}}", targetName);//$"You cast {spellData.spellName} on {targetName}.";
                }
            }
            else
            {
                if (target == PlayerManager.marker)
                {
					if (data.result.total > 0)
						return LocalizeLookUp.GetText ("spell_caster_buff").Replace ("{{Caster_Name}}", casterName).Replace ("{{damage}}", damage.ToString());//$"{casterName} buffed you. You gained {damage} energy.";
                    else if (data.result.total < 0)
						return LocalizeLookUp.GetText ("spell_caster_attacked").Replace ("{{Target_Name}}", targetName).Replace ("{{damage}}", damage.ToString());//$"{targetName} attacked you. You lost {damage} energy.";
                    else return null;

                }
                else
                {
                    if (data.result.total > 0)
						return LocalizeLookUp.GetText ("spell_you_buffed").Replace ("{{Target_Name}}", targetName).Replace ("{{damage}}", damage.ToString());//$"You buffed {targetName}. {targetName} gained {damage} energy.";
                    else if (data.result.total < 0)
						return LocalizeLookUp.GetText ("spell_you_attacked").Replace ("{{Target_Name}}", targetName).Replace ("{{damage}}", damage.ToString());//$"You attacked {targetName}. {targetName} lost {damage} energy.";
                    else return null;
                }
            }
        }
    }
}
