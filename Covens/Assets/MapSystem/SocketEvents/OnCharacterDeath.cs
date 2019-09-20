using UnityEngine;
using System.Collections;
using Raincrow.GameEventResponses;
using Raincrow.Maps;

public static class OnCharacterDeath
{
    public static System.Action<string> OnSummonDeath;
    public static System.Action<string> OnSpiritDeath;
    public static System.Action<string> OnWitchDeath;
    public static System.Action<string> OnCastSuicide;

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

    public static void ShowSpellCastSuicide()
    {
        HandleEvent(PlayerDataManager.playerData.name, "witch", "spell");
    }

    public static void ShowSpiritDeath(string spiritName)
    {
        HandleEvent(spiritName, "spirit", "spell");
    }

    public static void ShowWitchDeath(string witchName)
    {
        HandleEvent(witchName, "witch", "spell");
    }

    public static void ShowSummonDeath()
    {
        HandleEvent(PlayerDataManager.playerData.name, "witch", "summon");
    }
}
