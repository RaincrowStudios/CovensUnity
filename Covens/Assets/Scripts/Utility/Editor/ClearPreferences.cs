
using UnityEngine;
using UnityEditor;

public class ClearPreferences  {
	[MenuItem("Tools/ClearPreferenes %#l")]
	static void ClearPrefs()
	{
		PlayerPrefs.DeleteAll ();
	}
}
