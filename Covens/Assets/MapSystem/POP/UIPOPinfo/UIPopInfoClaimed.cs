using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPopInfoClaimed : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;

    [Space(2)]
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Owner;
    [SerializeField] private TextMeshProUGUI m_Reward;
    [SerializeField] private TextMeshProUGUI m_Cooldown;
    [SerializeField] private TextMeshProUGUI m_OwnerSchool;
    [SerializeField] private TextMeshProUGUI m_EnterText;

    [Space(2)]
    [SerializeField] private Image m_OwnerSchoolArt;

    [Space(2)]
    [SerializeField] private Button m_EnterBtn;
    [SerializeField] private Button m_CloseBtn;

    [Space(5)]
    [SerializeField] private Sprite m_ShadowGlyph;
    [SerializeField] private Sprite m_GreyGlyph;
    [SerializeField] private Sprite m_WhiteGlyph;


    private int m_TweenId;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        m_CanvasGroup.gameObject.SetActive(false);
        m_CanvasGroup.alpha = 0;
    }

    public void SetupListeners(UnityAction onEnter, UnityAction onClose)
    {
        m_EnterBtn.onClick.RemoveAllListeners();
        m_CloseBtn.onClick.RemoveAllListeners();

        m_EnterBtn.onClick.AddListener(onEnter);
        m_CloseBtn.onClick.AddListener(onClose);
    }

    public void Show(IMarker marker, Token data)
    {
        IsOpen = true;

        m_Title.text = LocalizeLookUp.GetText("pop_title"); ;
        m_Owner.text = "";
        m_Reward.text = "";
        m_Cooldown.text = "";
        m_OwnerSchool.text = "";

        m_OwnerSchoolArt.overrideSprite = null;

        m_EnterBtn.interactable = false;

        LeanTween.cancel(m_TweenId);
        m_CanvasGroup.gameObject.SetActive(true);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.5f).setEaseOutCubic().uniqueId;
    }

    public void SetupDetails(LocationMarkerDetail data)
    {
        StopAllCoroutines();

        if (!string.IsNullOrEmpty(data.displayName))
            m_Title.text = data.displayName;
        
        if (data.rewardOn != 0)
            m_Reward.text = LocalizeLookUp.GetText("pop_treasure_time").Replace("{{time}}", Utilities.GetSummonTime(data.rewardOn));
        else
            m_Reward.text = "";

        bool isMine = false;
        if (data.isCoven)
        {
            isMine = data.controlledBy == PlayerDataManager.playerData.covenName;
            m_Owner.text = LocalizeLookUp.GetText("pop_owner_coven").Replace("{{coven}}", data.controlledBy);
        }
        else
        {
            isMine = data.controlledBy == PlayerDataManager.playerData.displayName;
            m_Owner.text = LocalizeLookUp.GetText("pop_owner_player").Replace("{{player}}", data.controlledBy);
        }

        m_OwnerSchool.text = "Claimed";

        System.TimeSpan cooldownTimer = Utilities.TimespanFromJavaTime(data.takenOn);
        float secondsRemaining = (60 * 60) - Mathf.Abs((float)cooldownTimer.TotalSeconds);
        bool isCooldown = secondsRemaining > 0;

        if (isCooldown && isMine == false)
            StartCoroutine(CooldownCoroutine(secondsRemaining, data));

        Debug.Log(data.physicalOnly);
        bool canEnter = false; //!PlayerManager.inSpiritForm && data.physicalOnly;

        if (data.physicalOnly && !PlayerManager.inSpiritForm)
            canEnter = true;
        else if (data.physicalOnly && PlayerManager.inSpiritForm)
            canEnter = false;
        else
            canEnter = true;

        m_EnterBtn.interactable = (isMine || !isCooldown) && !data.full && canEnter;


        if (canEnter == false)
            m_EnterText.text = "You need to be in physical form";
        else if (data.full)
            m_EnterText.text = "The Place of Power is full";
        else
            m_EnterText.text = "Enter this Place of Power";
    }

    public void Close(System.Action onComplete = null)
    {
        StopAllCoroutines();

        IsOpen = false;

        if (m_CanvasGroup.gameObject.activeSelf == false)
        {
            onComplete?.Invoke();
            return;
        }

        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0, 0.5f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                m_CanvasGroup.gameObject.SetActive(false);
                onComplete?.Invoke();
            })
            .uniqueId;
    }

    private IEnumerator CooldownCoroutine(float totalseconds, LocationMarkerDetail location)
    {
        int minutes, seconds;
        while (totalseconds > 0)
        {
            minutes = (int)totalseconds / 60;
            seconds = (int)(totalseconds % 60);

            if (minutes > 0)
                m_Cooldown.text = string.Concat(LocalizeLookUp.GetText("pop_cooldown").Replace("{{time}}", string.Concat(minutes, LocalizeLookUp.GetText("lt_time_minutes"), " ", seconds, LocalizeLookUp.GetText("lt_time_secs"), " ")));
            else
                m_Cooldown.text = string.Concat(LocalizeLookUp.GetText("pop_cooldown").Replace("{{time}}", string.Concat(seconds, LocalizeLookUp.GetText("lt_time_secs"))));

            yield return new WaitForSeconds(1.001f);
            totalseconds -= 1;
        }

        m_Cooldown.text = "";
        SetupDetails(location);
    }
}
