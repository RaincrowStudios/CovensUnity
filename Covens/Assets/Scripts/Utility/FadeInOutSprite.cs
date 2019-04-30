using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeInOutSprite : MonoBehaviour
{


    public float frequency = 1f;
    SpriteRenderer sp;
    // Use this for initialization
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, Mathf.Abs(Mathf.Sin(Time.time * frequency)));
    }
}
