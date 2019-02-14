using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Raincrow.Maps;
using TMPro;

public class GardenMarkers : MonoBehaviour
{
	public static GardenMarkers Instance{ get; set;}
	public GameObject gardenPrefab;
	public float scale = 10;
	public GameObject gardenCanvas;
	public TextMeshProUGUI title;
	public Image img;
	public TextMeshProUGUI desc;

	void Awake()
	{
		Instance = this;
	}

	public void CreateGardens()
	{
		foreach (var item in PlayerDataManager.config.gardens) {
			var pos = new Vector2 (item.longitude, item.latitude);
            IMarker marker = MapsAPI.Instance.AddMarker(pos, gardenPrefab);
			marker.scale = scale;
			marker.SetRange(3, 12);
			marker.customData = item.id;
			marker.OnClick = OnClick;
            marker.instance.GetComponent<MarkerScaleManager>().m = marker;
		}
	}
	public void OnClick(IMarker m)
	{
		img.gameObject.SetActive (false);
		gardenCanvas.SetActive (true);
		title.text = DownloadedAssets.gardenDict [m.customData as string].title;
		desc.text = DownloadedAssets.gardenDict [m.customData as string].description;
		StartCoroutine (GetImage (m.customData as string));
	}

	IEnumerator	GetImage(string id){
		WWW www = new WWW (DownloadAssetBundle.baseURL + "gardens/" + id + ".png");
		yield return www;
		img.sprite =Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
		img.gameObject.SetActive (true);
	}
}

