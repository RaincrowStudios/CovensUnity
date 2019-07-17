using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;
using TMPro;
using Raincrow.Chat.UI;

public class PlayerManagerUI : UIAnimationManager
{
    public static PlayerManagerUI Instance { get; set; }

    [Header("PlayerInfo UI")]
    public Text Level;
    public TextMeshProUGUI Energy;
    public Slider EnergySlider;
    public GameObject overFlowEn;
    public TextMeshProUGUI silverDrachs;
    public TextMeshProUGUI goldDrachs;
    public TextMeshProUGUI silverDrachsStore;
    public TextMeshProUGUI goldDrachsStore;
    public GameObject physicalForm;
    public GameObject spiritForm;
    public GameObject coinGlow;
    FlightVisualManager FVM;
    public GameObject LandFX;
    public Button flightButton;
    public GameObject DailyBlessing;
    public Text blessingText;
    public Text locationEn;

    public Image LunarPhaseHolder;
    public Sprite[] LunarPhase;
    public Slider xpSlider;
    public TextMeshProUGUI xpText;

    public GameObject EnergyElixir;
    public Button elixirButton;

    int elixirCount;

    public CanvasGroup curDominion;

    public GameObject DeathReason;
    public Text deathDesc;
    public Text deathblessing;
    public Button ChatButton;

    [SerializeField] private UIChat uiChat;

    bool isDay = true;
    bool cancheck = true;

    bool firstRun = true;
    private bool m_IsPhysicalForm = true;

    //last daily check
    double LastDailyTimeStamp;

    void Awake()
    {
        if (spiritForm == null)
        {
            spiritForm = Instantiate(physicalForm);
            spiritForm.transform.SetParent(physicalForm.transform.parent);
            spiritForm.transform.position = physicalForm.transform.position;
            spiritForm.transform.localScale = physicalForm.transform.localScale;
            spiritForm.transform.rotation = physicalForm.transform.rotation;
        }
        Instance = this;
        FVM = GetComponent<FlightVisualManager>();
        physicalForm.SetActive(false);
        spiritForm.SetActive(false);

        if (PlayerPrefs.GetString("LastDailyTimeStamp") == string.Empty)
        {
            PlayerPrefs.SetString("LastDailyTimeStamp", "0.0");
            PlayerPrefs.Save();
        }

        LastDailyTimeStamp = Double.Parse(PlayerPrefs.GetString("LastDailyTimeStamp"));
    }

    private void Start()
    {
        PlayerManager.onFinishFlight += CheckPhysicalForm;
        SetupUI();
    }

    public void SetupChatAction()
    {
        Debug.Log("ADDED CHAT ACTION");
        ChatButton.onClick.AddListener(() => uiChat.Show());
    }
    // ___________________________________________ Main Player UI ________________________________________________________________________________________________

    private void SetupUI()
    {
        Level.text = PlayerDataManager.playerData.level.ToString();

        SetupEnergy();
        UpdateDrachs();

        CheckForNewDay();
        SetupAlignmentPhase();
        setupXP();

        if (PlayerDataManager.playerData.state == "dead")
        {
            DeathState.Instance.ShowDeath();
        }
        flightButton.onClick.AddListener(() => { MapFlightTransition.Instance.FlyOut(); });
    }

    public void checkTime()
    {
        try
        {
            if (!cancheck)
                return;

            Debug.LogError("TODO: CHECK DAY TIME");
            bool day = isDay;
            cancheck = false;

            //DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            //var sunSetTime = dtDateTime.AddMilliseconds(PlayerDataManager.config.sun.sunSet).ToUniversalTime();
            //var sunRiseTime = dtDateTime.AddMilliseconds(PlayerDataManager.config.sun.sunRise).ToUniversalTime();
            //var curTime = DateTime.UtcNow;
            //bool day = isDay;
            //if (DateTime.Now.Hour > 18)
            //{
            //    day = false;
            //}
            //else if (DateTime.Now.Hour < 7)
            //{
            //    day = false;
            //}
            //else
            //{
            //    day = true;

            //}

            if (isDay != day)
            {
                SwitchMapStyle();
                cancheck = false;
            }
        }
        catch
        {
        }
    }

