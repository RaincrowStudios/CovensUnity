using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class LocationPOPInfo : UIInfoPanel
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Content;
    [SerializeField] private CanvasGroup m_TimeCG;
    [SerializeField] private TextMeshProUGUI m_TimeSubtitle;
    [SerializeField] private TextMeshProUGUI m_TimeTitle;
    [SerializeField] private Image m_locationImage;
    [SerializeField] private Color m_NotEnoughSilver;
    [SerializeField] private Color m_EnoughSilver;
    [SerializeField] private Image m_Enter;
    [SerializeField] private GameObject m_Locked;
    [SerializeField] private Image m_Progress;
    [SerializeField] private CanvasGroup m_FadeOut;
    [SerializeField] private Button m_CloseBtn;
    [SerializeField] private GameObject m_EnterAnimation;
    [SerializeField] private GameObject m_PlayerJoined;
    [SerializeField] private TextMeshProUGUI m_PlayerJoinedTitle;
    [SerializeField] private TextMeshProUGUI m_PlayerJoinedSubtitle;
    [SerializeField] private Image m_PlayerJoinedColor;

    public Sprite disneyPark;
    public Sprite Hollywood;
    public Sprite Winchester;

    Dictionary<string, Sprite> imgs = new Dictionary<string, Sprite>();

    private Button m_EnterBtn;
    private LocationViewData m_LocationViewData;
    private static LocationPOPInfo m_Instance;
    private bool m_HasEnteredPOP = false;

    public static LocationPOPInfo Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<LocationPOPInfo>("LocationPOPInfo"));
            return m_Instance;

        }
    }

    public static bool isShowing
    {
        get
        {
            if (m_Instance == null) return false;
            else return m_Instance.IsShowing;
        }
    }

    protected override void Awake()
    {
        m_Instance = this;
        imgs.Add("5d642429d1bdb244e91395b0", disneyPark);
        imgs.Add("5d64251fd1bdb244e913ae7f", Winchester);
        imgs.Add("5d642429d1bdb244e91395b0", Hollywood);
        m_EnterBtn = m_Enter.GetComponent<Button>();
        m_CloseBtn.onClick.AddListener(Close);
        base.Awake();
    }

    public void Show(LocationViewData data)
    {
        m_LocationViewData = data;
        LocationIslandController.OnWitchEnter += AddWitch;
        LocationIslandController.OnWitchExit += RemoveWitch;
        base.Show();
        ShowUI();
    }
    private void ShowUI()
    {
        SoundManagerOneShot.Instance.SummonRiser();
        try
        {
            m_locationImage.sprite = imgs[m_LocationViewData._id];

        }
        catch (System.Exception)
        {
        }
        m_HasEnteredPOP = false;
        m_EnterAnimation.SetActive(false);
        m_Title.text = m_LocationViewData.name;
        FadeOutUITimer();
        if (m_LocationViewData.isOpen)
        {
            m_Locked.SetActive(false);
            m_Content.text = "Place of power is open and accepting players, pay 1 gold drach to go in";
            m_Enter.gameObject.SetActive(true);

            m_Enter.color = m_EnoughSilver;
            m_EnterBtn.onClick.RemoveAllListeners();
            InitiateTimeUI();
            // UpdateEnterTimer();
            // if (PlayerDataManager.playerData.gold >= 1)
            m_EnterBtn.onClick.AddListener(() => LocationIslandController.EnterPOP(m_LocationViewData._id, OnEnterPOP));
            // else
            // {
            //     m_Enter.color = m_NotEnoughSilver;
            //     //handle taking to store
            // }

        }
        else
        {
            m_Enter.gameObject.SetActive(false);
            m_Locked.SetActive(true);
            if (m_LocationViewData.isActive)
            {
                m_TimeCG.gameObject.SetActive(false);
                m_Content.text = "There is a battle going on in this place of power, check back later";
            }
            else
            {
                InitiateTimeUI();
                m_Content.text = "This place is under cooldown, check back later";
                //  UpdateCooldownTimer();
            }
        }
    }

    private async void InitiateTimeUI()
    {
        await Task.Delay(3000);
        m_TimeCG.alpha = 0;
        m_TimeCG.gameObject.SetActive(true);
        m_FadeOut.alpha = 0;
        m_FadeOut.gameObject.SetActive(true);
        m_Progress.gameObject.SetActive(true);
        LeanTween.value(0, 1, .8f).setOnUpdate((float v) =>
        {
            m_FadeOut.alpha = v;
            m_TimeCG.alpha = v;
        });
    }

    private void FadeOutUITimer()
    {
        if (m_FadeOut.alpha == 1)
        {
            LeanTween.value(1, 0, .8f).setOnUpdate((float v) =>
            {
                m_FadeOut.alpha = v;
                m_TimeCG.alpha = v;
            });
            m_TimeCG.gameObject.SetActive(false);
            m_FadeOut.gameObject.SetActive(false);
            m_Progress.gameObject.SetActive(false);
        }
    }

    private async void UpdateCooldownTimer()
    {
        await Task.Delay(1000);

        if (!Application.isPlaying || !gameObject.activeInHierarchy) return;

        var tStamp = GetTime(m_LocationViewData.coolDownEndOn);
        var tSec = GetSeconds(m_LocationViewData.coolDownEndOn);
        if (tSec == 0)
        {
            RefreshViewData();
        }
        else
        {
            m_TimeTitle.text = tStamp.timeStamp;
            m_TimeSubtitle.text = tStamp.timeType;
            m_Progress.fillAmount = MapUtils.scale(0, 1, 0, m_LocationViewData.coolDownTimeWindow, tSec);

            UpdateCooldownTimer();
        }
    }

    private async void UpdateEnterTimer()
    {

        await Task.Delay(1000);

        if (!Application.isPlaying || !gameObject.activeInHierarchy) return;

        var tStamp = GetSeconds(m_LocationViewData.battleBeginsOn);

        m_TimeTitle.text = tStamp.ToString();
        if (!m_HasEnteredPOP)
            m_TimeSubtitle.text = "seconds";

        m_Progress.fillAmount = MapUtils.scale(0, 1, 0, m_LocationViewData.openTimeWindow, tStamp);

        if (tStamp < 1)
        {
            RefreshViewData();
            return;
        }


        UpdateEnterTimer();
    }

    private void RefreshViewData()
    {
        APIManager.Instance.Get($"place-of-power/view/{m_LocationViewData._id}", (x, y) =>
        {
            if (y == 200)
            {
                m_LocationViewData = JsonConvert.DeserializeObject<LocationViewData>(x);
                ShowUI();
            }
            else
            {
                Debug.LogError(x);
            }
        });
    }

    private async void OnEnterPOP(LocationData locationData)
    {
        if (locationData != null)
        {
            SoundManagerOneShot.Instance.IngredientAdded();
            m_HasEnteredPOP = true;
            UpdateWitchCount();
            m_EnterAnimation.SetActive(true);
            m_Enter.gameObject.SetActive(false);
            m_Locked.SetActive(true);
            m_Content.text = $"Prepare to battle for {m_LocationViewData.name}. Defeat the Guardian. Last Witch Standing wins.";
            await Task.Delay(2200);
            m_EnterAnimation.SetActive(false);
        }
    }

    private void UpdateWitchCount()
    {
        m_TimeSubtitle.text = $"{LocationIslandController.locationData.currentOccupants} Witches Joined";
        SoundManagerOneShot.Instance.PlayWooshShort();
    }

    private void SetupWitchAnimation(WitchToken token, bool Add)
    {
        this.CancelInvoke("DisablePlayerJoined");
        if (m_PlayerJoined.activeInHierarchy)
        {
            DisablePlayerJoined();
        }
        UpdateWitchCount();
        m_PlayerJoined.SetActive(true);
        m_PlayerJoinedTitle.text = $"<color=white>{token.displayName}</color> {(Add ? "Joined" : "Left")}";
        m_PlayerJoinedSubtitle.text = $"Level: <color=white>{token.level}</color> | <color=white> {Utilities.GetDegree(token.degree)}";
        if (token.degree > 0)
            m_PlayerJoinedColor.color = Utilities.Orange;
        else if (token.degree < 0)
            m_PlayerJoinedColor.color = Utilities.Purple;
        else
            m_PlayerJoinedColor.color = Utilities.Blue;
        Invoke("DisablePlayerJoined", 2.1f);
    }

    private void AddWitch(WitchToken token)
    {
        SetupWitchAnimation(token, true);
    }

    private void RemoveWitch(WitchToken token)
    {
        SetupWitchAnimation(token, false);
    }

    private void DisablePlayerJoined()
    {
        m_PlayerJoined.SetActive(false);
    }

    public override void Close()
    {
        UnsubscribeWitchEvents();
        base.Close();
    }

    private void UnsubscribeWitchEvents()
    {
        LocationIslandController.OnWitchEnter -= AddWitch;
        LocationIslandController.OnWitchExit -= RemoveWitch;
    }

    private static (string timeStamp, string timeType) GetTime(double javaTimeStamp)
    {
        TimeSpan timeSpan = GetSpan(javaTimeStamp);

        if (timeSpan.Hours >= 1)
            return (timeSpan.Hours.ToString(), timeSpan.Hours > 1 ? "hours" : "hour");
        else if (timeSpan.Minutes >= 1)
            return (timeSpan.Minutes.ToString(), timeSpan.Minutes > 1 ? "minutes" : "minute");
        else
            return (timeSpan.Seconds.ToString(), timeSpan.Seconds > 1 ? "seconds" : "second");

    }

    private static int GetSeconds(double javaTimeStamp)
    {
        TimeSpan timeSpan = GetSpan(javaTimeStamp);
        return (int)timeSpan.TotalSeconds;
    }

    private static TimeSpan GetSpan(double javaTimeStamp)
    {
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToUniversalTime();
        var timeSpan = dtDateTime.Subtract(DateTime.UtcNow);
        return timeSpan;
    }

}
