using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.DynamicPlacesOfPower
{
    public class PopIslandManager : MonoBehaviour
    {
        [SerializeField] private PopIsland m_IslandPrefab;
        [SerializeField] private LineRendererUtility m_LinePrefab;
        [SerializeField] private PopIsland m_GuardianIsland;

        private SimplePool<PopIsland> m_IslandPool;
        private SimplePool<LineRendererUtility> m_LinePool;
        private List<PopIsland> m_Islands;

        private void Awake()
        {
            m_IslandPool = new SimplePool<PopIsland>(m_IslandPrefab, 0);
            m_LinePool = new SimplePool<LineRendererUtility>(m_LinePrefab, 0);
        }

        public void Setup(IMarker guardian, List<IMarker> witches)
        {
            m_GuardianIsland.AddGuardian(guardian);
        }

        public void AddWitch(IMarker marker)
        {
            //search an available island
                //update the island size
                //update the island position

            //create new for witch only
                //find a semi random position
                //setup with witch size
        }
    }
}