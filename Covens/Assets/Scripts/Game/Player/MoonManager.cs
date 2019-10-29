using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Raincrow.FTF;


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
    public TextMeshProUGUI spellEfficiency;
    public TextMeshProUGUI playerRelation;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI dailytext;
    public bool isopen;
    public GameObject moonState;
    MoonData data;
    public Animator anim;
    private Button alignmentButton;

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

    public static float[] LunarEffeciency { get; set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        moonAge = MoonAge(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year);
        moonAge = Mathf.Clamp(moonAge, 0, 28);
        alignmentButton = PlayerManagerUI.Instance.LunarPhaseHolder.transform.GetChild(0).GetComponent<Button>();
        dailytext = container.transform.GetChild(11).GetChild(2).GetComponent<TextMeshProUGUI>();
        alignmentButton.onClick.AddListener(() =>
        {
            alignmentButton.enabled = false;
        });

    }
    //
    //    public void DelayOpen()
    //    {
    //        Invoke("Open", 1f);
    //    }

    public void Open()
    {
        BackButtonListener.AddCloseAction(Close);

        UIStateManager.Instance.CallWindowChanged(false);
        container.transform.GetChild(9).GetComponent<Button>().onClick.AddListener(() =>
        {
            Close();
        });
        MapsAPI.Instance.HideMap(true);
        //Invoke("disableMap", 1f);
        SoundManagerOneShot.Instance.MenuSound();
        data = PlayerDataManager.moonData;

        container.SetActive(true);
        anim.Play("in");
        SetupMoon();
        DegreeSetup();
        SetupDailyBlessing();
        if (FirstTapManager.IsFirstTime("moonphase"))
            FirstTapManager.Show("moonphase", null);
        StartCoroutine(CountDown());
    }

    void disableMap()
    {
        MapsAPI.Instance.HideMap(true);
    }

    IEnumerator CountDown()
    {
        System.DateTime moonRise = Utilities.FromJavaTime(data.moonRise);

        while (true)
        {
            string t = Utilities.GetTimeRemaining(data.moonRise);
            if (t == "unknown")
            {
                timer.gameObject.SetActive(false);
                yield break;
            }
            else if (t == "")
            {
                timer.text = "\n" + LocalizeLookUp.GetText("moon_rise");// "\nMoon has risen";
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
        BackButtonListener.RemoveCloseAction();

        StartCoroutine(EnableAlignmentButtonInteractability());
        SoundManagerOneShot.Instance.MenuSound();
        anim.Play("out");
        UIStateManager.Instance.CallWindowChanged(true);
        MapsAPI.Instance.HideMap(false);
        StopCoroutine("CountDown");
        Disable(container, 1);

    }

    IEnumerator EnableAlignmentButtonInteractability()
    {
        yield return new WaitForSeconds(1f);
        alignmentButton.enabled = true;
    }
    public void SetupDailyBlessing()
    {
        TimeSpan nextBlessing = BlessingManager.TimeUntilNextBlessing();
        int hours = nextBlessing.Days > 0 ? 24 :  nextBlessing.Hours;
        int minutes = nextBlessing.Minutes;
        int seconds = nextBlessing.Seconds;

        dailytext.text = string.Concat(hours.ToString("D2"),
        ":", minutes.ToString("D2"),
        ":",seconds.ToString("D2"));

        if (container.gameObject.activeSelf)
        {
            LeanTween.value(0f, 1f, 1f).setOnComplete(() =>
            {
                SetupDailyBlessing();
            });
        }
    }
    public void SetupMoon()
    {
        data.phase = Math.Round(data.phase, 2);

        r1.sprite = returnMoonSprite(moonAge + 1);
        r2.sprite = returnMoonSprite(moonAge + 2);
        middle.sprite = returnMoonSprite(moonAge);
        l1.sprite = returnMoonSprite(moonAge - 1);
        l2.sprite = returnMoonSprite(moonAge - 2);

        if (data.phase == 0)
            currentMoonPhase.text = LocalizeLookUp.GetText("moon_new");// "New Moon";
        else if (data.phase > 0 && data.phase < .25)
            currentMoonPhase.text = LocalizeLookUp.GetText("moon_wax_cresc");//"Waxing Crescent";
        else if (data.phase == .25)
            currentMoonPhase.text = LocalizeLookUp.GetText("moon_first_quart");//"First Quarter";
        else if (data.phase > .25 && data.phase < .5)
            currentMoonPhase.text = LocalizeLookUp.GetText("moon_wax_gib");//"Waxing Gibbous";
        else if (data.phase == .5)
            currentMoonPhase.text = LocalizeLookUp.GetText("moon_full");//"Full Moon";
        else if (data.phase > .5 && data.phase < .75)
            currentMoonPhase.text = LocalizeLookUp.GetText("moon_wan_gib");//"Waning Gibbous";
        else if (data.phase == .75)
            currentMoonPhase.text = LocalizeLookUp.GetText("moon_last_quart");//"Last Quarter";
        else
            currentMoonPhase.text = LocalizeLookUp.GetText("moon_wan_cresc");//"Waning Crescent";

        playerRelation.text = LocalizeLookUp.GetText("moon_relation").Replace("{{Witch Type}}", Utilities.WitchTypeControlSmallCaps(PlayerDataManager.playerData.degree)).Replace("{{Alignment}}", SetPlayerRelationToMoon().ToString() + "%");//"As a <color=white>" + Utilities.witchTypeControlSmallCaps(PlayerDataManager.playerData.degree) + "</color>, you are <color=white>" + SetPlayerRelationToMoon().ToString() + "%</color> aligned with today's moon.";

        //moonDesc.text = "The <color=#ffffff>" + moonAge.ToString() + "</color> days young moon is <color=#ffffff>" + ((int)(data.luminosity * 100)).ToString() + "% </color> + illuminated.";
        moonDesc.text = LocalizeLookUp.GetText("moon_desc");
        moonDesc.text = moonDesc.text.Replace("{{Moon Age}}", "<color=#ffffff>" + moonAge.ToString() + "</color>")
            .Replace("{{Luminated}}", "<color=#ffffff>" + ((int)(data.luminosity * 100)).ToString() + "% </color>");

        float lunarEffect = GetLunarEfficiency(PlayerDataManager.playerData.degree, (float)data.luminosity);
        if (lunarEffect == 0)
            spellEfficiency.text = LocalizeLookUp.GetText("spell_efficiency_bonus").Replace("{{value}}", "+0");
        else
            spellEfficiency.text = LocalizeLookUp.GetText("spell_efficiency_bonus").Replace("{{value}}", (lunarEffect * 100).ToString(" +#;-#"));// + "% Spell efficiency";
    }

    //public void SetupSavannaEnergy(bool show, int amount = 0)
    //{
    //    if (show)
    //    {
    //        spellEfficiency.gameObject.SetActive(true);
    //        spellEfficiency.text = "+" + LocalizeLookUp.GetText("moon_energy").Replace("{{Amount}}", amount.ToString());// "+ " + amount.ToString() + " Energy";
    //    }
    //    else
    //    {
    //        spellEfficiency.gameObject.SetActive(true);
    //        spellEfficiency.text = "+" + LocalizeLookUp.GetText("moon_energy").Replace("{{Amount}}", "0");//"+0 Energy";
    //    }
    //}

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

#if UNITY_EDITOR
    [Header("Lunar efficiency debugging")]
    public int debugPlayerDegree;
    public float debugLuminosity = 0.6932794689390696f;

    [ContextMenu("Debug lunar efficiency")]
    private void DebugLunarEfficiency()
    {
        LunarEffeciency = new float[]
        {
            0.0f,
            0.0f,
            0.0f,
            0.0f,
            0.0f,
            0.0f,
            0.05f,
            0.1f,
            0.2f,
            0.3f,
            0.2f,
            0.1f,
            0.05f,
            0.0f,
            0.0f
        };

        Debug.Log("Lunar Efficiency: " + GetLunarEfficiency(debugPlayerDegree, debugLuminosity));
    }
#endif

    private float GetLunarEfficiency(int playerDegree, float luminosity)
    {
        float moonFraction = luminosity - 0.5f;
        int buffDegree = Mathf.RoundToInt(Mathf.Abs((moonFraction * 14) / 0.5f));
        float lunarEffect = (LunarEffeciency[buffDegree] * Mathf.Sign(playerDegree)) * Mathf.Sign(moonFraction);
        return lunarEffect;
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
            BarFill.fillAmount = MapUtils.scale(0f, 1f, -PlayerDataManager.playerData.maxAlignment, PlayerDataManager.playerData.maxAlignment, PlayerDataManager.playerData.alignment);
            BarFill.color = new Color(0.73f, 0.73f, 0.73f, 1f);//"#BCBCBC";
            CurrentDegree.gameObject.SetActive(false);
            NextDegree.gameObject.SetActive(false);
            WhiteIcon.SetActive(true);
            ShadowIcon.SetActive(true);
            BlackBar.SetActive(true);





        }
        else
        {
            //setting up the Degree Bar UI if the witch is not grey
            BlackBar.SetActive(false);
            WhiteIcon.SetActive(false);
            ShadowIcon.SetActive(false);
            CurrentDegree.gameObject.SetActive(true);
            NextDegree.gameObject.SetActive(true);
            CurrentDegree.text = Mathf.Abs(PlayerDataManager.playerData.degree).ToString();
            NextDegree.text = (Mathf.Abs(PlayerDataManager.playerData.degree) + 1).ToString();

            if (PlayerDataManager.playerData.degree > 0)
            {
                //white witch
                BarFill.sprite = whitebase;
                BarFill.color = new Color(1f, 1f, 1f, 1f);
                BarFill.fillAmount = MapUtils.scale(0f, 1f, PlayerDataManager.playerData.minAlignment, PlayerDataManager.playerData.maxAlignment, PlayerDataManager.playerData.alignment);
            }
            else if (PlayerDataManager.playerData.degree < 0)
            {
                //shadow witch
                BarFill.sprite = blackbase;
                BarFill.fillAmount = MapUtils.scale(0f, 1f, Mathf.Abs(PlayerDataManager.playerData.maxAlignment), Mathf.Abs(PlayerDataManager.playerData.minAlignment), Mathf.Abs(PlayerDataManager.playerData.alignment));
            }
        }
    }
}
