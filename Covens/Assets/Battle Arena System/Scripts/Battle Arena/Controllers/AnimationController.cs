using Raincrow.BattleArena.Model;
using Raincrow.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Controllers
{
    public class AnimationController : MonoBehaviour, IAnimationController
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

        // Variables
        private BattleController _battleController;
        private ObjectPool _objectPool;

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
    }

    public interface IAnimationController
    {
        IEnumerator Flee(ICharacterController characterController);
        IEnumerator Banish(ICharacterController characterController);
        IEnumerator Summon(ICharacterController characterController);
        IEnumerator Summon(IList<ICharacterController> characterControllers);
        IEnumerator Move(ICharacterController characterController, BattleSlot targetBattleSlot);
    }
}