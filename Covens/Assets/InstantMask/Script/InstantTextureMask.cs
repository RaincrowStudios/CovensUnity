using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
#if (UNITY_EDITOR)
using System;
using System.IO;
using UnityEditor;
#endif

[ExecuteInEditMode()]
public class InstantTextureMask : MonoBehaviour
{
	public Texture2D Result;
	[HideInInspector]
	public bool
		UseUnityGUI;
	[HideInInspector]
	public bool
		HoldUseUnityGUI;
	[HideInInspector]
	public Vector2
		ResultSize;
	Vector2 HoldResultSize;
	[HideInInspector]
	public Rect
		MaskRect;
	float HoldMaskRectX;
	float HoldMaskRectY;
	float HoldMaskRectW;
	float HoldMaskRectH;
	[HideInInspector]
	public bool
		MaskVisibility;
	[HideInInspector]
	public Color
		VisibleColor = new Color (0, 0.8f, 0, 0.5f);
	[HideInInspector]
	public bool
		ReverseMaskArea;
	bool HoldReverseMaskArea;
	[HideInInspector]
	public bool
		GrayscaleMask;
	bool HoldGrayscaleMask;
	[HideInInspector]
	public bool
		UseMaskRotate;
	[HideInInspector]
	public float
		ResultOpacity = 100;
	float HoldResultOpacity;
	[HideInInspector]
	public float
		MaskDegree = 100;
	float HoldMaskDegree;
	[HideInInspector]
	public int
		ResultWrapModeU = 1;
	[HideInInspector]
	public int
		ResultWrapModeV = 1;
	[HideInInspector]
	public int
		ResultOldWrapMode = 1;
	RectTransform thisRectTrns;
	RectTransform MaskDisplayRectTrns;
	RawImage MaskDisplayComponent;
	CanvasRenderer CvRend;
	[HideInInspector]
	public GameObject
		MaskDisplayGO;
	[HideInInspector]
	public GameObject
		MaskDisplayUGUI;
	Transform DisplayGOTrns;
	Texture2D rotMountTex;
	Image imageComponent;
	RawImage rawimageComponent;
	Color NoDisplayColor = new Color (0, 0, 0, 0);
	bool GrayAlphaCheck;
	Color[] BasePixels;
	Color[] BasePixelprocs;
	Color[] mpixels;
	List<Color> mpixelsList;
	List<Color> mpixelsRotList;
	Color[] MaskResultAry;
	int maskWidth;
	int maskHeight;
	int maskRotWidth;
	int maskRotHeight;
	bool UGUIErrorCheck;
	bool BaseImageNullCheck;
	bool MaskImageNullCheck;
	bool MaskImageNullRendCheck;
	bool BaseImageChangeCheck;
	bool maskImageChangeCheck;
	bool maskShapeChangeCheck;
	bool maskRotateChangeCheck;
	bool MaskRotateOffSwitchCheck;
	bool targetSizeChangeCheck;
	bool OpacityChangeCheck;
	bool ResultApplyCheck;
	float MnMaskAngle;
	bool MaskMoveLoopExit;
	[HideInInspector]
	public bool
		AncXDifCheck;
	[HideInInspector]
	public bool
		AncYDifCheck;
	float[] CosTable;
	float[] SinTable;
	bool TrigonomTableMakeCheck;
	int PytAdjustWidth;
	int PytAdjustHeight;
	int PytInt;
	[HideInInspector]
	public bool
		MaskingDisabledCheck;
	[HideInInspector]
	public bool
		MaskOutRangeCheck;
	public static InstantTextureMask IMScript;
	
	void Start ()
	{
		MaskDisplayGO = null;
		MaskDisplayUGUI = null;
		BasePixels = new Color[0];
		TrigonomTableMake ();
		Update ();
	}
	
