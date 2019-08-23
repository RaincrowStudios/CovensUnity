using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Raincrow;
using TMPro;

public class WitchSchoolManager : MonoBehaviour
{
    private static WitchSchoolManager m_Instance;
    public static string[] witchVideos = new string[0];

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [SerializeField] private RectTransform m_Window;
    [SerializeField] private GameObject m_ItemPrefab;
    [SerializeField] private LayoutGroup m_ItemContainer;

    [SerializeField] private MediaPlayerCtrl m_VideoPlayer;
    [SerializeField] private TextMeshProUGUI m_VideoTitle;

    private int m_TweenId;

    public static void Open()
    {
        //if (m_Instance != null)
        //{
        //    m_Instance.Show();
        //}
        //else
        //{
        //    LoadingOverlay.Show();
        //    SceneManager.LoadSceneAsync(SceneManager.Scene.WITCH_SCHOOL, UnityEngine.SceneManagement.LoadSceneMode.Additive,
        //        (progress) => { },
        //        () =>
        //        {
        //            LoadingOverlay.Hide();
        //            m_Instance.Show();
        //        });
        //}
    }

    void Awake()
    {
        m_Instance = this;

        foreach (var item in witchVideos)
        {
            var g = Instantiate(m_ItemPrefab, m_ItemContainer.transform);
            g.GetComponent<witchSchoolData>().Setup(item);
        }
    }

    private void Show()
    {
        //m_Canvas.enabled = false;
        //m_InputRaycaster.enabled = false;

        //m_TweenId = LeanTween.value(0, 1)

        //m_Window.transform.localScale = Vector3.zero;
        //CG.alpha = 0;
        //LeanTween.alphaCanvas(CG, 1, .6f);
        //LeanTween.cancel(m_TweenId);
        //m_TweenId = LeanTween.scale(m_Window, Vector3.one, .6f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
        //{
        //    UIStateManager.Instance.CallWindowChanged(false);
        //    MapsAPI.Instance.HideMap(true);
        //})
        //.uniqueId;
        ////anim.SetBool ("open", true);
    }

    public void Close()
    {
        //MapsAPI.Instance.HideMap(false);

        ////	anim.SetBool ("open", false);
        ////Disable (anim.gameObject, 1.5f);
        //LeanTween.alphaCanvas(CG, 0, .4f);
        //LeanTween.cancel(m_TweenId);
        //m_TweenId = LeanTween.scale(m_Window, Vector3.zero, .4f).setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
        //{
        //    UIStateManager.Instance.CallWindowChanged(true);
        //    m_Window.SetActive(false);
        //})
        //.uniqueId;
    }

    public void playVideo(string URL, string title)
    {
        //videoTitle.text = title;
        //StartCoroutine(FadeInFocus(CG));
        //player.Load(URL);
        //PlayerDataManager.Instance.GetComponent<AudioSource>().Pause();
    }

    public void CloseVideo()
    {
        //player.UnLoad();
        //StartCoroutine(FadeOutFocus(CG));
        //PlayerDataManager.Instance.GetComponent<AudioSource>().UnPause();
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