    public void SwitchMapStyle()
    {
        Debug.LogError("SwitchMapStyle disabled");
        //Debug.Log("Switiching style");
        //isDay = !isDay;
        //try
        //{
        //    MapsAPI.Instance.ClearAllCaches();
        //}
        //catch
        //{
        //}
        //if (isDay)
        //    MapsAPI.Instance.customProviderURL = "https://api.mapbox.com/styles/v1/raincrowgames/cjnxd56my4v7y2rqo9jf2tmxc/draft/tiles/256/{zoom}/{x}/{y}?access_token=pk.eyJ1IjoicmFpbmNyb3dnYW1lcyIsImEiOiJxZDZRWERnIn0.EmZcgJhT80027oPahMqJLA";
        //else
        //    MapsAPI.Instance.customProviderURL = "https://api.mapbox.com/styles/v1/raincrowgames/ciogu7y80000acom697bfaofp/draft/tiles/256/{zoom}/{x}/{y}?access_token=pk.eyJ1IjoicmFpbmNyb3dnYW1lcyIsImEiOiJxZDZRWERnIn0.EmZcgJhT80027oPahMqJLA";

        //MapsAPI.Instance.zoom += 5;
        //MapsAPI.Instance.RedrawImmediately();
        //MapsAPI.Instance.zoom -= 5;
    }

    void SetupEnergy()
    {
        var pData = PlayerDataManager.playerData;
        if (pData.baseEnergy >= pData.energy)
        {
            Energy.text = pData.energy.ToString() + "/" + pData.baseEnergy;
            EnergySlider.maxValue = pData.baseEnergy;
            EnergySlider.value = pData.energy;
            overFlowEn.SetActive(false);

        }
        else
        {
            overFlowEn.SetActive(true);
            EnergySlider.maxValue = pData.baseEnergy;
            EnergySlider.value = pData.baseEnergy;
            Energy.text = "<b>" + pData.energy.ToString() + "</b>/" + pData.baseEnergy;
        }
    }

    public void setupXP()
    {
        xpSlider.maxValue = PlayerDataManager.playerData.xpToLevelUp;
        xpSlider.value = PlayerDataManager.playerData.xp;
        xpText.text = PlayerDataManager.playerData.xp.ToString() + "/" + PlayerDataManager.playerData.xpToLevelUp.ToString();
    }

    void SetupAlignmentPhase()
    {
        var lp = PlayerDataManager.playerData.degree;
        if (lp == 0)
            LunarPhaseHolder.sprite = LunarPhase[7];
        if (lp == 1 || lp == 2)
            LunarPhaseHolder.sprite = LunarPhase[8];
        if (lp == 3 || lp == 4)
            LunarPhaseHolder.sprite = LunarPhase[9];
        if (lp == 5 || lp == 6)
            LunarPhaseHolder.sprite = LunarPhase[10];
        if (lp == 7 || lp == 8)
            LunarPhaseHolder.sprite = LunarPhase[11];
        if (lp == 9 || lp == 10)
            LunarPhaseHolder.sprite = LunarPhase[12];
        if (lp == 11 || lp == 12)
            LunarPhaseHolder.sprite = LunarPhase[13];
        if (lp == 13 || lp == 14)
            LunarPhaseHolder.sprite = LunarPhase[14];


        if (lp == -1 || lp == -2)
            LunarPhaseHolder.sprite = LunarPhase[6];
        if (lp == -3 || lp == -4)
            LunarPhaseHolder.sprite = LunarPhase[5];
        if (lp == -5 || lp == -6)
            LunarPhaseHolder.sprite = LunarPhase[4];
        if (lp == -7 || lp == -8)
            LunarPhaseHolder.sprite = LunarPhase[3];
        if (lp == -9 || lp == -10)
            LunarPhaseHolder.sprite = LunarPhase[2];
        if (lp == -11 || lp == -12)
            LunarPhaseHolder.sprite = LunarPhase[1];
        if (lp == -13 || lp == -14)
            LunarPhaseHolder.sprite = LunarPhase[0];
    }

    public void playerlevelUp()
    {
        Level.text = PlayerDataManager.playerData.level.ToString();
        //levelUp.SetActive(true);
        //titleLevelup.text = "You Leveled up!";
        //mainLevelup.text = "Level " + Level.text + "!";
        //iconLevelUp.sprite = levelSp;
        setupXP();
    }

    public void playerDegreeChanged()
    {
        LineRendererBasedDome.Instance.SetupDome();
        PlayerManager.Instance.AddAttackRing();
        //levelUp.SetActive(true);
        //titleLevelup.text = "Your Alignment Changed!";
        //mainLevelup.text = Utilities.witchTypeControlSmallCaps(PlayerDataManager.playerData.degree);
        //iconLevelUp.sprite = degreeSprite;
        SetupAlignmentPhase();
    }

