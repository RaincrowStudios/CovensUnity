using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class MoonManager : UIAnimationManager
{
    public GameObject container;
    public static MoonManager Instance { get; set; }
    int moonAge;
    public Sprite[] moonSprites;
    public Image middle;
    public Image r1;
    public Image r2;
    public Image l1;
    public Image l2;

    public TextMeshProUGUI currentMoonPhase;
    public TextMeshProUGUI moonDesc;
    public TextMeshProUGUI energyBonus;
    public TextMeshProUGUI playerRelation;
    public TextMeshProUGUI timer;
    public GameObject moonState;
    MoonData data;
    public Animator anim;


	[Header("Degree UI")]
	//[SerializeField] private GameObject AlignmentState;
	[SerializeField] private Image BarFill;
	[SerializeField] private TextMeshProUGUI CurrentDegree;
	[SerializeField] private TextMeshProUGUI NextDegree;
	[SerializeField] private GameObject WhiteIcon;
	[SerializeField] private GameObject ShadowIcon;
	[SerializeField] private GameObject BlackBar;
	public Sprite blackbase;
	public Sprite whitebase;



    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

        moonAge = MoonAge(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year);
        moonAge = Mathf.Clamp(moonAge, 0, 28);

    }
//
//    public void DelayOpen()
//    {
//        Invoke("Open", 1f);
//    }

    public void Open()
    {
        UIStateManager.Instance.CallWindowChanged(false);
        
		MapsAPI.Instance.HideMap(true);
		//Invoke("disableMap", 1f);
        SoundManagerOneShot.Instance.MenuSound();
        data = PlayerDataManager.moonData;
        container.SetActive(true);
        anim.Play("in");
        SetupMoon();
        DegreeSetup();
        StartCoroutine(CountDown());
    }

    void disableMap()
    {
        MapsAPI.Instance.HideMap(true);
    }

    IEnumerator CountDown()
    {
        while (true)
        {
            string t = Utilities.GetTimeRemaining(data.moonRise);
            if (t == "unknown")
            {
                timer.gameObject.SetActive(false);
                yield break;
            }
            else if (t == "null")
            {
                timer.text = "Moon has risen";
                moonState.SetActive(false);
                yield break;
            }
            else
            {
                moonState.SetActive(true);
                timer.text = t;
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void Close()
    {
        SoundManagerOneShot.Instance.MenuSound();
        anim.Play("out");
        UIStateManager.Instance.CallWindowChanged(true);
        MapsAPI.Instance.HideMap(false);
        StopCoroutine("CountDown");
        Disable(container, 1);
    }

    public void SetupMoon()
    {
        // Debug.Log();
        data.phase = Math.Round(data.phase, 2);
        Debug.Log("<b><color=red>MOON AGE " + moonAge + "</color></b>");
        r1.sprite = returnMoonSprite(moonAge + 1);
        r2.sprite = returnMoonSprite(moonAge + 2);
        middle.sprite = returnMoonSprite(moonAge);
        l1.sprite = returnMoonSprite(moonAge - 1);
        l2.sprite = returnMoonSprite(moonAge - 2);

        if (data.phase == 0)
            currentMoonPhase.text = "New Moon";
        else if (data.phase > 0 && data.phase < .25)
            currentMoonPhase.text = "Waxing Crescent";
        else if (data.phase == .25)
            currentMoonPhase.text = "First Quarter";
        else if (data.phase > .25 && data.phase < .5)
            currentMoonPhase.text = "Waxing Gibbous";
        else if (data.phase == .5)
            currentMoonPhase.text = "Full Moon";
        else if (data.phase > .5 && data.phase < .75)
            currentMoonPhase.text = "Waning Gibbous";
        else if (data.phase == .75)
            currentMoonPhase.text = "Last Quarter";
        else
            currentMoonPhase.text = "Waning Crescent";

        playerRelation.text = "As a <color=white>" + Utilities.witchTypeControlSmallCaps(PlayerDataManager.playerData.degree) + "</color>, you are <color=white>" + SetPlayerRelationToMoon().ToString() + "%</color> aligned with today's moon.";

        //moonDesc.text = "The <color=#ffffff>" + moonAge.ToString() + "</color> days young moon is <color=#ffffff>" + ((int)(data.luminosity * 100)).ToString() + "% </color> + illuminated.";
        moonDesc.text = LocalizeLookUp.GetText("moon_desc");
        moonDesc.text = moonDesc.text.Replace("{{Moon Age}}", "<color=#ffffff>" + moonAge.ToString() + "</color>")
            .Replace("{{Luminated}}", "<color=#ffffff>" + ((int)(data.luminosity * 100)).ToString() + "% </color>");
    }

    public void SetupSavannaEnergy(bool show, int amount = 0)
    {
        if (show)
        {
            energyBonus.gameObject.SetActive(true);
            energyBonus.text = "+ " + amount.ToString() + " Energy";
        }
        else
        {
            energyBonus.gameObject.SetActive(false);
        }
    }

    Sprite returnMoonSprite(int age)
    {

        if (age < 0)
        {
            return moonSprites[(moonSprites.Length - 1) + age];
        }
        else if (age >= 0 && age <= moonSprites.Length - 1)
            return moonSprites[age];
        else
        {
            return moonSprites[age - moonSprites.Length];
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
        return Mathf.RoundToInt((float)ag);

    }

    int SetPlayerRelationToMoon()
    {
        float degree = Mathf.Lerp(0, 1, (Mathf.InverseLerp(-14, 14, PlayerDataManager.playerData.degree)));
        float diff = Mathf.Abs(degree - (float)data.luminosity);
        return (int)((1 - diff) * 100);
    }


	public void DegreeSetup()
	{
        if (CurrentDegree == null || NextDegree == null)

        {
            Debug.LogError("orry, help, these are null");
            return;
        }


		if (PlayerDataManager.playerData.degree == 0) //setting up the Degree Bar UI if the witch is grey
		{
			BarFill.sprite = whitebase;
			BarFill.fillAmount = MapUtils.scale(0f,1f, -PlayerDataManager.playerData.maxAlignment, PlayerDataManager.playerData.maxAlignment, PlayerDataManager.playerData.currentAlignment);
			BarFill.color = new Color (0.73f, 0.73f, 0.73f, 1f);//"#BCBCBC";
			CurrentDegree.gameObject.SetActive (false);
			NextDegree.gameObject.SetActive (false);
			WhiteIcon.SetActive (true);
			ShadowIcon.SetActive (true);
			BlackBar.SetActive (true);
//			print ("min align" + PlayerDataManager.playerData.minAlignment);
//			print ("max align" + PlayerDataManager.playerData.maxAlignment);
//			print ("current" + PlayerDataManager.playerData.currentAlignment);
			//DegreeTitle.text = LocalizeLookUp.GetText ("chat_grey");




		} else {  //setting up the Degree Bar UI if the witch is not grey
			BlackBar.SetActive(false);
			WhiteIcon.SetActive (false);
			ShadowIcon.SetActive (false);
			CurrentDegree.gameObject.SetActive (true);
			NextDegree.gameObject.SetActive (true);
			CurrentDegree.text = Mathf.Abs(PlayerDataManager.playerData.degree).ToString();
			NextDegree.text = (Mathf.Abs(PlayerDataManager.playerData.degree)+1).ToString();
			//BarFill.fillAmount = (PDM.pD.attunementCurrent)/(PDM.pD.attunementNeeded);

//			print ("min align" + PlayerDataManager.playerData.minAlignment);
//			print ("max align" + PlayerDataManager.playerData.maxAlignment);
//			print ("current" + PlayerDataManager.playerData.currentAlignment);

			if (PlayerDataManager.playerData.degree > 0) { //white witch
				BarFill.sprite = whitebase;
				BarFill.color = new Color (1f, 1f, 1f, 1f);
				BarFill.fillAmount = MapUtils.scale(0f,1f, PlayerDataManager.playerData.minAlignment, PlayerDataManager.playerData.maxAlignment, PlayerDataManager.playerData.currentAlignment);
			} else if (PlayerDataManager.playerData.degree < 0) { //shadow witch
				BarFill.sprite = blackbase;
				BarFill.fillAmount = MapUtils.scale(0f,1f, Mathf.Abs(PlayerDataManager.playerData.maxAlignment), Mathf.Abs(PlayerDataManager.playerData.minAlignment), Mathf.Abs(PlayerDataManager.playerData.currentAlignment));
			}
			//BarFill.fillAmount = MapUtils.scale(0f,1f,PDM.pD.attunementMin, PDM.pD.attunementMax, PDM.pD.attunementCurrent);
		}
	}
}
