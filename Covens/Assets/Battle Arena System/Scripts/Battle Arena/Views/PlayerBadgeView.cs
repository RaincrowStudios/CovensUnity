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

        // Variables        
        private IWitchPortraitFactory _witchPortraitFactory;

        protected virtual void OnEnable()
        {
            if (_serviceLocator == null)
            {
                _serviceLocator = FindObjectOfType<ServiceLocator>();
            }

            if (_witchPortraitFactory == null)
            {
                _witchPortraitFactory = _serviceLocator.GetWitchPortraitFactory();
            }
        }

        public IEnumerator Show(IWitchModel witchModel)
        {
            gameObject.SetActive(true);

            // Wait for Service Locator to not be null anymore
            yield return new WaitUntil(() => _serviceLocator != null);

            IEnumerator<Sprite> getPortrait = GetPortrait(witchModel);
            yield return getPortrait;

            _playerIcon.overrideSprite = getPortrait.Current;
            _playerLevel.text = witchModel.Level.ToString();
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
        IEnumerator Show(IWitchModel witchModel);
        void Hide();
    }
}