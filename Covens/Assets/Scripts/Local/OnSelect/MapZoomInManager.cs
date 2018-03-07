using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class MapZoomInManager : MonoBehaviour {

	public static MapZoomInManager Instance {get; set;}
	PostProcessingProfile prof;
	public float BlurDelay = 1;
	public float blurOutSpeed = 1;
	public float blurSpeed = 1;

	OnlineMapsTileSetControl mapControl;
	OnlineMaps map;
	public float moveOutSpeed = 1;
	public float moveSpeed = 1;
	Vector2 curPos;
	public CanvasGroup MainUICanvasGroup;
	public GameObject SpellUI;
 	CanvasGroup SpellUICanvasGroup;

	public GameObject ArenaParticleFx;

	public  GameObject DistortObject;

	Renderer distort;

	public Light spotLight;

	public GameObject selectedObjectPRefab;

	OnlineMapsMarker3D selectedMarker;
	OnlineMapsControlBase3D control;

	Transform selectObjectTrans;

	public float markerSize = 10;
	void Awake()
	{
		Instance = this;		
	}

	void Start()
	{
		prof = Camera.main.GetComponent<PostProcessingBehaviour> ().profile;
		distort = DistortObject.GetComponent<Renderer> ();

		var dof = prof.depthOfField.settings;
		var desat = prof.colorGrading.settings;
		dof.aperture = .6f;
		desat.basic.saturation = 1;
		prof.depthOfField.settings = dof;
		prof.colorGrading.settings = desat;

		control = OnlineMapsControlBase3D.instance;
		mapControl= OnlineMapsTileSetControl.instance;  
		map = OnlineMaps.instance;
		SpellUICanvasGroup = SpellUI.GetComponent<CanvasGroup> ();
		EventManager.OnNPCDataReceived += SetupNPCInfo;
	}

	void SetupNPCInfo()
	{
		OnTapNPCUI.Instance.ShowInfo (MarkerSpawner.SelectedMarker);
	}

	public void OnSelect (Vector2 pos) {
		OnPlayerSelect.isPlayer = false;
		SpellUI.SetActive (true);
		SpellSelectParent.Instance.SetupSpellCast ();
		DistortObject.SetActive (true);
		selectedMarker = control.AddMarker3D (pos, selectedObjectPRefab);
		selectObjectTrans = selectedMarker.instance.transform.GetChild (0);
		ArenaParticleFx.SetActive (false);
		SpellUICanvasGroup.alpha = 0;

		Transform ring = PlayerManager.AttackRing.transform.GetChild(0);
		foreach (Transform item in ring) {
			item.gameObject.SetActive (false);
		}
		Utilities.allowMapControl (false, true);
		curPos = map.position;
		StartCoroutine (PersepectiveZoomIn (pos));
	} 

	IEnumerator Blur(float delay,float speed, bool isBlur = true)
	{
		yield return new WaitForSeconds (delay);

		var dof = prof.depthOfField.settings;
		var desat = prof.colorGrading.settings;
		float t = 0;
		while (t <= 1f) {
			t += Time.deltaTime*speed;
			if (isBlur) {
				dof.aperture = Mathf.SmoothStep (.6f, .1f, t);
				desat.basic.saturation = Mathf.SmoothStep (1, 0, t);
			} else {
				dof.aperture = Mathf.SmoothStep (.1f, .6f, t);
				desat.basic.saturation = Mathf.SmoothStep (0, 1.0f, t);
			}
			prof.depthOfField.settings = dof;
			prof.colorGrading.settings = desat;
			yield return null;
		}
	}

	IEnumerator PersepectiveZoomIn(Vector2 focusPos)
	{

		float t = 0;
		while (t <= 1f) {
			distort.material.SetFloat ("_DistortionStrength",Mathf.SmoothStep (0, -.4f, t));
			distort.material.SetFloat ("_NormalTexStrength",Mathf.SmoothStep (0, .035f, t));
			t += Time.deltaTime * moveSpeed;
			selectObjectTrans.localScale =  Vector3.Lerp (Vector3.zero, new Vector3 (156f,156, 156f), Mathf.SmoothStep (0, 1f, Mathf.SmoothStep (0, 1f, t)));
			selectObjectTrans.localEulerAngles = Vector3.Lerp (new Vector3 (90, 0, -60), new Vector3 (90, 0, 30), Mathf.SmoothStep (0, 1f, t));
			map.position = Vector2.Lerp (curPos, focusPos, Mathf.SmoothStep (0, 1f, t));
			mapControl.cameraRotation = Vector2.Lerp (Vector2.zero, new Vector2 (50.8f, -30f), Mathf.SmoothStep (0, 1f, Mathf.SmoothStep (0, 1f, t)));
			mapControl.cameraDistance = Mathf.SmoothStep (550f, 483.2f, Mathf.SmoothStep (0, 1f, t));
			MainUICanvasGroup.alpha = Mathf.SmoothStep (1f, 0f, t);
			SpellUICanvasGroup.alpha = t;
			RenderSettings.ambientLight = Color.Lerp (new Color (.875f, .875f, .875f), new Color (.05f, .05f, .05f), Mathf.SmoothStep (0, 1f, t));
			spotLight.intensity = Mathf.SmoothStep (0, 12.2f, t);
			yield return null;
		}

		StartCoroutine (Blur (BlurDelay, blurSpeed, true));

	}


	public void Back()
	{
		StopAllCoroutines ();
		StartCoroutine (PersepectiveZoomOut ());
		StartCoroutine (Blur (0, blurOutSpeed, false));
	}

	IEnumerator PersepectiveZoomOut( )
	{
		float t = 1;
		while (t >= 0f) {
			t -= Time.deltaTime * moveOutSpeed;
			distort.material.SetFloat ("_DistortionStrength",Mathf.SmoothStep (0, -.4f, t));
			distort.material.SetFloat ("_NormalTexStrength",Mathf.SmoothStep (0, .035f, t));
			selectObjectTrans.localScale =  Vector3.Lerp (Vector3.zero, new Vector3 (156f,156, 156f), Mathf.SmoothStep (0, 1f, Mathf.SmoothStep (0, 1f, t)));
			selectObjectTrans.localEulerAngles = Vector3.Lerp (new Vector3 (90, 0, -60), new Vector3 (90, 0, 30), Mathf.SmoothStep (0, 1f, t));
			mapControl.cameraRotation = Vector2.Lerp (Vector2.zero, new Vector2 (50.8f, -30f), Mathf.SmoothStep (0, 1f, t));
			mapControl.cameraDistance = Mathf.SmoothStep (550f, 483.2f, t);
			MainUICanvasGroup.alpha = Mathf.SmoothStep (1f, 0f, t);
			SpellUICanvasGroup.alpha = t;
			RenderSettings.ambientLight = Color.Lerp (new Color (.875f, .875f, .875f), new Color (.05f, .05f, .05f), Mathf.SmoothStep (0, 1f, t));
			spotLight.intensity = Mathf.SmoothStep (0, 12.2f, t);
			yield return null;
		}
		selectedMarker.control.RemoveMarker3D (selectedMarker);
		ArenaParticleFx.SetActive (true);
		Transform ring = PlayerManager.AttackRing.transform.GetChild(0);
		foreach (Transform item in ring) {
			item.gameObject.SetActive (true);
		}
		Utilities.allowMapControl (true);
		SpellUI.SetActive (false);
		DistortObject.SetActive (false);
	}
		
}
