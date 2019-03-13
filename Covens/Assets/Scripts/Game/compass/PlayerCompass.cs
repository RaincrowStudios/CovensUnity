using UnityEngine;

public class PlayerCompass : MonoBehaviour
{
    public Transform arrow;
    public Transform camTransform;

    void Start()
    {
        MapController.Instance.m_StreetMap.OnChangeZoom += () =>
        {
            arrow.localEulerAngles = new Vector3(0, 0, camTransform.localEulerAngles.y + 90);
        };
    }
}