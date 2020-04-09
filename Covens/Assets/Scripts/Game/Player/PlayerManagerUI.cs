using UnityEngine;
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
    public TextMeshProUGUI Energy;
    public Slider EnergySlider;
    public GameObject overFlowEn;
    public TextMeshProUGUI silverDrachs;
    public TextMeshProUGUI goldDrachs;
    public GameObject physicalForm;
    public GameObject spiritForm;
    public GameObject gardenUI;
    public GameObject coinGlow;
    public GameObject LandFX;
    public CanvasGroup WitchDisplay;
    public TextMeshProUGUI DisplayText;

    public Image LunarPhaseHolder;
    public Sprite[] LunarPhase;
    public Slider xpSlider;
    public TextMeshProUGUI xpText;
    
    public CanvasGroup curDominion;

    public GameObject DeathReason;
    public Text deathDesc;
    public Text deathblessing;

    [Space]
    [SerializeField] private UIMarkerPointer m_PlayerPositionPointer;
    //[SerializeField] private GameObject m_SilverGlimmer;
    //[SerializeField] private GameObject m_GoldGlimmer;
 
    private bool isDay = true;
    private bool cancheck = true;

    private bool m_IsPhysicalForm = true;
    private float m_LastSilverValue;
    private float m_LastGoldValue;

    private int m_CurrenciesTweenId;
    
    void Awake()
    {
        Instance = this;

        if (spiritForm == null)
        {
            spiritForm = Instantiate(physicalForm);
            spiritForm.transform.SetParent(physicalForm.transform.parent);
            spiritForm.transform.position = physicalForm.transform.position;
            spiritForm.transform.localScale = physicalForm.transform.localScale;
            spiritForm.transform.rotation = physicalForm.transform.rotation;
        }

        physicalForm.SetActive(false);
        spiritForm.SetActive(false);
        
        Raincrow.GameEventResponses.CharacterDeathHandler.OnPlayerDeath += CharacterDeathHandler_OnPlayerDeath;
    }

    private void CharacterDeathHandler_OnPlayerDeath(Raincrow.GameEventResponses.CharacterDeathHandler.DeathEventData data)
    {
        if (FTFManager.InFTF)
            return;

        if (PlayerDataManager.playerData.energy > 0)
        {
            LeanTween.value(0, 0, 0.2f).setOnComplete(() => CharacterDeathHandler_OnPlayerDeath(data));
            return;
        }

        //killed by another player
        if (string.IsNullOrEmpty(data.spirit))
        {
            deathDesc.text = LocalizeLookUp.GetText("ui_response_witch").Replace("{{Name}}", data.character).Replace("{{Witch Type}}", Utilities.GetSchool(data.degree));
        }
        //killed by spirit
        else
        {
            //spirit was wild
            if (string.IsNullOrEmpty(data.character))
            {
                deathDesc.text = LocalizeLookUp.GetText("ui_response_spirit_wild")
                    .Replace("{{Spirit Name}}", LocalizeLookUp.GetSpiritName(data.spirit));
            }
            //spirit had an owner
            else
            {
                deathDesc.text = LocalizeLookUp.GetText("ui_response_spirit")
                    .Replace("{{Spirit}}", LocalizeLookUp.GetSpiritName(data.spirit))
                    .Replace("{{Name}}", data.character);
            }
        }

        deathblessing.text = LocalizeLookUp.GetText("blessing_time")
            .Replace("{{Hours}}", ((int)BlessingManager.TimeUntilNextBlessing().TotalHours).ToString());// "Savannah's next blessing will come in " + hours + " hours or you can ask for a fellow witch to revive you.";

        LeanTween.value(0, 0, 0.5f).setOnComplete(() => DeathReason.SetActive(true));
    }

    private void OnDestroy()
    {
        Raincrow.GameEventResponses.CharacterDeathHandler.OnPlayerDeath -= CharacterDeathHandler_OnPlayerDeath;
    }

    private IEnumerator Start()
    {
        while (PlayerDataManager.playerData == null)
            yield return null;

        while (PlayerManager.marker == null)
            yield return null;
        

        PlayerManager.onFinishFlight += CheckPhysicalForm;

        m_PlayerPositionPointer.SetTarget(PlayerManager.witchMarker);

        m_LastGoldValue = PlayerDataManager.playerData.gold;
        m_LastSilverValue = PlayerDataManager.playerData.silver;

        Level.text = PlayerDataManager.playerData.level.ToString();        
        SetupAlignmentPhase();
        setupXP();
        SetupEnergy();
        UpdateDrachs();
    }

    // ___________________________________________ Main Player UI ________________________________________________________________________________________________

    public void SetupWitchDisplayText(int number)
    {
        if (number != 0)
        {
            LeanTween.alphaCanvas(WitchDisplay, 1f, 1f).setEaseInCubic();
            DisplayText.text = LocalizeLookUp.GetText("witch_display").Replace("{amount}", number.ToString());
        }
        else
        {
            LeanTween.alphaCanvas(WitchDisplay, 0f, 1f).setEaseOutCubic();
        }

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
        //if (pData.baseEnergy >= pData.energy)
        //{
        var fEnergy = (float)pData.energy;
        var energyLeftText = Energy.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        var energyRightText = Energy.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        var iEnergy = (float)Int32.Parse(energyLeftText.text);
        var fMax = pData.baseEnergy;
        energyRightText.text = fMax.ToString();
        LeanTween.value(iEnergy, fEnergy, 1f).setOnUpdate((float f) =>
        {
            f = (int)f;
            energyLeftText.text = f.ToString();
            if (pData.baseEnergy >= pData.energy)
            {
                EnergySlider.maxValue = fMax;
                EnergySlider.value = f;
                overFlowEn.SetActive(false);
            }
            else
            {
                EnergySlider.maxValue = fMax;
                EnergySlider.value = fMax;
                overFlowEn.SetActive(true);
            }
        }).setEaseInOutQuad();

        //     Energy.text = pData.energy.ToString() + "/" + pData.baseEnergy;
        //     EnergySlider.maxValue = pData.baseEnergy;
        //     EnergySlider.value = pData.energy;
        //     overFlowEn.SetActive(false);

        //  }
        //  else
        //  {
        //      overFlowEn.SetActive(true);
        //      EnergySlider.maxValue = pData.baseEnergy;
        //      EnergySlider.value = pData.baseEnergy;
        //      Energy.text = "<b>" + pData.energy.ToString() + "</b>/" + pData.baseEnergy;
        //  }



    }

    public void setupXP()
    {
        xpSlider.maxValue = PlayerDataManager.playerData.xpToLevelUp;
        xpSlider.value = PlayerDataManager.playerData.xp;
        xpText.text = PlayerDataManager.playerData.xp.ToString() + "/" + PlayerDataManager.playerData.xpToLevelUp.ToString();
    }

    public void CloseDeathMessage()
    {
        DeathReason.SetActive(false);
        LowEnergyPopup.Show();
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
        setupXP();
    }

    public void playerDegreeChanged()
    {
        LineRendererBasedDome.Instance.SetupDome();
        PlayerManager.Instance.AddAttackRing();
        SetupAlignmentPhase();
    }

    public void UpdateDrachs(bool updateStore = true)
    {
        LeanTween.cancel(m_CurrenciesTweenId);

        float goldStart = m_LastGoldValue;
        float silverStart = m_LastSilverValue;
        float goldTarget = PlayerDataManager.playerData.gold;
        float silverTarget = PlayerDataManager.playerData.silver;

        m_CurrenciesTweenId = LeanTween.value(0, 1, 1)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_LastSilverValue = Mathf.Lerp(silverStart, silverTarget, t);
                m_LastGoldValue = Mathf.Lerp(goldStart, goldTarget, t);
                silverDrachs.text = ((int)m_LastSilverValue).ToString();
                goldDrachs.text = ((int)m_LastGoldValue).ToString();
            })
            .setOnComplete(() =>
            {
                m_LastGoldValue = PlayerDataManager.playerData.gold;
                m_LastSilverValue = PlayerDataManager.playerData.silver;

                silverDrachs.text = ((int)m_LastSilverValue).ToString();
                goldDrachs.text = ((int)m_LastGoldValue).ToString();
            })
            .uniqueId;
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
    public void ShowGardenUI()
    {
        LeanTween.value(0f, 1f, 1.5f).setOnComplete(() =>
        {

            gardenUI.SetActive(true);
        });
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

    //public void Revived()
    //{
    //    Hide(EnergyElixir, true, 4);
    //}

    //public void ShowElixirOnBuy()
    //{
    //    // if (PlayerDataManager.playerData.state == "vulnerable")
    //    // {
    //    //     ShowElixirVulnerable(false);
    //    // }
    //    // else
    //    // {
    //    //     ShowElixirVulnerable(true);
    //    // }
    //}

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
}

