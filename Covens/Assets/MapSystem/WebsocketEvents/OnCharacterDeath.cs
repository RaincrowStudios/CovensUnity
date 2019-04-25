using UnityEngine;
using System.Collections;

public static class OnCharacterDeath
{
    public static event System.Action<string, string> OnPlayerDead;

    public static void HandleEvent(WSData data)
    {
        OnPlayerDead?.Invoke(data.displayName, data.spirit);

        LocalizeLookUp LLU_DeathDesc = PlayerManagerUI.Instance.deathDesc.GetComponent<LocalizeLookUp>();

        string msg = "";

        if (data.displayName == PlayerDataManager.playerData.displayName)
        {
            if (data.action.Contains("spell"))
            {
                msg = "You used the last of your energy with that spell.";
                LLU_DeathDesc.id = "ui_response_spell";
            }
            else if (data.action == "portal")
            {
                msg = "You used all of your energy attacking that portal.";
                LLU_DeathDesc.id = "ui_response_portal";
            }
            else if (data.action == "summon")
            {
                msg = "You used all of your energy in the summoning ritual.";
                LLU_DeathDesc.id = "ui_response_summon";
            }
            else if (data.action == "backfire")
            {
                msg = "Oh, dear. You were close to a Signature spell, but one wrong ingredient caused this spell to backfire.";
                LLU_DeathDesc.id = "ui_response_backfire";
            }
        }
        else
        {
            if (data.spirit == "")
            {
                LLU_DeathDesc.id = "ui_response_witch";
                string s = "";
                if (data.degree < 0)
                    s += "Shadow witch";
                else if (data.degree > 0)
                    s += "White witch";
                else
                    s = "Grey witch";
                PlayerManagerUI.Instance.deathDesc.text = PlayerManagerUI.Instance.deathDesc.text.Replace("{{Witch Type}}", s)
                    .Replace("{{Name}}", data.displayName);

                msg = "The " + s + data.displayName + " has taken all your energy.";
            }
            else
            {
                if (data.displayName != "")
                {
                    msg = data.displayName + "'s " + DownloadedAssets.spiritDictData[data.spirit].spiritName + " has attacked you, taking all of your energy.";
                    LLU_DeathDesc.id = "ui_response_spirit";
                    PlayerManagerUI.Instance.deathDesc.text = PlayerManagerUI.Instance.deathDesc.text.Replace("{{Name}}", data.displayName)
                        .Replace("{{Spirit}}", DownloadedAssets.spiritDictData[data.spirit].spiritName);
                }
                else
                {
                    msg = "The wild spirit " + DownloadedAssets.spiritDictData[data.spirit].spiritName + " has attacked you, taking all of your energy.";
                    LLU_DeathDesc.id = "ui_response_spirit_wild";
                    PlayerManagerUI.Instance.deathDesc.text = PlayerManagerUI.Instance.deathDesc.text.Replace("{{Spirit}}", DownloadedAssets.spiritDictData[data.spirit].spiritName);
                }
            }

        }

        PlayerManagerUI.Instance.ShowDeathReason(msg);
    }
}