	void Update ()
	{
		if (BaseImage == null) {
			if (Masking && BaseImageNullCheck != true) {
				Debug.LogWarning ("[InstantMask] Checked Masking, but BaseImage is Not Found. So Masking turn Off. GameObjegt name: " + gameObject.name);
				Masking = false;
			}
			BaseImageNullCheck = true;
			MaskingDisabledCheck = true;
			Result = null;
			if (UseUnityGUI) {
				if (GetComponent<Image> ())
					GetComponent<Image> ().sprite = null;
				if (GetComponent<RawImage> ())
					GetComponent<RawImage> ().texture = null;
			}
			return;
		}
		if (BaseImage != null && BaseImageNullCheck) {
			BaseImageNullCheck = false;
			ResultSize = new Vector2 (BaseImage.width, BaseImage.height);
		}
		//MaskImageがあるか判定
		if (MaskImage == null) {
			if (Masking && MaskImageNullCheck != true) {
				Debug.LogWarning ("[InstantMask] Checked Masking, but MaskImage is Not Found. So Masking turn Off. GameObjegt name: " + gameObject.name);
				Masking = false;
			}
			if (MaskImageNullCheck != true)
				MaskImageNullRendCheck = true;
			MaskImageNullCheck = true;
			MaskingDisabledCheck = true;
		}
		if (MaskImage != null && MaskImageNullCheck) {
			MaskImageNullCheck = false;
			MaskImageNullRendCheck = false;
			MaskRect = new Rect ((MaskImage.width / 2), (MaskImage.height / 2), MaskImage.width, MaskImage.height);
		}
		if (MaskImage != null && BaseImage != null) {
			MaskingDisabledCheck = false;
		}
		ResultSize.x = Mathf.Min (Mathf.Max (ResultSize.x, 0), 8192);
		ResultSize.y = Mathf.Min (Mathf.Max (ResultSize.y, 0), 8192);
		MaskRect.width = Mathf.Min (Mathf.Max (MaskRect.width, 0), 8192);
		MaskRect.height = Mathf.Min (Mathf.Max (MaskRect.height, 0), 8192);
		Pixelization = Mathf.Min (Mathf.Max (Pixelization, 0), 50);
		if (MaskAngle >= 360 || MaskAngle <= -360)
			MaskAngle = MaskAngle % 360;
		if (ResultOpacity < 0.0f)
			ResultOpacity = 0.0f;
		else if (ResultOpacity > 100.0f)
			ResultOpacity = 100.0f;
		if (MaskDegree < 0.0f)
			MaskDegree = 0.0f;
		else if (MaskDegree > 100.0f)
			MaskDegree = 100.0f;
		int PixelizationCalValue = 51 - (Pixelization);
		if (ResultWrapModeU < 0)
			ResultWrapModeU = 0;
		else if (ResultWrapModeU > 3)
			ResultWrapModeU = 1;
		if (ResultWrapModeV < 0)
			ResultWrapModeV = 0;
		else if (ResultWrapModeV > 3)
			ResultWrapModeV = 1;
		if (ResultOldWrapMode < 0)
			ResultOldWrapMode = 0;
		else if (ResultOldWrapMode > 1)
			ResultOldWrapMode = 1;
		if (HoldMaskRectX != MaskRect.x) {
			HoldMaskRectX = MaskRect.x;
			maskImageChangeCheck = true;
		}
		if (HoldMaskRectY != MaskRect.y) {
			HoldMaskRectY = MaskRect.y;
			maskImageChangeCheck = true;
		}
		if (HoldMaskRectW != MaskRect.width) {
			HoldMaskRectW = MaskRect.width;
			maskImageChangeCheck = true;
			maskShapeChangeCheck = true;
			if (UseMaskRotate)
				maskRotateChangeCheck = true;
		}
		if (HoldMaskRectH != MaskRect.height) {
			HoldMaskRectH = MaskRect.height;
			maskImageChangeCheck = true;
			maskShapeChangeCheck = true;
			if (UseMaskRotate)
				maskRotateChangeCheck = true;
		}
		if (HoldUseUnityGUI != UseUnityGUI) {
			HoldUseUnityGUI = UseUnityGUI;
			ResultApplyCheck = true;
		}
		if (HoldResultOpacity != ResultOpacity) {
			HoldResultOpacity = ResultOpacity;
			OpacityChangeCheck = true;
		}
		if (HoldMaskDegree != MaskDegree) {
			HoldMaskDegree = MaskDegree;
			OpacityChangeCheck = true;
			maskImageChangeCheck = true;
		}
		if (HoldReverseMaskArea != ReverseMaskArea) {
			HoldReverseMaskArea = ReverseMaskArea;
			OpacityChangeCheck = true;
			maskImageChangeCheck = true;
		}
		if (HoldGrayscaleMask != GrayscaleMask) {
			HoldGrayscaleMask = GrayscaleMask;
			maskRotateChangeCheck = true;
			OpacityChangeCheck = true;
			maskImageChangeCheck = true;
		}
		if (TrigonomTableMakeCheck != true)
			TrigonomTableMake ();
		
		if (UseUnityGUI) {
			if (MaskDisplayUGUI == null) {
				for (int i = 0; i < transform.childCount; i++) {
					if (transform.GetChild (i).name == "MaskDisplayGO") {
						MaskDisplayGO = transform.GetChild (i).gameObject;
						#if (UNITY_EDITOR)
						if (!EditorApplication.isPlaying && !EditorApplication.isPaused)
							DestroyImmediate (MaskDisplayGO);
						else
							#endif
							Destroy (MaskDisplayGO);
					}
				}
				if (MaskDisplayGO == null) {
					MaskDisplayGO = new GameObject ();
					MaskDisplayGO.name = "MaskDisplayGO";
					MaskDisplayUGUI = new GameObject ();
					MaskDisplayUGUI.name = "MaskDisplayUGUI";
					MaskDisplayUGUI.transform.parent = MaskDisplayGO.transform;
					MaskDisplayGO.transform.parent = transform;
					MaskDisplayGO.transform.SetAsFirstSibling ();
					MaskDisplayGO.hideFlags = HideFlags.HideInHierarchy;
				}
				if (MaskDisplayGO != null) {
					if (MaskDisplayGO.GetComponent<RectTransform> () != true) {
						RectTransform TempRect = MaskDisplayGO.AddComponent<RectTransform> ();
						TempRect.pivot = new Vector2 (0, 0);
					}
					if (DisplayGOTrns == null)
						DisplayGOTrns = MaskDisplayGO.transform;
					if (MaskDisplayGO.GetComponent<MaskDisplayUGUIDestroySelf> () != true)
						MaskDisplayGO.AddComponent<MaskDisplayUGUIDestroySelf> ();
				}
				if (MaskDisplayUGUI != null) {
					if (MaskDisplayUGUI.GetComponent<RectTransform> () != true)
						MaskDisplayRectTrns = MaskDisplayUGUI.AddComponent<RectTransform> ();
					if (MaskDisplayRectTrns == null)
						MaskDisplayRectTrns = MaskDisplayUGUI.GetComponent<RectTransform> ();
					if (MaskDisplayUGUI.GetComponent<CanvasRenderer> () != true)
						MaskDisplayUGUI.AddComponent<CanvasRenderer> ();
					if (MaskDisplayUGUI.GetComponent<RawImage> () != true)
						MaskDisplayComponent = MaskDisplayUGUI.AddComponent<RawImage> ();
					if (MaskDisplayComponent == null)
						MaskDisplayComponent = MaskDisplayUGUI.GetComponent<RawImage> ();
					
					//Maskの確認表示は入れ子なのでrotationとscaleは親に沿って自動変更されるから標準値で生成
					MaskDisplayRectTrns.localRotation = new Quaternion (0, 0, 0, 1);
					MaskDisplayRectTrns.localScale = new Vector3 (1, 1, 1);
					
					//画像とMaskの重ね合わせ&Maskのサイズ変更の基準を左下にするためにMask確認表示のanchorとpivotを0に(anchor範囲は入れ子の親の表示範囲を0~1とするので)
					MaskDisplayRectTrns.anchorMax = new Vector2 (0, 0);
					MaskDisplayRectTrns.anchorMin = new Vector2 (0, 0);
					MaskDisplayRectTrns.pivot = new Vector2 (0.5f, 0.5f);
					
					MaskImageDisplay ();
				}
			}
			if (imageComponent == null && rawimageComponent == null) {
				if (GetComponent<Image> ()) {
					imageComponent = GetComponent<Image> ();
				} else if (GetComponent<RawImage> ()) {
					rawimageComponent = GetComponent<RawImage> ();
				} else {
					Debug.LogError ("[InstantMask] Checked Use UnityGUI, but Image or RawImage component Not Found! GameObjegt name: " + gameObject.name);
					UGUIErrorCheck = true;
				}
			}
			if (thisRectTrns == null) {
				if (GetComponent<RectTransform> ()) {
					thisRectTrns = GetComponent<RectTransform> ();
				} else {
					Debug.LogError ("[InstantMask] Checked Use UnityGUI, but RectTransform component Not Found! GameObjegt name: " + gameObject.name);
					UGUIErrorCheck = true;
				}
			}
			if (CvRend == null) {
				if (GetComponent<CanvasRenderer> ()) {
					CvRend = GetComponent<CanvasRenderer> ();
				} else {
					Debug.LogError ("[InstantMask] Checked Use UnityGUI, but CanvasRenderer component Not Found! GameObjegt name: " + gameObject.name);
					UGUIErrorCheck = true;
				}
			}
			if (UGUIErrorCheck) {
				UGUIErrorCheck = false;
				UseUnityGUI = false;
			}
		} else {
			for (int i = 0; i < transform.childCount; i++) {
				if (transform.GetChild (i).name == "MaskDisplayGO")
					DestroyImmediate (transform.GetChild (i).gameObject);
			}
			if (imageComponent != null)
				imageComponent.sprite = null;
			if (rawimageComponent != null)
				rawimageComponent.texture = null;
			MaskDisplayGO = null;
			MaskDisplayUGUI = null;
			imageComponent = null;
			rawimageComponent = null;
			thisRectTrns = null;
			CvRend = null;
		}
		
		//ResultSizeとRectTransformを同じ値に連動させる場合は以下のアウト部分を追加
		//UnityGUIを使う場合anchorのMax,Minが同じなら単純にwidth,heightを指定できる
		//anchorのMax,Min位置が違う場合は表示範囲からの相対での大きさなのでRectTransformコンポーネントからResultの大きさを決める
		if (UseUnityGUI) {
			/*
			if (thisRectTrns.anchorMax.x == thisRectTrns.anchorMin.x) {
				if (HoldResultSize.x != ResultSize.x) {
					HoldResultSize.x = ResultSize.x;
					thisRectTrns.sizeDelta = new Vector2 (ResultSize.x, thisRectTrns.sizeDelta.y);
				} else {
					if (HoldResultSize.x != thisRectTrns.rect.width) {
						ResultSize.x = thisRectTrns.rect.width;
						HoldResultSize.x = ResultSize.x;
					}
				}
				AncXDifCheck = false;
			} else { //anchorの左右が別位置
				AncXDifCheck = true;
				if (HoldResultSize.x != thisRectTrns.rect.width) {
					HoldResultSize.x = thisRectTrns.rect.width;
					ResultSize.x = HoldResultSize.x;
				} else {
					if (HoldResultSize.x != ResultSize.x) {
						Debug.LogWarning ("[InstantMask] UnityGUI is in relative state at present.\nSo 'X(width)'of ResultSize is needed to set from RectTransform component of UnityGUI.");
						ResultSize.x = HoldResultSize.x;
					}
				}
			}
			if (thisRectTrns.anchorMax.y == thisRectTrns.anchorMin.y) {
				if (HoldResultSize.y != ResultSize.y) {
					HoldResultSize.y = ResultSize.y;
					thisRectTrns.sizeDelta = new Vector2 (thisRectTrns.sizeDelta.x, ResultSize.y);
				} else {
					if (HoldResultSize.y != thisRectTrns.rect.height) {
						ResultSize.y = thisRectTrns.rect.height;
						HoldResultSize.y = ResultSize.y;
					}
				}
				AncYDifCheck = false;
			} else { //anchorの上下が別位置
				AncYDifCheck = true;
				if (HoldResultSize.y != thisRectTrns.rect.height) {
					HoldResultSize.y = thisRectTrns.rect.height;
					ResultSize.y = HoldResultSize.y;
				} else {
					if (HoldResultSize.y != ResultSize.y) {
						Debug.LogWarning ("[InstantMask] UnityGUI is in relative state at present.\nSo 'Y(height)'of ResultSize is needed to set from RectTransform component of UnityGUI.");
						ResultSize.y = HoldResultSize.y;
					}
				}
			}
			*/
			if (MaskDisplayGO != null) {
				MaskImageDisplay ();
			}
		}
		/*
		else {
			AncXDifCheck = false;
			AncYDifCheck = false;
		}
		*/
		
		//画像幅か高さが０以下になる時はエラーが出ないよう画像なしに
		if (ResultSize.x <= 0 || ResultSize.y <= 0) {
			if (UseUnityGUI) {
				if (imageComponent != null)
					imageComponent.sprite = null;
				if (rawimageComponent != null)
					rawimageComponent.texture = null;
			}
			Result = null;
			return;
		}
		
		targetWidth = Mathf.Max (Mathf.RoundToInt (ResultSize.x), 1);//rect.size.x
		targetHeight = Mathf.Max (Mathf.RoundToInt (ResultSize.y), 1);//rect.size.y
		//targetWidth = Mathf.RoundToInt(thisRectTrns.sizeDelta.x);ではsizeDeltaはanchorが分離している際には画像表示部のpixelを表さないのでダメ
		
		if (Result == null) { 
			Result = new Texture2D (targetWidth, targetHeight, TextureFormat.RGBA32, true);
			//Result.alphaIsTransparency = true;
			Result.hideFlags = HideFlags.DontSave;
		}
		if (targetSizeChangeCheck) {
			targetSizeChangeCheck = false;
			Result.Resize (targetWidth, targetHeight);
		}
		#if UNITY_2017_1_OR_NEWER
		if (ResultWrapModeU == 0) {
			Result.wrapModeU = TextureWrapMode.Repeat; 
		} else if (ResultWrapModeU == 1) {
			Result.wrapModeU = TextureWrapMode.Clamp; 
		} else if (ResultWrapModeU == 2) {
			Result.wrapModeU = TextureWrapMode.Mirror; 
		} else if (ResultWrapModeU == 3) {
			Result.wrapModeU = TextureWrapMode.MirrorOnce; 
		} else {
			Result.wrapModeU = TextureWrapMode.Clamp; 
		}
		if (ResultWrapModeV == 0) {
			Result.wrapModeV = TextureWrapMode.Repeat; 
		} else if (ResultWrapModeV == 1) {
			Result.wrapModeV = TextureWrapMode.Clamp; 
		} else if (ResultWrapModeV == 2) {
			Result.wrapModeV = TextureWrapMode.Mirror; 
		} else if (ResultWrapModeV == 3) {
			Result.wrapModeV = TextureWrapMode.MirrorOnce; 
		} else {
			Result.wrapModeV = TextureWrapMode.Clamp; 
		}
		#else
		//wrapだとマスクしてるのに画像端がループしてゴミが見えることがあるのでその場合はclampに
		if (ResultOldWrapMode == 0) {
			Result.wrapMode = TextureWrapMode.Repeat; 
		} else {
			Result.wrapMode = TextureWrapMode.Clamp; 
		}
		#endif
		if (BasePixels == null || BasePixels.Length <= 0) {
			BaseImageChangeCheck = true;
		}
		
		if (BaseImageChangeCheck || MaskImageNullRendCheck) {
			BaseImageChangeCheck = true;
			MaskImageNullRendCheck = false;
			OpacityChangeCheck = true;
			ResultApplyCheck = true;
			if (Masking && MaskImage != null)
				maskImageChangeCheck = true;
			BasePixels = new Color[targetWidth * targetHeight];
			int y = 0;
			while (y < Result.height) {
				int x = 0;
				while (x < Result.width) {
					float xFrac = x * 1.0f / (Result.width);
					float yFrac = y * 1.0f / (Result.height);
					if (Pixelization != 0) {
						xFrac = Mathf.Floor (xFrac * (PixelizationCalValue * 2 * (targetWidth / 100))) / (PixelizationCalValue * 2 * (targetWidth / 100));
						yFrac = Mathf.Floor (yFrac * (PixelizationCalValue * 2 * (targetHeight / 100))) / (PixelizationCalValue * 2 * (targetHeight / 100));
					}
					BasePixels [y * Result.width + x] = BaseImage.GetPixelBilinear (xFrac, yFrac);
					x++;
				}
				y++;
			}
		}
		
		
		if (Masking && MaskImage != null) {
			maskWidth = Mathf.RoundToInt (MaskRect.width);
			maskHeight = Mathf.RoundToInt (MaskRect.height);
			
			//--マスク回転処理準備
			if (UseMaskRotate) {
				if (MnMaskAngle != MaskAngle) {
					maskImageChangeCheck = true;
					maskRotateChangeCheck = true;
					if (MaskRotateOffSwitchCheck) {
						MaskRotateOffSwitchCheck = false;
						maskShapeChangeCheck = true;
					}
					MnMaskAngle = MaskAngle;
					if (UseUnityGUI)
						MaskImageDisplay ();
				}
			} else {
				if (MnMaskAngle != 0) {
					maskImageChangeCheck = true;
					maskShapeChangeCheck = true;
					maskRotateChangeCheck = true;
					MaskRotateOffSwitchCheck = true;
					MnMaskAngle = 0;
					if (UseUnityGUI)
						MaskImageDisplay ();
				}
			}
			//--マスク回転処理準備ここまで
			
			if (maskRotateChangeCheck)
				PytInt = Mathf.FloorToInt (Mathf.Sqrt (Mathf.Pow (maskWidth, 2) + Mathf.Pow (maskHeight, 2)));
			//MaskRectの数値によってマスク画像がソース画像に全く重なっていない場合
			if (maskWidth <= 0 || maskHeight <= 0
				|| MaskRect.x > 0 && MaskRect.x >= targetWidth + (maskWidth / 2) + 1 && UseMaskRotate != true
				|| MaskRect.y > 0 && MaskRect.y >= targetHeight + (maskHeight / 2) + 1 && UseMaskRotate != true
				|| MaskRect.x < 0 && Mathf.Abs (MaskRect.x) > (maskWidth / 2) + 1 && UseMaskRotate != true
				|| MaskRect.y < 0 && Mathf.Abs (MaskRect.y) > (maskHeight / 2) + 1 && UseMaskRotate != true
				|| MaskRect.x > 0 && MaskRect.x >= targetWidth + (maskWidth / 2) + ((PytInt - maskWidth) / 2) + 1 && UseMaskRotate
				|| MaskRect.y > 0 && MaskRect.y >= targetHeight + (maskHeight / 2) + ((PytInt - maskHeight) / 2) + 1 && UseMaskRotate
				|| MaskRect.x < 0 && Mathf.Abs (MaskRect.x) > (maskWidth / 2) + ((PytInt - maskWidth) / 2) + 1 && UseMaskRotate
				|| MaskRect.y < 0 && Mathf.Abs (MaskRect.y) > (maskHeight / 2) + ((PytInt - maskHeight) / 2) + 1 && UseMaskRotate 
			    ) {
				maskImageChangeCheck = false;
				if (MaskOutRangeCheck != true) {
					MaskOutRangeCheck = true;
					Debug.LogWarning ("[InstantMask] MaskImage position is out of BaseImage range.");
				}
			} else {
				MaskOutRangeCheck = false;
			}
			
			if (maskImageChangeCheck) {
				maskImageChangeCheck = false;
				OpacityChangeCheck = true;
				ResultApplyCheck = true;
				if (GrayscaleMask)
					NoDisplayColor = Color.black;
				else
					NoDisplayColor = new Color (0, 0, 0, 0);
				if (mpixels == null || mpixels.Length <= 0) {
					maskShapeChangeCheck = true;
				}
				if (mpixelsRotList == null || mpixelsRotList.Count <= 0 || MaskResultAry == null || MaskResultAry.Length <= 0) {
					maskShapeChangeCheck = true;
					maskRotateChangeCheck = true;
				}
				// --マスク拡縮処理
				if (maskShapeChangeCheck) {
					mpixels = new Color[maskWidth * maskHeight];
					int my = 0;
					while (my < maskHeight) {
						int mx = 0;
						while (mx < maskWidth) {
							float mxFrac = mx * 1.0f / (maskWidth);
							float myFrac = my * 1.0f / (maskHeight);
							mpixels [my * maskWidth + mx] = MaskImage.GetPixelBilinear (mxFrac, myFrac);
							mx++;
						}
						my++;
					}
				}
				// --マスク拡縮処理ここまで
				
				//--マスク回転処理
				if (maskRotateChangeCheck) {
					maskRotateChangeCheck = false;
					Color[] RotatedmpixAry = RotateImageProc (mpixels, -MnMaskAngle);
					mpixelsRotList = new List<Color> (RotatedmpixAry);
					// -- maskRotWidth,maskRotHeightはRotateImageProc先で回転処理に必要な余白を足したものに書き換えられる
				} else if (maskShapeChangeCheck) {//回転処理を通さずwidth,Heightを変更するのはUseMaskRotateがfalseの場合
					mpixelsRotList = new List<Color> (mpixels);
					maskRotWidth = maskWidth;
					maskRotHeight = maskHeight;
				}
				maskShapeChangeCheck = false;
				//--マスク回転処理ここまで
				
				//--マスク移動処理
				MaskResultAry = new Color[targetWidth * targetHeight];
				for (int i=0; i<MaskResultAry.Length; i++) {
					MaskResultAry [i] = NoDisplayColor;
				}
				int aaa;
				int bbb;
				int ccc;
				int ddd;
				int diffX = Mathf.RoundToInt (MaskRect.x);
				int diffY = Mathf.RoundToInt (MaskRect.y);
				//MaskRectのposition基準点をマスク画像中央とする
				diffX = diffX - (maskRotWidth / 2);
				diffY = diffY - (maskRotHeight / 2);
				/* MaskRectのposition基準点を画像左下とする場合上記は要らないが
				 * 左&下側の空白を可変させず右&上の空白を増やすように処理を変える必要がある
				 * 上記を消すだけだとUseMaskRotateのOnOffでmaskRotWidth,maskRotHeightの
				 * 違いにより位置がずれる事に注意
				*/
				if (diffX < 0) {
					aaa = Mathf.Abs (diffX);
					bbb = 0;
				} else {
					aaa = 0;
					bbb = diffX;
				}
				if (diffY < 0) {
					ccc = Mathf.Abs (diffY);
					ddd = 0;
				} else {
					ccc = 0;
					ddd = diffY;
				}
				int CalH = Mathf.Min (targetHeight - ddd, maskRotHeight - ccc);
				int CalW = Mathf.Min (targetWidth - bbb, maskRotWidth - aaa);
				for (int i=0; i<CalH; i++) {
					if (MaskMoveLoopExit != true) {
						for (int j=0; j<CalW; j++) {
							if ((i * targetWidth) + j + bbb > MaskResultAry.Length
								|| (ddd * targetWidth) + (i * targetWidth) + j + bbb > MaskResultAry.Length
								|| (ccc * maskRotWidth) + (i * maskRotWidth) + j + aaa > mpixelsRotList.Count) {
								MaskMoveLoopExit = true;
								//for文をbreakで抜けたりしてループ回数をfor文内で変えるのは処理がおかしくなる場合があるのでやらない
							} else {
								MaskResultAry [(ddd * targetWidth) + (i * targetWidth) + j + bbb] = mpixelsRotList [(ccc * maskRotWidth) + (i * maskRotWidth) + j + aaa];
							}
						}
					}
				}
				MaskMoveLoopExit = false;
				//--マスク移動処理ここまで

				//ソース画像とマスクの修正画像のpixel数が違わないようチェック
				if (BasePixels.Length != MaskResultAry.Length) {
					Debug.LogError ("[InstantMask] Mask prosess pixel number Error!! GameObjegt name: " + gameObject.name);
					Debug.Log ("[InstantMask] MaskPixel:" + MaskResultAry.Length + " / " + "sourcePixel:" + BasePixels.Length);
				}

				if (GrayscaleMask != true) {
					if (ReverseMaskArea != true) {
						for (int i=0; i<MaskResultAry.Length; i++) {
							MaskResultAry [i].a = MaskResultAry [i].a + ((100 - MaskDegree) * (1 - MaskResultAry [i].a) / 100);
						}
					} else {
						for (int i=0; i<MaskResultAry.Length; i++) {
							MaskResultAry [i].a = MaskResultAry [i].a - ((100 - MaskDegree) * (MaskResultAry [i].a) / 100);
						}
					}
				}

				mpixelsList = new List<Color> (MaskResultAry);

			}// maskImageChangeCheck

		}// Masking && MaskImage != null
		else {
			mpixelsList = new List<Color> (BasePixels);
		}


		if (BasePixelprocs == null || BasePixelprocs.Length != BasePixels.Length)
			BasePixelprocs = new Color[BasePixels.Length];
		if (BaseImageChangeCheck || OpacityChangeCheck) {
			BaseImageChangeCheck = false;
			BasePixels.CopyTo (BasePixelprocs, 0);
		}

		if (MaskOutRangeCheck) {
			if (OpacityChangeCheck) {
				OpacityChangeCheck = false;
				ResultApplyCheck = true;
				if (ReverseMaskArea != true && Masking) {
					for (int i=0; i<BasePixelprocs.Length; i++) {
						BasePixelprocs [i].a = (100 - MaskDegree) * (BasePixelprocs [i].a / 100);
					}
				}
				for (int i=0; i<BasePixelprocs.Length; i++) {
					BasePixelprocs [i].a = BasePixelprocs [i].a * (ResultOpacity * 0.01f);
				}
			}
		} else {

			if (OpacityChangeCheck) {
				OpacityChangeCheck = false;
				ResultApplyCheck = true;
				if (GrayscaleMask != true || MaskOutRangeCheck || Masking != true) {
					for (int i=0; i<BasePixelprocs.Length; i++) {
						if (ReverseMaskArea != true || Masking != true)
							BasePixelprocs [i].a = mpixelsList [i].a * (ResultOpacity * 0.01f);
						else
							BasePixelprocs [i].a = (1 - (mpixelsList [i].a)) * (ResultOpacity * 0.01f);
					}
				} else {
					for (int i=0; i<mpixelsList.Count; i++) {
						if (GrayAlphaCheck != true) {
							if (mpixelsList [i].a != 0.0f && mpixelsList [i].a != 1.0f) {
								Debug.LogWarning ("[InstantMask] Current mode is GrayScale, but Mask Image has Alpha pixel. GameObjegt name: " + gameObject.name);
								GrayAlphaCheck = true;
							}
						}
						float mpixelA;
						//HSV色空間のV(明度)はRGBのうち最も高い値を0~1の範囲にした数値と同値,UnityのRGBは元々0~1なのでそのまま //anyColor.grayscaleでカラーをグレイスケールにできる
						if (ReverseMaskArea != true)
							mpixelA = Mathf.Max (Mathf.Max (mpixelsList [i].r, mpixelsList [i].g), mpixelsList [i].b);
						else
							mpixelA = (1 - (Mathf.Max (Mathf.Max (mpixelsList [i].r, mpixelsList [i].g), mpixelsList [i].b))) * (ResultOpacity * 0.01f);
						BasePixelprocs [i].a = mpixelA + ((100 - MaskDegree) * (1 - mpixelA) / 100);
						BasePixelprocs [i].a = BasePixelprocs [i].a * (ResultOpacity * 0.01f);
					}
					GrayAlphaCheck = false;
				}
			}

		}
		
		if (ResultApplyCheck) {
			ResultApplyCheck = false;
			ResultApplyProc ();
		}
		
	}
	
