using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;

public class CastingSound : MonoBehaviour {

	bool delay = false;

	public AudioMixerSnapshot[] BG;
	public AudioMixerSnapshot[] castSounds; 
	public List<AudioSource> AS = new List<AudioSource>();  
	 float castingSoundTranstion = .6f;
	 float castingSoundTranstionOut =3f;
	
	 float BgSoundTransition = 1f;
	public GameObject CastingSoundsContainer;


	void OnEnable () {
		if (AS.Count == 6) {
			foreach (AudioSource a in AS) {
				a.Stop ();
			}
		}

		delay = false;
		CastingSoundsContainer.SetActive (true);
		if (AS.Count != 6) {
			AS.Clear ();

			foreach (Transform t in CastingSoundsContainer.transform) {
				foreach (Transform a in t) {
					AS.Add (a.GetComponent<AudioSource> ());
				}
			}
		}
		BG[1].TransitionTo(.5f);
		Invoke ("delayOn", .1f);
	}

	void OnDisable()
	{
		PlayerManager.Instance.TransitionToBG (BG [0]);
		PlayerManager.Instance.TransitionToBG (castSounds[0]);
	}

	void delayOn()
	{
		delay = true;
	}

	void Update () {

		if (!delay)
			return;

		if(Input.GetMouseButtonDown(0) )
		{
			foreach (AudioSource a in AS) {
				a.Play ();
			}
			BG[1].TransitionTo(.3f);

		}
		if(Input.GetMouseButtonUp(0))
		{
//			BG[0].TransitionTo(BgSoundTransition*4);
//			castSounds[0].TransitionTo(3);
	
		}
		if(Input.GetMouseButton(0))
		{

	
			if(Input.mousePosition.y <(.475f*Screen.height))
			{
				castSounds[3].TransitionTo(.3f);
			}

			if(Input.mousePosition.y >(.475f*Screen.height) && Input.mousePosition.y <(.525f*Screen.height))
			{
				castSounds[2].TransitionTo(.3f);
			}

			if(Input.mousePosition.y >(.525f*Screen.height))
			{
				castSounds[1].TransitionTo(.3f);
			}
		}	

	}

}
