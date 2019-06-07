using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRandomRotation : MonoBehaviour
{
    void Start()
    {
        transform.Rotate(0, 0, Random.Range(0, 360));
        gameObject.AddComponent<Rotate>().rotationSpeed = Random.Range(5f, 8f);
    }
}
