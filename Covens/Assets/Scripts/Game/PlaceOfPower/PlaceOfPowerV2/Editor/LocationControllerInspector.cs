using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LoadPOPManager))]
public class LocationControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);
        GUILayout.Label("Debug");
        if (GUILayout.Button("Leave POP"))
        {
            LocationIslandController.ExitPOP();
        }
    }
}