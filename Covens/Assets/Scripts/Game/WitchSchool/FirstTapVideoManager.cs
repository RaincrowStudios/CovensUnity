using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FirstTapVideoManager : MonoBehaviour
{
    public static FirstTapVideoManager Instance;
    
    public Text title;
    public Text desc;
    public Image thumb;
    public CanvasGroup CG;

    [SerializeField] private Button m_ThumbnailButton;
    [SerializeField] private Button m_WatchButton;
    [SerializeField] private Button m_SkipButton;

    private string m_Id = "";
    private int m_TweenId;

    public static bool IsFirstFlight
    {
        get => PlayerPrefs.GetInt("first.flight." + PlayerDataManager.playerData.instance, 1) == 1;
        set => PlayerPrefs.SetInt("first.flight." + PlayerDataManager.playerData.instance, value ? 1 : 0);
    }

    //public static bool IsFirstSummon
    //{
    //    get => PlayerDataManager.playerData != null && PlayerDataManager.playerData.firsts != null && PlayerDataManager.playerData.firsts.Contains("summon") == false;
    //    set
    //    {
    //        if (PlayerDataManager.playerData != null && PlayerDataManager.playerData.firsts != null)
    //            PlayerDataManager.playerData.firsts.Add("summon");
    //    }
    //}

    //public static bool IsFirstCast
    //{
    //    get => PlayerDataManager.playerData != null && PlayerDataManager.playerData.firsts != null && PlayerDataManager.playerData.firsts.Contains("cast") == false;
    //    set
    //    {
    //        if (PlayerDataManager.playerData != null && PlayerDataManager.playerData.firsts != null)
    //            PlayerDataManager.playerData.firsts.Add("cast");
    //    }
    //}

    public static bool IsFirstSummon
    {
        get => PlayerPrefs.GetInt("first.summon." + PlayerDataManager.playerData.instance, 1) == 1;
        set => PlayerPrefs.SetInt("first.summon." + PlayerDataManager.playerData.instance, value ? 1 : 0);
    }

    public static bool IsFirstCast
    {
        get => PlayerPrefs.GetInt("first.cast." + PlayerDataManager.playerData.instance, 1) == 1;
        set => PlayerPrefs.SetInt("first.cast." + PlayerDataManager.playerData.instance, value ? 1 : 0);
    }

    void Awake()
    {
        Instance = this;

        m_ThumbnailButton.onClick.AddListener(PlayVideo);
        m_WatchButton.onClick.AddListener(PlayVideo);
        m_SkipButton.onClick.AddListener(OnSkip);
    }
    
    public bool CheckSummon()
    {
        if (PlayerDataManager.IsFTF)
            return true;

        if (IsFirstSummon)
        {
            SetupVideo("summoning");
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool CheckSpellCasting()
    {
        if (PlayerDataManager.IsFTF)
            return true;

        if (IsFirstCast)
        {
            SetupVideo("spellcasting");
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool CheckFlight()
    {
        if (IsFirstFlight)
        {
            SetupVideo("fly");
            return false;
        }
        else
            return true;
    }

    void SetupVideo(string id)
    {
        if (WitchSchoolManager.witchVideos.Contains(id) == false)
            return;

        LeanTween.cancel(m_TweenId);

        m_Id = id;
        title.text = LocalizeLookUp.GetText(id + "_title").ToUpper();
        desc.text = LocalizeLookUp.GetText(id + "_desc");
        StartCoroutine(getPic(id));

        CG.gameObject.SetActive(true);
        m_TweenId = LeanTween.alphaCanvas(CG, 1f, 1f).setEaseOutCubic().uniqueId;
    }

    IEnumerator getPic(string id)
    {
        WWW www = new WWW(DownloadAssetBundle.baseURL + "witch-school-new/thumbs/" + id + ".png");
        yield return www;
        thumb.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }

    public void OnSkip()
    {
        LeanTween.cancel(m_TweenId);
        m_TweenId =  LeanTween.alphaCanvas(CG, 0f, 1f).setEaseOutCubic().setOnComplete(() => CG.gameObject.SetActive(false)).uniqueId;

        if (m_Id == "fly")
        {
            IsFirstFlight = false;
            MapFlightTransition.Instance.FlyOut();
        }
        else if (m_Id == "summoning")
        {
            IsFirstSummon = false;
            SummoningController.Instance.Open();
        }
        else if (m_Id == "spellcasting")
        {
            IsFirstCast = false;
        }
    }

    public void PlayVideo()
    {
        WitchSchoolPlayer.Open(title.text, DownloadAssetBundle.baseURL + "witch-school-new/videos/" + m_Id + ".mp4", null, OnSkip);
    }
}

