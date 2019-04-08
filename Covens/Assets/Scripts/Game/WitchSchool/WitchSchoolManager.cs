using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WitchSchoolManager : MonoBehaviour
{
    public static WitchSchoolManager Instance { get; set; }
    public static Dictionary<string, LocalizeData> witchVideos = new Dictionary<string, LocalizeData>();

    public GameObject videoItem;
    public Transform container;
    public CanvasGroup CG;
    public GameObject witchSchool;
    public MediaPlayerCtrl player;
    public Text videoTitle;
    void Awake()
    {
        Instance = this;
    }

    public void Open()
    {
        witchSchool.SetActive(true);
        witchSchool.transform.localScale = Vector3.zero;
        CG.alpha = 0;
        LeanTween.alphaCanvas(CG, 1, .6f);
        LeanTween.scale(witchSchool, Vector3.one, .6f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
        {
            UIStateManager.Instance.CallWindowChanged(false);
            MapController.Instance.SetVisible(false);
        });
        //anim.SetBool ("open", true);
    }

    void Start()
    {
        foreach (var item in witchVideos)
        {
            var g = Utilities.Instantiate(videoItem, container);
            g.GetComponent<witchSchoolData>().Setup(item.Key, item.Value);
        }
    }

    public void Close()
    {
        //	anim.SetBool ("open", false);
        //Disable (anim.gameObject, 1.5f);
        LeanTween.alphaCanvas(CG, 0, .4f);
        LeanTween.scale(witchSchool, Vector3.zero, .4f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
        {
            UIStateManager.Instance.CallWindowChanged(true);
            MapController.Instance.SetVisible(true);
            witchSchool.SetActive(false);

        });
    }

    public void playVideo(string URL, string title)
    {
        videoTitle.text = title;
        StartCoroutine(FadeInFocus(CG));
        player.Load(URL);
    }

    public void CloseVideo()
    {
        player.UnLoad();
        StartCoroutine(FadeOutFocus(CG));

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
