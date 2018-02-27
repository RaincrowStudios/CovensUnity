using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(InstantTextureMask))] 
public class ITextureMaskInspector : Editor
{
	Rect PreTexRect;
	float PreTexWidth;
	float PreTexHeight;
	float resultTexWidth;
	float resultTexHeight;
	string AncDifMsg;
	Texture2D ResultTexture;
	#if UNITY_2017_1_OR_NEWER
	readonly string[] WrapModeText = {"Repeat", "Clamp", "Mirror", "Mirror Once"};
	readonly int[] WrapModeNum = {0, 1 , 2, 3};
	#else
	readonly string[] OldWrapModeText = {"Repeat", "Clamp"};
	readonly int[] OldWrapModeNum = {0, 1};
	#endif
	//Editor PreviewEditor;
	public bool DisabledCheck;

	public override bool HasPreviewGUI ()
	{
		InstantTextureMask IMScript = target as InstantTextureMask;

		bool PreviewCheck;
		if (IMScript.isActiveAndEnabled) {
			PreviewCheck = true;
		} else {
			PreviewCheck = false;
		}

		return PreviewCheck;
	}

	/*
		public override Texture2D RenderStaticPreview (string assetPath, Object[] subAssets, int width, int height)
		{
				InstantTextureMask IMScript = target as InstantTextureMask;
		} 
	*/

	public override void OnPreviewGUI (Rect r, GUIStyle bg)
	{
		InstantTextureMask IMScript = target as InstantTextureMask;

		if (IMScript.isActiveAndEnabled) {
			if (GUI.GetNameOfFocusedControl () == "BaseImageField") {
				if (IMScript.BaseImage != null)
					ResultTexture = IMScript.BaseImage;
				else
					ResultTexture = null;
			} else if (GUI.GetNameOfFocusedControl () == "MaskImageField") {
				if (IMScript.MaskImage != null)
					ResultTexture = IMScript.MaskImage;
				else
					ResultTexture = null;
			} else {
				if (IMScript.BaseImage != null && IMScript.Result != null)
					ResultTexture = IMScript.Result;
				else
					ResultTexture = null;
			}

			if (ResultTexture == null
				|| IMScript.ResultSize.x <= 0 && IMScript.UseUnityGUI != true
				|| IMScript.ResultSize.y <= 0 && IMScript.UseUnityGUI != true
			    ) {
				resultTexWidth = 0;
				resultTexHeight = 0;
			} else {
				PreTexWidth = ResultTexture.width * (r.height / ResultTexture.height);
				if (PreTexWidth < r.width) {
					PreTexHeight = r.height;
					PreTexRect = new Rect ((r.width / 2 - PreTexWidth / 2) + 2, r.y, PreTexWidth, PreTexHeight);
				} else {
					PreTexWidth = r.width;
					PreTexHeight = ResultTexture.height * (r.width / ResultTexture.width);
					PreTexRect = new Rect (r.x, r.y + (r.height / 2 - PreTexHeight / 2), PreTexWidth, PreTexHeight);
				}
				resultTexWidth = ResultTexture.width;
				resultTexHeight = ResultTexture.height;
						
				//下２つのどちらがいいのか
				/*
						PreviewEditor = Editor.CreateEditor (ResultTexture);
						PreviewEditor.OnPreviewGUI (PreTexRect, EditorStyles.whiteLabel);
						*/
				EditorGUI.DrawTextureTransparent (PreTexRect, ResultTexture);
			}
		
			GUIStyle BStyle = new GUIStyle (GUI.skin.label);
			BStyle.fontSize = 11;
			BStyle.fontStyle = FontStyle.Bold;
			BStyle.normal.textColor = new Color (0, 0, 0, 0.5f);
			BStyle.alignment = TextAnchor.UpperCenter;
			GUI.Label (new Rect ((r.width / 2 - 100) + 2, (r.y + r.height - 30), 200, 100), "Image\nImage Size: " + resultTexWidth + "x" + resultTexHeight, BStyle);
			GUI.Label (new Rect ((r.width / 2 - 100) + 2, (r.y + r.height - 31), 200, 100), "Image\nImage Size: " + resultTexWidth + "x" + resultTexHeight, BStyle);
			GUI.Label (new Rect ((r.width / 2 - 100) + 1, (r.y + r.height - 31), 200, 100), "Image\nImage Size: " + resultTexWidth + "x" + resultTexHeight, BStyle);
			GUI.Label (new Rect ((r.width / 2 - 100) + 3, (r.y + r.height - 31), 200, 100), "Image\nImage Size: " + resultTexWidth + "x" + resultTexHeight, BStyle);
			GUIStyle B2Style = new GUIStyle (GUI.skin.label);
			B2Style.fontSize = 11;
			B2Style.fontStyle = FontStyle.Bold;
			B2Style.normal.textColor = new Color (0, 0, 0, 0.2f);
			B2Style.alignment = TextAnchor.UpperCenter;
			GUI.Label (new Rect ((r.width / 2 - 100) + 2, (r.y + r.height - 29), 200, 200), "Image\nImage Size: " + resultTexWidth + "x" + resultTexHeight, B2Style);
			GUI.Label (new Rect ((r.width / 2 - 100) + 0, (r.y + r.height - 31), 200, 200), "Image\nImage Size: " + resultTexWidth + "x" + resultTexHeight, B2Style);
			GUI.Label (new Rect ((r.width / 2 - 100) + 4, (r.y + r.height - 31), 200, 200), "Image\nImage Size: " + resultTexWidth + "x" + resultTexHeight, B2Style);
			GUI.Label (new Rect ((r.width / 2 - 100) + 2, (r.y + r.height - 32), 200, 200), "Image\nImage Size: " + resultTexWidth + "x" + resultTexHeight, B2Style);
			GUI.Label (new Rect ((r.width / 2 - 100) + 2, (r.y + r.height - 33), 200, 200), "Image\nImage Size: " + resultTexWidth + "x" + resultTexHeight, B2Style);

			GUIStyle WStyle = new GUIStyle (GUI.skin.label);
			WStyle.fontSize = 11;
			WStyle.fontStyle = FontStyle.Bold;
			WStyle.alignment = TextAnchor.UpperCenter;
			WStyle.normal.textColor = new Color (1, 1, 1, 0.95f);
			GUI.Label (new Rect ((r.width / 2 - 100) + 2, (r.y + r.height - 32), 200, 100), "Image\nImage Size: " + resultTexWidth + "x" + resultTexHeight, WStyle);
		}
	}

	
	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();
		InstantTextureMask IMScript = target as InstantTextureMask;
		GUI.changed = false;

