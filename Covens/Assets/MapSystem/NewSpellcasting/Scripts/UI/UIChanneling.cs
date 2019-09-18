using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Raincrow.GameEventResponses;

public class UIChanneling : UIInfoPanel
{
    [Header("Generic")]
    [SerializeField] private TextMeshProUGUI m_Title;

    [Header("Channeling")]
    [SerializeField] private CanvasGroup m_ChannelingCanvasGroup;
    [SerializeField] private Button m_StopChannelingBtn;

    [Header("Results")]
    [SerializeField] private CanvasGroup m_ResultsCanvasGroup;
    [SerializeField] private TextMeshProUGUI m_ResultsTitle;
    [SerializeField] private TextMeshProUGUI m_ResultsContent;
    [SerializeField] private Button m_ContinueButton;

    private static UIChanneling m_Instance;
    public static UIChanneling Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UIChanneling>("UIChanneling"));
            return m_Instance;
        }
    }

    public static bool isOpen
    {
        get
        {
            if (m_Instance == null)
                return false;
            else
                return m_Instance.m_IsShowing;
        }
    }

    private System.Action<Raincrow.GameEventResponses.SpellCastHandler.Result> m_OnClickContinue;
    private int m_ChannelingTweenId;
    private int m_ResultsTweenId;
    private int m_DelayTweenId;
    //private string m_ChannelInstance;
    SpellCastHandler.Result m_Results;

    protected override void Awake()
    {
        base.Awake();

        m_StopChannelingBtn.onClick.AddListener(OnClickStop);
        m_ContinueButton.onClick.AddListener(OnClickContinue);

        m_ChannelingCanvasGroup.alpha = 0;
        m_ResultsCanvasGroup.alpha = 0;
    }

    public void Show(System.Action<Raincrow.GameEventResponses.SpellCastHandler.Result> onClickContinue)
    {
        //m_ChannelInstance = null;
        //m_Results = null;
        m_OnClickContinue = onClickContinue;

        LeanTween.cancel(m_ChannelingTweenId);
        LeanTween.cancel(m_DelayTweenId);
        HideResults();

        m_ChannelingCanvasGroup.blocksRaycasts = true;
        m_ChannelingCanvasGroup.interactable = false;

        ////animate the channeling ui
        m_ChannelingTweenId = LeanTween.alphaCanvas(m_ChannelingCanvasGroup, 1f, 0.5f).uniqueId;

        base.Show();
    }

    public void SetInteractable(bool interactable)
    {
        LeanTween.cancel(m_DelayTweenId);

        m_DelayTweenId = LeanTween.value(0, 0, 0.1f).setOnComplete(() =>
        {
            m_ChannelingCanvasGroup.interactable = interactable;
            m_ResultsCanvasGroup.interactable = interactable;
        }).uniqueId;
    }
    
    private void HideChanneling()
    {
        LeanTween.cancel(m_DelayTweenId);
        LeanTween.cancel(m_ChannelingTweenId);

        m_ChannelingCanvasGroup.interactable = false;
        m_ChannelingCanvasGroup.blocksRaycasts = false;

        //animate the channeling ui
        m_ChannelingTweenId = LeanTween.alphaCanvas(m_ChannelingCanvasGroup, 0f, 0.5f).uniqueId;
    }

    public void ShowResults(Raincrow.GameEventResponses.SpellCastHandler.Result result, string error)
    {
        m_Results = result;
        LeanTween.cancel(m_ResultsTweenId);

        if (string.IsNullOrEmpty(error))
            m_ResultsContent.text = Newtonsoft.Json.JsonConvert.SerializeObject(result);
        else
            m_ResultsContent.text = "error: " + error;

        m_ResultsCanvasGroup.interactable = true;
        m_ResultsCanvasGroup.blocksRaycasts = true;

        m_ResultsTweenId = LeanTween.alphaCanvas(m_ResultsCanvasGroup, 1f, 0.5f).uniqueId;
        HideChanneling();
    }

    private void HideResults()
    {
        m_ResultsCanvasGroup.blocksRaycasts = false;
        m_ResultsCanvasGroup.interactable = false;

        LeanTween.cancel(m_ResultsTweenId);
        m_ResultsTweenId = LeanTween.alphaCanvas(m_ResultsCanvasGroup, 0f, 0.5f).uniqueId;
    }

    public override void Close()
    {
        //hide main panel
        base.Close();

        //hide subpanels
        HideResults();
        HideChanneling();
    }

    private void OnClickStop()
    {
        m_ChannelingCanvasGroup.interactable = false;

        //send stop channeling request
        SpellChanneling.StopChanneling(
            (res, error) =>
            {
                ShowResults(res, error);
            });
    }

    private void OnClickContinue()
    {
        //propagate results back to whom called this UIChanneling.Show
        m_OnClickContinue?.Invoke(m_Results);
        m_OnClickContinue = null;

        Close();
    }
}
