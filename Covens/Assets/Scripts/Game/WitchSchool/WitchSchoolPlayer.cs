using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WitchSchoolPlayer : MonoBehaviour {

	public string url;

	public MediaPlayerCtrl player;
	public CanvasGroup play;
	public CanvasGroup pause;
	public Button togglePlayButton;
	bool isPlaying = false;
	public Slider sound;
	void Start()
	{
		play.alpha= 1;
		pause.alpha = 0;

		togglePlayButton.onClick.AddListener (TogglePlay);
		string k= "myname";
	
	}

	public void SetSound()
	{
		player.SetVolume (sound.value);
	}

	public void TogglePlay()
	{
		togglePlayButton.image.color = new Color (1, 1, 1, 0);
		isPlaying = !isPlaying;
		if (isPlaying) {
			play.alpha= 1;

			player.Play ();
			StartCoroutine (FadeOut (play));
		} else {
			pause.alpha= 1;
			player.Pause ();
			StartCoroutine (FadeOut (pause));
		}
	}

	IEnumerator FadeOut (CanvasGroup cg)
	{
		float t = 0;
		while (t <= 1) {
			t += Time.deltaTime;
			cg.alpha = Mathf.SmoothStep(1,0,t);
			yield return 0;
		}
	}
}
