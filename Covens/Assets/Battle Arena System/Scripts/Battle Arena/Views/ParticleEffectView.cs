using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectView : MonoBehaviour, IParticleEffectView
{
    [SerializeField] float _normalYScale = 9.6f;
    [SerializeField] float _timeAnimation = 0.5f;
    public Transform Transform { get { return gameObject.transform; } }

    public void DestroyEffect()
    {
        LeanTween.scaleY(gameObject, 0.0f, _timeAnimation)
            .setOnComplete(()=> { Destroy(gameObject); });
    }

    public IEnumerator ShowEffect()
    {
        yield return new WaitForSeconds(LeanTween.scaleY(gameObject, _normalYScale, _timeAnimation).time);
    }
}

public interface IParticleEffectView {
    IEnumerator ShowEffect();
    void DestroyEffect();

    Transform Transform { get; }
}