	void ResultApplyProc ()
	{
		Result.SetPixels (BasePixelprocs, 0);
		Result.Apply ();
		if (UseUnityGUI) {
			if (imageComponent != null) {
				imageComponent.sprite = Sprite.Create (Result, new Rect (0, 0, Result.width, Result.height), new Vector2 (0.5f, 0.5f));
				imageComponent.sprite.hideFlags = HideFlags.DontSave;
			}
			if (rawimageComponent != null) {
				rawimageComponent.texture = Result;
				rawimageComponent.texture.hideFlags = HideFlags.DontSave;
			}
		}
		Result.hideFlags = HideFlags.DontSave;
	}
	
	Color[] RotateImageProc (Color[] mpixels, float MaskAngle)
	{
		List<Color> mpixelsListCopy = new List<Color> (mpixels);
		int PythagoreanInt = Mathf.FloorToInt (Mathf.Sqrt (Mathf.Pow (maskWidth, 2) + Mathf.Pow (maskHeight, 2)));
		if (PythagoreanInt % 2 != 0) {
			PythagoreanInt += 1; //偶数にして上,下/左,右に追加する長さを割り切れる同じ長さにして小数点丸め込みでwidth,heightの変更時ガタガタ動かないように
		}
		PytAdjustWidth = PythagoreanInt - maskWidth;
		PytAdjustHeight = PythagoreanInt - maskHeight;
		
		Color[] leftMargin = new Color[PytAdjustWidth / 2];//左の余白,偶数にしてなければ小数点以下は切り捨てられる
		for (int i=0; i<leftMargin.Length; i++) {
			leftMargin [i] = NoDisplayColor;
		}
		Color[] rightMargin = new Color[PytAdjustWidth - leftMargin.Length];//右の余白
		for (int i=0; i<rightMargin.Length; i++) {
			rightMargin [i] = NoDisplayColor;
		}
		Color[] bottomMargin = new Color[(PytAdjustHeight / 2) * PythagoreanInt];//下の余白ブロック
		for (int i=0; i<bottomMargin.Length; i++) {
			bottomMargin [i] = NoDisplayColor;
		}
		Color[] topMargin = new Color[(PytAdjustHeight - (PytAdjustHeight / 2)) * PythagoreanInt];//上の余白ブロック
		for (int i=0; i<topMargin.Length; i++) {
			topMargin [i] = NoDisplayColor;
		}
		
		int leftAddPoint = (maskWidth + leftMargin.Length + rightMargin.Length);
		int rightAddPointPart = (maskWidth + leftMargin.Length);
		for (int i=0; i<maskHeight; i++) { //マスク左右の処理
			mpixelsListCopy.InsertRange (i * leftAddPoint, leftMargin);//左
			mpixelsListCopy.InsertRange (((i + 1) * rightAddPointPart) + (i * rightMargin.Length), rightMargin);//右
		}
		//マスク下側の処理
		mpixelsListCopy.InsertRange (0, bottomMargin);
		//マスク上側の処理
		mpixelsListCopy.AddRange (topMargin);
		
		if (rotMountTex == null) {
			rotMountTex = new Texture2D (PythagoreanInt, PythagoreanInt);
			rotMountTex.wrapMode = TextureWrapMode.Clamp; //マスクしてるのに画像端がループしてゴミが見えることがあるのでwrapしないように
			//rotMountTex.alphaIsTransparency = true;
			rotMountTex.hideFlags = HideFlags.DontSave;
		} else {
			rotMountTex.Resize (PythagoreanInt, PythagoreanInt);
		}
		rotMountTex.SetPixels (mpixelsListCopy.ToArray ());
		rotMountTex.Apply ();
		
		int rTexWidth = rotMountTex.width;
		int rTexHeight = rotMountTex.height;
		float x1, y1, x2, y2;
		float dx_x = rot_x (MaskAngle, 1.0f, 0.0f);
		float dx_y = rot_y (MaskAngle, 1.0f, 0.0f);
		float dy_x = rot_x (MaskAngle, 0.0f, 1.0f);
		float dy_y = rot_y (MaskAngle, 0.0f, 1.0f);
		x1 = rot_x (MaskAngle, -rTexWidth / 2.0f, -rTexHeight / 2.0f) + rTexWidth / 2.0f;
		y1 = rot_y (MaskAngle, -rTexWidth / 2.0f, -rTexHeight / 2.0f) + rTexHeight / 2.0f;
		Color[] pixColors = new Color[rTexWidth * rTexHeight];
		float xFrac;
		float yFrac;
		for (int y = 0; y < rTexHeight; y++) {
			x2 = x1;
			y2 = y1;
			for (int x = 0; x < rTexWidth; x++) {
				x2 += dx_x;
				y2 += dx_y;
				if (Mathf.FloorToInt (x2) > rTexWidth
					|| Mathf.FloorToInt (x2) < 0
					|| Mathf.FloorToInt (y2) > rTexHeight
					|| Mathf.FloorToInt (y2) < 0) {
					//回転により空いた部分に画像ループでいらない部分が出てくるのを抑制
					pixColors [y * rTexWidth + x] = NoDisplayColor;
				} else {
					xFrac = x2 / (rTexWidth);
					yFrac = y2 / (rTexHeight);
					pixColors [y * rTexWidth + x] = rotMountTex.GetPixelBilinear (xFrac, yFrac);
					//xxx.SetPixel ((int)Mathf.Floor (x), (int)Mathf.Floor (y), GetPix (rotMountTex, x2, y2));
				}
			}
			x1 += dy_x;
			y1 += dy_y;
		}
		
		maskRotWidth = PythagoreanInt;
		maskRotHeight = PythagoreanInt;
		return pixColors;
	}
	
