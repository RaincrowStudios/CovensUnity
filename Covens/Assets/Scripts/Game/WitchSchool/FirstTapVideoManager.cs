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
    // Use this for initialization
    public MediaPlayerCtrl player;
    public Text videoTitle;
    public string ID = "";
    void Awake()
    {
        Instance = this;
    }

    public bool CheckKyteler()
    {
        if (LoginUIManager.isInFTF)
            return true;

        if (!PlayerDataManager.playerData.firsts.kyteler)
        {
            SetupVideo("kyteler");
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool CheckSummon()
    {
        Debug.Log("Checking summon");
        if (LoginUIManager.isInFTF)
            return true;
        if (!PlayerDataManager.playerData.firsts.portalSummon)
        {
            Debug.Log("Showing the video");
            SetupVideo("summoning");
            return false;
        }
        else
        {
            Debug.Log("summon true");
            return true;
        }
    }

    public bool CheckSpellCasting()
    {
        if (LoginUIManager.isInFTF)
            return true;
        if (!PlayerDataManager.playerData.firsts.cast)
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
        if (!PlayerDataManager.playerData.firsts.flight)
        {
            SetupVideo("fly");
            return false;
        }
        else
            return true;
    }

    void SetupVideo(string id)
    {
        if (WitchSchoolManager.witchVideos.ContainsKey(id) == false)
            return;

        LocalizeData ld = WitchSchoolManager.witchVideos[id];
        ID = id;
        title.text = ld.title.ToUpper();
        desc.text = ld.description;
        StartCoroutine(getPic(id));
        StartCoroutine(FadeInFocus(CG));
    }

    IEnumerator getPic(string id)
    {
        WWW www = new WWW(DownloadAssetBundle.baseURL + "witch-school/thumb/" + id + ".png");
        yield return www;
        thumb.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }

    public void OnSkip()
    {
        StartCoroutine(FadeOutFocus(CG));

        if (ID == "fly")
        {
            PlayerDataManager.playerData.firsts.flight = true;
            //PlayerManager.Instance.Fly();
        }
        else if (ID == "summoning")
        {
            PlayerDataManager.playerData.firsts.portalSummon = true;
            SummoningController.Instance.Open();
        }
        else if (ID == "spellcasting")
        {
            PlayerDataManager.playerData.firsts.cast = true;
            //ShowSelectionCard.Instance.Attack();
        }

    }

    public void playVideo()
    {
        videoTitle.text = title.text;
        StartCoroutine(FadeInFocus(videoContainer));
        player.Load(DownloadAssetBundle.baseURL + "witch-school/videos/" + ID + ".mp4");
    }

    public void OnCloseVideo()
    {
        player.UnLoad();
        StartCoroutine(FadeOutFocus(videoContainer));
        StartCoroutine(FadeOutFocus(CG));
        if (ID == "fly")
        {
            PlayerDataManager.playerData.firsts.flight = true;
            //PlayerManager.Instance.Fly();
        }
        else if (ID == "summoning")
        {
            PlayerDataManager.playerData.firsts.portalSummon = true;
            SummoningController.Instance.Open();
        }
        else if (ID == "spellcasting")
        {
            PlayerDataManager.playerData.firsts.cast = true;
            //ShowSelectionCard.Instance.Attack();
        }
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

