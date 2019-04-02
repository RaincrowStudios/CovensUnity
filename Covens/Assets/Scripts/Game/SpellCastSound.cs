using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCastSound : MonoBehaviour
{

    public static SpellCastSound Instance { get; set; }
    public AudioSource w1;
    public AudioSource w2;
    public float multiplier = 2;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    void Update()
    {
        w1.volume = Mathf.Abs(Input.GetAxis("Mouse X") * multiplier);
        w2.volume = Mathf.Abs(Input.GetAxis("Mouse Y") * multiplier);
        Debug.Log(Input.GetAxis("Mouse X") / Time.deltaTime);
    }

}
