using UnityEngine;
using System.Collections.Generic;
using Raincrow.Services;
using Raincrow.BattleArena.Model;

namespace Raincrow.BattleArena.Views
{
    public class StatusEffectsView : MonoBehaviour, IStatusEffectsView
    {
        // Serialized Fields
        [SerializeField] private ServiceLocator _serviceLocator;
        [SerializeField] private StatusEffectView _statusEffectViewPrefab;

        // Private
        private ObjectPool _objectPool;

        // Methods
        protected virtual void OnEnable()
        {
            if (_objectPool == null)
            {
                _objectPool = _serviceLocator.GetObjectPool();
            }

            if (_objectPool != null)
            {
                _objectPool.RecycleAll(_statusEffectViewPrefab);
            }
        }

        protected virtual void OnDisable()
        {
            if (_objectPool != null)
            {
                _objectPool.RecycleAll(_statusEffectViewPrefab);
            }
        }

        public void UpdateView(IList<IStatusEffect> statusEffects)
        {
            _objectPool.RecycleAll(_statusEffectViewPrefab);

            foreach (var statusEffect in statusEffects)
            {
                IStatusEffectView statusEffectView = _objectPool.Spawn(_statusEffectViewPrefab);
                DownloadedAssets.GetSprite(statusEffect.SpellId, statusEffectView.SpellImage, true);

                int duration = statusEffect.Duration;
                float maxDuration = statusEffect.MaxDuration;
                statusEffectView.DurationNormalized = duration / maxDuration;
            }
        }
    }

    public interface IStatusEffectsView
    {
        void UpdateView(IList<IStatusEffect> statusEffects);
    }
}