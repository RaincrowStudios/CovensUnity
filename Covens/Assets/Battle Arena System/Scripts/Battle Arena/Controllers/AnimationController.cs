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

        [Header("Spell Animation")]
        [SerializeField] private float _chargeLifecycle = 0.5f;
        [SerializeField] private float _trailLifecycle = 0.25f;
        [SerializeField] private float _hitLifecycle = 0.5f;
        [SerializeField] private Vector3 _chargeScale = new Vector3(20, 20, 20);
        [SerializeField] private Vector3 _trailScale = new Vector3(15, 15, 15);
        [SerializeField] private Vector3 _hitScale = new Vector3(20, 20, 20);

        [Header("Damage Animation")]
        [SerializeField] private Easings.Functions _damageAnimationFunction = Easings.Functions.CubicEaseInOut;        
        [SerializeField] private float _damageAnimationTime = 2f;
        [SerializeField] private float _damageTextScale = 1f;
        [SerializeField] private float _criticalDamageTextScale = 1.4f;
        [SerializeField] private Transform _damageFeedback;
        [SerializeField] private Color _damageColor;
        [SerializeField] private Color _restoreColor;

        [Header("Shadow Animation Prefabs")]
        [SerializeField] private Transform _shadowChargePrefab;
        [SerializeField] private Transform _shadowTrailPrefab;
        [SerializeField] private Transform _shadowHitPrefab;

        [Header("Gray Animation Prefabs")]
        [SerializeField] private Transform _grayChargePrefab;
        [SerializeField] private Transform _grayTrailPrefab;
        [SerializeField] private Transform _grayHitPrefab;

        [Header("Light Animation Prefabs")]
        [SerializeField] private Transform _lightChargePrefab;
        [SerializeField] private Transform _lightTrailPrefab;
        [SerializeField] private Transform _lightHitPrefab;                

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
            Transform casterTransform = caster.Transform;
            Transform targetTransform = target.Transform;
            Transform chargePrefab, trailPrefab, hitPrefab;

            if (spellDegree < 0)
            {
                chargePrefab = _shadowChargePrefab;
                trailPrefab = _shadowTrailPrefab;
                hitPrefab = _shadowHitPrefab;
            }
            else if (spellDegree > 0)
            {
                chargePrefab = _lightChargePrefab;
                trailPrefab = _lightTrailPrefab;
                hitPrefab = _lightHitPrefab;
            }
            else
            {
                chargePrefab = _grayChargePrefab;
                trailPrefab = _grayTrailPrefab;
                hitPrefab = _grayHitPrefab;
            }

            Vector3 offset = targetTransform.up * 40;
            float distance = Vector3.Distance(casterTransform.position, targetTransform.position);

            Vector3 startPosition = casterTransform.position + offset;
            Vector3 targetPosition = targetTransform.position + offset;

            //spawn the charge
            Transform charge = _objectPool.Spawn(chargePrefab, null, casterTransform.position + offset, chargePrefab.transform.rotation);            
            charge.localScale = _chargeScale;
            StartCoroutine(ScheduleRecycle(_chargeLifecycle, charge));

            //just call on complete if the caster is casting on itself
            if (casterTransform != targetTransform)
            {
                //calculate path
                LTBezierPath path;
                Vector3 endcontrol = (startPosition - targetPosition) * Random.Range(0.3f, 0.5f);
                Vector3 startcontrol = (targetPosition - startPosition) * Random.Range(0.3f, 0.5f);

                startcontrol = Quaternion.Euler(0, Random.Range(-100, 100), Random.Range(-100, 100)) * startcontrol;
                endcontrol = Quaternion.Euler(0, Random.Range(-45, 45), Random.Range(-45, 45)) * endcontrol;

                path = new LTBezierPath(new Vector3[] {
                        startPosition, //start point
                        targetPosition + endcontrol,
                        startPosition + startcontrol,
                        targetPosition
                    });

                //spawn the trail                        
                Transform trail = _objectPool.Spawn(trailPrefab, casterTransform.position + offset);
                trail.localScale = _trailScale;
                StartCoroutine(ScheduleRecycle(_trailLifecycle, trail));

                for (float time = 0; time < _trailLifecycle; time += Time.deltaTime)
                {
                    float t = Mathf.InverseLerp(0f, _trailLifecycle, time);
                    trail.LookAt(targetTransform);
                    trail.position = path.point(t);
                    yield return null;
                }

                //spawn the hit
                Transform hitFx = _objectPool.Spawn(hitPrefab, targetTransform.position + offset);
                hitFx.rotation = Quaternion.LookRotation(casterTransform.position - targetTransform.position);
                hitFx.localScale = _hitScale;
                StartCoroutine(ScheduleRecycle(_hitLifecycle, hitFx));
            }
        }

        public IEnumerator ApplyDamage(ICharacterController target, int damage, bool isCritical)
        {
            float textScale = isCritical ? _criticalDamageTextScale : _damageTextScale;
            StartCoroutine(SpawnDamageText(target.Transform, damage, textScale));

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

        private IEnumerator SpawnDamageText(Transform target, int amount, float fontSize)
        {
            Transform damageFeedback = _objectPool.Spawn(_damageFeedback);
            StartCoroutine(ScheduleRecycle(_damageAnimationTime, damageFeedback));

            TMPro.TextMeshPro damageFeedbackText = damageFeedback.GetComponentInChildren<TMPro.TextMeshPro>();
            damageFeedbackText.color = amount > 0 ? _damageColor : _restoreColor;
            damageFeedbackText.fontSize = fontSize;
            damageFeedbackText.transform.localScale = target.lossyScale;
            damageFeedbackText.transform.rotation = target.transform.rotation;

            //animate the text
            damageFeedbackText.transform.position = new Vector3(target.position.x, target.position.y, target.position.z) + target.up * 20;
            Vector3 randomSpacing = new Vector3(Random.Range(-7, 7), Random.Range(20, 24), 0);
            damageFeedbackText.transform.Translate(randomSpacing);
            Vector3 startPos = damageFeedbackText.transform.localPosition;
            Vector3 targetPos = damageFeedbackText.transform.localPosition + new Vector3(0, Random.Range(8, 11), 0);

            for (float time = 0; time < _damageAnimationTime; time += Time.deltaTime)
            {
                float normalizedTime = Easings.Interpolate(time / _damageAnimationTime, _damageAnimationFunction);
                if (damageFeedbackText != null)
                {
                    damageFeedbackText.alpha = 1 - normalizedTime;
                    damageFeedbackText.transform.localPosition = Vector3.Lerp(startPos, targetPos, normalizedTime);

                    // Face Camera
                    damageFeedbackText.transform.LookAt(damageFeedbackText.transform.position +
                                _battleCamera.transform.rotation * Vector3.forward,
                                _battleCamera.transform.rotation * Vector3.up);
                }

                yield return null;
            }
        }

        private IEnumerator ScheduleRecycle(float lifecycle, Transform transform)
        {
            yield return new WaitForSeconds(lifecycle);
            _objectPool.Recycle(transform);
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
        //IEnumerator SpawnTrail(int degree, Transform caster, Transform target);
        //IEnumerator SpawnDamageText(Transform target, int amount, float scale);
    }
}