	private float rot_x (float angle, float x, float y)
	{
		/*
		float cos = Mathf.Cos (angle / 180.0f * Mathf.PI);
		float sin = Mathf.Sin (angle / 180.0f * Mathf.PI);
		return (x * cos + y * (-sin));
		*/
		int angleIntX = Mathf.RoundToInt (angle % 360);
		if (angleIntX < 0)
			angleIntX = 360 - Mathf.Abs (angleIntX);
		float rotValueX = (x * CosTable [angleIntX] + y * (-SinTable [angleIntX]));
		return rotValueX;
	}
	private float rot_y (float angle, float x, float y)
	{
		/*
		float cos = Mathf.Cos (angle / 180.0f * Mathf.PI);
		float sin = Mathf.Sin (angle / 180.0f * Mathf.PI);
		return (x * sin + y * cos);
		*/
		int angleIntY = Mathf.RoundToInt (angle % 360);
		if (angleIntY < 0)
			angleIntY = 360 - Mathf.Abs (angleIntY);
		float rotValueY = (x * SinTable [angleIntY] + y * CosTable [angleIntY]);
		return rotValueY;
	}
	
	void MaskImageDisplay ()
	{
		if (MaskVisibility && MaskImage) {
			if (MaskDisplayUGUI != null)
				MaskDisplayUGUI.GetComponent<RawImage> ().enabled = true;
			if (MaskDisplayComponent != null) {
				MaskDisplayComponent.texture = MaskImage;
				MaskDisplayComponent.color = VisibleColor;
			}
			if (MaskDisplayRectTrns != null) {
				MaskDisplayRectTrns.sizeDelta = new Vector2 (MaskRect.width, MaskRect.height);
				MaskDisplayRectTrns.localRotation = Quaternion.AngleAxis (-MnMaskAngle, -Vector3.forward);
				if (thisRectTrns != null)
					MaskDisplayRectTrns.anchoredPosition = new Vector2 (MaskRect.x + (thisRectTrns.rect.width / ResultSize.x) - (ResultSize.x / thisRectTrns.rect.width) - (thisRectTrns.rect.width / ResultSize.x), MaskRect.y + (thisRectTrns.rect.height / ResultSize.y) - (ResultSize.y / thisRectTrns.rect.height) - (thisRectTrns.rect.height / ResultSize.y));
				//MaskDisplayRectTrns.anchoredPosition = new Vector2 (MaskRect.x + (thisRectTrns.anchoredPosition.x * (ResultSize.x / thisRectTrns.rect.width)) - (thisRectTrns.rect.width / ResultSize.x), MaskRect.y + (thisRectTrns.anchoredPosition.y * (ResultSize.y / thisRectTrns.rect.height)));
				//入れ子のMask確認表示はPosZを0にしていないとこのオブジェクトのScaleZによって倍率がかかって手前奥に離れてしまう場合があるのでz=0
				//if (MaskDisplayRectTrns)
				//MaskDisplayRectTrns.localPosition = new Vector3 (thisRectTrns.localPosition.x, thisRectTrns.localPosition.y, 0);
			}
			if (DisplayGOTrns != null) {
				if (thisRectTrns != null) {
					if (thisRectTrns.rect.width <= 0 || ResultSize.x <= 0 || thisRectTrns.rect.height <= 0 || ResultSize.y <= 0)
						DisplayGOTrns.localScale = new Vector3 (0, 0, 0);
					else
						DisplayGOTrns.localScale = new Vector3 (thisRectTrns.rect.width / ResultSize.x, thisRectTrns.rect.height / ResultSize.y, 1);
					DisplayGOTrns.localPosition = new Vector3 (-(thisRectTrns.rect.width / 2), -(thisRectTrns.rect.height / 2), 0);
				}
				DisplayGOTrns.localRotation = Quaternion.Euler (0, 0, 0);
			}
			//MaskDisplayRectTrns.sizeDelta = new Vector2 ((MaskRect.width / ResultSize.x) * thisRectTrns.rect.width, (MaskRect.height / ResultSize.y) * thisRectTrns.rect.height);
			//MaskDisplayRectTrns.anchoredPosition = new Vector2 ((MaskRect.x / ResultSize.x) + MaskRect.x * (thisRectTrns.rect.width / ResultSize.x) - (ResultSize.x / thisRectTrns.rect.width), (MaskRect.y / ResultSize.y) + MaskRect.y * (thisRectTrns.rect.height / ResultSize.y) - (ResultSize.y / thisRectTrns.rect.height));
		} else {
			if (MaskDisplayUGUI != null)
				MaskDisplayUGUI.GetComponent<RawImage> ().enabled = false;
			if (MaskDisplayComponent != null)
				MaskDisplayComponent.texture = null;
		}
	}
	
