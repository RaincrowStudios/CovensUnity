using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerSelect))]
public class PlayerSelectEditor : Editor {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		PlayerSelect PS = (PlayerSelect)target;
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Reset CC")) {
			PS.ResetCC ();
		}

		if (GUILayout.Button ("Reset Position")) {
			PS.resetPosition ();
		}
		GUILayout.EndHorizontal ();

	}
}
