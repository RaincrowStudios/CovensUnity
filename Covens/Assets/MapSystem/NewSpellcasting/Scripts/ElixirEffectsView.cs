using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElixirEffectsView : MonoBehaviour
{
    [Header("Basic configs")]
    [SerializeField] private GameObject _shadowEffect;
    [SerializeField] private ParticleSystem _runeEffect;
    [SerializeField] private ParticleSystem _raysEffect;

    [Header("Effect Normal")]
    [SerializeField] private int _maxParticlesNormal = 25;
    [SerializeField] private Color _colorNormal = Color.white;

    [Header("Effect Reduced")]
    [SerializeField] private int _maxParticlesReduced = 10;
    [SerializeField] private Color _colorReduced = Color.white;

    /// <summary>
    /// Update elixir's visual effects
    /// </summary>
    /// <param name="index">The effect's index in Elixirs Dicitionary</param>
    /// <param name="multiple">If player consumed more than two elixirs</param>
    public void UpdateEffect(int index, bool multiple)
    {
        ParticleSystem.MainModule runeMain = _runeEffect.main;
        ParticleSystem.MainModule raysMain = _raysEffect.main;

        raysMain.maxParticles = multiple ? _maxParticlesReduced : _maxParticlesNormal;

        if (index == 0)
        {
            runeMain.startColor = new ParticleSystem.MinMaxGradient(multiple ? _colorReduced : _colorNormal);
            _runeEffect.gameObject.SetActive(true);
            _shadowEffect.SetActive(true);
        }
        else
        {
            _shadowEffect.SetActive(false);
            _runeEffect.gameObject.SetActive(!multiple);
        }
    }

}
