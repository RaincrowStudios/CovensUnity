using Raincrow.GameEventResponses;
using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellcastingTextFeedback
{
    public static string CreateSpellFeedback(IMarker caster, IMarker target, Raincrow.GameEventResponses.SpellCastHandler.SpellCastEventData response)
    {
        //basic caster/target info
        string casterName, targetName;
        string casterColor, targetColor;
        string casterDegree, targetDegree;
        int damage = Mathf.Abs((int)response.result.amount);
        int resilienceMod = 0;
        string spellSuccessMod = "";
        string intensityMod = "";
        string statusDuration = "";
        int newResilience = PlayerDataManager.playerData.GetResilience(PlayerDataManager.playerData.effects);
        int newPower = PlayerDataManager.playerData.GetPower(PlayerDataManager.playerData.effects);
        int powerMod = 0;
        int selfEnergyChange = 0;

        //setup spirit/witch specific info
        if (caster == PlayerManager.marker)
        {
            casterName = casterColor = casterDegree = "";
        }
        else if (caster.Type == MarkerSpawner.MarkerType.SPIRIT)
        {
            casterName = LocalizeLookUp.GetSpiritName((caster.Token as SpiritToken).spiritId);
            casterColor = "spirit";
            casterDegree = "";
        }
        else if (caster.Type == MarkerSpawner.MarkerType.WITCH)
        {
            casterName = (caster.Token as WitchToken).displayName;
            casterColor = Utilities.GetSchool((caster.Token as CharacterToken).degree).ToUpper();
            casterDegree = Utilities.GetDegree((caster.Token as CharacterToken).degree);
        }
        else
        {
            casterName = LocalizeLookUp.GetSpiritName((caster.Token as BossToken).spiritId);
            casterColor = "";
            casterDegree = "";
        }

        if (target == PlayerManager.marker)
        {
            targetName = targetColor = targetDegree = "";
        }
        else if (target.Type == MarkerSpawner.MarkerType.SPIRIT)
        {
            targetName = LocalizeLookUp.GetSpiritName((target.Token as SpiritToken).spiritId);
            targetColor = LocalizeLookUp.GetText("lt_spirit_s");//"spirit";
            targetDegree = "";
        }
        else if (target.Type == MarkerSpawner.MarkerType.WITCH)
        {
            targetName = (target.Token as WitchToken).displayName;
            targetColor = Utilities.GetSchool((target.Token as WitchToken).degree).ToUpper();
            targetDegree = Utilities.GetDegree((target.Token as WitchToken).degree).ToUpper();
        }
        else
        {
            targetName = LocalizeLookUp.GetSpiritName((target.Token as BossToken).spiritId);
            targetColor = "";
            targetDegree = "";
        }

        if (response.result.isSuccess == false)
        {
            SpellData spellData = DownloadedAssets.GetSpell(response.spell);

            if (target == PlayerManager.marker)
            {
                if (spellData != null)
                    return LocalizeLookUp.GetText("spell_caster_tried_spell_failed").Replace("{{Color}}", casterColor).Replace("{{Target Name}}", casterName).Replace("{{Spell Name}}", spellData.Name);//"The " + casterColor + " " + targetName + " tried to cast " + spellData.spellName + " on you but failed.";
                else
                    return LocalizeLookUp.GetText("spell_caster_tried_failed").Replace("{{Color}}", casterColor).Replace("{{Target Name}}", casterName);//"The " + casterColor + " " + targetName + " tried to cast a spell on you but failed.";
            }
            else
            {
                if (spellData != null)
                    return LocalizeLookUp.GetText("spell_you_cast_spell_failed").Replace("{{Spell Name}}", spellData.Name).Replace("{{Color}}", casterColor).Replace("{{Target Name}}", targetName);//"You tried to cast " + spellData.spellName + " on the " + casterColor + " " + targetName + " and failed.";
                else
                    return LocalizeLookUp.GetText("spell_you_cast_failed").Replace("{{Color}}", casterColor).Replace("{{Target Name}}", targetName);//"You tried to cast a spell on the " + casterColor + " " + targetName + " and failed.";
            }
        }

        string str = null;
        if (caster == PlayerManager.marker && DownloadedAssets.LocalizationDictionary.ContainsKey(response.spell + "_caster"))
        {
            str = DownloadedAssets.LocalizationDictionary[response.spell + "_caster"];
        }
        if (caster != PlayerManager.marker && DownloadedAssets.LocalizationDictionary.ContainsKey(response.spell + "_target"))
        {
            str = DownloadedAssets.LocalizationDictionary[response.spell + "_target"];
        }

        if (str != null)
        {
            if (target == PlayerManager.marker && caster.Type != MarkerSpawner.MarkerType.SPIRIT)
            {
                str = str.Insert(7, "{1}");
            }
            else if (response.spell == "attack")
            {
                str = str.Insert(21, "{1} ");
                str = str.Replace("{6}", "<color=red>{6}</color>");
            }
            else if (caster.Type != MarkerSpawner.MarkerType.WITCH) //== MarkerSpawner.MarkerType.SPIRIT)
            {
                str = str.Replace("{2}", "{1}");
            }

            if (str == null)
            {
                Debug.LogError($"empty feedback string for {response.spell}");
                return null;
            }

            //Debug.Log("contains key");
            //Debug.Log("str format: " + str);
            string formatedString;
            try
            {
                formatedString = string.Format(
                    str,

                    casterName,     //0
                    casterColor,    //1
                    casterDegree,   //2
                    targetName,     //3
                    targetColor,    //4
                    targetDegree,   //5
                    damage,         //6 
                    damage,         //7
                    resilienceMod,  //8
                    spellSuccessMod,//9
                    intensityMod,   //10
                    statusDuration, //11
                    newResilience,  //12
                    newPower,       //13
                    powerMod,       //14
                    selfEnergyChange//15
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
            Debug.LogError($"spell feedback not found for spell \"{response.spell}\"");

            SpellData spellData = DownloadedAssets.GetSpell(response.spell);

            if (spellData != null)
            {
                if (target == PlayerManager.marker)
                {
                    if (response.result.amount > 0)
                    {
                        return LocalizeLookUp.GetText("spell_caster_spell_gain").Replace("{{Caster Name}}", casterName).Replace("{{Spell Name}}", spellData.Name).Replace("{{amount}}", "<color=yellow>" + damage + "</color>");
                        //$"{casterName} cast {spellData.spellName} on you. You gain <color=yellow>{damage}</color> Energy.";
                    }
                    else if (response.result.amount < 0)
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
                    if (response.result.amount > 0)
                    {
                        return LocalizeLookUp.GetText("spell_you_target_gain").Replace("{{Spell Name}}", spellData.Name).Replace("{{Target Name}}", targetName).Replace("{{amount}}", "<color=yellow>" + damage + "</color>");
                        //$"You cast {spellData.spellName} on {targetName}. {targetName} gained <color=yellow>{damage}</color> Energy.";
                    }
                    else if (response.result.amount < 0)
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
                if (response.result.amount > 0)
                {
                    return LocalizeLookUp.GetText("spell_caster_buff").Replace("{{Caster_Name}}", casterName).Replace("{{damage}}", "<color=yellow>" + damage.ToString() + "</color>");
                    //$"{casterName} buffed you. You gained {damage} energy.";
                }
                else if (response.result.amount < 0)
                {
                    return LocalizeLookUp.GetText("spell_caster_attacked").Replace("{{Target_Name}}", targetName).Replace("{{damage}}", "<color=red>" + damage.ToString() + "</color>");
                    //$"{targetName} attacked you. You lost {damage} energy.";
                }
                else return null;
            }
            else if (response.result.amount > 0)
            {
                return LocalizeLookUp.GetText("spell_you_buffed").Replace("{{Target_Name}}", targetName).Replace("{{damage}}", "<color=yellow>" + damage.ToString() + "</color>");
                //$"You buffed {targetName}. {targetName} gained {damage} energy.";
            }
            else if (response.result.amount < 0)
            {
                return LocalizeLookUp.GetText("spell_you_attacked").Replace("{{Target_Name}}", targetName).Replace("{{damage}}", "<color=red>" + damage.ToString() + "</color>");
                //$"You attacked {targetName}. {targetName} lost {damage} energy.";
            }
            else return null;
        }
    }
}
