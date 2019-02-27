using UnityEngine;

public class MainUIShift : MonoBehaviour
{
    [SerializeField] private GameObject leftBar;
    [SerializeField] private GameObject bottomBar;
    [SerializeField] private GameObject[] scaleObjects;
    [SerializeField] private float time = 1;
    [SerializeField] private LeanTweenType tweenType;

    public static MainUIShift Instance { get; set; }

    void Awake()
    {
        Instance = this;
    }

    public void HideMainUI()
    {
        LeanTween.moveX(leftBar, -150, time).setEase(tweenType);
        LeanTween.moveY(bottomBar, -115, time).setEase(tweenType);
        foreach (var item in scaleObjects)
        {
            LeanTween.scale(item, Vector3.zero, time).setEase(tweenType);
        }
    }
    public void ShowMainUI()
    {
        LeanTween.moveX(leftBar, 130, time).setEase(tweenType);
        LeanTween.moveY(bottomBar, 50, time).setEase(tweenType);
        foreach (var item in scaleObjects)
        {
            LeanTween.scale(item, Vector3.one, time).setEase(tweenType);
        }
    }

}