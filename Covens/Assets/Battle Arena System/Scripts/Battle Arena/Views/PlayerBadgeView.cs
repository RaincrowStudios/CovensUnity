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

        [Header("Condition Button")]
        [SerializeField] private Button _conditionButton;
        [SerializeField] private CanvasGroup _groupButton;
        [SerializeField] private TMPro.TextMeshProUGUI _textAmountConditions;
        private int _currentAmountEffects = 0;
        private IStatusEffectsView _playerStatusEffectsView;

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

            if (_playerStatusEffectsView == null)
            {
                _playerStatusEffectsView = _serviceLocator.GetPlayerStatusEffectsView();
            }

            IEnumerator<Sprite> getPortrait = GetPortrait(witchModel);
            yield return getPortrait;

            _playerIcon.overrideSprite = getPortrait.Current;
            _playerLevel.text = witchModel.Level.ToString();

            float degreeNormalized = Mathf.InverseLerp(MaxShadowDegree, MaxWhiteDegree, witchModel.Degree);
            int moonPhase = Mathf.FloorToInt(Mathf.Lerp(0, _moonPhases.Length - 1, degreeNormalized));
            _moonPhaseIcon.sprite = _moonPhases[moonPhase];

            _conditionButton.onClick.AddListener(TogglePlayerStatusEffectsView);

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
            IEnumerator<SpriteRequest> enumerator = _witchPortraitFactory.CreateIWitchPortrait(witchModel, false);
            Coroutine<SpriteRequest> coroutine = this.StartCoroutine<SpriteRequest>(enumerator);
            while (coroutine.keepWaiting)
            {
                yield return null;
            }

            SpriteRequest request = coroutine.ReturnValue;
            yield return request.Sprite;
        }

        public void UpdateConditions(int amount)
        {
            _textAmountConditions.text = amount.ToString();

            if (_currentAmountEffects == 0 && amount > 0)
            {
                LeanTween.alphaCanvas(_groupButton, 1, 0.5f);
            }
            else if (amount == 0 && _currentAmountEffects > 0)
            {
                LeanTween.alphaCanvas(_groupButton, 0, 0.5f);
            }

            _currentAmountEffects = amount;
        }

        private void TogglePlayerStatusEffectsView()
        {
            if (_playerStatusEffectsView.IsOpen())
            {
                StartCoroutine(_playerStatusEffectsView.Hide());
            }
            else
            {
                Controllers.ICharacterController character = _serviceLocator.GetBattleController().GetCharacter(PlayerDataManager.playerData.instance);
                if (character != default && character.Model != default)
                {
                    IList<IStatusEffect> statusEffects = character.Model.StatusEffects;
                    StartCoroutine(_playerStatusEffectsView.Show(statusEffects));
                }
            }
        }
    }

    public interface IPlayerBadgeView
    {
        IEnumerator Init(IWitchModel witchModel);
        void Show();
        void Hide();
        void UpdateConditions(int amount);
    }
}