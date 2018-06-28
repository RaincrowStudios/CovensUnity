using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DownloadedAssets : MonoBehaviour
{
	public static DownloadedAssets Instance { get; set;}

	public static Dictionary<string,Sprite> spiritArt = new Dictionary<string,Sprite>();
	void Awake()
	{
		Instance = this;
	}
}

