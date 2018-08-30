using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;

public class CastingSound : MonoBehaviour {

	bool delay = false;

	public AudioMixerSnapshot[] BG;
	public AudioMixerSnapshot[] castSounds; 
	public List<AudioSource> AS = new List<AudioSource>();  
	public float castingSoundTranstion = .6f;
	public float bgSoundOut = 2.5f;
	 float castingSoundTranstionOut =3f;
	public shiftaudio[] SH;

	 float BgSoundTransition = 1f;
	public GameObject CastingSoundsContainer;


	void OnEnable () {
		SH [0].enabled = true;
		SH [1].enabled = true;
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
		BG[1].TransitionTo(bgSoundOut);
	}

	void OnDisable()
	{
		SH [0].enabled = false;
		SH [1].enabled = false;
		BG [0].TransitionTo (bgSoundOut);
		castSounds [0].TransitionTo (5f);

	}

	public void TransitionToBG(AudioMixerSnapshot AS)
	{
		AS.TransitionTo (1.5f);
	}

	void Update () {


		if(Input.GetMouseButtonDown(0) )
		{
			foreach (AudioSource a in AS) {
				a.Play ();
			}
			BG[1].TransitionTo(1.3f);

		}

		if(Input.GetMouseButton(0))
		{

	
			if(Input.mousePosition.y <(.475f*Screen.height))
			{
				castSounds[1].TransitionTo(castingSoundTranstion);
			}

			if(Input.mousePosition.y >(.475f*Screen.height) && Input.mousePosition.y <(.595f*Screen.height))
			{
				castSounds[2].TransitionTo(castingSoundTranstion);
			}

			if(Input.mousePosition.y >(.595f*Screen.height))
			{
				castSounds[3].TransitionTo(castingSoundTranstion);
			}
		}	

	}

}