		Undo.RecordObject (IMScript, "Inspector");
		if (Event.current.type == EventType.ValidateCommand) {
			if (Event.current.commandName == "UndoRedoPerformed") {
				IMScript.UndoRedoSet ();
				EditorUtility.SetDirty (IMScript);
			}
		}

		if (IMScript.Masking != true || IMScript.MaskingDisabledCheck)
			DisabledCheck = true;
		else
			DisabledCheck = false;

		DrawDefaultInspector ();
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.BeginVertical (GUI.skin.box);
		IMScript.ResultSize = EditorGUILayout.Vector2Field ("Result Size  (X:width/Y:height)", IMScript.ResultSize);
		if (IMScript.ResultSize.x < 0 || IMScript.ResultSize.x > 8192 || IMScript.ResultSize.y < 0 || IMScript.ResultSize.y > 8192)
			GUI.FocusControl ("");
		/* IMScript内でResultSizeとRectTransformを同じ値に連動させるようにした場合は以下を追加
		if (IMScript.AncXDifCheck || IMScript.AncYDifCheck) {
			EditorGUILayout.BeginVertical (GUI.skin.box);
			if (IMScript.AncXDifCheck && IMScript.AncYDifCheck)
				AncDifMsg = "UnityGUI is in relative state at present.\nSo 'X(width)' & 'Y(height)' is needed to set from RectTransform component of UnityGUI.";
			else if (IMScript.AncXDifCheck)
				AncDifMsg = "UnityGUI is in relative state at present.\nSo 'X(width)' is needed to set from RectTransform component of UnityGUI.";
			else if (IMScript.AncYDifCheck)
				AncDifMsg = "UnityGUI is in relative state at present.\nSo 'Y(height)' is needed to set from RectTransform component of UnityGUI.";
			EditorGUILayout.HelpBox (AncDifMsg, MessageType.Info);
			EditorGUILayout.EndVertical ();
		}
		*/
		EditorGUILayout.EndVertical ();
		EditorGUILayout.BeginVertical (GUI.skin.box);
		IMScript.ResultOpacity = EditorGUILayout.Slider ("Result Opacity", IMScript.ResultOpacity, 0.0f, 100.0f);
		EditorGUILayout.EndVertical ();
		EditorGUILayout.BeginVertical (GUI.skin.box);
		#if UNITY_2017_1_OR_NEWER
		IMScript.ResultWrapModeU = EditorGUILayout.IntPopup ("Result WrapMode U axis", IMScript.ResultWrapModeU, WrapModeText, WrapModeNum);
		IMScript.ResultWrapModeV = EditorGUILayout.IntPopup ("Result WrapMode V axis", IMScript.ResultWrapModeV, WrapModeText, WrapModeNum);
		#else
		IMScript.ResultOldWrapMode = EditorGUILayout.IntPopup ("Result WrapMode", IMScript.ResultOldWrapMode, OldWrapModeText, OldWrapModeNum);
		#endif
		EditorGUILayout.EndVertical ();
		GUI.SetNextControlName ("BaseImageField");
		IMScript.BaseImage = EditorGUILayout.ObjectField ("Base Image", IMScript.BaseImage, typeof(Texture2D), false) as Texture2D;
		EditorGUILayout.EndVertical ();
		EditorGUILayout.BeginVertical ();
		GUI.SetNextControlName ("MaskImageField");
		IMScript.MaskImage = EditorGUILayout.ObjectField ("Mask Image", IMScript.MaskImage, typeof(Texture2D), false) as Texture2D;
		EditorGUILayout.EndVertical ();
		EditorGUILayout.Space ();
		EditorGUI.BeginDisabledGroup (IMScript.MaskingDisabledCheck);
		IMScript.Masking = EditorGUILayout.Toggle ("Masking", IMScript.Masking);
		EditorGUI.EndDisabledGroup ();
		EditorGUI.BeginDisabledGroup (DisabledCheck);
		EditorGUILayout.BeginVertical (GUI.skin.box);
		IMScript.MaskRect = EditorGUILayout.RectField ("Mask Rect", IMScript.MaskRect);
		if (IMScript.MaskRect.width < 0 || IMScript.MaskRect.width > 8192 || IMScript.MaskRect.height < 0 || IMScript.MaskRect.height > 8192)
			GUI.FocusControl ("");
		EditorGUILayout.EndVertical ();
		EditorGUILayout.BeginVertical (GUI.skin.box);
		IMScript.MaskDegree = EditorGUILayout.Slider ("Mask Degree", IMScript.MaskDegree, 0.0f, 100.0f);
		EditorGUILayout.EndVertical ();
		EditorGUILayout.BeginVertical (GUI.skin.box);
		IMScript.UseMaskRotate = EditorGUILayout.Toggle ("Use Mask Rotate", IMScript.UseMaskRotate);
		EditorGUI.BeginDisabledGroup (IMScript.UseMaskRotate != true);
		IMScript.MaskAngle = EditorGUILayout.FloatField ("Mask Angle", IMScript.MaskAngle);
		EditorGUI.EndDisabledGroup ();
		EditorGUILayout.EndVertical ();

