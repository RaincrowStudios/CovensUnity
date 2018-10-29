using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MoonManager : UIAnimationManager {
	public GameObject container;
	public static MoonManager Instance { get; set;}
	int moonAge;
	public Sprite[] moonSprites;
	public Image middle;
	public Image r1;
	public Image r2;
	public Image l1;
	public Image l2;

	public Text currentMoonPhase;
	public Text moonDesc;
	public Text energyBonus;
	public Text playerRelation;
	public Text timer;
	public GameObject moonState;
	MoonData data;
	public Animator anim;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		moonAge = MoonAge (DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year);
		moonAge = Mathf.Clamp (moonAge, 0, 28);
	
	}

	public void DelayOpen()
	{
		Invoke ("Open", 4f);
	}

	public void Open()
	{
		SoundManagerOneShot.Instance.MenuSound ();
		data = PlayerDataManager.moonData;
		container.SetActive (true);
		anim.Play ("in");
		SetupMoon ();
		StartCoroutine (CountDown ());
	}

	IEnumerator CountDown()
	{
		while (true) {
				string t = Utilities.GetTimeRemaining (data.moonRise);
				if (t == "null") {
					timer.text = "Moon has risen";
					moonState.SetActive (false);
					yield break;
				} else {
					moonState.SetActive (true);
					timer.text = t;
				}
			yield return new WaitForSeconds (1);
		}
	}

	public void Close()
	{
		SoundManagerOneShot.Instance.MenuSound ();
		anim.Play ("out");
		StopCoroutine ("CountDown");
		Disable (container, 1);
	}

	public void SetupMoon( )
	{
		data.phase = Math.Round (data.phase, 2);
		r1.sprite = returnMoonSprite(moonAge+1); 
		r2.sprite =  returnMoonSprite(moonAge+2);
		middle.sprite =returnMoonSprite (moonAge);
		l1.sprite = returnMoonSprite(moonAge-1);
		l2.sprite =  returnMoonSprite(moonAge-2);

		if (data.phase == 0)
			currentMoonPhase.text = "New Moon";
		else if (data.phase > 0 && data.phase < .25)
			currentMoonPhase.text = "Waxing Crescent";
		else if (data.phase == .25)
			currentMoonPhase.text = "First Quarter";
		else if (data.phase >.25 && data.phase < .5)
			currentMoonPhase.text = "Waxing Gibbous";
		else if (data.phase == .5)
			currentMoonPhase.text = "Full Moon";
		else if (data.phase >.5 && data.phase < .75)
			currentMoonPhase.text = "Waning Gibbous";
		else if (data.phase == .75)
			currentMoonPhase.text = "Last Quarter";
		else
			currentMoonPhase.text = "Waning Crescent";

		playerRelation.text = "As a <color=white>" + Utilities.witchTypeControlSmallCaps (PlayerDataManager.playerData.degree) + "</color>, you are <color=white>" + SetPlayerRelationToMoon ().ToString () + "%</color> aligned with today's moon.";

		moonDesc.text = "The <color=#ffffff>" + moonAge.ToString() + "</color> days young moon is <color=#ffffff>" + ((int)(data.luminosity*100)).ToString() + "% </color> + illuminated.";

	}

	public void SetupSavannaEnergy(bool show, int amount = 0)
	{
		if (show) {
			energyBonus.gameObject.SetActive (true);
			energyBonus.text = "+ " + amount.ToString () + " Energy";
		} else {
			energyBonus.gameObject.SetActive (false);
		}
	}

	Sprite returnMoonSprite(int age )
	{
		if (age < 0) {
			return moonSprites [(moonSprites.Length - 1) + age];
		} else if (age >= 0 && age <= moonSprites.Length - 1)
			return moonSprites [age];
		else {
			return moonSprites[ age - moonSprites.Length - 1];
		} 
	}

	private int JulianDate(int d, int m, int y)
	{
		int mm, yy;
		int k1, k2, k3;
		int j;

		yy = y - (int)((12 - m) / 10);
		mm = m + 9;
		if (mm >= 12)
		{
			mm = mm - 12;
		}
		k1 = (int)(365.25 * (yy + 4712));
		k2 = (int)(30.6001 * mm + 0.5);
		k3 = (int)((int)((yy / 100) + 49) * 0.75) - 38;
		j = k1 + k2 + d + 59;
		if (j > 2299160)
		{
			j = j - k3; 
		}
		return j;
	}

	private int MoonAge(int d, int m, int y)
	{
		int j = JulianDate(d, m, y);
		double ag;
		double ip = (j + 4.867) / 29.53059;
		ip = ip - Mathf.Floor((float)ip);
		if (ip < 0.5)
			ag = ip * 29.53059 + 29.53059 / 2;
		else
			ag = ip * 29.53059 - 29.53059 / 2;
		return Mathf.RoundToInt((float) ag);

	}

	int SetPlayerRelationToMoon()
	{
		float degree = Mathf.Lerp(0,1,(Mathf.InverseLerp(-14,14,PlayerDataManager.playerData.degree)));
		float diff = Mathf.Abs (degree - (float)data.luminosity);
		return (int)((1 - diff) * 100);
	}

}
