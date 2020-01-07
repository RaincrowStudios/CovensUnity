using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace Raincrow.Maps
{
    public static class Utilities
    {
        public static Vector2 DistanceBetweenPoints(Vector2 point1, Vector2 point2)
        {
            double scfY = Math.Sin(point1.y * Mathf.Deg2Rad);
            double sctY = Math.Sin(point2.y * Mathf.Deg2Rad);
            double ccfY = Math.Cos(point1.y * Mathf.Deg2Rad);
            double cctY = Math.Cos(point2.y * Mathf.Deg2Rad);
            double cX = Math.Cos((point1.x - point2.x) * Mathf.Deg2Rad);
            double sizeX1 = Math.Abs(6371 * Math.Acos(scfY * scfY + ccfY * ccfY * cX));
            double sizeX2 = Math.Abs(6371 * Math.Acos(sctY * sctY + cctY * cctY * cX));
            float sizeX = (float)((sizeX1 + sizeX2) / 2.0);
            float sizeY = (float)(6371 * Math.Acos(scfY * sctY + ccfY * cctY));
            if (float.IsNaN(sizeY)) sizeY = 0;
            return new Vector2(sizeX, sizeY);
        }

        public static double DistanceBetweenPointsD(Vector2 point1, Vector2 point2)
        {
            double scfY = Math.Sin(point1.y * Mathf.Deg2Rad);
            double sctY = Math.Sin(point2.y * Mathf.Deg2Rad);
            double ccfY = Math.Cos(point1.y * Mathf.Deg2Rad);
            double cctY = Math.Cos(point2.y * Mathf.Deg2Rad);
            double cX = Math.Cos((point1.x - point2.x) * Mathf.Deg2Rad);
            double sizeX1 = Math.Abs(6371 * Math.Acos(scfY * scfY + ccfY * ccfY * cX));
            double sizeX2 = Math.Abs(6371 * Math.Acos(sctY * sctY + cctY * cctY * cX));
            double sizeX = (sizeX1 + sizeX2) / 2.0;
            double sizeY = 6371 * Math.Acos(scfY * sctY + ccfY * cctY);
            if (double.IsNaN(sizeY)) sizeY = 0;
            return Math.Sqrt(sizeX * sizeX + sizeY * sizeY);
        }

        public static IEnumerator GetRequest(string uri, System.Action<long, string> callback)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                if (webRequest.isNetworkError)
                {
                    Debug.Log(pages[page] + ": Error: " + webRequest.error);
                    callback?.Invoke(0, webRequest.error);
                }
                else
                {
                    //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    callback?.Invoke(webRequest.responseCode, webRequest.downloadHandler.text);
                }
            }
        }
    }
}