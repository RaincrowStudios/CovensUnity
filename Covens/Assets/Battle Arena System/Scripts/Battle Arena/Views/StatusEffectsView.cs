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
        [SerializeField] private Transform _statusEffectRoot;

        // Private
        private ObjectPool _objectPool;

        public void Show(IList<IStatusEffect> statusEffects)
        {
            if (_objectPool == null && _serviceLocator != null)
            {
                _objectPool = _serviceLocator.GetObjectPool();
            }

            if (_objectPool != null)
            {
                _objectPool.RecycleAll(_statusEffectViewPrefab);

                foreach (var statusEffect in statusEffects)
                {
                    IStatusEffectView statusEffectView = _objectPool.Spawn(_statusEffectViewPrefab, _statusEffectRoot, false);
                    DownloadedAssets.GetSprite(statusEffect.SpellId, statusEffectView.SpellImage, true);

                    int duration = statusEffect.Duration;
                    float maxDuration = statusEffect.MaxDuration;
                    statusEffectView.DurationNormalized = duration / maxDuration;
                }
            }
        }

        public void Hide()
        {
            if (_objectPool == null && _serviceLocator != null)
            {
                _objectPool = _serviceLocator.GetObjectPool();
            }

            if (_objectPool != null)
            {
                _objectPool.RecycleAll(_statusEffectViewPrefab);
            }
        }
    }

    public interface IStatusEffectsView
    {
        void Show(IList<IStatusEffect> statusEffects);
        void Hide();
    }
}