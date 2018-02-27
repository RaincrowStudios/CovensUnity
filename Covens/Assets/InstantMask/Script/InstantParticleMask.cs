using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if (UNITY_EDITOR)
using UnityEditor;
#endif

[ExecuteInEditMode ()]
public class InstantParticleMask : MonoBehaviour
{
	[HideInInspector]
	public string MaterialName;
	Material RndMaterial;
	[HideInInspector]
	public int
		MaterialNum = 0;
	[HideInInspector]
	public bool
		UseMask1;
	[HideInInspector]
	public bool
		UseMask2;
	[HideInInspector]
	public bool
		UseMask3;
	[HideInInspector]
	public bool
		HoldUseMask1Check;
	[HideInInspector]
	public bool
		HoldUseMask2Check;
	[HideInInspector]
	public bool
		HoldUseMask3Check;
	[HideInInspector]
	public float
		ResultOpacity = 100;
	[HideInInspector]
	public float
		HoldResultOpacity = 100;
	[HideInInspector]
	public float
		AlphaCutoff = 0.0f;
	[HideInInspector]
	public float
		HoldAlphaCutoff = 0.0f;
	[HideInInspector]
	public bool
		MaskVisibility;
	[HideInInspector]
	public bool
		HoldMaskVisibility;
	[HideInInspector]
	public Color
		VisibleColor = new Color (0, 0.6f, 0, 0.6f);
	[HideInInspector]
	public Color
		HoldVisibleColor = new Color (0, 0.6f, 0, 0.6f);
	[HideInInspector]
	public float
		Mask1Degree = 100;
	[HideInInspector]
	public float
		HoldMask1Degree = 100;
	[HideInInspector]
	public bool
		Mask1TilingPivotToCenter;
	[HideInInspector]
	public bool
		HoldMask1TilingPivotToCenter;
	[HideInInspector]
	public Vector2
		Mask1Position;
	[HideInInspector]
	public Vector2
		HoldMask1Pos;
	[HideInInspector]
	public float
		Mask1Angle;
	[HideInInspector]
	public float
		HoldMask1Angle;
	[HideInInspector]
	public bool
		ReverseMask1Area;
	[HideInInspector]
	public bool
		HoldReverseMask1AreaCheck;
	[HideInInspector]
	public bool
		GrayscaleMask1;
	[HideInInspector]
	public bool
		HoldGrayscaleMask1Check;
	[HideInInspector]
	public float
		Mask2Degree = 100;
	[HideInInspector]
	public float
		HoldMask2Degree = 100;
	[HideInInspector]
	public bool
		Mask2TilingPivotToCenter;
	[HideInInspector]
	public bool
		HoldMask2TilingPivotToCenter;
	[HideInInspector]
	public Vector2
		Mask2Position;
	[HideInInspector]
	public Vector2
		HoldMask2Pos;
	[HideInInspector]
	public float
		Mask2Angle;
	[HideInInspector]
	public float
		HoldMask2Angle;
	[HideInInspector]
	public bool
		ReverseMask2Area;
	[HideInInspector]
	public bool
		HoldReverseMask2AreaCheck;
	[HideInInspector]
	public bool
		GrayscaleMask2;
	[HideInInspector]
	public bool
		HoldGrayscaleMask2Check;
	[HideInInspector]
	public float
		Mask3Degree = 100;
	[HideInInspector]
	public float
		HoldMask3Degree = 100;
	[HideInInspector]
	public bool
		Mask3TilingPivotToCenter;
	[HideInInspector]
	public bool
		HoldMask3TilingPivotToCenter;
	[HideInInspector]
	public Vector2
		Mask3Position;
	[HideInInspector]
	public Vector2
		HoldMask3Pos;
	[HideInInspector]
	public float
		Mask3Angle;
	[HideInInspector]
	public float
		HoldMask3Angle;
	[HideInInspector]
	public bool
		ReverseMask3Area;
	[HideInInspector]
	public bool
		HoldReverseMask3AreaCheck;
	[HideInInspector]
	public bool
		GrayscaleMask3;
	[HideInInspector]
	public bool
		HoldGrayscaleMask3Check;
	[HideInInspector]
	public Vector2
		BaseImageTiling = new Vector2 (1, 1);
	[HideInInspector]
	public Vector2
		BaseImageOffset = new Vector2 (0, 0);
	[HideInInspector]
	public Vector2
		HoldBaseImageTiling = new Vector2 (1, 1);
	[HideInInspector]
	public Vector2
		HoldBaseImageOffset = new Vector2 (0, 0);
	[HideInInspector]
	public Vector2
		Mask1ImageTiling = new Vector2 (1, 1);
	[HideInInspector]
	public Vector2
		Mask1ImageOffset = new Vector2 (0, 0);
	[HideInInspector]
	public Vector2
		HoldMask1ImageTiling = new Vector2 (1, 1);
	[HideInInspector]
	public Vector2
		HoldMask1ImageOffset = new Vector2 (0, 0);
	[HideInInspector]
	public Vector2
		Mask2ImageTiling = new Vector2 (1, 1);
	[HideInInspector]
	public Vector2
		Mask2ImageOffset = new Vector2 (0, 0);
	[HideInInspector]
	public Vector2
		HoldMask2ImageTiling = new Vector2 (1, 1);
	[HideInInspector]
	public Vector2
		HoldMask2ImageOffset = new Vector2 (0, 0);
	[HideInInspector]
	public Vector2
		Mask3ImageTiling = new Vector2 (1, 1);
	[HideInInspector]
	public Vector2
		Mask3ImageOffset = new Vector2 (0, 0);
	[HideInInspector]
	public Vector2
		HoldMask3ImageTiling = new Vector2 (1, 1);
	[HideInInspector]
	public Vector2
		HoldMask3ImageOffset = new Vector2 (0, 0);
	[HideInInspector]
	public int
		BaseImageWrapModeU;
	[HideInInspector]
	public int
		HoldBaseImageWrapModeU;
	[HideInInspector]
	public int
		BaseImageWrapModeV;
	[HideInInspector]
	public int
		HoldBaseImageWrapModeV;
	[HideInInspector]
	public int
		BaseImageOldWrapMode;
	[HideInInspector]
	public int
		HoldBaseImageOldWrapMode;
	bool UndoRedoSetCheck;
	bool Mask1NullCheck;
	bool Mask2NullCheck;
	bool Mask3NullCheck;
	bool MaskListRemakeCheck;
	[HideInInspector]
	public bool
		Mask2PrescribeCheck;
	[HideInInspector]
	public bool
		Mask3PrescribeCheck;
	[HideInInspector]
	public int
		Mask2Prescribe;
	int HoldMask2Prescribe;
	[HideInInspector]
	public int
		Mask3Prescribe;
	int HoldMask3Prescribe;
	[HideInInspector]
	public bool
		MaskAllImpossibleCheck;
	[HideInInspector]
	public bool
		UseMask1ImpossibleCheck;
	[HideInInspector]
	public bool
		UseMask2ImpossibleCheck;
	[HideInInspector]
	public bool
		UseMask3ImpossibleCheck;
	[HideInInspector]
	public string[]
	MaskStringAry;
	List<string> MaskStringList = new List<string> ();
	[HideInInspector]
	public int[]
	MaskNumberAry;
	List<int> MaskNumberList = new List<int> ();
	[HideInInspector]
	public int
		VisibleMaskNumber;
	#if (UNITY_EDITOR)
	[HideInInspector]
	public bool
		BaseImageFoldoutBool;
	[HideInInspector]
	public bool
		Mask1FoldoutBool;
	[HideInInspector]
	public bool
		Mask2FoldoutBool;
	[HideInInspector]
	public bool
		Mask3FoldoutBool;
	[HideInInspector]
	public bool
		EditorCheck;
	#if UNITY_2017_1_OR_NEWER
	bool BaseImageWrapUWarningCheck;
	bool BaseImageWrapVWarningCheck;
	#else
	bool BaseImageRepeatWarningCheck;
	bool BaseImageClampWarningCheck;
	#endif
	#endif
	
