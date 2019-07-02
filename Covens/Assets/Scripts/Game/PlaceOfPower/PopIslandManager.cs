using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.DynamicPlacesOfPower
{
    public class PopIslandManager : MonoBehaviour
    {
        [SerializeField] private PopIsland m_IslandPrefab;
        [SerializeField] private PopIslandUnit m_UnitPrefab;
        [SerializeField] private LineRendererUtility m_LinePrefab;

        private SimplePool<PopIsland> m_IslandPool;
        private SimplePool<PopIslandUnit> m_UnitsPool;
        private SimplePool<LineRendererUtility> m_LinePool;
        private List<PopIsland> m_Islands;
        private PopIsland m_GuardianIsland;
        
        private void Awake()
        {
            m_IslandPool = new SimplePool<PopIsland>(m_IslandPrefab, 0);
            m_LinePool = new SimplePool<LineRendererUtility>(m_LinePrefab, 0);
            m_GuardianIsland = m_IslandPool.Spawn();
            m_Islands = new List<PopIsland>();
        }

        public void Setup(IMarker guardian, List<IMarker> witches)
        {
            if (m_GuardianIsland.Units.Count == 0)
            {
                PopIslandUnit guardianUnit = m_UnitsPool.Spawn();
                guardianUnit.Setup(guardian);

                m_GuardianIsland.Units.Add(guardianUnit);
            }

            foreach (IMarker _witch in witches)
                AddWitch(_witch);

            UpdateIslands();
        }

        public PopIsland AddWitch(IMarker marker)
        {
            PopIslandUnit newUnit;
            string newMarkerCoven = marker.IsPlayer ? PlayerDataManager.playerData.covenName : marker.token.coven;

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

                        if (aux == null || aux.isNull)
                            continue;

                        auxCoven = aux.IsPlayer ? PlayerDataManager.playerData.covenName : aux.token.coven;
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
                    newUnit.Setup(marker);
                    covenIsland.Units.Add(newUnit);
                    return covenIsland;
                }
            }


            //create new island
            PopIsland newIsland = m_IslandPool.Spawn(this.transform);
            newUnit = m_UnitsPool.Spawn(newIsland.UnitContainer);
            newIsland.Units.Add(newUnit);
            return newIsland;
        }

        public void AddWitch(IMarker marker, float duration)
        {
            PopIsland island = AddWitch(marker);
            UpdateIslands();
        }

        public void UpdateIslands()
        {
            //update size and position for the islands list

        }

        public void ResetPoP()
        {
            //reset and despawn all islands and units in it
        }
    }
}