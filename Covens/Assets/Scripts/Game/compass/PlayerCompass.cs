using UnityEngine;
using UnityEngine.UI;

public class PlayerCompass : MonoBehaviour
{
    public static PlayerCompass instance { get; set; }
    public Transform arrow;
    private Transform centerPoint;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        centerPoint = MapsAPI.Instance.mapCenter;

        MapsAPI.Instance.OnChangeRotation += () =>
        {
            arrow.localEulerAngles = new Vector3(0, 0, centerPoint.localEulerAngles.y);
        };

        GetComponent<Button>().onClick.AddListener(() =>
        {
            LeanTween.value(centerPoint.localRotation.eulerAngles.y, 0, .7f).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float v) =>
            {
                centerPoint.localRotation = Quaternion.Euler(centerPoint.localRotation.eulerAngles.x, v, 0);
                arrow.localEulerAngles = new Vector3(0, 0, centerPoint.localEulerAngles.y);
            });

        });
    }

    public void FTFCompass(float f)
    {
        arrow.localEulerAngles = new Vector3(0, 0, f);
    }

}