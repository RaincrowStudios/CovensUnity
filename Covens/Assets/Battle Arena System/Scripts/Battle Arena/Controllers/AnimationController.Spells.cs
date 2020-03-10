using UnityEngine;

namespace Raincrow.BattleArena.Controllers
{
    public partial class AnimationController : MonoBehaviour
    {
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

        [Header("Damage Text Prefab")]
        [SerializeField] private Transform _damageFeedback;
        [SerializeField] private Color _damageColor;
        [SerializeField] private Color _restoreColor;

        [Header("Animation Scales")]
        [SerializeField] private Vector3 _chargeScale = new Vector3(20, 20, 20);
        [SerializeField] private Vector3 _trailScale = new Vector3(15, 15, 15);
        [SerializeField] private Vector3 _hitScale = new Vector3(20, 20, 20);

        public void SpawnTrail(int degree, Transform caster, Transform target, System.Action onStart, System.Action onComplete)
        {
            if (caster == null || target == null) // || caster.isNull || target.isNull)
            {
                LeanTween.value(0, 0, 0.1f)
                    .setOnStart(onStart)
                    .setOnComplete(onComplete);
            }
            else
            {
                LeanTween.value(0, 0, 0.15f).setOnComplete(onStart);

                Transform chargePrefab, trailPrefab, hitPrefab;

                if (degree < 0)
                {
                    chargePrefab = _shadowChargePrefab;
                    trailPrefab = _shadowTrailPrefab;
                    hitPrefab = _shadowHitPrefab;
                }
                else if (degree > 0)
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

                Vector3 offset = target.up * 40;
                float distance = Vector3.Distance(caster.position, target.position);
                float trailTime = 0.25f;

                Vector3 startPosition = caster.position + offset;
                Vector3 targetPosition = target.position + offset;

                //spawn the charge
                Transform charge = _objectPool.Spawn(chargePrefab, null, caster.position + offset, chargePrefab.transform.rotation);
                charge.localScale = _chargeScale;

                //just call on complete if the caster is casting on itself
                if (caster == target)
                {
                    LeanTween.value(0, 0, 0.25f).setOnComplete(onComplete);
                    return;
                }

                LeanTween.value(0, 1, 0.25f)
                    .setOnComplete(() =>
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
                        Transform trail = _objectPool.Spawn(trailPrefab, caster.position + offset);
                        trail.localScale = _trailScale;
                        int tweenId = -1;


                        tweenId = LeanTween.value(0, 1, trailTime) //time for casting
                                                                   //.setEaseOutExpo()
                            .setOnStart(() =>
                            {
                                if (target == null || caster == null)
                                {
                                    LeanTween.cancel(tweenId, true);
                                    return;
                                }
                                //animate the trail
                                trail.LookAt(target);
                                trail.position = path.point(0);// Vector3.Lerp(caster.position + offset, target.position + offset, t);
                            })
                            .setOnUpdate((float t) =>
                            {
                                if (target == null || caster == null)
                                {
                                    LeanTween.cancel(tweenId, true);
                                    return;
                                }
                                //animate the trail
                                trail.LookAt(target);
                                trail.position = path.point(t);// Vector3.Lerp(caster.position + offset, target.position + offset, t);
                            })
                            .setOnComplete(() =>
                            {
                                //spawn the hit
                                if (caster != null && target != null)
                                {
                                    Transform hitFx = _objectPool.Spawn(hitPrefab, target.position + offset);
                                    hitFx.rotation = Quaternion.LookRotation(caster.position - target.position);
                                    hitFx.localScale = _hitScale;
                                }
                                onComplete?.Invoke();
                            }).uniqueId;
                    });
            }
        }

        public void SpawnDamageText(Transform target, int amount, float fontSize)
        {
            string color = string.Empty;
            if (amount > 0)
            {
                color = ColorUtility.ToHtmlStringRGB(_damageColor);
            }
            else
            {
                color = ColorUtility.ToHtmlStringRGB(_restoreColor);
            }

            TMPro.TextMeshPro damageFeedback = _objectPool.Spawn(_damageFeedback).GetComponentInChildren<TMPro.TextMeshPro>();
            damageFeedback.text = $"<color=#{color}>{amount}</color>";
            damageFeedback.fontSize = fontSize;
            damageFeedback.transform.localScale = target.lossyScale;
            damageFeedback.transform.rotation = target.transform.rotation;

            //animate the text
            damageFeedback.transform.position = new Vector3(target.position.x, target.position.y, target.position.z) + target.up * 20;
            var RandomSpacing = new Vector3(Random.Range(-7, 7), Random.Range(20, 24), 0);
            damageFeedback.transform.Translate(RandomSpacing);
            Vector3 startPos = damageFeedback.transform.localPosition;
            Vector3 targetPos = damageFeedback.transform.localPosition + new Vector3(0, Random.Range(8, 11), 0);

            LeanTween.value(0, 1, 2f)
                    .setOnUpdate((float t) =>
                    {
                        if (damageFeedback != null)
                        {
                            damageFeedback.alpha = 1 - t;
                            damageFeedback.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);

                            // Face Camera
                            damageFeedback.transform.LookAt(damageFeedback.transform.position + 
                                        _battleCamera.transform.rotation * Vector3.forward,
                                        _battleCamera.transform.rotation * Vector3.up);
                        }                        
                    })
                    .setEaseOutCubic();
        }
    }
}