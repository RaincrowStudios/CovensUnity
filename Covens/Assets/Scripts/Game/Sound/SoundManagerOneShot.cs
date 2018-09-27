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


	public AudioClip LevelChange;
	public AudioClip[] WhiteAlign;
	public AudioClip[] ShadowAlign;
	public float statsChangeSound;

	public AudioClip Spirit;

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

	public void PlayLevel()
	{
		if (AS.isPlaying) {
			AS.Stop ();
		}
		AS.volume = statsChangeSound;
		AS.PlayOneShot (LevelChange);
	}

	public void PlayShadow()
	{
		if (AS.isPlaying) {
			AS.Stop ();
		}
		AS.volume = statsChangeSound;
		AS.PlayOneShot (ShadowAlign[Random.Range(0,ShadowAlign.Length)]);
	}

	public void PlayWhite()
	{
		if (AS.isPlaying) {
			AS.Stop ();
		}
		AS.volume = statsChangeSound;
		AS.PlayOneShot (ShadowAlign[Random.Range(0,ShadowAlign.Length)]);
	}

	public void SpiritSummon()
	{
		print ("Playing SPIRIT!");
		if (AS.isPlaying) {
			AS.Stop ();
		}
		AS.volume = statsChangeSound;
		AS.PlayOneShot (Spirit);
	}
}

