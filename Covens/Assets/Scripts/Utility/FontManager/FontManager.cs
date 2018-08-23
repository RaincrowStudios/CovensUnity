using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[ExecuteInEditMode]
public class FontManager : MonoBehaviour {

	bool isWhite;

	 void SetupFont(bool isBGWhite)
	{
		if (gameObject.GetComponent<Text> () == null) {
			gameObject.AddComponent<Text> ();
		}
		var fonts = GameObject.FindGameObjectWithTag ("fontManager").GetComponent<FontVariables> ();
		Text t = GetComponent<Text> ();
		if (Type == FontVariables.TextType.Button) {
			t.font = fonts.LatoItalic;
			t.fontSize = fonts.buttonSize;
		} else if (Type == FontVariables.TextType.Title) {
			t.font = fonts.CinzelRegular;
			t.fontSize = fonts.titleSize;
		} else if (Type == FontVariables.TextType.Subtitle) {
			t.font = fonts.LatoItalic;
			t.fontSize = fonts.subtitleSize;
		} else if (Type == FontVariables.TextType.Numbers) {
			t.font = fonts.LatoItalic;
			t.fontSize = fonts.numberSize;
		} else if (Type == FontVariables.TextType.Description) {
			t.font = fonts.LatoItalic;
			t.fontSize = fonts.descriptionSize;
		} else if (Type == FontVariables.TextType.Notice) {
			t.font = fonts.GaramondItalic;
			t.fontSize = fonts.noticeSize;
		}

		SetupColor ( );
	}

	void SetupColor ( )
	{
		var fonts = GameObject.FindGameObjectWithTag ("fontManager").GetComponent<FontVariables> ();
		Text t = GetComponent<Text> ();

		if (fontColor == FontVariables.TextColor.Default) {
	
			if (Type == FontVariables.TextType.Button || Type == FontVariables.TextType.Title || Type == FontVariables.TextType.Numbers) {
				if (isWhite) {
					t.color = fonts.Black;
				} else {
					t.color = fonts.white;
				}
			} else {
				if (isWhite) {
					t.color = fonts.subBlack;
				} else {
					t.color = fonts.subWhite;
				}
			}
		} else if (fontColor == FontVariables.TextColor.Blue) {
			t.color = Utilities.Blue;
		} else if (fontColor == FontVariables.TextColor.Golden) {
			t.color = Utilities.Orange;
		} else {
			t.color =fonts.Red;
		}

	}

	public void SetupFontBlackBG()
	{
		SetupFont (false);
		isWhite = false;
	}
	public void SetupFontWhiteBG()
	{
		SetupFont (true);
		isWhite = true;
	}


	public FontVariables.TextType Type;
	public FontVariables.TextColor fontColor;

}


