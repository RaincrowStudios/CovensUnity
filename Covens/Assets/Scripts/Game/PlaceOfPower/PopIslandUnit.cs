using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.DynamicPlacesOfPower
{
    public class PopIslandUnit : MonoBehaviour
    {
        [SerializeField] private Animator m_Animator;

        public IMarker Marker { get; private set; }
        public Vector3 LocalPosition { get; set; }
        public Vector3 LerpPosition { get; set; }

        public void Setup(IMarker marker, Camera camera)
        {
            LocalPosition = LerpPosition = Vector3.zero;
            Marker = marker;
            marker.gameObject.transform.position = this.transform.position;
            marker.gameObject.transform.SetParent(this.transform);
            FaceCamera(camera);
        }

        public void FaceCamera(Camera camera)
        {
            if (Marker != null && !Marker.isNull)
                Marker.characterTransform.rotation = camera.transform.rotation;
        }
    }
}