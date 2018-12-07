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
    }

    public void Setup(TeamLocation data)
    {

    }

    public void Setup(TeamInvites data)
    {

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
