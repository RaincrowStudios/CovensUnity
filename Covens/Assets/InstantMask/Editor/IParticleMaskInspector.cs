using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(InstantParticleMask))] 
public class IParticleMaskInspector : Editor
{
		
	Rect PreTexRect;
	float PreTexWidth;
	float PreTexHeight;
	Texture ResultTexture;
	#if UNITY_2017_1_OR_NEWER
	readonly string[] WrapModeText = {"Repeat", "Clamp", "Mirror", "Mirror Once"};
	readonly int[] WrapModeNum = {0, 1 , 2, 3};
	#else
	readonly string[] OldWrapModeText = {"Repeat", "Clamp"};
	readonly int[] OldWrapModeNum = {0, 1};
	#endif
	readonly string[] MaskPrescribeText = {"Hide (Normal Mask)", "Add Visible Part", "Invert"};
	readonly int[] MaskPrescribeNum = {0, 1, 2};
	readonly string[] ReMaskPrescribeText = {"Hide (Normal Mask)", "Add Visible Part"};
	readonly int[] ReMaskPrescribeNum = {0, 1};

	bool Foldout (bool foldout, GUIContent content, bool toggleOnLabelClick, GUIStyle style)
	{
		Rect position = GUILayoutUtility.GetRect (40f, 40f, 16f, 16f, style);
		// EditorGUI.kNumberW == 40f but is internal
		return EditorGUI.Foldout (position, foldout, content, toggleOnLabelClick, style);
	}
	bool Foldout (bool foldout, string content, bool toggleOnLabelClick, GUIStyle style)
	{
		return Foldout (foldout, new GUIContent (content), toggleOnLabelClick, style);
	}
		
	/* Play中はpreview窓をoverrideで表示にしないでおけば自動で現在のマテリアルからの表示のプレビューがされる
	 * PaeticleはPlay中でない場合はParticleコンポーネントのプレビューが固定されているので未設定
	public override bool HasPreviewGUI ()
	{
		InstantShaderMask IMScript = target as InstantShaderMask;
		bool PreviewCheck;
		if (!EditorApplication.isPlaying && !EditorApplication.isPaused && !EditorApplication.isPlayingOrWillChangePlaymode) {
			if (IMScript.isActiveAndEnabled) {
				PreviewCheck = true;
			} else {
				PreviewCheck = false;
			}
		} else {
			PreviewCheck = false;
		}
		return PreviewCheck;
	}
	*/
		
	/*
		public override Texture2D RenderStaticPreview (string assetPath, Object[] subAssets, int width, int height)
		{
				InstantParticleMask IMScript = target as InstantParticleMask;
		} 
	*/
		
	public override void OnPreviewGUI (Rect r, GUIStyle bg)
	{
		InstantParticleMask IMScript = target as InstantParticleMask;
			
		if (IMScript.isActiveAndEnabled) {
			if (GUI.GetNameOfFocusedControl () == "BaseImageField" && IMScript.BaseImage != null) {
				ResultTexture = IMScript.BaseImage;
			} else if (GUI.GetNameOfFocusedControl () == "Mask1ImageField" && IMScript.Mask1Image != null) {
				ResultTexture = IMScript.Mask1Image;
			} else if (GUI.GetNameOfFocusedControl () == "Mask2ImageField" && IMScript.Mask2Image != null) {
				ResultTexture = IMScript.Mask2Image;
			} else if (GUI.GetNameOfFocusedControl () == "Mask3ImageField" && IMScript.Mask3Image != null) {
				ResultTexture = IMScript.Mask3Image;
			} else {
				ResultTexture = null; //ここでshaderの合成結果画像をpreviewに表示したい(AssetPreview.GetAssetPreview()だと回転等の値を適用したpreviewにならない)
			}
				
			if (ResultTexture == null) {
					
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
					
				EditorGUI.DrawTextureTransparent (PreTexRect, ResultTexture);
			}
				
		}
	}
		
	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space ();
		InstantParticleMask IMScript = target as InstantParticleMask;

		Undo.RecordObject (IMScript, "Inspector");
		if (Event.current.type == EventType.ValidateCommand) {
			if (Event.current.commandName == "UndoRedoPerformed") {
				IMScript.UndoRedoSet ();
				EditorUtility.SetDirty (IMScript);
			}
		}
			