    void SetupBlessing()
    {
        if (PlayerDataManager.playerData.blessing.daily != 0)
        {
            blessingText.text = LocalizeLookUp.GetText("blessing_grant");
            blessingText.text = blessingText.text.Replace("{{amount}}", "<color=#FF9900>" + PlayerDataManager.playerData.blessing.daily.ToString() + "</color>");
        }
        else
            blessingText.text = LocalizeLookUp.GetText("blessing_full");
        //blessingText.text = "The Dea Savannah Grey has granted you her daily gift of <color=#FF9900>" + PlayerDataManager.playerData.blessing.daily.ToString() + "</color> energy";
        if (PlayerDataManager.playerData.blessing.locations > 0)
        {
            locationEn.text = LocalizeLookUp.GetText("blessing_pop").Replace("{{amount}}", PlayerDataManager.playerData.blessing.locations.ToString());// "You also gained " + PlayerDataManager.playerData.blessing.locations.ToString() + " energy from your Places of Power";
        }
        else
        {
            locationEn.text = "";
        }
    }

    public void ShowBlessing()
    {
        SoundManagerOneShot.Instance.MenuSound();
        SetupBlessing();
        Show(DailyBlessing);

    }

    public void HideBlessing()
    {
        DailyBlessing.GetComponent<Animator>().Play("out");
        //Disable(DailyBlessing, 1.3f);
        Destroy(DailyBlessing.gameObject, 1.3f);
    }

