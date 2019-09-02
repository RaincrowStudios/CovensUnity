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

        public void Show(string message)
        {
            LeanTween.cancel(m_TweenId);
            LeanTween.cancel(m_ButtonTweenId);

            if (IsOpen)
            {
                //quickly fade out, set new message, fade in
                m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 0.0f, 0.2f).setEaseOutCubic().setOnComplete(() =>
                {
                    m_Message.text = message;
                    m_ContinueButton.interactable = false;
                    m_TweenId = LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.5f).uniqueId;
                    m_ButtonTweenId = LeanTween.value(0, 0, 0.5f).setOnComplete(() => m_ContinueButton.interactable = true).uniqueId;
                    m_Animator.Play("slideInDiag", 0, 1f);

                }).uniqueId;
                return;
            }

            //show message
            m_Message.text = message;

            //play slide anim
            m_Animator.Play("slideInDiag");

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
            m_Animator.Play("slideOutDiag");

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
    }
}