	void  TrigonomTableMake ()
	{
		if (CosTable == null || CosTable.Length != 360) {
			CosTable = new float[360];
			for (int i=0; i<360; i++) {
				CosTable [i] = Mathf.Cos (i / 180.0f * Mathf.PI);
			}
		}
		if (SinTable == null || SinTable.Length != 360) {
			SinTable = new float[360];
			for (int i=0; i<360; i++) {
				SinTable [i] = Mathf.Sin (i / 180.0f * Mathf.PI);
			}
		}
		TrigonomTableMakeCheck = true;
	}
	
	#if (UNITY_EDITOR)
	public void UndoRedoSet ()
	{
		BaseImageChangeCheck = true;
		maskImageChangeCheck = true;
		maskShapeChangeCheck = true;
		maskRotateChangeCheck = true;
		Update ();
	}
	
	public void BaseImageSetNativeSize ()
	{
		if (UseUnityGUI) {
			//Undo.RecordObject (thisRectTrns, "Inspector");
			if (imageComponent == null && rawimageComponent == null) {
				if (GetComponent<Image> ()) {
					imageComponent = GetComponent<Image> ();
				} else if (GetComponent<RawImage> ()) {
					rawimageComponent = GetComponent<RawImage> ();
				}
			}
			if (imageComponent != null) {
				imageComponent.sprite = Sprite.Create (BaseImage, new Rect (0, 0, BaseImage.width, BaseImage.height), new Vector2 (0.5f, 0.5f));
				imageComponent.sprite.hideFlags = HideFlags.DontSave;
				//RectTransformのサイズも元画像のサイズにするなら以下
				//imageComponent.SetNativeSize ();
			} else if (rawimageComponent != null) {
				rawimageComponent.texture = BaseImage;
				//RectTransformのサイズも元画像のサイズにするなら以下
				//rawimageComponent.SetNativeSize ();
			}
		}
		//Undo.RecordObject (this, "Inspector");
		ResultSize = new Vector2 (BaseImage.width, BaseImage.height);
		Update ();
	}
	