	void Start ()
	{
		#if (UNITY_EDITOR)
		if (Application.isEditor && !EditorApplication.isPlaying && !EditorApplication.isPaused) {
			BaseImageWrapModeU = -1;
			HoldBaseImageWrapModeU = -1;
			BaseImageWrapModeV = -1;
			HoldBaseImageWrapModeV = -1;
			BaseImageOldWrapMode = -1;
			HoldBaseImageOldWrapMode = -1;
		} else {
			#endif
			#if UNITY_2017_1_OR_NEWER
			if (BaseImage != null) {
				if (BaseImage.wrapModeU == TextureWrapMode.Repeat) {
					BaseImageWrapModeU = 0;
					HoldBaseImageWrapModeU = 0;
				} else if (BaseImage.wrapModeU == TextureWrapMode.Clamp) {
					BaseImageWrapModeU = 1;
					HoldBaseImageWrapModeU = 1;
				} else if (BaseImage.wrapModeU == TextureWrapMode.Mirror) {
					BaseImageWrapModeU = 2;
					HoldBaseImageWrapModeU = 2;
				} else if (BaseImage.wrapModeU == TextureWrapMode.MirrorOnce) {
					BaseImageWrapModeU = 3;
					HoldBaseImageWrapModeU = 3;
				} else {
					BaseImageWrapModeU = -1;
					HoldBaseImageWrapModeU = -1;
				}
				if (BaseImage.wrapModeV == TextureWrapMode.Repeat) {
					BaseImageWrapModeV = 0;
					HoldBaseImageWrapModeV = 0;
				} else if (BaseImage.wrapModeV == TextureWrapMode.Clamp) {
					BaseImageWrapModeV = 1;
					HoldBaseImageWrapModeV = 1;
				} else if (BaseImage.wrapModeV == TextureWrapMode.Mirror) {
					BaseImageWrapModeV = 2;
					HoldBaseImageWrapModeV = 2;
				} else if (BaseImage.wrapModeV == TextureWrapMode.MirrorOnce) {
					BaseImageWrapModeV = 3;
					HoldBaseImageWrapModeV = 3;
				} else {
					BaseImageWrapModeV = -1;
					HoldBaseImageWrapModeV = -1;
				}
			} else {
				BaseImageOldWrapMode = -1;
				HoldBaseImageOldWrapMode = -1;
			}
			#else
			if (BaseImage != null) {
				if (BaseImage.wrapMode == TextureWrapMode.Repeat) {
					BaseImageOldWrapMode = 0;
					HoldBaseImageOldWrapMode = 0;
				} else if (BaseImage.wrapMode == TextureWrapMode.Clamp) {
					BaseImageOldWrapMode = 1;
					HoldBaseImageOldWrapMode = 1;
				} else {
					BaseImageOldWrapMode = -1;
					HoldBaseImageOldWrapMode = -1;
				}
			} else {
				BaseImageOldWrapMode = -1;
				HoldBaseImageOldWrapMode = -1;
			}
			#endif
			#if (UNITY_EDITOR)
		}
		#endif
	}
	
	#if (UNITY_EDITOR)
	public void OnEnable ()
	{
		#if UNITY_2017_1_OR_NEWER
		BaseImageWrapUWarningCheck = false;
		BaseImageWrapVWarningCheck = false;
		#else
		BaseImageRepeatWarningCheck = false;
		BaseImageClampWarningCheck = false;
		#endif
	}
	#endif
	
