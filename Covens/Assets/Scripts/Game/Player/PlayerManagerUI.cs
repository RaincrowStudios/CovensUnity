﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;
using TMPro;

public class PlayerManagerUI : UIAnimationManager
{
    public static PlayerManagerUI Instance { get; set; }

    [Header("PlayerInfo UI")]
    public Text Level;
    public Text Energy;
    public Slider EnergySlider;
    public GameObject overFlowEn;
    public Text silverDrachs;
    public Text silverDrachsStore;
    public GameObject spiritForm;
    public GameObject physicalForm;
    public GameObject flyFX;
    public Text EnergyIso;
    FlightVisualManager FVM;

    public GameObject DailyBlessing;
    public Text blessingText;
    public Text locationEn;

    public GameObject levelUp;
    public Image iconLevelUp;
    public Sprite levelSp;
    public Sprite degreeSprite;
    public TextMeshProUGUI titleLevelup;
    public TextMeshProUGUI mainLevelup;
    public Image LunarPhaseHolder;
    public Sprite[] LunarPhase;
    public Slider xpSlider;
    public Text xpText;

    public Text EnergyElixirText;
    public GameObject EnergyElixir;
    public Button elixirButton;

    int elixirCount;

    public GameObject EnergyStore;
    public GameObject PotionsStore;
    public GameObject leftButton;
    public GameObject rightButton;
    public CanvasGroup curDominion;


    public GameObject DeathReason;
    public Text deathDesc;
    public Text deathblessing;
    bool isDay = true;
    bool cancheck = true;



    void Awake()
    {
        Instance = this;
        FVM = GetComponent<FlightVisualManager>();
    }

    // ___________________________________________ Main Player UI ________________________________________________________________________________________________

    public void SetupUI()
    {
        Level.text = PlayerDataManager.playerData.level.ToString();
        EnergyIso.text = PlayerDataManager.playerData.energy.ToString();
        //		Energy.text = PlayerDataManager.playerData.energy.ToString() + PlayerDataManager.pla;
        SetupEnergy();
        UpdateDrachs();
        StartCoroutine(CheckTime());
        SetupAlignmentPhase();
        setupXP();
        // if (PlayerDataManager.playerData.state == "vulnerable")
        // {
        //     // ShowElixirVulnerable(false);
        // }
        if (PlayerDataManager.playerData.state == "dead")
        {
            DeathState.Instance.ShowDeath();
        }
    }

    public void checkTime()
    {
        try
        {
            if (!cancheck)
                return;
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            var sunSetTime = dtDateTime.AddMilliseconds(PlayerDataManager.config.sun.sunSet).ToUniversalTime();
            var sunRiseTime = dtDateTime.AddMilliseconds(PlayerDataManager.config.sun.sunRise).ToUniversalTime();
            var curTime = DateTime.UtcNow;
            bool day = isDay;
            if (DateTime.Now.Hour > 18)
            {
                day = false;
            }
            else if (DateTime.Now.Hour < 7)
            {
                day = false;
            }
            else
            {
                day = true;

            }

            if (isDay != day)
            {
                Debug.Log("CHANGING STYLE");
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
        //print("Switiching style");
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
        levelUp.SetActive(true);
        titleLevelup.text = "You Leveled up!";
        mainLevelup.text = "Level " + Level.text + "!";
        iconLevelUp.sprite = levelSp;
        setupXP();
    }

    public void playerDegreeChanged()
    {
        levelUp.SetActive(true);
        titleLevelup.text = "Your Alignment Changed!";
        mainLevelup.text = Utilities.witchTypeControlSmallCaps(PlayerDataManager.playerData.degree);
        iconLevelUp.sprite = degreeSprite;
        SetupAlignmentPhase();
    }

    void SetupBlessing()
    {
        blessingText.text = "The Dea Savannah Grey has granted you her daily gift of <color=#FF9900>" + PlayerDataManager.playerData.blessing.daily.ToString() + "</color> energy";
        if (PlayerDataManager.playerData.blessing.locations > 0)
        {
            locationEn.text = "You also gained " + PlayerDataManager.playerData.blessing.locations.ToString() + " energy from your Places of Power";
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

    public void UpdateDrachs()
    {
        try
        {
            silverDrachs.text = PlayerDataManager.playerData.silver.ToString();
            silverDrachsStore.text = PlayerDataManager.playerData.silver.ToString();
        }
        catch
        {
        }
    }

    public void UpdateEnergy()
    {
        SetupEnergy();
    }

    public void Flight()
    {
        physicalForm.SetActive(false);
        spiritForm.SetActive(true);
        flyFX.SetActive(true);
        FVM.FadeOut();
    }

    public void Hunt()
    {
        flyFX.SetActive(false);
        FVM.FadeIn();

    }

    public void home()
    {
        spiritForm.SetActive(false);
        physicalForm.SetActive(true);
    }

    IEnumerator CheckTime()
    {
        while (true)
        {
            if (System.DateTime.Now.Hour == 0 && System.DateTime.Now.Minute == 0 && System.DateTime.Now.Second == 0)
            {
                //TODO add daily blessing check
                yield return new WaitForSeconds(1);
                print("Checking Reset");
                APIManager.Instance.GetData("character/get", (string s, int r) =>
                {

                    if (r == 200)
                    {
                        var rawData = JsonConvert.DeserializeObject<MarkerDataDetail>(s);
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
    //     print(s + r);
    //     if (r == 200)
    //     {
    //         SoundManagerOneShot.Instance.PlayReward();
    //         elixirButton.interactable = true;
    //         elixirCount--;
    //         print(elixirCount + "Elixir Changed");

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
        curDominion.GetComponent<Text>().text = "~ Dominion of " + dominion + " ~";
    }

    public void ShowGarden(string id)
    {
        StartCoroutine(domAnim());
        curDominion.GetComponent<Text>().text = "~ " + DownloadedAssets.gardenDict[id].title + " ~";
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

    public void ShowDeathReason(string s)
    {
        print(s);
        if (!LoginUIManager.isInFTF)
        {
            deathDesc.text = s;

            System.DateTime timeNow = System.DateTime.Now;
            System.DateTime timeMidnight = System.DateTime.Today.AddDays(1);
            System.TimeSpan ts = timeMidnight.Subtract(timeNow);
            int hours = (int)ts.TotalHours;

            deathblessing.text = "Savannah's next blessing will come in " + hours.ToString() + " hours.";
            Invoke("deathReasonShow", 2.5f);
        }
    }

    void deathReasonShow()
    {
        DeathReason.SetActive(true);
    }
}

