using UnityEngine;
using UnityEngine.UI;

public class PlayerCompass : MonoBehaviour
{
    public static PlayerCompass instance { get; set; }
    public Transform arrow;
    private Transform centerPoint;

    private bool m_Animating = false;

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

        GetComponent<Button>().onClick.AddListener(OnClick);

        arrow.localEulerAngles = new Vector3(0, 0, centerPoint.localEulerAngles.y);
    }

    private void OnClick()
    {
        if (m_Animating)
            return;

        m_Animating = true;
        MapCameraUtils.SetRotation(
            0, 
            0.7f, 
            false,
            () =>  m_Animating = false
        );
    }
}