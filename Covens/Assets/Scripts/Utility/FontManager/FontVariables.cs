using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class FontVariables : MonoBehaviour
{
	public Font LatoItalic;
	public Font GaramondItalic;
	public Font CinzelRegular;
	public Color white;
	public Color Black;
	public Color subWhite;
	public Color subBlack;
	public Color Red;

	public enum TextType{
		Title,Button,Description,Notice,Subtitle,Numbers
	}

	public enum TextColor{
		Default,Golden,Blue,Red
	}

	public int titleSize = 70;
	public int subtitleSize = 60;
	public int buttonSize = 60;
	public int descriptionSize = 55;
	public int noticeSize = 55;
	public int numberSize = 65;

	public void UpdateAllText()
	{
		var g = Resources.FindObjectsOfTypeAll<FontManager> ();
		foreach (var item in g) {
			item.SetupFontBlackBG ();
		}
	}

}

