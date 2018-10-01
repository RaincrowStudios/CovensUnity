using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class DeathState : MonoBehaviour {

	public static DeathState Instance { get; set;}

	public Transform[] ScaleDownObjects;
	public Transform[] ScaleUpObjects;
	public CanvasGroup[] FadeButtons;
	public GameObject[] DisableItems;
	PostProcessingProfile UIcamProfile;
	PostProcessingProfile mainCamProfile;
	public Camera UICamera;
	public Camera MainCamera;
	public float speed =1;
//	public GameObject Particles;
	public GameObject DeathContainer;
	public GameObject FlightGlowFX;
	bool isDead = false;
	public GameObject DeathPersist;
	public GameObject flyDead;
	public GameObject mapDarkBox;
	public Button[] turnOffInteraction;
	

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		mainCamProfile = MainCamera.GetComponent<PostProcessingBehaviour> ().profile;
		UIcamProfile =  UICamera.GetComponent<PostProcessingBehaviour> ().profile;
//		if (LoginAPIManager.loggedIn) {
//			if (PlayerDataManager.playerData.energy == 0) {
//				DeathState.Instance.ShowDeath ();
//			}
//		}
	}

	void OnEnable()
	{
		if (isDead) {
			StartCoroutine (BeginDeathState ());
			isDead = false;
		}
	}

	public void ShowDeath()
	{
		flyDead.SetActive (true);
		foreach (var item in turnOffInteraction) {
			item.interactable = false;
		}
		DeathPersist.SetActive (true);
		mapDarkBox.SetActive (true);
		if (MapSelection.currentView == CurrentView.MapView) {
			if (!PlayerManager.Instance.fly)
				PlayerManager.Instance.Fly ();
			FlightGlowFX.SetActive (false);
//		Particles.SetActive (true);
			DeathContainer.SetActive (true);
			if (gameObject.activeInHierarchy)
				StartCoroutine (BeginDeathState ());
			else
				isDead = true;
			MainCamera.GetComponent<PostProcessingBehaviour> ().enabled = true;
			UICamera.GetComponent<PostProcessingBehaviour> ().enabled = true;
			Utilities.allowMapControl (false);
			Invoke ("HideDeath", 5f);
			PlayerManagerUI.Instance.ShowElixirVulnerable (true);
		}
	}

	public void Revived()
	{
		flyDead.SetActive (false);
		mapDarkBox.SetActive (false);

		foreach (var item in turnOffInteraction) {
			item.interactable = true;
		}
		DeathPersist.SetActive (false);
		PlayerManagerUI.Instance.Revived ();
	}

	 void HideDeath()
	{
//		Particles.SetActive (false);
		FlightGlowFX.SetActive (true);
		DeathContainer.GetComponent<Fade> ().FadeOutHelper ();
		StartCoroutine (EndDeathState ());
		Utilities.allowMapControl (true);
	}

	IEnumerator EndDeathState()
	{
		float t = 1;
		while (t>=0) {
			t -= Time.deltaTime * speed;
			ManageState (t);
			yield return null;
		}	
		MainCamera.GetComponent<PostProcessingBehaviour> ().enabled = false;
		UICamera.GetComponent<PostProcessingBehaviour> ().enabled = false;
	}

	IEnumerator BeginDeathState()
	{
		foreach (var item in DisableItems) {
			item.SetActive (false);
		}
		float t = 0;
		while (t<=1) {
			t += Time.deltaTime * speed;
			ManageState (t);
			yield return null;
		}			
	}

	void ManageState(float t)
	{
		if (UIcamProfile != null) {
			var UIsettings = UIcamProfile.colorGrading.settings;
			UIsettings.basic.contrast = Mathf.SmoothStep (1, 1.3f, t);
			UIsettings.basic.saturation = Mathf.SmoothStep (1, 0, t);
			UIcamProfile.colorGrading.settings = UIsettings;
		}
		var mainCamSettings = mainCamProfile.colorGrading.settings;


		mainCamSettings.basic.saturation = Mathf.SmoothStep (1,0,t);

		mainCamSettings.basic.contrast = Mathf.SmoothStep (1,2,t);

		mainCamProfile.colorGrading.settings = mainCamSettings;


		foreach (var item in FadeButtons) {
			item.alpha = Mathf.SmoothStep (1,.4f, t);
		}


		foreach (var item in ScaleDownObjects) {
			item.localScale = Vector3.one * Mathf.SmoothStep (1, 0, t);
		}

		foreach (var item in ScaleUpObjects) {
			item.localScale = Vector3.one * Mathf.SmoothStep (0, 1, t);
		}
	}
}
