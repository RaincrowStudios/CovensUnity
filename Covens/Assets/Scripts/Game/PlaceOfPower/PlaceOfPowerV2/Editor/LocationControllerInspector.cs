using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocationIslandController))]
public class LocationControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        LocationIslandController islandManager = this.target as LocationIslandController;
        base.OnInspectorGUI();
        GUILayout.Space(10);
        GUILayout.Label("Debug");
        if (GUILayout.Button("Create Fake Pop"))
        {
            //
        }
    }
}