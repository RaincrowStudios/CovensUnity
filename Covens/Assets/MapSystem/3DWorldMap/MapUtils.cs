using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapUtils : MonoBehaviour
{

    //     latitude    = 41.145556; // (φ)
    // longitude   = -73.995;   // (λ)

    // float mapWidth = 20.48f;
    //     float mapHeight = 20.48f;

    //     // get x value
    //     x = (longitude+180)*(mapWidth/360)

    // // convert from degrees to radians
    // latRad = latitude* PI/180;

    //     // get y value
    //     mercN = ln(tan((PI/4)+(latRad/2)));
    // y     = (mapHeight/2)-(mapWidth* mercN/(2*PI));
    /// <summary>
    /// Degrees-to-radians conversion constant.
    /// </summary>
    public const double DEG2RAD = Math.PI / 180;

    /// <summary>
    /// PI * 2
    /// </summary>
    public const double PI2 = Math.PI * 2;

    /// <summary>
    /// PI * 4
    /// </summary>
    public const double PI4 = Math.PI * 4;

    /// <summary>
    /// Radians-to-degrees conversion constant.
    /// </summary>
    public const double RAD2DEG = 180 / Math.PI;
    public const double PID4 = Math.PI / 4;

    public static Vector3 GPSToUnityUnits(Vector2 pos)
    {
        var unityVector = Vector3.zero;

        unityVector.x = scale(-20.48f, 20.48f, -180, 180, pos.x);

        double lat = pos.y;
        double lng = pos.x;
        double rLat = lat * DEG2RAD;

        const double a = 6378137;
        const double d = 53.5865938 / 10.24;
        const double k = 0.0818191908426;

        double z = Math.Tan(PID4 + rLat / 2) / Math.Pow(Math.Tan(PID4 + Math.Asin(k * Math.Sin(rLat)) / 2), k);


        var ty = (20037508.342789 - a * Math.Log(z)) * d;
        unityVector.y = scale(-20.48f, 20.48f, 209148619f, 566580.0017f, (float)ty);

        return unityVector;
    }

    // public static Vector2 UnityUnitsToGPS(Vector3 pos)
    // {
    //     var unityVector = Vector2.zero;
    //     // pos.y = (1.0f / (Mathf.Cos(pos.y))) * pos.y;
    //     unityVector.y = Mathf.Lerp(-90, 90, Mathf.InverseLerp(-20.48f, 20.48f, pos.y));
    //     unityVector.x = Mathf.Lerp(-180, 180, Mathf.InverseLerp(-20.48f, 20.48f, pos.x));
    //     return unityVector;

    //     const double a = 6378137;
    //     const double c1 = 0.00335655146887969;
    //     const double c2 = 0.00000657187271079536;
    //     const double c3 = 0.00000001764564338702;
    //     const double c4 = 0.00000000005328478445;
    //     const double d = 10.24 / 53.5865938;
    //     double mercY = 20037508.342789 - ty *  d;

    //     double g = Math.PI / 2 - 2 * Math.Atan(1 / Math.Exp(mercY / a));
    //     double z = g + c1 * Math.Sin(2 * g) + c2 * Math.Sin(4 * g) + c3 * Math.Sin(6 * g) + c4 * Math.Sin(8 * g);

    //     lat = z * RAD2DEG;
    // }

    public static float scale(float NewMin, float NewMax, float OldMin, float OldMax, float OldValue)
    {
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
        return (NewValue);
    }

    public static bool inMapView(Vector3 pos, Camera cam)
    {

        var screenPos = cam.WorldToViewportPoint(pos);
        if (screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y <= 1 && screenPos.y >= 0)
            return true;
        else return false;
    }

    public static bool inMapViewTile(Vector3 pos, Camera cam)
    {

        var screenPos = cam.WorldToViewportPoint(pos);
        if (screenPos.x >= -.2 && screenPos.x <= 1.2 && screenPos.y <= 1.2 && screenPos.y >= -.2)
            return true;
        else return false;
    }
}
