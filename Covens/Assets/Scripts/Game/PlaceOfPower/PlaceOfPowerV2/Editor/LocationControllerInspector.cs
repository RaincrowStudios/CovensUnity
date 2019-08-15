using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LoadPOPManager))]
public class LocationControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(10);

    }
}