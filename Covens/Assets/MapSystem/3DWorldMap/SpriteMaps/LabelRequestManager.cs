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
    // Use this for initialization
    void Start()
    {
        spriteMaps = SpriteMapsController.instance;
        getLabels = GetLabels.instance;
        MapController.Instance.m_WorldMap.OnChangePosition += CheckRequest;
        cam = MapController.Instance.m_WorldMap.camera;
    }

    void CheckRequest()
    {

        if (cam.orthographicSize <= .3f)
        {
            float distance = MapUtils.scale(.005f, .1f, .01f, .3f, cam.orthographicSize);
            float actualDistance = Vector3.Distance(cam.transform.position, previousVec);
            if (actualDistance > distance)
            {
                int requestDistance = (int)MapUtils.scale(2000, 130000, .01f, .3f, cam.orthographicSize);
                previousVec = cam.transform.position;
                GetLabels.instance.RequestLabel(SpriteMapsController.mapCenter, requestDistance);
            }
        }
    }



    static double DistanceBetweenPointsD(Vector2 point1, Vector2 point2)
    {
        double scfY = Math.Sin(point1.y * DEG2RAD);
        double sctY = Math.Sin(point2.y * DEG2RAD);
        double ccfY = Math.Cos(point1.y * DEG2RAD);
        double cctY = Math.Cos(point2.y * DEG2RAD);
        double cX = Math.Cos((point1.x - point2.x) * DEG2RAD);
        double sizeX1 = Math.Abs(R * Math.Acos(scfY * scfY + ccfY * ccfY * cX));
        double sizeX2 = Math.Abs(R * Math.Acos(sctY * sctY + cctY * cctY * cX));
        double sizeX = (sizeX1 + sizeX2) / 2.0;
        double sizeY = R * Math.Acos(scfY * sctY + ccfY * cctY);
        if (double.IsNaN(sizeY)) sizeY = 0;
        return Math.Sqrt(sizeX * sizeX + sizeY * sizeY);
    }

}
