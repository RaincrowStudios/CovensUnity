using UnityEngine;
using System.Collections;

public static class OnCharacterDeath
{
    public static void HandleEvent(WSData data)
    {
        //    LocalizeLookUp LLU_DeathDesc = PlayerManagerUI.Instance.deathDesc.GetComponent<LocalizeLookUp>();
        UnityEngine.UI.Text txt = PlayerManagerUI.Instance.deathDesc;
        string msg = "";
        Debug.Log(PlayerDataManager.playerData.name);

        if (data.displayName == PlayerDataManager.playerData.name)
        {
            Debug.Log("your own player");

            if (data.action.Contains("spell"))
            {
                msg = "You used the last of your energy with that spell.";
                txt.text = LocalizeLookUp.GetText("ui_response_spell");
            }
            else if (data.action == "portal")
            {
                msg = "You used all of your energy attacking that portal.";
                txt.text = LocalizeLookUp.GetText("ui_response_portal");
            }
            else if (data.action == "summon")
            {
                msg = "You used all of your energy in the summoning ritual.";
                txt.text = LocalizeLookUp.GetText("ui_response_summon");
            }
            else if (data.action == "backfire")
            {
                msg = "Oh, dear. You were close to a Signature spell, but one wrong ingredient caused this spell to backfire.";
                txt.text = LocalizeLookUp.GetText("ui_response_backfire");
            }
        }
        else   
        {
            if (data.spirit == "")
            {
                // LLU_DeathDesc.id = "ui_response_witch";
                string s = "";
                if (data.degree < 0)
                    s += LocalizeLookUp.GetText("ui_response_shadow");//"Shadow witch";
                else if (data.degree > 0)
                    s += LocalizeLookUp.GetText("ui_response_white");//White witch";
                else
                    s = LocalizeLookUp.GetText("ui_response_grey");//Grey witch";
                string p = DownloadedAssets.localizedText["ui_response_witch"];

                p = p.Replace("{{Name}}", data.displayName);
                p = p.Replace("{{Witch Type}}", s);

                txt.text = p;
                Debug.Log(p);


            }
            else
            {
                if (data.displayName != "")
                {
                    // msg = data.displayName + "'s " + DownloadedAssets.spiritDictData[data.spirit].spiritName + " has attacked you, taking all of your energy.";

                    msg = LocalizeLookUp.GetText("ui_response_spirit");
                    msg = msg.Replace("{{Spirit}}", LocalizeLookUp.GetSpiritName(data.spirit));
                    msg = msg.Replace("{{Name}}", data.displayName);
                    txt.text = msg;
                    Debug.Log(msg);
                    // LLU_DeathDesc.id = "ui_response_spirit";
                    // PlayerManagerUI.Instance.deathDesc.text = PlayerManagerUI.Instance.deathDesc.text.Replace("{{Name}}", data.displayName)
                    //     .Replace("{{Spirit}}", DownloadedAssets.spiritDictData[data.spirit].spiritName);
                }
                else
                {
                    msg = LocalizeLookUp.GetText("ui_response_spirit_wild");
                    //msg = DownloadedAssets.localizedText["ui_response_spirit_wild"].value;
                    //Debug.Log(DownloadedAssets.spiritDictData[data.spirit].spiritName);
                    msg = msg.Replace("{{Spirit Name}}", LocalizeLookUp.GetSpiritName(data.spirit));
                    txt.text = msg;
                    Debug.Log(msg);

                }
            }

        }

        PlayerManagerUI.Instance.ShowDeathReason();
    }
}