		IMScript.OnEnable ();
		DrawDefaultInspector ();
		EditorGUILayout.Space ();
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.BeginHorizontal ();
		//IMScript.MaterialNum = EditorGUILayout.Popup ("Select Material", IMScript.MaterialNum, IMScript.MaterialsNameAry);
		EditorGUILayout.LabelField ("Material:", GUILayout.Width (52));
		EditorGUILayout.LabelField (IMScript.MaterialName, EditorStyles.boldLabel);
		EditorGUILayout.EndHorizontal ();
		if (IMScript.MaterialName == "Default-Particle") {
			EditorGUILayout.HelpBox("It is not work because Material that assigned particle renderer is the default as it is.", MessageType.Error, true);
		} else if (IMScript.MaterialName == string.Empty) {
			EditorGUILayout.HelpBox("It is not work because Material that assigned particle renderer is None.", MessageType.Error, true);
		}
		EditorGUILayout.Space ();
		EditorGUILayout.BeginVertical (GUI.skin.box);
		IMScript.ResultOpacity = EditorGUILayout.Slider ("Result Opacity", IMScript.ResultOpacity, 0.0f, 100.0f);
		EditorGUILayout.EndVertical ();
		IMScript.AlphaCutoff = EditorGUILayout.Slider ("Alpha Cutoff", IMScript.AlphaCutoff, 0.0f, 1.0f);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.BeginVertical ();
		GUI.SetNextControlName ("BaseImageField");
		IMScript.BaseImage = EditorGUILayout.ObjectField ("Base Image", IMScript.BaseImage, typeof(Texture), false) as Texture;
		EditorGUILayout.EndVertical ();

		EditorGUILayout.BeginVertical (GUI.skin.box);
		EditorGUI.indentLevel++;
		IMScript.BaseImageFoldoutBool = Foldout (IMScript.BaseImageFoldoutBool, "BaseImage Settings", true, EditorStyles.foldout);
		if (IMScript.BaseImageFoldoutBool) {
			EditorGUILayout.Space ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.BaseImageTiling = EditorGUILayout.Vector2Field ("BaseImage Tiling  (Default X:1/Y:1)", IMScript.BaseImageTiling);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.BaseImageOffset = EditorGUILayout.Vector2Field ("BaseImage Offset  (Default X:0/Y:0)", IMScript.BaseImageOffset);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUI.BeginDisabledGroup (IMScript.EditorCheck);
			#if UNITY_2017_1_OR_NEWER
			IMScript.BaseImageWrapModeU = EditorGUILayout.IntPopup ("BaseImage WrapMode U axis", IMScript.BaseImageWrapModeU, WrapModeText, WrapModeNum);
			IMScript.BaseImageWrapModeV = EditorGUILayout.IntPopup ("BaseImage WrapMode V axis", IMScript.BaseImageWrapModeV, WrapModeText, WrapModeNum);
			#else
			IMScript.BaseImageOldWrapMode = EditorGUILayout.IntPopup ("BaseImage WrapMode", IMScript.BaseImageOldWrapMode, OldWrapModeText, OldWrapModeNum);
			#endif
			EditorGUI.EndDisabledGroup ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.Space ();
		}
		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.BeginVertical (GUI.skin.box);
		IMScript.MaskVisibility = EditorGUILayout.Toggle ("Mask Visibility", IMScript.MaskVisibility);
		EditorGUI.BeginDisabledGroup (IMScript.MaskVisibility != true);
		IMScript.VisibleMaskNumber = EditorGUILayout.IntPopup ("Visible MaskImage", IMScript.VisibleMaskNumber, IMScript.MaskStringAry, IMScript.MaskNumberAry);
		IMScript.VisibleColor = EditorGUILayout.ColorField ("Visible Color", IMScript.VisibleColor);
		EditorGUI.EndDisabledGroup ();
		EditorGUILayout.EndVertical ();
		EditorGUILayout.Space ();

