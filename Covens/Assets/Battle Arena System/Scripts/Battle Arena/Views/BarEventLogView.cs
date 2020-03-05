using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Raincrow.BattleArena.Views
{
    public class BarEventLogView : MonoBehaviour, IBarEventLogView
    {
        [Header("UI")]
        [SerializeField] private Button _toggleButton;
        [SerializeField] private Image _barEventLog;
        [SerializeField] private Image _backgroundEventLog;
        [SerializeField] private Image _imageToogleOpen;
        [SerializeField] private Sprite _spriteToogleOpen;
        [SerializeField] private Sprite _spriteToogleClose;
        [SerializeField] private TextMeshProUGUI _textLog;
        [SerializeField] private ScrollRect scrollRectLogs;

        [Header("Animation")]
        [SerializeField] private float _speedAnimation = 0.3f;
        [SerializeField] private float _widthBarOpen = 750f;
        [SerializeField] private float _widthBarClose = 390f;
        [SerializeField] private float _heightBackgroundClose = 0f;
        [SerializeField] private float _heightBackgroundOpen = 800f;

        //Private variables
        private bool _isOpen;
        private int _leanAnimScrollID;

        private void Start()
        {
            _toggleButton.onClick.AddListener(OnClickToggle);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            _isOpen = false;
            _imageToogleOpen.sprite = _spriteToogleClose;

            Vector2 sizeWidthAnim = _barEventLog.rectTransform.sizeDelta;
            sizeWidthAnim.x = _widthBarClose;
            LeanTween.size(_barEventLog.rectTransform, sizeWidthAnim, 0);

            Vector2 sizeHeightAnim = _backgroundEventLog.rectTransform.sizeDelta;
            sizeHeightAnim.y = _heightBackgroundClose;
            LeanTween.size(_backgroundEventLog.rectTransform, sizeHeightAnim, 0);

            gameObject.SetActive(false);
        }

        public void AddLog(string log)
        {
            log = FormatLog(log);
            _textLog.text += string.IsNullOrWhiteSpace(_textLog.text) ? "" : "\n";
            _textLog.text += log;

            LeanTween.cancel(_leanAnimScrollID);
            float scrollY = scrollRectLogs.verticalNormalizedPosition;
            _leanAnimScrollID = LeanTween.value(scrollY, 0.0f, 1f).setOnUpdate((value) =>
            {
                scrollRectLogs.verticalNormalizedPosition = value;
            }).id;
        }

        private void OnClickToggle()
        {
            _isOpen = !_isOpen;
            _imageToogleOpen.sprite = _isOpen ? _spriteToogleOpen : _spriteToogleClose;

            Vector2 sizeWidthAnim = _barEventLog.rectTransform.sizeDelta;
            sizeWidthAnim.x = _isOpen ? _widthBarOpen : _widthBarClose;
            LeanTween.size(_barEventLog.rectTransform, sizeWidthAnim, _speedAnimation);

            Vector2 sizeHeightAnim = _backgroundEventLog.rectTransform.sizeDelta;
            sizeHeightAnim.y = _isOpen ? _heightBackgroundOpen : _heightBackgroundClose;
            LeanTween.size(_backgroundEventLog.rectTransform, sizeHeightAnim, _speedAnimation);
        }

        private string FormatLog(string value)
        {
            value = value.Replace("<witch>", "<b>");
            value = value.Replace("</witch>", "</b>");

            value = value.Replace("<spell>", "<b>");
            value = value.Replace("</spell>", "</b>");

            value = value.Replace("<damage>", "<color=#ff0000>");
            value = value.Replace("</damage>", "</color>");

            value = value.Replace("<time>", "<space=10><size=15>");
            value = value.Replace("</time>", "</size>");

            return value;
        }
    }

    public interface IBarEventLogView
    {
        void Show();
        void Hide();
        void AddLog(string log);
    }
}