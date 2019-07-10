using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUITransition : MonoBehaviour
{
    [SerializeField] private RectTransform leftBar;
    [SerializeField] private RectTransform bottomBar;
    [SerializeField] private GameObject[] scaleObjects;
    [SerializeField] private float time = 1;
    [SerializeField] private LeanTweenType tweenType;
    [SerializeField] private CanvasGroup[] bars;
    [SerializeField] private RectTransform energy;
    [SerializeField] private MapCenterPointerUI mapPointer;
    [SerializeField] private GameObject channelingStats;


    [Header("Buttons")]
    [SerializeField] private Button m_SummonButton;
    [SerializeField] private Button m_ShoutButton;
    [SerializeField] private Button m_LocationButton;
    [SerializeField] private Button o_EnergyButton;



    public static MainUITransition Instance { get; set; }

    private int m_TweenId;
    public bool isChanneled;
    public bool CanShowUI
    {
        get
        {
            return PlaceOfPower.IsInsideLocation == false;
        }
    }

    void Awake()
    {
        Instance = this;

        PlaceOfPower.OnLeavePlaceOfPower += () => ShowMainUI(true);
        PlaceOfPower.OnEnterPlaceOfPower += () => HideMainUI(false);
        isChanneled = false; //Change this based on backend
    }

    public void OnLocationClick()
    {

        var k = Resources.Load<GameObject>("LocationManagerUI");
        Instantiate(k);

    }

    public void OnEnergyTap()
    {
        if (CanShowUI == false)
        {
            return;
        }
        if (isChanneled == true)
        {
            LeanTween.moveLocalY(channelingStats, 504f, 0.6f).setEase(LeanTweenType.easeInCubic);
            LeanTween.alphaCanvas(channelingStats.GetComponent<CanvasGroup>(), 1f, 0.4f);
        }

    }
    public void OnChannelingTap()
    {
        LeanTween.moveLocalY(channelingStats, 878f, 0.3f).setEase(LeanTweenType.easeOutCubic);
        LeanTween.alphaCanvas(channelingStats.GetComponent<CanvasGroup>(), 0f, 0.3f);
    }
    public void HideMainUI(bool moveEnergy = true)
    {
        Debug.Log("hide ui");
        LeanTween.cancel(m_TweenId);

        float leftBar_Start = leftBar.anchoredPosition.x;
        float leftBar_End = -150;

        float bottomBarAnchor_Start = bottomBar.anchoredPosition.y;
        float bottomBarAnchor_End = -160;

        float energyBarOffset_Start = energy.offsetMin.x;
        float energyBarOffset_End = moveEnergy ? -433 : 585;

        float startScale = scaleObjects[0].transform.localScale.x;
        float scale;

        float startAlpha = bars[0].alpha;
        float alpha;

        m_TweenId = LeanTween.value(0, 1, time).setEase(tweenType)
            .setOnUpdate((float t) =>
            {
                leftBar.anchoredPosition = new Vector2(Mathf.Lerp(leftBar_Start, leftBar_End, t), leftBar.anchoredPosition.y);
                bottomBar.anchoredPosition = new Vector2(bottomBar.anchoredPosition.x, Mathf.Lerp(bottomBarAnchor_Start, bottomBarAnchor_End, t));

                energy.offsetMin = new Vector2(Mathf.Lerp(energyBarOffset_Start, energyBarOffset_End, t), bottomBar.offsetMin.y);

                scale = Mathf.Lerp(startAlpha, 0, t);
                foreach (var item in scaleObjects)
                    item.transform.localScale = new Vector3(scale, scale, scale);

                alpha = Mathf.Lerp(startAlpha, 0, t);
                foreach (var item in bars)
                    item.alpha = alpha;
            })
            .uniqueId;

        mapPointer.EnablePointer(false);
    }

    public void ShowMainUI(bool moveEnergy = true)
    {
        if (CanShowUI == false)
            return;

        LeanTween.cancel(m_TweenId);

        float leftBar_Start = leftBar.anchoredPosition.x;
        float leftBar_End = 130;

        float bottomBarAnchor_Start = bottomBar.anchoredPosition.y;
        float bottomBarAnchor_End = 50;

        float energyBarOffset_Start = energy.offsetMin.x;
        float energyBarOffset_End = moveEnergy ? 585 : -433;

        float startScale = scaleObjects[0].transform.localScale.x;
        float scale;

        float startAlpha = bars[0].alpha;
        float alpha;

        m_TweenId = LeanTween.value(0, 1, time).setEase(tweenType)
            .setOnUpdate((float t) =>
            {
                leftBar.anchoredPosition = new Vector2(Mathf.Lerp(leftBar_Start, leftBar_End, t), leftBar.anchoredPosition.y);
                bottomBar.anchoredPosition = new Vector2(bottomBar.anchoredPosition.x, Mathf.Lerp(bottomBarAnchor_Start, bottomBarAnchor_End, t));

                energy.offsetMin = new Vector2(Mathf.Lerp(energyBarOffset_Start, energyBarOffset_End, t), bottomBar.offsetMin.y);

                scale = Mathf.Lerp(startScale, 1, t);
                foreach (var item in scaleObjects)
                    item.transform.localScale = new Vector3(scale, scale, scale);

                alpha = Mathf.Lerp(startAlpha, 1, t);
                foreach (var item in bars)
                    item.alpha = alpha;
            })
            .uniqueId;

        mapPointer.EnablePointer(true);
    }

    public void EnableSummonButton(bool enable)
    {
        m_SummonButton.interactable = enable;
    }

    public void EnableShoutButton(bool enable)
    {
        m_ShoutButton.interactable = enable;
    }
    public void EnableLocationButton(bool enable)
    {
        m_LocationButton.interactable = enable;
    }
}