		EditorGUILayout.BeginVertical ();
		GUI.SetNextControlName ("Mask1ImageField");
		IMScript.Mask1Image = EditorGUILayout.ObjectField ("Mask1 Image", IMScript.Mask1Image, typeof(Texture2D), false) as Texture2D;
		EditorGUILayout.EndVertical ();
		EditorGUILayout.BeginVertical (GUI.skin.box);
		EditorGUI.indentLevel++;
		IMScript.Mask1FoldoutBool = Foldout (IMScript.Mask1FoldoutBool, "Mask1 Settings", true, EditorStyles.foldout);
		if (IMScript.Mask1FoldoutBool) {
			EditorGUILayout.Space ();
			EditorGUI.BeginDisabledGroup (IMScript.MaskAllImpossibleCheck || IMScript.UseMask1ImpossibleCheck);
			IMScript.UseMask1 = EditorGUILayout.Toggle ("Use Mask1", IMScript.UseMask1);
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.Mask1ImageTiling = EditorGUILayout.Vector2Field ("Mask1 Image Tiling  (Default X:1/Y:1)", IMScript.Mask1ImageTiling);
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("  Mask1 Tiling Pivot To Center", GUILayout.MinWidth (120));
			IMScript.Mask1TilingPivotToCenter = EditorGUILayout.Toggle ("", IMScript.Mask1TilingPivotToCenter, GUILayout.Width (79));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.Mask1ImageOffset = EditorGUILayout.Vector2Field ("Mask1 Image Offset  (Default X:0/Y:0)", IMScript.Mask1ImageOffset);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.Mask1Degree = EditorGUILayout.Slider ("Mask1 Degree", IMScript.Mask1Degree, 0.0f, 100.0f);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.Mask1Position = EditorGUILayout.Vector2Field ("Mask1 Position", IMScript.Mask1Position);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.Mask1Angle = EditorGUILayout.FloatField ("Mask1 Angle", IMScript.Mask1Angle);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Reverse Mask1 Area", GUILayout.Width (145));
			IMScript.ReverseMask1Area = EditorGUILayout.Toggle ("", IMScript.ReverseMask1Area, GUILayout.Width (60));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Grayscale Mask1", GUILayout.Width (140));
			IMScript.GrayscaleMask1 = EditorGUILayout.Toggle ("", IMScript.GrayscaleMask1, GUILayout.Width (65));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
			EditorGUI.EndDisabledGroup ();
			EditorGUILayout.Space ();
		}
		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.BeginVertical ();
		GUI.SetNextControlName ("Mask2ImageField");
		IMScript.Mask2Image = EditorGUILayout.ObjectField ("Mask2 Image", IMScript.Mask2Image, typeof(Texture2D), false) as Texture2D;
		EditorGUILayout.EndVertical ();
		EditorGUILayout.BeginVertical (GUI.skin.box);
		EditorGUI.indentLevel++;
		IMScript.Mask2FoldoutBool = Foldout (IMScript.Mask2FoldoutBool, "Mask2 Settings", true, EditorStyles.foldout);
		if (IMScript.Mask2FoldoutBool) {
			EditorGUILayout.Space ();
			EditorGUI.BeginDisabledGroup (IMScript.MaskAllImpossibleCheck || IMScript.UseMask2ImpossibleCheck);
			IMScript.UseMask2 = EditorGUILayout.Toggle ("Use Mask2", IMScript.UseMask2);
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.Mask2ImageTiling = EditorGUILayout.Vector2Field ("Mask2 Image Tiling  (Default X:1/Y:1)", IMScript.Mask2ImageTiling);
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("  Mask2 Tiling Pivot To Center", GUILayout.MinWidth (120));
			IMScript.Mask2TilingPivotToCenter = EditorGUILayout.Toggle ("", IMScript.Mask2TilingPivotToCenter, GUILayout.Width (79));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.Mask2ImageOffset = EditorGUILayout.Vector2Field ("Mask2 Image Offset  (Default X:0/Y:0)", IMScript.Mask2ImageOffset);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.Mask2Degree = EditorGUILayout.Slider ("Mask2 Degree", IMScript.Mask2Degree, 0.0f, 100.0f);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.Mask2Position = EditorGUILayout.Vector2Field ("Mask2 Position", IMScript.Mask2Position);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.Mask2Angle = EditorGUILayout.FloatField ("Mask2 Angle", IMScript.Mask2Angle);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUI.BeginDisabledGroup (IMScript.Mask2PrescribeCheck != true);
			if (IMScript.ReverseMask2Area)
				IMScript.Mask2Prescribe = EditorGUILayout.IntPopup ("Mask2 Prescribe", IMScript.Mask2Prescribe, ReMaskPrescribeText, ReMaskPrescribeNum);
			else
				IMScript.Mask2Prescribe = EditorGUILayout.IntPopup ("Mask2 Prescribe", IMScript.Mask2Prescribe, MaskPrescribeText, MaskPrescribeNum);
			EditorGUI.EndDisabledGroup ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Reverse Mask2 Area", GUILayout.Width (145));
			IMScript.ReverseMask2Area = EditorGUILayout.Toggle ("", IMScript.ReverseMask2Area, GUILayout.Width (60));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Grayscale Mask2", GUILayout.Width (140));
			IMScript.GrayscaleMask2 = EditorGUILayout.Toggle ("", IMScript.GrayscaleMask2, GUILayout.Width (65));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
			EditorGUI.EndDisabledGroup ();
			EditorGUILayout.Space ();
		}
		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical ();

