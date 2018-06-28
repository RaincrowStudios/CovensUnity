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
		if (PlayerPrefs.GetString ("AssetCacheJson") != "") {
			var cache = JsonConvert.DeserializeObject<AssetCacheJson> (PlayerPrefs.GetString ("AssetCacheJson"));
			existingBundles = cache.bundles;
		}
		DownloadAsset (new List<string>(){"spirits-2","spirits-3","spell-1","pop-2","pop-3"});
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
				}
			}
		}

		if (downloadableAssets.Count > 0) {
			DownloadAssetHelper (0);
		} else {
			slider.transform.parent.gameObject.SetActive (false);
		}
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
			slider.transform.parent.gameObject.SetActive (false);
		}
	}

	void LoadAsset(string assetKey)
	{
		var bundle = AssetBundle.LoadFromFile (Path.Combine (Application.persistentDataPath, assetKey+ ".unity3d"));
		if (bundle != null) {
			if (assetKey.Contains ("spirit")) {
				var spiritNew = new List<Sprite> ((Sprite[])bundle.LoadAllAssets<Sprite> ());
				foreach (var item in spiritNew) {
					DownloadedAssets.spiritArt.Add (item.texture.name, item);
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

public class AssetCacheJson{
	public List<string> bundles { get; set;}
}
