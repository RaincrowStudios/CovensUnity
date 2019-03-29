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
        sendRequestText.text = "Send Request";
        covenTitle.text = CD.name + " " + Utilities.GetSchoolCoven(CD.alignment);
        worldRank.text = $"Rank: <b><color=white>{CD.worldRank.ToString()}";
        xp.text = $"XP: <b><color=white>{CD.xp.ToString()}";
        level.text = $"Level: <b><color=white>{CD.level.ToString()}";
        members.text = $"Members: <b><color=white>{CD.members.ToString()}";
        founder.text = $"Founder: <b><color=white>{CD.founder.ToString()}";
        sendRequest.onClick.AddListener(() =>
        {
            sendRequest.interactable = false;
            TeamManager.RequestInvite((int r) =>
            {
                if (r == 200)
                {
                    sendRequestText.text = "Sent";
                }
                else
                {
                    sendRequestText.text = "Failed";
                }
            }, CD.name);
        });
    }
}