		EditorGUILayout.Space ();
		EditorGUILayout.BeginVertical ();
		GUI.SetNextControlName ("Mask3ImageField");
		IMScript.Mask3Image = EditorGUILayout.ObjectField ("Mask3 Image", IMScript.Mask3Image, typeof(Texture2D), false) as Texture2D;
		EditorGUILayout.EndVertical ();
		EditorGUILayout.BeginVertical (GUI.skin.box);
		EditorGUI.indentLevel++;
		IMScript.Mask3FoldoutBool = Foldout (IMScript.Mask3FoldoutBool, "Mask3 Settings", true, EditorStyles.foldout);
		if (IMScript.Mask3FoldoutBool) {
			EditorGUILayout.Space ();
			EditorGUI.BeginDisabledGroup (IMScript.MaskAllImpossibleCheck || IMScript.UseMask3ImpossibleCheck);
			IMScript.UseMask3 = EditorGUILayout.Toggle ("Use Mask3", IMScript.UseMask3);
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.Mask3ImageTiling = EditorGUILayout.Vector2Field ("Mask3 Image Tiling  (Default X:1/Y:1)", IMScript.Mask3ImageTiling);
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("  Mask3 Tiling Pivot To Center", GUILayout.MinWidth (120));
			IMScript.Mask3TilingPivotToCenter = EditorGUILayout.Toggle ("", IMScript.Mask3TilingPivotToCenter, GUILayout.Width (79));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.Mask3ImageOffset = EditorGUILayout.Vector2Field ("Mask3 Image Offset  (Default X:0/Y:0)", IMScript.Mask3ImageOffset);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.Mask3Degree = EditorGUILayout.Slider ("Mask3 Degree", IMScript.Mask3Degree, 0.0f, 100.0f);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.Mask3Position = EditorGUILayout.Vector2Field ("Mask3 Position", IMScript.Mask3Position);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			IMScript.Mask3Angle = EditorGUILayout.FloatField ("Mask3 Angle", IMScript.Mask3Angle);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUI.BeginDisabledGroup (IMScript.Mask3PrescribeCheck != true);
			if (IMScript.ReverseMask2Area)
				IMScript.Mask3Prescribe = EditorGUILayout.IntPopup ("Mask3 Prescribe", IMScript.Mask3Prescribe, ReMaskPrescribeText, ReMaskPrescribeNum);
			else
				IMScript.Mask3Prescribe = EditorGUILayout.IntPopup ("Mask3 Prescribe", IMScript.Mask3Prescribe, MaskPrescribeText, MaskPrescribeNum);
			EditorGUI.EndDisabledGroup ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Reverse Mask3 Area", GUILayout.Width (145));
			IMScript.ReverseMask3Area = EditorGUILayout.Toggle ("", IMScript.ReverseMask3Area, GUILayout.Width (60));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Grayscale Mask3", GUILayout.Width (140));
			IMScript.GrayscaleMask3 = EditorGUILayout.Toggle ("", IMScript.GrayscaleMask3, GUILayout.Width (65));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
			EditorGUI.EndDisabledGroup ();
			EditorGUILayout.Space ();
		}
		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical ();

		if (EditorGUI.EndChangeCheck()) {
			EditorUtility.SetDirty (IMScript);
		}
			
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
			
	}
}
