using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BOSController : BOSBase
{
    [SerializeField] private RectTransform characterRibbon;
    [SerializeField] private RectTransform spellRibbon;
    [SerializeField] private RectTransform spiritRibbon;
    [SerializeField] private float ribbonAnimTime;
    [SerializeField] private LeanTweenType easeOutType;
    [SerializeField] private GameObject characterScreen;
    [SerializeField] private GameObject spellScreen;
    [SerializeField] private GameObject spiritScreen;
    private GameObject currentObject;
    private CanvasGroup CG;
    void Awake()
    {
        characterRibbon.GetComponent<Button>().onClick.AddListener(() => { OnClickRibbon(0); });
        spellRibbon.GetComponent<Button>().onClick.AddListener(() => { OnClickRibbon(1); });
        spiritRibbon.GetComponent<Button>().onClick.AddListener(() => { OnClickRibbon(2); });
        GetComponentInChildren<Button>().onClick.AddListener(Close);
    }
    void Close()
    {
        LeanTween.alphaCanvas(CG, 0, .65f);
        LeanTween.scale(gameObject, Vector3.zero, .65f).setEase(LeanTweenType.easeOutQuint).setOnComplete(() => { Destroy(gameObject); });
    }
    void Start()
    {
        CG = GetComponent<CanvasGroup>();
        CG.alpha = 0;
        LeanTween.alphaCanvas(CG, 1, .8f);
        OnClickRibbon(0);
    }
    void ShowCharacter()
    {
        DestroyPrevious(currentObject);
        currentObject = CreateScreen(characterScreen);
    }


    void ShowSpells()
    {
        DestroyPrevious(currentObject);
        currentObject = CreateScreen(spellScreen);

    }
    void ShowSpirits()
    {
        DestroyPrevious(currentObject);
        currentObject = CreateScreen(spiritScreen);
    }

    void OnClickRibbon(int index)
    {
        if (index == 0)
        {
            AnimateRibbon(spellRibbon, -117);
            AnimateRibbon(spiritRibbon, -117);
            AnimateRibbon(characterRibbon, 137, ShowCharacter);
        }
        else if (index == 1)
        {
            AnimateRibbon(characterRibbon, -117);
            AnimateRibbon(spiritRibbon, -117);
            AnimateRibbon(spellRibbon, 137, ShowSpells);
        }
        else
        {
            AnimateRibbon(spellRibbon, -117);
            AnimateRibbon(characterRibbon, -117);
            AnimateRibbon(spiritRibbon, 137, ShowSpirits);
        }

    }

    void AnimateRibbon(RectTransform rt, int pos)
    {
        var k = LeanTween.descr(LeanTween.moveX(rt, pos, ribbonAnimTime).id).setEase(easeOutType);
    }
    void AnimateRibbon(RectTransform rt, int pos, Action callback)
    {
        var k = LeanTween.descr(LeanTween.moveX(rt, pos, ribbonAnimTime).id).setOnComplete(callback).setEase(easeOutType);
    }
}


