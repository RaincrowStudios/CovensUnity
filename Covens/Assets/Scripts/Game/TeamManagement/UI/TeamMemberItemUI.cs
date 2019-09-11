using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow.Team;

public class TeamMemberItemUI : MonoBehaviour
{
    [SerializeField] private Button m_Button;

    [Header("Base")]
    [SerializeField] private GameObject m_Background;
    [SerializeField] private GameObject m_DisableOverlay;
    [SerializeField] private Text m_Level;
    [SerializeField] private Text m_PlayerName;
    [SerializeField] private GameObject m_AdminIcon;
    [SerializeField] private GameObject m_ModIcon;
    [SerializeField] private Text m_Title;
    [SerializeField] private Text m_State;
    [SerializeField] private Text m_LastActive;

    [Header("Edit")]
    [SerializeField] private Button m_KickButton;
    [SerializeField] private Button m_DemoteButton;
    [SerializeField] private Button m_PromoteButton;
    [SerializeField] private Button m_TitleButton;
    [SerializeField] private InputField m_TitleField;

    public TeamData Coven { get; private set; }
    public TeamMemberData MemberData { get; private set; }

    private System.Action<TeamMemberItemUI> m_OnSelect;
    private System.Action<string> m_OnKick;
    private System.Action<string, CovenRole> m_OnPromote;
    private System.Action<string, CovenRole> m_OnDemote;
    private System.Action<TeamMemberItemUI> m_OnClickTitle;

    public void Awake()
    {
        m_Button.onClick.AddListener(OnClick);
        m_DemoteButton.onClick.AddListener(OnClickDemote);
        m_PromoteButton.onClick.AddListener(OnClickPromote);
        m_KickButton.onClick.AddListener(OnClickKick);
        m_TitleButton.onClick.AddListener(OnClickTitle);
        //m_TitleField.onEndEdit.AddListener(OnFinishEditingTitle);

        m_TitleField.gameObject.SetActive(false);
    }

    public void Setup(
        TeamData coven,
        TeamMemberData member,
        System.Action<TeamMemberItemUI> onSelect,
        System.Action<string> onKick,
        System.Action<string, CovenRole> onPromote,
        System.Action<string, CovenRole> onDemote,
        System.Action<TeamMemberItemUI> onClickTitle)
    {
        //Disable(false);
        transform.localScale = Vector3.one;

        Coven = coven;
        MemberData = member;
        m_OnSelect = onSelect;
        m_OnKick = onKick;
        m_OnPromote = onPromote;
        m_OnDemote = onDemote;
        m_OnClickTitle = onClickTitle;

        //setup data
        Refresh();
    }

    public void Refresh()
    {
        m_Level.text = MemberData.Level.ToString();
        m_PlayerName.text = MemberData.Name;
        if (string.IsNullOrEmpty(MemberData.Title))
        {
            if (MemberData.Role == CovenRole.ADMIN)
                m_Title.text = LocalizeLookUp.GetText("team_member_admin_role");
            else if (MemberData.Role == CovenRole.MODERATOR)
                m_Title.text = LocalizeLookUp.GetText("team_member_moderator_role");
            else
                m_Title.text = LocalizeLookUp.GetText("team_member_member_role");
        }
        else
        {
            m_Title.text = MemberData.Title;
        }
        //m_TitleField.text = MemberData.Title;
        m_AdminIcon.SetActive(MemberData.Role == CovenRole.ADMIN);
        m_ModIcon.SetActive(MemberData.Role == CovenRole.MODERATOR);
        m_LastActive.text = GetlastActive(MemberData.LastActiveOn);
        m_State.text = MemberData.State == "" ? "Normal" : MemberData.State;

        bool isFounder = TeamManager.MyCovenData != null && TeamManager.MyCovenData.CreatedBy == PlayerDataManager.playerData.instance;
        bool isMe = MemberData.Id == PlayerDataManager.playerData.instance;

        //edit options
        //m_TitleField.gameObject.SetActive(isFounder || (Coven.IsMember && TeamManager.MyRole > MemberData.Role));
        m_KickButton.gameObject.SetActive(isFounder || (Coven.IsMember && TeamManager.MyRole > MemberData.Role));
        m_PromoteButton.gameObject.SetActive(isFounder || (Coven.IsMember && TeamManager.MyRole > MemberData.Role));
        m_DemoteButton.gameObject.SetActive(isFounder || (Coven.IsMember && TeamManager.MyRole > MemberData.Role));

        m_PromoteButton.interactable = isMe == false && MemberData.Role < CovenRole.ADMIN;
        m_DemoteButton.interactable = isMe == false && MemberData.Role > CovenRole.MEMBER;
        m_KickButton.interactable = isMe == false;

        m_Background.SetActive(transform.GetSiblingIndex() % 2 == 0);
    }

