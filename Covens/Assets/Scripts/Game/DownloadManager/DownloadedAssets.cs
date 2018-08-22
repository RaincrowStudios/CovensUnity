using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DownloadedAssets : MonoBehaviour
{
	public static DownloadedAssets Instance { get; set;}

	public static Dictionary<string,Sprite> spiritArt = new Dictionary<string,Sprite>();
	public static Dictionary<int,Sprite> spellGlyphs = new Dictionary<int,Sprite>();
	public static Dictionary<string,SpiritDict> spiritDictData = new Dictionary<string, SpiritDict>();
	public static Dictionary<string,SpellDict> spellDictData = new Dictionary<string, SpellDict>();
	public static Dictionary<string,ConditionDict> conditionsDictData = new Dictionary<string, ConditionDict> ();
	public static Dictionary<string,IngredientDict> ingredientDictData = new Dictionary<string, IngredientDict> ();
	void Awake()
	{
		Instance = this;
	}
	public static Sprite getGlyph(string id)
	{
		var data = spellDictData [id].spellGlyph;
		return spellGlyphs [data];
	}
} 

