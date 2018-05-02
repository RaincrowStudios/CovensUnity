using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
public class AgeGate : MonoBehaviour {

	public InputField Date;
	public InputField Month;
	public InputField Year;
	public string Timer;
	public int d,m,y;
	public GameObject TOS,PP;
	public GameObject Holder;
	public GameObject UnderAge;
//	public PlayerLoginManager PLM;
	public GameObject AgeGateObject;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnChangeDate()
	{
		int i = int.Parse (Date.text);
		i = Mathf.Clamp (i, 1, 31);
		d =i;
		Date.text = i.ToString ();
	}

	public void OnChangeMonth()
	{
		int i = int.Parse (Month.text);
		i = Mathf.Clamp (i, 1, 12);
		m =i;
		Month.text = i.ToString ();
	}

	public void OnChangeYear()
	{
		int i = int.Parse (Year.text);
		if (Year.text.Length == 4) {
			i = Mathf.Clamp (i, 1900, System.DateTime.Now.Year);
		}
		y =i;
		Year.text = i.ToString ();
	}

	public void ContinueToReg()
	{
		string day, month ;
		if (d < 10)
			day = "0" + d.ToString ();
		else
			day = d.ToString ();

		if (m < 10)
			month = "0" + m.ToString ();
		else
			month = m.ToString ();

		Timer = day + ":" + month + ":" + y.ToString ().ToString() + ":00:00:00";
		DateTime EndTime = DateTime.ParseExact(Timer, "dd:MM:yyyy:HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture); 
		DateTime currentTime = DateTime.Now;
		TimeSpan timeLeft = currentTime - EndTime;
		print (timeLeft.Days);
		if (timeLeft.Days > 4745) {
			print ("Older than 13");
//			LoginManagerUI.Instance.Register ();
			AgeGateObject.SetActive (false);
		} else {
			Holder.SetActive (false);
			UnderAge.SetActive (true);
			TOS.SetActive (false);
			PP.SetActive (false);
		}

	}
}
