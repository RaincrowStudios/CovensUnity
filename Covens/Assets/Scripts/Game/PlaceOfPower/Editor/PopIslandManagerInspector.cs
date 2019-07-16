using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Raincrow.Maps;

namespace Raincrow.DynamicPlacesOfPower
{        
    [CustomEditor(typeof(PopIslandManager))]
    public class PopIslandManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            PopIslandManager islandManager = this.target as PopIslandManager;

            base.OnInspectorGUI();
            GUILayout.Space(10);
            GUILayout.Label("Debug");
            
            using (new BoxScope())
            {
                EditorGUI.BeginChangeCheck();
                islandManager.m_DebugGuardian = (SpiritMarker)EditorGUILayout.ObjectField("Guardian prefab", islandManager.m_DebugGuardian, typeof(SpiritMarker), true);
                islandManager.m_DebugWitch = (WitchMarker)EditorGUILayout.ObjectField("Witch prefab", islandManager.m_DebugWitch, typeof(WitchMarker), true);
                islandManager.m_DebugWitchesAmount = EditorGUILayout.IntField("Nº of witches", islandManager.m_DebugWitchesAmount);
                islandManager.m_DebugCovensAmount = EditorGUILayout.IntField("Nº of covens", islandManager.m_DebugCovensAmount);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(islandManager);
                }

                if (Application.isPlaying == false)
                    GUILayout.Label("Not in play mode!", "HelpBox");
                else if (PlayerDataManager.playerData != null)
                    GUILayout.Label("Game is running", "HelpBox");
                else if (islandManager.m_DebugGuardian == null)
                    GUILayout.Label("No guardian prefab", "HelpBox");
                else if (islandManager.m_DebugWitch == null)
                    GUILayout.Label("No witch prefab", "HelpBox");

                bool disable = Application.isPlaying == false || PlayerDataManager.playerData != null;
                disable |= islandManager.m_DebugGuardian == null;
                disable |= islandManager.m_DebugWitch == null;

                EditorGUI.BeginDisabledGroup(disable);
                if (GUILayout.Button("Setup"))
                {
                    SetupIsland();
                }
                if (GUILayout.Button("Add 1 witch"))
                {
                    AddWitch();
                }
                if (GUILayout.Button("Add 1 witch (coven)"))
                {
                    AddCovenWitch();
                }
                EditorGUI.EndDisabledGroup();
            }
        }

        private void SetupIsland()
        {
            PopIslandManager islandManager = this.target as PopIslandManager;
            islandManager.ResetPoP();

            if (Application.isPlaying == false)
                return;
            
            SpiritMarker guardian = GameObject.Instantiate(islandManager.m_DebugGuardian);
            guardian.transform.gameObject.SetActive(true);
            guardian.m_Data = new SpiritToken
            {

            };

            List<IMarker> witches = new List<IMarker>();
            for (int i = 0; i < islandManager.m_DebugWitchesAmount; i++)
            {
                WitchMarker witch = GameObject.Instantiate(islandManager.m_DebugWitch);
                witch.transform.gameObject.SetActive(true);
                witch.m_Data = new WitchToken();
                if (islandManager.m_DebugCovensAmount > 0)
                {
                    witch.witchToken.coven = Random.Range(0, 2) == 0 ? "coven" + Random.Range(0, islandManager.m_DebugCovensAmount) : "";
                }
                witches.Add(witch);
            }

            islandManager.Setup(guardian, witches);
        }

        private void AddWitch()
        {
            PopIslandManager islandManager = this.target as PopIslandManager;
            WitchMarker witch = GameObject.Instantiate(islandManager.m_DebugWitch);
            witch.m_Data = new WitchToken();
            witch.transform.gameObject.SetActive(true);
            islandManager.AddWitch(witch);
            islandManager.UpdateIslands();
        }

        private void AddCovenWitch()
        {
            PopIslandManager islandManager = this.target as PopIslandManager;
            WitchMarker witch = GameObject.Instantiate(islandManager.m_DebugWitch);
            witch.m_Data = new WitchToken();
            if (islandManager.m_DebugCovensAmount == 0)
                islandManager.m_DebugCovensAmount = 1;
            witch.witchToken.coven = "coven" + Random.Range(0, islandManager.m_DebugCovensAmount);
            witch.transform.gameObject.SetActive(true);
            islandManager.AddWitch(witch);
            islandManager.UpdateIslands();
        }
    }
}