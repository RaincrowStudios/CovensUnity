using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOfPower : MonoBehaviour
{
    [SerializeField] private UIPOPOptions m_OptionsMenu;
    [SerializeField] private Transform[] m_PositionTransforms;

    public class POPPosition
    {
        public Transform transform;
        public IMarker marker;
    }

    private POPPosition[] m_Positions;

    public void Show(MarkerDataDetail locationData)
    {

    }
}
