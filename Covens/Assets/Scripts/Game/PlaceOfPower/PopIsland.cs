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

        [SerializeField] private Transform m_IslandTransform;
        [SerializeField] private Animator m_Animator;
        [SerializeField] private Transform m_UnitContainer;

        public const float WORLDSIZE_PER_SCALE = 60;

        public Vector3 LocalPosition { get; private set; }
        public float Scale { get; private set; }
        public float Size { get; private set; }
        public IslandType Type { get; set; }
        public Transform UnitContainer { get { return m_UnitContainer; } }
        public List<PopIslandUnit> Units { get; private set; }

        private float m_LerpScale;
        private Vector3 m_LerpPosition;

        private int m_PositionTweenId;
        private int m_ScaleTweenId;
        private int m_UnitsTweenId;

        private void Awake()
        {
            Scale = m_LerpScale = transform.localScale.x;
            Size = WORLDSIZE_PER_SCALE * Scale;
            Type = IslandType.None;
            Units = new List<PopIslandUnit>();
        }
        
        public void TweenIsland(float duration)
        {
            TweenPosition(LocalPosition, duration);
            TweenScale(Scale, duration);
            TweenUnits(duration);
        }

        public void TweenPosition(Vector3 position, float duration)
        {
            LocalPosition = position;
            Vector3 startPosition = m_LerpPosition;

            LeanTween.cancel(m_PositionTweenId);
            m_PositionTweenId = LeanTween.value(0, 1, duration)
                .setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    transform.localPosition = m_LerpPosition = Vector3.Lerp(startPosition, position, t);
                })
                .uniqueId;
        }

        public void TweenScale(float scale, float duration)
        {
            Scale = scale;
            Size = WORLDSIZE_PER_SCALE * scale;

            LeanTween.cancel(m_ScaleTweenId);
            m_ScaleTweenId = LeanTween.value(m_LerpScale, scale, duration)
                .setEaseOutCubic()
                .setOnUpdate((float v) =>
                {
                    m_LerpScale = v;
                    m_IslandTransform.localScale = new Vector3(v, 1, v);
                })
                .uniqueId;
        }

        public void TweenUnits(float duration)
        {
            LeanTween.cancel(m_UnitsTweenId);

            Vector3[] startPositions = new Vector3[Units.Count];
            for (int i = 0; i < startPositions.Length; i++)
                startPositions[i] = Units[i].LerpPosition;

            PopIslandUnit[] aux = Units.ToArray();

            m_UnitsTweenId = LeanTween.value(0, 1, duration)
                .setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    for (int i = 0; i < aux.Length; i++)
                    {
                        aux[i].LerpPosition = Vector3.Lerp(
                            startPositions[i], 
                            aux[i].LocalPosition, 
                            t
                        );

                        aux[i].transform.localPosition = aux[i].LerpPosition;
                    }
                })
                .uniqueId;
        }
    }
}