	void Update ()
	{
		if (ResultOpacity <= 0.0f)
			ResultOpacity = 0.0f;
		else if (ResultOpacity >= 100.0f)
			ResultOpacity = 100.0f;
		if (AlphaCutoff <= 0.0f)
			AlphaCutoff = 0.0f;
		else if (AlphaCutoff >= 1.0f)
			AlphaCutoff = 1.0f;
		if (Mask1Degree <= 0.0f)
			Mask1Degree = 0.0f;
		else if (Mask1Degree >= 100.0f)
			Mask1Degree = 100.0f;
		if (Mask1Angle >= 360 || Mask1Angle <= -360)
			Mask1Angle = Mask1Angle % 360;
		if (Mask2Degree <= 0.0f)
			Mask2Degree = 0.0f;
		else if (Mask2Degree >= 100.0f)
			Mask2Degree = 100.0f;
		if (Mask2Angle >= 360 || Mask2Angle <= -360)
			Mask2Angle = Mask2Angle % 360;
		if (Mask3Degree <= 0.0f)
			Mask3Degree = 0.0f;
		else if (Mask3Degree >= 100.0f)
			Mask3Degree = 100.0f;
		if (Mask3Angle >= 360 || Mask3Angle <= -360)
			Mask3Angle = Mask3Angle % 360;
		
		if (GetComponent<ParticleSystemRenderer> ()) {
			#if (UNITY_EDITOR)
			if (!EditorApplication.isPlaying && !EditorApplication.isPaused || !EditorApplication.isPlayingOrWillChangePlaymode) {
				if (RndMaterial != GetComponent<ParticleSystemRenderer> ().sharedMaterial)
					RndMaterial = GetComponent<ParticleSystemRenderer> ().sharedMaterial;
			} else {
				#endif
				if (RndMaterial != GetComponent<ParticleSystemRenderer> ().material)
					RndMaterial = GetComponent<ParticleSystemRenderer> ().material;
				#if (UNITY_EDITOR)
			}
			#endif
		} else {
			RndMaterial = null;
		}
		/*
		//old particle <ParticleRenderer> with some materials set
	Material[] RndMaterialsAry;
	Material[] HoldRndMaterialsAry;
	[HideInInspector] public string[] MaterialsNameAry;
		} else if (GetComponent<ParticleRenderer> ()) {
			#if (UNITY_EDITOR)
			if (!EditorApplication.isPlaying && !EditorApplication.isPaused) {
				if (RndMaterialsAry != GetComponent<ParticleRenderer> ().sharedMaterials)
					RndMaterialsAry = GetComponent<ParticleRenderer> ().sharedMaterials;
			} else {
				#endif
				if (RndMaterialsAry != GetComponent<ParticleRenderer> ().materials)
					RndMaterialsAry = GetComponent<ParticleRenderer> ().materials;
				#if (UNITY_EDITOR)
			}
			#endif
			if (RndMaterialsAry != HoldRndMaterialsAry) {
				MaterialsNameAry = new string[RndMaterialsAry.Length];
				for (int i=0; i<RndMaterialsAry.Length; i++) {
					if (RndMaterialsAry [i] != null)
						MaterialsNameAry [i] = "E" + i + ": " + RndMaterialsAry [i].name;
					else
						MaterialsNameAry [i] = "";
				}
				HoldRndMaterialsAry = RndMaterialsAry;
			}
		}
		*/
		
		
		if (RndMaterial != null) {
			MaterialName = RndMaterial.name;
		} else {
			MaterialName = string.Empty;
		}
		/*
		//old particle <ParticleRenderer> with some materials set
		if(!GetComponent<ParticleRenderer> ()) {
			if (RndMaterialsAry == null || RndMaterialsAry.Length != 1)
			RndMaterialsAry = new Material[1];
			if (MaterialsNameAry == null || MaterialsNameAry.Length != 1)
				MaterialsNameAry = new string[1];
			if (RndMaterial != null) {
				if (RndMaterial.name == "Default UI Material")
					MaterialsNameAry [0] = "";
				else
					MaterialsNameAry [0] = RndMaterial.name;
			} else {
				MaterialsNameAry [0] = "";
			}
		} else {
			if (RndMaterialsAry.Length > 0)
				MaterialNum = RndMaterialsAry.Length - 1;
			else
				MaterialNum = 0;
			if (RndMaterial != RndMaterialsAry [MaterialNum])
				RndMaterial = RndMaterialsAry [MaterialNum];
		}
		*/
		
		if (RndMaterial != null) {
			if (BaseImage == null) {
				if (MaskAllImpossibleCheck != true) {
					if (UseMask1)
						Debug.LogWarning ("[InstantMask] Checked UseMask1, but BaseImage is Not Found. So UseMask1 turn Off. GameObjegt name: " + gameObject.name);
					if (UseMask2)
						Debug.LogWarning ("[InstantMask] Checked UseMask2, but BaseImage is Not Found. So UseMask2 turn Off. GameObjegt name: " + gameObject.name);
					if (UseMask3)
						Debug.LogWarning ("[InstantMask] Checked UseMask3, but BaseImage is Not Found. So UseMask3 turn Off. GameObjegt name: " + gameObject.name);
				}
				MaskAllImpossibleCheck = true;
				UseMask1 = false;
				UseMask2 = false;
				UseMask3 = false;
			} else {
				MaskAllImpossibleCheck = false;
			}
			if (Mask1Image == null) {
				if (UseMask1 && UseMask1ImpossibleCheck != true)
					Debug.LogWarning ("[InstantMask] Checked UseMask1, but Mask1Image is Not Found. So UseMask1 turn Off. GameObjegt name: " + gameObject.name);
				UseMask1 = false;
				UseMask1ImpossibleCheck = true;
			} else {
				UseMask1ImpossibleCheck = false;
			}
			if (Mask2Image == null) {
				if (UseMask2 && UseMask2ImpossibleCheck != true)
					Debug.LogWarning ("[InstantMask] Checked UseMask2, but Mask2Image is Not Found. So UseMask2 turn Off. GameObjegt name: " + gameObject.name);
				UseMask2 = false;
				UseMask2ImpossibleCheck = true;
			} else {
				UseMask2ImpossibleCheck = false;
			}
			if (Mask3Image == null) {
				if (UseMask3 && UseMask3ImpossibleCheck != true)
					Debug.LogWarning ("[InstantMask] Checked UseMask3, but Mask3Image is Not Found. So UseMask3 turn Off. GameObjegt name: " + gameObject.name);
				UseMask3 = false;
				UseMask3ImpossibleCheck = true;
			} else {
				UseMask3ImpossibleCheck = false;
			}
			
			if (Mask1Image != null) {
				Mask2PrescribeCheck = true;
			} else {
				Mask2PrescribeCheck = false;
				Mask2Prescribe = 0;
			}
			if (Mask1Image != null || Mask2Image != null) {
				Mask3PrescribeCheck = true;
			} else {
				Mask3PrescribeCheck = false;
				Mask3Prescribe = 0;
			}
			
			
			//値とhold値のずれをチェック
			ConsistentAllSettings ();
			
			
			//shaderの疑似boolの値がおかしくならないように挟み込み
			if (RndMaterial.HasProperty ("_MaskVisibility")) {
				if (RndMaterial.GetInt ("_MaskVisibility") > 1)
					RndMaterial.SetInt ("_MaskVisibility", 1);
				else if (RndMaterial.GetInt ("_MaskVisibility") < 0)
					RndMaterial.SetInt ("_MaskVisibility", 0);
			}
			if (RndMaterial.HasProperty ("_UseMask1Check")) {
				if (RndMaterial.GetInt ("_UseMask1Check") > 1)
					RndMaterial.SetInt ("_UseMask1Check", 1);
				else if (RndMaterial.GetInt ("_UseMask1Check") < 0)
					RndMaterial.SetInt ("_UseMask1Check", 0);
			}
			if (RndMaterial.HasProperty ("_UseMask2Check")) {
				if (RndMaterial.GetInt ("_UseMask2Check") > 1)
					RndMaterial.SetInt ("_UseMask2Check", 1);
				else if (RndMaterial.GetInt ("_UseMask2Check") < 0)
					RndMaterial.SetInt ("_UseMask2Check", 0);
			}
			if (RndMaterial.HasProperty ("_UseMask3Check")) {
				if (RndMaterial.GetInt ("_UseMask3Check") > 1)
					RndMaterial.SetInt ("_UseMask3Check", 1);
				else if (RndMaterial.GetInt ("_UseMask3Check") < 0)
					RndMaterial.SetInt ("_UseMask3Check", 0);
			}
			if (RndMaterial.HasProperty ("_ReverseMask1")) {
				if (RndMaterial.GetInt ("_ReverseMask1") > 1)
					RndMaterial.SetInt ("_ReverseMask1", 1);
				else if (RndMaterial.GetInt ("_ReverseMask1") < 0)
					RndMaterial.SetInt ("_ReverseMask1", 0);
			}
			if (RndMaterial.HasProperty ("_ReverseMask2")) {
				if (RndMaterial.GetInt ("_ReverseMask2") > 1)
					RndMaterial.SetInt ("_ReverseMask2", 1);
				else if (RndMaterial.GetInt ("_ReverseMask2") < 0)
					RndMaterial.SetInt ("_ReverseMask2", 0);
			}
			if (RndMaterial.HasProperty ("_ReverseMask3")) {
				if (RndMaterial.GetInt ("_ReverseMask3") > 1)
					RndMaterial.SetInt ("_ReverseMask3", 1);
				else if (RndMaterial.GetInt ("_ReverseMask3") < 0)
					RndMaterial.SetInt ("_ReverseMask3", 0);
			}
			if (RndMaterial.HasProperty ("_GrayscaleMask1")) {
				if (RndMaterial.GetInt ("_GrayscaleMask1") > 1)
					RndMaterial.SetInt ("_GrayscaleMask1", 1);
				else if (RndMaterial.GetInt ("_GrayscaleMask1") < 0)
					RndMaterial.SetInt ("_GrayscaleMask1", 0);
			}
			if (RndMaterial.HasProperty ("_GrayscaleMask2")) {
				if (RndMaterial.GetInt ("_GrayscaleMask2") > 1)
					RndMaterial.SetInt ("_GrayscaleMask2", 1);
				else if (RndMaterial.GetInt ("_GrayscaleMask2") < 0)
					RndMaterial.SetInt ("_GrayscaleMask2", 0);
			}
			if (RndMaterial.HasProperty ("_GrayscaleMask3")) {
				if (RndMaterial.GetInt ("_GrayscaleMask3") > 1)
					RndMaterial.SetInt ("_GrayscaleMask3", 1);
				else if (RndMaterial.GetInt ("_GrayscaleMask3") < 0)
					RndMaterial.SetInt ("_GrayscaleMask3", 0);
			}
			//shaderの疑似bool挟み込みここまで
			
			
			//WrapModeをこのスクリプトのInspectorから変えたあと画像のInspectorを表示すると設定が巻き戻るので
			//WrapModeが変わるたびにwarningを出して知らせる
			#if UNITY_2017_1_OR_NEWER
			if (BaseImageWrapModeU != HoldBaseImageWrapModeU) {
				if (BaseImage != null) {
					if (BaseImageWrapModeU == 0)
						BaseImage.wrapModeU = TextureWrapMode.Repeat;
					else if (BaseImageWrapModeU == 1)
						BaseImage.wrapModeU = TextureWrapMode.Clamp;
					else if (BaseImageWrapModeU == 2)
						BaseImage.wrapModeU = TextureWrapMode.Mirror;
					else if (BaseImageWrapModeU == 3)
						BaseImage.wrapModeU = TextureWrapMode.MirrorOnce;
					else
						BaseImage.wrapModeU = TextureWrapMode.Clamp;
				}
				HoldBaseImageWrapModeU = BaseImageWrapModeU;
				#if (UNITY_EDITOR)
				BaseImageWrapUWarningCheck = false;
				#endif
			}
			if (BaseImageWrapModeV != HoldBaseImageWrapModeV) {
				if (BaseImage != null) {
					if (BaseImageWrapModeV == 0)
						BaseImage.wrapModeV = TextureWrapMode.Repeat;
					else if (BaseImageWrapModeV == 1)
						BaseImage.wrapModeV = TextureWrapMode.Clamp;
					else if (BaseImageWrapModeV == 2)
						BaseImage.wrapModeV = TextureWrapMode.Mirror;
					else if (BaseImageWrapModeV == 3)
						BaseImage.wrapModeV = TextureWrapMode.MirrorOnce;
					else
						BaseImage.wrapModeV = TextureWrapMode.Clamp;
				}
				HoldBaseImageWrapModeV = BaseImageWrapModeV;
				#if (UNITY_EDITOR)
				BaseImageWrapVWarningCheck = false;
				#endif
			}
			#if (UNITY_EDITOR)
			if (EditorApplication.isPlaying || EditorApplication.isPaused) {
				if (BaseImage != null) {
					if (BaseImageWrapUWarningCheck != true) {
						if (BaseImage.wrapModeU == TextureWrapMode.Repeat && BaseImageWrapModeU != 0) {
							if (BaseImageWrapModeU != -1)
								Debug.LogWarning ("[InstantMask] WrapMode of BaseImage has been changed, current that is 'Repeat'. GameObjegt name: " + gameObject.name);
							BaseImageWrapModeU = 0;
							HoldBaseImageWrapModeU = 0;
							BaseImageWrapUWarningCheck = true;
						} else if (BaseImage.wrapModeU == TextureWrapMode.Clamp && BaseImageWrapModeU != 1) {
							if (BaseImageWrapModeU != -1)
								Debug.LogWarning ("[InstantMask] WrapMode of BaseImage has been changed, current that is 'Clamp'. GameObjegt name: " + gameObject.name);
							BaseImageWrapModeU = 1;
							HoldBaseImageWrapModeU = 1;
							BaseImageWrapUWarningCheck = true;
						} else if (BaseImage.wrapModeU == TextureWrapMode.Mirror && BaseImageWrapModeU != 2) {
							if (BaseImageWrapModeU != -1)
								Debug.LogWarning ("[InstantMask] WrapMode of BaseImage has been changed, current that is 'Mirror'. GameObjegt name: " + gameObject.name);
							BaseImageWrapModeU = 2;
							HoldBaseImageWrapModeU = 2;
							BaseImageWrapUWarningCheck = true;
						} else if (BaseImage.wrapModeU == TextureWrapMode.MirrorOnce && BaseImageWrapModeU != 3) {
							if (BaseImageWrapModeU != -1)
								Debug.LogWarning ("[InstantMask] WrapMode of BaseImage has been changed, current that is 'MirrorOnce'. GameObjegt name: " + gameObject.name);
							BaseImageWrapModeU = 3;
							HoldBaseImageWrapModeU = 3;
							BaseImageWrapUWarningCheck = true;
						}
					} else {
						BaseImageWrapUWarningCheck = false;
					}
					if (BaseImageWrapVWarningCheck != true) {
						if (BaseImage.wrapModeV == TextureWrapMode.Repeat && BaseImageWrapModeV != 0) {
							if (BaseImageWrapModeV != -1)
								Debug.LogWarning ("[InstantMask] WrapMode of BaseImage has been changed, current that is 'Repeat'. GameObjegt name: " + gameObject.name);
							BaseImageWrapModeV = 0;
							HoldBaseImageWrapModeV = 0;
							BaseImageWrapVWarningCheck = true;
						} else if (BaseImage.wrapModeV == TextureWrapMode.Clamp && BaseImageWrapModeV != 1) {
							if (BaseImageWrapModeV != -1)
								Debug.LogWarning ("[InstantMask] WrapMode of BaseImage has been changed, current that is 'Clamp'. GameObjegt name: " + gameObject.name);
							BaseImageWrapModeV = 1;
							HoldBaseImageWrapModeV = 1;
							BaseImageWrapVWarningCheck = true;
						} else if (BaseImage.wrapModeV == TextureWrapMode.Mirror && BaseImageWrapModeV != 2) {
							if (BaseImageWrapModeV != -1)
								Debug.LogWarning ("[InstantMask] WrapMode of BaseImage has been changed, current that is 'Mirror'. GameObjegt name: " + gameObject.name);
							BaseImageWrapModeV = 2;
							HoldBaseImageWrapModeV = 2;
							BaseImageWrapVWarningCheck = true;
						} else if (BaseImage.wrapModeV == TextureWrapMode.MirrorOnce && BaseImageWrapModeV != 3) {
							if (BaseImageWrapModeV != -1)
								Debug.LogWarning ("[InstantMask] WrapMode of BaseImage has been changed, current that is 'MirrorOnce'. GameObjegt name: " + gameObject.name);
							BaseImageWrapModeV = 3;
							HoldBaseImageWrapModeV = 3;
							BaseImageWrapVWarningCheck = true;
						}
					} else {
						BaseImageWrapVWarningCheck = false;
					}
				}
			}
			#endif
			#else
			if (BaseImageOldWrapMode != HoldBaseImageOldWrapMode) {
				if (BaseImage != null) {
					if (BaseImageOldWrapMode == 0)
						BaseImage.wrapMode = TextureWrapMode.Repeat;
					else
						BaseImage.wrapMode = TextureWrapMode.Clamp;
				}
				HoldBaseImageOldWrapMode = BaseImageOldWrapMode;
				#if (UNITY_EDITOR)
				BaseImageRepeatWarningCheck = false;
				BaseImageClampWarningCheck = false;
				#endif
			}
			#if (UNITY_EDITOR)
			if (EditorApplication.isPlaying || EditorApplication.isPaused) {
				if (BaseImage != null) {
					if (BaseImage.wrapMode == TextureWrapMode.Repeat && BaseImageOldWrapMode != 0) {
						if (BaseImageRepeatWarningCheck != true && BaseImageOldWrapMode != -1)
							Debug.LogWarning ("[InstantMask] WrapMode of BaseImage has been changed, current that is 'Repeat'. GameObjegt name: " + gameObject.name);
						BaseImageOldWrapMode = 0;
						HoldBaseImageOldWrapMode = 0;
						BaseImageRepeatWarningCheck = true;
						BaseImageClampWarningCheck = false;
					} else if (BaseImage.wrapMode == TextureWrapMode.Clamp && BaseImageOldWrapMode != 1) {
						if (BaseImageClampWarningCheck != true && BaseImageOldWrapMode != -1)
							Debug.LogWarning ("[InstantMask] WrapMode of BaseImage has been changed, current that is 'Clamp'. GameObjegt name: " + gameObject.name);
						BaseImageOldWrapMode = 1;
						HoldBaseImageOldWrapMode = 1;
						BaseImageRepeatWarningCheck = false;
						BaseImageClampWarningCheck = true;
					}
				}
			}
			#endif
			#endif
			//WrapModeの整合ここまで
			
			//メイン値とshader値のずれをチェック(スクリプトセット直後＆shader側の値をいじった時用)
			if (RndMaterial.HasProperty ("_MainTex")) {
				if (BaseImage != null)
					RndMaterial.SetTexture ("_MainTex", BaseImage);
				else
					RndMaterial.mainTexture = null;
				if (RndMaterial.GetTextureScale ("_MainTex") != BaseImageTiling) {
					RndMaterial.SetTextureScale ("_MainTex", BaseImageTiling);
				}
				if (RndMaterial.GetTextureOffset ("_MainTex") != BaseImageOffset) {
					RndMaterial.SetTextureOffset ("_MainTex", BaseImageOffset);
				}
			}
			if (RndMaterial.HasProperty ("_MaskTex")) {
				if (Mask1Image != null)
					RndMaterial.SetTexture ("_MaskTex", Mask1Image);
				else
					RndMaterial.SetTexture ("_MaskTex", null);
				if (RndMaterial.GetTextureScale ("_MaskTex") != Mask1ImageTiling) {
					RndMaterial.SetTextureScale ("_MaskTex", Mask1ImageTiling);
				}
				if (RndMaterial.GetTextureOffset ("_MaskTex") != Mask1ImageOffset) {
					RndMaterial.SetTextureOffset ("_MaskTex", Mask1ImageOffset);
				}
			}
			if (RndMaterial.HasProperty ("_MaskTex2")) {
				if (Mask2Image != null)
					RndMaterial.SetTexture ("_MaskTex2", Mask2Image);
				else
					RndMaterial.SetTexture ("_MaskTex2", null);
				if (RndMaterial.GetTextureScale ("_MaskTex2") != Mask2ImageTiling) {
					RndMaterial.SetTextureScale ("_MaskTex2", Mask2ImageTiling);
				}
				if (RndMaterial.GetTextureOffset ("_MaskTex2") != Mask2ImageOffset) {
					RndMaterial.SetTextureOffset ("_MaskTex2", Mask2ImageOffset);
				}
			}
			if (RndMaterial.HasProperty ("_MaskTex3")) {
				if (Mask3Image != null)
					RndMaterial.SetTexture ("_MaskTex3", Mask3Image);
				else
					RndMaterial.SetTexture ("_MaskTex3", null);
				if (RndMaterial.GetTextureScale ("_MaskTex3") != Mask3ImageTiling) {
					RndMaterial.SetTextureScale ("_MaskTex3", Mask3ImageTiling);
				}
				if (RndMaterial.GetTextureOffset ("_MaskTex3") != Mask3ImageOffset) {
					RndMaterial.SetTextureOffset ("_MaskTex3", Mask3ImageOffset);
				}
			}
			if (RndMaterial.HasProperty ("_UseMask1Check")) {
				if (UseMask1 && RndMaterial.GetInt ("_UseMask1Check") == 0) {
					Debug.LogWarning ("[InstantMask] UseMask1 check turn Off. GameObjegt name: " + gameObject.name);
					UseMask1 = false;
					HoldUseMask1Check = false;
				}
				if (UseMask1 != true && RndMaterial.GetInt ("_UseMask1Check") == 1) {
					Debug.LogWarning ("[InstantMask] UseMask1 check turn On. GameObjegt name: " + gameObject.name);
					UseMask1 = true;
					HoldUseMask1Check = true;
				}
			}
			if (RndMaterial.HasProperty ("_UseMask2Check")) {
				if (UseMask2 && RndMaterial.GetInt ("_UseMask2Check") == 0) {
					Debug.LogWarning ("[InstantMask] UseMask2 check turn Off. GameObjegt name: " + gameObject.name);
					UseMask2 = false;
					HoldUseMask2Check = false;
				}
				if (UseMask2 != true && RndMaterial.GetInt ("_UseMask2Check") == 1) {
					Debug.LogWarning ("[InstantMask] UseMask2 check turn On. GameObjegt name: " + gameObject.name);
					UseMask2 = true;
					HoldUseMask2Check = true;
				}
			}
			if (RndMaterial.HasProperty ("_UseMask3Check")) {
				if (UseMask3 && RndMaterial.GetInt ("_UseMask3Check") == 0) {
					Debug.LogWarning ("[InstantMask] UseMask3 check turn Off. GameObjegt name: " + gameObject.name);
					UseMask3 = false;
					HoldUseMask3Check = false;
				}
				if (UseMask3 != true && RndMaterial.GetInt ("_UseMask3Check") == 1) {
					Debug.LogWarning ("[InstantMask] UseMask3 check turn On. GameObjegt name: " + gameObject.name);
					UseMask3 = true;
					HoldUseMask3Check = true;
				}
			}
			if (RndMaterial.HasProperty ("_ResultOpacity")) {
				if (ResultOpacity != RndMaterial.GetFloat ("_ResultOpacity")) {
					ResultOpacity = RndMaterial.GetFloat ("_ResultOpacity");
					HoldResultOpacity = RndMaterial.GetFloat ("_ResultOpacity");
				}
			}
			if (RndMaterial.HasProperty ("_AlphaCutoff")) {
				if (AlphaCutoff != RndMaterial.GetFloat ("_AlphaCutoff")) {
					AlphaCutoff = RndMaterial.GetFloat ("_AlphaCutoff");
					HoldAlphaCutoff = RndMaterial.GetFloat ("_AlphaCutoff");
				}
			}
			if (RndMaterial.HasProperty ("_MaskVisibility")) {
				if (MaskVisibility && RndMaterial.GetInt ("_MaskVisibility") == 0) {
					MaskVisibility = false;
					HoldMaskVisibility = false;
				}
				if (MaskVisibility != true && RndMaterial.GetInt ("_MaskVisibility") == 1) {
					MaskVisibility = true;
					HoldMaskVisibility = true;
				}
			}
			if (RndMaterial.HasProperty ("_VisibleColor")) {
				if (VisibleColor != RndMaterial.GetColor ("_VisibleColor")) {
					VisibleColor = RndMaterial.GetColor ("_VisibleColor");
					HoldVisibleColor = RndMaterial.GetColor ("_VisibleColor");
				}
			}
			if (RndMaterial.HasProperty ("_Mask1Degree")) {
				if (Mask1Degree != RndMaterial.GetFloat ("_Mask1Degree")) {
					Mask1Degree = RndMaterial.GetFloat ("_Mask1Degree");
					HoldMask1Degree = RndMaterial.GetFloat ("_Mask1Degree");
				}
			}
			if (RndMaterial.HasProperty ("_Mask1TilingPivotToCenter")) {
				if (Mask1TilingPivotToCenter && RndMaterial.GetInt ("_Mask1TilingPivotToCenter") == 0) {
					Mask1TilingPivotToCenter = false;
					HoldMask1TilingPivotToCenter = false;
				}
				if (Mask1TilingPivotToCenter != true && RndMaterial.GetInt ("_Mask1TilingPivotToCenter") == 1) {
					Mask1TilingPivotToCenter = true;
					HoldMask1TilingPivotToCenter = true;
				}
			}
			if (RndMaterial.HasProperty ("_Mask1PosX") && RndMaterial.HasProperty ("_Mask1PosY")) {
				if (Mask1Position.x != RndMaterial.GetFloat ("_Mask1PosX") || Mask1Position.y != RndMaterial.GetFloat ("_Mask1PosY")) {
					Mask1Position = new Vector2 (RndMaterial.GetFloat ("_Mask1PosX"), RndMaterial.GetFloat ("_Mask1PosY"));
					HoldMask1Pos = new Vector2 (RndMaterial.GetFloat ("_Mask1PosX"), RndMaterial.GetFloat ("_Mask1PosY"));
				}
			}
			if (RndMaterial.HasProperty ("_Mask1Angle")) {
				if (Mask1Angle != RndMaterial.GetFloat ("_Mask1Angle")) {
					Mask1Angle = RndMaterial.GetFloat ("_Mask1Angle");
					HoldMask1Angle = RndMaterial.GetFloat ("_Mask1Angle");
				}
			}
			if (RndMaterial.HasProperty ("_ReverseMask1")) {
				if (ReverseMask1Area && RndMaterial.GetInt ("_ReverseMask1") == 0) {
					ReverseMask1Area = false;
					HoldReverseMask1AreaCheck = false;
				}
				if (ReverseMask1Area != true && RndMaterial.GetInt ("_ReverseMask1") == 1) {
					ReverseMask1Area = true;
					HoldReverseMask1AreaCheck = true;
				}
			}
			if (RndMaterial.HasProperty ("_GrayscaleMask1")) {
				if (GrayscaleMask1 && RndMaterial.GetInt ("_GrayscaleMask1") == 0) {
					GrayscaleMask1 = false;
					HoldGrayscaleMask1Check = false;
				}
				if (GrayscaleMask1 != true && RndMaterial.GetInt ("_GrayscaleMask1") == 1) {
					GrayscaleMask1 = true;
					HoldGrayscaleMask1Check = true;
				}
			}
			if (RndMaterial.HasProperty ("_Mask2Degree")) {
				if (Mask2Degree != RndMaterial.GetFloat ("_Mask2Degree")) {
					Mask2Degree = RndMaterial.GetFloat ("_Mask2Degree");
					HoldMask2Degree = RndMaterial.GetFloat ("_Mask2Degree");
				}
			}
			if (RndMaterial.HasProperty ("_Mask2TilingPivotToCenter")) {
				if (Mask2TilingPivotToCenter && RndMaterial.GetInt ("_Mask2TilingPivotToCenter") == 0) {
					Mask2TilingPivotToCenter = false;
					HoldMask2TilingPivotToCenter = false;
				}
				if (Mask2TilingPivotToCenter != true && RndMaterial.GetInt ("_Mask2TilingPivotToCenter") == 1) {
					Mask2TilingPivotToCenter = true;
					HoldMask2TilingPivotToCenter = true;
				}
			}
			if (RndMaterial.HasProperty ("_Mask2PosX") && RndMaterial.HasProperty ("_Mask2PosY")) {
				if (Mask2Position.x != RndMaterial.GetFloat ("_Mask2PosX") || Mask2Position.y != RndMaterial.GetFloat ("_Mask2PosY")) {
					Mask2Position = new Vector2 (RndMaterial.GetFloat ("_Mask2PosX"), RndMaterial.GetFloat ("_Mask2PosY"));
					HoldMask2Pos = new Vector2 (RndMaterial.GetFloat ("_Mask2PosX"), RndMaterial.GetFloat ("_Mask2PosY"));
				}
			}
			if (RndMaterial.HasProperty ("_Mask2Angle")) {
				if (Mask2Angle != RndMaterial.GetFloat ("_Mask2Angle")) {
					Mask2Angle = RndMaterial.GetFloat ("_Mask2Angle");
					HoldMask2Angle = RndMaterial.GetFloat ("_Mask2Angle");
				}
			}
			if (RndMaterial.HasProperty ("_Mask2Prescribe")) {
				if (Mask2Prescribe != RndMaterial.GetInt ("_Mask2Prescribe")) {
					Mask2Prescribe = RndMaterial.GetInt ("_Mask2Prescribe");
					HoldMask2Prescribe = RndMaterial.GetInt ("_Mask2Prescribe");
				}
			}
			if (RndMaterial.HasProperty ("_ReverseMask2")) {
				if (ReverseMask2Area && RndMaterial.GetInt ("_ReverseMask2") == 0) {
					ReverseMask2Area = false;
					HoldReverseMask2AreaCheck = false;
				}
				if (ReverseMask2Area != true && RndMaterial.GetInt ("_ReverseMask2") == 1) {
					ReverseMask2Area = true;
					HoldReverseMask2AreaCheck = true;
				}
			}
			if (RndMaterial.HasProperty ("_GrayscaleMask2")) {
				if (GrayscaleMask2 && RndMaterial.GetInt ("_GrayscaleMask2") == 0) {
					GrayscaleMask2 = false;
					HoldGrayscaleMask2Check = false;
				}
				if (GrayscaleMask2 != true && RndMaterial.GetInt ("_GrayscaleMask2") == 1) {
					GrayscaleMask2 = true;
					HoldGrayscaleMask2Check = true;
				}
			}
			if (RndMaterial.HasProperty ("_Mask3Degree")) {
				if (Mask3Degree != RndMaterial.GetFloat ("_Mask3Degree")) {
					Mask3Degree = RndMaterial.GetFloat ("_Mask3Degree");
					HoldMask3Degree = RndMaterial.GetFloat ("_Mask3Degree");
				}
			}
			if (RndMaterial.HasProperty ("_Mask3TilingPivotToCenter")) {
				if (Mask3TilingPivotToCenter && RndMaterial.GetInt ("_Mask3TilingPivotToCenter") == 0) {
					Mask3TilingPivotToCenter = false;
					HoldMask3TilingPivotToCenter = false;
				}
				if (Mask3TilingPivotToCenter != true && RndMaterial.GetInt ("_Mask3TilingPivotToCenter") == 1) {
					Mask3TilingPivotToCenter = true;
					HoldMask3TilingPivotToCenter = true;
				}
			}
			if (RndMaterial.HasProperty ("_Mask3PosX") && RndMaterial.HasProperty ("_Mask3PosY")) {
				if (Mask3Position.x != RndMaterial.GetFloat ("_Mask3PosX") || Mask3Position.y != RndMaterial.GetFloat ("_Mask3PosY")) {
					Mask3Position = new Vector2 (RndMaterial.GetFloat ("_Mask3PosX"), RndMaterial.GetFloat ("_Mask3PosY"));
					HoldMask3Pos = new Vector2 (RndMaterial.GetFloat ("_Mask3PosX"), RndMaterial.GetFloat ("_Mask3PosY"));
				}
			}
			if (RndMaterial.HasProperty ("_Mask3Angle")) {
				if (Mask3Angle != RndMaterial.GetFloat ("_Mask3Angle")) {
					Mask3Angle = RndMaterial.GetFloat ("_Mask3Angle");
					HoldMask3Angle = RndMaterial.GetFloat ("_Mask3Angle");
				}
			}
			if (RndMaterial.HasProperty ("_Mask3Prescribe")) {
				if (Mask3Prescribe != RndMaterial.GetInt ("_Mask3Prescribe")) {
					Mask3Prescribe = RndMaterial.GetInt ("_Mask3Prescribe");
					HoldMask3Prescribe = RndMaterial.GetInt ("_Mask3Prescribe");
				}
			}
			if (RndMaterial.HasProperty ("_ReverseMask3")) {
				if (ReverseMask3Area && RndMaterial.GetInt ("_ReverseMask3") == 0) {
					ReverseMask3Area = false;
					HoldReverseMask3AreaCheck = false;
				}
				if (ReverseMask3Area != true && RndMaterial.GetInt ("_ReverseMask3") == 1) {
					ReverseMask3Area = true;
					HoldReverseMask3AreaCheck = true;
				}
			}
			if (RndMaterial.HasProperty ("_GrayscaleMask3")) {
				if (GrayscaleMask3 && RndMaterial.GetInt ("_GrayscaleMask3") == 0) {
					GrayscaleMask3 = false;
					HoldGrayscaleMask3Check = false;
				}
				if (GrayscaleMask3 != true && RndMaterial.GetInt ("_GrayscaleMask3") == 1) {
					GrayscaleMask3 = true;
					HoldGrayscaleMask3Check = true;
				}
			}
			//メイン値とshader値のずれをチェックここまで
			
			if (Mask2Prescribe == 2) {
				if (ReverseMask2Area) {
					Mask2Prescribe = 0;
					Debug.LogWarning ("[InstantMask] When ReverseMask2Area is On, Mask2Prescribe is not able to chosen 'Invert'. So that have been changed 'Hide'. GameObjegt name: " + gameObject.name);
				}
			}
			if (Mask3Prescribe == 2) {
				if (ReverseMask3Area) {
					Mask3Prescribe = 0;
					Debug.LogWarning ("[InstantMask] When ReverseMask3Area is On, Mask3Prescribe is not able to chosen 'Invert'. So that have been changed 'Hide'. GameObjegt name: " + gameObject.name);
				}
			}
			
			if (Mask1NullCheck && Mask1Image != null) {
				Mask1NullCheck = false;
				MaskListRemakeCheck = true;
			}
			if (Mask1NullCheck != true && Mask1Image == null) {
				Mask1NullCheck = true;
				MaskListRemakeCheck = true;
			}
			if (Mask2NullCheck && Mask2Image != null) {
				Mask2NullCheck = false;
				MaskListRemakeCheck = true;
			}
			if (Mask2NullCheck != true && Mask2Image == null) {
				Mask2NullCheck = true;
				MaskListRemakeCheck = true;
			}
			if (Mask3NullCheck && Mask3Image != null) {
				Mask3NullCheck = false;
				MaskListRemakeCheck = true;
			}
			if (Mask3NullCheck != true && Mask3Image == null) {
				Mask3NullCheck = true;
				MaskListRemakeCheck = true;
			}
			if (MaskListRemakeCheck) {
				MaskStringList.Clear ();
				MaskNumberList.Clear ();
				if (Mask1NullCheck != true) {
					MaskStringList.Add ("Mask1 Image");
					MaskNumberList.Add (0);
				}
				if (Mask2NullCheck != true) {
					MaskStringList.Add ("Mask2 Image");
					MaskNumberList.Add (1);
				}
				if (Mask3NullCheck != true) {
					MaskStringList.Add ("Mask3 Image");
					MaskNumberList.Add (2);
				}
				MaskStringAry = MaskStringList.ToArray ();
				MaskNumberAry = MaskNumberList.ToArray ();
				if (MaskNumberAry.Length <= 0) {
					VisibleMaskNumber = 0;
				} else {
					if (MaskNumberList.IndexOf (VisibleMaskNumber) == -1)
						VisibleMaskNumber = MaskNumberAry [0];
				}
			}
			if (RndMaterial.HasProperty ("_VisibleMaskNumber")) {
				RndMaterial.SetInt ("_VisibleMaskNumber", VisibleMaskNumber);
			}
			
			#if (UNITY_EDITOR)
			if (!EditorApplication.isPlaying && !EditorApplication.isPaused) {
				EditorCheck = true;
			} else {
				EditorCheck = false;
			}
			#endif
			
		}
		
	}
	
