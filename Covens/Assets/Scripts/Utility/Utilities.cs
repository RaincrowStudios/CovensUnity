using UnityEngine;
using System.Collections;
using System;

public class Utilities : MonoBehaviour
{

	public static Color Orange = new Color(1,0.515625f,0,1);
	public static Color Red = Color.red;
	public static Color Grey = Color.grey;
	public static Color darkGrey = new Color(0.2156f,0.2156f,0.2156f,1);
	public static Color Green = Color.green;
	public static Color Blue = new Color(0,0.67588235f,1,1);
	public static Color Purple = new Color(0.6980f,0,1,1);
	public static float ChannelSpeed = .15f;
	public static int BaseBuff = 10;

	public static float DamageMultiplier = 1;
	public static float XPMultiplier = 1;
	public static float CostMultiplier = 1;
	public static float SuccessRateMultiplier = 1;
	public static int minSuccessRate = 5;
	public static float maxSuccessRate = 95;

	private float t= 0;

	static bool showLogs = true;

	public enum Spells
	{
		hex,bless,suneater,whiteflames,grace,ressurect,banish,bind,silence,waste
	};


	public static string witchTypeControl(int lp)
	{
		int i = Mathf.Abs (lp);
		string s = "";
		if (i == 1)
			s = "1ST DEGREE";
		if (i == 2)
			s = "2ND DEGREE";
		if (i == 3)
			s = "3RD DEGREE";
		if (i == 4)
			s = "4TH DEGREE";
		if (i == 5)
			s = "5TH DEGREE";
		if (i == 6) 
			s = "6TH DEGREE";
		if (i == 7)
			s = "7TH DEGREE";
		if (i == 8)
			s = "8TH DEGREE";
		if (i == 9)
			s = "9TH DEGREE";
		if (i == 10)
			s = "10TH DEGREE";
		if (i == 11)
			s = "11TH DEGREE";
		if (i == 12)
			s = "12TH DEGREE";
		if (i == 13)
			s = "13TH DEGREE";
		if (i == 14)
			s = "14TH DEGREE";
		if (lp < 0) {
			s += " SHADOW WITCH";
		} else if (lp > 0)
			s += " WHITE WITCH";
		else
			s = "GREY WITCH";
		
		return s;
	}


	public static string witchTypeControlSmallCaps(int lp)
	{
		int i = Mathf.Abs (lp);
		string s = "";
		if (i == 1)
			s = "1st Degree";
		if (i == 2)  
			s = "2nd Degree";
		if (i == 3)  
			s = "3rd Degree";
		if (i == 4)   
			s = "4th Degree";
		if (i == 5)   
			s = "5th Degree";
		if (i == 6)  
			s = "6th Degree";
		if (i == 7)  
			s = "7th Degree";
		if (i == 8)  
			s = "8th Degree";
		if (i == 9)   
			s = "9th Degree";
		if (i == 10)
			s = "10th Degree";
		if (i == 11)
			s = "11th Degree";
		if (i == 12)
			s = "12th Degree";
		if (i == 13)
			s = "13th Degree";
		if (i == 14)
			s = "14th Degree";
		if (lp < 0) {
			s += " Shadow Witch";
		} else if (lp > 0)
			s += " White Witch";
		else
			s = "Grey Witch";

		return s;
	}

	// Update is called once per frame
	void Update ()
	{
	
	}

	public static GameObject InstantiateObject (GameObject prefab, Transform parent, float scale = 1)
	{
		GameObject g = Instantiate (prefab, parent);
		g.transform.SetParent (parent);
		g.transform.localPosition = Vector3.zero; 
		g.transform.localEulerAngles = Vector3.zero; 
		g.transform.transform.localScale = new Vector3 (scale, scale, scale);
		return g;
	}

	public static void allowMapControl(bool allow, bool allowCameraControl = false)
	{
		OnlineMapsTileSetControl.instance.allowZoom = allow;
		OnlineMapsTileSetControl.instance.allowUserControl = allow;
		OnlineMapsTileSetControl.instance.allowCameraControl = allowCameraControl;
	}
}

