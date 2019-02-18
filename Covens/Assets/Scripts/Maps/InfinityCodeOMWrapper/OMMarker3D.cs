using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Raincrow.Maps
{
    public class OMMarker3D : IMarker
    {
        private OnlineMapsMarker3D m_Marker3D;
        private Action<IMarker> m_OnClick;

        public object customData
        {
            get { return m_Marker3D.customData; }
            set { m_Marker3D.customData = value; }
        }

        public Vector2 position
        {
            get { return m_Marker3D.position; }
            set { m_Marker3D.position = value; m_Marker3D.transform.position += new Vector3(0, 0.2f, 0); }
        }

        public Action<IMarker> OnClick
        {
            get { return m_OnClick; }
            set
            {
                m_OnClick = value;
                m_Marker3D.OnClick = (marker) => m_OnClick(this);
            }
        }

        public float scale
        {
            get { return m_Marker3D.scale; }
            set { m_Marker3D.scale = value; }
        }

        public GameObject instance
        {
            get { return m_Marker3D.instance; }
            set { m_Marker3D.instance = value; }
        }

        public bool inMapView
        {
            get { return m_Marker3D.inMapView; }
        }

        public OMMarker3D(OnlineMapsMarker3D marker)
        {
            this.m_Marker3D = marker;
        }

        public void SetRange(int min = int.MinValue, int max = int.MaxValue, int minLimit = 3, int maxLimit = OnlineMaps.MAXZOOM)
        {
            m_Marker3D.range = new OnlineMapsRange(min, max, minLimit, maxLimit);
        }

        public void SetPosition(double lng, double lat)
        {
            m_Marker3D.SetPosition(lng, lat);
            instance.transform.position += Vector3.up * 0.15f;
        }
    }
}