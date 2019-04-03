using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LabelRequestManager : MonoBehaviour
{
    SpriteMapsController spriteMaps;
    GetLabels getLabels;
    const double DEG2RAD = Math.PI / 180;
    const double R = 6371;
    Camera cam;
    Vector2 previousVec = Vector2.zero;
    int previousZoom = 0;
    public DynamicLabelManager dynamicLabelManager;

    private Vector2 m_LastCoordinates;

    private void Start()
    {
        spriteMaps = SpriteMapsController.instance;
        getLabels = GetLabels.instance;
        cam = MapController.Instance.m_WorldMap.camera;
    }

    private void OnEnable()
    {
        //MapController.Instance.m_WorldMap.OnChangePosition += CheckRequest;
        StartCoroutine(GetMarkersCoroutine());
    }

    private void OnDisable()
    {
        //MapController.Instance.m_WorldMap.OnChangePosition -= CheckRequest;
        StopAllCoroutines();
    }

    void CheckRequest(float lng, float lat)
    {
        //float distance = MapUtils.scale(.005f, .1f, .01f, .3f, cam.orthographicSize);
        //float actualDistance = Vector3.Distance(cam.transform.position, previousVec);
        //if (actualDistance > distance)
        //{
        //    int requestDistance = (int)MapUtils.scale(2000, 130000, .01f, .3f, cam.orthographicSize);
        //    previousVec = cam.transform.position;
            GetLabels.instance.RequestLabel(new Vector2(lng, lat), 10000);
        //}
    }

    private IEnumerator GetMarkersCoroutine()
    {
        while (cam == null)
            yield return 1;

        while (true)
        {
            if (cam.orthographicSize <= .3f)
            {
                double lng, lat;
                MapController.Instance.m_WorldMap.GetPosition(out lng, out lat);
                double distance = MapsAPI.Instance.DistanceBetweenPointsD(m_LastCoordinates, new Vector2((float)lng, (float)lat));

                if (distance > 2)
                {
                    m_LastCoordinates = new Vector2((float)lng, (float)lat);
                    CheckRequest(m_LastCoordinates.x, m_LastCoordinates.y);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}