	#if (UNITY_EDITOR)
	public void UndoRedoSet ()
	{
		UndoRedoSetCheck = true;
		ConsistentAllSettings ();
	}
	#endif
	
	void ConsistentAllSettings ()
	{
		if (UseMask1 != HoldUseMask1Check || UndoRedoSetCheck) {
			HoldUseMask1Check = UseMask1;
			if (RndMaterial.HasProperty ("_UseMask1Check")) {
				if (UseMask1 != true)
					RndMaterial.SetInt ("_UseMask1Check", 0);
				else
					RndMaterial.SetInt ("_UseMask1Check", 1);
			}
		}
		if (UseMask2 != HoldUseMask2Check || UndoRedoSetCheck) {
			HoldUseMask2Check = UseMask2;
			if (RndMaterial.HasProperty ("_UseMask2Check")) {
				if (UseMask2 != true)
					RndMaterial.SetInt ("_UseMask2Check", 0);
				else
					RndMaterial.SetInt ("_UseMask2Check", 1);
			}
		}
		if (UseMask3 != HoldUseMask3Check || UndoRedoSetCheck) {
			HoldUseMask3Check = UseMask3;
			if (RndMaterial.HasProperty ("_UseMask3Check")) {
				if (UseMask3 != true)
					RndMaterial.SetInt ("_UseMask3Check", 0);
				else
					RndMaterial.SetInt ("_UseMask3Check", 1);
			}
		}
		if (ResultOpacity != HoldResultOpacity || UndoRedoSetCheck) {
			HoldResultOpacity = ResultOpacity;
			if (RndMaterial.HasProperty ("_ResultOpacity"))
				RndMaterial.SetFloat ("_ResultOpacity", ResultOpacity);
		}
		if (AlphaCutoff != HoldAlphaCutoff || UndoRedoSetCheck) {
			HoldAlphaCutoff = AlphaCutoff;
			if (RndMaterial.HasProperty ("_AlphaCutoff"))
				RndMaterial.SetFloat ("_AlphaCutoff", AlphaCutoff);
		}
		if (MaskVisibility != HoldMaskVisibility || UndoRedoSetCheck) {
			HoldMaskVisibility = MaskVisibility;
			if (RndMaterial.HasProperty ("_MaskVisibility")) {
				if (MaskVisibility != true)
					RndMaterial.SetInt ("_MaskVisibility", 0);
				else
					RndMaterial.SetInt ("_MaskVisibility", 1);
			}
		}
		if (VisibleColor != HoldVisibleColor || UndoRedoSetCheck) {
			HoldVisibleColor = VisibleColor;
			if (RndMaterial.HasProperty ("_VisibleColor"))
				RndMaterial.SetColor ("_VisibleColor", VisibleColor);
		}
		if (Mask1Degree != HoldMask1Degree || UndoRedoSetCheck) {
			HoldMask1Degree = Mask1Degree;
			if (RndMaterial.HasProperty ("_Mask1Degree"))
				RndMaterial.SetFloat ("_Mask1Degree", Mask1Degree);
		}
		if (Mask1TilingPivotToCenter != HoldMask1TilingPivotToCenter || UndoRedoSetCheck) {
			HoldMask1TilingPivotToCenter = Mask1TilingPivotToCenter;
			if (RndMaterial.HasProperty ("_Mask1TilingPivotToCenter")) {
				if (Mask1TilingPivotToCenter != true)
					RndMaterial.SetInt ("_Mask1TilingPivotToCenter", 0);
				else
					RndMaterial.SetInt ("_Mask1TilingPivotToCenter", 1);
			}
		}
		if (Mask1Position.x != HoldMask1Pos.x || Mask1Position.y != HoldMask1Pos.y || UndoRedoSetCheck) {
			HoldMask1Pos = new Vector2 (Mask1Position.x, Mask1Position.y);
			if (RndMaterial.HasProperty ("_Mask1PosX"))
				RndMaterial.SetFloat ("_Mask1PosX", Mask1Position.x);
			if (RndMaterial.HasProperty ("_Mask1PosY"))
				RndMaterial.SetFloat ("_Mask1PosY", Mask1Position.y);
		}
		if (Mask1Angle != HoldMask1Angle || UndoRedoSetCheck) {
			HoldMask1Angle = Mask1Angle;
			if (RndMaterial.HasProperty ("_Mask1Angle"))
				RndMaterial.SetFloat ("_Mask1Angle", Mask1Angle);
		}
		if (ReverseMask1Area != HoldReverseMask1AreaCheck || UndoRedoSetCheck) {
			HoldReverseMask1AreaCheck = ReverseMask1Area;
			if (RndMaterial.HasProperty ("_ReverseMask1")) {
				if (ReverseMask1Area != true)
					RndMaterial.SetInt ("_ReverseMask1", 0);
				else
					RndMaterial.SetInt ("_ReverseMask1", 1);
			}
		}
		if (GrayscaleMask1 != HoldGrayscaleMask1Check || UndoRedoSetCheck) {
			HoldGrayscaleMask1Check = GrayscaleMask1;
			if (RndMaterial.HasProperty ("_GrayscaleMask1")) {
				if (GrayscaleMask1 != true)
					RndMaterial.SetInt ("_GrayscaleMask1", 0);
				else
					RndMaterial.SetInt ("_GrayscaleMask1", 1);
			}
		}
		if (Mask2Degree != HoldMask2Degree || UndoRedoSetCheck) {
			HoldMask2Degree = Mask2Degree;
			if (RndMaterial.HasProperty ("_Mask2Degree"))
				RndMaterial.SetFloat ("_Mask2Degree", Mask2Degree);
		}
		if (Mask2TilingPivotToCenter != HoldMask2TilingPivotToCenter || UndoRedoSetCheck) {
			HoldMask2TilingPivotToCenter = Mask2TilingPivotToCenter;
			if (RndMaterial.HasProperty ("_Mask2TilingPivotToCenter")) {
				if (Mask2TilingPivotToCenter != true)
					RndMaterial.SetInt ("_Mask2TilingPivotToCenter", 0);
				else
					RndMaterial.SetInt ("_Mask2TilingPivotToCenter", 1);
			}
		}
		if (Mask2Position.x != HoldMask2Pos.x || Mask2Position.y != HoldMask2Pos.y || UndoRedoSetCheck) {
			HoldMask2Pos = new Vector2 (Mask2Position.x, Mask2Position.y);
			if (RndMaterial.HasProperty ("_Mask2PosX"))
				RndMaterial.SetFloat ("_Mask2PosX", Mask2Position.x);
			if (RndMaterial.HasProperty ("_Mask2PosY"))
				RndMaterial.SetFloat ("_Mask2PosY", Mask2Position.y);
		}
		if (Mask2Angle != HoldMask2Angle || UndoRedoSetCheck) {
			HoldMask2Angle = Mask2Angle;
			if (RndMaterial.HasProperty ("_Mask2Angle"))
				RndMaterial.SetFloat ("_Mask2Angle", Mask2Angle);
		}
		if (Mask2Prescribe != HoldMask2Prescribe || UndoRedoSetCheck) {
			HoldMask2Prescribe = Mask2Prescribe;
			if (RndMaterial.HasProperty ("_Mask2Prescribe"))
				RndMaterial.SetInt ("_Mask2Prescribe", Mask2Prescribe);
		}
		if (ReverseMask2Area != HoldReverseMask2AreaCheck || UndoRedoSetCheck) {
			HoldReverseMask2AreaCheck = ReverseMask2Area;
			if (RndMaterial.HasProperty ("_ReverseMask2")) {
				if (ReverseMask2Area != true)
					RndMaterial.SetInt ("_ReverseMask2", 0);
				else
					RndMaterial.SetInt ("_ReverseMask2", 1);
			}
		}
		if (GrayscaleMask2 != HoldGrayscaleMask2Check || UndoRedoSetCheck) {
			HoldGrayscaleMask2Check = GrayscaleMask2;
			if (RndMaterial.HasProperty ("_GrayscaleMask2")) {
				if (GrayscaleMask2 != true)
					RndMaterial.SetInt ("_GrayscaleMask2", 0);
				else
					RndMaterial.SetInt ("_GrayscaleMask2", 1);
			}
		}
		if (Mask3Degree != HoldMask3Degree || UndoRedoSetCheck) {
			HoldMask3Degree = Mask3Degree;
			if (RndMaterial.HasProperty ("_Mask3Degree"))
				RndMaterial.SetFloat ("_Mask3Degree", Mask3Degree);
		}
		if (Mask3TilingPivotToCenter != HoldMask3TilingPivotToCenter || UndoRedoSetCheck) {
			HoldMask3TilingPivotToCenter = Mask3TilingPivotToCenter;
			if (RndMaterial.HasProperty ("_Mask3TilingPivotToCenter")) {
				if (Mask3TilingPivotToCenter != true)
					RndMaterial.SetInt ("_Mask3TilingPivotToCenter", 0);
				else
					RndMaterial.SetInt ("_Mask3TilingPivotToCenter", 1);
			}
		}
		if (Mask3Position.x != HoldMask3Pos.x || Mask3Position.y != HoldMask3Pos.y || UndoRedoSetCheck) {
			HoldMask3Pos = new Vector3 (Mask3Position.x, Mask3Position.y);
			if (RndMaterial.HasProperty ("_Mask3PosX"))
				RndMaterial.SetFloat ("_Mask3PosX", Mask3Position.x);
			if (RndMaterial.HasProperty ("_Mask3PosY"))
				RndMaterial.SetFloat ("_Mask3PosY", Mask3Position.y);
		}
		if (Mask3Angle != HoldMask3Angle || UndoRedoSetCheck) {
			HoldMask3Angle = Mask3Angle;
			if (RndMaterial.HasProperty ("_Mask3Angle"))
				RndMaterial.SetFloat ("_Mask3Angle", Mask3Angle);
		}
		if (Mask3Prescribe != HoldMask3Prescribe || UndoRedoSetCheck) {
			HoldMask3Prescribe = Mask3Prescribe;
			if (RndMaterial.HasProperty ("_Mask3Prescribe"))
				RndMaterial.SetInt ("_Mask3Prescribe", Mask3Prescribe);
		}
		if (ReverseMask3Area != HoldReverseMask3AreaCheck || UndoRedoSetCheck) {
			HoldReverseMask3AreaCheck = ReverseMask3Area;
			if (RndMaterial.HasProperty ("_ReverseMask3")) {
				if (ReverseMask3Area != true)
					RndMaterial.SetInt ("_ReverseMask3", 0);
				else
					RndMaterial.SetInt ("_ReverseMask3", 1);
			}
		}
		if (GrayscaleMask3 != HoldGrayscaleMask3Check || UndoRedoSetCheck) {
			HoldGrayscaleMask3Check = GrayscaleMask3;
			if (RndMaterial.HasProperty ("_GrayscaleMask3")) {
				if (GrayscaleMask3 != true)
					RndMaterial.SetInt ("_GrayscaleMask3", 0);
				else
					RndMaterial.SetInt ("_GrayscaleMask3", 1);
			}
		}
		if (BaseImageTiling != HoldBaseImageTiling || UndoRedoSetCheck) {
			HoldBaseImageTiling = BaseImageTiling;
			if (RndMaterial.HasProperty ("_MainTex"))
				RndMaterial.SetTextureScale ("_MainTex", BaseImageTiling);
		}
		if (BaseImageOffset != HoldBaseImageOffset || UndoRedoSetCheck) {
			HoldBaseImageOffset = BaseImageOffset;
			if (RndMaterial.HasProperty ("_MainTex"))
				RndMaterial.SetTextureOffset ("_MainTex", BaseImageOffset);
		}
		if (Mask1ImageTiling != HoldMask1ImageTiling || UndoRedoSetCheck) {
			HoldMask1ImageTiling = Mask1ImageTiling;
			if (RndMaterial.HasProperty ("_MaskTex"))
				RndMaterial.SetTextureScale ("_MaskTex", Mask1ImageTiling);
		}
		if (Mask1ImageOffset != HoldMask1ImageOffset || UndoRedoSetCheck) {
			HoldMask1ImageOffset = Mask1ImageOffset;
			if (RndMaterial.HasProperty ("_MaskTex"))
				RndMaterial.SetTextureOffset ("_MaskTex", Mask1ImageOffset);
		}
		if (Mask2ImageTiling != HoldMask2ImageTiling || UndoRedoSetCheck) {
			HoldMask2ImageTiling = Mask2ImageTiling;
			if (RndMaterial.HasProperty ("_MaskTex2"))
				RndMaterial.SetTextureScale ("_MaskTex2", Mask2ImageTiling);
		}
		if (Mask2ImageOffset != HoldMask2ImageOffset || UndoRedoSetCheck) {
			HoldMask2ImageOffset = Mask2ImageOffset;
			if (RndMaterial.HasProperty ("_MaskTex2"))
				RndMaterial.SetTextureOffset ("_MaskTex2", Mask2ImageOffset);
		}
		if (Mask3ImageTiling != HoldMask3ImageTiling || UndoRedoSetCheck) {
			HoldMask3ImageTiling = Mask3ImageTiling;
			if (RndMaterial.HasProperty ("_MaskTex3"))
				RndMaterial.SetTextureScale ("_MaskTex3", Mask3ImageTiling);
		}
		if (Mask3ImageOffset != HoldMask3ImageOffset || UndoRedoSetCheck) {
			HoldMask3ImageOffset = Mask3ImageOffset;
			if (RndMaterial.HasProperty ("_MaskTex3"))
				RndMaterial.SetTextureOffset ("_MaskTex3", Mask3ImageOffset);
		}
		UndoRedoSetCheck = false;
	}
	
