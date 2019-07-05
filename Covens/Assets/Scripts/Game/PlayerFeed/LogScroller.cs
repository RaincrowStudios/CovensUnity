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
        // Debug.Log(data.type + " logging!");
        var t = text.GetComponent<TextMeshProUGUI>();
        if (data.type == "dailyBlessing")
        {
			t.text = LocalizeLookUp.GetText("log_blessing_savannah").Replace("{{Energy}}", "<color=#FF9900FF>" + data.energyChange.ToString()).Replace("{{Color}}", "</color>").Replace("{{Timestamp}}", GetTimeStamp(data.timestamp));//  "Savannah Grey granted you her <b>daily blessing</b>. Long live the Deal! <color=#FF9900FF>+" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.timestamp);
        }
        else if (data.type == "lunarBlessing")
        {
			t.text = LocalizeLookUp.GetText ("log_blessing_moon").Replace ("{{Energy}}", "<color=#FF9900FF>+" + data.energyChange.ToString ()).Replace ("{{Color}}", "</color>").Replace ("{{Timestamp}}", GetTimeStamp (data.timestamp));// "The <b>Moon</b> is in your <b>favor</b>.<color=#FF9900FF>+" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.timestamp);
        }
        else if (data.type == "uponSpiritBorn")
        {
			t.text = LocalizeLookUp.GetText ("log_spirit_born").Replace ("{{Spirit Name}}",LocalizeLookUp.GetSpiritName(data.spirit)).Replace ("{{Timestamp}}", GetTimeStamp (data.timestamp)); // "Your <b>" + DownloadedAssets.spiritDictData[data.spirit].spiritName + "</b> has entered the world. <size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.timestamp);
        }
        else if (data.type == "ifSpiritFlips")
        {
            try
            {
				t.text = LocalizeLookUp.GetText("log_spirit_turn").Replace("{{Spirit Name}}", LocalizeLookUp.GetSpiritName(data.spirit)).Replace("{{Energy}}", "<color=red>" + data.energyChange.ToString()).Replace("{{Color}}", "</color>").Replace("{{Timestamp}}", GetTimeStamp(data.timestamp));    // "Your <b>" + DownloadedAssets.spiritDictData[data.spirit].spiritName + "</b> has turned against you!.<color=red>" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";

            }
            catch (System.Exception)
            {
				t.text = LocalizeLookUp.GetText("log_spirit_turn").Replace("{{Spirit Name}}", LocalizeLookUp.GetText("attacked_spirit")).Replace("{{Energy}}", "<color=red>" + data.energyChange.ToString()).Replace("{{Color}}", "</color>").Replace("{{Timestamp}}", GetTimeStamp(data.timestamp));//"Your <b>Spirit</b> has turned against you!.<color=red>" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";

            }
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.timestamp);
        }
        else if (data.type == "spellCast")
        {
            Debug.Log(data.spirit + "|");
            if (data.spirit == "" || data.spirit == null)
            {
                string school = "";
				if (data.casterDegree < 0)
					school = " " + LocalizeLookUp.GetText ("log_witch_shadow");// " Shadow Witch";
                else if (data.casterDegree > 0)
					school = " " + LocalizeLookUp.GetText ("log_witch_white");//" White Witch";
                else
					school = " " + LocalizeLookUp.GetText ("log_witch_grey");//"Grey Witch";

                if (data.energyChange > 0)
                {
                   // t.text = "The " + school + " <b>" + data.casterName + "</b> cast <b>" + DownloadedAssets.spellDictData[data.spellId].spellName + " </b>on you. <color=#FF9900FF>+" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";
					t.text = LocalizeLookUp.GetText ("log_cast_energy").Replace ("{{Witch School}}", school).Replace ("{{Witch Name}}", data.casterName).Replace ("{{Spell Name}}", DownloadedAssets.spellDictData [data.spellId].spellName).Replace ("{{Energy}}", "<color=#FF9900FF>+" + data.energyChange.ToString ()).Replace("{{Color}}", "</color>").Replace ("{{Timestamp}}", GetTimeStamp (data.timestamp));
				}
                else if (data.energyChange < 0)
                {
                   // t.text = "The " + school + " <b>" + data.casterName + "</b> cast <b>" + DownloadedAssets.spellDictData[data.spellId].spellName + " </b>on you. <color=red>" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";
					t.text = LocalizeLookUp.GetText ("log_cast_energy").Replace ("{{Witch School}}", school).Replace ("{{Witch Name}}", data.casterName).Replace ("{{Spell Name}}", DownloadedAssets.spellDictData [data.spellId].spellName).Replace ("{{Energy}}", "<color=red>" + data.energyChange.ToString ()).Replace ("{{Color}}", "</color>").Replace ("{{Timestamp}}", GetTimeStamp (data.timestamp));
				}
                else
                {
					t.text = LocalizeLookUp.GetText ("log_cast_msg").Replace ("{{Witch School}}", school).Replace ("{{Witch Name}}", data.casterName).Replace ("{{Spell Name}}", DownloadedAssets.spellDictData [data.spellId].spellName).Replace ("{{Timestamp}}", GetTimeStamp (data.timestamp));
                   // t.text = "The " + school + " <b>" + data.casterName + "</b> cast <b>" + DownloadedAssets.spellDictData[data.spellId].spellName + " </b>on you. <size=35>[" + GetTimeStamp(data.timestamp) + "]</size>";
                }
            }
            else //I used witch localization because there was no localized version for spirits
            {
                if (data.energyChange > 0)
                {
					t.text = LocalizeLookUp.GetText ("log_cast_energy").Replace (" {{Witch School}}", "").Replace ("{{Witch Name}}", LocalizeLookUp.GetSpiritName(data.spirit)).Replace ("{{Spell Name}}", DownloadedAssets.spellDictData[data.spellId].spellName).Replace ("{{Energy}}", "<color=#FF9900FF>+" + data.energyChange.ToString ()).Replace("{{Color}}", "</color>").Replace ("{{Timestamp}}", GetTimeStamp (data.timestamp));
                    //t.text = $"The <b>{DownloadedAssets.spiritDictData[data.spirit].spiritName}</b> cast <b>{DownloadedAssets.spellDictData[data.spellId].spellName} </b>on you. <color=#FF9900FF>{data.energyChange.ToString()} energy </color><size=35> [{GetTimeStamp(data.timestamp)}]</size>";
                }
                else if (data.energyChange < 0)
                {
					t.text = LocalizeLookUp.GetText ("log_cast_energy").Replace (" {{Witch School}}", "").Replace ("{{Witch Name}}", LocalizeLookUp.GetSpiritName(data.spirit)).Replace ("{{Spell Name}}", DownloadedAssets.spellDictData [data.spellId].spellName).Replace ("{{Energy}}", "<color=red>" + data.energyChange.ToString ()).Replace ("{{Color}}", "</color>").Replace ("{{Timestamp}}", GetTimeStamp (data.timestamp));
                    //t.text = $"The <b>{DownloadedAssets.spiritDictData[data.spirit].spiritName}</b> cast <b>{DownloadedAssets.spellDictData[data.spellId].spellName} </b>on you. <color=red>{data.energyChange.ToString()} energy </color><size=35> [{GetTimeStamp(data.timestamp)}]</size>";
                }
                else
                {
					t.text = LocalizeLookUp.GetText ("log_cast_msg").Replace (" {{Witch School}}", "").Replace ("{{Witch Name}}", LocalizeLookUp.GetSpiritName(data.spirit)).Replace ("{{Spell Name}}", DownloadedAssets.spellDictData[data.spellId].spellName).Replace ("{{Timestamp}}", GetTimeStamp (data.timestamp));
                   // t.text = $"The <b>{DownloadedAssets.spiritDictData[data.spirit].spiritName}</b> cast <b>{DownloadedAssets.spellDictData[data.spellId].spellName} </b>on you. <size=35> [{GetTimeStamp(data.timestamp)}]</size>";
                }
            }
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.timestamp);
        }
        else if (data.type == "sentinel")
        {
			t.text = LocalizeLookUp.GetText ("log_sentinel").Replace ("{{Spirit Name}}", "<color=red>" + LocalizeLookUp.GetSpiritName(data.spirit) + "</color>").Replace ("{{Timestamp}}", GetTimeStamp (data.timestamp));
           // t.text = "Your <b> Sentinel Owl </b> has spotted <color=red>" + DownloadedAssets.spiritDictData[data.spiritId].spiritName + "</color>.<size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.timestamp);
        }
        else if (data.type == "witchCreated")
        {
			//PAUSE
			t.text = LocalizeLookUp.GetText("daily_witch_created").Replace("{{witchCreated}}", data.witchCreated);// "You created your witch character - <b><color=white>" + data.witchCreated;
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


