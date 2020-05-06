using UnityEngine;
using System.Collections.Generic;
using Raincrow.Services;
using Raincrow.BattleArena.Model;
using System.Collections;

namespace Raincrow.BattleArena.Views
{
    public class StatusEffectsView : MonoBehaviour, IStatusEffectsView
    {
        // Serialized Fields
        [SerializeField] private ServiceLocator _serviceLocator;
        [SerializeField] private StatusEffectView _statusEffectViewPrefab;
        [SerializeField] private Transform _statusEffectRoot;

        [Header("Animation")]
        [SerializeField] private float _startPositionX = 200f;
        [SerializeField] private float _endPositionX = 200f;
        [SerializeField] private float _showAnimationTime = 1f;
        [SerializeField] private Easings.Functions _showAnimationType = Easings.Functions.Linear;

        // Private variable
        private ObjectPool _objectPool;
        private RectTransform _rectTransform;
        private bool isOpen;

        protected virtual void OnEnable()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            if (_objectPool == null)
            {
                _objectPool = _serviceLocator.GetObjectPool();
            }
        }

        public IEnumerator Show(IList<IStatusEffect> statusEffects)
        {
            _objectPool.RecycleAll(_statusEffectViewPrefab);

            isOpen = true;

            Vector2 position = _rectTransform.anchoredPosition;

            foreach (var statusEffect in statusEffects)
            {
                IStatusEffectView statusEffectView = _objectPool.Spawn(_statusEffectViewPrefab, _statusEffectRoot, false);
                DownloadedAssets.GetSprite(statusEffect.SpellId, statusEffectView.SpellImage, true);

                int duration = statusEffect.Duration;
                float maxDuration = statusEffect.MaxDuration;
                statusEffectView.DurationNormalized = duration / maxDuration;
            }

            LTDescr openingAnimation = LeanTween.value(position.x, _endPositionX, _showAnimationTime).setOnUpdate((float positionX) => {
                position.x = positionX;
                _rectTransform.anchoredPosition = position;
            });

            yield return new WaitForSeconds(_showAnimationTime);
        }

        public IEnumerator Hide()
        {
            _objectPool.RecycleAll(_statusEffectViewPrefab);

            isOpen = false;


            Vector2 position = _rectTransform.anchoredPosition;

            LTDescr openingAnimation = LeanTween.value(position.x, _startPositionX, _showAnimationTime).setOnUpdate((float positionX) => {
                position.x = positionX;
                _rectTransform.anchoredPosition = position;
            });

            yield return new WaitForSeconds(_showAnimationTime);
        }

        public bool IsOpen()
        {
            return isOpen;
        }
    }

    public interface IStatusEffectsView
    {
        IEnumerator Show(IList<IStatusEffect> statusEffects);
        IEnumerator Hide();
        bool IsOpen();
    }
}