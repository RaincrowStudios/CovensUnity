using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatCovenItem : MonoBehaviour
{
    public TextMeshProUGUI covenTitle;
    public TextMeshProUGUI worldRank;
    public TextMeshProUGUI members;
    public TextMeshProUGUI founder;
    public TextMeshProUGUI level;
    public TextMeshProUGUI xp;
    public Button sendRequest;
    public TextMeshProUGUI sendRequestText;
    public Image dp;
    public void Setup(ChatCovenData CD)
    {
        if (CD.alignment < 0)
            dp.color = Utilities.Purple;
        else if (CD.alignment > 0)
            dp.color = Utilities.Orange;
        else
            dp.color = Utilities.Blue;
		sendRequestText.text = LocalizeLookUp.GetText ("invite_send_request");// "Send Request";
        covenTitle.text = CD.name + " " + Utilities.GetSchoolCoven(CD.alignment);
		worldRank.text = LocalizeLookUp.GetText("lt_world_rank") + "<b><color=white>" + CD.worldRank.ToString();
		xp.text = LocalizeLookUp.GetText("spell_xp").Replace("{{Number}}", "<b><color=white>" + CD.xp.ToString() + "</b></color>");
		level.text = LocalizeLookUp.GetText ("card_witch_level") + ": " + "<b><color=white>" + CD.level.ToString();
		members.text = LocalizeLookUp.GetText("invite_member").Replace("{{member}}", "<b><color=white>" + CD.members.ToString());
		founder.text = LocalizeLookUp.GetText("coven_founder") + "<b><color=white>" + CD.founder.ToString();
        sendRequest.onClick.AddListener(() =>
        {
            Debug.LogError("TODO: SEND REQUEST");
      //      sendRequest.interactable = false;
      //      TeamManager.SendRequest(
      //          CD.name,
      //          (int r, string s) =>  {
      //              if (r == 200)
      //              {
						//sendRequestText.text = LocalizeLookUp.GetText("coven_request_success");// "Sent";
      //              }
      //              else
      //              {
						//sendRequestText.text = LocalizeLookUp.GetText("lt_failed");// "Failed";
      //              }
      //          });
        });
    }
}