using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.DynamicPlacesOfPower
{
    public class PopIsland : MonoBehaviour
    {
        public enum IslandType
        {
            None = 0,
            Guardian,
            Witch,
            Coven,
        }

        [SerializeField] private Animator m_Animator;
        [SerializeField] private PopIslandUnit m_UnitPrefab;

        public const float WORLDSIZE_PER_SCALE = 60;

        public float Size { get; private set; }
        public IslandType Type { get; set; }
        public List<IMarker> Markers { get; private set; }

        public float m_Scale;
        public float m_TargetScale;

        private void Awake()
        {
            m_Scale = m_TargetScale = transform.localScale.x;
            Size = WORLDSIZE_PER_SCALE * m_TargetScale;
            Type = IslandType.None;
            Markers = new List<IMarker>();
        }

        public void AddGuardian(IMarker marker)
        {

        }

        public void AddWitch(IMarker witch)
        {

        }

        public void SetScale(float scale)
        {

        }

        public void SetSize(float size)
        {

        }

        public void Reset()
        {
            //unbind all data
            //resets to initial size
        }
    }
}