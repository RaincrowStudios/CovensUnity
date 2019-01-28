using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Oktagon.Localization;

public class TeamItemData : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI level;
    public TextMeshProUGUI username;
    public TextMeshProUGUI status;
    public TextMeshProUGUI lastActiveOn;
    public TextMeshProUGUI controlledOn;
    public TextMeshProUGUI title;

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
    public GameObject adminIcon;
    public GameObject modIcon;
    public GameObject highlight;

    public CanvasGroup editGroup;
    public Button kickButton;
    public Button promoteButton;
    public TMP_InputField titleField;

    private TeamMember memberData;

    public void Setup(TeamMember data)
    {
        level.text = data.level.ToString();
        username.text = data.displayName;
        title.text = data.title;
        adminIcon.SetActive(data.role == 2);
        modIcon.SetActive(data.role == 1);
        lastActiveOn.text = GetlastActive(data.lastActiveOn);
        status.text = data.state == "" ? "Normal" : data.state;
        kickButton.gameObject.SetActive(false);
        highlight.SetActive(false);
        playerButton.onClick.AddListener(() => { TeamManagerUI.Instance.SendViewCharacter(data.displayName); });

        memberData = data;
        if (editGroup)
        {
            editGroup.gameObject.SetActive(false);
            editGroup.alpha = 0f;
        }
    }

    public void EnableEdit(bool enable)
    {
        float valueStart = editGroup.alpha;
        float valueTarget = enable ? 1 : 0;
        TeamManager.CovenRole myRole = TeamManager.CurrentRole;

        //enable and setup listeners
        if (enable)
        {
            editGroup.gameObject.SetActive(true);

            //can only edit a members with lower role
            bool showEditOptions = (int)myRole > memberData.role;
            bool showTitleEdit = (int)myRole > memberData.role || myRole == TeamManager.CovenRole.Administrator;

            if (showEditOptions)
            {
                promoteButton.onClick.AddListener(OnClickPromotePlayer);
                kickButton.onClick.AddListener(OnClickKickPlayer);
            }
            if (showTitleEdit)
            {
                titleField.text = memberData.title;
                titleField.onEndEdit.AddListener(OnFinishEditingTitle);
            }

            titleField.gameObject.SetActive(showTitleEdit);
            promoteButton.gameObject.SetActive(showEditOptions);
            kickButton.gameObject.SetActive(showEditOptions);
        }

        LeanTween.value(valueStart, valueTarget, 0.2f)
            .setOnUpdate((float t) =>
            {
                //tween alpha and scale
                editGroup.alpha = t;
                //kickButton.transform.localScale = Vector2.one * t;
                //promoteButton.transform.localScale = Vector2.one * t;
                //titleField.transform.localScale = Vector2.one * t;
            })
            .setOnComplete(() =>
            {
                //disable and clear listeners
                if (enable == false)
                {
                    editGroup.gameObject.SetActive(false);
                    titleField.onEndEdit.RemoveAllListeners();
                    kickButton.onClick.RemoveAllListeners();
                    promoteButton.onClick.RemoveAllListeners();
                }
            })
            .setEaseInOutCubic();
    }

    private void OnFinishEditingTitle(string title)
    {
        if (title == memberData.title)
            return;

        TeamManager.CovenSetTitle(
            (result) => {
                if (result != 200)
                {
                    //show error popup
                }
            },
            memberData.displayName,
            titleField.text
        );
    }

    private void OnClickPromotePlayer()
    {
        TeamManager.CovenRole role = (TeamManager.CovenRole)memberData.role + 1;
        string playerName = memberData.displayName;
        TeamManagerUI.Instance.SendPromote(playerName, role);
    }

    private void OnClickKickPlayer()
    {
        TeamManagerUI.Instance.KickCovenMember(memberData.displayName, () =>
        {
            //todo: use a websocketevent to inform a coven member was kicked
            Destroy(this.gameObject);
            TeamUIHelper.Instance.uiItems.Remove(memberData.displayName);
        });
    }

    public void SetupAlly(TeamAlly ally)
    {
        level.text = ally.rank.ToString();
        username.text = ally.covenName;

        playerButton.onClick.RemoveAllListeners();
        allyBtn.onClick.RemoveAllListeners();
        unAllyBtn.onClick.RemoveAllListeners();

        playerButton.onClick.AddListener(() => TeamManagerUI.Instance.ShowCovenInfo(ally.covenName));
        unAllyBtn.onClick.AddListener(() => OnClickUnally(ally));

        allyBtn.transform.parent.gameObject.SetActive(false);
        unAllyBtn.transform.parent.gameObject.SetActive(true);
    }

    private void OnClickUnally(TeamAlly ally)
    {
        TeamManagerUI.ConfirmPopup.ShowPopUp(
            () => {
                TeamManagerUI.Instance.SendCovenUnally(ally.covenName);
            },
            () => { },
            "Do you want to unally with this coven?"
        );
    }

    public void SetupAllied(TeamAlly allied)
    {
        level.text = allied.rank.ToString();
        username.text = allied.covenName;

        playerButton.onClick.RemoveAllListeners();
        allyBtn.onClick.RemoveAllListeners();
        unAllyBtn.onClick.RemoveAllListeners();

        playerButton.onClick.AddListener(() => TeamManagerUI.Instance.ShowCovenInfo(allied.covenName));
        allyBtn.onClick.AddListener(() => OnClickAlly(allied));

        allyBtn.transform.parent.gameObject.SetActive(true);
        unAllyBtn.transform.parent.gameObject.SetActive(false);
    }

    private void OnClickAlly(TeamAlly ally)
    {
        TeamManagerUI.ConfirmPopup.ShowPopUp(
            () => {
                TeamManagerUI.Instance.SendCovenAllyRequest(ally.covenName);
            },
            () => { }, 
            "Do you want to ally with this coven?"
        );
    }

    public void Setup(TeamLocation data)
    {
        title.text = data.displayName;
        controlledOn.text = GetTimeAgo(data.controlledOn);

        visitBtn.onClick.RemoveAllListeners();
        visitBtn.onClick.AddListener(() => OnClickVisitLocation(data));
    }

    private void OnClickVisitLocation(TeamLocation data)
    {
        PlayerManager.Instance.FlyTo(data.longitude, data.latitude);
        TeamManagerUI.Instance.Close();
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

            acceptBtn.transform.parent.gameObject.SetActive(false);
            rejectBtn.transform.parent.gameObject.SetActive(false);
            
            cancelBtn.onClick.RemoveAllListeners();
            cancelBtn.onClick.AddListener(() => TeamManagerUI.Instance.SendCancel(data));
            cancelBtn.transform.parent.gameObject.SetActive(true);
        }
        //invite sent from other covens to me
        else
        {
            level.text = "";
            username.text = data.covenName;

            playerButton.onClick.RemoveAllListeners();
            playerButton.onClick.AddListener(() => TeamManagerUI.Instance.ShowCovenInfo(data.covenName));

            cancelBtn.transform.parent.gameObject.SetActive(false);

            rejectBtn.onClick.RemoveAllListeners();
            rejectBtn.onClick.AddListener(() => OnClickDeclineInvite(data));
            rejectBtn.transform.parent.gameObject.SetActive(true);

            acceptBtn.onClick.RemoveAllListeners();
            acceptBtn.onClick.AddListener(() => OnClickAcceptInvite(data));
            acceptBtn.transform.parent.gameObject.SetActive(true);
        }
    }

    private void OnClickDeclineInvite(TeamInvites data)
    {
        TeamManager.CovenDecline(
            (result) =>
            {
                if (result == 200)
                {
                    TeamUIHelper.Instance.uiItems.Remove(data.covenName);
                    if(TeamUIHelper.Instance.uiItems.Count == 0)
                        Utilities.InstantiateObject(TeamUIHelper.Instance.emptyPrefab, TeamUIHelper.Instance.container);
                    Destroy(this.gameObject);
                }
                else
                {
                    if (result == 4300)
                        OnInvitationAlreadyCanceled(data);
                }
            },
            data.inviteToken
        );
    }

    private void OnClickAcceptInvite(TeamInvites data)
    {
        TeamManager.JoinCoven(
            (result) =>
            {
                if (result == 200)
                {
                    TeamManagerUI.ConfirmPopup.ShowPopUp(
                        cancelAction: () =>
                        {
                            PlayerDataManager.playerData.covenName = data.covenName;
                            TeamManagerUI.Instance.SetScreenType(TeamManagerUI.ScreenType.CovenDisplay);
                        }, 
                        txt: $"You are now a member of {data.covenName}"
                    );
                    PlayerDataManager.playerData.covenName = data.covenName;
                }
                else
                {
                    if (result == 4300)
                        OnInvitationAlreadyCanceled(data);
                }
            },
            data.inviteToken
        );
    }

    private void OnInvitationAlreadyCanceled(TeamInvites data)
    {
        if (TeamUIHelper.Instance.uiItems.ContainsKey(data.covenName))
            TeamUIHelper.Instance.uiItems.Remove(data.covenName);
        TeamManagerUI.ConfirmPopup.ShowPopUp(() => { }, "Your invitation was already revoked.");
        Destroy(this.gameObject);
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
                    TeamUIHelper.Instance.uiItems.Remove(data.displayName);
                    Destroy(this.gameObject);
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
                    TeamUIHelper.Instance.uiItems.Remove(data.displayName);
                    Destroy(this.gameObject);
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
            return "unknown";
        }

        System.TimeSpan timeSpan = GetTimespan(javaTimeStamp);
        string stamp = "";

        if (timeSpan.TotalDays > 1)
        {
            stamp = ((int)timeSpan.TotalDays).ToString() + " days ago";
        }
        else if (timeSpan.TotalHours > 1)
        {
            stamp = ((int)timeSpan.TotalHours).ToString() + " hours ago";

        }
        else if (timeSpan.TotalMinutes > 5)
        {
            stamp = ((int)timeSpan.TotalMinutes).ToString() + " mins ago";
        }
        else
        {
            stamp = "Active";
        }

        return stamp;
    }

    static string GetTimeAgo(double javaTimestamp)
    {
        if (javaTimestamp < 159348924)
        {
            return "unknown";
        }
        else
        {
            System.TimeSpan timeSpan = GetTimespan(javaTimestamp);
            string text = "";

            if(timeSpan.TotalDays > 30)
            {
                int months = Mathf.Abs((int)timeSpan.TotalDays) / 30;
                text = months.ToString() + (months == 1 ? "month" : "months") + " ago";
            }
            else if (timeSpan.TotalDays > 1)
            {
                text = ((int)timeSpan.TotalDays).ToString() + " days ago";
            }
            else if (timeSpan.TotalHours > 1)
            {
                text = ((int)timeSpan.TotalHours).ToString() + " hours ago";
            }
            else if (timeSpan.TotalMinutes > 1)
            {
                text = ((int)timeSpan.TotalMinutes).ToString() + " mins ago";
            }
            else
            {
                text = ((int)timeSpan.TotalSeconds).ToString() + " seconds ago";
            }

            return text;
        }
    }

    static System.TimeSpan GetTimespan(double javaTimestamp)
    {
        System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimestamp).ToUniversalTime();
        return System.DateTime.UtcNow.Subtract(dtDateTime);
    }
}
