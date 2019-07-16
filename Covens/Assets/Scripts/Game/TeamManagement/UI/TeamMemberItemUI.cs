using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamMemberItemUI : MonoBehaviour
{
    [SerializeField] private Button m_Button;

    [Header("Base")]
    [SerializeField] private TextMeshProUGUI m_Level;
    [SerializeField] private TextMeshProUGUI m_Name;
    [SerializeField] private GameObject m_AdminIcon;
    [SerializeField] private GameObject m_ModIcon;
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_State;
    [SerializeField] private TextMeshProUGUI m_LastActive;

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

    public void Setup(TeamMemberData data, System.Action<TeamMemberData> onClick)
    {
        m_Data = data;
        m_OnClick = () => onClick?.Invoke(m_Data);

        m_Level.text = data.level.ToString();
        m_Name.text = data.displayName;
        m_Title.text = data.title;
        m_TitleField.text = data.title;
        m_AdminIcon.SetActive(data.role == 2);
        m_ModIcon.SetActive(data.role == 1);
        m_LastActive.text = GetlastActive(data.lastActiveOn);
        m_State.text = data.state == "" ? "Normal" : data.state;

        //EnableEdit(data.role);
    }

    public void EnableEdit(bool enable)
    {

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

    }

    private void OnClickDemote()
    {

    }

    private void OnClickPromote()
    {

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
