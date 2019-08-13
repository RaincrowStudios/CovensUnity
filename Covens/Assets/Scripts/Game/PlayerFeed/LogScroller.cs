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
    public List<EventLog> log = new List<EventLog>();
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
        EventLog sd = log[dataIndex];
        EnhancedScrollerCellView item = scroller.GetCellView(text) as EnhancedScrollerCellView;
        SetupText(item, sd);
        return item;
    }

    void SetupText(EnhancedScrollerCellView text, EventLog data)
    {
        // Debug.Log(data.type + " logging!");
        var t = text.GetComponent<TextMeshProUGUI>();
        if (data.type == "dailyBlessing")
        {
			t.text = LocalizeLookUp.GetText("log_blessing_savannah")
                .Replace("{{Energy}}", "<color=#FF9900FF>" + data.data.energyChange.ToString())
                .Replace("{{Color}}", "</color>").Replace("{{Timestamp}}", GetTimeStamp(data.createdOn));//  "Savannah Grey granted you her <b>daily blessing</b>. Long live the Deal! <color=#FF9900FF>+" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";

            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.createdOn);
        }
        else if (data.type == "lunarBlessing")
        {
			t.text = LocalizeLookUp.GetText ("log_blessing_moon")
                .Replace ("{{Energy}}", "<color=#FF9900FF>+" + data.data.energyChange.ToString ())
                .Replace ("{{Color}}", "</color>")
                .Replace ("{{Timestamp}}", GetTimeStamp (data.createdOn));// "The <b>Moon</b> is in your <b>favor</b>.<color=#FF9900FF>+" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";

            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.createdOn);
        }
        else if (data.type == "summonedSpirit")
        {
			t.text = LocalizeLookUp.GetText ("log_spirit_born")
                .Replace ("{{Spirit Name}}",LocalizeLookUp.GetSpiritName(data.data.spirit))
                .Replace ("{{Timestamp}}", GetTimeStamp (data.createdOn)); // "Your <b>" + DownloadedAssets.spiritDictData[data.spirit].spiritName + "</b> has entered the world. <size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";

            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.createdOn);
        }
        else if (data.type == "flipSpirit")
        {
            try
            {
				t.text = LocalizeLookUp.GetText("log_spirit_turn")
                    .Replace("{{Spirit Name}}", LocalizeLookUp.GetSpiritName(data.data.spirit))
                    .Replace("{{Energy}}", "<color=red>" + data.data.energyChange.ToString())
                    .Replace("{{Color}}", "</color>")
                    .Replace("{{Timestamp}}", GetTimeStamp(data.createdOn));    // "Your <b>" + DownloadedAssets.spiritDictData[data.spirit].spiritName + "</b> has turned against you!.<color=red>" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";

            }
            catch (System.Exception)
            {
				t.text = LocalizeLookUp.GetText("log_spirit_turn")
                    .Replace("{{Spirit Name}}", LocalizeLookUp.GetText("attacked_spirit"))
                    .Replace("{{Energy}}", "<color=red>" + data.data.energyChange.ToString())
                    .Replace("{{Color}}", "</color>").Replace("{{Timestamp}}", GetTimeStamp(data.createdOn));//"Your <b>Spirit</b> has turned against you!.<color=red>" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";

            }
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.createdOn);
        }
        else if (data.type == "castSpell")
        {
            Debug.Log(data.data.spirit + "|");
            if (data.data.spirit == "" || data.data.spirit == null)
            {
                string school = "";
				if (data.data.casterDegree < 0)
					school = " " + LocalizeLookUp.GetText ("log_witch_shadow");// " Shadow Witch";
                else if (data.data.casterDegree > 0)
					school = " " + LocalizeLookUp.GetText ("log_witch_white");//" White Witch";
                else
					school = " " + LocalizeLookUp.GetText ("log_witch_grey");//"Grey Witch";

                if (data.data.energyChange > 0)
                {
                   t.text = LocalizeLookUp.GetText ("log_cast_energy")
                        .Replace ("{{Witch School}}", school)
                        .Replace ("{{Witch Name}}", data.data.casterName)
                        .Replace ("{{Spell Name}}", LocalizeLookUp.GetSpellName(data.data.spellId))
                        .Replace ("{{Energy}}", "<color=#FF9900FF>+" + data.data.energyChange.ToString ())
                        .Replace("{{Color}}", "</color>")
                        .Replace ("{{Timestamp}}", GetTimeStamp (data.createdOn));
				}
                else if (data.data.energyChange < 0)
                {
                    t.text = LocalizeLookUp.GetText ("log_cast_energy")
                        .Replace ("{{Witch School}}", school)
                        .Replace ("{{Witch Name}}", data.data.casterName)
                        .Replace ("{{Spell Name}}", LocalizeLookUp.GetSpellName(data.data.spellId))
                        .Replace ("{{Energy}}", "<color=red>" + data.data.energyChange.ToString ())
                        .Replace ("{{Color}}", "</color>")
                        .Replace ("{{Timestamp}}", GetTimeStamp (data.createdOn));
				}
                else
                {
					t.text = LocalizeLookUp.GetText ("log_cast_msg")
                        .Replace ("{{Witch School}}", school)
                        .Replace ("{{Witch Name}}", data.data.casterName)
                        .Replace ("{{Spell Name}}", LocalizeLookUp.GetSpellName(data.data.spellId))
                        .Replace ("{{Timestamp}}", GetTimeStamp (data.createdOn));
                }
            }
            else //I used witch localization because there was no localized version for spirits
            {
                if (data.data.energyChange > 0)
                {
					t.text = LocalizeLookUp.GetText ("log_cast_energy")
                        .Replace (" {{Witch School}}", "")
                        .Replace ("{{Witch Name}}", LocalizeLookUp.GetSpiritName(data.data.spirit))
                        .Replace ("{{Spell Name}}", LocalizeLookUp.GetSpellName(data.data.spellId))
                        .Replace ("{{Energy}}", "<color=#FF9900FF>+" + data.data.energyChange.ToString ())
                        .Replace("{{Color}}", "</color>")
                        .Replace ("{{Timestamp}}", GetTimeStamp (data.createdOn));
                }
                else if (data.data.energyChange < 0)
                {
					t.text = LocalizeLookUp.GetText ("log_cast_energy")
                        .Replace (" {{Witch School}}", "")
                        .Replace ("{{Witch Name}}", LocalizeLookUp.GetSpiritName(data.data.spirit))
                        .Replace ("{{Spell Name}}", LocalizeLookUp.GetSpellName(data.data.spellId))
                        .Replace ("{{Energy}}", "<color=red>" + data.data.energyChange.ToString ())
                        .Replace ("{{Color}}", "</color>")
                        .Replace ("{{Timestamp}}", GetTimeStamp (data.createdOn));
                }
                else
                {
					t.text = LocalizeLookUp.GetText ("log_cast_msg")
                        .Replace (" {{Witch School}}", "")
                        .Replace ("{{Witch Name}}", LocalizeLookUp.GetSpiritName(data.data.spirit))
                        .Replace ("{{Spell Name}}", LocalizeLookUp.GetSpellName(data.data.spellId))
                        .Replace ("{{Timestamp}}", GetTimeStamp (data.createdOn));
                }
            }
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.createdOn);
        }
        else if (data.type == "sentinel")
        {
			t.text = LocalizeLookUp.GetText ("log_sentinel")
                .Replace ("{{Spirit Name}}", "<color=red>" + LocalizeLookUp.GetSpiritName(data.data.spirit) + "</color>")
                .Replace ("{{Timestamp}}", GetTimeStamp (data.createdOn));

            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.createdOn);
        }
        else if (data.type == "characterCreated")
        {
			t.text = LocalizeLookUp.GetText("daily_witch_created")
                .Replace("{{witchCreated}}", data.data.casterName);// "You created your witch character - <b><color=white>" + data.data.witchCreated;
            t.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetDayStamp(data.createdOn);
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


