namespace Mapbox.Editor
{
	using UnityEditor;
	using UnityEngine;
	using Mapbox.Unity.Map;

	[CustomPropertyDrawer(typeof(CameraBoundsTileProviderOptions))]
	public class CameraBoundsTileProviderOptionsDrawer : PropertyDrawer
	{
		static float _lineHeight = EditorGUIUtility.singleLineHeight;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var camera = property.FindPropertyRelative("camera");
            var visibleBuffer = property.FindPropertyRelative("visibleBuffer");
            var disposeBuffer = property.FindPropertyRelative("disposeBuffer");
            var centerPoint = property.FindPropertyRelative("cameraCenterPoint");
            var maxSpawn = property.FindPropertyRelative("maxRangeFromCenter");


            EditorGUILayout.PropertyField(camera, new GUIContent
			{
				text = camera.displayName,
				tooltip = "Camera to control map extent."
			}, GUILayout.Height(_lineHeight));
            
            EditorGUILayout.PropertyField(visibleBuffer, new GUIContent
            {
                text = visibleBuffer.displayName,
                tooltip = ""
            }, GUILayout.Height(_lineHeight));

            EditorGUILayout.PropertyField(disposeBuffer, new GUIContent
            {
                text = disposeBuffer.displayName,
                tooltip = ""
            }, GUILayout.Height(_lineHeight));

            EditorGUILayout.PropertyField(centerPoint, new GUIContent
            {
                text = "Center",
                tooltip = "The transform representing the center point"
            }, GUILayout.Height(_lineHeight));
            
            EditorGUILayout.PropertyField(maxSpawn, new GUIContent
            {
                text = "MaxRange",
                tooltip = "Maximum tile distance from the center point"
            }, GUILayout.Height(_lineHeight));
        }
	}
}