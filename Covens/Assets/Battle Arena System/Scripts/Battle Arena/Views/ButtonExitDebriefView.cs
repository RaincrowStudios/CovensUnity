using UnityEngine;

namespace Raincrow.BattleArena.Views
{
    public class ButtonExitDebriefView : MonoBehaviour, IButtonExitDebriefView
    {
        [SerializeField] UnityEngine.UI.Button _buttonExit;

        public void Hide()
        {
            gameObject.SetActive(false);

            _buttonExit.onClick.RemoveAllListeners();
        }

        public void Show(UnityEngine.Events.UnityAction OnClickExit)
        {
            gameObject.SetActive(true);

            _buttonExit.onClick.AddListener(OnClickExit);
        }
    }
    public interface IButtonExitDebriefView
    {
        void Show(UnityEngine.Events.UnityAction OnClickExit);
        void Hide();
    }
}


