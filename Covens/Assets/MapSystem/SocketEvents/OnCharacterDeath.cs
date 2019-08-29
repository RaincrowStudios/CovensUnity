using UnityEngine;
using System.Collections;
using Raincrow.GameEventResponses;
using Raincrow.Maps;

public static class OnCharacterDeath
{
    public static void HandleEvent(string name, string type, string reason)
    {
        UnityEngine.UI.Text txt = PlayerManagerUI.Instance.deathDesc;

        if (name == PlayerDataManager.playerData.name)
        {
            if (reason == "spell")
            {
                txt.text = LocalizeLookUp.GetText("ui_response_spell");
            }
            else if (reason == "summon")
            {
                txt.text = LocalizeLookUp.GetText("ui_response_summon");
            }
        }
        else   
        {
            if (type != "spirit")
            {
                txt.text = LocalizeLookUp.GetText("ui_response_witch").Replace("{{Name}}", name).Replace("{{Witch Type}}", "");
            }
            else
            {
                //if (string.IsNullOrEmpty(owner))
                //{
                //    txt.text = LocalizeLookUp.GetText("ui_response_spirit_wild")
                //        .Replace("{{Spirit Name}}", LocalizeLookUp.GetSpiritName(name));
                //}
                //else
                {
                    txt.text = LocalizeLookUp.GetText("ui_response_spirit")
                        .Replace("{{Spirit}}", LocalizeLookUp.GetSpiritName(name))
                        .Replace("{{Name}}", "");
                }
            }
        }

        PlayerManagerUI.Instance.ShowDeathReason();
    }

    public static void CheckSummonDeath(string spirit, int cost)
    {
        if (PlayerDataManager.playerData.energy <= cost)
            HandleEvent(PlayerDataManager.playerData.name, "witch", "summon");
    }

    public static void CheckSpellDeath(SpellCastHandler.SpellCastEventData data)
    {
        bool isCaster = data.caster.name == PlayerDataManager.playerData.name;
        bool isTarget = data.target.name == PlayerDataManager.playerData.name;

        bool died = (isCaster && data.caster.energy == 0) || (isTarget && data.target.energy == 0);

        if (!died)
            return;

        int previousEnergy = PlayerDataManager.playerData.energy;
        SpellData spell = DownloadedAssets.GetSpell(data.spell);

        if (isCaster && spell.cost > previousEnergy)
        {
            HandleEvent(PlayerDataManager.playerData.name, "witch", "spell");
            return;
        }

        if (isTarget)
        {
            if (data.caster.Type == MarkerManager.MarkerType.SPIRIT)
            {
                HandleEvent(data.caster.name, "spirit", "spell");
            }
            else  if (data.caster.Type == MarkerManager.MarkerType.WITCH)
            {
                HandleEvent(data.caster.name, "witch", "spell");
            }
        }
    }
}
