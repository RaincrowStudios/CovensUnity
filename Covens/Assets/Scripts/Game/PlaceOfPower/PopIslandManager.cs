using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.DynamicPlacesOfPower
{
    public class PopIslandManager : MonoBehaviour
    {
        [SerializeField] private PopCameraController m_CameraController;

        [SerializeField] private PopIsland m_IslandPrefab;
        [SerializeField] private PopIslandUnit m_UnitPrefab;
        [SerializeField] private LineRenderer m_LinePrefab;

        [SerializeField, HideInInspector] public SpiritMarker m_DebugGuardian;
        [SerializeField, HideInInspector] public WitchMarker m_DebugWitch;
        [SerializeField, HideInInspector] public int m_DebugCovensAmount;
        [SerializeField, HideInInspector] public int m_DebugWitchesAmount;

        private SimplePool<PopIsland> m_IslandPool;
        private SimplePool<PopIslandUnit> m_UnitsPool;
        private SimplePool<LineRenderer> m_LinePool;
        private List<PopIsland> m_Islands;
        private PopIsland m_GuardianIsland;

        private List<LineRenderer> m_GuardianLines;
        private LineRenderer m_IslandLine;
                
        private void Awake()
        {
            m_IslandPool = new SimplePool<PopIsland>(m_IslandPrefab, 0);
            m_UnitsPool = new SimplePool<PopIslandUnit>(m_UnitPrefab, 0);
            m_LinePool = new SimplePool<LineRenderer>(m_LinePrefab, 0);

            m_Islands = new List<PopIsland>();

            m_GuardianLines = m_GuardianLines = new List<LineRenderer>();
            m_IslandLine = m_LinePool.Spawn();

            m_CameraController.onUpdate += (a, b, c) => FaceCamera();
        }

        public void Setup(IMarker guardian, List<IMarker> witches)
        {
            SetupGuardian(guardian);

            //add the witches and create their islands
            foreach (IMarker _witch in witches)
                AddWitch(_witch);

            //update the witches island size and postion
            UpdateIslands();
        }

        public void SetupGuardian(IMarker marker)
        {
            //spawn the guardian island
            if (m_GuardianIsland == null)
            {
                m_GuardianIsland = m_IslandPool.Spawn(this.transform);
                m_GuardianIsland.transform.localPosition = Vector3.zero;
                m_GuardianIsland.TweenScale(3, 1f);
            }

            //init the guardian island
            if (m_GuardianIsland.Units.Count != 0)
            {
                foreach (PopIslandUnit _unit in m_GuardianIsland.Units)
                    m_UnitsPool.Despawn(_unit);
                m_GuardianIsland.Units.Clear();
            }

            PopIslandUnit guardianUnit = m_UnitsPool.Spawn(m_GuardianIsland.UnitContainer);
            guardianUnit.transform.localPosition = Vector3.zero;
            guardianUnit.Setup(marker, m_CameraController.camera);
            m_GuardianIsland.Units.Add(guardianUnit);
        }

        public PopIsland AddWitch(IMarker marker)
        {
            PopIslandUnit newUnit;
            string newMarkerCoven = marker.IsPlayer ? PlayerDataManager.playerData.covenId : (marker.Token as CharacterToken).coven;

            //search an island of the same coven
            if (string.IsNullOrEmpty(newMarkerCoven) == false)
            {
                PopIsland covenIsland = null;
                IMarker aux;

                for (int i = 0; i < m_Islands.Count; i++)
                {
                    //check if the units in this islands belong to the same coven
                    string auxCoven;
                    for (int j = 0; j < m_Islands[i].Units.Count; j++)
                    {
                        aux = m_Islands[i].Units[j].Marker;

                        if (aux == null/* || aux.isNull*/)
                            continue;

                        auxCoven = aux.IsPlayer ? PlayerDataManager.playerData.covenId : (aux.Token as CharacterToken).coven;
                        if (auxCoven == newMarkerCoven)
                        {
                            covenIsland = m_Islands[i];
                            break;
                        }
                    }

                    if (covenIsland != null)
                        break;
                }

                if (covenIsland != null)
                {
                    newUnit = m_UnitsPool.Spawn(covenIsland.UnitContainer);
                    newUnit.Setup(marker, m_CameraController.camera);
                    covenIsland.Units.Add(newUnit);
                    return covenIsland;
                }
            }
            
            //create new island
            PopIsland newIsland = m_IslandPool.Spawn(this.transform);
            m_Islands.Add(newIsland);

            //create new unit for that island
            newUnit = m_UnitsPool.Spawn(newIsland.UnitContainer);
            newUnit.Setup(marker, m_CameraController.camera);
            newIsland.Units.Add(newUnit);

            //update line renderers
            while (m_GuardianLines.Count < m_Islands.Count)
            {
                LineRenderer line = m_LinePool.Spawn(this.transform);
                line.positionCount = 0;
                m_GuardianLines.Add(line);
            }
            while (m_GuardianLines.Count > m_Islands.Count)
            {
                m_LinePool.Despawn(m_GuardianLines[0]);
                m_GuardianLines.RemoveAt(0);
            }

            return newIsland;
        }



        public void UpdateIslands()
        {
            //calculate the island size
            float[] islandScale = new float[m_Islands.Count];
            for (int i = 0; i < m_Islands.Count; i++)
            {
                float unitCount = m_Islands[i].Units.Count;
                //islandScale[i] = 5.098267f + (-2676776f - 5.098267f) / Mathf.Pow(1 + (float)(unitCount / 6.435206e-8), 0.8090155f);
                islandScale[i] = 137.747f + (-27.44875f - 137.747f) / (1 + Mathf.Pow( (unitCount / 771741200f), 0.07594893f));
            }

            //calculate the island position
            float angle = 0;
            Vector3[] islandPosition = new Vector3[m_Islands.Count];
            float maxDist = 0;
            float angleDistance = 360f / m_Islands.Count;

            for (int i = 0; i < m_Islands.Count; i++)
            {
                float radius = islandScale[i] * PopIsland.WORLDSIZE_PER_SCALE * 0.5f;

                Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
                islandPosition[i]
                    = direction
                    * (Random.Range(Mathf.Max(3f, islandScale[i]), 5f))
                    * PopIsland.WORLDSIZE_PER_SCALE;

                float dist = islandPosition[i].magnitude;
                if (dist > maxDist)
                    maxDist = dist;

                if (i < m_Islands.Count - 1)
                {
                    float minDist = (radius + islandScale[i + 1] * PopIsland.WORLDSIZE_PER_SCALE * 0.5f) * 0.3f;
                    angle += Random.Range(minDist, Mathf.Max(minDist, minDist + (angleDistance - minDist) * 0.5f));
                }
            }

            //animate the islands and units
            for (int i = 0; i < m_Islands.Count; i++)
            {
                m_Islands[i].TweenScale(islandScale[i], 1f);
                m_Islands[i].TweenPosition(islandPosition[i], 2f);

                angle = 0;
                angleDistance = 360f / m_Islands[i].Units.Count;
                for (int j = 0; j < m_Islands[i].Units.Count; j++)
                {
                    //50% of puting the first unit in the center
                    if (
                        (j == 0 && m_Islands[i].Units.Count == 1) || (j==0 && m_Islands[i].Units.Count > 2 && Random.Range(0,2) == 0)
                        //m_Islands[i].Units.Count == 1 
                        //|| (j == 0 && m_Islands[i].Units.Count > 2 && Random.Range(0, 2) == 0)
                    )
                    {
                        m_Islands[i].Units[j].LocalPosition = Vector3.zero;
                        angleDistance = 360f / (m_Islands[i].Units.Count - 1);
                        continue;
                    }

                    Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
                    m_Islands[i].Units[j].LocalPosition = direction * Random.Range(1f, 1.2f) * islandScale[i] * PopIsland.WORLDSIZE_PER_SCALE * 0.25f;
                    angle += Random.Range(Mathf.Min(50, angleDistance), angleDistance);
                }
                m_Islands[i].TweenUnits(1f);
            }

            Vector3 centerPoint = Vector3.zero;

            if (m_Islands.Count > 0)
            {
                for (int i = 0; i < m_Islands.Count; i++)
                    centerPoint += m_Islands[i].LocalPosition;
                centerPoint /= m_Islands.Count;
            }

            //update camera bounds
            maxDist = Mathf.Max(maxDist, m_GuardianIsland.Size * 2);
            m_CameraController.SetCameraBounds(centerPoint, maxDist);
        }

        public void ResetPoP()
        {
            //reset and despawn all islands and units in it
            foreach (PopIsland _island in m_Islands)
            {
                foreach (PopIslandUnit _unit in _island.Units)
                {
                    if (_unit.Marker.GameObject != null)
                        Destroy(_unit.Marker.GameObject);
                    m_UnitsPool.Despawn(_unit);
                }
                _island.Units.Clear();
                _island.TweenScale(0, 0);
                _island.TweenPosition(Vector3.zero, 0);
                m_IslandPool.Despawn(_island);
            }
            m_Islands.Clear();

            if (m_GuardianIsland != null)
            {
                foreach (PopIslandUnit _unit in m_GuardianIsland.Units)
                {
                    if (_unit.Marker.GameObject != null)
                        Destroy(_unit.Marker.GameObject);
                    m_UnitsPool.Despawn(_unit);
                }
            }

            foreach (LineRenderer _line in m_GuardianLines)
                m_LinePool.Despawn(_line);
            m_GuardianLines.Clear();
            m_IslandLine.positionCount = 0;
        }

        public void FaceCamera()
        {
            if (m_GuardianIsland != null)
            {
                foreach(PopIslandUnit _unit in m_GuardianIsland.Units)
                {
                    _unit.FaceCamera(m_CameraController.camera);
                }
            }

            foreach(PopIsland _island in m_Islands)
            {
                foreach (PopIslandUnit _unit in _island.Units)
                {
                    _unit.FaceCamera(m_CameraController.camera);
                }
            }
        }

        private void Update()
        {
            //update line rendere positions
            m_IslandLine.positionCount = m_Islands.Count;
            Vector3[] positions = new Vector3[m_Islands.Count];
            for (int i = 0; i < m_Islands.Count; i++)
            {
                positions[i] = m_Islands[i].transform.localPosition;

                m_GuardianLines[i].positionCount = 2;
                m_GuardianLines[i].SetPositions(new Vector3[]
                {
                    m_GuardianIsland.transform.localPosition,
                    m_Islands[i].transform.localPosition
                });
            }
            m_IslandLine.SetPositions(positions);
        }
    }
}