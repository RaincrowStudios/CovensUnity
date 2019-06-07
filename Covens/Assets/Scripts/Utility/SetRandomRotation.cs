using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRandomRotation : MonoBehaviour
{
    void Start()
    {
        var p = this.transform.rotation;
        var q = p;
        q.z = (Random.Range(-360f,360f))/(Random.Range(2f,7f))*5f;
        p = q;
        var t = gameObject.AddComponent<Rotate>();
        t.rotationSpeed = Random.Range(5f, 8f);
    /*var r = gameObject.GetComponent<Rotate>();
    r.rotationSpeed = Random.Range(5f, 8f);
    r.enabled = true;*/
    }
}
