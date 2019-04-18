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

    [Header("Buttons")]
    [SerializeField] private Button m_SummonButton;
    [SerializeField] private Button m_ShoutButton;

    public static MainUITransition Instance { get; set; }
    bool forward;
    void Awake()
    {
        Instance = this;
    }


    public void HideMainUI()
    {
        LeanTween.value(130.0f, -150, time).setEase(tweenType).setOnUpdate((float f) =>
        {
            leftBar.anchoredPosition = new Vector2(f, leftBar.anchoredPosition.y);
        });

        LeanTween.value(50.0f, -115, time).setEase(tweenType).setOnUpdate((float f) =>
        {
            bottomBar.anchoredPosition = new Vector2(bottomBar.anchoredPosition.x, f);
        });

        LeanTween.value(585, -433, time).setEase(tweenType).setOnUpdate((float f) =>
          {
              energy.offsetMin = new Vector2(f, bottomBar.offsetMin.y);
          });


        foreach (var item in scaleObjects)
        {
            LeanTween.scale(item, Vector3.zero, time).setEase(tweenType);
        }

        foreach (var item in bars)
        {
            LeanTween.alphaCanvas(item, 0, time).setEase(tweenType);
        }
    }
    public void ShowMainUI()
    {
        LeanTween.value(-150.0f, 130, time).setEase(tweenType).setOnUpdate((float f) =>
         {
             leftBar.anchoredPosition = new Vector2(f, leftBar.anchoredPosition.y);
         });

        LeanTween.value(-433, 585, time).setEase(tweenType).setOnUpdate((float f) =>
        {
            energy.offsetMin = new Vector2(f, bottomBar.offsetMin.y);
        });

        LeanTween.value(-115f, 50, time).setEase(tweenType).setOnUpdate((float f) =>
        {
            bottomBar.anchoredPosition = new Vector2(bottomBar.anchoredPosition.x, f);
        });
        foreach (var item in scaleObjects)
        {
            LeanTween.scale(item, Vector3.one, time).setEase(tweenType);
        }
        foreach (var item in bars)
        {
            LeanTween.alphaCanvas(item, 1, time).setEase(tweenType);
        }
    }

    public void EnableSummonButton(bool enable)
    {
        //m_SummonButton.interactable = enable;
    }

    public void EnableShoutButton(bool enable)
    {
        //m_ShoutButton.interactable = enable;
    }
}
