using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AddTestMarker))]
public class AddItemTestEditor : Editor {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		AddTestMarker PS = (AddTestMarker)target;
	
		if (GUILayout.Button ("Cast Portal")) {
			PS.CastPortal ();
		}

		if (GUILayout.Button ("DeathToggle")) {
			PS.DeathToggle ();
		}

	}
}
