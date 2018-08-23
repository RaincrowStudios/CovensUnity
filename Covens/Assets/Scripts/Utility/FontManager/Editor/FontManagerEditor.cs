using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(FontManager))]
public class FontManagerEditor : Editor {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		FontManager FM = (FontManager)target;

		if (GUILayout.Button ("WhiteBG")) {
			FM.SetupFontWhiteBG ();
		}

		if (GUILayout.Button ("BlackBG")) {
			FM.SetupFontBlackBG ();
		}

} 

}