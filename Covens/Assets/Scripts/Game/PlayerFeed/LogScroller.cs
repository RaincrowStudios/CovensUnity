using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using UnityEngine.UI;

public class LogScroller : MonoBehaviour, IEnhancedScrollerDelegate
{

	public EnhancedScroller scroller;
	public List<EventLogData> log = new List<EventLogData> ();
	public EnhancedScrollerCellView text;

	void Start()
	{
		scroller.Delegate = this;
	}

	public void InitScroll()
	{
		scroller.ReloadData ();
	}

	public int GetNumberOfCells (EnhancedScroller scroller)
	{
		return log.Count;
	}
	public float GetCellViewSize (EnhancedScroller scroller, int dataIndex)
	{
		return 100;
	}
	public EnhancedScrollerCellView GetCellView (EnhancedScroller scroller, int dataIndex, int cellIndex)
	{
		EventLogData sd = log [dataIndex];
		EnhancedScrollerCellView item = scroller.GetCellView (text) as EnhancedScrollerCellView;  
		SetupText (item,sd);
		return item;
	}

	void SetupText (EnhancedScrollerCellView text, EventLogData data){
		var t = text.GetComponent<Text> ();
		if (data.type == "dailyBlessing") {
			t.text = "Savannah Grey granted you her <b>daily blessing</b>. Long live the Deal! <color=#FF9900FF>+" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";
			t.transform.GetChild (0).GetComponent<Text> ().text = GetDayStamp (data.timestamp);
		} else if (data.type == "lunarBlessing") {
			t.text = "The <b>Moon</b> is in your <b>favor</b>.<color=#FF9900FF>+" + data.energyChange.ToString() + " energy </color><size=35> [" + GetTimeStamp(data.timestamp) + "]</size>";
			t.transform.GetChild (0).GetComponent<Text> ().text = GetDayStamp (data.timestamp);
		}
	}

	string GetTimeStamp(double javaTimeStamp)
	{
		if (javaTimeStamp < 159348924)
		{
			string s = "unknown";
			return s;
		}
		System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local); 
		dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime(); 

		return dtDateTime.ToString("t");
	}

	string GetDayStamp(double javaTimeStamp)
	{
		if (javaTimeStamp < 159348924)
		{
			string s = "unknown";
			return s;
		}
		System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local); 
		dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime(); 

		return dtDateTime.ToString("M");
	}
}


