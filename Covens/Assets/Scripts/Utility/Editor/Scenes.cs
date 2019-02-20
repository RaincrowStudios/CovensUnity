using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using TMPro;


public class Scenes : MonoBehaviour
{

	[MenuItem("Scenes/Main Scene")]
	static void MainScene()
	{
		EditorApplication.SaveCurrentSceneIfUserWantsTo();
		EditorApplication.OpenScene("Assets/Scenes/MainScene.unity");
	}

	[MenuItem("Scenes/Main Scene Reduced")]
	static void MainSceneReduced()
	{
		EditorApplication.SaveCurrentSceneIfUserWantsTo();
		EditorApplication.OpenScene("Assets/Scenes/MainScene-Reduced.unity");
	}


	[MenuItem("Scenes/Start Scene")]
	static void StartScene()
	{
		EditorApplication.SaveCurrentSceneIfUserWantsTo();
		EditorApplication.OpenScene("Assets/Scenes/StartScene.unity");
	}

	[MenuItem("Tools/Play")]
	static void PlayTest()
	{
		EditorApplication.SaveCurrentSceneIfUserWantsTo();
		EditorApplication.OpenScene("Assets/Scenes/StartScene.unity");
		EditorApplication.isPlaying = true;
	}

	[MenuItem("Tools/Add Localize %#e")]
	static void AddLocalizeLookUp()
	{
		GameObject obj = Selection.activeGameObject;
		obj.AddComponent<LocalizeLookUp>();
	}

	[MenuItem("Tools/Make Text Mesh Pro %#t")]
	static void MakeTextMeshPro()
	{
		GameObject obj = Selection.activeGameObject;

		if (obj.GetComponent<FontManager>() != null){
			DestroyImmediate(obj.GetComponent<FontManager>());
		}

		Text objText = obj.GetComponent<Text>();
		string textStr; //
		string fontName;
		int fontSize;   //
		float lineSpace;//
		Color textColor;//
		bool isRichText;//
		bool autoSize;  //
		//enums
		int horizWrap;      //
		int vertOverflow;   //
		int textAlignment;  //

		//checks if text object exists and sets valuesf
		if (objText != null)
		{
			textStr = objText.text;
			fontName = objText.font.ToString();
			fontSize = objText.fontSize;
			lineSpace = objText.lineSpacing;
			textColor = objText.color;
			isRichText = objText.supportRichText;
			autoSize = objText.resizeTextForBestFit;

			switch(objText.horizontalOverflow)
			{
			case HorizontalWrapMode.Wrap:
				horizWrap = 1;
				break;
			case HorizontalWrapMode.Overflow:
				horizWrap = 2;
				break;
			default:
				horizWrap = 1;
				break;
			}

			switch(objText.verticalOverflow)
			{
			case VerticalWrapMode.Overflow:
				vertOverflow = 1;
				break;
			case VerticalWrapMode.Truncate:
				vertOverflow = 2;
				break;
			default:
				vertOverflow = 1;
				break;
			}

			switch(objText.alignment)
			{
			case TextAnchor.UpperLeft:
				textAlignment = 1;
				break;
			case TextAnchor.UpperCenter:
				textAlignment = 2;
				break;
			case TextAnchor.UpperRight:
				textAlignment = 3;
				break;
			case TextAnchor.MiddleLeft:
				textAlignment = 4;
				break;
			case TextAnchor.MiddleCenter:
				textAlignment = 5;
				break;
			case TextAnchor.MiddleRight:
				textAlignment = 6;
				break;
			case TextAnchor.LowerLeft:
				textAlignment = 7;
				break;
			case TextAnchor.LowerCenter:
				textAlignment = 8;
				break;
			case TextAnchor.LowerRight:
				textAlignment = 9;
				break;
			default:
				textAlignment = 1;
				break;
			}

			DestroyImmediate(objText);
		}
		else
		{
			//If not then we default to some values
			textStr = "";
			fontName = "";
			fontSize = 30;
			lineSpace = 1.0f;
			textColor = Color.white;
			isRichText = true;
			horizWrap = 1;
			vertOverflow = 1;
			textAlignment = 1;
			autoSize = false;
		}
		Debug.Log(textStr);
		TextMeshProUGUI newText = obj.AddComponent<TextMeshProUGUI>();
		if (newText != null)
		{
			newText.text = textStr;
			newText.fontSize = fontSize;
			newText.lineSpacing = lineSpace;
			newText.color = textColor;
			newText.richText = isRichText;
			newText.enableAutoSizing = autoSize;

			switch(horizWrap)
			{
			case 1:
				newText.enableWordWrapping = true;
				break;
			default:
				newText.enableWordWrapping = false;
				break;
			}

			switch(vertOverflow)
			{
			case 1:
				newText.overflowMode = TextOverflowModes.Overflow;
				break;
			case 2:
				newText.overflowMode = TextOverflowModes.Truncate;
				break;
			default:
				newText.overflowMode = TextOverflowModes.Overflow;
				break;
			}

			switch(textAlignment)
			{
			case 1:
				newText.alignment = TextAlignmentOptions.TopLeft;
				break;
			case 2:
				newText.alignment = TextAlignmentOptions.Top;
				break;
			case 3:
				newText.alignment = TextAlignmentOptions.TopRight;
				break;
			case 4:
				newText.alignment = TextAlignmentOptions.Center;
				newText.alignment = TextAlignmentOptions.Left;
				break;
			case 5:
				newText.alignment = TextAlignmentOptions.Center;
				break;
			case 6:
				newText.alignment = TextAlignmentOptions.Center;
				newText.alignment = TextAlignmentOptions.Right;
				break;
			case 7:
				newText.alignment = TextAlignmentOptions.BottomLeft;
				break;
			case 8:
				newText.alignment = TextAlignmentOptions.Bottom;
				break;
			case 9:
				newText.alignment = TextAlignmentOptions.BottomRight;
				break;
			default:
				newText.alignment = TextAlignmentOptions.TopLeft;
				break;
			}
		}
	}
}