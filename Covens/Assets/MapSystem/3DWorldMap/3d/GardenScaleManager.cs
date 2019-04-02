using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenScaleManager : MonoBehaviour
{

    Camera cam;
    public float minScale = .2f;
    public float maxScale = .6f;
    float minZoom;
    float maxZoom;
    public LineRenderer lr;
    // Use this for initialization
    void Start()
    {
        cam = SpriteMapsController.instance.m_Camera;
        var sM = SpriteMapsController.instance;
        minZoom = sM.m_MinZoom;
        maxZoom = sM.m_MaxZoom;
        sM.onChangeZoom += updateGardenScale;
        sM.onChangePosition += updateGardenScale;
        lr.positionCount = transform.childCount;
        foreach (Transform item in transform)
        {
            //			Debug.Log (item.GetSiblingIndex ());
            lr.SetPosition(item.GetSiblingIndex(), item.localPosition);
        }
    }

    // Update is called once per frame
    void updateGardenScale()
    {
        float sMultiplier = MapUtils.scale(minScale, maxScale, minZoom, maxZoom, cam.orthographicSize);
        float lineWidth = Mathf.Clamp(cam.orthographicSize, 2, maxZoom);
        float lineThickness = MapUtils.scale(.08f, 0, maxZoom, 2, lineWidth);
        lr.widthMultiplier = lineThickness;
        foreach (Transform item in transform)
        {
            item.localScale = Vector3.one * sMultiplier;
        }
    }
}
