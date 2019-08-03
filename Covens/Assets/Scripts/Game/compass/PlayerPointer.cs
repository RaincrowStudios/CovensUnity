using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPointer : MonoBehaviour
{
    public Transform pointer2d;
    private float offset = 25;
    // float angle;
    public CanvasGroup alphaCanvas;
    public CanvasGroup glowFX;
    Vector3 dir;
    bool isVisible = false;

    private Transform centerPointTransform;

    void Start()
    {
        centerPointTransform = MapsAPI.Instance.mapCenter;
        MapsAPI.Instance.OnChangePosition += UpdateRotation;
        isVisible = true;
    }


    // Update is called once per frame
    void UpdateRotation()
    {
        var marker = PlayerManager.marker.GameObject.transform;
        var m_Distance = Vector2.Distance(
                   new Vector2(centerPointTransform.position.x, centerPointTransform.position.z), new Vector2(marker.position.x, marker.position.z));

        if (m_Distance > 100)
        {
            if (!isVisible)
            {
                glowFX.alpha = 0;
                LeanTween.alphaCanvas(alphaCanvas, 1, .4f);
                LeanTween.alphaCanvas(glowFX, 1, .3f).setOnComplete(() =>
                {
                    LeanTween.alphaCanvas(glowFX, 0, .3f);
                });
                isVisible = true;
            }

            dir = marker.position - centerPointTransform.position;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            pointer2d.localEulerAngles = new Vector3(0, 0, -angle + offset);

        }
        else
        {
            if (isVisible)
            {
                LeanTween.alphaCanvas(alphaCanvas, 0, .3f);
                isVisible = false;
            }
        }


    }
}
