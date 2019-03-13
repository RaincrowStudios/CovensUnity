using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPointer : MonoBehaviour
{
    public Transform centerPoint;
    public Transform centerPointTransform;
    public Transform pointer2d;
    public float offset = 0;
    // float angle;
    public CanvasGroup alphaCanvas;
    public CanvasGroup glowFX;
    Vector3 dir;
    bool isVisible = false;

    void Start()
    {
        centerPoint = centerPointTransform.parent;
        MapController.Instance.m_StreetMap.OnChangePosition += UpdateRotation;
    }


    // Update is called once per frame
    void UpdateRotation()
    {
        var marker = PlayerManager.marker.gameObject.transform;
        var m_Distance = Vector2.Distance(
                   new Vector2(centerPoint.position.x, centerPoint.position.z), new Vector2(marker.position.x, marker.position.z));

        if (m_Distance > 50)
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