    public void UpdateDrachs(bool updateStore = true)
    {
        Debug.Log("Update Drachs");
        try
        {
            //Lerping between old gold # and new gold #
            var g = goldDrachs.text;
            var d = PlayerDataManager.playerData.gold.ToString();
            var g2 = float.Parse(g);
            var d2 = float.Parse(d);
            if (g2 < d2)
            {
                goldDrachs.transform.parent.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true); //activating glimmer fx
            }
            LeanTween.value(g2, d2, 1f).setOnUpdate((float i) =>
            {
                i = (int)i;
                goldDrachs.text = i.ToString();
            });
            //Lerping between old silver # and new silver #
            var p = silverDrachs.text;
            var s = PlayerDataManager.playerData.silver.ToString();
            var p2 = float.Parse(p);
            var s2 = float.Parse(s);
            if (p2 < s2)
            {
                goldDrachs.transform.parent.transform.parent.GetChild(0).GetChild(0).gameObject.SetActive(true); //activating glimmer fx
            }

            LeanTween.value(p2, s2, 1f).setOnUpdate((float i) =>
            {
                i = (int)i;
                silverDrachs.text = i.ToString();
            });
            Debug.Log("p: " + p);
            Debug.Log("s: " + s);


            //running Lerp if you are in the store
            if (updateStore)
            {
                silverDrachsStore.text = PlayerDataManager.playerData.silver.ToString();
                goldDrachsStore.text = PlayerDataManager.playerData.gold.ToString();
            }

        }
        catch
        {
        }
    }

    public void UpdateEnergy()
    {
        SetupEnergy();
    }
    // private bool hasPlayed = false;
    public void CheckPhysicalForm()
    {
        bool isPhysical = !PlayerManager.inSpiritForm;

        if (m_IsPhysicalForm != isPhysical)
        {
            m_IsPhysicalForm = isPhysical;
            physicalForm.SetActive(isPhysical);
            spiritForm.SetActive(!isPhysical);

            if (!isPhysical)
            {
                SoundManagerOneShot.Instance.PlaySpiritForm();
            }
            else
            {
                SoundManagerOneShot.Instance.PlayReturnPhysical();
            }
        }
    }

    /*
    private void OnGUI()
    {
        if (GUI.Button(new Rect(30,30,100,50), "set pref"))
        {
            PlayerPrefs.SetString("LastDailyTimeStamp", "1559485920000");
            Debug.Log(PlayerPrefs.GetString("LastDailyTimeStamp"));
            StopCoroutine("WaitForTime");
            CheckForNewDay();
        }
    }
    */

    IEnumerator WaitForTime(float time)
    {
        yield return new WaitForSeconds(time);
        DailyBlessingCheck(-Utilities.TimespanFromJavaTime(LastDailyTimeStamp));
    }


    void DailyBlessingCheck(TimeSpan time)
    {


        Debug.Log(time.TotalHours);
        Debug.Log(time.TotalMinutes);
        Debug.Log(time.TotalSeconds);
        if (time.TotalHours < 23)
        {
            Debug.Log("gonna wait an hour");
            StartCoroutine(WaitForTime(3600f));
            return;
        }
        else if (time.TotalMinutes < 1439)
        {
            Debug.Log("gonna wait a minute");
            StartCoroutine(WaitForTime(60f));
            return;
        }
        else if (time.TotalSeconds < 86400)
        {
            Debug.Log("gonna wait a second");
            StartCoroutine(WaitForTime(1f));
            return;
        }
        Debug.LogError("executing daily blessing");
        CheckForNewDay();
        //PlayerPrefs.SetString("secondsSinceLastDaily", totalMilliseconds.ToString());
    }

    public void CheckForNewDay()
    {
        //here I put the double that gustavo sends me
        //currently it is hard coded to yesterday
        LastDailyTimeStamp = Double.Parse(PlayerPrefs.GetString("LastDailyTimeStamp"));
        // Debug.Log(LastDailyTimeStamp);

        var timeSpan = -Utilities.TimespanFromJavaTime(LastDailyTimeStamp);
        Debug.LogError("TODO: DAILY CHECK");

        //if (timeSpan.TotalDays > 1)
        //{
        //    APIManager.Instance.Get("character/get", (string s, int r) =>
        //    {

        //        if (r == 200)
        //        {
        //            LastDailyTimeStamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        //            PlayerPrefs.SetString("LastDailyTimeStamp", LastDailyTimeStamp.ToString());

        //            //PlayerPrefs.Save();
        //            var rawData = JsonConvert.DeserializeObject<PlayerDataDetail>(s);
        //            if (rawData.dailyBlessing)
        //            {
        //                PlayerDataManager.playerData.blessing = rawData.blessing;
        //                ShowBlessing();
        //            }
        //        }
        //        else
        //        {
        //            Debug.LogError("character/get failure : " + s);
        //        }

        //    });
        //}
        //else
        //{
        //    // Debug.Log("A day hasn't passed");
        //    DailyBlessingCheck(-Utilities.TimespanFromJavaTime(LastDailyTimeStamp));
        //}
    }

    IEnumerator CheckTime()
    {
        while (true)
        {
            if (System.DateTime.Now.Hour == 0 && System.DateTime.Now.Minute == 0 && System.DateTime.Now.Second == 0)
            {
                //TODO add daily blessing check
                yield return new WaitForSeconds(1);
                Debug.Log("Checking Reset");
                APIManager.Instance.Get("character/get", (string s, int r) =>
                {

                    if (r == 200)
                    {
                        var rawData = JsonConvert.DeserializeObject<PlayerData>(s);
                        if (rawData.dailyBlessing)
                        {
                            PlayerDataManager.playerData.blessing = rawData.blessing;
                            ShowBlessing();
                        }
                    }

                });
            }

            yield return new WaitForSeconds(1);

            if (EnergyElixir.activeSelf)
            {
                if (PlayerDataManager.playerData.energy > PlayerDataManager.playerData.baseEnergy * 0.6f)
                {
                    elixirButton.onClick.RemoveAllListeners();
                    Hide(EnergyElixir, true, 6);
                }
            }
        }
    }

    public void Revived()
    {
        Hide(EnergyElixir, true, 4);
    }

    public void ShowElixirOnBuy()
    {
        // if (PlayerDataManager.playerData.state == "vulnerable")
        // {
        //     ShowElixirVulnerable(false);
        // }
        // else
        // {
        //     ShowElixirVulnerable(true);
        // }
    }

    // public void ShowElixirVulnerable(bool Persist)
    // {
    //     if (Persist)
    //     {
    //         Show(EnergyElixir, true);
    //     }
    //     else
    //     {
    //         Show(EnergyElixir, false);
    //     }
    //     foreach (var item in PlayerDataManager.playerData.inventory.consumables)
    //     {
    //         if (item.id.Contains("energy"))
    //         {
    //             elixirCount = item.count;
    //         }
    //     }

    //     if (PlayerDataManager.playerData.energy > PlayerDataManager.playerData.baseEnergy * 0.6f)
    //     {
    //         elixirButton.onClick.RemoveAllListeners();
    //         Hide(EnergyElixir, true, 6);
    //     }
    //     else if (elixirCount == 0)
    //     {
    //         elixirButton.onClick.RemoveListener(ConsumeElixir);
    //         elixirButton.onClick.RemoveListener(ShowStore);
    //         elixirButton.onClick.AddListener(ShowStore);
    //         EnergyElixirText.text = "Buy Energy";
    //         if (!Persist)
    //         {
    //             Hide(EnergyElixir, true, 6);
    //         }
    //     }
    //     else
    //     {
    //         EnergyElixirText.text = "Consume (" + elixirCount.ToString() + ")";
    //         elixirButton.onClick.RemoveListener(ShowStore);
    //         elixirButton.onClick.RemoveListener(ConsumeElixir);
    //         elixirButton.onClick.AddListener(ConsumeElixir);
    //     }
    // }

    // public void UpdateElixirCount()
    // {
    //     foreach (var item in PlayerDataManager.playerData.inventory.consumables)
    //     {
    //         if (item.id.Contains("energy"))
    //         {
    //             elixirCount = item.count;
    //             if (elixirCount == 0)
    //                 Hide(EnergyElixir, true);
    //             EnergyElixirText.text = "Consume (" + elixirCount.ToString() + ")";
    //         }
    //     }
    // }

    // public void ShowStore()
    // {
    //     StoreUIManager.Instance.GetStore();
    //     Invoke("showEnergyStore", .3f);

    // }

    // void showEnergyStore()
    // {
    //     StoreUIManager.Instance.ShowElixir(true);
    //     EnergyStore.SetActive(true);
    //     PotionsStore.SetActive(false);
    //     leftButton.SetActive(false);
    //     rightButton.SetActive(true);
    // }

    // public void ConsumeElixir()
    // {
    //     var data = new { consumable = "consumable_energyElixir1" };
    //     APIManager.Instance.PostData("inventory/consume", JsonConvert.SerializeObject(data), Result);
    //     elixirButton.interactable = false;
    // }

    // public void Result(string s, int r)
    // {
    //     Debug.Log(s + r);
    //     if (r == 200)
    //     {
    //         SoundManagerOneShot.Instance.PlayReward();
    //         elixirButton.interactable = true;
    //         elixirCount--;
    //         Debug.Log(elixirCount + "Elixir Changed");

    //         foreach (var item in PlayerDataManager.playerData.inventory.consumables)
    //         {
    //             if (item.id.Contains("energy"))
    //             {
    //                 item.count = elixirCount;
    //             }
    //         }

    //         if (elixirCount > 0)
    //         {
    //             EnergyElixirText.text = "Consume (" + elixirCount.ToString() + ")";
    //             Invoke("HideDelay", 6f);
    //         }
    //         else
    //         {
    //             Hide(EnergyElixir, true, .1f);
    //         }
    //     }
    // }

    // void HideDelay()
    // {
    //     Hide(EnergyElixir, true);
    // }

    public void ShowDominion(string dominion)
    {
        StartCoroutine(domAnim());
        curDominion.GetComponent<Text>().text = LocalizeLookUp.GetText("show_dominion").Replace("{{Dominion Name}}", dominion);// "~ Dominion of " + dominion + " ~";
    }

    public void ShowGarden(string id)
    {
        StartCoroutine(domAnim());
        curDominion.GetComponent<Text>().text = LocalizeLookUp.GetGardenName(id);
    }

    IEnumerator domAnim()
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * 1.5f;
            curDominion.alpha = Mathf.SmoothStep(0, 1, t);
            yield return 0;
        }

        yield return new WaitForSeconds(5);
        t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * 2;
            curDominion.alpha = Mathf.SmoothStep(1, 0, t);
            yield return 0;
        }
    }

    public void ShowDeathReason()
    {
        //Debug.Log(localizeID);
        if (!LoginUIManager.isInFTF)
        {

            System.DateTime timeNow = System.DateTime.Now;
            System.DateTime timeMidnight = System.DateTime.Today.AddDays(1);
            System.TimeSpan ts = timeMidnight.Subtract(timeNow);
            int hours = (int)ts.TotalHours;

            deathblessing.text = LocalizeLookUp.GetText("blessing_time").Replace("{{Hours}}", hours.ToString());// "Savannah's next blessing will come in " + hours + " hours or you can ask for a fellow witch to revive you.";
            Invoke("deathReasonShow", 2.5f);
        }
    }

    void deathReasonShow()
    {
        DeathReason.SetActive(true);
    }
}

