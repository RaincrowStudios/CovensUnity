using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BOSController : BOSBase
{
    public static BOSController Instance { get; set; }
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
    private Button closeBtn;

    private static int m_TweenId;
    private static int m_RibbonTweenId;

    void Awake()
    {
        Instance = this;
        closeBtn = GetComponentInChildren<Button>();
        characterRibbon.GetComponent<Button>().onClick.AddListener(() => { OnClickRibbon(0); });
        spellRibbon.GetComponent<Button>().onClick.AddListener(() => { OnClickRibbon(1); });
        spiritRibbon.GetComponent<Button>().onClick.AddListener(() => { OnClickRibbon(2); });
        closeBtn.onClick.AddListener(Close);
    }

    public void CloseDefault()
    {
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(Close);
    }

    public void AssignCloseListner(Action closeAction)
    {
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(() => { closeAction(); });
    }

    public void Close()
    {
        BackButtonListener.RemoveCloseAction();

        MapsAPI.Instance.HideMap(false);
        //UIStateManager.Instance.CallWindowChanged(true);

        //LeanTween.alphaCanvas(CG, 0, .65f);
        //LeanTween.scale(gameObject, Vector3.zero, .65f).setEase(LeanTweenType.easeOutQuint).setOnComplete(() => { Destroy(gameObject); });

        LeanTween.cancel(m_TweenId);
        LeanTween.cancel(m_RibbonTweenId);
        LeanTween.cancel(m_CallbackTweenId);

        m_TweenId = LeanTween.value(1, 0, 0.65f)
            .setOnUpdate((float t) =>
            {
                CG.alpha = t;
                transform.localScale = Vector3.one * LeanTween.easeOutQuint(0, 1, t);
            })
            .setOnComplete(() => Destroy(gameObject))
            .uniqueId;
    }

    void Start()
    {
        CG = GetComponent<CanvasGroup>();
        CG.alpha = 0;
        OnClickRibbon(0);
        
        LeanTween.cancel(m_TweenId);
        m_TweenId = LeanTween.value(0, 1, 0.65f)
            .setOnUpdate((float t) =>
            {
                CG.alpha = t;
            })
            .setOnComplete(() =>
            {
                MapsAPI.Instance.HideMap(true);
            })
            .uniqueId;

        BackButtonListener.AddCloseAction(Close);
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

    private int m_AnimTweenId;
    private int m_CallbackTweenId;

    void AnimateRibbon(RectTransform rt, int pos)
    {
        var k = LeanTween.descr(LeanTween.moveX(rt, pos, ribbonAnimTime).id).setEase(easeOutType);
        m_RibbonTweenId = k.uniqueId;
    }

    void AnimateRibbon(RectTransform rt, int pos, Action callback)
    {
        LeanTween.cancel(m_CallbackTweenId);
        m_CallbackTweenId = LeanTween.value(0, 0, ribbonAnimTime / 2f).setOnComplete(callback).uniqueId;

        var k = LeanTween.descr(LeanTween.moveX(rt, pos, ribbonAnimTime).id).setEase(easeOutType);
        m_RibbonTweenId = k.uniqueId;
    }
}


