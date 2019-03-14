using UnityEngine;
using UnityEngine.UI;

public class PlayerCompass : MonoBehaviour
{
    public Transform arrow;
    public Transform camTransform;

    void Start()
    {
        MapController.Instance.m_StreetMap.OnChangeZoom += () =>
        {
            arrow.localEulerAngles = new Vector3(0, 0, camTransform.localEulerAngles.y);
        };

        GetComponent<Button>().onClick.AddListener(() =>
        {
            LeanTween.value(camTransform.localRotation.eulerAngles.y, 0, .7f).setEase(LeanTweenType.easeInOutQuad).setOnUpdate((float v) =>
            {
                camTransform.localRotation = Quaternion.Euler(camTransform.localRotation.eulerAngles.x, v, 0);
                arrow.localEulerAngles = new Vector3(0, 0, camTransform.localEulerAngles.y);
            });

        });
    }


}