		EditorGUILayout.BeginVertical (GUI.skin.box);
		IMScript.UseUnityGUI = EditorGUILayout.Toggle ("Use UnityGUI", IMScript.UseUnityGUI);
		if (IMScript.UseUnityGUI) {
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.MaskVisibility = EditorGUILayout.Toggle ("Mask Visibility", IMScript.MaskVisibility);
			EditorGUI.BeginDisabledGroup (IMScript.MaskVisibility != true);
			IMScript.VisibleColor = EditorGUILayout.ColorField ("Visible Color", IMScript.VisibleColor);
			EditorGUI.EndDisabledGroup ();
			EditorGUILayout.EndVertical ();
		} else {
			EditorGUILayout.Space ();
		}
		EditorGUILayout.EndVertical ();

		EditorGUILayout.BeginVertical (GUI.skin.box);
		IMScript.ReverseMaskArea = EditorGUILayout.Toggle ("Reverse Mask Area", IMScript.ReverseMaskArea);
		EditorGUILayout.EndVertical ();
		EditorGUILayout.BeginVertical (GUI.skin.box);
		IMScript.GrayscaleMask = EditorGUILayout.Toggle ("Grayscale Mask", IMScript.GrayscaleMask);
		EditorGUILayout.EndVertical ();
		EditorGUI.EndDisabledGroup ();

