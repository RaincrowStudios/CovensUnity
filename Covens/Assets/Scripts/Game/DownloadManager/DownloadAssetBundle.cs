using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;
using UnityEngine.UI;

public class DownloadAssetBundle : MonoBehaviour {

	public static DownloadAssetBundle Instance { get; set;}

	public Text downloadingTitle;
	public Text downloadingInfo;
	public Slider slider;

	string baseURL = "https://storage.googleapis.com/raincrow-covens/";
	bool isDownload = false;

	List<string> existingBundles = new List<string>();
	List<string> downloadableAssets = new List<string> ();
	int TotalAssets = 0;
	private bool isDictLoaded = false;
	private bool isAssetBundleLoaded = false;
	enum AssetType
	{
		spirit
	}

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		isDictLoaded = false; 
		isAssetBundleLoaded = false;
		StartCoroutine (InitiateLogin ());
		if (PlayerPrefs.GetString ("AssetCacheJson") != "") {
			var cache = JsonConvert.DeserializeObject<AssetCacheJson> (PlayerPrefs.GetString ("AssetCacheJson"));
			existingBundles = cache.bundles;
		}
		DownloadAsset (new List<string>(){"spirit-1","spells-1","wardrobe-1","icons-1"});
		StartCoroutine (AnimateDownloadingText ());
		StartCoroutine (GetDictionaryMatrix ());
	}

	IEnumerator GetDictionaryMatrix (int version = 0)
	{
			using (UnityWebRequest www = UnityWebRequest.Get (baseURL + "Dictionary3.json")) {
			yield return www.SendWebRequest ();
				if (www.isNetworkError || www.isHttpError) {
					Debug.Log (www.error);
					Debug.Log ("Couldnt Load the Dictionary");
				} else {
//				print (www.downloadHandler.text);
				string tempPath = Path.Combine(Application.persistentDataPath, "dict.text");
				File.WriteAllText (tempPath, www.downloadHandler.text);
				try{
					string text = System.IO.File.ReadAllText(tempPath);
					print(text);
					var data = JsonConvert.DeserializeObject<DictMatrixData> (text);
					print("done");
					SaveDict(data);
				}catch(Exception e) {
					Debug.LogError (e);
				}
				}
			}
	}

	public void SaveDict(DictMatrixData data)
	{
		try{
			foreach (var item in data.Spells) {
				DownloadedAssets.spellDictData.Add (item.spellID, item);
			}
			foreach (var item in data.Spirits) {
				DownloadedAssets.spiritDictData.Add (item.spiritID, item);
			}
			foreach (var item in data.Conditions) {
				DownloadedAssets.conditionsDictData.Add (item.conditionID, item);
			}
			foreach (var item in data.Collectibles) {
				DownloadedAssets.ingredientDictData.Add (item.id, item); 
			}
			isDictLoaded = true;
		}catch (Exception e){
			Debug.LogError (e);
		}

	}

	IEnumerator AnimateDownloadingText ()
	{
		float delay = .5f;
		downloadingTitle.text = "Downloading";
		yield return new WaitForSeconds (delay);
		downloadingTitle.text = "Downloading .";
		yield return new WaitForSeconds (delay);
		downloadingTitle.text = "Downloading . .";
		yield return new WaitForSeconds (delay);
		downloadingTitle.text = "Downloading . . .";
		StartCoroutine (AnimateDownloadingText ());
	}

	public void DownloadAsset(List<string> assetKeys)
	{
		foreach (var item in assetKeys) {
			if (!existingBundles.Contains (item)) {
				TotalAssets++;
				downloadableAssets.Add (item);
			} else {
				if (item.Contains ("spirit")) {
					LoadAsset (item);
				} else if (item.Contains ("spell")) {
					LoadAsset (item);
				} else if (item.Contains ("preview")) {
					LoadAsset (item);
				} else if (item.Contains ("icons")) {
					LoadAsset (item);
				}
			}
		}

		if (downloadableAssets.Count > 0) {
			DownloadAssetHelper (0);
		} else {
		//	slider.transform.parent.gameObject.SetActive (false);
			isAssetBundleLoaded = true;
		}
	}

	IEnumerator InitiateLogin()
	{
		yield return new WaitUntil (() => isAssetBundleLoaded == true);
		yield return new WaitUntil (() => isDictLoaded == true);
		yield return new WaitForSeconds (1);
		slider.transform.parent.gameObject.SetActive (false);
		LoginUIManager.Instance.AutoLogin ();
	}

	void DownloadAssetHelper(int i)
	{
		//		if (downloadableAssets [i].Contains ("spirits"))
		StartCoroutine (StartDownload (AssetType.spirit, downloadableAssets [i],i));
	}

	IEnumerator StartDownload(AssetType asset, string assetKey,int i) {

		string url = baseURL + assetKey;
		UnityWebRequest webRequest = UnityWebRequest.Head(url);
		webRequest.Send();
		while (!webRequest.isDone)
		{
			yield return null;
		}
		float size = float.Parse (webRequest.GetResponseHeader ("Content-Length")) * 0.000001f;
		downloadingInfo.text = "Assets " + (i+1).ToString () + " out of " + TotalAssets.ToString () + " (" + size.ToString ("F2") + "MB)";

		using (UnityWebRequest request = UnityWebRequest.Get (url)) {
			print ("Pulling assets from : " + url);
			isDownload = true;
			StartCoroutine (Progress (request));
			yield return request.SendWebRequest ();
			isDownload = false;
			if (request.isNetworkError || request.isHttpError) {
				Debug.Log ("Couldn't reach the servers!");
			} else {
				print ("Bundle Downloaded");
				i++;
				string tempPath = Path.Combine(Application.persistentDataPath,assetKey + ".unity3d");
				File.WriteAllBytes (tempPath, request.downloadHandler.data);
				print ("Asset Stored : " + tempPath);
				existingBundles.Add (assetKey);
				AssetCacheJson CacheJson = new AssetCacheJson{ bundles = existingBundles };
				PlayerPrefs.SetString ("AssetCacheJson", JsonConvert.SerializeObject (CacheJson));
				LoadAsset (assetKey);
			}
		}
		if (downloadableAssets.Count > i) {
			DownloadAssetHelper (i);
		} else {
		//	slider.transform.parent.gameObject.SetActive (false);
			isAssetBundleLoaded = true;
//			LoginUIManager.Instance.AutoLogin ();
		}
	}

	void LoadAsset(string assetKey)
	{
		print ("Loading : " + assetKey);
		var bundle = AssetBundle.LoadFromFile (Path.Combine (Application.persistentDataPath, assetKey+ ".unity3d"));
		if (bundle != null) {
			if (assetKey.Contains ("spirit")) {
				var spiritNew = new List<Sprite> ((Sprite[])bundle.LoadAllAssets<Sprite> ());
				foreach (var item in spiritNew) {
					DownloadedAssets.spiritArt.Add (item.texture.name, item);
				}
			} else if (assetKey.Contains ("spell")) {
				var spellNew = new List<Sprite> ((Sprite[])bundle.LoadAllAssets<Sprite> ()); 
				foreach (var item in spellNew) {
					DownloadedAssets.spellGlyphs.Add (int.Parse( item.texture.name), item);
				}
			} else if (assetKey.Contains ("wardrobe")) {
				var inventoryNew = new List<Sprite> ((Sprite[])bundle.LoadAllAssets<Sprite> ()); 
				foreach (var item in inventoryNew) {
					DownloadedAssets.wardobeArt.Add (item.texture.name, item); 
				}
			}else if (assetKey.Contains ("icons")) {
				var inventoryNew = new List<Sprite> ((Sprite[])bundle.LoadAllAssets<Sprite> ()); 
				print ("INVENTORY COUNT" +inventoryNew.Count); 
				foreach (var item in inventoryNew) {
					DownloadedAssets.wardobePreviewArt.Add (item.texture.name, item);
				}
			}
			bundle.Unload (false);
		}
	}

	IEnumerator Progress(UnityWebRequest req)
	{
		while (isDownload) {
			slider.value = req.downloadProgress; 
			yield return null;
		}
	}
}

#region json classes
public class AssetCacheJson{
	public List<string> bundles { get; set;}
}

public class ConditionDict
{
	public string conditionID { get; set; }
	public string spellID { get; set; }
	public string conditionDescription { get; set; }
}

public class SpellDict
{
	public string spellID { get; set; }
	public string spellName { get; set; }
	public int spellGlyph { get; set; }
	public string spellDescription { get; set; }
	public int spellSchool { get; set; }
}

public class SpiritDict
{
	public string spiritID { get; set; }
	public string spiritName { get; set; }
	public string spiritDescription { get; set; }
	public int spiritTier { get; set; }
	public string spiritLegend { get; set; }
	public string spiritTool { get; set; }
}

public class DictMatrixData
{
	public List<SpellDict> Spells { get; set; }
	public List<SpiritDict> Spirits { get; set; }
	public List<ConditionDict> Conditions { get; set; }
	public List<IngredientDict> Collectibles { get; set; }

}

public class IngredientDict
{
	public string id { get; set; }
	public string description { get; set; }
	public string hint { get; set; }
	public int rarity { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string spirit { get; set; }
}


#endregion