using Raincrow.GameEvent;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellcastingTextFeedback
{
    public static string CreateSpellFeedback(IMarker caster, IMarker target, SpellCastResponse response)
    {
        Debug.Log(JsonUtility.ToJson(response));

        //basic caster/target info
        string casterName, targetName;
        string casterColor, targetColor;
        string casterDegree, targetDegree;
        int damage = Mathf.Abs(response.Result.Damage);
        //string intensityModifier = "";
		//if (response.hexCount == 0)
		//	intensityModifier = "";
		//else if (response.hexCount == 1)
		//	intensityModifier = LocalizeLookUp.GetText ("spell_intensity_slight");// "slightly more";
		//else if (response.hexCount == 2) intensityModifier = LocalizeLookUp.GetText ("spell_intensity_more");//"more";
		//else if (response.hexCount >= 3) intensityModifier = LocalizeLookUp.GetText ("spell_intensity_sig");//"significantly more";

        //setup spirit/witch specific info
        if (caster == PlayerManager.marker)
        {
            casterName = casterColor = casterDegree = "";
        }
        else if (caster.type == MarkerSpawner.MarkerType.SPIRIT)
        {
            casterName = LocalizeLookUp.GetSpiritName(caster.token.spiritId);
            casterColor = "spirit";
            casterDegree = "";
        }
        else
        {
            Debug.Log("reached else statement");
            casterName = caster.token.displayName;
            casterColor = Utilities.GetSchool(caster.token.degree).ToUpper();
            Debug.Log(casterColor);
            casterDegree = Utilities.GetDegree(caster.token.degree);
        }

        if (target == PlayerManager.marker)
        {
            targetName = targetColor = targetDegree = "";
        }
        else if (target.type == MarkerSpawner.MarkerType.SPIRIT)
        {
            targetName = LocalizeLookUp.GetSpiritName(target.token.spiritId);
			targetColor = LocalizeLookUp.GetText ("lt_spirit_s");//"spirit";
            targetDegree = "";
        }
        else
        {
            targetName = target.token.displayName;
            targetColor = Utilities.GetSchool(target.token.degree).ToUpper();
            targetDegree = Utilities.GetDegree(target.token.degree).ToUpper();
        }

        if (response.Result.IsSuccess)
        {
            SpellData spellData = DownloadedAssets.GetSpell(response.Spell);

            if (target == PlayerManager.marker)
            {
				if (spellData != null)
					return LocalizeLookUp.GetText ("spell_caster_tried_spell_failed").Replace ("{{Color}}", casterColor).Replace ("{{Target Name}}", targetName).Replace ("{{Spell Name}}", spellData.Name);//"The " + casterColor + " " + targetName + " tried to cast " + spellData.spellName + " on you but failed.";
                else
					return LocalizeLookUp.GetText ("spell_caster_tried_failed").Replace ("{{Color}}", casterColor).Replace ("{{Target Name}}", targetName);//"The " + casterColor + " " + targetName + " tried to cast a spell on you but failed.";
            }
            else
            {
				if (spellData != null)
					return LocalizeLookUp.GetText ("spell_you_cast_spell_failed").Replace ("{{Spell Name}}", spellData.Name).Replace ("{{Color}}", casterColor).Replace ("{{Target Name}}", targetName);//"You tried to cast " + spellData.spellName + " on the " + casterColor + " " + targetName + " and failed.";
                else
					return LocalizeLookUp.GetText ("spell_you_cast_failed").Replace ("{{Color}}", casterColor).Replace ("{{Target Name}}", targetName);//"You tried to cast a spell on the " + casterColor + " " + targetName + " and failed.";
            }
        }

        string str = null;
        if (caster == PlayerManager.marker && DownloadedAssets.localizedText.ContainsKey(response.Spell + "_caster"))
        {
            str = DownloadedAssets.localizedText[response.Spell + "_caster"];
        }
        if (caster != PlayerManager.marker && DownloadedAssets.localizedText.ContainsKey(response.Spell + "_target"))
        {
            str = DownloadedAssets.localizedText[response.Spell + "_target"];
        }

        if (str != null)
        {
            if (target == PlayerManager.marker && caster.type != MarkerSpawner.MarkerType.SPIRIT)
            {
                str = str.Insert(7, "{1}");
            }
            else if (response.Spell == "attack")
            {
                str = str.Insert(21, "{1} ");
            }
            else if (caster.type == MarkerSpawner.MarkerType.SPIRIT)
            {
                str = str.Replace("{2}", "{1}");
            }

            if (str == null)
            {
                Debug.LogError($"empty feedback string for {response.Spell}");
                return null;
            }
            //Debug.Log("contains key");
            //Debug.Log("str format: " + str);
            string formatedString;
            try
            {
                formatedString = string.Format(
                    str,
                    casterName,
                    casterColor,
                    casterDegree,
                    targetName,
                    targetColor,
                    targetDegree,
                    damage,
                    damage
                    //response.result.resilienceChanged.ToString(),//power
                    //response.result.successChance.ToString(),//resilience
                    //intensityModifier,
                    //"1 min",
                    //response.result.newResilience.ToString(),
                    //response.result.newPower.ToString(),
                    //response.result.powerChanged.ToString(),
                    //response.result.selfEnergy.ToString()
                );
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error formating spell feedback string {str}: " + e.Message);
                formatedString = str;
            }
            return formatedString;
        }
        else //default feedback texts
        {
            Debug.LogError($"spell feedback not found for spell \"{response.Spell}\"");

            SpellData spellData = DownloadedAssets.GetSpell(response.Spell);

            if (spellData != null)
            {
                if (target == PlayerManager.marker)
                {
					if (response.Result.Damage > 0)
                    {
                        return LocalizeLookUp.GetText("spell_caster_spell_gain").Replace("{{Caster Name}}", casterName).Replace("{{Spell Name}}", spellData.Name).Replace("{{amount}}", "<color=yellow>" + damage + "</color>");
                        //$"{casterName} cast {spellData.spellName} on you. You gain <color=yellow>{damage}</color> Energy.";
                    }						
                    else if (response.Result.Damage < 0)
                    {
                        return LocalizeLookUp.GetText("spell_caster_spell_lose").Replace("{{Caster Name}}", casterName).Replace("{{Spell Name}}", spellData.Name).Replace("{{amount}}", "<color=red>" + damage + "</color>");
                        //$"{casterName} cast {spellData.spellName} on you. You lose <color=red>{damage}</color> Energy.";
                    }
                    else
                    {
                        return LocalizeLookUp.GetText("spell_caster_spell").Replace("{{Caster Name}}", casterName).Replace("{{Spell Name}}", spellData.Name);
                        //$"{casterName} cast {spellData.spellName} on you.";
                    }
                }
                else
                {
                    if (response.Result.Damage > 0)
                    {
                        return LocalizeLookUp.GetText("spell_you_target_gain").Replace("{{Spell Name}}", spellData.Name).Replace("{{Target Name}}", targetName).Replace("{{amount}}", "<color=yellow>" + damage + "</color>");
                        //$"You cast {spellData.spellName} on {targetName}. {targetName} gained <color=yellow>{damage}</color> Energy.";
                    }
                    else if (response.Result.Damage < 0)
                    {
                        return LocalizeLookUp.GetText("spell_you_target_lost").Replace("{{Spell Name}}", spellData.Name).Replace("{{Target Name}}", targetName).Replace("{{amount}}", "<color=red>" + damage + "</color>");
                        //$"You cast {spellData.spellName} on {targetName}. {targetName} lost <color=red>{damage}</color> Energy.";
                    }
                    else
                    {
                        return LocalizeLookUp.GetText("spell_you_target").Replace("{{Spell Name}}", spellData.Name).Replace("{{Target Name}}", targetName);
                        //$"You cast {spellData.spellName} on {targetName}.";
                    }
                }
            }
            else if (target == PlayerManager.marker)
            {
                if (response.Result.Damage > 0)
                {
                    return LocalizeLookUp.GetText("spell_caster_buff").Replace("{{Caster_Name}}", casterName).Replace("{{damage}}", damage.ToString());
                    //$"{casterName} buffed you. You gained {damage} energy.";
                }
                else if (response.Result.Damage < 0)
                {
                    return LocalizeLookUp.GetText("spell_caster_attacked").Replace("{{Target_Name}}", targetName).Replace("{{damage}}", damage.ToString());
                    //$"{targetName} attacked you. You lost {damage} energy.";
                }
                else return null;
            }
            else if (response.Result.Damage > 0)
            {
                return LocalizeLookUp.GetText("spell_you_buffed").Replace("{{Target_Name}}", targetName).Replace("{{damage}}", damage.ToString());
                //$"You buffed {targetName}. {targetName} gained {damage} energy.";
            }
            else if (response.Result.Damage < 0)
            {
                return LocalizeLookUp.GetText("spell_you_attacked").Replace("{{Target_Name}}", targetName).Replace("{{damage}}", damage.ToString());
                //$"You attacked {targetName}. {targetName} lost {damage} energy.";
            }
            else return null;
        }
    }
}
