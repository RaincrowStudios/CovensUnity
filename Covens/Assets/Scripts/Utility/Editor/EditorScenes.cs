using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public class EditorScenes : MonoBehaviour
{
    [MenuItem("Scenes/Start Scene",false,0)]
    static void StartScene() => OpenScene("StartScene");

    [MenuItem("Scenes/Main Scene", false, 1)]
    static void MainScene() => OpenScene("MainScene");

    [MenuItem("Scenes/Place of Power", false, 2)]
    static void PopScene() => OpenScene("PlaceOfPower");

    [MenuItem("Scenes/FTF", false, 3)]
    static void FTFSCene() => OpenScene("FTF");

    [MenuItem("Scenes/Screens/Login")]
    static void LoginScene() => OpenScene("LoginScene");

    [MenuItem("Scenes/Screens/Coven Managment")]
    static void CovenScene() => OpenScene("CovenManagement");

    [MenuItem("Scenes/Screens/Daily Quests")]
    static void DailiesScene() => OpenScene("DailyQuests");

    [MenuItem("Scenes/Screens/Spellcast Book")]
    static void SpellcastingScene() => OpenScene("SpellcastBook");

    [MenuItem("Scenes/Screens/Chat")]
    static void ChatScene() => OpenScene("Chat");

    [MenuItem("Scenes/Screens/Store")]
    static void StoreScene() => OpenScene("Store");

    [MenuItem("Scenes/Screens/Daily Blessing")]
    static void DailyBlessingScene() => OpenScene("DailyBlessing");

    [MenuItem("Scenes/Screens/Grey Hand Office")]
    static void GreyHandOfficeScene() => OpenScene("GreyHandOffice");

    [MenuItem("Scenes/Screens/QuickCast")]
    static void QuickCastScene() => OpenScene("QuickCast");

    [MenuItem("Scenes/Screens/PlayerSelect")]
    static void PlayerSelectScene() => OpenScene("PlayerSelect");

    [MenuItem("Scenes/Screens/SpiritSelect")]
    static void SpiritSelectScene() => OpenScene("SpiritSelect");

    [MenuItem("Scenes/Screens/Nearby PoPs")]
    static void NearbyPopsScene() => OpenScene("NearbyPops");

    [MenuItem("Scenes/Screens/Settings")]
    static void SettingsScene() => OpenScene("Settings");

    [MenuItem("Scenes/Screens/WitchSchool")]
    static void WitchSchoolScene() => OpenScene("WitchSchool");

    [MenuItem("Scenes/Screens/VideoPlayer")]
    static void VideoPlayerScene() => OpenScene("VideoPlayer");

    [MenuItem("Scenes/Screens/Popup")]
    static void PopupScene() => OpenScene("Popup");

    [MenuItem("Scenes/Screens/Summoning")]
    static void SummoningScene() => OpenScene("Summoning");


    private static void OpenScene(string name)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene($"Assets/Scenes/{name}.unity");
    }

    [MenuItem("Tools/Play")]
	static void PlayTest()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/Scenes/StartScene.unity");
            EditorApplication.isPlaying = true;
        }
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