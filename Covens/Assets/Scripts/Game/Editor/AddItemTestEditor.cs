using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AddTestMarker))]
public class AddItemTestEditor : Editor {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		AddTestMarker PS = (AddTestMarker)target;
	
		if (GUILayout.Button ("Switch Map")) {
			PS.AddSpiritItem ();
		}

		if (GUILayout.Button ("DeathToggle")) {
			PS.DeathToggle ();
		}

		if (GUILayout.Button ("ClearPrefs")) {
			PS.ClearAllPrefs (); 
		}

	}
}
