using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellcastingTrailFX : MonoBehaviour
{
    //shadow
    private static SimplePool<Transform> m_ShadowCharge = new SimplePool<Transform>("SpellFX/Trails/Shadow/MagicCharge");
    private static SimplePool<Transform> m_ShadowTrail = new SimplePool<Transform>("SpellFX/Trails/Shadow/PortalCast");
    private static SimplePool<Transform> m_ShadowHit = new SimplePool<Transform>("SpellFX/Trails/Shadow/MagicHit");

    //gray
    private static SimplePool<Transform> m_GrayCharge = new SimplePool<Transform>("SpellFX/Trails/Gray/MagicCharge");
    private static SimplePool<Transform> m_GrayTrail = new SimplePool<Transform>("SpellFX/Trails/Gray/PortalCast");
    private static SimplePool<Transform> m_GrayHit = new SimplePool<Transform>("SpellFX/Trails/Gray/MagicHit");

    //light
    private static SimplePool<Transform> m_LightCharge = new SimplePool<Transform>("SpellFX/Trails/Light/MagicCharge");
    private static SimplePool<Transform> m_LightTrail = new SimplePool<Transform>("SpellFX/Trails/Light/PortalCast");
    private static SimplePool<Transform> m_LightHit = new SimplePool<Transform>("SpellFX/Trails/Light/MagicHit");

    public static void SpawnTrail(int degree, IMarker caster, IMarker target, System.Action onComplete)
    {
        if (caster == null || target == null || caster.isNull || target.isNull)
            LeanTween.value(0, 0, 1f).setOnComplete(onComplete);
        else
            SpawnTrail(degree, caster.characterTransform, target.characterTransform, onComplete);
    }

    public static void SpawnTrail(int degree, Transform caster, Transform target, System.Action onComplete)
    {
        SimplePool<Transform> chargeFxPool, trailFxPool, hitFxPool;

        if (degree < 0)
        {
            chargeFxPool = m_ShadowCharge;
            trailFxPool = m_ShadowTrail;
            hitFxPool = m_ShadowHit;
        }
        else if (degree > 0)
        {
            chargeFxPool = m_LightCharge;
            trailFxPool = m_LightTrail;
            hitFxPool = m_LightHit;
        }
        else
        {
            chargeFxPool = m_GrayCharge;
            trailFxPool = m_GrayTrail;
            hitFxPool = m_GrayHit;
        }

        Vector3 offset = target.up * 40;

        //spawn the charge
        chargeFxPool.Spawn(caster.position + offset, 2f).transform.localScale = new Vector3(5, 5, 5);
        LeanTween.value(0, 1, 0.25f)
            .setOnComplete(() =>
            {
                //spawn the trail
                Transform trail = trailFxPool.Spawn(caster.position + offset, 8f);
                trail.localScale = new Vector3(4, 4, 4);
                int tweenId = -1;
                var u = Vector2.Distance(new Vector2 (caster.position.x, caster.position.y), new Vector2(target.position.x, target.position.y));                       // MapsAPI.Instance.DistanceBetweenPointsD(new Vector2 (caster.position.x, caster.position.y), new Vector2(target.position.x, target.position.y));
                var dist = (MapUtils.scale(0.5f,1.5f, 0f, 1000f, (float)u));
                Debug.Log("float u: " + (float)u);
                Debug.Log("dist: " + dist);
                //var dist = Mathf.Abs(((caster.position.x * target.position.x)/2f) + ((caster.position.y * target.position.y)/2f)); 
                tweenId = LeanTween.value(0, 1, dist) //time for casting
                    .setEaseInExpo()
                    .setOnUpdate((float t) =>
                    {
                        if (target == null || caster == null)
                        {
                            LeanTween.cancel(tweenId, true);
                            return;
                        }
                        //animate the trail
                        trail.LookAt(target);
                        trail.position = Vector3.Lerp(caster.position + offset, target.position + offset, t);
                    })
                    .setOnComplete(() =>
                    {
                        //spawn the hit
                        if (caster != null && target != null)
                        {
                            Transform hitFx = hitFxPool.Spawn(target.position + offset, 2f);
                            hitFx.rotation = Quaternion.LookRotation(caster.position - target.position);
                            hitFx.localScale = new Vector3(4, 4, 4);
                        }
                        onComplete?.Invoke();
                    }).uniqueId;
            });
    }
}