    private void OnClick()
    {
        m_OnSelect?.Invoke(this);
    }

    private void OnClickKick()
    {
        UIGlobalPopup.ShowPopUp(
            confirmAction: () =>
            {
                LoadingOverlay.Show();

                TeamManager.KickMember(MemberData, (error) =>
                {
                    LoadingOverlay.Hide();
                    if (string.IsNullOrEmpty(error))
                        m_OnKick?.Invoke(MemberData.Id);
                    else
                        UIGlobalPopup.ShowError(null, error);
                });
            },
            cancelAction: () => {},
            LocalizeLookUp.GetText("coven_member_remove").Replace("{{name}}", MemberData.Name));
    }

    private void OnClickDemote()
    {
        CovenRole role = MemberData.Role - 1;
        string roleName;

        if (role == CovenRole.MODERATOR)
            roleName = LocalizeLookUp.GetText("team_member_moderator_role");
        else
            roleName = LocalizeLookUp.GetText("team_member_member_role");

        UIGlobalPopup.ShowPopUp(
            confirmAction: () =>
            {
                LoadingOverlay.Show();

                TeamManager.DemoteMember(MemberData, (error) =>
                {
                    LoadingOverlay.Hide();
                    if (string.IsNullOrEmpty(error))
                        m_OnDemote?.Invoke(MemberData.Id, role);
                    else
                        UIGlobalPopup.ShowError(null, error);
                });
            },
            cancelAction: () => {},
            LocalizeLookUp.GetText("coven_member_demote").Replace("{{name}}", MemberData.Name).Replace("{{role}}", roleName));
    }

    private void OnClickPromote()
    {
        CovenRole role = MemberData.Role + 1;
        string roleName;

        if (role == CovenRole.MODERATOR)
            roleName = LocalizeLookUp.GetText("team_member_moderator_role");
        else
            roleName = LocalizeLookUp.GetText("team_member_admin_role");

        UIGlobalPopup.ShowPopUp(
            confirmAction: () =>
            {
                LoadingOverlay.Show();

                TeamManager.PromoteMember(MemberData, (error) =>
                {
                    LoadingOverlay.Hide();
                    if (string.IsNullOrEmpty(error))
                        m_OnPromote?.Invoke(MemberData.Id, role);
                    else
                        UIGlobalPopup.ShowError(null, error);
                });
            },
            cancelAction: () => {},
            LocalizeLookUp.GetText("coven_member_promote").Replace("{{name}}", MemberData.Name).Replace("{{role}}", roleName));
    }

    private void OnClickTitle()
    {
        m_OnClickTitle?.Invoke(this);
    }

    private static string GetlastActive(double javaTimeStamp)
    {
        if (javaTimeStamp < 159348924)
        {
            return LocalizeLookUp.GetText("spirit_deck_unknown");//"unknown";
        }

        System.TimeSpan timeSpan = GetTimespan(javaTimeStamp);
        string stamp = "";

        if (timeSpan.TotalDays > 1)
        {
            stamp = ((int)timeSpan.TotalDays).ToString() + " " + LocalizeLookUp.GetText("active_days");// " days ago";
        }
        else if (timeSpan.TotalHours > 1)
        {
            stamp = ((int)timeSpan.TotalHours).ToString() + " " + LocalizeLookUp.GetText("active_hours");//hours ago";

        }
        else if (timeSpan.TotalMinutes > 5)
        {
            stamp = ((int)timeSpan.TotalMinutes).ToString() + " " + LocalizeLookUp.GetText("active_mins");//mins ago";
        }
        else
        {
            stamp = LocalizeLookUp.GetText("active_word");//"Active";
        }

        return stamp;
    }

    private static System.TimeSpan GetTimespan(double javaTimestamp)
    {
        System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimestamp).ToUniversalTime();
        return System.DateTime.UtcNow.Subtract(dtDateTime);
    }

    //public void Disable(bool disable)
    //{
    //    m_DisableOverlay.SetActive(disable);
    //}
}
