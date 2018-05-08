using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class DeathState : MonoBehaviour {

	public static DeathState Instance { get; set;}

	public Transform[] ScaleDownObjects;
	public Transform[] ScaleUpObjects;
	public CanvasGroup[] FadeButtons;
	public CanvasGroup[] DisableItems;
	PostProcessingProfile UIcamProfile;
	PostProcessingProfile mainCamProfile;
	public Camera UICamera;
	public Camera MainCamera;
	public float speed =1;
	public GameObject Particles;
	public GameObject DeathContainer;
	public GameObject FlightGlowFX;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		mainCamProfile = MainCamera.GetComponent<PostProcessingBehaviour> ().profile;
		UIcamProfile =  UICamera.GetComponent<PostProcessingBehaviour> ().profile;
	}

	public void ShowDeath()
	{
		FlightGlowFX.SetActive (false);
		Particles.SetActive (true);
		DeathContainer.SetActive (true);
		StartCoroutine (BeginDeathState ());
	}

	public void HideDeath()
	{
		Particles.SetActive (false);
		FlightGlowFX.SetActive (true);
		DeathContainer.GetComponent<Fade> ().FadeOutHelper ();
		StartCoroutine (EndDeathState ());
	}

	IEnumerator EndDeathState()
	{
		float t = 1;
		while (t>=0) {
			t -= Time.deltaTime * speed;
			ManageState (t);
			yield return null;
		}	
	}

	IEnumerator BeginDeathState()
	{
		float t = 0;
		while (t<=1) {
			t += Time.deltaTime * speed;
			ManageState (t);
			yield return null;
		}			
	}

	void ManageState(float t)
	{
		print (t);
		var UIsettings = UIcamProfile.colorGrading.settings;
		var mainCamSettings = mainCamProfile.colorGrading.settings;
		UIsettings.basic.contrast = Mathf.SmoothStep (1, 1.3f,t);
		UIsettings.basic.saturation = Mathf.SmoothStep (1,0,t);

		mainCamSettings.basic.saturation = Mathf.SmoothStep (1,0,t);
		print (mainCamSettings.basic.saturation + " sat");

		mainCamSettings.basic.contrast = Mathf.SmoothStep (1,2,t);

		UIcamProfile.colorGrading.settings = UIsettings;
		mainCamProfile.colorGrading.settings = mainCamSettings;


		foreach (var item in FadeButtons) {
			item.alpha = Mathf.SmoothStep (1,.4f, t);
		}

		foreach (var item in DisableItems) {
			item.alpha = Mathf.SmoothStep (1, 0, t);
		}
		foreach (var item in ScaleDownObjects) {
			item.localScale = Vector3.one * Mathf.SmoothStep (1, 0, t);
		}

		foreach (var item in ScaleUpObjects) {
			item.localScale = Vector3.one * Mathf.SmoothStep (0, 1, t);
		}
	}
}
