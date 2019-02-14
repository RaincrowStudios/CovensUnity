using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class witchSchoolData : MonoBehaviour
{

	public Image thumbnail;
	public TextMeshProUGUI title;
	public Button button;
	public TextMeshProUGUI desc;
	string id;
	public void Setup(LocalizeData data){
		id = data.id;
		title.text = data.title.ToUpper();
		desc.text = data.description;
		button.onClick.AddListener (playVideo);
	}

	void Start()
	{
		StartCoroutine (getPic (id));
	}

	IEnumerator getPic (string id){
		WWW www = new WWW (DownloadAssetBundle.baseURL + "witch-school/thumb/" + id + ".png");
		yield return www;
		thumbnail.sprite =Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
	}

	void playVideo()
	{
		WitchSchoolManager.Instance.playVideo (DownloadAssetBundle.baseURL + "witch-school/videos/" + id + ".mp4",title.text);
	}

}

