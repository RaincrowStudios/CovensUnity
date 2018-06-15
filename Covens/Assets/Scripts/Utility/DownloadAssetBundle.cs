using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadAssetBundle : MonoBehaviour {
	public List<Sprite> sprites = new List<Sprite>();
	IEnumerator Start() {
		WWW download;
		string url = "https://storage.googleapis.com/raincrow-covens/tokens";

		while (!Caching.ready)
			yield return null;

		download = WWW.LoadFromCacheOrDownload(url, 0);

		yield return download;

		AssetBundle assetBundle = download.assetBundle;
		if (assetBundle != null) {
			print ("got the bundle");
			// Alternatively you can also load an asset by name (assetBundle.Load("my asset name"))
			sprites = new List<Sprite>((Sprite[])assetBundle.LoadAllAssets<Sprite>());
//			if (go != null)
//			else
//				Debug.Log("Couldn't load resource");    
		} else {
//			Debug.Log("Couldn't load resource");    
		}
	}
}
