using System.Collections;
using System.Collections.Generic;
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

        public void Show(float time)
        {
            gameObject.SetActive(true);
            StartCoroutine(StartCountdown(time));
        }

        private IEnumerator StartCountdown(float time)
        {
            for(float t = time; t > 0; t -= Time.deltaTime)
            {
                _textTime.text = Mathf.FloorToInt(t).ToString();
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public interface ICountdownView
    {
        void Show(float time);

        void Hide();
    }
}