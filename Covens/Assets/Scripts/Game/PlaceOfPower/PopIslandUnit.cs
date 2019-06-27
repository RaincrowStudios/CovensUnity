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

        public void SetupWitch(IMarker marker)
        {

        }

        public void SetupSpirit(IMarker marker)
        {

        }
    }
}