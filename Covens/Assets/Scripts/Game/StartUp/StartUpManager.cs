using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
public class StartUpManager : MonoBehaviour {
	
	public static StartUpManager Instance {get; set;}

	public GameObject LoadingImage;
	public GameObject StatScreen;
	public GameObject continueButton;
	public GameObject VideoPlayback;
	public float splashTime = 9f;
	public Image progressBar;
	public CanvasGroup[] logos;
	public float logoTime;
	public float fadeTime;
	int i = 0;
	public GameObject HintObject;
	public Text hintText;
	public GameObject splash;
	public static Config config;
	public Text tribunalTimer;
	public Text tribunalTitle;

	public Text currentDominion;
	public Text strongestWitch;
	public Text strongestCoven;
	bool hasTriedLogin = false;

	public Text tip;
	public Image spirit;
	public Text spiritName;
	public RectTransform Hint;
	AsyncOperation SceneAO;

	public GameObject hint2;
	public Text tip2;
	public Image spirit2;
	public Text spiritName2;

	public GameObject ServerDown;


	public GameObject OutdatedBuild;

	void Awake(){
		Instance = this;
	}

	public void Init () {
		StartCoroutine (FadeIn (0));
		VideoPlayback.SetActive (false);
		continueButton.SetActive (false);
		StatScreen.SetActive (false);
		LoadingImage.SetActive (false);

	}

	IEnumerator FadeIn (int i)
	{
		if(!Application.isEditor){
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime*fadeTime;
			logos [i].alpha  = Mathf.SmoothStep (0,1, t);
			yield return null;
		}

		yield return new WaitForSeconds (logoTime);

		while (t >= 0f) {
			t -= Time.deltaTime*fadeTime;
			logos [i].alpha  = Mathf.SmoothStep (0,1, t);
			yield return null;
		}
		i++;

		if (i < logos.Length) {
			StartCoroutine (FadeIn (i));
		} else {
			VideoPlayback.SetActive (true);

			StartCoroutine (ShowHint ());
		}
	}else{
			StartCoroutine (ShowHint ());
			yield return null;
	}
	}

	public void OutDatedBuild()
	{
		OutdatedBuild.SetActive (true);
		this.StopAllCoroutines ();
	}

	IEnumerator ShowHint ()
	{
		if (!Application.isEditor)
			yield return new WaitForSeconds (splashTime);
		else {
			logos [0].alpha = 0;
		}
		VideoPlayback.SetActive (false);
		HintObject.SetActive (true);
		yield return new WaitUntil (() => DownloadAssetBundle.isDictLoaded == true);
		tip.text = DownloadedAssets.tips [Random.Range (0, DownloadedAssets.tips.Count)].id;


			var k =  DownloadedAssets.spiritDictData.ElementAt (Random.Range( 0, DownloadedAssets.spiritDictData.Count));
			//spirit.sprite = k.Value;
		spiritName.text = k.Value.spiritName;
		WWW www = new WWW (DownloadAssetBundle.baseURL + "spirits/" + k.Key + ".png");
		yield return www;
		spirit.sprite =Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));

		yield return new WaitUntil (() => DownloadAssetBundle.isAssetBundleLoaded == true);

		Hint.anchoredPosition = new Vector2 (0, -92);
		Hint.localScale = Vector3.one * 1.05f;

		tip2.text = tip.text;
		spirit2.sprite = spirit.sprite;
		spiritName2.text = spiritName.text;
		LoginAPIManager.AutoLogin ();

	}

	public void ShowTribunalTimer ()
	{

		if (config.tribunal == 1) {
			tribunalTitle.text = "THE SUMMER TOURNAMENT OF WITCHCRAFT";
		} else if (config.tribunal == 2) {
			tribunalTitle.text = "THE SPRING TOURNAMENT OF WITCHCRAFT";
		} else if (config.tribunal == 3) {
			tribunalTitle.text = "THE AUTUMN TOURNAMENT OF WITCHCRAFT";
		} else {
			tribunalTitle.text = "THE WINTER TOURNAMENT OF WITCHCRAFT";
		}

		tribunalTimer.text = config.daysRemaining.ToString();
		currentDominion.text = "You are in the dominion of " + config.dominion;
		strongestWitch.text = "The Strongest witch in this dominion is " + config.strongestWitch;
		strongestCoven.text = "The Strongest coven is " + config.strongestCoven;
		HintObject.SetActive (false);
		splash.SetActive (true);
	
		Invoke ("EnableStats", 3f);

	}

	public void DoSceneLoading()
	{
		StartCoroutine (LoadMainScene());
		DownloadAssetBundle.Instance.DownloadUI.SetActive (false);
		LoadingImage.SetActive (true);
		hasTriedLogin = true;
	}

	void EnableStats()
	{
		splash.GetComponent<Fade> ().FadeOutHelper ();
		StatScreen.SetActive (true);
		LoadingImage.SetActive (true);
		hasTriedLogin = true;
		StartCoroutine (LoadMainScene());
		Invoke ("ShowHint2", 5);
	}

	void ShowHint2()
	{
		hint2.SetActive (true);
	}

	IEnumerator LoadMainScene()
	{
		yield return new WaitForSeconds (splashTime+1);
		 SceneAO = SceneManager.LoadSceneAsync ("MainScene");
		SceneAO.allowSceneActivation = false;
		while (!SceneAO.isDone) {
			progressBar.fillAmount = SceneAO.progress;
			if (SceneAO.progress >= .9f) {
				progressBar.fillAmount = 1;
				continueButton.SetActive (true);
				LoadingImage.SetActive (false);
			}
			yield return null;
		}
	
	}

	void StartDelayedLoop()
	{
		StartCoroutine (DelayedLoop ());
	}

	IEnumerator DelayedLoop()
	{
	
		for (int i = 0; i < 5; i++) {
			//your stuff
			yield return new WaitForSeconds (1);  // delay in seconds
		}

	}

	public void ContinueToMain()
	{
		SceneAO.allowSceneActivation = true;
	}

	public void UpdateApp()
	{
		Application.OpenURL ("https://play.google.com/store/apps/details?id=com.raincrow.covens");
	}
}
