using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(FontVariables))]
public class FontVariableEditor : Editor
{

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		FontVariables FM = (FontVariables)target;
		if (GUILayout.Button ("Default Fonts")) {
			FM.UpdateAllText ();
		}
	}
}

