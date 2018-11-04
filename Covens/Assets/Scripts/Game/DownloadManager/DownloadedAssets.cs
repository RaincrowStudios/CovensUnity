using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine.UI;

public class DownloadedAssets : MonoBehaviour
{
	public static DownloadedAssets Instance { get; set;}
	public static Dictionary<string,SpiritDict> spiritDictData = new Dictionary<string, SpiritDict>();
	public static Dictionary<string,SpellDict> spellDictData = new Dictionary<string, SpellDict>();
	public static Dictionary<string,ConditionDict> conditionsDictData = new Dictionary<string, ConditionDict> ();
	public static Dictionary<string,IngredientDict> ingredientDictData = new Dictionary<string, IngredientDict> ();
	public static Dictionary<string,StoreDictData> storeDict = new Dictionary<string, StoreDictData> (); 
	public static Dictionary<string,LocalizeData> questsDict = new Dictionary<string, LocalizeData> (); 
	public static Dictionary<string,LocalizeData> countryCodesDict = new Dictionary<string, LocalizeData> (); 
	public static List<LocalizeData> tips = new List<LocalizeData> ();
	public static Dictionary<string,LocalizeData> spiritTypeDict = new Dictionary<string,LocalizeData> ();



	static Dictionary<string, Sprite> AllSprites = new Dictionary<string, Sprite> (); 
	static Dictionary<string, Sprite> IconSprites = new Dictionary<string, Sprite> (); 
	 
	public static Dictionary<string,List< string>> assetBundleDirectory = new Dictionary<string, List<string>> ();

	static Dictionary<string,List<AssetBundle>> loadedBundles = new Dictionary<string,List<AssetBundle>> ();

	public SpriteRenderer sp;
	public Sprite s;
	public static Sprite s1;

	void Awake()
	{
		Instance = this;
		DontDestroyOnLoad (this.gameObject);
	}

	#region SpriteGetters
	public static void GetSprite(string id, SpriteRenderer spr, bool isIcon = false){
		if (!isIcon && AllSprites.ContainsKey (id)) {
			spr.sprite = AllSprites [id];
		} else if (isIcon && IconSprites.ContainsKey (id)) {
			spr.sprite = IconSprites [id]; 
		} else {
		Timing.RunCoroutine (getSpiritHelper (id, spr, isIcon)); 
		}
	}

	public static void GetSprite(string id, Image spr, bool isIcon = false){

		if (!isIcon && AllSprites.ContainsKey (id)) { 
			spr.sprite = AllSprites [id];
		
		} else if (isIcon && IconSprites.ContainsKey (id)) { 
			spr.sprite = IconSprites [id]; 
		
		} else {
		Timing.RunCoroutine (getSpiritHelper (id, spr, isIcon));
		}
	}
	#endregion


	static IEnumerator<float> getSpiritHelper(string id, SpriteRenderer spr, bool isIcon){ 
		string type = "";
		if(id.Contains("spirit"))
			type =  "spirit";
		else if(id.Contains("spell"))
			type = "spell";
		else if(!isIcon)
			type = "apparel";
		else if(isIcon)
			type = "icon";
			

	
		if (id.Contains (type)) {
			if (!loadedBundles.ContainsKey (type)) {
				loadedBundles [type] = new List<AssetBundle> (); 
				foreach (var item in assetBundleDirectory[type]) {
					var bundleRequest = AssetBundle.LoadFromFileAsync (item);
					Timing.WaitUntilDone (bundleRequest);
					loadedBundles[type].Add (bundleRequest.assetBundle);
				}
			}

			foreach (var item in loadedBundles[type]) {

				if (item.Contains (id + ".png")) {
					var request = item.LoadAssetAsync(id+".png",typeof(Sprite));
					Timing.WaitUntilDone( request);
					var tempSp = request.asset as Sprite;
					spr.sprite = tempSp;
					if(isIcon)
					IconSprites [tempSp.texture.name] = tempSp;
					else
					AllSprites [tempSp.texture.name] = tempSp;
				}
			}
		}
		yield return Timing.WaitForOneFrame;
	}

	static IEnumerator<float> getSpiritHelper(string id, Image spr, bool isIcon){ 

		string type = "";
		if(id.Contains("spirit"))
			type =  "spirit";
		else if(id.Contains("spell"))
			type = "spell";
		else if(!isIcon)
			type = "apparel";
		else if(isIcon)
			type = "icon";

			if (!loadedBundles.ContainsKey (type)) {
				loadedBundles [type] = new List<AssetBundle> (); 
				foreach (var item in assetBundleDirectory[type]) {
					var bundleRequest = AssetBundle.LoadFromFileAsync (item);
					Timing.WaitUntilDone (bundleRequest);
					loadedBundles [type].Add (bundleRequest.assetBundle);
				}
			} else {
			
			}

		if (type == "spell")
			id = spellDictData [id].spellGlyph.ToString ();
		
			foreach (var item in loadedBundles[type]) {
				if (item.Contains (id + ".png")) {
					var request = item.LoadAssetAsync (id + ".png", typeof(Sprite));
					Timing.WaitUntilDone (request);
					var tempSp = request.asset as Sprite;
					spr.sprite = tempSp;
					if (isIcon)
						IconSprites [tempSp.texture.name] = tempSp;
					else
						AllSprites [tempSp.texture.name] = tempSp;
				} 
			}

		yield return Timing.WaitForOneFrame;

	}
} 





