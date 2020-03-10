using Raincrow.BattleArena.Model;
using Raincrow.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Controllers
{
    public partial class AnimationController : MonoBehaviour, IAnimationController
    {
        // Serialized Variables
        [SerializeField] private ServiceLocator _serviceLocator;

        [Header("Move Animation")]
        [SerializeField] private float _moveAnimationTime = 1f;
        [SerializeField] private Easings.Functions _moveAnimationFunction = Easings.Functions.Linear;

        [Header("Summon Animation")]
        [SerializeField] private ParticleSystem _summonAnimPrefab;
        [SerializeField] private float _summonAnimationTime = 1f;
        [SerializeField] private Easings.Functions _summonAnimationFunction = Easings.Functions.Linear;

        [Header("Damage Animation")]
        [SerializeField] private float _damageAnimationTime = 2f;
        [SerializeField] private float _damageTextScale = 1f;
        [SerializeField] private float _criticalDamageTextScale = 1.4f;        

        // Variables
        private BattleController _battleController;
        private ObjectPool _objectPool;
        private Camera _battleCamera;

        protected virtual void OnEnable()
        {
            if (_serviceLocator == null)
            {
                _serviceLocator = FindObjectOfType<ServiceLocator>();
            }

            // Could not lazily initialize Service Locator
            if (_serviceLocator == null)
            {
                Debug.LogError("Could not find Service Locator!");
            }

            if (_battleController == null)
            {
                _battleController = _serviceLocator.GetBattleController();
            }

            if (_objectPool == null)
            {
                _objectPool = _serviceLocator.GetObjectPool();
            }

            if (_battleCamera == null)
            {
                _battleCamera = _serviceLocator.GetBattleCamera();
            }
        }

        public IEnumerator Summon(ICharacterController characterController)
        {
            // Spawn Particle System
            Vector3 position = characterController.Transform.position;
            Quaternion quartenion = _summonAnimPrefab.transform.rotation;
            ParticleSystem summonParticles = _objectPool.Spawn(_summonAnimPrefab, position, quartenion);
            summonParticles.Play();

            // Summon Animation
            for (float elapsedTime = 0; elapsedTime < _summonAnimationTime; elapsedTime += Time.deltaTime)
            {
                float t = Easings.Interpolate(elapsedTime / _summonAnimationTime, _summonAnimationFunction);
                characterController.Transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                yield return null;
            }
            yield return new WaitUntil(() => summonParticles.isStopped);

            // Recycle Particle System
            _objectPool.Recycle(summonParticles);
        }

        public IEnumerator Move(ICharacterController characterController, BattleSlot targetBattleSlot)
        {            
            ICellUIModel model = _battleController.Cells[targetBattleSlot.Row, targetBattleSlot.Col];
            Vector3 position = model.Transform.position;

            for (float elapsedTime = 0; elapsedTime < _moveAnimationTime; elapsedTime += Time.deltaTime)
            {
                float t = Easings.Interpolate(elapsedTime / _moveAnimationTime, _moveAnimationFunction);
                characterController.Transform.position = Vector3.Lerp(position, position, t);
                yield return null;
            }
        }

        public IEnumerator Summon(IList<ICharacterController> characterControllers)
        {
            foreach (var characterController in characterControllers)
            {
                StartCoroutine(Summon(characterController));
            }
            yield return new WaitForSeconds(_summonAnimationTime);
        }

        public IEnumerator Banish(ICharacterController characterController)
        {
            Vector3 position = characterController.Transform.position;
            Quaternion quartenion = _summonAnimPrefab.transform.rotation;

            // Summon Animation
            for (float elapsedTime = 0; elapsedTime < _summonAnimationTime; elapsedTime += Time.deltaTime)
            {
                float t = Easings.Interpolate(elapsedTime / _summonAnimationTime, _summonAnimationFunction);
                characterController.Transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
                yield return null;
            }
        }

        public IEnumerator Flee(ICharacterController characterController)
        {
            Vector3 position = characterController.Transform.position;
            Quaternion quartenion = _summonAnimPrefab.transform.rotation;

            // Summon Animation
            for (float elapsedTime = 0; elapsedTime < _summonAnimationTime; elapsedTime += Time.deltaTime)
            {
                float t = Easings.Interpolate(elapsedTime / _summonAnimationTime, _summonAnimationFunction);
                characterController.Transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
                yield return null;
            }
        }

        public IEnumerator CastSpell(int spellDegree, ICharacterController caster, ICharacterController target)
        {
            bool isCastSpellComplete = false;
            SpawnTrail(spellDegree, caster.Transform, target.Transform, null, () =>
            {
                isCastSpellComplete = true;
            });
            yield return new WaitUntil(() => isCastSpellComplete);
        }

        public IEnumerator ApplyDamage(ICharacterController target, int damage, bool isCritical)
        {
            float textScale = isCritical ? _criticalDamageTextScale : _damageTextScale;
            SpawnDamageText(target.Transform, damage, textScale);

            int previousEnergy = target.Model.Energy;
            int nextEnergy = target.Model.Energy - damage;
            nextEnergy = Mathf.Max(nextEnergy, 0);

            //Show Damage decreasing over time
            for (float elapsedTime = 0; elapsedTime < _damageAnimationTime; elapsedTime += Time.deltaTime)
            {
                float energy = Mathf.Lerp(previousEnergy, nextEnergy, elapsedTime / _damageAnimationTime);
                target.UpdateView(target.Model.BaseEnergy, Mathf.FloorToInt(energy));
                yield return null;
            }            
        }
    }

    public interface IAnimationController
    {
        IEnumerator Flee(ICharacterController characterController);
        IEnumerator Banish(ICharacterController characterController);
        IEnumerator Summon(ICharacterController characterController);
        IEnumerator Summon(IList<ICharacterController> characterControllers);
        IEnumerator Move(ICharacterController characterController, BattleSlot targetBattleSlot);
        IEnumerator CastSpell(int spellDegree, ICharacterController caster, ICharacterController target);
        IEnumerator ApplyDamage(ICharacterController target, int damage, bool isCritical);        
        void SpawnTrail(int degree, Transform caster, Transform target, System.Action onStart, System.Action onComplete);
        void SpawnDamageText(Transform target, int amount, float scale);
    }
}