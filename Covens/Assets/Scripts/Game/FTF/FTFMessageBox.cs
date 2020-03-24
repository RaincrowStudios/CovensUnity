using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Raincrow.FTF
{
    public class FTFMessageBox : MonoBehaviour
    {
        [SerializeField] private Animator m_Animator;
        [SerializeField] private Animator m_GlitterFx;
        [SerializeField] private CanvasGroup m_CanvasGroup;
        [SerializeField] private TextMeshProUGUI m_Message;
        [SerializeField] private Button m_ContinueButton;

        public bool IsOpen { get; private set; }
        private int m_TweenId;
        private int m_ButtonTweenId;
        public event System.Action OnClick;

        private void Awake()
        {
            m_CanvasGroup.alpha = 0;
            m_ContinueButton.interactable = false;
            m_ContinueButton.onClick.AddListener(OnClickNext);
            gameObject.SetActive(false);
        }

        public void Show(string message, List<string> replace)
        {
            LeanTween.cancel(m_TweenId);
            LeanTween.cancel(m_ButtonTweenId);

            if (replace != null && replace.Count > 0)
                message = ReplaceWords(message, replace);

            if (IsOpen)
            {
                //quickly fade out, set new message, fade in
                m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0.0f, 0.2f).setEaseOutCubic().setOnComplete(() =>
                {
                    m_Message.text = message;
                    m_ContinueButton.interactable = false;
                    m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.5f).uniqueId;
                    m_ButtonTweenId = LeanTween.value(0, 0, 0.5f).setOnComplete(() => m_ContinueButton.interactable = true).uniqueId;
                    m_Animator.Play("slideInDiag", 0, 0.8f);
                    m_GlitterFx.Play("menuFX", 0, 0);

                }).uniqueId;
                return;
            }

            //show message
            m_Message.text = message;

            //play slide anim
            m_Animator.Play("slideInDiag");
            m_GlitterFx.Play("menuFX", 0, 0);

            //fade in
            m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1f, 1f)
                .setEaseOutCubic()
                .uniqueId;

            //enable button
            m_ButtonTweenId = LeanTween.value(0, 0, 0.5f).setOnComplete(() => m_ContinueButton.interactable = true).uniqueId;

            gameObject.SetActive(true);
            IsOpen = true;
        }

        public void EnableButton(bool enable)
        {
            m_ContinueButton.gameObject.SetActive(enable);
        }

        public void Hide()
        {
            Hide(null, 1, LeanTweenType.easeOutCubic);
        }

        public void Hide(System.Action onComplete, float time, LeanTweenType easeType)
        {
            if (IsOpen == false)
                return;

            //disable button
            m_ContinueButton.interactable = false;

            //play slide reversed
            //m_Animator.Play("slideOutDiag");

            //fade out
            LeanTween.cancel(m_TweenId);
            m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0f, time)
                .setEase(easeType)
                .setOnComplete(() =>
                {
                    IsOpen = false;
                    gameObject.SetActive(false);
                    onComplete?.Invoke();
                })
                .uniqueId;
        }

        private void OnClickNext()
        {
            Hide(null, 0.25f, LeanTweenType.easeOutCubic);
            OnClick?.Invoke();
        }


        private delegate string ReplaceStringDelegate();

        private string ReplaceWords(string message, List<string> keys)
        {
            var replacers = new Dictionary<string, ReplaceStringDelegate>()
            {
                { "{witch name}", GetPlayerName  },
                { "{he/she}", GetHeShe },
                { "{his/her}", GetHisHer },
                { "{locationName}", GetNearbyPoPName },
                { "{tribunalSeason}", GetTribunalSeason },
                { "{tribunalDays}", GetTribunalDays },
            };

            string word;
            foreach (string k in keys)
            {
                if (replacers.ContainsKey(k))
                    word = replacers[k].Invoke();
                else
                    word = k;

                message = message.Replace(k, word);
            }

            return message;
        }

        private string GetPlayerName()
        {
            return "<color=#4FD5FF>" + PlayerDataManager.playerData.name + "</color>";
        }

        private string GetHeShe()
        {
            return PlayerDataManager.playerData.male ? LocalizeLookUp.GetText("ftf_he") : LocalizeLookUp.GetText("ftf_she");
        }

        private string GetHisHer()
        {
            return PlayerDataManager.playerData.male ? LocalizeLookUp.GetText("ftf_his") : LocalizeLookUp.GetText("ftf_her");
        }

        private string GetNearbyPoPName()
        {
            return UINearbyLocations.CachedLocations != null && UINearbyLocations.CachedLocations.Count > 0 ?
                "<color=#4FD5FF>" + UINearbyLocations.CachedLocations[0].name + "</color>" :
                "<color=#FF3939></color>";
        }

        private string GetTribunalSeason()
        {
            int tribunal = PlayerDataManager.tribunal;

            string season = "";
            if (tribunal == 2)
                season = LocalizeLookUp.GetText("ftf_summer");
            else if (tribunal == 1)
                season = LocalizeLookUp.GetText("ftf_spring");
            else if (tribunal == 3)
                season = LocalizeLookUp.GetText("ftf_autumn");
            else
                season = LocalizeLookUp.GetText("ftf_winter");

            return "<color=#4FD5FF>" + season + "</color>";
        }

        private string GetTribunalDays()
        {
            System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(PlayerDataManager.endOfTribunal);
            var timeSpan = dtDateTime.Subtract(System.DateTime.UtcNow);
            return "<color=#4FD5FF>" + timeSpan.TotalDays.ToString("N0") + "</color>";
        }
    }
}