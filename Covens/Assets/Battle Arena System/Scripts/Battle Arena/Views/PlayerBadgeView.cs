using Raincrow.BattleArena.Factory;
using Raincrow.BattleArena.Model;
using Raincrow.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raincrow.BattleArena.Views
{
    public class PlayerBadgeView : MonoBehaviour, IPlayerBadgeView
    {
        // Serialized Variables
        [SerializeField] private Image _playerIcon; 
        [SerializeField] private Text _playerLevel;
        [SerializeField] private Image _moonPhaseIcon;
        [SerializeField] private ServiceLocator _serviceLocator;
        [SerializeField] private Sprite[] _moonPhases = new Sprite[0];

        // Variables        
        private IWitchPortraitFactory _witchPortraitFactory;

        private static readonly int MaxShadowDegree = -14;
        private static readonly int MaxWhiteDegree = 14;

        public IEnumerator Init(IWitchModel witchModel)
        {
            bool isInactive = !isActiveAndEnabled;
            if (isInactive)
            {
                gameObject.SetActive(true);
            }

            if (_serviceLocator == null)
            {
                _serviceLocator = FindObjectOfType<ServiceLocator>();
            }

            if (_witchPortraitFactory == null)
            {
                _witchPortraitFactory = _serviceLocator.GetWitchPortraitFactory();
            }

            IEnumerator<Sprite> getPortrait = GetPortrait(witchModel);
            yield return getPortrait;

            _playerIcon.overrideSprite = getPortrait.Current;
            _playerLevel.text = witchModel.Level.ToString();

            float degreeNormalized = Mathf.InverseLerp(MaxShadowDegree, MaxWhiteDegree, witchModel.Degree);
            int moonPhase = Mathf.FloorToInt(Mathf.Lerp(0, _moonPhases.Length - 1, degreeNormalized));
            _moonPhaseIcon.sprite = _moonPhases[moonPhase];

            if (isInactive)
            {
                gameObject.SetActive(false);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator<Sprite> GetPortrait(IWitchModel witchModel)
        {
            IEnumerator<SpriteRequest> enumerator = _witchPortraitFactory.CreateIWitchPortrait(witchModel);
            Coroutine<SpriteRequest> coroutine = this.StartCoroutine<SpriteRequest>(enumerator);
            while (coroutine.keepWaiting)
            {
                yield return null;
            }

            SpriteRequest request = coroutine.ReturnValue;
            yield return request.Sprite;
        }
    }

    public interface IPlayerBadgeView
    {
        IEnumerator Init(IWitchModel witchModel);
        void Show();
        void Hide();
    }
}