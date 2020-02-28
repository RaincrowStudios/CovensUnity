using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace Raincrow.BattleArena.Views
{
    public class QuickCastView : MonoBehaviour, IQuickCastView
    {
        // Constants
        private const float TimeToToggle = 0.05f;

        // Serialized Variables
        [Header("UI")]
        [SerializeField] private Image _imageIcon;
        [SerializeField] private Sprite _iconClose;
        [SerializeField] private Sprite _iconOpen;
        [SerializeField] private TextMeshProUGUI _textAmountActions;

        [Header("Actions Buttons")]
        [SerializeField] private Button _buttonFly;
        [SerializeField] private Button _buttonSummon;
        [SerializeField] private Button _buttonFlee;

        [Header("Menus")]
        [SerializeField] private GameObject _actionsMenu;
        [SerializeField] private GameObject _spellMenu;

        // Variables
        private GameObject _currentMenu;
        private bool _isOpen;

        public void Show(UnityAction onClickFly, UnityAction onClickSummon, UnityAction onClickFlee)
        {
            gameObject.SetActive(true);
            _buttonFly.onClick.AddListener(onClickFly);
            _buttonSummon.onClick.AddListener(onClickSummon);
            _buttonFlee.onClick.AddListener(onClickFlee);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _buttonFly.onClick.RemoveAllListeners();
            _buttonSummon.onClick.RemoveAllListeners();
            _buttonFlee.onClick.RemoveAllListeners();
        }

        public void OpenSpellMenu()
        {
            if (isActiveAndEnabled)
            {
                ChangeMenu(_spellMenu);
            }
        }

        public void OpenActionsMenu()
        {
            if (isActiveAndEnabled)
            {
                ChangeMenu(_actionsMenu);
            }
        }

        private void ChangeMenu(GameObject menu)
        {
            if (menu == _currentMenu)
            {
                if (!_isOpen)
                {
                    OnClickToggle();
                }
                return;
            }

            menu.SetActive(true);

            if (_currentMenu == null || !_isOpen)
            {
                if (_currentMenu != null)
                {
                    _currentMenu.SetActive(false);
                }

                LeanTween.scaleY(menu, 1.0f, TimeToToggle);
                _currentMenu = menu;
            }
            else if (_isOpen)
            {
                LeanTween.scaleY(_currentMenu, 0.0f, TimeToToggle).setOnComplete(() =>
                {
                    _currentMenu.SetActive(false);
                    LeanTween.scaleY(menu, 1.0f, TimeToToggle);
                    _currentMenu = menu;
                });
            }


            _isOpen = true;
            _imageIcon.sprite = _iconOpen;
        }

        public void OnClickToggle(float time = 0.05f)
        {
            if (_currentMenu == null)
            {
                return;
            }

            LeanTween.scaleY(_currentMenu, _isOpen ? 0 : 1, time);
            _isOpen = !_isOpen;
            _imageIcon.sprite = _isOpen ? _iconOpen : _iconClose;
        }
    }

    public interface IQuickCastView
    {
        void Show(UnityAction onClickFly, UnityAction onClickSummon, UnityAction onClickFlee);

        void Hide();

        void OpenSpellMenu();

        void OpenActionsMenu();
    }
}