	public void MaskImageSetNativeSize ()
	{
		//Undo.RecordObject (this, "Inspector");
		MaskRect.width = MaskImage.width;
		MaskRect.height = MaskImage.height;
		Update ();
	}
	
	public void ExportResultImage ()
	{
		if (Result != null) {
			IMScript = this;
			byte[] ResultBytes = Result.EncodeToPNG ();
			if (Directory.Exists (Application.dataPath + "/InstantMask/") != true) {
				Directory.CreateDirectory (Application.dataPath + "/InstantMask/");
			}
			if (Directory.Exists (Application.dataPath + "/InstantMask/ExportImages/") != true) {
				Directory.CreateDirectory (Application.dataPath + "/InstantMask/ExportImages/");
			}
			DateTime thisDay = DateTime.Now;
			string FileDayString = thisDay.ToString ("yyyMMdd") + thisDay.ToString ("HHmmss");
			string FileNumString = string.Format ("{0:D04}", Time.frameCount);
			File.WriteAllBytes (Application.dataPath + "/InstantMask/ExportImages/" + FileDayString + FileNumString + ".png", ResultBytes);
			AssetDatabase.Refresh ();
			IMScript = null;
		} else {
			Debug.LogWarning ("[InstantMask] Result Image is Null!! GameObjegt name: " + gameObject.name);
		}
	}
	#endif
	
