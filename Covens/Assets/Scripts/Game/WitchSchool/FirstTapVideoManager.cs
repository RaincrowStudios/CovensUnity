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
    public CanvasGroup videoContainer;
    public MediaPlayerCtrl player;
    public Text videoTitle;
    public string ID = "";

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

        ID = id;
        title.text = LocalizeLookUp.GetText(id + "_title").ToUpper();
        desc.text = LocalizeLookUp.GetText(id + "_desc");
        StartCoroutine(getPic(id));
        StartCoroutine(FadeInFocus(CG));
    }

    IEnumerator getPic(string id)
    {
        WWW www = new WWW(DownloadAssetBundle.baseURL + "witch-school-new/thumbs/" + id + ".png");
        yield return www;
        thumb.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }

    public void OnSkip()
    {
        StartCoroutine(FadeOutFocus(CG));

        if (ID == "fly")
        {
            IsFirstFlight = false;
            MapFlightTransition.Instance.FlyOut();
        }
        else if (ID == "summoning")
        {
            IsFirstSummon = false;
            SummoningController.Instance.Open();
        }
        else if (ID == "spellcasting")
        {
            IsFirstCast = false;
        }

    }

    public void playVideo()
    {
        videoTitle.text = title.text;
        StartCoroutine(FadeInFocus(videoContainer));
        player.Load(DownloadAssetBundle.baseURL + "witch-school-new/videos/" + ID + ".mp4");
    }

    public void OnCloseVideo()
    {
        player.UnLoad();
        StartCoroutine(FadeOutFocus(videoContainer));

        OnSkip();
    }

    IEnumerator FadeOutFocus(CanvasGroup cg)
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * 2.8f;
            cg.alpha = Mathf.SmoothStep(1, 0, t);
            yield return 0;
        }
        cg.gameObject.SetActive(false);
    }

    IEnumerator FadeInFocus(CanvasGroup cg, float delay = 0)
    {

        cg.gameObject.SetActive(true);
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * 2;
            cg.alpha = Mathf.SmoothStep(0, 1, t);
            yield return 0;
        }

    }

    public void Disable(GameObject g, float delay = 1.5f)
    {
        StartCoroutine(disableObject(g, delay));
    }

    IEnumerator disableObject(GameObject g, float delay)
    {
        yield return new WaitForSeconds(delay);
        g.SetActive(false);
    }
}