	[HideInInspector, SerializeField] Texture HoldBaseImage;
	
	public Texture BaseImage {
		get { return HoldBaseImage; }
		set {
			if (value != HoldBaseImage) {
				HoldBaseImage = value;
				if (RndMaterial != null && RndMaterial.HasProperty ("_MainTex"))
					RndMaterial.SetTexture ("_MainTex", HoldBaseImage);
			}
		}
	}
	
	[HideInInspector, SerializeField] Texture2D HoldMask1Image;
	
	public Texture2D Mask1Image {
		get { return HoldMask1Image; }
		set {
			if (value != HoldMask1Image) {
				HoldMask1Image = value;
				if (HoldMask1Image)
					UseMask1 = true;
				if (RndMaterial != null && RndMaterial.HasProperty ("_MaskTex"))
					RndMaterial.SetTexture ("_MaskTex", HoldMask1Image);
			}
		}
	}
	
	[HideInInspector, SerializeField] Texture2D HoldMask2Image;
	
	public Texture2D Mask2Image {
		get { return HoldMask2Image; }
		set {
			if (value != HoldMask2Image) {
				HoldMask2Image = value;
				if (HoldMask2Image)
					UseMask2 = true;
				if (RndMaterial != null && RndMaterial.HasProperty ("_MaskTex2"))
					RndMaterial.SetTexture ("_MaskTex2", HoldMask2Image);
			}
		}
	}
	
	[HideInInspector, SerializeField] Texture2D HoldMask3Image;
	
	public Texture2D Mask3Image {
		get { return HoldMask3Image; }
		set {
			if (value != HoldMask3Image) {
				HoldMask3Image = value;
				if (HoldMask3Image)
					UseMask3 = true;
				if (RndMaterial != null && RndMaterial.HasProperty ("_MaskTex3"))
					RndMaterial.SetTexture ("_MaskTex3", HoldMask3Image);
			}
		}
	}
	
}
