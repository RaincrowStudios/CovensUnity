using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class SoundManagerOneShot : MonoBehaviour
{
	public static SoundManagerOneShot Instance{ get;set;}

	public AudioClip whisper;
	public float whisperSound;
	public AudioClip itemAdded;
	public float itemAddedSound;
	public AudioClip Error;
	public float errorSound;
	public AudioClip buttonTap;
	public float buttonTapSound;

	AudioSource AS;
	void Awake()
	{
		Instance = this;
		AS = GetComponent<AudioSource> ();
	}

	public void PlayWhisper()
	{
		if (AS.isPlaying) {
			AS.Stop ();
		}
		AS.volume = whisperSound;
		AS.PlayOneShot (whisper);
	}

	public void PlayItemAdded()
	{
		if (AS.isPlaying) {
			AS.Stop ();
		}
		AS.volume = itemAddedSound;

		AS.PlayOneShot (itemAdded);
	}

	public void PlayError()
	{
		if (AS.isPlaying) {
			AS.Stop ();
		}
		AS.volume = errorSound;
		AS.PlayOneShot (Error);
	}

	public void PlayButtonTap()
	{
		if (AS.isPlaying) {
			AS.Stop ();
		}
		AS.volume = buttonTapSound;
		AS.PlayOneShot (buttonTap);
	}
}

