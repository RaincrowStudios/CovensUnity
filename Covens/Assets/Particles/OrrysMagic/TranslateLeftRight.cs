using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateLeftRight : MonoBehaviour
{
    public float m_Amount;
    public float m_Time;
    private Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.localPosition;
        Anim();
    }
    void OnEnable()
    {
        Anim();

    }
    void Anim()
    {
        var a = gameObject.GetComponent<ParticleSystem>();
        var b = a.emission.enabled;
        b = false;
        LeanTween.moveLocal(gameObject, pos, 0f);
        b = true;
        LeanTween.moveLocalX(gameObject, pos.x + m_Amount, m_Time).setEase(LeanTweenType.easeInOutCubic);
        /* LeanTween.value(0f, 1f, m_Time + 1f).setOnComplete(() =>
         {
             gameObject.SetActive(false);
             LeanTween.moveLocal(gameObject, pos, 0f).setOnComplete(() =>
             {
                 //gameObject.SetActive(true);
             });
         });*/
    }
}