		EditorGUI.BeginDisabledGroup (IMScript.BaseImage == null);
		IMScript.Pixelization = EditorGUILayout.IntSlider ("Pixelization", IMScript.Pixelization, 0, 50);
		EditorGUI.EndDisabledGroup ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Box ("", GUIStyle.none, GUILayout.ExpandWidth (true));
		EditorGUI.BeginDisabledGroup (IMScript.BaseImage == null);
		if (GUILayout.Button ("Base Image Set Native Size", EditorStyles.miniButton)) { //,GUILayout.Width (80)
			if (IMScript.BaseImage != null)
				IMScript.BaseImageSetNativeSize ();
			GUI.FocusControl (""); //Inspectorのフォーカスを解除して入力欄を更新
		}
		EditorGUI.EndDisabledGroup ();
		GUILayout.Box ("", GUIStyle.none, GUILayout.ExpandWidth (false), GUILayout.Width (5));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.Space ();
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Box ("", GUIStyle.none, GUILayout.ExpandWidth (true));
		EditorGUI.BeginDisabledGroup (DisabledCheck);
		if (GUILayout.Button ("Mask Image Set Native Size", EditorStyles.miniButton)) { //,GUILayout.Width (80)
			if (IMScript.MaskImage != null)
				IMScript.MaskImageSetNativeSize ();
			GUI.FocusControl (""); //Inspectorのフォーカスを解除して入力欄を更新
		}
		EditorGUI.EndDisabledGroup ();
		GUILayout.Box ("", GUIStyle.none, GUILayout.ExpandWidth (false), GUILayout.Width (5));
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUI.BeginDisabledGroup (IMScript.BaseImage == null);
		if (GUILayout.Button ("Export Result Image", EditorStyles.miniButton)) { //,GUILayout.Width (100)
			if (IMScript.BaseImage != null)
				IMScript.ExportResultImage ();
			GUI.FocusControl (""); //Inspectorのフォーカスを解除して入力欄を更新
		}
		EditorGUI.EndDisabledGroup ();

		if (GUI.changed) {
			EditorUtility.SetDirty (IMScript);
			//SceneView.RepaintAll ();
			//HandleUtility.Repaint ();
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

	}

}

class IMaskPostprocess : AssetPostprocessor
{
	void OnPreprocessTexture ()
	{
		TextureImporter importer = assetImporter as TextureImporter;
		string FilePath = assetPath;
		if (FilePath.StartsWith ("Assets/InstantMask/ExportImages/") && FilePath.EndsWith (".png")) {
			importer.textureType = TextureImporterType.Default;
			importer.isReadable = true;
			importer.filterMode = FilterMode.Bilinear;
			importer.npotScale = TextureImporterNPOTScale.None;
			importer.mipmapEnabled = true;
			//importer.textureFormat = TextureImporterFormat.RGBA32;
			importer.alphaIsTransparency = true;
			if (InstantTextureMask.IMScript != null) {
				InstantTextureMask IMScript = InstantTextureMask.IMScript;
				#if UNITY_2017_1_OR_NEWER
				if (IMScript.ResultWrapModeU == 0)
					importer.wrapModeU = TextureWrapMode.Clamp;
				else if (IMScript.ResultWrapModeU == 1)
					importer.wrapModeU = TextureWrapMode.Repeat;
				else if (IMScript.ResultWrapModeU == 2)
					importer.wrapModeU = TextureWrapMode.Mirror;
				else if (IMScript.ResultWrapModeU == 3)
					importer.wrapModeU = TextureWrapMode.MirrorOnce;
				else
					importer.wrapModeU = TextureWrapMode.Repeat;
				if (IMScript.ResultWrapModeV == 0)
					importer.wrapModeV = TextureWrapMode.Clamp;
				else if (IMScript.ResultWrapModeV == 1)
					importer.wrapModeV = TextureWrapMode.Repeat;
				else if (IMScript.ResultWrapModeV == 2)
					importer.wrapModeV = TextureWrapMode.Mirror;
				else if (IMScript.ResultWrapModeV == 3)
					importer.wrapModeV = TextureWrapMode.MirrorOnce;
				else
					importer.wrapModeV = TextureWrapMode.Repeat;
				#else
				if (IMScript.ResultOldWrapMode > 0)
					importer.wrapMode = TextureWrapMode.Clamp;
				else
					importer.wrapMode = TextureWrapMode.Repeat;
				#endif
				int ResultSizeInt = Mathf.Max (IMScript.Result.width, IMScript.Result.height);
				int ResultSizePowerOfTwo = 2048;
				if (ResultSizeInt >= 8192) {
					ResultSizePowerOfTwo = 8192;
				} else {
					ResultSizePowerOfTwo = Mathf.ClosestPowerOfTwo (ResultSizeInt);
					if (ResultSizePowerOfTwo < ResultSizeInt)
						ResultSizePowerOfTwo = ResultSizePowerOfTwo * 2;
				}
				importer.maxTextureSize = ResultSizePowerOfTwo;
			}
		}
	}

}

