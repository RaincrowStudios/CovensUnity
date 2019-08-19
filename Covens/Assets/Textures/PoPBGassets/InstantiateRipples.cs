using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateRipples : MonoBehaviour
{
    public float distance;
    public GameObject ripple;
    public float scale;
    // Start is called before the first frame update
    void Start()
    {
        // Timer();
    }
    void Timer()
    {
        // LeanTween.value(0f, 1f, Random.Range(0.2f, 1f)).setOnComplete(() =>
        // {
        //     Spawn();
        //     Timer();
        // });
    }

    // Update is called once per frame
    void Spawn()
    {
        // if (ripple != null)
        // {
        //     var p = Utilities.InstantiateObject(ripple, transform, scale);
        //     LeanTween.moveLocal(p, (new Vector3(Random.Range(-distance, distance), Random.Range(-distance, distance), 0)), 0.01f);
        // }
    }
}
