using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeOutDelay : MonoBehaviour
{

    public float fadeDelay = 5;
    public float fadeSpeed = 1;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(fadeDelay);
        CanvasGroup cg = GetComponent<CanvasGroup>();
        LeanTween.value(0f, 1f, fadeDelay).setOnComplete(() =>
        {
            LeanTween.alphaCanvas(cg, 0f, fadeSpeed);
        });
        /*		float t = 0;
                while (t <= 1) {
                    t += Time.deltaTime;
                    cg.alpha = Mathf.SmoothStep (1, 0, t);
                    yield return 0;
                }*/
    }
}

