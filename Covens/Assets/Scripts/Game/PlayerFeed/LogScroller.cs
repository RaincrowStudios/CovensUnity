using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using UnityEngine.UI;
using TMPro;

public class LogScroller : MonoBehaviour, IEnhancedScrollerDelegate
{

    public EnhancedScroller scroller;
    public List<EventLogData> log = new List<EventLogData>();
    public EnhancedScrollerCellView text;

    void Start()
    {
        scroller.Delegate = this;
    }

    public void InitScroll()
    {
        log.Reverse();
        scroller.ReloadData();
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return log.Count;
    }
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 100;
    }
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        EventLogData sd = log[dataIndex];
        EnhancedScrollerCellView item = scroller.GetCellView(text) as EnhancedScrollerCellView;
        SetupText(item, sd);
        return item;
    }

    void SetupText(EnhancedScrollerCellView text, EventLogData data)
    {
        Debug.Log(data.type + " logging!");
        var t = text.GetComponent<TextMeshProUGUI>();
        if (data.type == "dailyBlessing")
        {
            t.text = "Savannah Grey granted you her <b>daily blessing</b>. Long live the Deal! <color=#FF9900FF>+" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.timestamp);
        }
        else if (data.type == "lunarBlessing")
        {
            t.text = "The <b>Moon</b> is in your <b>favor</b>.<color=#FF9900FF>+" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.timestamp);
        }
        else if (data.type == "uponSpiritBorn")
        {
            t.text = "Your <b>" + DownloadedAssets.spiritDictData[data.spirit].spiritName + "</b> has entered the world. <size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.timestamp);
        }
        else if (data.type == "ifSpiritFlips")
        {
            t.text = "Your <b>" + DownloadedAssets.spiritDictData[data.spirit].spiritName + "</b> has turned against you!.<color=red>-" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.timestamp);
        }
        else if (data.type == "spellCast")
        {
            Debug.Log("SPELLCAST!");
            string school = "";
            if (data.casterDegree < 0)
                school = " Shadow Witch";
            else if (data.casterDegree > 0)
                school = " White Witch";
            else
                school = "Grey Witch";

            if (data.energyChange > 0)
            {
                t.text = "The " + school + " <b>" + data.casterName + "</b> cast <b>" + DownloadedAssets.spellDictData[data.spellId].spellName + " </b>on you. <color=#FF9900FF>+" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";
            }
            else if (data.energyChange < 0)
            {
                t.text = "The " + school + " <b>" + data.casterName + "</b> cast <b>" + DownloadedAssets.spellDictData[data.spellId].spellName + " </b>on you. <color=red>" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";
            }
            else
            {
                t.text = "The " + school + " <b>" + data.casterName + "</b> cast <b>" + DownloadedAssets.spellDictData[data.spellId].spellName + " </b>on you. <size=35>[" + GetTimeStamp(data.timestamp) + "]</size>";
            }

            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.timestamp);
        }
        else if (data.type == "sentinel")
        {
            t.text = "Your <b> Sentinel Owl </b> has spotted <color=red>" + DownloadedAssets.spiritDictData[data.spiritId].spiritName + "</color>.<size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.timestamp);
        }
        else if (data.type == "witchCreated")
        {
            t.text = "You created your witch character - <b><color=white>" + data.witchCreated;
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.timestamp);
        }
    }

    string GetTimeStamp(double javaTimeStamp)
    {
        System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();

        return dtDateTime.ToString("t");
    }

    string GetDayStamp(double javaTimeStamp)
    {

        System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();

        return dtDateTime.ToString("M");
    }
}


