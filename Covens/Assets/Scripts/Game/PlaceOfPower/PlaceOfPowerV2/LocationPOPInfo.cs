using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Networking;

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
    [SerializeField] private GameObject m_FTFEnter;
    [SerializeField] private Button m_HelpBtn;

    private Dictionary<string, Sprite> m_LocationImagesCache = new Dictionary<string, Sprite>();

    [SerializeField] private Button m_EnterBtn;
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
            else return m_Instance.m_IsShowing;
        }
    }

    protected override void Awake()
    {
        m_Instance = this;
        m_CloseBtn.onClick.AddListener(Close);
        m_HelpBtn.onClick.AddListener(() =>
        {
            LocationTutorial.Instance.Open();
        });
        base.Awake();
    }

    public void Show(LocationViewData data)
    {
        m_FTFEnter.SetActive(false);
        StartCoroutine(DownloadThumb(data.name));
        m_LocationViewData = data;
        LocationIslandController.OnWitchEnter += AddWitch;
        LocationIslandController.OnWitchExit += RemoveWitch;
        LocationIslandController.popName = data.name;
        m_TimeCG.alpha = 0;
        m_TimeCG.gameObject.SetActive(false);
        m_FadeOut.alpha = 0;
        m_FadeOut.gameObject.SetActive(false);
        m_Progress.gameObject.SetActive(false);
        m_HasEnteredPOP = false;
        m_CloseBtn.gameObject.SetActive(true);
        base.Show();
        ShowUI();
    }

    public void FTFOpen()
    {
        //m_Enter.gameObject.SetActive(false);
        m_Locked.SetActive(false);
        m_TimeCG.gameObject.SetActive(false);
        m_Content.text = LocalizeLookUp.GetText("pop_ongoing");// + ", " + LocalizeLookUp.GetText("generic_please_wait");// "There is a battle going on in this place of power, check back later";

        m_Enter.color = m_EnoughSilver;
        m_EnterBtn.onClick.RemoveAllListeners();
        m_Enter.gameObject.SetActive(false);
        m_FTFEnter.SetActive(true);
    }

    private void ShowUI(bool disableInit = false)
    {
        SoundManagerOneShot.Instance.SummonRiser();

        m_EnterAnimation.SetActive(false);
        m_Title.text = m_LocationViewData.name;
        if (m_LocationViewData.isOpen)
        {
            m_Enter.color = m_EnoughSilver;
            m_EnterBtn.onClick.RemoveAllListeners();
            if (compareTime(m_LocationViewData.battleBeginsOn))
            {
                if (!disableInit)
                    StartCoroutine(InitiateTimeUI());
                StartCoroutine(UpdateEnterTimer());
                m_Locked.SetActive(false);
                m_Content.text = LocalizeLookUp.GetText("pop_open_pay").Replace("{amount}", "1 " + LocalizeLookUp.GetText("store_gold")); //1 gold drach entry             // "Place of power is open and accepting players, pay 1 gold drach to go in";
                m_Enter.gameObject.SetActive(true);
            }
            else
            {
                m_Locked.SetActive(true);
                m_Content.text = LocalizeLookUp.GetText("pop_unavailable");// "Place of power is unavailable";
                m_TimeTitle.text = "--";
                m_Enter.gameObject.SetActive(false);
                m_TimeSubtitle.text = LocalizeLookUp.GetText("lt_time_seconds");// "seconds";
                Debug.LogError("Wrong battle begins on timer for pop " + m_LocationViewData._id);
            }

            if (PlayerDataManager.playerData.gold >= 1)
                m_EnterBtn.onClick.AddListener(() => LocationIslandController.EnterPOP(m_LocationViewData._id, OnEnterPOP));
            else
            {
                m_Enter.color = m_NotEnoughSilver;
                m_EnterBtn.onClick.AddListener(() => UIGlobalPopup.ShowPopUp(() => { }, LocalizeLookUp.GetText("store_not_enough_gold")));// "Not enough gold."));
                //handle taking to store
            }

        }
        else
        {
            m_Enter.gameObject.SetActive(false);
            m_Locked.SetActive(true);
            if (m_LocationViewData.isActive)
            {
                m_TimeCG.gameObject.SetActive(false);
                m_Content.text = LocalizeLookUp.GetText("pop_ongoing");// "There is a battle going on in this place of power, check back later";
            }
            else
            {
                StartCoroutine(InitiateTimeUI());
                m_Content.text = LocalizeLookUp.GetText("pop_cooldown_desc");// "This place is under cooldown, check back later";
                StartCoroutine(UpdateCooldownTimer());
            }
        }
    }

    IEnumerator InitiateTimeUI()
    {
        yield return new WaitForSeconds(3);
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

    IEnumerator UpdateCooldownTimer()
    {
        yield return new WaitForSeconds(1);

        if (!Application.isPlaying || !gameObject.activeInHierarchy) yield break;

        var cooldownEnd = m_LocationViewData.battleFinishedOn + (m_LocationViewData.coolDownTimeWindow * 1000);

        var tStamp = GetTime(cooldownEnd);
        var tSec = GetSeconds(cooldownEnd);
        if (tSec == 0)
        {
            LoadingOverlay.Show(LocalizeLookUp.GetText("loading"));// "Updating Place of power status...");
            yield return new WaitForSeconds(2);
            RefreshViewData();
        }
        else if (tSec > 0)
        {
            m_TimeTitle.text = tStamp.timeStamp;
            m_TimeSubtitle.text = tStamp.timeType;
            m_Progress.fillAmount = MapUtils.scale(0, 1, 0, m_LocationViewData.coolDownTimeWindow, tSec);
            StartCoroutine(UpdateCooldownTimer());
        }
        else
        {
            Debug.LogError("Enter Timer Value Negetive : " + tStamp);
        }
    }

    IEnumerator UpdateEnterTimer()
    {
        yield return new WaitForSeconds(1);
        //await Task.Delay(1000);

        if (!Application.isPlaying || !gameObject.activeInHierarchy) yield break;

        var tStamp = GetSeconds(m_LocationViewData.battleBeginsOn);

        m_TimeTitle.text = tStamp.ToString();
        m_Progress.fillAmount = MapUtils.scale(0, 1, 0, m_LocationViewData.openTimeWindow, tStamp);

        if (!m_HasEnteredPOP)
        {
            m_TimeSubtitle.text = LocalizeLookUp.GetText("lt_time_seconds");//"seconds";
        }

        if (tStamp == 0 && !m_HasEnteredPOP)
        {
            LoadingOverlay.Show(LocalizeLookUp.GetText("loading"));//"Updating Place of power status...");
            yield return new WaitForSeconds(2);
            RefreshViewData();
        }
        else if (tStamp > 0)
        {
            StartCoroutine(UpdateEnterTimer());
        }
        else
        {
            Debug.Log("Enter Timer Value Negetive : " + tStamp);
        }

    }

    private void RefreshViewData()
    {
        APIManager.Instance.Get($"place-of-power/view/{m_LocationViewData._id}", (x, y) =>
        {
            if (y == 200)
            {
                m_LocationViewData = JsonConvert.DeserializeObject<LocationViewData>(x);
                LoadingOverlay.Hide();
                Debug.Log(x);
                ShowUI(true);
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
            PlayerDataManager.playerData.gold--;
            PlayerManagerUI.Instance.UpdateDrachs();
            m_CloseBtn.gameObject.SetActive(false);
            SoundManagerOneShot.Instance.IngredientAdded();
            m_HasEnteredPOP = true;
            UpdateWitchCount();
            m_EnterAnimation.SetActive(true);
            m_Enter.gameObject.SetActive(false);
            m_Locked.SetActive(true);
            m_Content.text = LocalizeLookUp.GetText("pop_prepare").Replace("{locationName}", m_LocationViewData.name); // $"Prepare to battle for {m_LocationViewData.name}. Defeat the Guardian. Last Witch Standing wins.";
            await Task.Delay(2200);
            m_EnterAnimation.SetActive(false);
        }
    }

    private void UpdateWitchCount()
    {
        m_TimeSubtitle.text = LocalizeLookUp.GetText("pop_joined").Replace("{amount}", LocationIslandController.locationData.currentOccupants.ToString()); // $"{LocationIslandController.locationData.currentOccupants} Witches Joined";
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
        m_PlayerJoinedTitle.text = $"<color=white>{token.displayName}</color> {(Add ? LocalizeLookUp.GetText("generic_joined") : LocalizeLookUp.GetText("generic_left"))}";
        m_PlayerJoinedSubtitle.text = $"{LocalizeLookUp.GetText("lt_level")} <color=white>{token.level}</color> | <color=white> {Utilities.GetDegree(token.degree)}";
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
        StopAllCoroutines();
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
            return (timeSpan.Hours.ToString(), timeSpan.Hours > 1 ? LocalizeLookUp.GetText("lt_time_hours") : LocalizeLookUp.GetText("lt_time_hour"));
        else if (timeSpan.Minutes >= 1)
            return (timeSpan.Minutes.ToString(), timeSpan.Minutes > 1 ? LocalizeLookUp.GetText("lt_time_minutes") : LocalizeLookUp.GetText("lt_time_minute"));
        else
            return (timeSpan.Seconds.ToString(), timeSpan.Seconds > 1 ? LocalizeLookUp.GetText("lt_time_seconds") : LocalizeLookUp.GetText("lt_time_seconds"));

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

    private Boolean compareTime(double timeStamp)
    {

        double current = (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Debug.Log(current);
        Debug.Log(timeStamp);
        return timeStamp > current;
    }

    IEnumerator DownloadThumb(string id)
    {
        if (m_LocationImagesCache.ContainsKey(id))
        {
            m_locationImage.sprite = m_LocationImagesCache[id];
        }
        else
        {
            m_locationImage.color = new Color(1, 1, 1, 0);
            string url = DownloadAssetBundle.baseURL + "pop-circle/" + id + ".png";
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                yield return new WaitForSeconds(1f);
                StartCoroutine(DownloadThumb(id));
            }
            else
            {
                if (www.isHttpError)
                {
                    Debug.LogError($"failed to download \"{url}\"");
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(www);
                    if (texture != null)
                    {
                        m_LocationImagesCache[id] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                        m_locationImage.color = Color.white;
                        m_locationImage.sprite = m_LocationImagesCache[id];
                    }
                }
            }
        }
    }

}
