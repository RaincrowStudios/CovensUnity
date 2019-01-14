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

        public Vector2 position
        {
            get { return m_Marker3D.position; }
            set { m_Marker3D.position = value; }
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

        public OMMarker3D(OnlineMapsMarker3D marker)
        {
            this.m_Marker3D = marker;
        }

        public void SetRange(int min = int.MinValue, int max = int.MaxValue, int minLimit = 3, int maxLimit = OnlineMaps.MAXZOOM)
        {
            m_Marker3D.range = new OnlineMapsRange(min, max, minLimit, maxLimit);
        }
    }
}