using System.Collections;
using UnityEngine;
using TMPro;

namespace Raincrow.BattleArena.Views
{
    public class CountdownView : MonoBehaviour, ICountdownView
    {
        [SerializeField] TextMeshProUGUI _textTime;

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void UpdateTime(int time)
        {
            _textTime.text = time.ToString();
        }
       
    }

    public interface ICountdownView
    {
        void Show();

        void Hide();

        void UpdateTime(int time);
    }
}