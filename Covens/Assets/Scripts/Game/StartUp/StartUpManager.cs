using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

	public GameObject splash;

	AsyncOperation SceneAO;
	void Awake(){
		Instance = this;
	}

	void Start () {
		StartCoroutine (FadeIn (0));
		VideoPlayback.SetActive (false);
		continueButton.SetActive (false);
		StatScreen.SetActive (false);
		LoadingImage.SetActive (false);
	}

	IEnumerator FadeIn (int i)
	{
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

			Invoke ("EnableSplash", splashTime);
		}
	}


	void EnableSplash()
	{
		splash.SetActive (true);
		VideoPlayback.SetActive (false);
		Invoke ("EnableStats", 5);

	}

	void EnableStats()
	{
		splash.GetComponent<Fade> ().FadeOutHelper ();
		StatScreen.SetActive (true);
		LoadingImage.SetActive (true);
		StartCoroutine (LoadMainScene());
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
}
