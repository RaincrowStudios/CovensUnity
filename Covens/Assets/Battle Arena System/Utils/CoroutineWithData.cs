using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coroutine<T> : CustomYieldInstruction
{
    // Properties
    public override bool keepWaiting => _keepWaiting;
    public T ReturnValue { get; private set; }

    // Private variables    
    private Coroutine _coroutine;
    private IEnumerator<T> _target;
    private bool _keepWaiting;

    public Coroutine(MonoBehaviour owner, IEnumerator<T> target)
    {
        _keepWaiting = true;
        _target = target;
        _coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (_target.MoveNext())
        {
            ReturnValue = _target.Current;
            yield return ReturnValue;
        }

        _keepWaiting = false;
    }
}

public static class CoroutineExtensions
{
    public static Coroutine<T> StartCoroutine<T>(this MonoBehaviour behaviour, IEnumerator<T> routine)
    {
        return new Coroutine<T>(behaviour, routine);
    }
}

