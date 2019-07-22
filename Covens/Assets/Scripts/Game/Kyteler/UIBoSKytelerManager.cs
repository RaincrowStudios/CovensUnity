using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UIBoSKytelerManager : MonoBehaviour
{
    public Button openInfo;
    public Button back;
    public GameObject RingInfo;
    public float closedXLocal = 2160f;
    public float openXLocal = -65f;

    // Start is called before the first frame update
    void Start()
    {
        openInfo.onClick.AddListener(AnimOpen);
        back.onClick.AddListener(AnimClose);
    }

    // Update is called once per frame
    public void AnimOpen()
    {
        LeanTween.moveLocalX(RingInfo, openXLocal, 0.8f).setEase(LeanTweenType.easeInCubic);
    }
    public void AnimClose()
    {
        LeanTween.moveLocalX(RingInfo, closedXLocal, 0.5f).setEase(LeanTweenType.easeOutCubic);
    }
}
