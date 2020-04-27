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
            Vector2 position = _rectTransform.anchoredPosition;
            
            _objectPool.RecycleAll(_statusEffectViewPrefab);
            for (float elapsedTime = 0f; elapsedTime <= _showAnimationTime; elapsedTime = Mathf.MoveTowards(elapsedTime, _showAnimationTime + Mathf.Epsilon, Time.deltaTime))
            {
                float t = Easings.Interpolate(elapsedTime / _showAnimationTime, _showAnimationType);
                position.x = Mathf.Lerp(_startPositionX, _endPositionX, t);
                _rectTransform.anchoredPosition = position;
                yield return null;
            }

            foreach (var statusEffect in statusEffects)
            {
                IStatusEffectView statusEffectView = _objectPool.Spawn(_statusEffectViewPrefab, _statusEffectRoot, false);
                DownloadedAssets.GetSprite(statusEffect.SpellId, statusEffectView.SpellImage, true);

                int duration = statusEffect.Duration;
                float maxDuration = statusEffect.MaxDuration;
                statusEffectView.DurationNormalized = duration / maxDuration;
            }
        }

        public IEnumerator Hide()
        {
            _objectPool.RecycleAll(_statusEffectViewPrefab);

            Vector2 position = _rectTransform.anchoredPosition;
            for (float elapsedTime = 0f; elapsedTime <= _showAnimationTime; elapsedTime = Mathf.MoveTowards(elapsedTime, _showAnimationTime + Mathf.Epsilon, Time.deltaTime))
            {
                float t = Easings.Interpolate(elapsedTime / _showAnimationTime, _showAnimationType);
                position.x = Mathf.Lerp(_endPositionX, _startPositionX, t);
                _rectTransform.anchoredPosition = position;
                yield return null;
            }
        }
    }

    public interface IStatusEffectsView
    {
        IEnumerator Show(IList<IStatusEffect> statusEffects);
        IEnumerator Hide();
    }
}