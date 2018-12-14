using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamItemData : MonoBehaviour
{
    [Header("Text")]
    public Text level;
    public Text username;
    public Text status;
    public Text lastActiveOn;
    public Text controlledOn;
    public Text title;

    [Header("Buttons")]
    public Button acceptBtn;
    public Button rejectBtn;
    public Button allyBtn;
    public Button unAllyBtn;
    public Button visitBtn;
    public Button playerButton;
    public Button levelCloseBtn;
    public Button cancelBtn;

    [Header("Other")]
    public GameObject rankIcon;
    public GameObject highlight;
    public Button kickButton;
    // public 

    public void Setup(TeamMember data)
    {
        level.text = data.level.ToString();
        username.text = data.displayName;
        title.text = data.title;
        rankIcon.SetActive(data.role == 2);
        lastActiveOn.text = GetlastActive(data.lastActiveOn);
        status.text = data.state == "" ? "Normal" : data.state;
        kickButton.gameObject.SetActive(false);
        highlight.SetActive(false);
        playerButton.onClick.AddListener(() => { TeamManagerUI.Instance.SendViewCharacter(data.displayName); });
    }

    public void Setup(TeamLocation data)
    {

    }

    public void Setup(TeamInvites data)
    {
        //invite sent from my coven to other players
        if (string.IsNullOrEmpty(data.covenName))
        {
            level.text = "";
            username.text = data.displayName;

            playerButton.onClick.RemoveAllListeners();
            playerButton.onClick.AddListener(() => { TeamManagerUI.Instance.SendViewCharacter(data.displayName); });

            acceptBtn.gameObject.SetActive(false);
            rejectBtn.gameObject.SetActive(false);
            
            cancelBtn.onClick.RemoveAllListeners();
            cancelBtn.onClick.AddListener(() => TeamManagerUI.Instance.SendCancel(data));
            cancelBtn.gameObject.SetActive(true);
        }
        //invite sent from other covens to me
        else
        {
            level.text = "";
            username.text = data.covenName;

            playerButton.onClick.RemoveAllListeners();
            playerButton.onClick.AddListener(() => TeamManagerUI.Instance.ShowCovenInfo(data.covenName));

            cancelBtn.gameObject.SetActive(false);

            rejectBtn.onClick.RemoveAllListeners();
            rejectBtn.onClick.AddListener(() => OnClickRejectInvite(data));
            rejectBtn.gameObject.SetActive(true);

            acceptBtn.onClick.RemoveAllListeners();
            acceptBtn.onClick.AddListener(() => OnClickAcceptInvite(data));
            acceptBtn.gameObject.SetActive(true);
        }
    }

    private void OnClickRejectInvite(TeamInvites data)
    {

    }

    private void OnClickAcceptInvite(TeamInvites data)
    {

    }

    public void Setup(TeamInviteRequest data)
    {
        level.text = data.level.ToString();
        username.text = data.displayName;

        playerButton.onClick.RemoveAllListeners();
        playerButton.onClick.AddListener(() => { TeamManagerUI.Instance.SendViewCharacter(data.displayName); });

        rejectBtn.onClick.RemoveAllListeners();
        rejectBtn.onClick.AddListener(() => OnClickRejectRequest(data));
        acceptBtn.onClick.RemoveAllListeners();
        acceptBtn.onClick.AddListener(() => OnClickAcceptRequest(data));
    }

    private void OnClickRejectRequest(TeamInviteRequest data)
    {
        TeamManager.CovenReject(
            (result) =>
            {
                if (result == 200)
                {
                    //updat the cached invites and update the UI
                    List<TeamInviteRequest> pUpdatedInvites = TeamUIHelper.Instance.lastRequests;
                    pUpdatedInvites.Remove(data);
                    TeamUIHelper.Instance.CreateRequests(pUpdatedInvites.ToArray());
                }
                else
                {

                }
            }, 
            data.request
        );
    } 

    private void OnClickAcceptRequest(TeamInviteRequest data)
    {
        TeamManager.InviteCoven(
            (result) =>
            {
                if (result == 200)
                {
                    //updat the cached invites and update the UI
                    List<TeamInviteRequest> pUpdatedInvites = TeamUIHelper.Instance.lastRequests;
                    pUpdatedInvites.Remove(data);
                    TeamUIHelper.Instance.CreateRequests(pUpdatedInvites.ToArray());
                }
                else
                {

                }
            },
            data.displayName
        );
    }

    static string GetlastActive(double javaTimeStamp)
    {
        if (javaTimeStamp < 159348924)
        {
            string s = "unknown";
            return s;
        }

        System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToUniversalTime();
        var timeSpan = dtDateTime.Subtract(System.DateTime.UtcNow);
        string stamp = "";

        if (timeSpan.TotalDays > 1)
        {
            stamp = (Mathf.Abs((int)timeSpan.TotalDays)).ToString() + " days ago";
        }
        else if (timeSpan.TotalHours > 1)
        {
            stamp = (Mathf.Abs((int)timeSpan.TotalHours)).ToString() + " hours ago";

        }
        else if (timeSpan.TotalMinutes > 5)
        {
            stamp = (Mathf.Abs((int)timeSpan.TotalMinutes)).ToString() + " mins ago";
        }
        else
        {
            stamp = "Active";
        }

        return stamp;
    }

}