	[HideInInspector, SerializeField] Texture2D HoldBaseImage;
	public Texture2D BaseImage {
		get { return HoldBaseImage; }
		set {
			if (value != HoldBaseImage) {
				HoldBaseImage = value;
				BaseImageChangeCheck = true;
			}
		}
	}

	[HideInInspector, SerializeField] Texture2D HoldMaskImage;
	public Texture2D MaskImage {
		get { return HoldMaskImage; }
		set {
			if (value != HoldMaskImage) {
				if(HoldMaskImage != true)
					Masking = true;
				HoldMaskImage = value;
				maskImageChangeCheck = true;
				maskShapeChangeCheck = true;
				maskRotateChangeCheck = true;
			}
		}
	}

	int HoldTargetWidth;
	int targetWidth {
		get { return HoldTargetWidth; }
		set {
			if (value != HoldTargetWidth) {
				HoldTargetWidth = value;
				BaseImageChangeCheck = true;
				targetSizeChangeCheck = true;
			}
		}
	}

	int HoldTargetHeight;
	int targetHeight {
		get { return HoldTargetHeight; }
		set {
			if (value != HoldTargetHeight) {
				HoldTargetHeight = value;
				BaseImageChangeCheck = true;
				targetSizeChangeCheck = true;
			}
		}
	}
	
	[HideInInspector, SerializeField] bool HoldMaskingCheck;
	public bool Masking {
		get { return HoldMaskingCheck; }
		set {
			if (value != HoldMaskingCheck) {
				HoldMaskingCheck = value;
				BaseImageChangeCheck = true;
			}
		}
	}

	[HideInInspector, SerializeField] float HoldMaskAngle;
	public float MaskAngle {
		get { return HoldMaskAngle; }
		set {
			if (value != HoldMaskAngle) {
				HoldMaskAngle = value;
				BaseImageChangeCheck = true;
			}
		}
	}

	[HideInInspector, SerializeField] int HoldPixelization;
	public int Pixelization {
		get { return HoldPixelization; }
		set {
			if (value != HoldPixelization) {
				HoldPixelization = value;
				BaseImageChangeCheck = true;
			}
		}
	}
	
}
