using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class PlayRandomSound : MonoBehaviour {

	public AudioClip[] sounds;
	AudioSource AS;
	void Awake()
	{
		AS = GetComponent<AudioSource> ();
	}
	void OnEnable () {
		AS.PlayOneShot(sounds[Random.Range(0,sounds.Length)]);
	}

}
