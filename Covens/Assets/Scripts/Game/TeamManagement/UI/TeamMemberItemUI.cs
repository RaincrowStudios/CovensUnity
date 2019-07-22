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
    [SerializeField] private InputField m_TitleField;

    private TeamMemberData m_Data;
    private System.Action m_OnClick;

    public void Awake()
    {
        m_Button.onClick.AddListener(() => m_OnClick?.Invoke());
    }

    public void Setup(TeamMemberData data, bool showEdit, System.Action<TeamMemberData> onClick)
    {
        transform.localScale = Vector3.one;

        m_Data = data;
        m_OnClick = () => onClick?.Invoke(m_Data);

        //setup data
        m_Level.text = data.Level.ToString();
        m_PlayerName.text = data.Name;
        m_Title.text = data.Title;
        m_TitleField.text = data.Title;
        m_AdminIcon.SetActive(data.Role == CovenRole.ADMIN);
        m_ModIcon.SetActive(data.Role == CovenRole.MODERATOR);
        m_LastActive.text = GetlastActive(data.LastActiveOn);
        m_State.text = data.State == "" ? "Normal" : data.State;

        //edit options
        m_TitleField.gameObject.SetActive(showEdit && TeamManager.MyRole > data.Role);
        m_KickButton.gameObject.SetActive(showEdit && TeamManager.MyRole > data.Role);
        m_PromoteButton.gameObject.SetActive(showEdit && TeamManager.MyRole > data.Role);
        m_DemoteButton.gameObject.SetActive(showEdit && TeamManager.MyRole > data.Role);

        m_PromoteButton.interactable = data.Role < CovenRole.ADMIN;
        m_DemoteButton.interactable = data.Role > CovenRole.MEMBER;

        m_Background.SetActive(transform.GetSiblingIndex() % 2 == 0);
    }

    private void OnClick()
    {
        m_OnClick?.Invoke();
        //TeamPlayerView.ViewCharacter(
        //    m_Data.displayName, 
        //    (character, result) =>
        //    {
        //        if (character != null)
        //            TeamPlayerView.Instance.Setup(character);
        //    });
    }

    private void OnClickKick()
    {
        Debug.LogError("TODO: KICK");
    }

    private void OnClickDemote()
    {
        Debug.LogError("TODO: DEMOTE");
    }

    private void OnClickPromote()
    {
        Debug.LogError("TODO: PROMOTE");
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
}
