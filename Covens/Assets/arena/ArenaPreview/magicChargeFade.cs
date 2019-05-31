using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magicChargeFade : MonoBehaviour
{
    SpriteRenderer rune;
    private float t;
    // Start is called before the first frame update
    void Start()
    {
        rune = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        var t = rune.color.a;
        LeanTween.value(0f,1f,0.4f).setOnUpdate((float f) => {
            t = f;
            rune.color = new Color (rune.color.r, rune.color.g, rune.color.b, t);
        }).setOnComplete(() => {
            LeanTween.value(1f,0f,0.4f).setOnUpdate((float f) => {
            t = f;
            rune.color = new Color (rune.color.r, rune.color.g, rune.color.b, t);
            });
        });
            //.setOnComplete(() => {
        
       // });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
