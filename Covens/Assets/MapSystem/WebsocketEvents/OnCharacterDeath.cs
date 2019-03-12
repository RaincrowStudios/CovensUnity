using UnityEngine;
using System.Collections;

public static class OnCharacterDeath 
{
    public static event System.Action<string, string> OnPlayerDead;

    public static void HandleEvent(WSData data)
    {
        OnPlayerDead?.Invoke(data.displayName, data.spirit);

        string msg = "";

        if (data.displayName == PlayerDataManager.playerData.displayName)
        {
            if (data.action.Contains("spell"))
            {
                msg = "You used the last of your energy with that spell.";
            }
            else if (data.action == "portal")
            {
                msg = "You used all of your energy attacking that portal.";
            }
            else if (data.action == "summon")
            {
                msg = "You used all of your energy in the summoning ritual.";
            }
            else if (data.action == "backfire")
            {
                msg = "Oh, dear. You were close to a Signature spell, but one wrong ingredient caused this spell to backfire.";
            }
        }
        else
        {
            if (data.spirit != "")
            {
                string s = "";
                if (data.degree < 0)
                    s += " Shadow witch ";
                else if (data.degree > 0)
                    s += " White witch ";
                else
                    s = "Grey witch ";


                msg = "The " + s + data.displayName + " has taken all your energy.";
            }
            else
            {
                msg = data.displayName + "'s " + DownloadedAssets.spiritDictData[data.spirit].spiritName + " has attacked you, taking all of your energy.";
            }

        }
        PlayerManagerUI.Instance.ShowDeathReason(msg);
    }
}
