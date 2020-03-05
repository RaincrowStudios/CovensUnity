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
            _moonPhaseIcon.sprite = GetMoonPhase(witchModel.Degree);
            if (isInactive)
            {
                gameObject.SetActive(false);
            }
        }

        private Sprite GetMoonPhase(int alignment)
        {
            switch (alignment)
            {
                case 0:
                    return _moonPhases[7];
                case 1:
                case 2:
                    return _moonPhases[7];
                case 3:
                case 4:
                    return _moonPhases[9];
                case 5:
                case 6:
                    return _moonPhases[10];
                case 7:
                case 8:
                    return _moonPhases[11];
                case 9:
                case 10:
                    return _moonPhases[12];
                case 11:
                case 12:
                    return _moonPhases[13];
                case 13:
                case 14:
                    return _moonPhases[14];
                case -1:
                case -2:
                    return _moonPhases[6];
                case -3:
                case -4:
                    return _moonPhases[5];
                case -5:
                case -6:
                    return _moonPhases[4];
                case -7:
                case -8:
                    return _moonPhases[3];
                case -9:
                case -10:
                    return _moonPhases[2];
                case -11:
                case -12:
                    return _moonPhases[1];
                default:
                    return _moonPhases[0];
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