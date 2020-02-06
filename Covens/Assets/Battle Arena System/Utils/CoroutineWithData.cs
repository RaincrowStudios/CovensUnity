using System.Collections;
using UnityEngine;

public class Coroutine<T> : CustomYieldInstruction
{
    // Properties
    public override bool keepWaiting => _keepWaiting;
    public T ReturnValue { get; private set; }

    // Private variables    
    private Coroutine _coroutine;
    private IEnumerator _target;
    private bool _keepWaiting;

    public Coroutine(MonoBehaviour owner, IEnumerator target)
    {
        _keepWaiting = true;
        _target = target;
        _coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (_target.MoveNext())
        {
            ReturnValue = (T) _target.Current;
            yield return ReturnValue;
        }

        _keepWaiting = false;
    }
}

public static class CoroutineWithDataExtensions
{
    public static Coroutine<T> StartCoroutine<T>(this MonoBehaviour t, IEnumerator routine)
    {
        return new Coroutine<T>(t, routine);
    }
}

